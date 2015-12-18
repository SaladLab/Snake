using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityNetwork;
using ProtoBuf;
using TrackableData;
using TypeAlias;

namespace Domain
{
    [TypeAlias]
    public interface ISnake : IEntityPrototype
    {
        SnakeSnapshot Snapshot { get; }
        ISnakeData Data { get; }

        void Move(int x, int y);

        [ToClient]
        void GrowUp(int length);
    }

    public enum SnakeState
    {
        None = 0,
        Ready = 1,
        Playing = 2,
        Stopped = 3,
    }

    [ProtoContract]
    public interface ISnakeData : ITrackablePoco<ISnakeData>
    {
        [ProtoMember(1)] SnakeState State { get; set; }
        [ProtoMember(2)] int Score { get; set; }
    }

    [ProtoContract, TypeAlias]
    public class SnakeSnapshot
    {
        [ProtoMember(1)] public List<Tuple<int, int>> Parts;
        [ProtoMember(2)] public bool UseAi;
    }
}
