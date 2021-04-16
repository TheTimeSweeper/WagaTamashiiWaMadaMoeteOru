using BepInEx;
using RoR2;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using R2API.Utils;
using R2API;

namespace SillyHitboxViewer {

    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync)]
    [BepInDependency("com.bepis.r2api")]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("com.TheTimeSweeper.HitboxViewer", "Silly Hitbox Viewer", "1.3.1")]
    public class HitboxViewerMod : BaseUnityPlugin {

        public static HitboxViewerMod instance;

        public static BepInEx.Logging.ManualLogSource log;

        private List<HitboxGroupRevealer> _hitboxGroupRevealers = new List<HitboxGroupRevealer>();
        private Queue<HitboxRevealer> _revealerPool = new Queue<HitboxRevealer>();
        private static int hitPoolStart = 50;
        private static int totalPool = 0;

        private Queue<HitboxRevealer> _blastPool = new Queue<HitboxRevealer>();
        private static int blastPoolStart = 50;

        private List<HitboxRevealer> _hurtboxRevealers = new List<HitboxRevealer>();
         
        private HitboxRevealer _hitboxBoxPrefab; 
        private HitboxRevealer _hitboxNotBoxPrefab;
        private HitboxRevealer _hitboxNotBoxPrefabTall;

        private bool keysDisable;

        void Awake() {

            instance = this;

            log = Logger;

            populateAss();

            if (_hitboxBoxPrefab == null || _hitboxNotBoxPrefabTall == null || _hitboxNotBoxPrefab == null) {
                log.LogError($"unable to get a hitboxprefab from the bundle. Timesweeper did an oops | box: {_hitboxBoxPrefab != null}, capsule: {_hitboxNotBoxPrefabTall != null}, sphere: {_hitboxNotBoxPrefab != null}");
                return;
            }

            doConfig();

            if (RiskOfOptionsCompat.enabled) {
                RiskOfOptionsCompat.doOptions();
            } else {
                setDefaultOptions();
            }

            CommandHelper.AddToConsoleWhenReady();

            //createPool(hitPoolStart, _revealerPool, false);
            //createPool(blastPoolStart, _blastPool, true); 

            On.RoR2.BodyCatalog.Init += BodyCatalog_Init;

            On.RoR2.OverlapAttack.Fire += OverlapAttack_Fire;

            On.RoR2.BlastAttack.Fire += BlastAttack_Fire;

            On.RoR2.HurtBox.Awake += HurtBox_Awake;
        }

        void Start() {
            if (RiskOfOptionsCompat.enabled)
                RiskOfOptionsCompat.readOptions();
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

        #region options n commands
        //remember the latest settings
        private void setDefaultOptions() {
            HitboxRevealer.showingHitBoxes = PlayerPrefs.GetInt("showHitbox", 1) == 1;
            HitboxRevealer.showingHurtBoxes = PlayerPrefs.GetInt("showHurtbox", 0) == 1;
        }

        [ConCommand(commandName = "show_hitboxes", flags = ConVarFlags.None, helpText = "Enables/disables attack Hitboxes")]
        private static void ShowHitboxes(ConCommandArgs args) {

            int? enabledArg = args.TryGetArgInt(0);

            Debug.LogWarning(args.Count); 
            if (args.Count > 0 && !enabledArg.HasValue) {
                Debug.LogError("ya dun goofed it. Pass in 1 to enable Hitboxes, 0 to disable, or pass in nothing to toggle");
                return;
            }

            bool enabled;

            if (args.Count > 0) {
                enabled = enabledArg == 1;
            } else {
                enabled = !HitboxRevealer.showingHitBoxes;
            }

            setShowingHitboxes(enabled);

            Utils.Log($"showing hitboxes option set to {enabledArg == 1}", true, true);
        }

        [ConCommand(commandName = "show_hurtboxes", flags = ConVarFlags.None, helpText = "Enables/disables character Hurtboxes")]
        private static void ShowHurtboxes(ConCommandArgs args) {

            int? enabledArg = args.TryGetArgInt(0);

            if (args.Count > 0 && !enabledArg.HasValue) {
                Debug.LogError("ya dun fucked up. Pass in 1 to enable Hurtboxes, 0 to disable, or pass in nothing to toggle");
                return;
            }

            bool enabled;

            if (args.Count > 0) {
                enabled = enabledArg == 1;
            } else {
                enabled = !HitboxRevealer.showingHurtBoxes;
            }

            setShowingHurtboxes(enabled, true);

            Utils.Log($"showing hurtboxes option set to {enabledArg == 1}", true, true);
        }

        public static void setShowingHitboxes(bool set) {

            HitboxRevealer.showingHitBoxes = set;
            PlayerPrefs.SetInt("showHitbox", set ? 1 : 0);
        }

        public static void setShowingHurtboxes(bool set, bool showAll) {

            HitboxRevealer.showingHurtBoxes = set;
            PlayerPrefs.SetInt("showHurtbox", set ? 1 : 0);
            if (showAll) {
               HitboxViewerMod.instance.showAllHurtboxes();
            }
        }
        #endregion

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
                            "MercBody, MinerBody, MiniMushroomBody",
                            $"The wacky characters who need softening, separated by commas.\n - In addition to these, the following characters are always on the list: {Utils.defaultSoftenedCharacters}\n - Character's internal names are: CommandoBody, HuntressBody, ToolbotBody, EngiBody, MageBody, MercBody, TreebotBody, LoaderBody, CrocoBody, Captainbody\n - Use the DebugToolkit mod's body_list command to see a complete list (including enemies and moddeds)").Value;

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
                            "press this key to toggle disabling hitbox viewer on and off in game. Overrides current settings menu").Value;

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

        #region hooks
        private void BodyCatalog_Init(On.RoR2.BodyCatalog.orig_Init orig) {
            orig();

            Utils.setSoftenedCharacters();
        }

        private bool OverlapAttack_Fire(On.RoR2.OverlapAttack.orig_Fire orig, OverlapAttack self, List<HurtBox> hitResults) {

            bool didAHit = orig(self, hitResults);

            HitboxGroupRevealer hitboxGroupRevealer = _hitboxGroupRevealers.Find((revealer) => {
                return revealer != null && revealer.hitboxGroup == self.hitBoxGroup;
            });

            if (hitboxGroupRevealer == null) {

                hitboxGroupRevealer = self.hitBoxGroup.gameObject.AddComponent<HitboxGroupRevealer>();
                _hitboxGroupRevealers.Add(hitboxGroupRevealer);

                hitboxGroupRevealer.init(self.hitBoxGroup, _hitboxBoxPrefab,  self.attacker);
            }

            hitboxGroupRevealer.reveal(true);

            return didAHit;
        }

        private BlastAttack.Result BlastAttack_Fire(On.RoR2.BlastAttack.orig_Fire orig, BlastAttack self) {
            BlastAttack.Result result = orig(self);

            HitboxRevealer box = Instantiate(_hitboxNotBoxPrefab).initBlastBox(self.position, self.radius);

            Utils.LogReadout($"making blast hitbox at {self.position}, {self.radius}: {box != null}");

            return result;
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
        #endregion

        #region pool (rip)
        //lot of code copy pasted from the first Hitbox pool i wrote to get the blast hitbox pool working
        //  ideally i'll have a general pool class that'll more gracefully handle both 
        //      (and any new ones (lookin at you, beetles with 10 hurtboxes when there should just be one sphere))

        //alright so apparently pools are less performant (I assume with how i've written them. 
        //  maybe it's my computer and i should test on a weaker computer to really see the difference?)
        //in the distant future 2005 when I decide to try my hand at object pools again i'll revisit and remaster this.
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

        #region toggle and debug
        void Update() {

            //override hotkey
            if (Input.GetKeyDown(Utils.cfg_toggleKey)) {

                HitboxRevealer.showingBoxes = !HitboxRevealer.showingBoxes;

                if (HitboxRevealer.showingBoxes) {
                    Utils.Log("hitboxes enabled", true, true);
                } else {
                    Utils.Log("all hitboxes disabled", true, true);
                }
                showAllHurtboxes();
            }
            if (!Utils.cfg_useDebug)
                return;

            //debug hotkeys
            if (Input.GetKeyDown(KeyCode.Quote)) {
                keysDisable = !keysDisable;
                Utils.Log($"hitbox debug hotkeys toggled {!keysDisable}", true);
                if (keysDisable && Time.timeScale != 1) {
                    setTimeScale(1);
                }
            }

            if (keysDisable)
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

                string log = "";
                int poolCount = _revealerPool.Count;

                HitboxRevealer revealer;
                for (int i = 1; i <= _revealerPool.Count; i++) {
                    revealer = _revealerPool.Dequeue();

                    log += $"\nhitbox {revealer != null}, {i} revealers checked";

                    _revealerPool.Enqueue(revealer);
                }
                Utils.LogReadout($"hitbox pool count: {poolCount}:{log}");

                log = "";
                poolCount = _blastPool.Count;

                for (int i = 1; i <= _blastPool.Count; i++) {

                    revealer = _blastPool.Dequeue();
                    log += $"\nblastbox {revealer != null}, {i} revealers checked";
                    _blastPool.Enqueue(revealer);
                }
                Utils.LogReadout($"blast pool count: {poolCount}:{log}");
            }
        }

        public void showAllHurtboxes() {

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
