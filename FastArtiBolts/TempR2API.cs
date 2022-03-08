using FastArtiBolts;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace R2API {
    public static class LanguageAPI {

        private static readonly Dictionary<string, string> LanguageDict = new Dictionary<string, string>();

        private const string genericLanguage = "generic";

        public static void Add(string key, string value) {

            if (!LanguageDict.ContainsKey(key)) {
                LanguageDict.Add(key, value);
            }
        }

        internal static void LanguageAwake() {

            On.RoR2.Language.GetLocalizedStringByToken += Language_GetLocalizedStringByToken;
            On.RoR2.Language.TokenIsRegistered += Language_TokenIsRegistered;
        }


        private static bool Language_TokenIsRegistered(On.RoR2.Language.orig_TokenIsRegistered orig, Language self, string token) {
            var languagename = self.name;
                if (LanguageDict.ContainsKey(token)) {
                    return true;
                }
            return orig(self, token);
        }

        private static string Language_GetLocalizedStringByToken(On.RoR2.Language.orig_GetLocalizedStringByToken orig, Language self, string token) {
            var languagename = self.name;
                if (LanguageDict.ContainsKey(token)) {
                return LanguageDict[token];
                }
            return orig(self, token);
        }
    }

    public static class PrefabAPI {

        private static GameObject _parent;

        public static GameObject InstantiateClone(this GameObject prefab, string newName, bool network = false) {
            GameObject instant = Object.Instantiate(prefab, GetParent().transform);
            instant.name = newName;
            return instant;
        }

        
        private static GameObject GetParent() {

            if (!_parent) {
                _parent = new GameObject("ModdedPrefabs");
                Object.DontDestroyOnLoad(_parent);

                _parent.SetActive(false);

                On.RoR2.Util.IsPrefab += (orig, obj) => {
                    if (obj.transform.parent && obj.transform.parent.gameObject.name == "ModdedPrefabs") return true;
                    return orig(obj);
                };
            }

            return _parent;
        }
    }

    public static class SoundAPI {
        public static class SoundBanks {

            public static void Add(byte[] bank) {

            }
        }
    }

    public static class ProjectileAPI {
        
        public static void Add(GameObject prefab) {
            Modules.ContentPacks.projectilePrefabs.Add(prefab);
        }
    }

    public static class LoadoutAPI {

        public static void AddSkillDef(SkillDef skillDef) {
            Modules.ContentPacks.skillDefs.Add(skillDef);
        }

        public static void AddSkill(Type type) {
            Modules.ContentPacks.entityStates.Add(type);
        }
    }
}
