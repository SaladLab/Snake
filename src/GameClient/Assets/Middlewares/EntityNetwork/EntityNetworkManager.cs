using System.Collections.Generic;
using EntityNetwork;
using ProtoBuf.Meta;
using TypeAlias;
using UnityEngine;
using UnityEngine.Networking;

public abstract class EntityNetworkManager : NetworkManager
{
    public static EntityNetworkManager Instance;

    private ServerZone _zone;
    private Dictionary<int, ProtobufChannelToServerZoneInbound> _zoneChannelMap;

    public ServerZone Zone
    {
        get { return _zone; }
    }

    public abstract TypeAliasTable GetTypeAliasTable();
    public abstract TypeModel GetTypeModel();

    public virtual IServerEntityFactory GetServerEntityFactory()
    {
        return EntityFactory.Default;
    }

    public virtual IClientEntityFactory GetClientEntityFactory()
    {
        return ClientEntityFactory.Default;
    }

    private void Awake()
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

        OnZoneStart(_zone);
    }

    public override void OnStopServer()
    {
        Debug.Log("EntityNetworkManager.OnStopServer");

        if (_zone != null)
        {
            OnZoneStop(_zone);

            _zone = null;
            _zoneChannelMap = null;
        }
    }

    public override void OnStartClient(NetworkClient client)
    {
        Debug.Log("EntityNetworkManager.OnStartClient");
    }

    public override void OnStopClient()
    {
        Debug.Log("EntityNetworkManager.OnStopClient");

        // Remove all client entity objects

        var t = ClientEntityFactory.Default.RootTransform;
        if (t != null)
        {
            for (var i = t.childCount - 1; i >= 0; i--)
                Destroy(t.GetChild(i).gameObject);
        }
    }

    public bool AddClientToZone(int clientId, EntityNetworkClient networkClient)
    {
        var channelDown = new ProtobufChannelToClientZoneOutbound()
        {
            OutboundChannel = new EntityNetworkChannelToClientZone { NetworkClient = networkClient },
            TypeTable = GetTypeAliasTable(),
            TypeModel = GetTypeModel(),
        };
        if (_zone.AddClient(clientId, channelDown) == false)
            return false;

        var channelUp = new ProtobufChannelToServerZoneInbound
        {
            TypeTable = GetTypeAliasTable(),
            TypeModel = GetTypeModel(),
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
        if (_zone == null)
            return;

        _zone.RunAction(z => { ((EntityTimerProvider)z.TimerProvider).ProcessWork(); });
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
    }

    protected virtual void OnEntityDespawn(IServerEntity entity)
    {
    }

    protected virtual void OnEntityInvalidTargetInvoke(int clientId, int entityId, IInvokePayload payload)
    {
        Debug.LogWarningFormat("OnEntityInvalidTargetInvoke({0}, {1}, {2})", clientId, entityId, payload.GetType().Name);
    }

    protected virtual void OnEntityInvalidOwnershipInvoke(int clientId, IServerEntity entity, IInvokePayload payload)
    {
        Debug.LogWarningFormat("OnEntityInvalidOwnershipInvoke({0}, {1}, {2}, {3})", clientId, entity.Id, entity.OwnerId,
                               payload.GetType().Name);
    }
}
