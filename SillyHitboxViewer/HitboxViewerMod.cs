using BepInEx;
using RoR2;
using R2API.Utils;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using RiskOfOptions;
using System;

namespace SillyHitboxViewer {

    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync)]
    [BepInDependency("com.bepis.r2api")]
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin("com.TheTimeSweeper.HitboxViewer", "Silly Hitbox Viewer", "0.3.0")]
    public class HitboxViewerMod : BaseUnityPlugin {

        public static HitboxViewerMod instance;

        public static BepInEx.Logging.ManualLogSource log;

        private List<HitboxGroupRevealer> _hitboxGroupRevealers = new List<HitboxGroupRevealer>();
        private Queue<HitboxRevealer> _revealerPool = new Queue<HitboxRevealer>();
        private static int hitPoolStart = 50;
        private static int totalPool = 0;

        private List<HitboxRevealer> _blastRevealers = new List<HitboxRevealer>();
        private Queue<HitboxRevealer> _blastPool = new Queue<HitboxRevealer>();
        private static int blastPoolStart = 50;

        private List<HitboxRevealer> _hurtboxRevealers = new List<HitboxRevealer>();
         
        private HitboxRevealer _hitboxBoxPrefab; 
        private HitboxRevealer _hitboxNotBoxPrefab;
        private HitboxRevealer _hitboxNotBoxPrefabTall;

        private bool keyDisable;

        void Awake() {

            instance = this;

            log = Logger;

            populateAss();

            if (_hitboxBoxPrefab == null || _hitboxNotBoxPrefabTall == null || _hitboxNotBoxPrefab == null) {
                Logger.LogError($"unable to get a hitboxprefab from the bundle. Timesweeper did an oops | {_hitboxBoxPrefab != null}, {_hitboxNotBoxPrefabTall != null}, {_hitboxNotBoxPrefab != null}");
                return;
            }

            doConfig();

            doOptions();

            createPool(hitPoolStart, _revealerPool, false);
            createPool(blastPoolStart, _blastPool, true); 

            On.RoR2.BodyCatalog.Init += BodyCatalog_Init;

            On.RoR2.OverlapAttack.Fire += OverlapAttack_Fire;

            On.RoR2.HurtBox.Awake += HurtBox_Awake;

            On.RoR2.BlastAttack.Fire += BlastAttack_Fire;
        }

        void Start() {
            readOptions();
        }

        private void populateAss() {
            AssetBundle MainAss = null;
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SillyHitboxViewer.sillyhitbox")) {
                    MainAss = AssetBundle.LoadFromStream(assetStream);
                }

            _hitboxBoxPrefab = MainAss.LoadAsset<GameObject>("hitboxPreviewInator")?.GetComponent<HitboxRevealer>();
            _hitboxNotBoxPrefab = MainAss.LoadAsset<GameObject>("hitboxPreviewInatorSphere")?.GetComponent<HitboxRevealer>();
            _hitboxNotBoxPrefabTall = MainAss.LoadAsset<GameObject>("hitboxPreviewInatorCapsule")?.GetComponent<HitboxRevealer>();
        }

        #region config
        private void doConfig() {

            HitboxRevealer.cfg_BoxAlpha =
                Config.Bind("hitboxes",
                            "hitbox alpha",
                            0.22f,
                            "0-1. Around 0.22 is ok. don't make it higher if you have epilepsy").Value;

            HitboxRevealer.cfg_MercSoften =
                Config.Bind("hitboxes",
                            "tone down merc",
                            true,
                            "Make merc's hitboxes lighter cause he's a crazy fool (and might actually hurt your eyes)\n - overrides alpha brightness to 0.1 and keeps colors cool blue-ish range").Value;

            Utils.cfg_softenedCharactersString =
                Config.Bind("hitboxes",
                            "tone-down characters",
                            "MercBody, MinerBody, MiniMushroomBody, NemesisEnforcerBody",
                            "The wacky characters who need softening, separated by commas.\n - Character's internal names are: CommandoBody, HuntressBody, ToolbotBody, EngiBody, MageBody, MercBody, TreebotBody, LoaderBody, CrocoBody, Captainbody\n - Use the DebugToolkit mod's body_list command to see a complete list (including enemies and moddeds)").Value;

            HitboxRevealer.cfg_HurtAlpha =
                Config.Bind("hitboxes",
                            "hurtbox capsule alpha",
                            0.169f,
                            "0-1. Around 0.16 is ok.").Value;

            HitboxRevealer.cfg_BlastShowTime =
                Config.Bind("hitboxes",
                            "blast attack visual time",
                            0.2f,
                            "the amount of time blast hitboxes show up (their actual damage happens in one frame)").Value;

            Utils.cfg_toggleKey =
                Config.Bind("pls be safe",
                            "hitbox toggle Key",
                            KeyCode.Semicolon,
                            "press this key to toggle disabling hitbox viewer on and off in game. Overrides settings in options menu").Value;

            Utils.cfg_useDebug =
                Config.Bind("pls be safe",
                            "debug",
                            false,
                            "welcom 2m y twisted mind\ntimescale hotkeys on I, K, O, and L. press quote key to disable").Value;

            Utils.cfg_showLogsVerbose=
                Config.Bind("pls be safe",
                            "logs",
                            false,
                            "print debug logs").Value;

        }
        #endregion

        #region options
        private void doOptions() {

            ModSettingsManager.setPanelTitle("Hitbox Viewer");
            ModSettingsManager.setPanelDescription("Enable/disable hitbox or hurtbox viewer");

            ModSettingsManager.addOption(new ModOption(ModOption.OptionType.Bool, "Disable Hitboxes", "", "0"));
            ModSettingsManager.addListener(ModSettingsManager.getOption("Disable Hitboxes"), new UnityEngine.Events.UnityAction<bool>(hitboxBoolEvent));

            ModSettingsManager.addOption(new ModOption(ModOption.OptionType.Bool, "Disable Hurtboxes", "", "1"));
            ModSettingsManager.addListener(ModSettingsManager.getOption("Disable Hurtboxes"), new UnityEngine.Events.UnityAction<bool>(hurtboxBoolEvent));
        
        }

        private static void readOptions() {

            string disableHit = ModSettingsManager.getOptionValue("Disable Hitboxes");
            if (!string.IsNullOrEmpty(disableHit)) {
                HitboxRevealer.showingHitBoxes = disableHit == "0";
            }

            string disableHurt = ModSettingsManager.getOptionValue("Disable Hurtboxes");
            if (!string.IsNullOrEmpty(disableHurt)) {
                HitboxRevealer.showingHurtBoxes = disableHurt == "0";
            }
        }

        public void hitboxBoolEvent(bool active) {

            HitboxRevealer.showingHitBoxes = active;
        }
        public void hurtboxBoolEvent(bool active) {

            HitboxRevealer.showingHurtBoxes = active;
            showAllHurtboxes();
        }
        #endregion

        #region hooks
        private void BodyCatalog_Init(On.RoR2.BodyCatalog.orig_Init orig) {
            orig();

            Utils.setSoftenedCharacters();
        }

        private bool OverlapAttack_Fire(On.RoR2.OverlapAttack.orig_Fire orig, OverlapAttack self, List<HealthComponent> hitResults) {

            bool didAHit = orig(self, hitResults);

            HitboxGroupRevealer hitboxGroupRevealer = _hitboxGroupRevealers.Find((revealer) => {
                return revealer != null && revealer.hitboxGroup == self.hitBoxGroup;
            });

            if (hitboxGroupRevealer == null) {

                hitboxGroupRevealer = self.hitBoxGroup.gameObject.AddComponent<HitboxGroupRevealer>();
                _hitboxGroupRevealers.Add(hitboxGroupRevealer);

                hitboxGroupRevealer.init(self.hitBoxGroup, self.attacker);
            }

            hitboxGroupRevealer.reveal(true);

            return didAHit;
        }

        private void HurtBox_Awake(On.RoR2.HurtBox.orig_Awake orig, HurtBox self) {
            orig(self);

            if (self.collider is CapsuleCollider) {
                _hurtboxRevealers.Add(Instantiate(_hitboxNotBoxPrefabTall).initHurtbox(self.collider.transform, self.collider as CapsuleCollider));
            }
            if (self.collider is SphereCollider) {
                _hurtboxRevealers.Add(Instantiate(_hitboxNotBoxPrefab).initHurtbox(self.collider.transform, self.collider as SphereCollider));
            }
            if (self.collider is BoxCollider) {
                _hurtboxRevealers.Add(Instantiate(_hitboxBoxPrefab).initHurtbox(self.collider.transform, self.collider as BoxCollider));
            }
        }

        private BlastAttack.Result BlastAttack_Fire(On.RoR2.BlastAttack.orig_Fire orig, BlastAttack self) {
            BlastAttack.Result result = orig(self);

            HitboxRevealer box = requestPooledBlastRevealer().initBlastBox(self.position, self.radius);

            Utils.LogReadout($"making blast hitbox at {self.position}, {self.radius}: {box != null}");

            return result;
        }
        #endregion

        #region pool
        //lot of code copy pasted from the first Hitbox pool i wrote to get the blast hitbox pool working
        //  ideally i'll have a general pool class that'll more gracefully handle both 
        //      (and any new ones (lookin at you, beetles with 10 hurtboxes (when there should just be one sphere)))
        private void createPool(int poolStart, Queue<HitboxRevealer> poolQueue, bool blast) {

            for (int i = 0; i < poolStart; i++) {
                createPooledRevealer(poolQueue, blast);
            }
        }

        private void createPooledRevealer(Queue<HitboxRevealer> poolQueue, bool blast) {

            HitboxRevealer rev = Instantiate(blast ? _hitboxNotBoxPrefab : _hitboxBoxPrefab, transform);

            poolQueue.Enqueue(rev);
            //totalPool++;
        }

        public HitboxRevealer requestPooledHitboxRevealer() {
            return requestPooledRevealer(_revealerPool, false);
        }

        public HitboxRevealer requestPooledBlastRevealer() {
            return requestPooledRevealer(_blastPool, true);
        }

        private HitboxRevealer requestPooledRevealer(Queue<HitboxRevealer> poolQueue, bool blast) {

            if (poolQueue.Count <= 0) {
                instance.createPooledRevealer(poolQueue, blast);

                Utils.LogWarning($"requestPooledRevealer: pool full. adding revealer to total");
            }

            HitboxRevealer revealer = poolQueue.Dequeue();

            if (revealer == null) {
                Utils.LogWarning($"requestPooledRevealer: pooled revealer is null. trying again");
                return requestPooledRevealer(poolQueue, blast);
            }

            return revealer;
        }

        public void returnPooledHitboxRevealers(HitboxRevealer[] revs) {

            for (int i = 0; i < revs.Length; i++) {
                returnPooledRevealer(revs[i], _revealerPool);
            }
        }

        public void returnPooledBlastRevealer(HitboxRevealer rev) {

            returnPooledRevealer(rev, _blastPool);
        }

        private void returnPooledRevealers(HitboxRevealer[] revs, Queue<HitboxRevealer> poolQueue) {
            //if revs[i] == null count killed revealers
            for (int i = 0; i < revs.Length; i++) {
                returnPooledRevealer(revs[i], poolQueue);
            }
        }

        private void returnPooledRevealer(HitboxRevealer rev, Queue<HitboxRevealer> poolQueue) {
            rev.transform.parent = instance.transform;
            poolQueue.Enqueue(rev);
        }

        public void removeHitBoxGroupRevealer(HitboxGroupRevealer rev) {
            _hitboxGroupRevealers.Remove(rev);
        }
        #endregion

        #region debug
        void Update() {

            if (Input.GetKeyDown(Utils.cfg_toggleKey)) {
                HitboxRevealer.showingBoxes = !HitboxRevealer.showingBoxes;
                if (HitboxRevealer.showingBoxes) {
                    Utils.Log($"hitboxes enabled", true);
                } else {
                    Utils.Log($"all hitboxes disabled", true);
                }
                showAllHurtboxes();
            }

            if (!Utils.cfg_useDebug)
                return;

            if (Input.GetKeyDown(KeyCode.Quote)) {
                keyDisable = !keyDisable;
                Utils.Log($"hitbox debug hotkeys toggled {!keyDisable}", true);
                if (Time.timeScale != 1) {
                    setTimeScale(1);
                }
            }

            if (keyDisable)
                return;

            if (Input.GetKeyDown(KeyCode.I)) {
                if (Time.timeScale == 0) {
                    setTimeScale(Time.timeScale + 0.1f);
                } else {
                    setTimeScale(Time.timeScale + 0.5f);
                }
            }
            if (Input.GetKeyDown(KeyCode.K)) {

                setTimeScale(Time.timeScale - 0.1f);
            }
            if (Input.GetKeyDown(KeyCode.O)) {
                setTimeScale(1);
            }
            if (Input.GetKeyDown(KeyCode.L)) {
                setTimeScale(0);
            }

            if (Input.GetKeyDown(KeyCode.P)) {
                Utils.LogReadout($"pool count: {_revealerPool.Count}");
                HitboxRevealer rev;
                for (int i = 1; i <= _revealerPool.Count; i++) {
                    rev = _revealerPool.Dequeue();
                    Utils.LogReadout($"hitbox {rev != null}, {i} revs checked ");
                    _revealerPool.Enqueue(rev);
                }

                Utils.LogReadout($"blast pool count: {_blastPool.Count}");
                for (int i = 1; i <= _blastPool.Count; i++) {
                    rev = _blastPool.Dequeue();
                    Utils.LogReadout($"blastbox {rev != null}, {i} revs checked ");
                    _blastPool.Enqueue(rev);
                }
            }
        }

        private void showAllHurtboxes() {

            bool shouldShow = HitboxRevealer.showingBoxes && HitboxRevealer.showingHurtBoxes;

            for (int i = _hurtboxRevealers.Count - 1; i >= 0; i--) {
                if (_hurtboxRevealers[i] == null) {
                    _hurtboxRevealers.RemoveAt(i);
                    continue;
                }
                _hurtboxRevealers[i].showHurtboxes(shouldShow);
            }
        }

        private void setTimeScale(float tim) {
            Time.timeScale = tim;

            Utils.Log($"set tim: {Time.timeScale}", true);
        }
        #endregion
    }
}
