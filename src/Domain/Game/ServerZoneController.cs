using System;
using System.Collections.Generic;
using System.Linq;
using EntityNetwork;

namespace Domain
{
    public class ServerZoneController : ZoneControllerServerBase, IZoneControllerServerHandler
    {
        private readonly ServerSnake[] _snakes = new ServerSnake[2];

        public Action<ServerZoneController, ZoneState> StateChanged;

        private void SetState(ZoneState state)
        {
            if (Data.State == state)
                return;

            Data.State = state;
            StateChanged?.Invoke(this, state);
        }

        public void Start(int clientId1, int clientId2, bool useAi1, bool useAi2)
        {
            if (Data.State != ZoneState.None)
                throw new InvalidOperationException("State should be None. but " + Data.State);

            SetState(ZoneState.Ready);
            SpawnSnakes(clientId1, clientId2, useAi1, useAi2);

            SetTimerOnce(1, TimeSpan.FromSeconds(1), (e, t) =>
            {
                if (Data.State != ZoneState.Ready)
                    return;

                foreach (var snake in _snakes)
                    snake.MakePlaying();

                SetState(ZoneState.Playing);
                SetTimerOnce(2, TimeSpan.FromSeconds(1), OnFruitSpawnTimer);
            });
        }

        public void Stop()
        {
            if (Data.State != ZoneState.Playing)
                return;

            SetState(ZoneState.Stopped);
            foreach (var snake in _snakes)
                snake.MakeDead();
        }

        private void SpawnSnakes(int clientId1, int clientId2, bool useAi1, bool useAi2)
        {
            var x1 = Rule.BoardWidth / 2;
            var x2 = Rule.BoardWidth / 2 + 1;
            var y1 = Rule.BoardHeight / 4;
            var y2 = Rule.BoardHeight * 3 / 4;

            _snakes[0] = (ServerSnake)Zone.Spawn(
                typeof(ISnake), clientId1, EntityFlags.Normal,
                new SnakeSnapshot
                {
                    PlayerId = 1,
                    Parts = new List<Tuple<int, int>>
                    {
                        Tuple.Create(x1, y1),
                        Tuple.Create(x2, y1)
                    },
                    UseAi = useAi1,
                });

            _snakes[1] = (ServerSnake)Zone.Spawn(
                typeof(ISnake), clientId2, EntityFlags.Normal,
                new SnakeSnapshot
                {
                    PlayerId = 2,
                    Parts = new List<Tuple<int, int>>
                    {
                        Tuple.Create(x2, y2),
                        Tuple.Create(x1, y2)
                    },
                    UseAi = useAi2,
                });
        }

        public void OnSnakeDead(ServerSnake snake)
        {
            if (Data.State != ZoneState.Playing)
                return;

            if (snake == _snakes[0])
                Data.WinnerId = _snakes[1].Id;
            if (snake == _snakes[1])
                Data.WinnerId = _snakes[0].Id;

            SetState(ZoneState.Stopped);

            if (snake == _snakes[0])
                _snakes[1].MakeDead();
            if (snake == _snakes[1])
                _snakes[0].MakeDead();
        }

        private void SpawnFruit()
        {
            var rnd = new Random();

            var snakes = Zone.GetEntities(typeof(ISnake)).Select(e => (ServerSnake)e).ToArray();
            while (true)
            {
                var x = rnd.Next(Rule.BoardWidth);
                var y = rnd.Next(Rule.BoardHeight);

                if (snakes.All(s => s.Parts.All(p => p.Item1 != x || p.Item2 != y)))
                {
                    Zone.Spawn(typeof(IFruit), 0, EntityFlags.Normal, Tuple.Create(x, y));
                    return;
                }
            }
        }

        public void OnFruitDespawn(ServerFruit fruit)
        {
            SetTimerOnce(2, TimeSpan.FromSeconds(2), OnFruitSpawnTimer);
        }

        private void OnFruitSpawnTimer(IEntity entity, int timerId)
        {
            if (Data.State != ZoneState.Playing)
                return;

            SpawnFruit();
        }
    }
}
