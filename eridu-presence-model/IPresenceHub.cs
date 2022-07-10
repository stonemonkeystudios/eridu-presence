using System;
using System.Threading.Tasks;
using MagicOnion;
using UnityEngine;
using MessagePack;
using HQDotNet;

namespace HQDotNet.Presence
{
    public interface IPresenceHubReceiver : IDispatchListener
    {
        // return type should be `void` or `Task`, parameters are free.
        void OnJoin(PresenceClient client);
        void OnLeave(PresenceClient client);
        void OnClientTransformRegistered(PresenceClient client, Matrix4x4[] transforms);
        void OnClientTransformsUnregistered(PresenceClient client);
        void OnTransformUpdate(PresenceClient client, Matrix4x4[] transforms);
    }

    public interface IPresenceHub : IStreamingHub<IPresenceHub, IPresenceHubReceiver> {
        // return type should be `Task` or `Task<T>`, parameters are free.
        Task<PresenceClient[]> JoinAsync(string roomName, int userID, string userName);
        Task<int> RegisterTransforms(Matrix4x4[] transforms);
        Task<int> UnregisterTransforms(Matrix4x4[] transforms);
        Task<int> MoveTransformsAsync(Matrix4x4[] transforms);
        Task LeaveAsync();
    }

    [MessagePackObject]
    public class PresenceClient {
        [Key(0)]
        public string Username { get; set; }
        [Key(1)]
        public int Id { get; set; }
    }
}
