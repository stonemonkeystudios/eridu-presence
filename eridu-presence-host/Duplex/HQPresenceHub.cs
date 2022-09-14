using System;
using MagicOnion;
using MagicOnion.Server;
using MagicOnion.Server.Hubs;
using UnityEngine;
using System.Linq;
using Eridu.Common;

namespace HQDotNet.Presence {
    // Implements RPC service in the server project.
    // The implementation class must inherit `ServiceBase<IMyFirstService>` and `IMyFirstService`
    [GroupConfiguration(typeof(ConcurrentDictionaryGroupRepositoryFactory))]
    public class HQPresenceHub : StreamingHubBase<IPresenceHub, IPresenceHubReceiver>, IPresenceHub {
        //testing regenerstion of model files in ci/cs
        bool test = false;
        IGroup room;
        EriduPlayer self;
        EriduCharacter character;
        Matrix4x4 lastKnownPosition;
        IInMemoryStorage<EriduPlayer> _clientStorage;
        IInMemoryStorage<EriduCharacter> _characterStorage;

        //Matrix4x4[] _clientTransforms;
        //Dictionary<int, int> _clientIDTransformIndex;>

        #region IPresenceHub Methods

        public async Task<EriduPlayer[]> JoinAsync(string roomName, EriduPlayer player) {
            //_clientIDTransformIndex = new Dictionary<int, int>();
            self = player;

            //Group can bundle many connections and it has inmemory-storage so add any type per group
            (room, _clientStorage) = await Group.AddAsync(roomName, self);

            Broadcast(room).OnJoin(self);

            return _clientStorage.AllValues.ToArray();
        }

        public async Task<EriduCharacter> RegisterCharacterAsync(string roomName, int characterId) {
            character = GetSampleCharacter(characterId);


            (room, _characterStorage) = await Group.AddAsync(roomName, this.character);

            Broadcast(room).OnCharacterRegistered(this.character);
            return character;
        }

        public async Task<EriduCharacter[]> GetAllCharactersInRoom() {
            return _characterStorage.AllValues.ToArray();
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
            if (transforms == null || transforms.Length == 0)
                return -1;

            lastKnownPosition = transforms[0];

            if (room != null)
                Broadcast(room).OnClientTransformRegistered(self, transforms);
            return 0;
        }

        public async Task<int> MoveTransformsAsync(Matrix4x4[] transforms) {
            if (transforms == null || transforms.Length == 0)
                return -1;

            lastKnownPosition = transforms[0];

            if (room != null)
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

        private EriduCharacter GetSampleCharacter(int characterId) {
            var character = new EriduCharacter() { characterId = characterId, characterName = "TestName", playerId = self.clientId };
            character.equipment = GetSampleEquipment();
            return character;
        }

        private EriduEquipment GetSampleEquipment() {
            EriduEquipment equipment = new EriduEquipment();
            equipment.leftHandItem = new EriduInventoryItem() { id = 0, worldObjectGuid = "ec834644-bbf2-4b45-8a5b-8380b9eac8ec" };
            equipment.rightHandItem = new EriduInventoryItem() { id = 0, worldObjectGuid = "ec834644-bbf2-4b45-8a5b-8380b9eac8ec" };
            return equipment;
        }
    }
}