using MessagePack;

namespace Eridu.Common {
    [MessagePackObject]
    public class EriduPlayer {
        [Key(0)]
        public int clientId;
        [Key(1)]
        public int characterId;
        [Key(2)]
        public string clientEmail;
        [Key(3)]
        public bool IsRoot;
        [Key(4)]
        public bool IsAdmin;
        [Key(5)]
        public bool IsModerator;
        [Key(6)]
        public bool IsLocal;
    }
}
