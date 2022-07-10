using System;
using MagicOnion;
using UnityEngine;

namespace HQDotNet.Presence
{
    public interface IPresenceService : IService<IPresenceService>
    {
        UnaryResult<Matrix4x4> SetPosition(Matrix4x4 newPosition);
    }
}
