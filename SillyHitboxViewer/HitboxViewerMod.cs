using BepInEx;
using RoR2;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using System.Security.Permissions;
using System.Security;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[assembly: HG.Reflection.SearchableAttribute.OptIn]

namespace SillyHitboxViewer {

    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("com.TheTimeSweeper.HitboxViewer", "Silly Hitbox Viewer", "1.5.5")]
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

        private List<HitboxRevealer> _kinoRevealers = new List<HitboxRevealer>();

        private List<HitboxRevealer> _bulletHitPointRevealers = new List<HitboxRevealer>();

        private HitboxRevealer _hitboxBoxPrefab; 
        private HitboxRevealer _hitboxNotBoxPrefab;
        private HitboxRevealer _hitboxNotBoxPrefabSmol;
        private HitboxRevealer _hitboxNotBoxPrefabTall;
        private HitboxRevealer _hitboxNotBoxPrefabTallFlat;
        private HitboxRevealer _hitboxNotBoxPrefabTallFlatSmol;

        void Awake() {
            instance = this;

            log = Logger;

            populateAss();

            Utils.doConfig();
            Utils.doHitbox.SettingChanged += DoHitbox_SettingChanged;
            Utils.doHurtbox.SettingChanged += DoHurtbox_SettingChanged;
            Utils.doKinos.SettingChanged += DoKinos_SettingChanged;

            setShowingHitboxes(Utils.cfg_doHitbox);
            setShowingHurtboxes(Utils.cfg_doHurtbox, false);
            setShowingKinos(Utils.cfg_doKinos, false);

            if (RiskOfOptionsCompat.enabled) {
                RiskOfOptionsCompat.doOptions();
            }

            //createPool(hitPoolStart, _revealerPool, false);
            //createPool(blastPoolStart, _blastPool, true);

            On.RoR2.BodyCatalog.Init += BodyCatalog_Init;

            On.RoR2.OverlapAttack.Fire += OverlapAttack_Fire;

            On.RoR2.BlastAttack.Fire += BlastAttack_Fire;

            On.RoR2.HurtBox.Awake += HurtBox_Awake;

            On.RoR2.BulletAttack.FireSingle += BulletAttack_FireSingle; ;
            On.RoR2.BulletAttack.InitBulletHitFromRaycastHit += BulletAttack_InitBulletHitFromRaycastHit;

            On.RoR2.CharacterMotor.Awake += CharacterMotor_Awake;
        }

        private void DoHitbox_SettingChanged(object sender, EventArgs e) {
            setShowingHitboxes(Utils.cfg_doHitbox);
        }

        private void DoHurtbox_SettingChanged(object sender, EventArgs e) {
            setShowingHurtboxes(Utils.cfg_doHurtbox, true);
        }

        private void DoKinos_SettingChanged(object sender, EventArgs e) {
            setShowingKinos(Utils.cfg_doKinos, true);
        }

        #region setup Ass
        private void populateAss() {

            AssetBundle MainAss = null;
            using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SillyHitboxViewer.sillyhitbox")) {
                MainAss = AssetBundle.LoadFromStream(assetStream);
            }

            _hitboxBoxPrefab = LoadHitboxAss(MainAss, "hitboxPreviewInator");
            _hitboxNotBoxPrefab = LoadHitboxAss(MainAss,"hitboxPreviewInatorSphere");
            _hitboxNotBoxPrefabSmol = LoadHitboxAss(MainAss, "hitboxPreviewInatorSphereSmol");
            _hitboxNotBoxPrefabTall = LoadHitboxAss(MainAss,"hitboxPreviewInatorCapsule");
            _hitboxNotBoxPrefabTallFlat = LoadHitboxAss(MainAss, "hitboxPreviewInatorCylinder");
            _hitboxNotBoxPrefabTallFlatSmol = LoadHitboxAss(MainAss, "hitboxPreviewInatorCylinderSmol");
            RiskOfOptionsCompat.icon = MainAss.LoadAsset<Sprite>("hitboxIcon");

        }

        private static HitboxRevealer LoadHitboxAss(AssetBundle MainAss, string assString) {
            HitboxRevealer loadedAss = MainAss.LoadAsset<GameObject>(assString)?.GetComponent<HitboxRevealer>();

            if(loadedAss == null) {
                log.LogError($"unable to load prefab, {assString}, from the bundle. Timesweeper did an oops");
            }

            return loadedAss?.GetComponent<HitboxRevealer>();
        }
        #endregion

        #region commands
        [ConCommand(commandName = "show_hitbox", flags = ConVarFlags.None, helpText = "Enables/disables attack Hitboxes")]
        private static void ShowHitbox(ConCommandArgs args) => ShowHitboxes(args);

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

            Utils.setHitboxConfig(enabled);

            Utils.Log($"showing hitboxes option set to {enabledArg == 1}", true);
        }

        [ConCommand(commandName = "show_hurtbox", flags = ConVarFlags.None, helpText = "Enables/disables character Hurtboxes")]
        private static void ShowHurtbox(ConCommandArgs args) => ShowHurtboxes(args);

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

            Utils.setHurtboxConfig(enabled);

            Utils.Log($"showing hurtboxes option set to {enabledArg == 1}", true);
        }


        [ConCommand(commandName = "show_kinos", flags = ConVarFlags.None, helpText = "Enables/disables character motors")]
        private static void ShowKino(ConCommandArgs args) => ShowKinos(args);

        [ConCommand(commandName = "show_characterMotors", flags = ConVarFlags.None, helpText = "Enables/disables character motors")]
        private static void ShowKinos(ConCommandArgs args) {

            int? enabledArg = args.TryGetArgInt(0);

            if (args.Count > 0 && !enabledArg.HasValue) {
                Debug.LogError("ya dun fucked up. Pass in 1 to enable Hurtboxes, 0 to disable, or pass in nothing to toggle");
                return;
            }

            bool enabled;

            if (args.Count > 0) {
                enabled = enabledArg == 1;
            } else {
                enabled = !HitboxRevealer.showingKinos;
            }

            Utils.setKinoConfig(enabled);

            Utils.Log($"showing Character Motors option set to {enabledArg == 1}", true);
        }

        #endregion commands

        public static void setShowingHitboxes(bool set) {
            //can make this bool a property that reads the config now, but don't sneed to
            HitboxRevealer.showingHitBoxes = set;
        }

        public static void setShowingHurtboxes(bool set, bool showAll) {

            HitboxRevealer.showingHurtBoxes = set;
            if (showAll) {
               HitboxViewerMod.instance.bindShowAllHurtboxes();
            }
        }

        public static void setShowingKinos(bool set, bool showAll) {

            HitboxRevealer.showingKinos = set;
            if (showAll) {
                HitboxViewerMod.instance.bindShowAllKinos();
            }
        }

        #region hooks
        private System.Collections.IEnumerator BodyCatalog_Init(On.RoR2.BodyCatalog.orig_Init orig)
        {
            var nip = orig();

            Utils.setSoftenedCharacters();
            return nip;
        }

        //hit box
        private bool OverlapAttack_Fire(On.RoR2.OverlapAttack.orig_Fire orig, OverlapAttack self, List<HurtBox> hitResults) {

            bool didAHit = orig(self, hitResults);

            HitboxGroupRevealer hitboxGroupRevealer = _hitboxGroupRevealers.Find((revealer) => {
                return revealer != null && revealer.hitboxGroup == self.hitBoxGroup;
            });

            if (hitboxGroupRevealer == null) {

                if(self.hitBoxGroup == null)
                {
                    Logger.LogError("could not show hitbox. hitboxgroup is null");
                    return didAHit;
                }

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
                //avoiding an extra reference creating some garbage
                Instantiate(self.radius < 0.1f ? _hitboxNotBoxPrefabSmol : _hitboxNotBoxPrefab).initBlastBox(self.position, self.radius);
                return result;
            }

            HitboxRevealer box = Instantiate(self.radius < 0.1f ? _hitboxNotBoxPrefabSmol : _hitboxNotBoxPrefab).initBlastBox(self.position, self.radius);

            //this still needed?
            //Utils.LogReadout($"making blast hitbox at {self.position}, {self.radius}: {box != null}");
            
            return result;
        }

        //hit bullet
        private void BulletAttack_FireSingle(On.RoR2.BulletAttack.orig_FireSingle orig, BulletAttack self, BulletAttack.FireSingleArgs args)
        {
            orig(self, args);

            Instantiate(self.radius < 0.1f ? _hitboxNotBoxPrefabTallFlatSmol : _hitboxNotBoxPrefabTallFlat).initBulletBox(self.origin, args.ray.direction, self.maxDistance, self.radius);
        }

        private void BulletAttack_InitBulletHitFromRaycastHit(On.RoR2.BulletAttack.orig_InitBulletHitFromRaycastHit orig, BulletAttack self, ref BulletAttack.BulletHit bulletHit, Ray ray, ref RaycastHit raycastHit)
        {
            orig(self, ref bulletHit, ray, ref raycastHit);

            if (Vector3.Distance(self.origin, bulletHit.point) < 0.5f)
                return;

            if (!HitboxRevealer.bulletModeEnabled) {

                Instantiate(self.radius < 0.1f? _hitboxNotBoxPrefabSmol : _hitboxNotBoxPrefab).initBulletPoint(bulletHit.point, self.radius);
                //Instantiate(_hitboxNotBoxPrefabSmol).initBulletPoint(bulletHit.point, 0.1f);
            }

            //code repeated to avoid uncesseary allocation of a new gameobject in memory
                //immediately point and laugh at me publicly if that is wrong so I know
            _bulletHitPointRevealers.Add(Instantiate(_hitboxNotBoxPrefabSmol).initBulletPoint(bulletHit.point, 0.1f));

        }

        //hurt
        private void HurtBox_Awake(On.RoR2.HurtBox.orig_Awake orig, HurtBox self) {
            orig(self);

            if (Utils.cfg_unDynamicHurtboxes && (!HitboxRevealer.showingAnyBoxes || !HitboxRevealer.showingHurtBoxes))
                return;

            if (self.collider is CapsuleCollider) {
                _hurtboxRevealers.Add(Instantiate(_hitboxNotBoxPrefabTall).initHurtbox(self.collider.transform, self.collider as CapsuleCollider));
            }
            if (self.collider is SphereCollider) {
                _hurtboxRevealers.Add(Instantiate(_hitboxNotBoxPrefab).initHurtbox(self.collider.transform, self.collider as SphereCollider));

                Utils.LogWarning($"intititating {self.hurtBoxGroup.gameObject.name}");
            }
            if (self.collider is BoxCollider) {
                _hurtboxRevealers.Add(Instantiate(_hitboxBoxPrefab).initHurtbox(self.collider.transform, self.collider as BoxCollider));
            }
        }

        private void CharacterMotor_Awake(On.RoR2.CharacterMotor.orig_Awake orig, CharacterMotor self) {
            orig(self);

            if (Utils.cfg_unDynamicHurtboxes && (!HitboxRevealer.showingAnyBoxes || !HitboxRevealer.showingKinos))
                return;

            _kinoRevealers.Add(Instantiate(_hitboxNotBoxPrefabTall).initKino(self.Motor.Capsule.transform, self.Motor.Capsule));
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

                HitboxRevealer.showingAnyBoxes = !HitboxRevealer.showingAnyBoxes;
                Utils.Log(HitboxRevealer.showingAnyBoxes? "hitboxes enabled" : "all hitboxes disabled", true);

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

            if(RoR2.Console.instance && RoR2.Console.instance.enabled)
            {
                for (int i = 0; i < LocalUserManager.readOnlyLocalUsersList.Count; i++)
                {
                    if (LocalUserManager.readOnlyLocalUsersList[i] != null && LocalUserManager.readOnlyLocalUsersList[i].currentNetworkUser && LocalUserManager.readOnlyLocalUsersList[i].currentNetworkUser.localPlayerAuthority)
                    {
                        if (LocalUserManager.readOnlyLocalUsersList[i].isUIFocused)
                            return;
                    }
                }
            }

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

            bool shouldClearBullets = !HitboxRevealer.showingAnyBoxes || !HitboxRevealer.bulletModeEnabled;

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

            bool shouldShow = HitboxRevealer.showingAnyBoxes && HitboxRevealer.showingHurtBoxes;

            for (int i = _hurtboxRevealers.Count - 1; i >= 0; i--) {
                if (_hurtboxRevealers[i] == null) {
                    _hurtboxRevealers.RemoveAt(i);
                    continue;
                }
                _hurtboxRevealers[i].hurtboxShow(shouldShow);
            }
        }

        //that's some tasty copy pasta mamma mia
        public void bindShowAllKinos() {

            bool shouldShow = HitboxRevealer.showingAnyBoxes && HitboxRevealer.showingKinos;

            for (int i = _kinoRevealers.Count - 1; i >= 0; i--) {
                if (_kinoRevealers[i] == null) {
                    _kinoRevealers.RemoveAt(i);
                    continue;
                }
                _kinoRevealers[i].kinoShow(shouldShow);
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
