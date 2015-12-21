using System.Collections.Generic;
using EntityNetwork;
using ProtoBuf.Meta;
using TypeAlias;
using UnityEngine;
using UnityEngine.Networking;

public abstract class EntityNetworkManager : NetworkManager
{
    public static EntityNetworkManager Instance;
    public static TypeAliasTable TypeAliasTable;
    public static TypeModel TypeModel;

    private ServerZone _zone;
    private Dictionary<int, ProtobufChannelToServerZoneInbound> _zoneChannelMap;

    public ServerZone Zone { get { return _zone; } }

    protected abstract TypeAliasTable GetTypeAliasTable();
    protected abstract TypeModel GetTypeModel();

    void Awake()
    {
        Instance = this;
        TypeAliasTable = GetTypeAliasTable();
        TypeModel = GetTypeModel();
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

        OnZoneStart(_zone);
    }

    public override void OnStopServer()
    {
        Debug.Log("EntityNetworkManager.OnStopServer");

        if (_zone != null)
        {
            OnZoneStart(_zone);

            _zone = null;
            _zoneChannelMap = null;
        }
    }

    public bool AddClientToZone(int clientId, EntityNetworkClient networkClient)
    {
        var channelDown = new ProtobufChannelToClientZoneOutbound()
        {
            OutboundChannel = new EntityNetworkChannelToClientZone {NetworkClient = networkClient},
            TypeTable = TypeAliasTable,
            TypeModel = TypeModel,
        };
        if (_zone.AddClient(clientId, channelDown) == false)
            return false;

        var channelUp = new ProtobufChannelToServerZoneInbound
        {
            TypeTable = TypeAliasTable,
            TypeModel = TypeModel,
            ClientId = clientId,
            InboundServerZone = _zone
        };
        _zoneChannelMap.Add(clientId, channelUp);

        OnClientAdd(clientId);
        return true;
    }

    public void RemoveClientToZone(int id)
    {
        if (_zone == null)
            return;

        var ok = _zone.RemoveClient(id);
        _zoneChannelMap.Remove(id);

        if (ok)
            OnClientRemove(id);
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

    protected virtual void OnZoneStart(IServerZone zone)
    {
    }

    protected virtual void OnZoneStop(IServerZone zone)
    {
    }

    protected virtual void OnClientAdd(int clientId)
    {
    }

    protected virtual void OnClientRemove(int clientId)
    {
    }

    protected virtual void OnEntitySpawn(IServerEntity entity)
    {
        Debug.LogFormat("OnEntitySpawn({0})", entity.Id);
    }

    protected virtual void OnEntityDespawn(IServerEntity entity)
    {
        Debug.LogFormat("OnEntityDespawn({0})", entity.Id);
    }

    protected virtual void OnEntityInvalidTargetInvoke(int clientId, int entityId, IInvokePayload payload)
    {
        Debug.LogWarningFormat("OnEntityInvalidTargetInvoke({0}, {1}, {2})", clientId, entityId, payload.GetType().Name);
    }

    protected virtual void OnEntityInvalidOwnershipInvoke(int clientId, IServerEntity entity, IInvokePayload payload)
    {
        Debug.LogWarningFormat("OnEntityInvalidOwnershipInvoke({0}, {1}, {2}, {3})", clientId, entity.Id, entity.OwnerId, payload.GetType().Name);
    }
}
