using MessagePack;

namespace Eridu.Common {
    [MessagePackObject]
    [System.Serializable]
    public class EriduEquipment {
        [Key(0)]
        public EriduInventoryItem rightHandItem;
        [Key(1)]
        public EriduInventoryItem leftHandItem;
    }
}
