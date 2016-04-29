﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Akka.Interfaced CodeGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Akka.Interfaced;
using Akka.Actor;
using ProtoBuf;
using TypeAlias;
using System.ComponentModel;

#region Domain.IGame

namespace Domain
{
    [PayloadTableForInterfacedActor(typeof(IGame))]
    public static class IGame_PayloadTable
    {
        public static Type[,] GetPayloadTypes()
        {
            return new Type[,] {
                { typeof(Join_Invoke), typeof(Join_Return) },
                { typeof(Leave_Invoke), null },
            };
        }

        [ProtoContract, TypeAlias]
        public class Join_Invoke
            : IInterfacedPayload, IAsyncInvokable
        {
            [ProtoMember(1)] public System.Int64 userId;
            [ProtoMember(2)] public System.String userName;
            [ProtoMember(3)] public Domain.GameObserver observer;
            public Type GetInterfaceType() { return typeof(IGame); }
            public async Task<IValueGetable> InvokeAsync(object target)
            {
                var __v = await ((IGame)target).Join(userId, userName, observer);
                return (IValueGetable)(new Join_Return { v = (System.Tuple<System.Int32, Domain.GameInfo>)__v });
            }
        }

        [ProtoContract, TypeAlias]
        public class Join_Return
            : IInterfacedPayload, IValueGetable
        {
            [ProtoMember(1)] public System.Tuple<System.Int32, Domain.GameInfo> v;
            public Type GetInterfaceType() { return typeof(IGame); }
            public object Value { get { return v; } }
        }

        [ProtoContract, TypeAlias]
        public class Leave_Invoke
            : IInterfacedPayload, IAsyncInvokable
        {
            [ProtoMember(1)] public System.Int64 userId;
            public Type GetInterfaceType() { return typeof(IGame); }
            public async Task<IValueGetable> InvokeAsync(object target)
            {
                await ((IGame)target).Leave(userId);
                return null;
            }
        }
    }

    public interface IGame_NoReply
    {
        void Join(System.Int64 userId, System.String userName, Domain.IGameObserver observer);
        void Leave(System.Int64 userId);
    }

    [ProtoContract, TypeAlias]
    public class GameRef : InterfacedActorRef, IGame, IGame_NoReply
    {
        [ProtoMember(1)] private ActorRefBase _actor
        {
            get { return (ActorRefBase)Actor; }
            set { Actor = value; }
        }

        private GameRef() : base(null)
        {
        }

        public GameRef(IActorRef actor) : base(actor)
        {
        }

        public GameRef(IActorRef actor, IRequestWaiter requestWaiter, TimeSpan? timeout) : base(actor, requestWaiter, timeout)
        {
        }

        public IGame_NoReply WithNoReply()
        {
            return this;
        }

        public GameRef WithRequestWaiter(IRequestWaiter requestWaiter)
        {
            return new GameRef(Actor, requestWaiter, Timeout);
        }

        public GameRef WithTimeout(TimeSpan? timeout)
        {
            return new GameRef(Actor, RequestWaiter, timeout);
        }

        public Task<System.Tuple<System.Int32, Domain.GameInfo>> Join(System.Int64 userId, System.String userName, Domain.IGameObserver observer)
        {
            var requestMessage = new RequestMessage {
                InvokePayload = new IGame_PayloadTable.Join_Invoke { userId = userId, userName = userName, observer = (Domain.GameObserver)observer }
            };
            return SendRequestAndReceive<System.Tuple<System.Int32, Domain.GameInfo>>(requestMessage);
        }

        public Task Leave(System.Int64 userId)
        {
            var requestMessage = new RequestMessage {
                InvokePayload = new IGame_PayloadTable.Leave_Invoke { userId = userId }
            };
            return SendRequestAndWait(requestMessage);
        }

        void IGame_NoReply.Join(System.Int64 userId, System.String userName, Domain.IGameObserver observer)
        {
            var requestMessage = new RequestMessage {
                InvokePayload = new IGame_PayloadTable.Join_Invoke { userId = userId, userName = userName, observer = (Domain.GameObserver)observer }
            };
            SendRequest(requestMessage);
        }

        void IGame_NoReply.Leave(System.Int64 userId)
        {
            var requestMessage = new RequestMessage {
                InvokePayload = new IGame_PayloadTable.Leave_Invoke { userId = userId }
            };
            SendRequest(requestMessage);
        }
    }
}

#endregion
#region Domain.IGameClient

namespace Domain
{
    [PayloadTableForInterfacedActor(typeof(IGameClient))]
    public static class IGameClient_PayloadTable
    {
        public static Type[,] GetPayloadTypes()
        {
            return new Type[,] {
                { typeof(ZoneMessage_Invoke), null },
            };
        }

        [ProtoContract, TypeAlias]
        public class ZoneMessage_Invoke
            : IInterfacedPayload, ITagOverridable, IAsyncInvokable
        {
            [ProtoMember(1)] public System.Byte[] bytes;
            [ProtoMember(2)] public System.Int32 clientId;
            public Type GetInterfaceType() { return typeof(IGameClient); }
            public void SetTag(object value) { clientId = (System.Int32)value; }
            public async Task<IValueGetable> InvokeAsync(object target)
            {
                await ((IGameClient)target).ZoneMessage(bytes, clientId);
                return null;
            }
        }
    }

    public interface IGameClient_NoReply
    {
        void ZoneMessage(System.Byte[] bytes, System.Int32 clientId = 0);
    }

    [ProtoContract, TypeAlias]
    public class GameClientRef : InterfacedActorRef, IGameClient, IGameClient_NoReply
    {
        [ProtoMember(1)] private ActorRefBase _actor
        {
            get { return (ActorRefBase)Actor; }
            set { Actor = value; }
        }

        private GameClientRef() : base(null)
        {
        }

        public GameClientRef(IActorRef actor) : base(actor)
        {
        }

        public GameClientRef(IActorRef actor, IRequestWaiter requestWaiter, TimeSpan? timeout) : base(actor, requestWaiter, timeout)
        {
        }

        public IGameClient_NoReply WithNoReply()
        {
            return this;
        }

        public GameClientRef WithRequestWaiter(IRequestWaiter requestWaiter)
        {
            return new GameClientRef(Actor, requestWaiter, Timeout);
        }

        public GameClientRef WithTimeout(TimeSpan? timeout)
        {
            return new GameClientRef(Actor, RequestWaiter, timeout);
        }

        public Task ZoneMessage(System.Byte[] bytes, System.Int32 clientId = 0)
        {
            var requestMessage = new RequestMessage {
                InvokePayload = new IGameClient_PayloadTable.ZoneMessage_Invoke { bytes = bytes, clientId = clientId }
            };
            return SendRequestAndWait(requestMessage);
        }

        void IGameClient_NoReply.ZoneMessage(System.Byte[] bytes, System.Int32 clientId)
        {
            var requestMessage = new RequestMessage {
                InvokePayload = new IGameClient_PayloadTable.ZoneMessage_Invoke { bytes = bytes, clientId = clientId }
            };
            SendRequest(requestMessage);
        }
    }
}

#endregion
#region Domain.IGamePairMaker

namespace Domain
{
    [PayloadTableForInterfacedActor(typeof(IGamePairMaker))]
    public static class IGamePairMaker_PayloadTable
    {
        public static Type[,] GetPayloadTypes()
        {
            return new Type[,] {
                { typeof(RegisterPairing_Invoke), null },
                { typeof(UnregisterPairing_Invoke), null },
            };
        }

        [ProtoContract, TypeAlias]
        public class RegisterPairing_Invoke
            : IInterfacedPayload, IAsyncInvokable
        {
            [ProtoMember(1)] public System.Int64 userId;
            [ProtoMember(2)] public System.String userName;
            [ProtoMember(3)] public Domain.GameDifficulty difficulty;
            [ProtoMember(4)] public Domain.UserPairingObserver observer;
            public Type GetInterfaceType() { return typeof(IGamePairMaker); }
            public async Task<IValueGetable> InvokeAsync(object target)
            {
                await ((IGamePairMaker)target).RegisterPairing(userId, userName, difficulty, observer);
                return null;
            }
        }

        [ProtoContract, TypeAlias]
        public class UnregisterPairing_Invoke
            : IInterfacedPayload, IAsyncInvokable
        {
            [ProtoMember(1)] public System.Int64 userId;
            public Type GetInterfaceType() { return typeof(IGamePairMaker); }
            public async Task<IValueGetable> InvokeAsync(object target)
            {
                await ((IGamePairMaker)target).UnregisterPairing(userId);
                return null;
            }
        }
    }

    public interface IGamePairMaker_NoReply
    {
        void RegisterPairing(System.Int64 userId, System.String userName, Domain.GameDifficulty difficulty, Domain.IUserPairingObserver observer);
        void UnregisterPairing(System.Int64 userId);
    }

    [ProtoContract, TypeAlias]
    public class GamePairMakerRef : InterfacedActorRef, IGamePairMaker, IGamePairMaker_NoReply
    {
        [ProtoMember(1)] private ActorRefBase _actor
        {
            get { return (ActorRefBase)Actor; }
            set { Actor = value; }
        }

        private GamePairMakerRef() : base(null)
        {
        }

        public GamePairMakerRef(IActorRef actor) : base(actor)
        {
        }

        public GamePairMakerRef(IActorRef actor, IRequestWaiter requestWaiter, TimeSpan? timeout) : base(actor, requestWaiter, timeout)
        {
        }

        public IGamePairMaker_NoReply WithNoReply()
        {
            return this;
        }

        public GamePairMakerRef WithRequestWaiter(IRequestWaiter requestWaiter)
        {
            return new GamePairMakerRef(Actor, requestWaiter, Timeout);
        }

        public GamePairMakerRef WithTimeout(TimeSpan? timeout)
        {
            return new GamePairMakerRef(Actor, RequestWaiter, timeout);
        }

        public Task RegisterPairing(System.Int64 userId, System.String userName, Domain.GameDifficulty difficulty, Domain.IUserPairingObserver observer)
        {
            var requestMessage = new RequestMessage {
                InvokePayload = new IGamePairMaker_PayloadTable.RegisterPairing_Invoke { userId = userId, userName = userName, difficulty = difficulty, observer = (Domain.UserPairingObserver)observer }
            };
            return SendRequestAndWait(requestMessage);
        }

        public Task UnregisterPairing(System.Int64 userId)
        {
            var requestMessage = new RequestMessage {
                InvokePayload = new IGamePairMaker_PayloadTable.UnregisterPairing_Invoke { userId = userId }
            };
            return SendRequestAndWait(requestMessage);
        }

        void IGamePairMaker_NoReply.RegisterPairing(System.Int64 userId, System.String userName, Domain.GameDifficulty difficulty, Domain.IUserPairingObserver observer)
        {
            var requestMessage = new RequestMessage {
                InvokePayload = new IGamePairMaker_PayloadTable.RegisterPairing_Invoke { userId = userId, userName = userName, difficulty = difficulty, observer = (Domain.UserPairingObserver)observer }
            };
            SendRequest(requestMessage);
        }

        void IGamePairMaker_NoReply.UnregisterPairing(System.Int64 userId)
        {
            var requestMessage = new RequestMessage {
                InvokePayload = new IGamePairMaker_PayloadTable.UnregisterPairing_Invoke { userId = userId }
            };
            SendRequest(requestMessage);
        }
    }
}

#endregion
#region Domain.IUser

namespace Domain
{
    [PayloadTableForInterfacedActor(typeof(IUser))]
    public static class IUser_PayloadTable
    {
        public static Type[,] GetPayloadTypes()
        {
            return new Type[,] {
                { typeof(JoinGame_Invoke), typeof(JoinGame_Return) },
                { typeof(LeaveGame_Invoke), null },
                { typeof(RegisterPairing_Invoke), null },
                { typeof(UnregisterPairing_Invoke), null },
            };
        }

        [ProtoContract, TypeAlias]
        public class JoinGame_Invoke
            : IInterfacedPayload, IAsyncInvokable
        {
            [ProtoMember(1)] public System.Int64 gameId;
            [ProtoMember(2)] public System.Int32 observerId;
            public Type GetInterfaceType() { return typeof(IUser); }
            public async Task<IValueGetable> InvokeAsync(object target)
            {
                var __v = await ((IUser)target).JoinGame(gameId, observerId);
                return (IValueGetable)(new JoinGame_Return { v = (System.Tuple<System.Int32, System.Int32, Domain.GameInfo>)__v });
            }
        }

        [ProtoContract, TypeAlias]
        public class JoinGame_Return
            : IInterfacedPayload, IValueGetable
        {
            [ProtoMember(1)] public System.Tuple<System.Int32, System.Int32, Domain.GameInfo> v;
            public Type GetInterfaceType() { return typeof(IUser); }
            public object Value { get { return v; } }
        }

        [ProtoContract, TypeAlias]
        public class LeaveGame_Invoke
            : IInterfacedPayload, IAsyncInvokable
        {
            [ProtoMember(1)] public System.Int64 gameId;
            public Type GetInterfaceType() { return typeof(IUser); }
            public async Task<IValueGetable> InvokeAsync(object target)
            {
                await ((IUser)target).LeaveGame(gameId);
                return null;
            }
        }

        [ProtoContract, TypeAlias]
        public class RegisterPairing_Invoke
            : IInterfacedPayload, IAsyncInvokable
        {
            [ProtoMember(1)] public Domain.GameDifficulty difficulty;
            [ProtoMember(2)] public System.Int32 observerId;
            public Type GetInterfaceType() { return typeof(IUser); }
            public async Task<IValueGetable> InvokeAsync(object target)
            {
                await ((IUser)target).RegisterPairing(difficulty, observerId);
                return null;
            }
        }

        [ProtoContract, TypeAlias]
        public class UnregisterPairing_Invoke
            : IInterfacedPayload, IAsyncInvokable
        {
            public Type GetInterfaceType() { return typeof(IUser); }
            public async Task<IValueGetable> InvokeAsync(object target)
            {
                await ((IUser)target).UnregisterPairing();
                return null;
            }
        }
    }

    public interface IUser_NoReply
    {
        void JoinGame(System.Int64 gameId, System.Int32 observerId);
        void LeaveGame(System.Int64 gameId);
        void RegisterPairing(Domain.GameDifficulty difficulty, System.Int32 observerId);
        void UnregisterPairing();
    }

    [ProtoContract, TypeAlias]
    public class UserRef : InterfacedActorRef, IUser, IUser_NoReply
    {
        [ProtoMember(1)] private ActorRefBase _actor
        {
            get { return (ActorRefBase)Actor; }
            set { Actor = value; }
        }

        private UserRef() : base(null)
        {
        }

        public UserRef(IActorRef actor) : base(actor)
        {
        }

        public UserRef(IActorRef actor, IRequestWaiter requestWaiter, TimeSpan? timeout) : base(actor, requestWaiter, timeout)
        {
        }

        public IUser_NoReply WithNoReply()
        {
            return this;
        }

        public UserRef WithRequestWaiter(IRequestWaiter requestWaiter)
        {
            return new UserRef(Actor, requestWaiter, Timeout);
        }

        public UserRef WithTimeout(TimeSpan? timeout)
        {
            return new UserRef(Actor, RequestWaiter, timeout);
        }

        public Task<System.Tuple<System.Int32, System.Int32, Domain.GameInfo>> JoinGame(System.Int64 gameId, System.Int32 observerId)
        {
            var requestMessage = new RequestMessage {
                InvokePayload = new IUser_PayloadTable.JoinGame_Invoke { gameId = gameId, observerId = observerId }
            };
            return SendRequestAndReceive<System.Tuple<System.Int32, System.Int32, Domain.GameInfo>>(requestMessage);
        }

        public Task LeaveGame(System.Int64 gameId)
        {
            var requestMessage = new RequestMessage {
                InvokePayload = new IUser_PayloadTable.LeaveGame_Invoke { gameId = gameId }
            };
            return SendRequestAndWait(requestMessage);
        }

        public Task RegisterPairing(Domain.GameDifficulty difficulty, System.Int32 observerId)
        {
            var requestMessage = new RequestMessage {
                InvokePayload = new IUser_PayloadTable.RegisterPairing_Invoke { difficulty = difficulty, observerId = observerId }
            };
            return SendRequestAndWait(requestMessage);
        }

        public Task UnregisterPairing()
        {
            var requestMessage = new RequestMessage {
                InvokePayload = new IUser_PayloadTable.UnregisterPairing_Invoke {  }
            };
            return SendRequestAndWait(requestMessage);
        }

        void IUser_NoReply.JoinGame(System.Int64 gameId, System.Int32 observerId)
        {
            var requestMessage = new RequestMessage {
                InvokePayload = new IUser_PayloadTable.JoinGame_Invoke { gameId = gameId, observerId = observerId }
            };
            SendRequest(requestMessage);
        }

        void IUser_NoReply.LeaveGame(System.Int64 gameId)
        {
            var requestMessage = new RequestMessage {
                InvokePayload = new IUser_PayloadTable.LeaveGame_Invoke { gameId = gameId }
            };
            SendRequest(requestMessage);
        }

        void IUser_NoReply.RegisterPairing(Domain.GameDifficulty difficulty, System.Int32 observerId)
        {
            var requestMessage = new RequestMessage {
                InvokePayload = new IUser_PayloadTable.RegisterPairing_Invoke { difficulty = difficulty, observerId = observerId }
            };
            SendRequest(requestMessage);
        }

        void IUser_NoReply.UnregisterPairing()
        {
            var requestMessage = new RequestMessage {
                InvokePayload = new IUser_PayloadTable.UnregisterPairing_Invoke {  }
            };
            SendRequest(requestMessage);
        }
    }
}

#endregion
#region Domain.IUserLogin

namespace Domain
{
    [PayloadTableForInterfacedActor(typeof(IUserLogin))]
    public static class IUserLogin_PayloadTable
    {
        public static Type[,] GetPayloadTypes()
        {
            return new Type[,] {
                { typeof(Login_Invoke), typeof(Login_Return) },
            };
        }

        [ProtoContract, TypeAlias]
        public class Login_Invoke
            : IInterfacedPayload, IAsyncInvokable
        {
            [ProtoMember(1)] public System.String id;
            [ProtoMember(2)] public System.String password;
            [ProtoMember(3)] public System.Int32 observerId;
            public Type GetInterfaceType() { return typeof(IUserLogin); }
            public async Task<IValueGetable> InvokeAsync(object target)
            {
                var __v = await ((IUserLogin)target).Login(id, password, observerId);
                return (IValueGetable)(new Login_Return { v = __v });
            }
        }

        [ProtoContract, TypeAlias]
        public class Login_Return
            : IInterfacedPayload, IValueGetable
        {
            [ProtoMember(1)] public Domain.LoginResult v;
            public Type GetInterfaceType() { return typeof(IUserLogin); }
            public object Value { get { return v; } }
        }
    }

    public interface IUserLogin_NoReply
    {
        void Login(System.String id, System.String password, System.Int32 observerId);
    }

    [ProtoContract, TypeAlias]
    public class UserLoginRef : InterfacedActorRef, IUserLogin, IUserLogin_NoReply
    {
        [ProtoMember(1)] private ActorRefBase _actor
        {
            get { return (ActorRefBase)Actor; }
            set { Actor = value; }
        }

        private UserLoginRef() : base(null)
        {
        }

        public UserLoginRef(IActorRef actor) : base(actor)
        {
        }

        public UserLoginRef(IActorRef actor, IRequestWaiter requestWaiter, TimeSpan? timeout) : base(actor, requestWaiter, timeout)
        {
        }

        public IUserLogin_NoReply WithNoReply()
        {
            return this;
        }

        public UserLoginRef WithRequestWaiter(IRequestWaiter requestWaiter)
        {
            return new UserLoginRef(Actor, requestWaiter, Timeout);
        }

        public UserLoginRef WithTimeout(TimeSpan? timeout)
        {
            return new UserLoginRef(Actor, RequestWaiter, timeout);
        }

        public Task<Domain.LoginResult> Login(System.String id, System.String password, System.Int32 observerId)
        {
            var requestMessage = new RequestMessage {
                InvokePayload = new IUserLogin_PayloadTable.Login_Invoke { id = id, password = password, observerId = observerId }
            };
            return SendRequestAndReceive<Domain.LoginResult>(requestMessage);
        }

        void IUserLogin_NoReply.Login(System.String id, System.String password, System.Int32 observerId)
        {
            var requestMessage = new RequestMessage {
                InvokePayload = new IUserLogin_PayloadTable.Login_Invoke { id = id, password = password, observerId = observerId }
            };
            SendRequest(requestMessage);
        }
    }
}

#endregion
#region Domain.IGameObserver

namespace Domain
{
    public static class IGameObserver_PayloadTable
    {
        [ProtoContract, TypeAlias]
        public class Join_Invoke : IInvokable
        {
            [ProtoMember(1)] public System.Int64 userId;
            [ProtoMember(2)] public System.String userName;
            [ProtoMember(3)] public System.Int32 clientId;
            public void Invoke(object target)
            {
                ((IGameObserver)target).Join(userId, userName, clientId);
            }
        }

        [ProtoContract, TypeAlias]
        public class Leave_Invoke : IInvokable
        {
            [ProtoMember(1)] public System.Int64 userId;
            public void Invoke(object target)
            {
                ((IGameObserver)target).Leave(userId);
            }
        }

        [ProtoContract, TypeAlias]
        public class ZoneMessage_Invoke : IInvokable
        {
            [ProtoMember(1)] public System.Byte[] bytes;
            public void Invoke(object target)
            {
                ((IGameObserver)target).ZoneMessage(bytes);
            }
        }

        [ProtoContract, TypeAlias]
        public class Begin_Invoke : IInvokable
        {
            public void Invoke(object target)
            {
                ((IGameObserver)target).Begin();
            }
        }

        [ProtoContract, TypeAlias]
        public class End_Invoke : IInvokable
        {
            [ProtoMember(1)] public System.Int32 winnerId;
            public void Invoke(object target)
            {
                ((IGameObserver)target).End(winnerId);
            }
        }

        [ProtoContract, TypeAlias]
        public class Abort_Invoke : IInvokable
        {
            public void Invoke(object target)
            {
                ((IGameObserver)target).Abort();
            }
        }
    }

    [ProtoContract, TypeAlias]
    public class GameObserver : InterfacedObserver, IGameObserver
    {
        [ProtoMember(1)] private ActorRefBase _actor
        {
            get { return Channel != null ? (ActorRefBase)(((ActorNotificationChannel)Channel).Actor) : null; }
            set { Channel = new ActorNotificationChannel(value); }
        }

        [ProtoMember(2)] private int _observerId
        {
            get { return ObserverId; }
            set { ObserverId = value; }
        }

        private GameObserver() : base(null, 0)
        {
        }

        public GameObserver(IActorRef target, int observerId)
            : base(new ActorNotificationChannel(target), observerId)
        {
        }

        public GameObserver(INotificationChannel channel, int observerId)
            : base(channel, observerId)
        {
        }

        public void Join(System.Int64 userId, System.String userName, System.Int32 clientId)
        {
            var payload = new IGameObserver_PayloadTable.Join_Invoke { userId = userId, userName = userName, clientId = clientId };
            Notify(payload);
        }

        public void Leave(System.Int64 userId)
        {
            var payload = new IGameObserver_PayloadTable.Leave_Invoke { userId = userId };
            Notify(payload);
        }

        public void ZoneMessage(System.Byte[] bytes)
        {
            var payload = new IGameObserver_PayloadTable.ZoneMessage_Invoke { bytes = bytes };
            Notify(payload);
        }

        public void Begin()
        {
            var payload = new IGameObserver_PayloadTable.Begin_Invoke {  };
            Notify(payload);
        }

        public void End(System.Int32 winnerId)
        {
            var payload = new IGameObserver_PayloadTable.End_Invoke { winnerId = winnerId };
            Notify(payload);
        }

        public void Abort()
        {
            var payload = new IGameObserver_PayloadTable.Abort_Invoke {  };
            Notify(payload);
        }
    }
}

#endregion
#region Domain.IUserEventObserver

namespace Domain
{
    public static class IUserEventObserver_PayloadTable
    {
        [ProtoContract, TypeAlias]
        public class UserContextChange_Invoke : IInvokable
        {
            [ProtoMember(1)] public Domain.TrackableUserContextTracker userContextTracker;
            public void Invoke(object target)
            {
                ((IUserEventObserver)target).UserContextChange(userContextTracker);
            }
        }
    }

    [ProtoContract, TypeAlias]
    public class UserEventObserver : InterfacedObserver, IUserEventObserver
    {
        [ProtoMember(1)] private ActorRefBase _actor
        {
            get { return Channel != null ? (ActorRefBase)(((ActorNotificationChannel)Channel).Actor) : null; }
            set { Channel = new ActorNotificationChannel(value); }
        }

        [ProtoMember(2)] private int _observerId
        {
            get { return ObserverId; }
            set { ObserverId = value; }
        }

        private UserEventObserver() : base(null, 0)
        {
        }

        public UserEventObserver(IActorRef target, int observerId)
            : base(new ActorNotificationChannel(target), observerId)
        {
        }

        public UserEventObserver(INotificationChannel channel, int observerId)
            : base(channel, observerId)
        {
        }

        public void UserContextChange(Domain.TrackableUserContextTracker userContextTracker)
        {
            var payload = new IUserEventObserver_PayloadTable.UserContextChange_Invoke { userContextTracker = userContextTracker };
            Notify(payload);
        }
    }
}

#endregion
#region Domain.IUserPairingObserver

namespace Domain
{
    public static class IUserPairingObserver_PayloadTable
    {
        [ProtoContract, TypeAlias]
        public class MakePair_Invoke : IInvokable
        {
            [ProtoMember(1)] public System.Int64 gameId;
            [ProtoMember(2)] public System.String opponentName;
            public void Invoke(object target)
            {
                ((IUserPairingObserver)target).MakePair(gameId, opponentName);
            }
        }
    }

    [ProtoContract, TypeAlias]
    public class UserPairingObserver : InterfacedObserver, IUserPairingObserver
    {
        [ProtoMember(1)] private ActorRefBase _actor
        {
            get { return Channel != null ? (ActorRefBase)(((ActorNotificationChannel)Channel).Actor) : null; }
            set { Channel = new ActorNotificationChannel(value); }
        }

        [ProtoMember(2)] private int _observerId
        {
            get { return ObserverId; }
            set { ObserverId = value; }
        }

        private UserPairingObserver() : base(null, 0)
        {
        }

        public UserPairingObserver(IActorRef target, int observerId)
            : base(new ActorNotificationChannel(target), observerId)
        {
        }

        public UserPairingObserver(INotificationChannel channel, int observerId)
            : base(channel, observerId)
        {
        }

        public void MakePair(System.Int64 gameId, System.String opponentName)
        {
            var payload = new IUserPairingObserver_PayloadTable.MakePair_Invoke { gameId = gameId, opponentName = opponentName };
            Notify(payload);
        }
    }
}

#endregion
