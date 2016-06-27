using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Utility;
using Akka.Interfaced;
using Akka.Interfaced.LogFilter;
using Akka.Interfaced.SlimServer;
using Common.Logging;
using Domain;

namespace GameServer
{
    [Log]
    [ResponsiveException(typeof(ResultException))]
    public class UserActor : InterfacedActor, IUser
    {
        private ILog _logger;
        private ClusterNodeContext _clusterContext;
        private ActorBoundChannelRef _channel;
        private long _id;
        private TrackableUserContext _userContext;
        private IUserEventObserver _userEventObserver;
        private Dictionary<long, GameRef> _joinedGameMap;

        public UserActor(ClusterNodeContext clusterContext, ActorBoundChannelRef channel,
                         long id, TrackableUserContext userContext, IUserEventObserver observer)
        {
            _logger = LogManager.GetLogger($"UserActor({id})");
            _clusterContext = clusterContext;
            _channel = channel;
            _id = id;
            _userContext = userContext;
            _userEventObserver = observer;
            _joinedGameMap = new Dictionary<long, GameRef>();
        }

        protected override void PostStop()
        {
            UnlinkAll();
            base.PostStop();
        }

        private void UnlinkAll()
        {
            foreach (var game in _joinedGameMap.Values)
                game.WithNoReply().Leave(_id);
            _joinedGameMap.Clear();
        }

        Task IUser.RegisterPairing(GameDifficulty difficulty, IUserPairingObserver observer)
        {
            return _clusterContext.GamePairMaker.RegisterPairing(_id, _userContext.Data.Name, difficulty, observer);
        }

        Task IUser.UnregisterPairing()
        {
            return _clusterContext.GamePairMaker.UnregisterPairing(_id);
        }

        async Task<Tuple<IGameClient, int, GameInfo>> IUser.JoinGame(long gameId, IGameObserver observer)
        {
            if (_joinedGameMap.ContainsKey(gameId))
                throw new ResultException(ResultCodeType.NeedToBeOutOfGame);

            // Try to get game ref

            var reply = await _clusterContext.GameTable.Ask<DistributedActorTableMessage<long>.GetOrCreateReply>(
                new DistributedActorTableMessage<long>.GetOrCreate(gameId, null));
            if (reply.Actor == null)
                throw new ResultException(ResultCodeType.GameNotFound);

            var game = reply.Actor.Cast<GameRef>().WithRequestWaiter(this);

            // Let's enter the game !

            var joinRet = await game.Join(_id, _userContext.Data.Name, observer);

            // Bind an game actor to channel

            var boundActor = await _channel.BindActor(game.CastToIActorRef(),
                                                  new[] { new TaggedType(typeof(IGameClient), joinRet.Item1) });
            if (boundActor == null)
            {
                await game.Leave(_id);
                _logger.Error($"Failed in binding GameClient");
                throw new ResultException(ResultCodeType.InternalError);
            }

            _joinedGameMap[gameId] = game;
            return Tuple.Create((IGameClient)boundActor.Cast<GameClientRef>(), joinRet.Item1, joinRet.Item2);
        }

        async Task IUser.LeaveGame(long gameId)
        {
            GameRef game;
            if (_joinedGameMap.TryGetValue(gameId, out game) == false)
                throw new ResultException(ResultCodeType.NeedToBeInGame);

            // Let's exit from the game !

            await game.Leave(_id);

            // Unbind an player actor with client session

            _channel.WithNoReply().UnbindActor(game.CastToIActorRef());
            _joinedGameMap.Remove(gameId);
        }

        private void FlushUserContext()
        {
            // Notify changes to Client
            _userEventObserver.UserContextChange(_userContext.Tracker);

            // Notify change to MongoDB
            MongoDbStorage.UserContextMapper.SaveAsync(MongoDbStorage.Instance.UserCollection,
                                                       _userContext.Tracker, _id);

            // Clear changes
            _userContext.Tracker = new TrackableUserContextTracker();
        }
    }
}
