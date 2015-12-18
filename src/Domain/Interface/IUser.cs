using System;
using System.Threading.Tasks;
using Akka.Interfaced;

namespace Domain
{
    public interface IUser : IInterfacedActor
    {
        Task RegisterPairing(GameDifficulty difficulty, int observerId);
        Task UnregisterPairing();
        Task<Tuple<int, int, GameInfo>> JoinGame(long gameId, int observerId);
        Task LeaveGame(long gameId);
    }
}
