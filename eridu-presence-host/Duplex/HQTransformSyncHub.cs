using System;
using MagicOnion;
using MagicOnion.Server;
using MagicOnion.Server.Hubs;
using UnityEngine;
using System.Linq;

namespace HQDotNet.Presence {
    // Implements RPC service in the server project.

    [GroupConfiguration(typeof(ConcurrentDictionaryGroupRepositoryFactory))]
    public class HQTransformSyncHub : StreamingHubBase<ITransformSyncHub, ITransformSyncHubReceiver>, ITransformSyncHub {
        const string ENTITY_KEY = "SYNCED_ENTITIES";

        IGroup room;
        TransformSyncedEntity entity;
        int _clientID;
        int _nextEntityId = 0;
        //Matrix4x4[] _clientTransforms;
        //Dictionary<int, int> _clientIDTransformIndex;>

        #region IPresenceHub Methods

        public async Task<TransformSyncedEntity[]> JoinAsync(string roomName, int clientID, string clientToken) {
            room = await this.Group.AddAsync(roomName);
            _clientID = clientID;

            if (!Context.Items.ContainsKey(ENTITY_KEY)) {
                var list = new List<TransformSyncedEntity>();
                Context.Items.TryAdd(ENTITY_KEY, list);
            }

            //Broadcast(room)(entity);
            var items = (List<TransformSyncedEntity>)Context.Items[ENTITY_KEY];
            return items.ToArray();
        }

        public async Task LeaveAsync() {
            if(room != null) {
                await room.RemoveAsync(this.Context);
            }
        }

        protected override ValueTask OnDisconnected() {
            return CompletedTask;
        }

        protected override ValueTask OnConnecting() {
            return CompletedTask;
        }

        public async Task<int> RegisterTransforms(Matrix4x4[] transforms, string clientToken) {
            if (!Context.Items.ContainsKey(ENTITY_KEY)) {
                throw new InvalidOperationException("Entity storage has not been set up.");
            }

            var list = (List<TransformSyncedEntity>)Context.Items[ENTITY_KEY];

            TransformSyncedEntity entity = new TransformSyncedEntity() { Transforms = transforms, EntityId = _nextEntityId };
            _nextEntityId++;

            Broadcast(room).OnSyncedTransformEntityRegistered(entity);
            return entity.EntityId;
        }

        public async Task UnregisterTransforms(int entityId, string clientToken) {
            if (!Context.Items.ContainsKey(ENTITY_KEY)) {
                throw new InvalidOperationException("Entity storage has not been set up.");
            }

            var list = (List<TransformSyncedEntity>)Context.Items[ENTITY_KEY];
            var foundEntity = list.Find((entity) => { return entity.EntityId == entityId; });
            if(foundEntity != null) {
                list.Remove(foundEntity);
            }


            Broadcast(room).OnSyncedTransformEntityUnegistered(entityId);
        }

        public async Task MoveSyncedTrasnformsAsync(int entityId, Matrix4x4[] transforms, string clientToken) {
            var list = (List<TransformSyncedEntity>)Context.Items[ENTITY_KEY];
            var foundEntity = list.Find((entity) => { return entity.EntityId == entityId; });
            if (foundEntity != null) {
                foundEntity.Transforms = transforms;
            }
            Broadcast(room).OnSyncedTransformEntityUpdated(entity);
        }

        #endregion
    }
}