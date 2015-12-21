using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Interfaced;
using Akka.Interfaced.LogFilter;
using Akka.Interfaced.ProtobufSerializer;
using Common.Logging;
using Domain;
using EntityNetwork;
using ProtoBuf.Meta;
using TypeAlias;

namespace GameServer
{
    [Log]
    public class GameActor : InterfacedActor<GameActor>, IExtendedInterface<IGame, IGameClient>
    {
        private ILog _logger;
        private ClusterNodeContext _clusterContext;
        private long _id;
        private GameState _state;
        private CreateGameParam _param;

        private class Client : IByteChannel
        {
            public long UserId;
            public string UserName;
            public GameObserver Observer;
            public ProtobufChannelToClientZoneOutbound OutboundChannel;
            public ProtobufChannelToServerZoneInbound InboundChannel;

            void IByteChannel.Write(byte[] bytes)
            {
                Observer.ZoneMessage(bytes);
            }
        }

        private List<Client> _clients = new List<Client>();
        private ServerZone _zone;
        private ServerZoneController _zoneController;

        public GameActor(ClusterNodeContext clusterContext, long id, CreateGameParam param)
        {
            _logger = LogManager.GetLogger($"GameActor({id})");
            _clusterContext = clusterContext;
            _id = id;
            _param = param;
            _zone = new ServerZone(EntityFactory.Default);
            ((EntityTimerProvider)_zone.TimerProvider).ActionScheduled = OnActionSchedule;
        }

        private GameInfo GetGameInfo()
        {
            return new GameInfo
            {
                Id = _id,
                WithBot = _param.WithBot,
                State = _state,
                PlayerNames = _clients.Select(p => p.UserName).ToList(),
            };
        }

        private void NotifyToAllObservers(Action<int, GameObserver> notifyAction)
        {
            for (var i = 0; i < _clients.Count; i++)
            {
                if (_clients[i].Observer != null)
                    notifyAction(i + 1, _clients[i].Observer);
            }
        }

        private static Lazy<TypeAliasTable> _typeTable = new Lazy<TypeAliasTable>(() =>
        {
            var typeTable = new TypeAliasTable();
            return typeTable;
        });

        private static Lazy<TypeModel> _typeModel = new Lazy<TypeModel>(() =>
        {
            var typeModel = TypeModel.Create();
            AutoSurrogate.Register(typeModel);
            return typeModel;
        });

        private void OnActionSchedule(TimeSpan delay, Action action)
        {
            Context.System.Scheduler.ScheduleTellOnce(delay, Self, EntityTimerScheduleMessage.Instance, Self);
        }

        private class EntityTimerScheduleMessage
        {
            public static readonly EntityTimerScheduleMessage Instance = new EntityTimerScheduleMessage();
        }

        [MessageHandler]
        private void OnMessage(EntityTimerScheduleMessage m)
        {
            _zone.RunAction(z =>
            {
                ((EntityTimerProvider)z.TimerProvider).ProcessWork();
            });
        }

        [ExtendedHandler]
        private Tuple<int, GameInfo> Join(long userId, string userName, IGameObserver observer)
        {
            if (_state != GameState.Waiting)
                throw new ResultException(ResultCodeType.GameStarted);

            if (_clients.Count > 2)
                throw new ResultException(ResultCodeType.GamePlayerFull);

            var clientId = _clients.Count + 1;
            NotifyToAllObservers((id, o) => o.Join(userId, userName, clientId));

            var client = new Client
            {
                UserId = userId,
                UserName = userName,
                Observer = (GameObserver)observer,
            };

            client.OutboundChannel = new ProtobufChannelToClientZoneOutbound
            {
                TypeTable = _typeTable.Value,
                TypeModel = _typeModel.Value,
                OutboundChannel = client
            };

            client.InboundChannel = new ProtobufChannelToServerZoneInbound
            {
                TypeTable = _typeTable.Value,
                TypeModel = _typeModel.Value,
                ClientId = clientId,
                InboundServerZone = _zone
            };

            _clients.Add(client);
            _zone.AddClient(clientId, client.OutboundChannel);

            if ((_param.WithBot && _clients.Count == 1) || _clients.Count == 2)
                RunTask(() => BeginGame());

            return Tuple.Create(clientId, GetGameInfo());
        }

        private void BeginGame()
        {
            if (_state != GameState.Waiting)
                return;

            _state = GameState.Playing;

            NotifyToAllObservers((id, o) => o.Begin());

            _zone.RunAction(zone =>
            {
                _zoneController = (ServerZoneController)zone.Spawn(typeof(IZoneController), 0);
                _zoneController.Start(1, _clients.Count);
                _zoneController.StateChanged = OnZoneStateChange;
            });
        }

        private void OnZoneStateChange(ServerZoneController zoneController, ZoneState state)
        {
            if (state == ZoneState.Stopped)
            {
                if (_state == GameState.Playing)
                {
                    _state = GameState.Ended;
                    NotifyToAllObservers((id, o) => o.End(zoneController.Data.WinnerId));
                }
            }
        }

        private int GetClientId(long userId)
        {
            var index = _clients.FindIndex(p => p.UserId == userId);
            if (index == -1)
                throw new ResultException(ResultCodeType.NeedToBeInGame);
            return index + 1;
        }

        [ExtendedHandler]
        private void Leave(long userId)
        {
            var clientId = GetClientId(userId);

            var client = _clients[clientId - 1];
            _clients[clientId - 1].Observer = null;

            NotifyToAllObservers((id, o) => o.Leave(client.UserId));

            if (_state != GameState.Ended)
            {
                _zoneController?.Stop();
                _state = GameState.Aborted;
                NotifyToAllObservers((id, o) => o.Abort());
            }

            if (_clients.Count(p => p.Observer != null) == 0)
            {
                Self.Tell(InterfacedPoisonPill.Instance);
            }
        }

        [ExtendedHandler]
        private void ZoneMessage(byte[] bytes, int clientId = 0)
        {
            if (clientId < 1 || clientId > _clients.Count)
                throw new InvalidOperationException();

            var client = _clients[clientId - 1];
            _zone.RunAction(_ =>
            {
                client.InboundChannel.Write(bytes);
            });
        }
    }
}
