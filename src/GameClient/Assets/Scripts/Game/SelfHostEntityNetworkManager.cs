using Domain;
using ProtoBuf.Meta;
using TypeAlias;

public class SelfHostEntityNetworkManager : EntityNetworkManager
{
    protected override TypeAliasTable GetTypeAliasTable()
    {
        return new TypeAliasTable();
    }

    protected override TypeModel GetTypeModel()
    {
        return new DomainProtobufSerializer();
    }

    protected override void OnClientAdd(int clientId)
    {
        Zone.RunAction(zone =>
        {
            var controller = (ServerZoneController)zone.Spawn(typeof(IZoneController), 0);
            controller.Start(clientId, clientId);
        });
    }
}
