using System;
using System.Collections.Generic;

namespace WebGame.Domain
{
    public interface ITurnsRepository
    {
        IReadOnlyList<GameTurnEntity> GetLastTurns(Guid gameId, int maxCount);
        GameTurnEntity Insert(GameTurnEntity entity);
    }
}