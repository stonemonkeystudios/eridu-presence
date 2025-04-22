using System;
using MagicOnion;
using MagicOnion.Server;
using MagicOnion.Server.Hubs;
using UnityEngine;
using System.Linq;
using Eridu.Common;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace HQDotNet.Presence {
    // Implements RPC service in the server project.
    // The implementation class must inherit `ServiceBase<IMyFirstService>` and `IMyFirstService`
    [GroupConfiguration(typeof(ConcurrentDictionaryGroupRepositoryFactory))]
    public class HQPresenceHub : StreamingHubBase<IPresenceHub, IPresenceHubReceiver>, IPresenceHub {
        //testing regenerstion of model files in ci/cs
        bool test = false;
        IGroup room;
        EriduPlayer self;
        EriduCharacter playerCharacter;
        Matrix4x4 lastKnownPosition;
        IInMemoryStorage<EriduPlayer> _clientStorage;
        //IInMemoryStorage<EriduCharacter> _characterStorage;

        private bool leftRoom = false;

        #region IPresenceHub Methods

        public async Task<EriduPlayer[]> JoinAsync(string roomName, EriduPlayer player) {
            //_clientIDTransformIndex = new Dictionary<int, int>();
            self = player;

            //Group can bundle many connections and it has inmemory-storage so add any type per group
            (room, _clientStorage) = await Group.AddAsync(roomName, self);

            ClientStorage.Instance.AddClient(self.clientId, this.Context.ContextId);


            BroadcastExceptSelf(room).OnJoin(self);

            return _clientStorage.AllValues.ToArray();
        }

        public async Task<EriduCharacter> RegisterCharacterAsync(string roomName, int characterId) {
            playerCharacter = GetSampleCharacter(characterId);

            if(PresenceStorage.Instance != null) {
                PresenceStorage.Instance.RegisterCharacter(self, playerCharacter);
            }
            //(room, _characterStorage) = await Group.AddAsync(roomName, this.character);

            Broadcast(room).OnCharacterRegistered(this.playerCharacter);
            return playerCharacter;
        }

        public async Task EquipItem(EriduCharacter character, Hand hand, EriduInventoryItem equipment) {
            if(character != null) {
                if(character.equipment != null) {
                    switch (hand) {
                        case Hand.left:
                            if(character.equipment.leftHandItem != null) {
                                character.equipment.leftHandItem = equipment;
                                Broadcast(room).OnItemEquipped(character, hand, equipment);
                            }
                            break;
                        case Hand.right:
                            if(character.equipment.rightHandItem != null) {
                                character.equipment.rightHandItem = equipment;
                                Broadcast(room).OnItemEquipped(character, hand, equipment);
                            }
                            break;
                    }

                    if (PresenceStorage.Instance != null) {
                        PresenceStorage.Instance.UpdateCharacter(character);
                    }
                }
            }
        }

        /// <summary>
        /// This method is used to equip an item for a character on a specific client.
        /// Used by auth server on receipt of world object spawn.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="hand"></param>
        /// <param name="equipment"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task EquipItemForCharacterOnClient(EriduCharacter character, Hand hand, EriduInventoryItem equipment, int clientId) {

            Guid? connectionGuid = ClientStorage.Instance.GetConnectionIdFromClientId(clientId);
            if (!connectionGuid.HasValue) {
                return;
            }

            if (character != null) {
                if (character.equipment != null) {
                    switch (hand) {
                        case Hand.left:
                            if (character.equipment.leftHandItem != null) {
                                character.equipment.leftHandItem = equipment;
                                BroadcastTo(room, connectionGuid.Value).OnItemEquipped(character, hand, equipment);
                            }
                            break;
                        case Hand.right:
                            if (character.equipment.rightHandItem != null) {
                                character.equipment.rightHandItem = equipment;
                                BroadcastTo(room, connectionGuid.Value).OnItemEquipped(character, hand, equipment);
                            }
                            break;
                    }
                    if (PresenceStorage.Instance != null) {
                        PresenceStorage.Instance.UpdateCharacter(character);
                    }
                }
            }
        }

        public async Task WieldWeapon(Hand hand, bool wielding) {
            Broadcast(room).OnWieldWeapon(playerCharacter, hand, wielding);
        }

        public async Task<EriduCharacter[]> GetAllCharactersInRoom() {
            if(PresenceStorage.Instance == null) {
                return new EriduCharacter[0];
            }
            return PresenceStorage.Instance.GetAllCharacters();
        }

        public async Task LeaveAsync() {
            await LeaveRoom();
        }

        protected async override ValueTask OnDisconnected() {
            await LeaveRoom();
        }

        private async Task LeaveRoom() {
            if (!leftRoom) {
                if(room != null) {
                    await room.RemoveAsync(this.Context);

                    if (PresenceStorage.Instance != null) {
                        var storedCharacter = PresenceStorage.Instance.GetCharacterForPlayer(self);
                        if (storedCharacter != null)
                            Broadcast(room).OnCharacterUnregistered(storedCharacter);
                        PresenceStorage.Instance.UnRegisterCharacter(self);
                    }
                    Broadcast(room).OnLeave(self);
                    leftRoom = true;
                }
            }
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
                BroadcastExceptSelf(room).OnTransformUpdate(self, transforms);
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

        public Task UpdateEntityHP(int rpgEntityId, bool isPlayer, int damageTaken, int baseValue, int currentValue) {
            if(room != null)
                Broadcast(room).OnUpdateEntityHP(rpgEntityId, isPlayer, damageTaken, baseValue, currentValue);
            return Task.CompletedTask;
        }

        public Task EntityDie(int rpgEntityId, bool isPlayer) {
            if(room != null)
                Broadcast(room).OnEntityDie(rpgEntityId, isPlayer);
            return Task.CompletedTask;
        }

        public Task RequestResurrectPlayer(int rpgEntityId) {
            if (room != null)
                Broadcast(room).OnRequestResurrectPlayer(rpgEntityId);
            return Task.CompletedTask;
        }

        public Task ResurrectPlayer(int rpgEntityId) {
            if (room != null)
                Broadcast(room).OnResurrectPlayer(rpgEntityId);
            return Task.CompletedTask;
        }

        public Task PrototypePresenceMessage(string messageType, string messageData) {
            if (room != null)
                Broadcast(room).OnPrototypeMessageReceived(messageType, messageData);
            return Task.CompletedTask;
        }
    }
}