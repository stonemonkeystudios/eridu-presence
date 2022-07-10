using System;
using System.Threading.Tasks;
using MagicOnion;
using UnityEngine;
using MessagePack;
using HQDotNet;

namespace HQDotNet.Presence
{
    public interface ITransformSyncHubReceiver : IDispatchListener{

        void OnSyncedTransformEntityRegistered(TransformSyncedEntity entity);
        void OnSyncedTransformEntityUnegistered(int entityId);
        void OnSyncedTransformEntityUpdated(TransformSyncedEntity entity);
    }

    public interface ITransformSyncHub : IStreamingHub<ITransformSyncHub, ITransformSyncHubReceiver> {
        Task<TransformSyncedEntity[]> JoinAsync(string roomName, int clientID, string clientToken);
        // return type should be `Task` or `Task<T>`, parameters are free.
        Task<int> RegisterTransforms(Matrix4x4[] transforms, string clientToken);
        Task UnregisterTransforms(int entityId, string clientToken);
        Task MoveSyncedTrasnformsAsync(int entityId, Matrix4x4[] transforms, string clientToken);
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
