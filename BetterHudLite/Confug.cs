using BepInEx.Configuration;


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
                plugin.Config.Bind<bool>(barSection,
                        "Do the health bar",
                        true,
                        "Brings the health and level bar to the center");

            healthBarWidth =
                plugin.Config.Bind<float>(barSection,
                        "Health Bar Width Factor",
                        1f,
                        "size of health bar, range 0 to 4.5, 0 making it nonexistent, 4.5 making it cover the screen\n"
                        + "less than 0 just shifts it to the right, higher than 4.5 goes off screen obviously").Value;

            healthBarHeight =
                plugin.Config.Bind<float>(barSection,
                        "Health Bar Height Multiplier",
                        1f,
                        "multiply health bar height by this value\n"
                        + "expands both up and down. attempts to move skills with it").Value;

            string skillsSection = "Skills UI";

            _doSkills =
                plugin.Config.Bind<bool>(skillsSection,
                        "Do the skills",
                        true,
                        "Brings the skills to the center above where the health bar is (if that's also enabled)");

            
        }
    }
}