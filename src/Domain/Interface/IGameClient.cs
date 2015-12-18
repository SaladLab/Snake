using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Interfaced;

namespace Domain
{
    [TagOverridable("clientId")]
    public interface IGameClient : IInterfacedActor
    {
        Task ZoneMessage(byte[] bytes, int clientId = 0);
    }
}
