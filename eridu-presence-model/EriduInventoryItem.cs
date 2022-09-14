using MessagePack;

namespace Eridu.Common {
    [MessagePackObject]
    public class EriduInventoryItem {
        [Key(0)]
        public int id;
        [Key(1)]
        public string worldObjectGuid;
    }
}
