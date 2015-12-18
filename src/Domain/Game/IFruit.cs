using System;
using System.Collections.Generic;
using EntityNetwork;
using ProtoBuf;
using TrackableData;
using TypeAlias;

namespace Domain
{
    [TypeAlias]
    public interface IFruit : IEntityPrototype
    {
        FruitSnapshot Snapshot { get; }
    }

    [ProtoContract, TypeAlias]
    public class FruitSnapshot
    {
        [ProtoMember(1)] public Tuple<int, int> Pos;
    }
}
