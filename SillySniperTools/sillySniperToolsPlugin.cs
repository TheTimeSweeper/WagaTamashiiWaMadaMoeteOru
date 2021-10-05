using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using System.Security;
using System.Security.Permissions;
using R2API.Utils;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace SillySniperTools
{
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync)]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.TheTimeSweeper.MachinimaThingys", "Silly Machinima Tools", "6.9.6.9")]
    public class sillySniperToolsPlugin : BaseUnityPlugin {

        public void Awake() {
            On.EntityStates.Sniper.Scope.ScopeSniper.OnEnter += ScopeSniper_OnEnter;
        }

        private void ScopeSniper_OnEnter(On.EntityStates.Sniper.Scope.ScopeSniper.orig_OnEnter orig, EntityStates.Sniper.Scope.ScopeSniper self) {
            orig(self);
            self.laserPointerObject.SetActive(false);
        }
    }
}
