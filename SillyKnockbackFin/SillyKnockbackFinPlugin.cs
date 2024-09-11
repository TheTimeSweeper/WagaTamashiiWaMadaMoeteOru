using BepInEx;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using BepInEx.Configuration;

namespace SillyKnockbackFin
{
    [BepInPlugin("com.TheTimeSweeper.SillyKnockbackFinP", "Silly Knockback Fin", "1.0.0")]
    public class SillyKnockbackFinPlugin : BaseUnityPlugin
    {
        ConfigEntry<float> knockback;

        public static SillyKnockbackFinPlugin instance;

        void Awake()
        {
            instance = this;
            knockback = SillyKnockbackFin.Config.BindAndOptions("hi", "multiplier", -4f, -100, 100, "multiplier for knockback", false);
            
            IL.RoR2.GlobalEventManager.ProcessHitEnemy += GlobalEventManager_ProcessHitEnemy;
        }

        private void GlobalEventManager_ProcessHitEnemy(MonoMod.Cil.ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            cursor.GotoNext(MoveType.After,
                instruction => instruction.MatchCallvirt<CharacterMotor>("ApplyForce"),
                Instruction => Instruction.MatchLdloc(out _),
                Instruction => Instruction.MatchLdloc(out _),
                Instruction => Instruction.MatchLdcR4(2f),
                Instruction => Instruction.MatchDiv()
                );
            cursor.Index--;
            cursor.EmitDelegate<Func<float, float>>((two) => { return knockback.Value; });
        }
    }
}