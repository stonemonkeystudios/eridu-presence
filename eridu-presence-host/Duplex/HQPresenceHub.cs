using System;
using MagicOnion;
using MagicOnion.Server;
using MagicOnion.Server.Hubs;
using UnityEngine;
using System.Linq;

namespace HQDotNet.Presence {
    // Implements RPC service in the server project.
    // The implementation class must inherit `ServiceBase<IMyFirstService>` and `IMyFirstService`
    [GroupConfiguration(typeof(ConcurrentDictionaryGroupRepositoryFactory))]
    public class HQPresenceHub : StreamingHubBase<IPresenceHub, IPresenceHubReceiver>, IPresenceHub {

        IGroup room;
        PresenceClient self;
        IInMemoryStorage<PresenceClient> _clientStorage;
        //Matrix4x4[] _clientTransforms;
        //Dictionary<int, int> _clientIDTransformIndex;>

        #region IPresenceHub Methods

        public async Task<PresenceClient[]> JoinAsync(string roomName, int userID, string userName) {
            //_clientIDTransformIndex = new Dictionary<int, int>();
            self = new PresenceClient() { Username = userName, Id=userID};

            //Group can bundle many connections and it has inmemory-storage so add any type per group
            (room, _clientStorage) = await Group.AddAsync(roomName, self);

            Broadcast(room).OnJoin(self);

            return _clientStorage.AllValues.ToArray();
        }

        public async Task LeaveAsync() {
            if (room != null) {
                await room.RemoveAsync(this.Context);
                Broadcast(room).OnLeave(self);
            }
        }

        protected override ValueTask OnDisconnected() {
            return CompletedTask;
        }

        protected override ValueTask OnConnecting() {
            return CompletedTask;
        }

        public async Task<int> RegisterTransforms(Matrix4x4[] transforms) {
            if (room != null)
                Broadcast(room).OnClientTransformRegistered(self, transforms);
            return 0;
        }

        public async Task<int> MoveTransformsAsync(Matrix4x4[] transforms) {
            if(room != null)
                Broadcast(room).OnTransformUpdate(self, transforms);
            return 0;
        }

        public async Task<int> UnregisterTransforms(Matrix4x4[] transforms) {
            if (room != null)
                Broadcast(room).OnClientTransformsUnregistered(self);
            return 0;
        }

        public IPresenceHub FireAndForget() {
            return this;
        }

        public Task DisposeAsync() {
            return Task.CompletedTask;
        }

        public Task WaitForDisconnect() {
            return Task.CompletedTask;
        }

        #endregion
    }
}