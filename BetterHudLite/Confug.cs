using BepInEx.Configuration;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RiskOfOptions;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.IO;


namespace BetterHudLite {
    public class Confug
    {
        private static ConfigEntry<bool> _doBar;
        public static bool doBar
        {
            get
            {
                reloadConfig();
                return _doBar.Value;
            }
        }

        private static ConfigEntry<bool> _doSkills;
        public static bool doSkills
        {
            get
            {
                reloadConfig();
                return _doSkills.Value;
            }
        }

        public static float healthBarWidth = 1;
        public static float healthBarHeight = 1;

        private static BetterHudLitePlugin plugin => BetterHudLitePlugin.instance;

        private static void reloadConfig()
        {
            plugin.Config.Reload();
        }

        public static void doConfig()
        {
            string barSection = "Health UI";

            _doBar =
                BindAndOptions(barSection,
                        "Do the health bar",
                        true,
                        "Brings the health and level bar to the center");

            healthBarWidth =
                BindAndOptions(barSection,
                        "Health Bar Width Factor",
                        1f,
                        "size of health bar, range 0 to 4.5, 0 making it nonexistent, 4.5 making it cover the screen\n"
                        + "less than 0 just shifts it to the right, higher than 4.5 goes off screen obviously").Value;

            healthBarHeight =
                BindAndOptions(barSection,
                        "Health Bar Height Multiplier",
                        1f,
                        "multiply health bar height by this value\n"
                        + "expands both up and down. attempts to move skills with it").Value;

            string skillsSection = "Skills UI";

            _doSkills =
                BindAndOptions(skillsSection,
                        "Do the skills",
                        true,
                        "Brings the skills to the center above where the health bar is (if that's also enabled)");

            
        }

        private static bool loadedIcon;

        public static ConfigEntry<T> BindAndOptions<T>(string section, string name, T defaultValue, string description = "", bool restartRequired = false) =>
            BindAndOptions(section, name, defaultValue, 0, 20, description, restartRequired);
        public static ConfigEntry<T> BindAndOptions<T>(string section, string name, T defaultValue, float min, float max, string description = "", bool restartRequired = false)
        {
            if (string.IsNullOrEmpty(description))
            {
                description = name;
            }
            description += $"\nDefault: {defaultValue}";
            if (restartRequired)
            {
                description += " (restart required)";
            }
            ConfigEntry<T> configEntry = BetterHudLitePlugin.instance.Config.Bind(section, name, defaultValue, description);

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions"))
            {
                TryRegisterOption(configEntry, min, max, restartRequired);
            }

            return configEntry;
        }

        //back compat
        public static ConfigEntry<float> BindAndOptionsSlider(string section, string name, float defaultValue, string description, float min = 0, float max = 20, bool restartRequired = false) =>
            BindAndOptions(section, name, defaultValue, min, max, description, restartRequired);

        //add risk of options dll to your project libs and uncomment this for a soft dependency
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void TryRegisterOption<T>(ConfigEntry<T> entry, float min, float max, bool restartRequired)
        {
            if (entry is ConfigEntry<float>)
            {
                ModSettingsManager.AddOption(new SliderOption(entry as ConfigEntry<float>, new SliderConfig() { min = min, max = max, formatString = "{0:0.00}", restartRequired = restartRequired }));
            }
            if (entry is ConfigEntry<int>)
            {
                ModSettingsManager.AddOption(new IntSliderOption(entry as ConfigEntry<int>, new IntSliderConfig() { min = (int)min, max = (int)max, restartRequired = restartRequired }));
            }
            if (entry is ConfigEntry<bool>)
            {
                ModSettingsManager.AddOption(new CheckBoxOption(entry as ConfigEntry<bool>, restartRequired));
            }
            if (entry is ConfigEntry<KeyboardShortcut>)
            {
                ModSettingsManager.AddOption(new KeyBindOption(entry as ConfigEntry<KeyboardShortcut>, restartRequired));
            }
            if (!loadedIcon)
            {
                loadedIcon = true;
                try
                {
                    ModSettingsManager.SetModIcon(LoadSpriteFromModFolder("icon.png"));
                }
                catch (System.Exception e)
                {
                    Debug.LogError("error adding ROO mod icon\n" + e);
                }
            }
        }

        internal static Sprite LoadSpriteFromModFolder(string fileName, bool pointFilter = false)
        {
            string fullFilePath = Path.Combine(Path.GetDirectoryName(BetterHudLitePlugin.instance.Info.Location), fileName);

            Texture2D texture2D = new Texture2D(2, 2);
            byte[] data = File.ReadAllBytes(fullFilePath);
            texture2D.LoadImage(data);
            texture2D.filterMode = (pointFilter ? FilterMode.Point : FilterMode.Bilinear);
            texture2D.Apply();
            if (pointFilter)
            {
                texture2D.filterMode = FilterMode.Point;
                texture2D.Apply();
            }

            texture2D.name = fileName;
            texture2D.filterMode = FilterMode.Point;
            texture2D.Apply();
            Rect rect = new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height);
            Sprite sprite = Sprite.Create(texture2D, rect, new Vector2(0.5f, 0.5f), 16f);
            sprite.name = fileName;
            return sprite;
        }
    }
}