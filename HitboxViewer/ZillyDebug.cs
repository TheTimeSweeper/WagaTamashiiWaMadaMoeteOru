using BepInEx;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ZillyDebug {

    [R2APISubmoduleDependency(nameof(CommandHelper))]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.TheTimeSweeper.ZillyDebug", "Zilly Debug", "0.0.3")]
    public class ZillyDebugPlugin : BaseUnityPlugin {
    
        private List<HitBox> _seenHitboxes = new List<HitBox>();

        void Awake() {
            Invoke("donworryboutit", 10);
        }

        private void donworryboutit() {

            Debug.Log($"nuking networkModList:\n{NetworkModCompatibilityHelper.networkModList}");

            NetworkModCompatibilityHelper.networkModList = new List<string>();

            Debug.Log($"networkModList nuked:\n{NetworkModCompatibilityHelper.networkModList}");
        }

        //private IEnumerator printBodies() {
        //    Debug.Log("GONNA PRINT BODIES IN 20 SECONDS");
        //    yield return new WaitForSeconds(10);
        //    Debug.Log("GONNA PRINT BODIES IN 10 SECONDS");
        //    yield return new WaitForSeconds(10);
        //    for (int i = 0; i < BodyCatalog.bodyCount; i++) {
        //        Debug.Log($"BODY: {BodyCatalog.GetBodyName(i)}");
        //    }
        //}
    }
}
