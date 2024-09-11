using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;

namespace SillyDeltaTime
{

    [BepInPlugin("com.TheTimeSweeper.SillyDeltaTime", "Silly deltaTime", "1.0.0")]
    public class SillyDeltaTimePlugin : BaseUnityPlugin
    {
        public static SillyDeltaTimePlugin instance;

        void Awake()
        {
            instance = this;
            //ConfigureHook("Halcyonite WhilwindRush", "hopefully fixes them flying across the fuckin world", true, () =>
            //{
            //    IL.EntityStates.Halcyonite.WhirlwindRush.HandleIdentifySafePathForward += SwapTime;
            //});
            ConfigureHook("SteppedSkillDef StepGrace", "affects how fast combos reset if not held. very minor", true, () =>
            {
                IL.RoR2.Skills.SteppedSkillDef.OnFixedUpdate += SwapTime;
            });
            ConfigureHook("MercDash Reset Timer", "fixes merc dash time until re-dashing dependent on frames", true, () =>
            {
                IL.RoR2.Skills.MercDashSkillDef.OnFixedUpdate += SwapTime;
            });
            ConfigureHook("ReloadSkillDef graceTime", "affects time it takes for reload skills (bandit) to happen", true, () =>
            {
                IL.RoR2.Skills.ReloadSkillDef.OnFixedUpdate += SwapTime;
            });

            ConfigureHook("Character AI", "not sure what this effects but it has the same code issue as the others. untested so disable by default. if someone can verify this is good let me know", false, () =>
            {
                IL.RoR2.CharacterAI.BaseAI.ManagedFixedUpdate += SwapTime;
                IL.RoR2.CharacterAI.BaseAI.UpdateBodyAim += SwapTime;
            });
        }

        private static void SwapTime(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            cursor.GotoNext(MoveType.After,
                instruction => instruction.MatchCall<Time>("get_fixedDeltaTime")
                );
            cursor.EmitDelegate<Func<float, float>>((toime) =>
            {
                return Time.deltaTime;
            });
        }

        public void ConfigureHook(string hookName, string description, bool bydefault, Action performHook)
        {
            ConfigEntry<bool> nip = Configuration.BindAndOptions("NEVA GIV UP", hookName, bydefault, description, true);
            if (nip.Value)
            {
                performHook();
            }
        }
    }
}
