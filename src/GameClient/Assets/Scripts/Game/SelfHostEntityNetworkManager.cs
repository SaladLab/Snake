using System;
using Domain;
using EntityNetwork;
using ProtoBuf.Meta;
using TypeAlias;
using System.Linq;
using UnityEngine;

public class SelfHostEntityNetworkManager : EntityNetworkManager
{
    public bool UseAi;

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
            var controller = zone.GetEntity<ServerZoneController>();
            if (controller == null)
                controller = (ServerZoneController)zone.Spawn(typeof(IZoneController), 0);

            if (controller.Data.State == ZoneState.None)
            {
                if (UseAi)
                {
                    controller.Start(clientId, clientId, false, true);
                }
                else
                {
                    var clientIds = zone.GetClientIds().ToList();
                    if (clientIds.Count == 2)
                        controller.Start(clientIds[0], clientIds[1], false, false); 
                }
            }
        });
    }

    private void OnGUI()
    {
        if (isNetworkActive == false)
            UseAi = GUI.Toggle(new Rect(230, 40, 200, 30), UseAi, "Use AI");
    }
}
