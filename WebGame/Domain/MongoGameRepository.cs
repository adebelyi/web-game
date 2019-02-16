using System;
using MongoDB.Driver;

namespace WebGame.Domain
{
    // TODO Сделате по аналогии с MongoUserRepository
    public class MongoGameRepository : IGameRepository
    {
        private readonly IMongoCollection<GameEntity> collection;
        public const string CollectionName = "games";

        public MongoGameRepository(IMongoDatabase db)
        {
            collection = db.GetCollection<GameEntity>(CollectionName);
        }

        public GameEntity Insert(GameEntity game)
        {
            collection.InsertOne(game);
            return game;
        }

        public GameEntity FindById(Guid gameId)
        {
            return collection.Find(g => g.Id == gameId).SingleOrDefault();
        }

        public void Update(GameEntity game)
        {
            collection.ReplaceOne(g => g.Id == game.Id, game);
        }
    }
}