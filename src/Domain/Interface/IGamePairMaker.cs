using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Interfaced;

namespace Domain
{
    public interface IGamePairMaker : IInterfacedActor
    {
        Task RegisterPairing(long userId, string userName, GameDifficulty difficulty, IUserPairingObserver observer);
        Task UnregisterPairing(long userId);
    }
}
