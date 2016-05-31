using System;
using System.Threading.Tasks;
using Akka.Interfaced;

namespace Domain
{
    public interface IUser : IInterfacedActor
    {
        Task RegisterPairing(GameDifficulty difficulty, IUserPairingObserver observer);
        Task UnregisterPairing();
        Task<Tuple<IGameClient, int, GameInfo>> JoinGame(long gameId, IGameObserver observer);
        Task LeaveGame(long gameId);
    }
}
