using MessagePack;

namespace Eridu.Common {
    [MessagePackObject]
    public class EriduInventory {
        [Key(0)]
        public EriduInventoryItem[] inventoryItems;
    }
}
