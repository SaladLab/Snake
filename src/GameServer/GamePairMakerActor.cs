using System;
using System.Collections.Generic;
using System.Linq;
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
    public class GamePairMakerActor : InterfacedActor<GamePairMakerActor>, IExtendedInterface<IGamePairMaker>
    {
        private readonly ILog _logger = LogManager.GetLogger("GamePairMaker");
        private readonly ClusterNodeContext _clusterContext;
        private static readonly TimeSpan BotPairingTimeout = TimeSpan.FromSeconds(3);

        private class QueueEntity
        {
            public long UserId;
            public string UserName;
            public IUserPairingObserver Observer;
            public DateTime EnqueueTime;
        }

        // NOTE: If more performance required, lookup could be optimized further.
        private readonly List<QueueEntity>[] _pairingQueues;

        public GamePairMakerActor(ClusterNodeContext clusterContext)
        {
            _clusterContext = clusterContext;

            _clusterContext.ClusterActorDiscovery.Tell(
                new ClusterActorDiscoveryMessage.RegisterActor(Self, nameof(IGamePairMaker)),
                Self);

            _pairingQueues = new List<QueueEntity>[Enum.GetValues(typeof(GameDifficulty)).Length];
            for (var i = 0; i < _pairingQueues.Length; i++)
                _pairingQueues[i] = new List<QueueEntity>();
        }

        protected override Task OnPreStart()
        {
            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), Self, new Schedule(), null);
            return Task.FromResult(0);
        }

        private class Schedule
        {
        }

        [MessageHandler]
        private async Task OnSchedule(Schedule tick)
        {
            if (_clusterContext.GameTable == null)
                return;

            foreach (var pairingQueue in _pairingQueues)
                await SchedulePairing(pairingQueue);
        }

        private async Task SchedulePairing(List<QueueEntity> pairingQueue)
        {
            // Pairing for two users

            while (pairingQueue.Count >= 2)
            {
                var entry0 = pairingQueue[0];
                var entry1 = pairingQueue[1];

                pairingQueue.RemoveAt(0);
                pairingQueue.RemoveAt(0);

                long gameId;
                try
                {
                    var ret = await _clusterContext.GameTable.Ask<DistributedActorTableMessage<long>.CreateReply>(
                        new DistributedActorTableMessage<long>.Create(
                            new object[]
                            {
                                new CreateGameParam
                                {
                                    Difficulty = GameDifficulty.Normal,
                                    WithBot = false,
                                }
                            }));
                    gameId = ret.Id;
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("Failed to create game", e);
                    return;
                }

                entry0.Observer.MakePair(gameId, entry1.UserName);
                entry1.Observer.MakePair(gameId, entry0.UserName);
            }

            // Pairing an user with a bot

            if (pairingQueue.Count == 1)
            {
                var entry = pairingQueue[0];
                if ((DateTime.UtcNow - entry.EnqueueTime) > BotPairingTimeout)
                {
                    pairingQueue.RemoveAt(0);

                    long gameId;
                    try
                    {
                        var ret = await _clusterContext.GameTable.Ask<DistributedActorTableMessage<long>.CreateReply>(
                            new DistributedActorTableMessage<long>.Create(
                                new object[]
                                {
                                    new CreateGameParam
                                    {
                                        Difficulty = GameDifficulty.Normal,
                                        WithBot = true,
                                    }
                                }));
                        gameId = ret.Id;
                    }
                    catch (Exception e)
                    {
                        _logger.ErrorFormat("Failed to create game", e);
                        return;
                    }

                    entry.Observer.MakePair(gameId, null);
                }
            }
        }

        [ExtendedHandler]
        private void RegisterPairing(long userId, string userName, GameDifficulty difficulty, IUserPairingObserver observer)
        {
            if (_pairingQueues.Any(q => q.Any(i => i.UserId == userId)))
                throw new ResultException(ResultCodeType.AlreadyPairingRegistered);

            _pairingQueues[(int)difficulty].Add(new QueueEntity
            {
                UserId = userId,
                UserName = userName,
                Observer = observer,
                EnqueueTime = DateTime.UtcNow
            });
        }

        [ExtendedHandler]
        private void UnregisterPairing(long userId)
        {
            foreach (var pairingQueue in _pairingQueues)
                pairingQueue.RemoveAll(i => i.UserId == userId);
        }
    }
}
