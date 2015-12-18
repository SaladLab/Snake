using System;
using System.Collections.Generic;
using EntityNetwork;
using ProtoBuf.Meta;
using TypeAlias;
using UnityEngine;
using UnityEngine.Networking;

public class EntityNetworkManager : NetworkManager
{
    public static EntityNetworkManager Instance;

    public static TypeAliasTable TypeTable;
    public static TypeModel ProtobufTypeModel;

    private ServerZone _zone;
    private Dictionary<int, ProtobufChannelToServerZoneInbound> _zoneChannelMap;

    public ServerZone Zone { get { return _zone; } }

    void Awake()
    {
        Instance = this;
    }

    public override void OnStartServer()
    {
        Debug.Log("EntityNetworkManager.OnStartServer");

        _zone = new ServerZone(EntityFactory.Default);
        _zone.EntitySpawned = OnEntitySpawn;
        _zone.EntityDespawned = OnEntityDespawn;
        _zone.EntityInvalidTargetInvoked = OnEntityInvalidTargetInvoke;
        _zone.EntityInvalidOwnershipInvoked = OnEntityInvalidOwnershipInvoke;

        _zoneChannelMap = new Dictionary<int, ProtobufChannelToServerZoneInbound>();
    }

    public override void OnStopServer()
    {
        Debug.Log("EntityNetworkManager.OnStopServer");
        _zone = null;
        _zoneChannelMap = null;
    }

    public bool AddClientToZone(int clientId, EntityNetworkClient networkClient)
    {
        var channelDown = new ProtobufChannelToClientZoneOutbound()
        {
            OutboundChannel = new EntityNetworkChannelToClientZone {NetworkClient = networkClient},
            TypeTable = TypeTable,
            TypeModel = ProtobufTypeModel,
        };
        if (_zone.AddClient(clientId, channelDown) == false)
            return false;

        var channelUp = new ProtobufChannelToServerZoneInbound
        {
            TypeTable = TypeTable,
            TypeModel = ProtobufTypeModel,
            ClientId = clientId,
            InboundServerZone = _zone
        };
        _zoneChannelMap.Add(clientId, channelUp);
        return true;
    }

    public void RemoveClientToZone(int id)
    {
        if (_zone == null)
            return;

        _zone.RemoveClient(id);
        _zoneChannelMap.Remove(id);
    }

    public void WriteZoneChannel(int clientId, byte[] bytes)
    {
        ProtobufChannelToServerZoneInbound channel;
        if (_zoneChannelMap.TryGetValue(clientId, out channel))
        {
            _zone.BeginAction();
            channel.Write(bytes);
            _zone.EndAction();
        }
    }

    private void Update()
    {
        if (_zone != null)
        {
            _zone.RunAction(z =>
            {
                ((EntityTimerProvider)z.TimerProvider).ProcessWork();
            });
        }
    }

    private void OnEntitySpawn(IServerEntity entity)
    {
        Debug.LogFormat("OnEntitySpawn({0})", entity.Id);
    }

    private void OnEntityDespawn(IServerEntity entity)
    {
        Debug.LogFormat("OnEntityDespawn({0})", entity.Id);
    }

    private void OnEntityInvalidTargetInvoke(int clientId, int entityId, IInvokePayload payload)
    {
        Debug.LogWarningFormat("OnEntityInvalidTargetInvoke({0}, {1}, {2})", clientId, entityId, payload.GetType().Name);
    }

    private void OnEntityInvalidOwnershipInvoke(int clientId, IServerEntity entity, IInvokePayload payload)
    {
        Debug.LogWarningFormat("OnEntityInvalidOwnershipInvoke({0}, {1}, {2}, {3})", clientId, entity.Id, entity.OwnerId, payload.GetType().Name);
    }
}
