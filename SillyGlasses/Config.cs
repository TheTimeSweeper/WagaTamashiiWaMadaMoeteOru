using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SillyGlasses
{
    public class ConfigEntry
    {

    }

    public class ConfigEntry<T> : ConfigEntry
    {
        public BepInEx.Configuration.ConfigEntry<T> ActualConfigEntry;
        public T DefaultValue;

        public static implicit operator T(ConfigEntry<T> config)
        {
            return config.Value;
        }

        public ConfigEntry(T defaultValue)
        {
            ActualConfigEntry = null;
            DefaultValue = defaultValue;
        }

        public ConfigEntry(BepInEx.Configuration.ConfigEntry<T> actualConfigEntry, T defaultValue)
        {
            ActualConfigEntry = actualConfigEntry;
            DefaultValue = defaultValue;
        }

        public T Value
        {
            get
            {
                if (ActualConfigEntry != null)
                {
                    return ActualConfigEntry.Value;
                }
                return DefaultValue;
            }
        }
    }

    public static class Config
    {
        public static ConfigFile MyConfig = SillyGlassesPlugin.instance.Config;

        private static List<string> disabledSections = new List<string>();

        private static bool enableAll = true;

        private static bool loadedIcon;

        public static void DisableSection(string section)
        {
            disabledSections.Add(section);
        }
        private static bool SectionDisabled(string section)
        {
            return disabledSections.Contains(section);
        }

        public static void ConfigureBody(CharacterBody bodyComponent, string section, string bodyInfoTitle = "")
        {
            if (string.IsNullOrEmpty(bodyInfoTitle))
            {
                bodyInfoTitle = bodyComponent.name;
            }

            bodyComponent.baseMaxHealth = Config.BindAndOptions(
                    section,
                    $"{bodyInfoTitle} Base Max Health",
                    bodyComponent.baseMaxHealth,
                    0,
                    1000,
                    "levelMaxHealth will be adjusted accordingly (baseMaxHealth * 0.3)",
                    true).Value;
            bodyComponent.levelMaxHealth = Mathf.Round(bodyComponent.baseMaxHealth * 0.3f);

            bodyComponent.baseRegen = Config.BindAndOptions(
                    section,
                    $"{bodyInfoTitle} Base Regen",
                    bodyComponent.baseRegen,
                    "levelRegen will be adjusted accordingly (baseRegen * 0.2)",
                    true).Value;
            bodyComponent.levelRegen = bodyComponent.baseRegen * 0.2f;

            bodyComponent.baseArmor = Config.BindAndOptions(
                    section,
                    $"{bodyInfoTitle} Armor",
                    bodyComponent.baseArmor,
                    "",
                    true).Value;

            bodyComponent.baseDamage = Config.BindAndOptions(
                    section,
                    $"{bodyInfoTitle} Base Damage",
                    bodyComponent.baseDamage,
                    "pretty much all survivors are 12. If you want to change damage, change damage of the moves instead.\nlevelDamage will be adjusted accordingly (baseDamage * 0.2)",
                    true).Value;
            bodyComponent.levelDamage = bodyComponent.baseDamage * 0.2f;

            bodyComponent.baseJumpCount = Config.BindAndOptions(
                    section,
                    $"{bodyInfoTitle} Jump Count",
                    bodyComponent.baseJumpCount,
                    "",
                    true).Value;
        }

        public static void ConfigureSkillDef(SkillDef skillDef, string section, string skillTitle, bool cooldown = true, bool maxStock = true, bool rechargeStock = false)
        {
            if (cooldown)
            {
                skillDef.baseRechargeInterval = Config.BindAndOptions(
                    section,
                    $"{skillTitle} cooldown",
                    skillDef.baseRechargeInterval,
                    0,
                    20,
                    "",
                    true).Value;
            }
            if (maxStock)
            {
                skillDef.baseMaxStock = Config.BindAndOptions(
                    section,
                    $"{skillTitle} stocks",
                    skillDef.baseMaxStock,
                    0,
                    100,
                    "",
                    true).Value;
            }
            if (rechargeStock)
            {
                skillDef.rechargeStock = Config.BindAndOptions(
                    section,
                    $"{skillTitle} recharge stocks",
                    skillDef.baseMaxStock,
                    0,
                    100,
                    "",
                    true).Value;
            }
        }

        /// <summary>
        /// automatically makes config entries for disabling survivors
        /// </summary>
        /// <param name="section"></param>
        /// <param name="characterName"></param>
        /// <param name="description"></param>
        /// <param name="enabledByDefault"></param>
        public static ConfigEntry<bool> CharacterEnableConfig(string section, string characterName, string description = "", bool enabledByDefault = true)
        {

            if (string.IsNullOrEmpty(description))
            {
                description = "Set to false to disable this character and as much of its code and content as possible";
            }
            return BindAndOptions<bool>(section,
                                        "Enable " + characterName,
                                        enabledByDefault,
                                        description,
                                        true);
        }
        public static ConfigEntry<T> BindAndOptionsSlider<T>(string section, string name, T defaultValue, float min = 0, float max = 20, string description = "", bool restartRequired = false) =>
            BindAndOptions<T>(section, name, defaultValue, min, max, description, restartRequired);
        public static ConfigEntry<T> BindAndOptions<T>(string section, string name, T defaultValue, string description = "", bool restartRequired = false) =>
            BindAndOptions<T>(section, name, defaultValue, 0, 20, description, restartRequired);
        public static ConfigEntry<T> BindAndOptions<T>(string section, string name, T defaultValue, float min, float max, string description = "", bool restartRequired = false)
        {
            if (string.IsNullOrEmpty(description))
            {
                description = name;
            }
            description += $"\nDefault: {defaultValue}";
            if (restartRequired)
            {
                description += "\n(restart required)";
            }

            if(!enableAll && SectionDisabled(section))
                return new ConfigEntry<T>(null, defaultValue);

            BepInEx.Configuration.ConfigEntry<T> configEntry = MyConfig.Bind(section, name, defaultValue, description);

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions"))
            {
                TryRegisterOption(configEntry, min, max, restartRequired);
            }

            return new ConfigEntry<T>(configEntry, defaultValue);
        }
        public static ConfigEntry BindAndOptions(string section, string name, System.ValueType defaultValue, float min, float max, string description = "", bool restartRequired = false)
        {
            if(defaultValue is float)
            {
                return BindAndOptions(section, name, (float)defaultValue, min, max, description, restartRequired);
            }
            if (defaultValue is int)
            {
                return BindAndOptions(section, name, (int)defaultValue, min, max, description, restartRequired);
            }
            if (defaultValue is bool)
            {
                return BindAndOptions(section, name, (bool)defaultValue, min, max, description, restartRequired);
            }
            if (defaultValue is KeyboardShortcut)
            {
                return BindAndOptions(section, name, (KeyboardShortcut)defaultValue, min, max, description, restartRequired);
            }
            if (defaultValue == null)
            {
                Debug.LogError($"defaultvalue was null somehow for {section}:{name} {defaultValue}");
            }
            Debug.LogError($"Configuring a field with unsupported type {defaultValue.GetType()} for {section}:{name}");
            return null;
        }

        //add risk of options dll to your project libs and uncomment this for a soft dependency
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void TryRegisterOption<T>(BepInEx.Configuration.ConfigEntry<T> entry, float min, float max, bool restartRequired)
        {
            if (entry is BepInEx.Configuration.ConfigEntry<float>)
            {
                ModSettingsManager.AddOption(new SliderOption(entry as BepInEx.Configuration.ConfigEntry<float>, new SliderConfig() { min = min, max = max, formatString = "{0:0.000}", restartRequired = restartRequired }));
            }
            if (entry is BepInEx.Configuration.ConfigEntry<int>)
            {
                ModSettingsManager.AddOption(new IntSliderOption(entry as BepInEx.Configuration.ConfigEntry<int>, new IntSliderConfig() { min = (int)min, max = (int)max, restartRequired = restartRequired }));
            }
            if (entry is BepInEx.Configuration.ConfigEntry<bool>)
            {
                ModSettingsManager.AddOption(new CheckBoxOption(entry as BepInEx.Configuration.ConfigEntry<bool>, restartRequired));
            }
            if (entry is BepInEx.Configuration.ConfigEntry<KeyboardShortcut>)
            {
                ModSettingsManager.AddOption(new KeyBindOption(entry as BepInEx.Configuration.ConfigEntry<KeyboardShortcut>, restartRequired));
            }
            if (!loadedIcon)
            {
                loadedIcon = true;
                try
                {
                    ModSettingsManager.SetModIcon(ImgHandler.LoadSpriteFromModFolder("icon.png"));
                }
                catch (System.Exception e)
                {
                    Debug.LogError("error adding ROO mod icon\n" + e);
                }
            }
        }

        //public static void InitConfigAttributes(System.Type typeWithStaticConfigFieldsMakeSureTheyreStatic)
        //{
        //    var fields = typeWithStaticConfigFieldsMakeSureTheyreStatic.GetFields();

        //    foreach (FieldInfo field in fields)
        //    {
        //        ConfigureAttribute attribute = field.GetCustomAttribute<ConfigureAttribute>();
        //        if (attribute != null)
        //        {
        //            attribute.InitFromField(field);
        //            try
        //            {
        //                field.SetValue(null, Config.BindAndOptions(attribute.section, attribute.name, attribute.defaultValue, attribute.min, attribute.max, attribute.description, attribute.restartRequired));
        //            }
        //            catch
        //            {
        //                throw new System.Exception($"Error adding config {attribute.section}: {attribute.name}.");
        //            }
        //        }
        //    }
        //}

        //Taken from https://github.com/ToastedOven/CustomEmotesAPI/blob/main/CustomEmotesAPI/CustomEmotesAPI/CustomEmotesAPI.cs
        public static bool GetKeyPressed(KeyboardShortcut entry)
        {
            foreach (var item in entry.Modifiers)
            {
                if (!Input.GetKey(item))
                {
                    return false;
                }
            }
            return Input.GetKeyDown(entry.MainKey);
        }
    }

    //copypasted from another project
    public static class ImgHandler
    {
        public static Sprite LoadSprite(string path, string name)
        {
            Texture2D texture2D = LoadTex2D(path, true);

            return CreateSprite(texture2D, name);
        }

        public static Texture2D LoadTex2D(string fullFilePath, bool pointFilter = false)
        {
            Texture2D texture2D = ImgHandler.LoadPNG(fullFilePath, pointFilter);
            if (pointFilter)
            {
                texture2D.filterMode = FilterMode.Point;
                texture2D.Apply();
            }
            return texture2D;
        }
        private static Sprite CreateSprite(Texture2D texture2D, string name)
        {
            texture2D.name = name;
            texture2D.filterMode = FilterMode.Point;
            texture2D.Apply();
            Rect rect = new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height);
            Sprite sprite = Sprite.Create(texture2D, rect, new Vector2(0.5f, 0.5f), 16f);
            sprite.name = name;
            return sprite;
        }

        public static Texture2D LoadPNG(params string[] pathDirectories)
        {
            string fullFilePath = pathDirectories[0];
            for (int i = 1; i < pathDirectories.Length; i++)
            {
                fullFilePath = System.IO.Path.Combine(fullFilePath, pathDirectories[i]);
            }

            return LoadPNG(fullFilePath);
        }
        public static Texture2D LoadPNG(string filePath, bool pointFilter = false)
        {
            Texture2D texture2D = new Texture2D(2, 2);
            byte[] data = File.ReadAllBytes(filePath);
            texture2D.LoadImage(data);
            texture2D.filterMode = (pointFilter ? FilterMode.Point : FilterMode.Bilinear);
            texture2D.Apply();

            return texture2D;
        }

        internal static Sprite LoadSpriteFromModFolder(string fileName)
        {
            Texture2D texture2D = LoadTex2DFromPModFolder(fileName);

            return CreateSprite(texture2D, fileName);
        }
        internal static Texture2D LoadTex2DFromPModFolder(string fileName, bool pointFilter = false)
        {
            string fullFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(SillyGlassesPlugin.instance.Info.Location), fileName);

            return LoadTex2D(fullFilePath, pointFilter);
        }
    }
}
