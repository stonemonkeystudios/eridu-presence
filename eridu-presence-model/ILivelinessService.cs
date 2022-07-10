using MagicOnion;

namespace HQDotNet.Presence {

    public interface ILivelinessService : IService<ILivelinessService> {
        UnaryResult<string> Status();
    }
}
