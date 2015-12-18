using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Utility;
using Akka.Interfaced;
using Akka.Interfaced.LogFilter;
using Common.Logging;
using Domain;

namespace GameServer
{
    [Log]
    public class UserActor : InterfacedActor<UserActor>, IUser
    {
        private ILog _logger;
        private ClusterNodeContext _clusterContext;
        private IActorRef _clientSession;
        private long _id;
        private TrackableUserContext _userContext;
        private UserEventObserver _userEventObserver;
        private Dictionary<long, GameRef> _joinedGameMap;

        public UserActor(ClusterNodeContext clusterContext, IActorRef clientSession,
                         long id, TrackableUserContext userContext, int observerId)
        {
            _logger = LogManager.GetLogger($"UserActor({id})");
            _clusterContext = clusterContext;
            _clientSession = clientSession;
            _id = id;
            _userContext = userContext;
            _userEventObserver = new UserEventObserver(clientSession, observerId);
            _joinedGameMap = new Dictionary<long, GameRef>();
        }

        private void UnlinkAll()
        {
            foreach (var game in _joinedGameMap.Values)
                game.WithNoReply().Leave(_id);
            _joinedGameMap.Clear();
        }

        [MessageHandler]
        protected void OnMessage(ActorBoundSessionMessage.SessionTerminated message)
        {
            UnlinkAll();
            Context.Stop(Self);
        }

        Task IUser.RegisterPairing(GameDifficulty difficulty, int observerId)
        {
            var observer = new UserPairingObserver(_clientSession, observerId);
            return _clusterContext.GamePairMaker.RegisterPairing(_id, _userContext.Data.Name, difficulty, observer);
        }

        Task IUser.UnregisterPairing()
        {
            return _clusterContext.GamePairMaker.UnregisterPairing(_id);
        }

        async Task<Tuple<int, int, GameInfo>> IUser.JoinGame(long gameId, int observerId)
        {
            if (_joinedGameMap.ContainsKey(gameId))
                throw new ResultException(ResultCodeType.NeedToBeOutOfGame);

            // Try to get game ref

            var reply = await _clusterContext.GameTable.Ask<DistributedActorTableMessage<long>.GetOrCreateReply>(
                new DistributedActorTableMessage<long>.GetOrCreate(gameId, null));
            if (reply.Actor == null)
                throw new ResultException(ResultCodeType.GameNotFound);

            var game = new GameRef(reply.Actor, this, null);

            // Let's enter the game !

            var observer = new GameObserver(_clientSession, observerId);

            var joinRet = await game.Join(_id, _userContext.Data.Name, observer);

            // Bind an player actor with client session

            var reply2 = await _clientSession.Ask<ActorBoundSessionMessage.BindReply>(
                new ActorBoundSessionMessage.Bind(game.Actor, typeof(IGameClient), joinRet.Item1));

            _joinedGameMap[gameId] = game;
            return Tuple.Create(reply2.ActorId, joinRet.Item1, joinRet.Item2);
        }

        async Task IUser.LeaveGame(long gameId)
        {
            GameRef game;
            if (_joinedGameMap.TryGetValue(gameId, out game) == false)
                throw new ResultException(ResultCodeType.NeedToBeInGame);

            // Let's exit from the game !

            await game.Leave(_id);

            // TODO: Remove observer when leave

            // Unbind an player actor with client session

            _clientSession.Tell(new ActorBoundSessionMessage.Unbind(game.Actor));
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
