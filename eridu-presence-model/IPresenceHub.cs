using System;
using System.Threading.Tasks;
using MagicOnion;
using UnityEngine;
using MessagePack;
using HQDotNet;
using Eridu.Common;

namespace HQDotNet.Presence
{
    public interface IPresenceHubReceiver : IDispatchListener
    {
        // return type should be `void` or `Task`, parameters are free.
        void OnJoin(EriduPlayer player);
        void OnLeave(EriduPlayer player);
        void OnCharacterRegistered(EriduCharacter character);

        void OnCharacterUnregistered(EriduCharacter character);
        void OnClientTransformRegistered(EriduPlayer player, Matrix4x4[] transforms);
        void OnClientTransformsUnregistered(EriduPlayer player);
        void OnTransformUpdate(EriduPlayer player, Matrix4x4[] transforms);
        void OnItemEquipped(EriduCharacter character, Hand hand, EriduInventoryItem item);
        void OnWieldWeapon(EriduCharacter character, Hand hand, bool wielding);
    }

    public interface IPresenceHub : IStreamingHub<IPresenceHub, IPresenceHubReceiver> {
        // return type should be `Task` or `Task<T>`, parameters are free.
        Task<EriduPlayer[]> JoinAsync(string roomName, EriduPlayer player);
        Task<EriduCharacter> RegisterCharacterAsync(string roomName, int characterId);
        Task EquipItem(EriduCharacter character, Hand hand, EriduInventoryItem equipment);
        Task WieldWeapon(Hand hand, bool wielding);
        Task<EriduCharacter[]> GetAllCharactersInRoom();
        Task<int> RegisterTransforms(Matrix4x4[] transforms);
        Task<int> UnregisterTransforms(Matrix4x4[] transforms);
        Task<int> MoveTransformsAsync(Matrix4x4[] transforms);
        Task LeaveAsync();
    }

    [MessagePackObject]
    public class TransformSyncedEntity {
        [Key(0)]
        public int EntityId { get; set; }
        [Key(1)]
        public Matrix4x4[] Transforms { get; set; }
    }
}
