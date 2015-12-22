using Domain;
using ProtoBuf.Meta;
using TypeAlias;

public class SelfHostEntityNetworkManager : EntityNetworkManager
{
    private static TypeAliasTable _typeAliasTable;
    private static TypeModel _typeModel;

    public override TypeAliasTable GetTypeAliasTable()
    {
        return _typeAliasTable ?? (_typeAliasTable = new TypeAliasTable());
    }

    public override TypeModel GetTypeModel()
    {
        return _typeModel ?? (_typeModel = new DomainProtobufSerializer());
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
