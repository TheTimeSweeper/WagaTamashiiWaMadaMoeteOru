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
    [BepInPlugin("com.TheTimeSweeper.HitboxViewer", "Silly Hitbox Viewer", "1.5.0")]
    public class HitboxViewerMod : BaseUnityPlugin {
        
        public static HitboxViewerMod instance;

        public static BepInEx.Logging.ManualLogSource log;

        #region pools (rip)
        private List<HitboxGroupRevealer> _hitboxGroupRevealers = new List<HitboxGroupRevealer>();
        private Queue<HitboxRevealer> _revealerPool = new Queue<HitboxRevealer>();
        private static int hitPoolStart = 50;
        private static int totalPool = 0;

        private Queue<HitboxRevealer> _blastPool = new Queue<HitboxRevealer>();
        private static int blastPoolStart = 50;
        #endregion

        private List<HitboxRevealer> _hurtboxRevealers = new List<HitboxRevealer>();

        private List<HitboxRevealer> _bulletHitPointRevealers = new List<HitboxRevealer>();

        private HitboxRevealer _hitboxBoxPrefab; 
        private HitboxRevealer _hitboxNotBoxPrefab;
        private HitboxRevealer _hitboxNotBoxPrefabTall;
        private HitboxRevealer _hitboxNotBoxPrefabTallFlat;

        private bool _keysDisable;

        void Awake() {

            instance = this;

            log = Logger;

            populateAss();

            if (_hitboxBoxPrefab == null || _hitboxNotBoxPrefabTall == null || _hitboxNotBoxPrefab == null || _hitboxNotBoxPrefabTallFlat == null) {
                log.LogError($"unable to get a hitboxprefab from the bundle. Timesweeper did an oops | box: {_hitboxBoxPrefab != null}, capsule: {_hitboxNotBoxPrefabTall != null}, sphere: {_hitboxNotBoxPrefab != null}, can: {_hitboxNotBoxPrefabTallFlat != null}");
                return;
            }

            Utils.doConfig();

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

            On.RoR2.BulletAttack.FireSingle += BulletAttack_FireSingle;
            On.RoR2.BulletAttack.InitBulletHitFromRaycastHit += BulletAttack_InitBulletHitFromRaycastHit;

            //initbulletfrom raycasthit
            // spawnblast at origin, radius 1

        }

        void Start() {
            if (RiskOfOptionsCompat.enabled)
                RiskOfOptionsCompat.readOptions();
        }
        #region setup Ass
        private void populateAss() {
            AssetBundle MainAss = null;
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SillyHitboxViewer.sillyhitbox")) {
                    MainAss = AssetBundle.LoadFromStream(assetStream);
                }

            _hitboxBoxPrefab = MainAss.LoadAsset<GameObject>("hitboxPreviewInator")?.GetComponent<HitboxRevealer>();
            _hitboxNotBoxPrefab = MainAss.LoadAsset<GameObject>("hitboxPreviewInatorSphere")?.GetComponent<HitboxRevealer>();
            _hitboxNotBoxPrefabTall = MainAss.LoadAsset<GameObject>("hitboxPreviewInatorCapsule")?.GetComponent<HitboxRevealer>();
            _hitboxNotBoxPrefabTallFlat = MainAss.LoadAsset<GameObject>("hitboxPreviewInatorCylinder")?.GetComponent<HitboxRevealer>();
        }
        #endregion
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

            Utils.Log($"showing hitboxes option set to {enabledArg == 1}", true);
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

            Utils.Log($"showing hurtboxes option set to {enabledArg == 1}", true);
        }

        public static void setShowingHitboxes(bool set) {

            HitboxRevealer.showingHitBoxes = set;
            PlayerPrefs.SetInt("showHitbox", set ? 1 : 0);
        }

        public static void setShowingHurtboxes(bool set, bool showAll) {

            HitboxRevealer.showingHurtBoxes = set;
            PlayerPrefs.SetInt("showHurtbox", set ? 1 : 0);
            if (showAll) {
               HitboxViewerMod.instance.bindShowAllHurtboxes();
            }
        }
        #endregion

        #region hooks
        private void BodyCatalog_Init(On.RoR2.BodyCatalog.orig_Init orig) {
            orig();

            Utils.setSoftenedCharacters();
        }

        //hit box
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

        //hit blast
        private BlastAttack.Result BlastAttack_Fire(On.RoR2.BlastAttack.orig_Fire orig, BlastAttack self) {
            BlastAttack.Result result = orig(self);

            if(!Utils.cfg_showLogsVerbose) {
                //avoiding creating some garbage
                Instantiate(_hitboxNotBoxPrefab).initBlastBox(self.position, self.radius);
                return result;
            }

            HitboxRevealer box = Instantiate(_hitboxNotBoxPrefab).initBlastBox(self.position, self.radius);

            //this still needed?
            Utils.LogReadout($"making blast hitbox at {self.position}, {self.radius}: {box != null}");
            
            return result;
        }

        //hit bullet
        private void BulletAttack_FireSingle(On.RoR2.BulletAttack.orig_FireSingle orig, BulletAttack self, Vector3 normal, int muzzleIndex) {
            orig(self, normal, muzzleIndex);

            Instantiate(_hitboxNotBoxPrefabTallFlat).initBulletBox(self.origin, normal, self.maxDistance, self.radius);
        }

        private void BulletAttack_InitBulletHitFromRaycastHit(On.RoR2.BulletAttack.orig_InitBulletHitFromRaycastHit orig, BulletAttack self, ref BulletAttack.BulletHit bulletHit, Vector3 origin, Vector3 direction, ref RaycastHit raycastHit) {
            orig(self, ref bulletHit, origin, direction, ref raycastHit);

            if (!HitboxRevealer.bulletModeEnabled) {

                Instantiate(_hitboxNotBoxPrefab).initBulletPoint(bulletHit.point, self.radius);
            }

            _bulletHitPointRevealers.Add(Instantiate(_hitboxNotBoxPrefab).initBulletPoint(bulletHit.point, self.radius));

        }

        //hurt
        private void HurtBox_Awake(On.RoR2.HurtBox.orig_Awake orig, HurtBox self) {
            orig(self);

            if (Utils.cfg_unDynamicHurtboxes && (!HitboxRevealer.showingBoxes || !HitboxRevealer.showingHurtBoxes))
                return;

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

        #region toggle and debug
        void Update() {

            //timer to use for randomizing to avoid float rounding errors
            HitboxRevealer.randomTimer += Time.deltaTime;
            if (HitboxRevealer.randomTimer > 100) {
                HitboxRevealer.randomTimer -= 100;
            }

            //show box override hotkey
            if (Input.GetKeyDown(Utils.cfg_toggleKey)) {

                HitboxRevealer.showingBoxes = !HitboxRevealer.showingBoxes;
                Utils.Log(HitboxRevealer.showingBoxes? "hitboxes enabled" : "all hitboxes disabled", true);

                bindShowAllHurtboxes();

                bindClearBulletPoints();
            }

            //clear bullet hits key
            if (Input.GetKeyDown(Utils.cfg_bulletModeKey)) {

                HitboxRevealer.bulletModeEnabled = !HitboxRevealer.bulletModeEnabled;                                             //lol typo
                Utils.Log(HitboxRevealer.bulletModeEnabled? "Lingering Bullet Mode Enabled" : "Lingering Bullets disabaled", true);

                bindClearBulletPoints();
            }

            if (!Utils.cfg_useDebug)
                return;

            //debug hotkeys
            if (Input.GetKeyDown(KeyCode.Quote)) {
                _keysDisable = !_keysDisable;
                Utils.Log($"hitbox debug hotkeys toggled {!_keysDisable}", true);

                if (_keysDisable && Time.timeScale != 1) {
                    setTimeScale(1);
                }
            }

            //ability to disable keys because keyboard focus is on console window
            //  should have just looked up how to check keyboard focus lol
            if (_keysDisable)
                return;

            //time keys
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

            //pool (rip) keys
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

        private void bindClearBulletPoints() {

            bool shouldClearBullets = !HitboxRevealer.showingBoxes || !HitboxRevealer.bulletModeEnabled;

            if (shouldClearBullets) {

                for (int i = 0; i < _bulletHitPointRevealers.Count; i++) {
                    HitboxRevealer pointRevealer = _bulletHitPointRevealers[i];

                    if(pointRevealer != null)
                        pointRevealer.kill();
                }

                _bulletHitPointRevealers = new List<HitboxRevealer>();
            }
        }

        public void bindShowAllHurtboxes() {

            bool shouldShow = HitboxRevealer.showingBoxes && HitboxRevealer.showingHurtBoxes;

            for (int i = _hurtboxRevealers.Count - 1; i >= 0; i--) {
                if (_hurtboxRevealers[i] == null) {
                    _hurtboxRevealers.RemoveAt(i);
                    continue;
                }
                _hurtboxRevealers[i].hurtboxShow(shouldShow);
            }
        }

        private void setTimeScale(float tim) {
            Time.timeScale = tim;

            Utils.Log($"set tim: {Time.timeScale}", true);
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
    }
}
