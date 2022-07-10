using System;
using MagicOnion;
using MagicOnion.Server;
using UnityEngine;

namespace HQDotNet.Presence {
    // Implements RPC service in the server project.
    // The implementation class must inherit `ServiceBase<IMyFirstService>` and `IMyFirstService`
    public class HQPresenceService : ServiceBase<IPresenceService>, IPresenceService {
        public async UnaryResult<Matrix4x4> SetPosition(Matrix4x4 newPosition) {
            return newPosition;
        }
    }
}