using MessagePack;

namespace Eridu.Common {
    [MessagePackObject]
    [System.Serializable]
    public class EriduInventory {
        [Key(0)]
        public EriduInventoryItem[] inventoryItems;
    }
}
