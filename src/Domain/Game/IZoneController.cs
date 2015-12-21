using System;
using EntityNetwork;
using ProtoBuf;
using TrackableData;
using TypeAlias;

namespace Domain
{
    public enum ZoneState
    {
        None,
        Ready,
        Playing,
        Stopped,
    }

    [TypeAlias, Singleton]
    public interface IZoneController : IEntityPrototype
    {
        IZoneControllerData Data { get; }
    }

    [ProtoContract]
    public interface IZoneControllerData : ITrackablePoco<IZoneControllerData>
    {
        [ProtoMember(1)] ZoneState State { get; set; }
        [ProtoMember(2)] TimeSpan StartTime { get; set; }
        [ProtoMember(3)] int WinnerId { get; set; }
    }
}
