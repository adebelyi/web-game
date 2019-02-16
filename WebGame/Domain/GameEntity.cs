using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace WebGame.Domain
{
    public class GameEntity
    {
        [BsonElement]
        private readonly List<Player> players;

        public GameEntity(int turnsCount)
            : this(Guid.Empty, GameStatus.WaitingToStart, turnsCount, 0, new List<Player>())
        {
        }

        [BsonConstructor]
        public GameEntity(Guid id, GameStatus status, int turnsCount, int currentTurnIndex, List<Player> players)
        {
            Id = id;
            Status = status;
            TurnsCount = turnsCount;
            CurrentTurnIndex = currentTurnIndex;
            this.players = players;
        }

        public Guid Id
        {
            get;
            // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local For MongoDB
            private set;
        }

        public IReadOnlyList<Player> Players => players.AsReadOnly();

        [BsonElement]
        public int TurnsCount { get; }

        public int CurrentTurnIndex { get; private set; }

        public GameStatus Status { get; private set; }

        public void AddPlayer(UserEntity user)
        {
            if (Status != GameStatus.WaitingToStart)
                throw new ArgumentException(Status.ToString());
            players.Add(new Player(user.Id, user.Login));
            if (Players.Count == 2)
                Status = GameStatus.Playing;
        }

        public bool IsFinished()
        {
            return CurrentTurnIndex >= TurnsCount;
        }

        public bool HaveDecisionOfEveryPlayer => Players.All(p => p.Decision != PlayerDecision.None);

        public void SetPlayerDecision(Guid userId, PlayerDecision decision)
        {
            if (Status != GameStatus.Playing)
                throw new ArgumentException(Status.ToString());
            foreach (var player in Players.Where(p => p.UserId == userId))
                player.Decision = decision;
        }

        public GameTurnEntity FinishTurn()
        {
            var winnerId = Guid.Empty;
            for (int i = 0; i < 2; i++)
            {
                var player = Players[i];
                var opponent = Players[1 - i];
                if (player.Decision.Beats(opponent.Decision))
                {
                    player.Score++;
                    winnerId = player.UserId;
                }
            }
            var result = new GameTurnEntity(Id, CurrentTurnIndex, Players.ToDictionary(p => p.UserId, p => p.Decision), winnerId);
            // Это должно быть после создания GameTurnEntity
            foreach (var player in Players)
                player.Decision = PlayerDecision.None;
            CurrentTurnIndex++;
            if (CurrentTurnIndex == TurnsCount)
                Status = GameStatus.Finished;
            return result;
        }
    }
}