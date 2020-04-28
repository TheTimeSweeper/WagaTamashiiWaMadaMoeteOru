using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates;
using R2API.Utils;
using System.Collections;

namespace SillyDebug {

    [R2APISubmoduleDependency(nameof(CommandHelper))]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.TheTimeSweeper.SillyDebug", "Silly Debug", "0.0.3")]
    public class SillyDebugPlugin : BaseUnityPlugin {

        private List<HitBox> _seenHitboxes = new List<HitBox>();

        public void Awake() {
        }

        public void Update() {
        }

        private void ShowHitboxes() {
            HitBox[] allHitboxes = FindObjectsOfType<HitBox>();

            for (int i = 0; i < allHitboxes.Length; i++) {

                if (_seenHitboxes.Contains(allHitboxes[i]))
                    continue;

                Transform hitboxBox = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                Destroy(hitboxBox.GetComponent<Collider>());

                hitboxBox.SetParent(allHitboxes[i].transform, false);
                hitboxBox.localScale = Vector3.one;

                _seenHitboxes.Add(allHitboxes[i]);
            }
        }

        private IEnumerator printBodies() {
            Debug.Log("GONNA PRINT BODIES IN 20 SECONDS");
            yield return new WaitForSeconds(10);
            Debug.Log("GONNA PRINT BODIES IN 10 SECONDS");
            yield return new WaitForSeconds(10);
            for (int i = 0; i < BodyCatalog.bodyCount; i++) {
                Debug.Log($"BODY: {BodyCatalog.GetBodyName(i)}");
            }
        }
    }
}
