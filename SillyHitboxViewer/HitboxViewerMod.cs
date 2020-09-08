using BepInEx;
using RoR2;
using R2API.Utils;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SillyHitboxViewer {

    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync)]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.TheTimeSweeper.HitboxViewer", "Melee Hitbox Viewer", "0.0.1")]
    public class HitboxViewerMod : BaseUnityPlugin {

        private HitboxRevealer _hitboxBoxPrefab;

        private List<HitboxGroupRevealer> _hitboxes = new List<HitboxGroupRevealer>();

        private void populateAss() {
            AssetBundle MainAss = null;
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SillyHitboxViewer.sillyhitbox")) {
                    MainAss = AssetBundle.LoadFromStream(assetStream);
                }

            _hitboxBoxPrefab = MainAss.LoadAsset<GameObject>("hitboxPreviewInator").GetComponent<HitboxRevealer>();
        }

        private void doConfig() {

            #pragma warning disable CS0618 // Type or member is obsolete. sorry I'm lazy
            HitboxRevealer.cfg_BoxAlpha =
                Config.Wrap("be safe",
                            "hitbox alpha",
                            "under 0.5 is about aight",
                            0.23f).Value;
        }

        void Awake () {

            populateAss();
            if (_hitboxBoxPrefab == null) {
                Debug.LogError("hitboxBoxPrefab not assigned. Timesweeper did an oops");
                return;
            }
            doConfig();

            On.RoR2.OverlapAttack.Fire += OverlapAttack_Fire;
        }

        private bool OverlapAttack_Fire(On.RoR2.OverlapAttack.orig_Fire orig, OverlapAttack self, List<HealthComponent> hitResults) {

            bool didAHit = orig(self, hitResults);

            HitboxGroupRevealer hitboxGroupRevealer = _hitboxes.Find((revealer) => {
                return revealer != null && revealer.hitboxGroup == self.hitBoxGroup;
            });

            if (hitboxGroupRevealer == null) {

                hitboxGroupRevealer = self.hitBoxGroup.gameObject.AddComponent<HitboxGroupRevealer>();
                _hitboxes.Add(hitboxGroupRevealer);

                hitboxGroupRevealer.init(self.hitBoxGroup, _hitboxes, _hitboxBoxPrefab);
            }

            hitboxGroupRevealer.reveal(true);

            return didAHit;
        }

        #region you nothing's easy you gotta practice and lose and keep losing and keep losing
        private void BasicMeleeAttack_AuthorityFixedUpdate(On.EntityStates.BasicMeleeAttack.orig_AuthorityFixedUpdate orig, EntityStates.BasicMeleeAttack self) {
            orig(self);
        }

        //old
        private bool BaseState_FireMeleeOverlap(On.EntityStates.BaseState.orig_FireMeleeOverlap orig, 
                                                EntityStates.BaseState self, 
                                                OverlapAttack attack,
                                                Animator animator, 
                                                string mecanimHitboxActiveParameter,
                                                float forceMagnitude, 
                                                bool calculateForceVector) {

            bool hit = orig(self, attack, animator, mecanimHitboxActiveParameter, forceMagnitude, calculateForceVector);

            Debug.Log("uh");

            HitboxGroupRevealer hitboxGroupRevealer = _hitboxes.Find((revealer) => { 
                return revealer != null && revealer.transform == attack.hitBoxGroup.transform; 
            });

            if (hitboxGroupRevealer == null) {

                hitboxGroupRevealer = attack.hitBoxGroup.gameObject.AddComponent<HitboxGroupRevealer>();
                _hitboxes.Add(hitboxGroupRevealer);

                hitboxGroupRevealer.init(attack.hitBoxGroup, _hitboxes, _hitboxBoxPrefab);
            }

            bool hitboxActive = animator && animator.GetFloat(mecanimHitboxActiveParameter) > 0.1f;

            hitboxGroupRevealer.reveal(hitboxActive);

            return hit;
        }

        private void BasicMeleeAttack_OnEnter(On.EntityStates.BasicMeleeAttack.orig_OnEnter orig, EntityStates.BasicMeleeAttack self) {

            orig(self);
        }
        #endregion


        void Update() {
            if (Input.GetKeyDown(KeyCode.U)) {
                HitboxRevealer.cfg_BoxAlpha += 0.01f;
                Chat.AddMessage(HitboxRevealer.cfg_BoxAlpha.ToString());
            }
            if (Input.GetKeyDown(KeyCode.J)) {
                HitboxRevealer.cfg_BoxAlpha -= 0.01f;
                Chat.AddMessage(HitboxRevealer.cfg_BoxAlpha.ToString());
            }
        }

    }
}
