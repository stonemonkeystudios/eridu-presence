using System.Collections.Generic;
using System.Linq;
using Eridu.Common;

namespace HQDotNet.Presence {
    public class PresenceStorage {
        Dictionary<EriduPlayer, EriduCharacter> playerCharacters = new Dictionary<EriduPlayer, EriduCharacter>();

        static PresenceStorage _instance;

        public static PresenceStorage Instance {
            get { return _instance; }
        }

        public static void CreateInstance() {
            _instance = new PresenceStorage();
        }

        public static void DeleteInstance() {
            _instance = null;
        }

        public EriduCharacter GetCharacterForPlayer(EriduPlayer player) {
            if (!playerCharacters.ContainsKey(player))
                return null;
            return playerCharacters[player];
        }

        public EriduCharacter[] GetAllCharacters() {
            return playerCharacters.Values.ToArray();
        }

        public void RegisterCharacter(EriduPlayer player, EriduCharacter character) {
            if(player != null && character != null) {
                playerCharacters.Add(player, character);
            }
        }

        public void UnRegisterCharacter(EriduPlayer player) {
            if (playerCharacters != null && player != null && playerCharacters.ContainsKey(player)) {
                playerCharacters.Remove(player);
            }
        }

        public void UnRegisterCharacter(EriduCharacter character) {
            if (playerCharacters.ContainsValue(character)) {
                var kvp = playerCharacters.First(kvp => kvp.Value == character);
                playerCharacters.Remove(kvp.Key);
            }
        }
    }
}