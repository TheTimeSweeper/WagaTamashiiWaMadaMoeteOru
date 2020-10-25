using BepInEx;
using RoR2;
using R2API.Utils;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;

namespace SillyHitboxViewer {

    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync)]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.TheTimeSweeper.HitboxViewer", "Melee Hitbox Viewer", "0.0.1")]
    public class HitboxViewerMod : BaseUnityPlugin {

        public static Queue<HitboxRevealer> _revealerPool;

        public static HitboxViewerMod instance;

        private HitboxRevealer _hitboxBoxPrefab;

        private HitboxRevealer _hitboxBoxPrefabSphere;

        private List<HitboxGroupRevealer> _hitboxGroupRevealers = new List<HitboxGroupRevealer>();

        private static int poolStart = 100;
        private static int totalPool = 0;

        private void populateAss() {
            AssetBundle MainAss = null;
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SillyHitboxViewer.sillyhitbox")) {
                    MainAss = AssetBundle.LoadFromStream(assetStream);
                }

            _hitboxBoxPrefab = MainAss.LoadAsset<GameObject>("hitboxPreviewInator").GetComponent<HitboxRevealer>();
            _hitboxBoxPrefab = MainAss.LoadAsset<GameObject>("hitboxPreviewInatorSphere").GetComponent<HitboxRevealer>();
        }

        private void doConfig() {

            #pragma warning disable CS0618 // Type or member is obsolete. sorry I'm lazy
            HitboxRevealer.cfg_BoxAlpha =
                Config.Wrap("be safe",
                            "hitbox alpha",
                            "around 0.2 is ok. don't make it higher if you have epilepsy",
                            0.22f).Value;
            HitboxRevealer.MercSoften =
                Config.Wrap("be safe",
                            "tone down merc",
                            "make merc's hitboxes lighter cause he's a crazy fool (and might actually hurt your eyes)",
                            true).Value;
            Utils.useDebug =
                Config.Wrap("be safe",
                            "debug",
                            "welcom 2m y twisted mind",
                            false).Value;
        }

        void Awake () {

            instance = this;

            populateAss();
            if (_hitboxBoxPrefab == null) {
                Debug.LogError("hitboxBoxPrefab not assigned. Timesweeper did an oops");
                return;
            }

            doConfig();

            createPool();

            On.RoR2.OverlapAttack.Fire += OverlapAttack_Fire;
        }

        private bool OverlapAttack_Fire(On.RoR2.OverlapAttack.orig_Fire orig, OverlapAttack self, List<HealthComponent> hitResults) {

            bool didAHit = orig(self, hitResults);

            HitboxGroupRevealer hitboxGroupRevealer = _hitboxGroupRevealers.Find((revealer) => {
                return revealer != null && revealer.hitboxGroup == self.hitBoxGroup;
            });

            if (hitboxGroupRevealer == null) {

                hitboxGroupRevealer = self.hitBoxGroup.gameObject.AddComponent<HitboxGroupRevealer>();
                _hitboxGroupRevealers.Add(hitboxGroupRevealer);

                hitboxGroupRevealer.init(self.hitBoxGroup, _hitboxGroupRevealers);
            }

            hitboxGroupRevealer.reveal(true);

            return didAHit;
        }

        private void createPool() {

            for (int i = 0; i < poolStart; i++) {
                createPooledRevealer();
            }
        }

        private void createPooledRevealer() {

            HitboxRevealer rev = Instantiate(_hitboxBoxPrefab, transform);

            _revealerPool.Enqueue(rev);
            totalPool++;
        }

        public static HitboxRevealer requestPooledRevealer() {

            if (_revealerPool.Count <= 0) {
                instance.createPooledRevealer();
                Utils.Log($"pool full. adding {totalPool}");

            }

            return _revealerPool.Dequeue();
        }

        public static void returnPooledRevealers(HitboxRevealer[] revs) {
            //if revs[i] == null count killed revealers
            for (int i = 0; i < revs.Length; i++) {
                returnPooledRevealer(revs[i]);
            }
        }

        public static void returnPooledRevealer(HitboxRevealer rev) {
            rev.transform.parent = transform;
            _revealerPool.Enqueue(rev);
        }

        #region well nothing's easy you gotta practice and lose and keep losing and keep losing
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

            HitboxGroupRevealer hitboxGroupRevealer = _hitboxGroupRevealers.Find((revealer) => { 
                return revealer != null && revealer.transform == attack.hitBoxGroup.transform; 
            });

            if (hitboxGroupRevealer == null) {

                hitboxGroupRevealer = attack.hitBoxGroup.gameObject.AddComponent<HitboxGroupRevealer>();
                _hitboxGroupRevealers.Add(hitboxGroupRevealer);

                hitboxGroupRevealer.init(attack.hitBoxGroup, _hitboxGroupRevealers, _hitboxBoxPrefab);
            }

            bool hitboxActive = animator && animator.GetFloat(mecanimHitboxActiveParameter) > 0.1f;

            hitboxGroupRevealer.reveal(hitboxActive);

            return hit;
        }

        private void BasicMeleeAttack_OnEnter(On.EntityStates.BasicMeleeAttack.orig_OnEnter orig, EntityStates.BasicMeleeAttack self) {

            orig(self);
        }
        #endregion

        #region debug
        void Update() {

            if (!Utils.useDebug)
                return;

            if (Input.GetKeyDown(KeyCode.I)) {

                setTimeScale(Time.timeScale + 0.1f);

                //HitboxRevealer.cfg_BoxAlpha += 0.01f;
                //Chat.AddMessage(HitboxRevealer.cfg_BoxAlpha.ToString());
            }
            if (Input.GetKeyDown(KeyCode.K)) {

                setTimeScale(Time.timeScale - 0.1f);

                //HitboxRevealer.cfg_BoxAlpha -= 0.01f;
                //Chat.AddMessage(HitboxRevealer.cfg_BoxAlpha.ToString());
            }
            if (Input.GetKeyDown(KeyCode.O)) {
                setTimeScale(1);
            }
            if (Input.GetKeyDown(KeyCode.L)) {
                setTimeScale(0);
            }
        }

        private void setTimeScale(float tim) {
            Time.timeScale = tim;

            Chat.AddMessage($"tim: {Time.timeScale}");
        }
        #endregion
    }
}
