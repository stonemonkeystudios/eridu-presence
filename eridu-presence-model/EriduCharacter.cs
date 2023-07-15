using MessagePack;

namespace Eridu.Common {
    [MessagePackObject]
    [System.Serializable]
    public class EriduCharacter {
        [Key(0)]
        public int characterId;
        [Key(1)]
        public int playerId;
        [Key(2)]
        public string characterName;
        [Key(3)]
        public EriduEquipment equipment;
    }
}
