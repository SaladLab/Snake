using System;
using ProtoBuf;
using System.Collections.Generic;

namespace Domain
{
    public enum GameState
    {
        Waiting,
        Playing,
        Ended,
        Aborted,
    }

    public enum GameDifficulty
    {
        Easy,
        Normal,
        Hard
    }

    [ProtoContract]
    public class GameInfo
    {
        [ProtoMember(1)] public long Id;
        [ProtoMember(2)] public bool WithBot;
        [ProtoMember(3)] public GameState State;
        [ProtoMember(4)] public List<string> PlayerNames;
    }
}
