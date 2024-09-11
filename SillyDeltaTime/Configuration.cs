using RoR2;
using BepInEx.Configuration;
using System.Runtime.CompilerServices;
using UnityEngine;
using RiskOfOptions;
using RiskOfOptions.Options;
using RiskOfOptions.OptionConfigs;
using RoR2.Skills;
using SillyDeltaTime;
using System.IO;
using Path = System.IO.Path;

internal static partial class Configuration
{
    private static bool loadedIcon;

    public static ConfigEntry<T> BindAndOptions<T>(string section, string name, T defaultValue, string description = "", bool restartRequired = false) =>
        BindAndOptions(section, name, defaultValue, 0, 20, description, restartRequired);
    public static ConfigEntry<T> BindAndOptions<T>(string section, string name, T defaultValue, float min, float max, string description = "", bool restartRequired = false)
    {
        if (string.IsNullOrEmpty(description))
        {
            description = name;
        }
        description += $"\nDefault: {defaultValue}";
        if (restartRequired)
        {
            description += " (restart required)";
        }
        ConfigEntry<T> configEntry = SillyDeltaTimePlugin.instance.Config.Bind(section, name, defaultValue, description);

        if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions"))
        {
            TryRegisterOption(configEntry, min, max, restartRequired);
        }

        return configEntry;
    }

    //back compat
    public static ConfigEntry<float> BindAndOptionsSlider(string section, string name, float defaultValue, string description, float min = 0, float max = 20, bool restartRequired = false) =>
        BindAndOptions(section, name, defaultValue, min, max, description, restartRequired);

    //add risk of options dll to your project libs and uncomment this for a soft dependency
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private static void TryRegisterOption<T>(ConfigEntry<T> entry, float min, float max, bool restartRequired)
    {
        if (entry is ConfigEntry<float>)
        {
            ModSettingsManager.AddOption(new SliderOption(entry as ConfigEntry<float>, new SliderConfig() { min = min, max = max, formatString = "{0:0.00}", restartRequired = restartRequired }));
        }
        if (entry is ConfigEntry<int>)
        {
            ModSettingsManager.AddOption(new IntSliderOption(entry as ConfigEntry<int>, new IntSliderConfig() { min = (int)min, max = (int)max, restartRequired = restartRequired }));
        }
        if (entry is ConfigEntry<bool>)
        {
            ModSettingsManager.AddOption(new CheckBoxOption(entry as ConfigEntry<bool>, restartRequired));
        }
        if (entry is ConfigEntry<KeyboardShortcut>)
        {
            ModSettingsManager.AddOption(new KeyBindOption(entry as ConfigEntry<KeyboardShortcut>, restartRequired));
        }
        if (!loadedIcon)
        {
            loadedIcon = true;
            try 
            {
                ModSettingsManager.SetModIcon(LoadSpriteFromModFolder("icon.png"));
            }
            catch (System.Exception e)
            {
                Debug.LogError("error adding ROO mod icon for aliem\n" + e);
            }
        }
    }
    /*[Message:   BepInEx] BepInEx 5.4.21.0 - Risk of Rain 2 (8/27/2024 10:06:08 AM)
[Info   :   BepInEx] Running under Unity v2021.3.33.15620650
[Info   :   BepInEx] CLR runtime version: 4.0.30319.42000
[Info   :   BepInEx] Supports SRE: True
[Info   :   BepInEx] System platform: Bits64, Windows
[Message:   BepInEx] Preloader started
[Info   :   BepInEx] Loaded 1 patcher method from [BepInEx.Preloader 5.4.21.0]
[Info   :   BepInEx] Loaded 1 patcher method from [BepInEx.GUI.Loader 1.0.0.0]
[Info   :   BepInEx] Loaded 1 patcher method from [FixPluginTypesSerialization 1.0.0.0]
[Info   :   BepInEx] Loaded 1 patcher method from [BepInEx.MonoMod.HookGenPatcher 1.2.1.0]
[Info   :   BepInEx] 4 patcher plugins loaded
[Info   :BepInEx.GUI.Loader] BepInEx regular console is enabled, aborting launch.
[Info   :HookGenPatcher] Previous MMHOOK location found. Using that location to save instead.
[Info   :HookGenPatcher] Already ran for this version, reusing that file.
[Info   :FixPluginTypesSerialization] Running under Unity v2021.3.33
[Debug  :FixPluginTypesSerialization] Unity version obtained from main application module.
[Info   :FixPluginTypesSerialization] Found offsets for current version, using them.
[Info   :   BepInEx] Patching [RoR2] with [BepInEx.Chainloader]
[Message:   BepInEx] Preloader finished
[Message:   BepInEx] Chainloader ready
[Message:   BepInEx] Chainloader started
[Info   :   BepInEx] 43 plugins to load
[Info   :   BepInEx] TS Manifest: RiskofThunder-RoR2BepInExPack-1.24.1
[Info   :   BepInEx] Loading [RoR2BepInExPack 1.24.1]
[Debug  :RoR2BepInExPack] ILHook RoR2BepInExPack.VanillaFixes.ILLine.ShowILLine added by assembly: RoR2BepInExPack.dll for: System.Diagnostics.StackTrace.AddFrames
[Debug  :RoR2BepInExPack] OnHook RoR2BepInExPack.VanillaFixes.FixSystemInitializer.EnqueueAllInitializers added by assembly: RoR2BepInExPack.dll for: RoR2.SystemInitializerAttribute.ExecuteStatic
[Debug  :RoR2BepInExPack] OnHook RoR2BepInExPack.VanillaFixes.FixSystemInitializer.LogBadCycle added by assembly: RoR2BepInExPack.dll for: RoR2.SystemInitializerAttribute.ExecuteCoroutine
[Debug  :RoR2BepInExPack] OnHook RoR2BepInExPack.ReflectionHooks.AutoCatchReflectionTypeLoadException.SaferGetTypes added by assembly: RoR2BepInExPack.dll for: System.Reflection.Assembly.GetTypes
[Debug  :RoR2BepInExPack] OnHook RoR2BepInExPack.VanillaFixes.SaferAchievementManager.SaferCollectAchievementDefs added by assembly: RoR2BepInExPack.dll for: RoR2.AchievementManager.CollectAchievementDefs
[Debug  :RoR2BepInExPack] OnHook RoR2BepInExPack.VanillaFixes.SaferResourceAvailability.TryCatchEachLoopIteration added by assembly: RoR2BepInExPack.dll for: ResourceAvailability.MakeAvailable
[Debug  :RoR2BepInExPack] ILHook RoR2BepInExPack.VanillaFixes.SaferSearchableAttribute.SaferScanAssemblyILManipulator added by assembly: RoR2BepInExPack.dll for: HG.Reflection.SearchableAttribute.ScanAssembly
[Info   : Unity Log] SCANNING ASSEMBLIES (not using cached searchables/assemblies!)
[Debug  :RoR2BepInExPack] OnHook RoR2BepInExPack.VanillaFixes.SaferSearchableAttribute.DeterministicInitTimingHook added by assembly: RoR2BepInExPack.dll for: RoR2.Console.Awake
[Debug  :RoR2BepInExPack] OnHook RoR2BepInExPack.VanillaFixes.FixConsoleLog.DoNothing added by assembly: RoR2BepInExPack.dll for: RoR2.UnitySystemConsoleRedirector.Redirect
[Debug  :RoR2BepInExPack] ILHook RoR2BepInExPack.VanillaFixes.FixConVar.ScanAllAssemblies added by assembly: RoR2BepInExPack.dll for: RoR2.Console+<InternalInitConVarsCoroutine>d__26.MoveNext
[Debug  :RoR2BepInExPack] ILHook RoR2BepInExPack.VanillaFixes.FixDeathAnimLog.FixLackingAnim added by assembly: RoR2BepInExPack.dll for: EntityStates.GenericCharacterDeath.PlayDeathAnimation
[Debug  :RoR2BepInExPack] ILHook RoR2BepInExPack.VanillaFixes.FixNullBone.FixBoneCheck added by assembly: RoR2BepInExPack.dll for: DynamicBone.ApplyParticlesToTransforms
[Debug  :RoR2BepInExPack] OnHook RoR2BepInExPack.VanillaFixes.FixExtraGameModesMenu.FixIt added by assembly: RoR2BepInExPack.dll for: RoR2.UI.MainMenu.MainMenuController.Start
[Debug  :RoR2BepInExPack] ILHook RoR2BepInExPack.VanillaFixes.FixProjectileCatalogLimitError.IncreaseCatalogLimit added by assembly: RoR2BepInExPack.dll for: RoR2.ProjectileCatalog.SetProjectilePrefabs
[Debug  :RoR2BepInExPack] ILHook RoR2BepInExPack.ModCompatibility.FixNullEntitlement.FixEntitledCheck added by assembly: RoR2BepInExPack.dll for: RoR2.EntitlementManagement.BaseUserEntitlementTracker`1[[RoR2.LocalUser, RoR2, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]].UserHasEntitlement
[Debug  :RoR2BepInExPack] ILHook RoR2BepInExPack.ModCompatibility.FixNullEntitlement.FixEntitledCheck added by assembly: RoR2BepInExPack.dll for: RoR2.EntitlementManagement.BaseUserEntitlementTracker`1[[RoR2.NetworkUser, RoR2, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]].UserHasEntitlement
[Debug  :RoR2BepInExPack] ILHook RoR2BepInExPack.VanillaFixes.FixExposeLog.FixAddingExpose added by assembly: RoR2BepInExPack.dll for: RoR2.HealthComponent.TakeDamageProcess
[Debug  :RoR2BepInExPack] ILHook RoR2BepInExPack.VanillaFixes.FixNonLethalOneHP.FixLethality added by assembly: RoR2BepInExPack.dll for: RoR2.HealthComponent.TakeDamageProcess
[Debug  :RoR2BepInExPack] ILHook RoR2BepInExPack.VanillaFixes.FixRunScaling.CalculateDifficultyCoefficientOnRunStart added by assembly: RoR2BepInExPack.dll for: RoR2.Run.Start
[Debug  :RoR2BepInExPack] ILHook RoR2BepInExPack.VanillaFixes.FixCharacterBodyRemoveOldestTimedBuff.FixRemoveOldestTimedBuff added by assembly: RoR2BepInExPack.dll for: RoR2.CharacterBody.RemoveOldestTimedBuff
[Debug  :RoR2BepInExPack] OnHook RoR2BepInExPack.VanillaFixes.FixDedicatedServerMaxPlayerCount.FixUsageOfMaxPlayerCountVariable added by assembly: RoR2BepInExPack.dll for: UnityEngine.Networking.NetworkManager.StartHost
[Debug  :RoR2BepInExPack] OnHook RoR2BepInExPack.VanillaFixes.FixDedicatedServerMaxPlayerCount.FixUsageOfMaxPlayerCountVariable2 added by assembly: RoR2BepInExPack.dll for: UnityEngine.Networking.NetworkManager.StartServer
[Debug  :RoR2BepInExPack] ILHook RoR2BepInExPack.VanillaFixes.FixHasEffectiveAuthority.FixHook added by assembly: RoR2BepInExPack.dll for: RoR2.Util.HasEffectiveAuthority
[Debug  :RoR2BepInExPack] ILHook RoR2BepInExPack.ModCompatibility.FixMultiCorrupt.FixStep added by assembly: RoR2BepInExPack.dll for: RoR2.Items.ContagiousItemManager.StepInventoryInfection
[Debug  :RoR2BepInExPack] OnHook RoR2BepInExPack.ModCompatibility.FixMultiCorrupt.OnGenerateStageRNG added by assembly: RoR2BepInExPack.dll for: RoR2.Run.GenerateStageRNG
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_ArtifactCode-1.0.1
[Info   :   BepInEx] Loading [R2API.ArtifactCode 1.0.1]
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_CommandHelper-1.0.2
[Info   :   BepInEx] Loading [R2API.CommandHelper 1.0.2]
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_ContentManagement-1.0.6
[Info   :   BepInEx] Loading [R2API.ContentManagement 1.0.6]
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_DamageType-1.1.2
[Info   :   BepInEx] Loading [R2API.DamageType 1.1.2]
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_Deployable-1.0.1
[Info   :   BepInEx] Loading [R2API.Deployable 1.0.1]
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_Difficulty-1.1.2
[Info   :   BepInEx] Loading [R2API.Difficulty 1.1.2]
[Debug  :RoR2BepInExPack] OnHook R2API.DifficultyAPI.GetExtendedDifficultyDef added by assembly: R2API.Difficulty.dll for: RoR2.DifficultyCatalog.GetDifficultyDef
[Debug  :RoR2BepInExPack] OnHook R2API.DifficultyAPI.InitialiseRuleBookAndFinalizeList added by assembly: R2API.Difficulty.dll for: RoR2.RuleDef.FromDifficulty
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_Director-2.1.3
[Info   :   BepInEx] Loading [R2API.Director 2.1.3]
[Debug  :RoR2BepInExPack] OnHook R2API.DirectorAPI.ApplyChangesOnStart added by assembly: R2API.Director.dll for: RoR2.ClassicStageInfo.Start
[Debug  :RoR2BepInExPack] ILHook R2API.DirectorAPI.SwapVanillaDccsWithOurs modifier by assembly: R2API.Director.dll for: RoR2.ClassicStageInfo.HandleMixEnemyArtifact
[Debug  :RoR2BepInExPack] OnHook R2API.DirectorAPI.InitStageEnumToSceneDefs added by assembly: R2API.Director.dll for: RoR2.SceneCatalog.Init
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_Dot-1.0.3
[Info   :   BepInEx] Loading [R2API.Dot 1.0.3]
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_Elites-1.0.4
[Info   :   BepInEx] Loading [R2API.Elites 1.0.4]
[Debug  :R2API.Elites] EliteAPI.cctor finished.
[Debug  :RoR2BepInExPack] ILHook R2API.EliteAPI.RetrieveVanillaEliteTierCount modifier by assembly: R2API.Elites.dll for: RoR2.CombatDirector.Init
[Debug  :RoR2BepInExPack] OnHook R2API.EliteAPI.UseOurCombatDirectorInitInstead added by assembly: R2API.Elites.dll for: RoR2.CombatDirector.Init
[Debug  :RoR2BepInExPack] ILHook R2API.EliteRamp.UpdateRampProperly modifier by assembly: R2API.Elites.dll for: RoR2.CharacterModel.UpdateMaterials
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_Items-1.0.3
[Info   :   BepInEx] Loading [R2API.Items 1.0.3]
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_Language-1.0.1
[Info   :   BepInEx] Loading [R2API.Language 1.0.1]
[Debug  :RoR2BepInExPack] OnHook R2API.LanguageAPI.Language_GetLocalizedStringByToken added by assembly: R2API.Language.dll for: RoR2.Language.GetLocalizedStringByToken
[Debug  :RoR2BepInExPack] OnHook R2API.LanguageAPI.Language_TokenIsRegistered added by assembly: R2API.Language.dll for: RoR2.Language.TokenIsRegistered
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_Skins-1.2.0
[Info   :   BepInEx] Loading [R2API.Skins 1.2.0]
[Debug  :RoR2BepInExPack] OnHook R2API.SkinIDRS.SetCustomIDRS added by assembly: R2API.Skins.dll for: RoR2.ModelSkinController.ApplySkin
[Debug  :RoR2BepInExPack] OnHook R2API.SkinVFX.ApplyModifier added by assembly: R2API.Skins.dll for: RoR2.EffectComponent.Start
[Debug  :RoR2BepInExPack] OnHook R2API.SkinVFX.ApplyReplacement added by assembly: R2API.Skins.dll for: RoR2.EffectManager.SpawnEffect
[Debug  :RoR2BepInExPack] ILHook R2API.SkinVFX.ModifyBulletAttack modifier by assembly: R2API.Skins.dll for: RoR2.BulletAttack.FireSingle
[Warning: Unity Log] Layer "SplitscreenPlayerMain" is defined in this project's "Tags and Layers" settings but is not defined in LayerIndex!
[Warning: Unity Log] Layer "SplitscreenPlayer2" is defined in this project's "Tags and Layers" settings but is not defined in LayerIndex!
[Warning: Unity Log] Layer "SplitscreenPlayer3" is defined in this project's "Tags and Layers" settings but is not defined in LayerIndex!
[Warning: Unity Log] Layer "SplitscreenPlayer4" is defined in this project's "Tags and Layers" settings but is not defined in LayerIndex!
[Debug  :RoR2BepInExPack] ILHook R2API.SkinVFX.ModifyMuzzleFlash modifier by assembly: R2API.Skins.dll for: RoR2.EffectManager.SimpleMuzzleFlash
[Debug  :RoR2BepInExPack] OnHook R2API.SkinVFX.ModifyGenericMelee added by assembly: R2API.Skins.dll for: EntityStates.BasicMeleeAttack.BeginMeleeAttackEffect
[Debug  :RoR2BepInExPack] ILHook R2API.SkinVFX.ModifyGenericOrb modifier by assembly: R2API.Skins.dll for: RoR2.Orbs.GenericDamageOrb.Begin
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_Loadout-1.0.2
[Info   :   BepInEx] Loading [R2API.Loadout 1.0.2]
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_LobbyConfig-1.0.1
[Info   :   BepInEx] Loading [R2API.LobbyConfig 1.0.1]
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_Networking-1.0.2
[Info   :   BepInEx] Loading [R2API.Networking 1.0.2]
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_Orb-1.0.1
[Info   :   BepInEx] Loading [R2API.Orb 1.0.1]
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_Prefab-1.0.4
[Info   :   BepInEx] Loading [R2API.Prefab 1.0.4]
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_RecalculateStats-1.4.0
[Info   :   BepInEx] Loading [R2API.RecalculateStats 1.4.0]
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_SceneAsset-1.1.2
[Info   :   BepInEx] Loading [R2API.SceneAsset 1.1.2]
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_Sound-1.0.3
[Info   :   BepInEx] Loading [R2API.Sound 1.0.3]
[Debug  :RoR2BepInExPack] OnHook R2API.SoundAPI.AddBanksAfterEngineInit added by assembly: R2API.Sound.dll for: AkSoundEngineInitialization.InitializeSoundEngine
[Debug  :RoR2BepInExPack] OnHook R2API.SoundAPI+Music.AddCustomMusicDatas added by assembly: R2API.Sound.dll for: AkSoundEngineInitialization.InitializeSoundEngine
[Debug  :RoR2BepInExPack] OnHook R2API.SoundAPI+Music.EnableCustomMusicSystems added by assembly: R2API.Sound.dll for: RoR2.MusicController.StartIntroMusic
[Debug  :RoR2BepInExPack] OnHook R2API.SoundAPI+Music.IsGameMusicBankInUse added by assembly: R2API.Sound.dll for: RoR2.MusicController.UpdateState
[Debug  :RoR2BepInExPack] ILHook R2API.SoundAPI+Music.PauseMusicIfGameMusicBankNotInUse modifier by assembly: R2API.Sound.dll for: RoR2.MusicController.LateUpdate
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_TempVisualEffect-1.0.3
[Info   :   BepInEx] Loading [R2API.TempVisualEffect 1.0.3]
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_Unlockable-1.0.2
[Info   :   BepInEx] Loading [R2API.Unlockable 1.0.2]
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_Core-5.1.1
[Info   :   BepInEx] Loading [R2API 5.1.1]
[Debug  :RoR2BepInExPack] OnHook R2API.R2API.CheckIfUsedOnRightGameVersion added by assembly: R2API.Core.dll for: RoR2.RoR2Application.Awake
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_Addressables-1.0.3
[Info   :   BepInEx] Loading [R2API.Addressables 1.0.3]
[Info   :   BepInEx] TS Manifest: RiskofThunder-R2API_Colors-1.0.1
[Info   :   BepInEx] Loading [R2API.Colors 1.0.1]
[Info   :   BepInEx] TS Manifest: Dragonyck-PhotoMode-1.4.4
[Info   :   BepInEx] Loading [PhotoMode 1.4.4]
[Debug  :RoR2BepInExPack] OnHook PhotoMode.PhotoModePlugin.<Awake>b__3_0 added by assembly: PhotoMode.dll for: RoR2.CameraRigController.OnEnable
[Debug  :RoR2BepInExPack] OnHook PhotoMode.PhotoModePlugin.<Awake>b__3_1 added by assembly: PhotoMode.dll for: RoR2.CameraRigController.OnDisable
[Debug  :RoR2BepInExPack] OnHook PhotoMode.PhotoModePlugin.<Awake>b__3_2 added by assembly: PhotoMode.dll for: RoR2.UI.PauseScreenController.Awake
[Info   :   BepInEx] TS Manifest: IHarbHD-DebugToolkit-3.16.2
[Info   :   BepInEx] Loading [DebugToolkit 3.16.2]
[Info   :DebugToolkit] Created by Harb, iDeathHD and . Based on RoR2Cheats by Morris1927.
[Debug  :RoR2BepInExPack] ILHook DebugToolkit.Hooks.UnlockConsole added by assembly: DebugToolkit.dll for: RoR2.Console+<AwakeCoroutine>d__57.MoveNext
[Debug  :RoR2BepInExPack] OnHook DebugToolkit.Hooks.InitCommandsAndFreeConvars added by assembly: DebugToolkit.dll for: RoR2.Console.InitConVars
[Debug  :RoR2BepInExPack] OnHook DebugToolkit.Hooks.LogNetworkCommandsAndCheckPermissions added by assembly: DebugToolkit.dll for: RoR2.Console.RunCmd
[Debug  :RoR2BepInExPack] ILHook DebugToolkit.Hooks.WarnUserOfServerCommandOffline modifier by assembly: DebugToolkit.dll for: RoR2.Console.RunCmd
[Debug  :RoR2BepInExPack] OnHook DebugToolkit.Hooks.BetterAutoCompletion added by assembly: DebugToolkit.dll for: RoR2.Console+AutoComplete.SetSearchString
[Debug  :RoR2BepInExPack] OnHook DebugToolkit.Hooks.AddConCommandSignatureHint added by assembly: DebugToolkit.dll for: RoR2.UI.ConsoleWindow.Start
[Debug  :RoR2BepInExPack] OnHook DebugToolkit.Hooks.UpdateCommandSignature added by assembly: DebugToolkit.dll for: RoR2.UI.ConsoleWindow.OnInputFieldValueChanged
[Debug  :RoR2BepInExPack] ILHook DebugToolkit.Hooks.ApplyTextWithoutColorTags modifier by assembly: DebugToolkit.dll for: RoR2.UI.ConsoleWindow.ApplyAutoComplete
[Debug  :RoR2BepInExPack] ILHook DebugToolkit.Hooks.SmoothDropDownSuggestionNavigation modifier by assembly: DebugToolkit.dll for: RoR2.UI.ConsoleWindow.Update
[Debug  :RoR2BepInExPack] ILHook DebugToolkit.Hooks.EnableCheatsInCCSetScene modifier by assembly: DebugToolkit.dll for: RoR2.Networking.NetworkManagerSystem.CCSetScene
[Debug  :RoR2BepInExPack] OnHook DebugToolkit.Hooks.OverrideVanillaSceneList added by assembly: DebugToolkit.dll for: RoR2.Networking.NetworkManagerSystem.CCSceneList
[Debug  :RoR2BepInExPack] OnHook DebugToolkit.Commands.Profile.PreventSave added by assembly: DebugToolkit.dll for: RoR2.SaveSystem.Save
[Debug  :RoR2BepInExPack] OnHook DebugToolkit.Hooks.InterceptPing added by assembly: DebugToolkit.dll for: RoR2.PingerController.RebuildPing
[Debug  :RoR2BepInExPack] ILHook DebugToolkit.Hooks.InfiniteTowerRun_BeginNextWave modifier by assembly: DebugToolkit.dll for: RoR2.InfiniteTowerRun.BeginNextWave
[Debug  :RoR2BepInExPack] OnHook DebugToolkit.NetworkManager.CreateNetworkObject added by assembly: DebugToolkit.dll for: RoR2.NetworkSession.Start
[Debug  :RoR2BepInExPack] OnHook DebugToolkit.NetworkManager.DestroyNetworkObject added by assembly: DebugToolkit.dll for: RoR2.NetworkSession.OnDestroy
[Debug  :RoR2BepInExPack] OnHook DebugToolkit.Hooks.NonLethatDamage added by assembly: DebugToolkit.dll for: RoR2.HealthComponent.TakeDamage
[Debug  :RoR2BepInExPack] OnHook DebugToolkit.Hooks.SetGodMode added by assembly: DebugToolkit.dll for: RoR2.CharacterMaster.Awake
[Debug  :RoR2BepInExPack] OnHook R2API.PrefabAPI+<>c.<GetParent>b__14_0 added by assembly: R2API.Prefab.dll for: RoR2.Util.IsPrefab
[Info   :R2API.ContentManagement] Created a SerializableContentPack for mod iHarbHD.DebugToolkit
[Info   :   BepInEx] TS Manifest: Dragonyck-DebuggingPlains-1.1.1
[Info   :   BepInEx] Loading [DebuggingPlains 1.1.1]
[Debug  :RoR2BepInExPack] OnHook DebuggingPlains.DebuggingPlains.MainMenuController_UpdateMenuTransition added by assembly: DebuggingPlains.dll for: RoR2.UI.MainMenu.MainMenuController.UpdateMenuTransition
[Debug  :RoR2BepInExPack] OnHook DebuggingPlains.DebuggingPlains.PreGameRuleVoteController_UpdateGameVotes added by assembly: DebuggingPlains.dll for: RoR2.PreGameRuleVoteController.UpdateGameVotes
[Debug  :RoR2BepInExPack] OnHook DebuggingPlains.DebuggingPlains.Run_PickNextStageScene added by assembly: DebuggingPlains.dll for: RoR2.Run.PickNextStageScene
[Debug  :RoR2BepInExPack] OnHook DebuggingPlains.DebuggingPlains.Stage_RespawnCharacter added by assembly: DebuggingPlains.dll for: RoR2.Stage.RespawnCharacter
[Debug  :RoR2BepInExPack] OnHook DebuggingPlains.DebuggingPlains.SceneDirector_PopulateScene added by assembly: DebuggingPlains.dll for: RoR2.SceneDirector.PopulateScene
[Debug  :RoR2BepInExPack] OnHook DebuggingPlains.DebuggingPlains+<>c.<Awake>b__2_2 added by assembly: DebuggingPlains.dll for: RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect
[Info   :   BepInEx] TS Manifest: Rune580-Risk_Of_Options-2.8.2
[Info   :   BepInEx] Loading [RiskOfOptions 2.8.2]
[Debug  :RoR2BepInExPack] OnHook RiskOfOptions.Lib.LanguageApi.GetLocalizedStringByToken added by assembly: RiskOfOptions.dll for: RoR2.Language.GetLocalizedStringByToken
[Debug  :RoR2BepInExPack] OnHook RiskOfOptions.ModSettingsManager.PauseManagerOnCCTogglePause added by assembly: RiskOfOptions.dll for: RoR2.PauseManager.CCTogglePause
[Info   :   BepInEx] TS Manifest: Nuxlar-MultiplayerModTesting-1.0.0
[Info   :   BepInEx] Loading [MultiplayerModTesting 1.0.0]
[Debug  :RoR2BepInExPack] OnHook MultiplayerModTesting.MultiplayerModTesting+<>c.<Awake>b__5_1 added by assembly: MultiplayerModTesting.dll for: RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect
[Info   :   BepInEx] TS Manifest: dgosling-RuntimeInspector-4.0.1
[Info   :   BepInEx] Loading [RuntimeInspectorPlugin 4.0.1]
[Info   :   BepInEx] TS Manifest: Risky_Sleeps-ClassicItemsReturns-3.1.15
[Info   :   BepInEx] Loading [Classic Items Returns 3.1.14]
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Modules.LanguageOverrides.FinalizeLanguage added by assembly: ClassicItemsReturns.dll for: RoR2.UI.MainMenu.MainMenuController.Start
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Modules.LanguageOverrides.LogBookController_Start added by assembly: ClassicItemsReturns.dll for: RoR2.UI.LogBook.LogBookController.Start
[Info   :R2API.ContentManagement] Created a SerializableContentPack for mod com.RiskySleeps.ClassicItemsReturns
[Debug  :RoR2BepInExPack] ILHook ClassicItemsReturns.Modules.Buffs+<>c.<Initialize>b__13_0 modifier by assembly: ClassicItemsReturns.dll for: RoR2.CharacterBody.UpdateAllTemporaryVisualEffects
[Debug  :RoR2BepInExPack] ILHook ClassicItemsReturns.Modules.Buffs+<>c.<Initialize>b__13_1 modifier by assembly: ClassicItemsReturns.dll for: RoR2.CharacterModel.UpdateOverlays
[Debug  :RoR2BepInExPack] OnHook R2API.OrbAPI.AddOrbs added by assembly: R2API.Orb.dll for: RoR2.Orbs.OrbCatalog.GenerateCatalog
[Info   : Unity Log] ClassicItemsReturns: Swapping material to Vending Machine Glass material for mdl3dJarSouls
[Info   : Unity Log] ClassicItemsReturns: Swapping material to Vending Machine Glass material for mdl3dJarSouls
[Info   : Unity Log] ClassicItemsReturns: Swapping material to Vending Machine Glass material for mdl3dJarSouls
[Debug  :RoR2BepInExPack] ILHook R2API.ItemAPI.MaterialFixForItemDisplayOnCharacter modifier by assembly: R2API.Items.dll for: RoR2.CharacterModel.UpdateMaterials
[Debug  :RoR2BepInExPack] OnHook R2API.ItemAPI.AddingItemDisplayRulesToCharacterModels added by assembly: R2API.Items.dll for: RoR2.ItemDisplayRuleSet.Init
[Debug  :RoR2BepInExPack] ILHook R2API.ItemAPI.AddCustomTagsToItemCatalog modifier by assembly: R2API.Items.dll for: RoR2.ItemCatalog.SetItemDefs
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Equipment.EquipmentBase.PerformEquipmentAction added by assembly: ClassicItemsReturns.dll for: RoR2.EquipmentSlot.PerformEquipmentAction
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Equipment.EquipmentBase.EquipmentSlot_UpdateTargets added by assembly: ClassicItemsReturns.dll for: RoR2.EquipmentSlot.UpdateTargets
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Equipment.CreateGhostTargeting.FixGhostGupSplit added by assembly: ClassicItemsReturns.dll for: EntityStates.Gup.BaseSplitDeath.OnEnter
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Equipment.EquipmentBase.PerformEquipmentAction added by assembly: ClassicItemsReturns.dll for: RoR2.EquipmentSlot.PerformEquipmentAction
[Debug  :RoR2BepInExPack] ILHook R2API.RecalculateStatsAPI.HookRecalculateStats modifier by assembly: R2API.RecalculateStats.dll for: RoR2.CharacterBody.RecalculateStats
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Equipment.EquipmentBase.PerformEquipmentAction added by assembly: ClassicItemsReturns.dll for: RoR2.EquipmentSlot.PerformEquipmentAction
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Equipment.EquipmentBase.EquipmentSlot_UpdateTargets added by assembly: ClassicItemsReturns.dll for: RoR2.EquipmentSlot.UpdateTargets
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Equipment.EquipmentBase.PerformEquipmentAction added by assembly: ClassicItemsReturns.dll for: RoR2.EquipmentSlot.PerformEquipmentAction
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Equipment.EquipmentBase.PerformEquipmentAction added by assembly: ClassicItemsReturns.dll for: RoR2.EquipmentSlot.PerformEquipmentAction
[Info   : Unity Log] ClassicItemsReturns: Swapping material to Vending Machine Glass material for mdl3dCell
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Items.Uncommon.RustyJetpack.RecalculateStats_AddJump added by assembly: ClassicItemsReturns.dll for: RoR2.CharacterBody.RecalculateStats
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Items.Uncommon.RustyJetpack.ApplyJumpVelocity_DoShorthop added by assembly: ClassicItemsReturns.dll for: EntityStates.GenericCharacterMain.ApplyJumpVelocity
[Debug  :RoR2BepInExPack] ILHook ClassicItemsReturns.Items.Uncommon.RustyJetpack.ReplaceJumpEffect modifier by assembly: ClassicItemsReturns.dll for: EntityStates.GenericCharacterMain.ProcessJump
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Items.Uncommon.SmartShopper.DeathRewards_OnKilledServer added by assembly: ClassicItemsReturns.dll for: RoR2.DeathRewards.OnKilledServer
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Items.Rare.HitList.CharacterBody_OnClientBuffsChanged added by assembly: ClassicItemsReturns.dll for: RoR2.CharacterBody.OnClientBuffsChanged
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Items.Rare.Permafrost.SetStateOnHurt_OnTakeDamageServer added by assembly: ClassicItemsReturns.dll for: RoR2.SetStateOnHurt.OnTakeDamageServer
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Items.Rare.USB.ChargingState_OnEnter added by assembly: ClassicItemsReturns.dll for: RoR2.TeleporterInteraction+ChargingState.OnEnter
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Items.Rare.USB.TeleporterInteraction_Start added by assembly: ClassicItemsReturns.dll for: RoR2.TeleporterInteraction.Start
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Items.Rare.USB.GoldshoresBossfight_SetBossImmunity added by assembly: ClassicItemsReturns.dll for: EntityStates.Missions.Goldshores.GoldshoresBossfight.SetBossImmunity
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Items.Rare.USB.ScriptedCombatEncounter_Spawn added by assembly: ClassicItemsReturns.dll for: RoR2.ScriptedCombatEncounter.Spawn
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Items.Rare.USB.SceneDirector_PopulateScene added by assembly: ClassicItemsReturns.dll for: RoR2.SceneDirector.PopulateScene
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Items.Rare.USB.CharacterBody_OnInventoryChanged added by assembly: ClassicItemsReturns.dll for: RoR2.CharacterBody.OnInventoryChanged
[Info   : Unity Log] ClassicItemsReturns: Swapping material to Infusion Glass material for mdl3dVial
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Items.Common.SnakeEyes.DiceOnShrineUse added by assembly: ClassicItemsReturns.dll for: RoR2.PurchaseInteraction.OnInteractionBegin
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Items.Common.SnakeEyes.GeodeController_OnInteractionBegin added by assembly: ClassicItemsReturns.dll for: RoR2.GeodeController.OnInteractionBegin
[Debug  :RoR2BepInExPack] ILHook R2API.DotAPI.RetrieveVanillaCount modifier by assembly: R2API.Dot.dll for: RoR2.DotController.InitDotCatalog
[Debug  :RoR2BepInExPack] ILHook R2API.DotAPI.ResizeTimerArray modifier by assembly: R2API.Dot.dll for: RoR2.DotController.Awake
[Debug  :RoR2BepInExPack] OnHook R2API.DotAPI.AddCustomDots added by assembly: R2API.Dot.dll for: RoR2.DotController.InitDotCatalog
[Debug  :RoR2BepInExPack] OnHook R2API.DotAPI.TrackActiveCustomDots added by assembly: R2API.Dot.dll for: RoR2.DotController.Awake
[Debug  :RoR2BepInExPack] OnHook R2API.DotAPI.TrackActiveCustomDots2 added by assembly: R2API.Dot.dll for: RoR2.DotController.OnDestroy
[Debug  :RoR2BepInExPack] OnHook R2API.DotAPI.GetDotDef added by assembly: R2API.Dot.dll for: RoR2.DotController.GetDotDef
[Debug  :RoR2BepInExPack] OnHook R2API.DotAPI.FixedUpdate added by assembly: R2API.Dot.dll for: RoR2.DotController.FixedUpdate
[Debug  :RoR2BepInExPack] ILHook R2API.DotAPI.FixInflictDotReturnCheck modifier by assembly: R2API.Dot.dll for: RoR2.DotController.InflictDot
[Debug  :RoR2BepInExPack] ILHook R2API.DotAPI.CallCustomDotBehaviours modifier by assembly: R2API.Dot.dll for: RoR2.DotController.AddDot
[Debug  :RoR2BepInExPack] OnHook R2API.DotAPI.OnHasDotActive added by assembly: R2API.Dot.dll for: RoR2.DotController.HasDotActive
[Debug  :RoR2BepInExPack] ILHook R2API.DotAPI.EvaluateDotStacksForType modifier by assembly: R2API.Dot.dll for: RoR2.DotController.EvaluateDotStacksForType
[Debug  :RoR2BepInExPack] ILHook R2API.DotAPI.FixDeathMark modifier by assembly: R2API.Dot.dll for: RoR2.GlobalEventManager.ProcessHitEnemy
[Info   : R2API.Dot] Custom Dot (Index: 10) that uses Buff : CIR_ThalliumBuff added
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Modules.Dots+Thallium.HealthComponent_TakeDamage added by assembly: ClassicItemsReturns.dll for: RoR2.HealthComponent.TakeDamageProcess
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Modules.Dots.DotController_Awake added by assembly: ClassicItemsReturns.dll for: RoR2.DotController.Awake
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.SharedHooks.TakeDamage.HealthComponent_TakeDamage added by assembly: ClassicItemsReturns.dll for: RoR2.HealthComponent.TakeDamageProcess
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.SharedHooks.OnHitEnemy.GlobalEventManager_OnHitEnemy added by assembly: ClassicItemsReturns.dll for: RoR2.GlobalEventManager.ProcessHitEnemy
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.SharedHooks.OnCharacterDeath.GlobalEventManager_OnCharacterDeath added by assembly: ClassicItemsReturns.dll for: RoR2.GlobalEventManager.OnCharacterDeath
[Debug  :RoR2BepInExPack] ILHook ClassicItemsReturns.SharedHooks.ModifyFinalDamage+<>c.<Initialize>b__2_0 modifier by assembly: ClassicItemsReturns.dll for: RoR2.HealthComponent.TakeDamageProcess
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Utils.IsTeleActivatedTracker.ChargingState_OnEnter added by assembly: ClassicItemsReturns.dll for: RoR2.TeleporterInteraction+ChargingState.OnEnter
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Utils.IsTeleActivatedTracker.ChargedState_OnEnter added by assembly: ClassicItemsReturns.dll for: RoR2.TeleporterInteraction+ChargedState.OnEnter
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Utils.IsTeleActivatedTracker.Active_OnEnter added by assembly: ClassicItemsReturns.dll for: EntityStates.InfiniteTowerSafeWard.Active.OnEnter
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Utils.IsTeleActivatedTracker.Travelling_OnEnter added by assembly: ClassicItemsReturns.dll for: EntityStates.InfiniteTowerSafeWard.Travelling.OnEnter
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Utils.IsTeleActivatedTracker.BrotherEncounterPhaseBaseState_OnEnter added by assembly: ClassicItemsReturns.dll for: EntityStates.Missions.BrotherEncounter.BrotherEncounterPhaseBaseState.OnEnter
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Utils.IsTeleActivatedTracker.ArenaMissionController_BeginRound added by assembly: ClassicItemsReturns.dll for: RoR2.ArenaMissionController.BeginRound
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Utils.IsTeleActivatedTracker.MoonBatteryActive_OnEnter added by assembly: ClassicItemsReturns.dll for: EntityStates.Missions.Moon.MoonBatteryActive.OnEnter
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Utils.IsTeleActivatedTracker.StartHurt_OnEnter added by assembly: ClassicItemsReturns.dll for: EntityStates.ArtifactShell.StartHurt.OnEnter
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Utils.IsTeleActivatedTracker.Charging_OnEnter added by assembly: ClassicItemsReturns.dll for: EntityStates.DeepVoidPortalBattery.Charging.OnEnter
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Utils.IsTeleActivatedTracker.GoldshoresBossfight_SpawnBoss added by assembly: ClassicItemsReturns.dll for: EntityStates.Missions.Goldshores.GoldshoresBossfight.SpawnBoss
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Utils.IsTeleActivatedTracker.MeridianEventStart_OnEnter added by assembly: ClassicItemsReturns.dll for: RoR2.MeridianEventTriggerInteraction+MeridianEventStart.OnEnter
[Info   :   BepInEx] TS Manifest: TheTimesweeper-3rdRowCSS-1.0.0
[Info   :   BepInEx] Loading [Bigger CSS 1.0.0]
[Info   :   BepInEx] Loading [CursorFreedom2 0.0.1]
[Debug  :RoR2BepInExPack] OnHook CursorFreedom2.CursorFreedom2Plugin.RoR2Application_UpdateCursorState added by assembly: CursorFreedom2.dll for: RoR2.RoR2Application.UpdateCursorState
[Debug  :RoR2BepInExPack] OnHook CursorFreedom2.CursorFreedom2Plugin.MPEventSystemManager_Update added by assembly: CursorFreedom2.dll for: RoR2.MPEventSystemManager.Update
[Info   :   BepInEx] TS Manifest: TheTimesweeper-HitboxViewerMod-1.5.4
[Info   :   BepInEx] Loading [Silly Hitbox Viewer 1.5.4]
[Warning:Silly Hitbox Viewer] uh1
[Warning:Silly Hitbox Viewer] uh2
[Warning:Silly Hitbox Viewer] uh3
[Warning:Silly Hitbox Viewer] uh4
[Warning:Silly Hitbox Viewer] uh5
[Debug  :RoR2BepInExPack] OnHook SillyHitboxViewer.HitboxViewerMod.BodyCatalog_Init added by assembly: SillyHitboxViewer.dll for: RoR2.BodyCatalog.Init
[Debug  :RoR2BepInExPack] OnHook SillyHitboxViewer.HitboxViewerMod.OverlapAttack_Fire added by assembly: SillyHitboxViewer.dll for: RoR2.OverlapAttack.Fire
[Debug  :RoR2BepInExPack] OnHook SillyHitboxViewer.HitboxViewerMod.BlastAttack_Fire added by assembly: SillyHitboxViewer.dll for: RoR2.BlastAttack.Fire
[Debug  :RoR2BepInExPack] OnHook SillyHitboxViewer.HitboxViewerMod.HurtBox_Awake added by assembly: SillyHitboxViewer.dll for: RoR2.HurtBox.Awake
[Debug  :RoR2BepInExPack] OnHook SillyHitboxViewer.HitboxViewerMod.BulletAttack_FireSingle added by assembly: SillyHitboxViewer.dll for: RoR2.BulletAttack.FireSingle
[Debug  :RoR2BepInExPack] OnHook SillyHitboxViewer.HitboxViewerMod.BulletAttack_InitBulletHitFromRaycastHit added by assembly: SillyHitboxViewer.dll for: RoR2.BulletAttack.InitBulletHitFromRaycastHit
[Debug  :RoR2BepInExPack] OnHook SillyHitboxViewer.HitboxViewerMod.CharacterMotor_Awake added by assembly: SillyHitboxViewer.dll for: RoR2.CharacterMotor.Awake
[Info   :   BepInEx] TS Manifest: TheTimesweeper-Red_Alert-3.0.2
[Info   :   BepInEx] Loading [Red Alert 3.0.1]
[Debug  :RoR2BepInExPack] OnHook RA2Mod.General.GeneralHooks.ModelSkinController_ApplySkin added by assembly: RA2Mod.dll for: RoR2.ModelSkinController.ApplySkin
[Info   :   BepInEx] Loading [Silly deltaTime 1.0.0]
[Info   :   BepInEx] Loading [Silly False Son 1.0.0]
[Error  : Unity Log] error adding ROO mod icon for aliem
System.IO.FileNotFoundException: Could not find file "E:\r2modmanPlus-local\RiskOfRain2\profiles\Blinx Returns\BepInEx\plugins\TheTimesweeper-SillyFalseSon\icon.png"
File name: 'E:\r2modmanPlus-local\RiskOfRain2\profiles\Blinx Returns\BepInEx\plugins\TheTimesweeper-SillyFalseSon\icon.png'
  at System.IO.FileStream..ctor (System.String path, System.IO.FileMode mode, System.IO.FileAccess access, System.IO.FileShare share, System.Int32 bufferSize, System.Boolean anonymous, System.IO.FileOptions options) [0x0019e] in <7e05db41a20b45108859fa03b97088d4>:IL_019E 
  at System.IO.FileStream..ctor (System.String path, System.IO.FileMode mode, System.IO.FileAccess access, System.IO.FileShare share, System.Int32 bufferSize) [0x00000] in <7e05db41a20b45108859fa03b97088d4>:IL_0000 
  at (wrapper remoting-invoke-with-check) System.IO.FileStream..ctor(string,System.IO.FileMode,System.IO.FileAccess,System.IO.FileShare,int)
  at System.IO.File.ReadAllBytes (System.String path) [0x00000] in <7e05db41a20b45108859fa03b97088d4>:IL_0000 
  at Configuration.LoadSpriteFromModFolder (System.String fileName, System.Boolean pointFilter) [0x00024] in <b40479f3895840b6b604118dc241ff78>:IL_0024 
  at Configuration.TryRegisterOption[T] (BepInEx.Configuration.ConfigEntry`1[T] entry, System.Single min, System.Single max, System.Boolean restartRequired) [0x000da] in <b40479f3895840b6b604118dc241ff78>:IL_00DA 
[Info   :   BepInEx] TS Manifest: TheTimesweeper-SillyItems-1.3.2
[Info   :   BepInEx] Loading [Silly Items 1.3.2]
[Debug  :RoR2BepInExPack] OnHook SillyGlasses.SillyGlassesPlugin.InvChangedHook added by assembly: SillyGlasses.dll for: RoR2.CharacterBody.OnInventoryChanged
[Debug  :RoR2BepInExPack] OnHook SillyGlasses.SillyGlassesPlugin.EnableItemDisplayHook added by assembly: SillyGlasses.dll for: RoR2.CharacterModel.EnableItemDisplay
[Debug  :RoR2BepInExPack] OnHook SillyGlasses.SillyGlassesPlugin.DisableItemDisplayHook added by assembly: SillyGlasses.dll for: RoR2.CharacterModel.DisableItemDisplay
[Info   :   BepInEx] Loading [SillyMod2 0.0.1]
[Info   :   BepInEx] TS Manifest: TheTimesweeper-SurvivorSortOrder-1.1.1
[Info   :   BepInEx] Loading [SurvivorSortOrder 1.1.1]
[Debug  :RoR2BepInExPack] OnHook SillyMod.SillySortPlugin.SurvivorCatalog_SetSurvivorDefs added by assembly: SillySortOrder.dll for: RoR2.SurvivorCatalog.SetSurvivorDefs
[Debug  :RoR2BepInExPack] OnHook SillyMod.SillySortPlugin.CharacterSelectBarController_Awake added by assembly: SillySortOrder.dll for: RoR2.CharacterSelectBarController.Awake
[Info   :   BepInEx] TS Manifest: TheTimeSweeper-ZillyDebug-6.9.069
[Info   :   BepInEx] Loading [Zilly Debug 0.0.4]
[Debug  :RoR2BepInExPack] OnHook R2API.Utils.CommandHelper.ConsoleReady added by assembly: R2API.CommandHelper.dll for: RoR2.Console.InitConVarsCoroutine
[Debug  :RoR2BepInExPack] OnHook ZillyDebug.ZillyDebugPlugin.LogBookController_GetSurvivorStatus added by assembly: ZillyDebug.dll for: RoR2.UI.LogBook.LogBookController.GetSurvivorStatus
[Debug  :RoR2BepInExPack] OnHook ZillyDebug.ZillyDebugPlugin.LogBookController_GetPickupStatus added by assembly: ZillyDebug.dll for: RoR2.UI.LogBook.LogBookController.GetPickupStatus
[Message:   BepInEx] Chainloader startup complete
[Warning:     R2API] This version of R2API was built for build id "1.3.1", you are running "1.3.4".
[Warning:     R2API] Should any problems arise, please check for a new version before reporting issues.
[Info   :     R2API] [NetworkCompatibility] Adding to the networkModList : 
[Info   :     R2API] com.RiskySleeps.ClassicItemsReturns;3.1.14
[Debug  :RoR2BepInExPack] OnHook DebugToolkit.StringFinder.AddCurrentStageIscsToCache added by assembly: DebugToolkit.dll for: RoR2.ClassicStageInfo.Start
[Debug  :RoR2BepInExPack] OnHook AcridHitbox.BiggerCSSPlugin.CharacterSelectController_Awake added by assembly: BiggerCSS.dll for: RoR2.UI.CharacterSelectController.Awake
[Debug  :RoR2BepInExPack] Assembly.GetTypes() failed for RA2Mod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null (threw ReflectionTypeLoadException). System.Reflection.ReflectionTypeLoadException: Exception of type 'System.Reflection.ReflectionTypeLoadException' was thrown.
Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
  at (wrapper managed-to-native) System.Reflection.Assembly.GetTypes(System.Reflection.Assembly,bool)
  at (wrapper dynamic-method) System.Reflection.Assembly.DMD<System.Reflection.Assembly::GetTypes>(System.Reflection.Assembly)
  at (wrapper dynamic-method) MonoMod.Utils.DynamicMethodDefinition.Trampoline<System.Reflection.Assembly::GetTypes>?-624099166(System.Reflection.Assembly)
  at RoR2BepInExPack.ReflectionHooks.AutoCatchReflectionTypeLoadException.SaferGetTypes (System.Func`2[T,TResult] orig, System.Reflection.Assembly self) [0x00006] in <dcfd801117434c69ad117b0772e91f5f>:IL_0006 
System.IO.FileNotFoundException: Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
File name: 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
System.IO.FileNotFoundException: Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
File name: 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
System.IO.FileNotFoundException: Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
File name: 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
System.IO.FileNotFoundException: Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
File name: 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
System.IO.FileNotFoundException: Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
File name: 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
System.IO.FileNotFoundException: Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
File name: 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
[Info   : Unity Log] WwiseUnity: Wwise(R) SDK Version 2023.1.4 Build 8496.
[Info   : Unity Log] WwiseUnity: Setting Plugin DLL path to: C:/Program Files (x86)/Steam/steamapps/common/Risk of Rain 2/Risk of Rain 2_Data\Plugins\x86_64
[Info   : Unity Log] WwiseUnity: Sound engine initialized successfully.
[Info   :R2API.Sound] Custom sound banks loaded.
[Info   : Unity Log] Queueing Soundbank for load antiHeal 
[Info   : Unity Log] Queueing Soundbank for load ghost 
[Info   : Unity Log] Queueing Soundbank for load zoneDampCaveSimple 
[Info   : Unity Log] Queueing Soundbank for load zoneShipGraveyard 
[Info   : Unity Log] Queueing Soundbank for load zoneWispGraveyard 
[Info   : Unity Log] Queueing Soundbank for load Boss_Gravekeeper 
[Info   : Unity Log] Queueing Soundbank for load Global 
[Info   : Unity Log] Queueing Soundbank for load proc_armorReduction 
[Info   : Unity Log] Queueing Soundbank for load proc_guillotine 
[Info   : Unity Log] Queueing Soundbank for load proc_nearbyDamageBonus 
[Info   : Unity Log] Queueing Soundbank for load proc_TPhealingNova 
[Info   : Unity Log] Queueing Soundbank for load proc_warhorn 
[Info   : Unity Log] Queueing Soundbank for load use_cleanse 
[Info   : Unity Log] Queueing Soundbank for load use_fireballDash 
[Info   : Unity Log] Queueing Soundbank for load use_gateway 
[Info   : Unity Log] Queueing Soundbank for load use_lunar_utilityReplacement 
[Info   : Unity Log] Queueing Soundbank for load use_tonic 
[Info   : Unity Log] Queueing Soundbank for load Mob_clayBruiser 
[Info   : Unity Log] Queueing Soundbank for load Music 
[Info   : Unity Log] Queueing Soundbank for load char_loader 
[Info   : Unity Log] Queueing Soundbank for load char_treeBot 
[Info   : Unity Log] Queueing Soundbank for load Mob_vulture 
[Info   : Unity Log] Queueing Soundbank for load Boss_RoboBall 
[Info   : Unity Log] Queueing Soundbank for load char_engineer_walkingTurret 
[Info   : Unity Log] Soundbanks queued, pendingLoads = 24
[Info   : Unity Log] Queueing Soundbank for load proc_thorns 
[Info   : Unity Log] Queueing Soundbank for load proc_regenOnKill 
[Info   : Unity Log] Queueing Soundbank for load proc_moneyOnKill 
[Info   : Unity Log] Queueing Soundbank for load proc_laserTurbine 
[Info   : Unity Log] Queueing Soundbank for load proc_boss_vagrantNova 
[Info   : Unity Log] Queueing Soundbank for load use_gainArmor 
[Info   : Unity Log] Queueing Soundbank for load item_lunar_primaryReplace 
[Info   : Unity Log] Queueing Soundbank for load char_acrid 
[Info   : Unity Log] Queueing Soundbank for load Boss_Scav 
[Info   : Unity Log] Queueing Soundbank for load Mob_nullifier 
[Info   : Unity Log] Queueing Soundbank for load obj_lunarPool 
[Info   : Unity Log] Queueing Soundbank for load obj_nullWard 
[Info   : Unity Log] Queueing Soundbank for load drone_emergencyHealing 
[Info   : Unity Log] Soundbanks queued, pendingLoads = 37
[Info   : Unity Log] Queueing Soundbank for load Boss_Artifact 
[Info   : Unity Log] Queueing Soundbank for load zoneArtifactWorld 
[Info   : Unity Log] Queueing Soundbank for load zoneSkyMeadow 
[Info   : Unity Log] Queueing Soundbank for load Boss_Grandparent 
[Info   : Unity Log] Queueing Soundbank for load Mob_parent 
[Info   : Unity Log] Queueing Soundbank for load Mob_miniMushroom 
[Info   : Unity Log] Queueing Soundbank for load proc_deathmark 
[Info   : Unity Log] Queueing Soundbank for load proc_interstellarDeskPlant 
[Info   : Unity Log] Queueing Soundbank for load use_sawmerang 
[Info   : Unity Log] Queueing Soundbank for load use_recycler 
[Info   : Unity Log] Queueing Soundbank for load proc_repulsionArmor 
[Info   : Unity Log] Queueing Soundbank for load proc_squidTurret 
[Info   : Unity Log] Queueing Soundbank for load item_lunar_FocusedConverence 
[Info   : Unity Log] Soundbanks queued, pendingLoads = 50
[Info   : Unity Log] Queueing Soundbank for load Boss_MoonBrother 
[Info   : Unity Log] Queueing Soundbank for load char_captain 
[Info   : Unity Log] Queueing Soundbank for load zoneMoon 
[Info   : Unity Log] Queueing Soundbank for load Mob_lunarGolem 
[Info   : Unity Log] Queueing Soundbank for load use_lifestealOnhit 
[Info   : Unity Log] Queueing Soundbank for load use_teamWarCry 
[Info   : Unity Log] Queueing Soundbank for load use_deathProjectile 
[Info   : Unity Log] Queueing Soundbank for load proc_bleedOnCritAndExplode 
[Info   : Unity Log] Queueing Soundbank for load proc_fireballsOnHit 
[Info   : Unity Log] Queueing Soundbank for load item_lunar_MonstersOnShrineUse 
[Info   : Unity Log] Queueing Soundbank for load item_lunar_randomDamageZone 
[Info   : Unity Log] Queueing Soundbank for load proc_siphonOnLowHealth 
[Info   : Unity Log] Queueing Soundbank for load Mob_lunarWisp 
[Info   : Unity Log] Queueing Soundbank for load zoneRootJungle 
[Info   : Unity Log] Soundbanks queued, pendingLoads = 64
[Info   : Unity Log] Queueing Soundbank for load Mob_lunarExploder 
[Info   : Unity Log] Queueing Soundbank for load char_bandit2 
[Info   : Unity Log] Queueing Soundbank for load obj_lunarReroller 
[Info   : Unity Log] Queueing Soundbank for load char_heretic 
[Info   : Unity Log] Queueing Soundbank for load item_lunar_secondaryReplace 
[Info   : Unity Log] Queueing Soundbank for load item_lunar_specialReplace 
[Info   : Unity Log] Soundbanks queued, pendingLoads = 70
[Info   : Unity Log] Queueing Soundbank for load Mob_acidLarva 
[Info   : Unity Log] Queueing Soundbank for load proc_goldOnHurt 
[Info   : Unity Log] Queueing Soundbank for load proc_healingPotion 
[Info   : Unity Log] Queueing Soundbank for load proc_moveSpeedOnKill 
[Info   : Unity Log] Queueing Soundbank for load proc_attackAndMoveBuff 
[Info   : Unity Log] Queueing Soundbank for load proc_regenScrap 
[Info   : Unity Log] Queueing Soundbank for load proc_premDebuffOnHit 
[Info   : Unity Log] Queueing Soundbank for load use_bossHunter 
[Info   : Unity Log] Queueing Soundbank for load use_gummyClone 
[Info   : Unity Log] Queueing Soundbank for load use_molotov 
[Info   : Unity Log] Queueing Soundbank for load void_bleedOnHit 
[Info   : Unity Log] Queueing Soundbank for load void_mushroom 
[Info   : Unity Log] Queueing Soundbank for load void_critGlasses 
[Info   : Unity Log] Queueing Soundbank for load void_bear 
[Info   : Unity Log] Queueing Soundbank for load void_treasureCache 
[Info   : Unity Log] Queueing Soundbank for load void_missile 
[Info   : Unity Log] Queueing Soundbank for load void_chainLightning 
[Info   : Unity Log] Queueing Soundbank for load void_extraLife 
[Info   : Unity Log] Queueing Soundbank for load void_explodeOnDeath 
[Info   : Unity Log] Queueing Soundbank for load void_clover 
[Info   : Unity Log] Queueing Soundbank for load Mob_clayGrenadier 
[Info   : Unity Log] Queueing Soundbank for load Mob_flyingVermin 
[Info   : Unity Log] Queueing Soundbank for load Mob_blindVermin 
[Info   : Unity Log] Queueing Soundbank for load proc_strengthenBurn 
[Info   : Unity Log] Queueing Soundbank for load Mob_gup 
[Info   : Unity Log] Queueing Soundbank for load obj_eradicator 
[Info   : Unity Log] Queueing Soundbank for load obj_voidCradle 
[Info   : Unity Log] Queueing Soundbank for load char_railgunner 
[Info   : Unity Log] Queueing Soundbank for load Mob_minorConstruct 
[Info   : Unity Log] Queueing Soundbank for load Boss_majorConstruct 
[Info   : Unity Log] Queueing Soundbank for load Mob_voidDevastator 
[Info   : Unity Log] Queueing Soundbank for load mending 
[Info   : Unity Log] Queueing Soundbank for load void 
[Info   : Unity Log] Queueing Soundbank for load Mob_arenaCrab 
[Info   : Unity Log] Queueing Soundbank for load char_voidman 
[Info   : Unity Log] Queueing Soundbank for load proc_delicateWatch 
[Info   : Unity Log] Queueing Soundbank for load proc_scrapGoop 
[Info   : Unity Log] Queueing Soundbank for load obj_itemShip 
[Info   : Unity Log] Queueing Soundbank for load Mob_voidBarnacle 
[Info   : Unity Log] Queueing Soundbank for load Mob_voidJailer 
[Info   : Unity Log] Queueing Soundbank for load Boss_voidRaid 
[Info   : Unity Log] Soundbanks queued, pendingLoads = 111
[Info   : Unity Log] Queueing Soundbank for load zoneLakesDay 
[Info   : Unity Log] Queueing Soundbank for load zoneLakesNight 
[Info   : Unity Log] Queueing Soundbank for load zoneLemurianTemple 
[Info   : Unity Log] Queueing Soundbank for load zoneHabitat 
[Info   : Unity Log] Queueing Soundbank for load zoneHelminthRoost 
[Info   : Unity Log] Queueing Soundbank for load zoneMeridian 
[Info   : Unity Log] Queueing Soundbank for load obj_devotion_egg 
[Info   : Unity Log] Queueing Soundbank for load artifact_delusion 
[Info   : Unity Log] Queueing Soundbank for load item_proc_lowerPricedChest 
[Info   : Unity Log] Queueing Soundbank for load item_lunar_onLevelUpFreeUnlock 
[Info   : Unity Log] Queueing Soundbank for load item_proc_boostAllStats 
[Info   : Unity Log] Queueing Soundbank for load item_proc_delayedDamage 
[Info   : Unity Log] Queueing Soundbank for load item_proc_extraShrineItem 
[Info   : Unity Log] Queueing Soundbank for load item_proc_extraStatsOnLevelUp 
[Info   : Unity Log] Queueing Soundbank for load item_proc_goldOnStageStart 
[Info   : Unity Log] Queueing Soundbank for load item_proc_increaseDamageMultiKill 
[Info   : Unity Log] Queueing Soundbank for load item_proc_increasePrimaryDamage 
[Info   : Unity Log] Queueing Soundbank for load item_proc_knockBackHitEnemies 
[Info   : Unity Log] Queueing Soundbank for load item_proc_lowerHealthHigherDamage 
[Info   : Unity Log] Queueing Soundbank for load item_proc_lowerPricedChest 
[Warning: Unity Log] WwiseUnity: Trying to load soundbank that's already loaded: item_proc_lowerPricedChest
[Info   : Unity Log] Duplicate Soundbank loaded. 129 remaining
[Info   : Unity Log] Queueing Soundbank for load item_proc_meteorAttackOnHighDamage 
[Info   : Unity Log] Queueing Soundbank for load item_proc_negateAttack 
[Info   : Unity Log] Queueing Soundbank for load item_proc_stunAndPierce 
[Info   : Unity Log] Queueing Soundbank for load item_proc_teleportOnLowHealth 
[Info   : Unity Log] Queueing Soundbank for load item_proc_triggerEnemyDebuffs 
[Info   : Unity Log] Queueing Soundbank for load item_use_healAndRevive 
[Info   : Unity Log] Queueing Soundbank for load obj_shrineHalcyonite 
[Info   : Unity Log] Queueing Soundbank for load obj_falseSonCore 
[Info   : Unity Log] Queueing Soundbank for load obj_shrineColossus 
[Info   : Unity Log] Queueing Soundbank for load obj_chefWok 
[Info   : Unity Log] Queueing Soundbank for load char_chef 
[Info   : Unity Log] Queueing Soundbank for load char_seeker 
[Info   : Unity Log] Queueing Soundbank for load Boss_FalseSon 
[Info   : Unity Log] Queueing Soundbank for load zoneHabitatFall 
[Info   : Unity Log] Queueing Soundbank for load zoneVillage 
[Info   : Unity Log] Queueing Soundbank for load zoneVillageNight 
[Info   : Unity Log] Queueing Soundbank for load Mob_child 
[Info   : Unity Log] Queueing Soundbank for load Mob_halcyonite 
[Info   : Unity Log] Queueing Soundbank for load Mob_scorchling 
[Info   : Unity Log] Queueing Soundbank for load zoneArtifactWorld01 
[Info   : Unity Log] Queueing Soundbank for load zoneArtifactWorld02 
[Info   : Unity Log] Queueing Soundbank for load zoneArtifactWorld03 
[Info   : Unity Log] Queueing Soundbank for load obj_shrineShaping 
[Info   : Unity Log] Queueing Soundbank for load char_falseson 
[Info   : Unity Log] Soundbanks queued, pendingLoads = 154
[Info   : Unity Log] Soundbank 1689932043 loaded. 153 remaining
[Info   : Unity Log] Soundbank 4023194814 loaded. 152 remaining
[Info   : Unity Log] Soundbank 38422548 loaded. 151 remaining
[Info   : Unity Log] Soundbank 856009684 loaded. 150 remaining
[Info   : Unity Log] Soundbank 3926047285 loaded. 149 remaining
[Info   : Unity Log] Soundbank 4143562310 loaded. 148 remaining
[Info   : Unity Log] Soundbank 1465331116 loaded. 147 remaining
[Info   : Unity Log] Soundbank 9845752 loaded. 146 remaining
[Info   : Unity Log] Soundbank 2655849890 loaded. 145 remaining
[Info   : Unity Log] Soundbank 4187452863 loaded. 144 remaining
[Info   : Unity Log] Soundbank 404387444 loaded. 143 remaining
[Info   : Unity Log] Soundbank 1953513989 loaded. 142 remaining
[Info   : Unity Log] Soundbank 3413481868 loaded. 141 remaining
[Info   : Unity Log] Soundbank 3985223118 loaded. 140 remaining
[Info   : Unity Log] Soundbank 1966948937 loaded. 139 remaining
[Info   : Unity Log] Soundbank 1862879174 loaded. 138 remaining
[Info   : Unity Log] Soundbank 2246927104 loaded. 137 remaining
[Info   : Unity Log] Soundbank 1293665629 loaded. 136 remaining
[Info   : Unity Log] Soundbank 3991942870 loaded. 135 remaining
[Info   : Unity Log] Soundbank 2703043425 loaded. 134 remaining
[Info   : Unity Log] Soundbank 2514167983 loaded. 133 remaining
[Info   : Unity Log] Soundbank 1113084135 loaded. 132 remaining
[Info   : Unity Log] Soundbank 24228054 loaded. 131 remaining
[Info   : Unity Log] Soundbank 994832409 loaded. 130 remaining
[Info   : Unity Log] Soundbank 2504342686 loaded. 129 remaining
[Info   : Unity Log] Soundbank 3199121628 loaded. 128 remaining
[Info   : Unity Log] Soundbank 3554196939 loaded. 127 remaining
[Info   : Unity Log] Soundbank 3745641270 loaded. 126 remaining
[Info   : Unity Log] Soundbank 3489024319 loaded. 125 remaining
[Info   : Unity Log] Soundbank 1205451137 loaded. 124 remaining
[Info   : Unity Log] Soundbank 442554254 loaded. 123 remaining
[Info   : Unity Log] Soundbank 71570353 loaded. 122 remaining
[Info   : Unity Log] Soundbank 1309824022 loaded. 121 remaining
[Info   : Unity Log] Soundbank 345483614 loaded. 120 remaining
[Info   : Unity Log] Soundbank 1011852739 loaded. 119 remaining
[Info   : Unity Log] Soundbank 1059560718 loaded. 118 remaining
[Info   : Unity Log] Soundbank 1732406355 loaded. 117 remaining
[Info   : Unity Log] Soundbank 324352007 loaded. 116 remaining
[Info   :   Console] Facepunch.Steamworks Unity: WindowsPlayer
[Info   :   Console] Facepunch.Steamworks Os: Windows
[Info   :   Console] Facepunch.Steamworks Arch: x64
[Info   : Unity Log] Soundbank 2392758795 loaded. 115 remaining
[Info   : Unity Log] Soundbank 4282989037 loaded. 114 remaining
[Info   : Unity Log] Soundbank 1606673849 loaded. 113 remaining
[Info   : Unity Log] Soundbank 1374198700 loaded. 112 remaining
[Info   : Unity Log] Soundbank 1847546455 loaded. 111 remaining
[Info   : Unity Log] Soundbank 934222025 loaded. 110 remaining
[Info   : Unity Log] Soundbank 2359913785 loaded. 109 remaining
[Info   : Unity Log] Soundbank 615700520 loaded. 108 remaining
[Info   : Unity Log] Soundbank 1963867030 loaded. 107 remaining
[Info   : Unity Log] Soundbank 3572910250 loaded. 106 remaining
[Info   : Unity Log] Soundbank 3001196300 loaded. 105 remaining
[Info   : Unity Log] Soundbank 2891636949 loaded. 104 remaining
[Info   : Unity Log] Soundbank 3167730908 loaded. 103 remaining
[Info   : Unity Log] Soundbank 3203567460 loaded. 102 remaining
[Info   : Unity Log] Soundbank 4092664088 loaded. 101 remaining
[Info   : Unity Log] Soundbank 2430928744 loaded. 100 remaining
[Info   : Unity Log] Soundbank 2556564726 loaded. 99 remaining
[Info   : Unity Log] Soundbank 3371428416 loaded. 98 remaining
[Info   : Unity Log] Soundbank 3032470812 loaded. 97 remaining
[Info   : Unity Log] Soundbank 3238167061 loaded. 96 remaining
[Info   : Unity Log] Soundbank 1018875288 loaded. 95 remaining
[Info   : Unity Log] Soundbank 106168028 loaded. 94 remaining
[Info   : Unity Log] Soundbank 1370025320 loaded. 93 remaining
[Info   : Unity Log] Soundbank 2142415572 loaded. 92 remaining
[Info   : Unity Log] Soundbank 2737679425 loaded. 91 remaining
[Info   : Unity Log] Soundbank 2594581044 loaded. 90 remaining
[Info   : Unity Log] Soundbank 3799815731 loaded. 89 remaining
[Info   : Unity Log] Soundbank 610066632 loaded. 88 remaining
[Info   : Unity Log] Soundbank 426133298 loaded. 87 remaining
[Info   : Unity Log] Soundbank 2895515998 loaded. 86 remaining
[Info   : Unity Log] Soundbank 3742616922 loaded. 85 remaining
[Info   : Unity Log] Soundbank 3823187751 loaded. 84 remaining
[Info   : Unity Log] Soundbank 2279537123 loaded. 83 remaining
[Info   : Unity Log] Soundbank 2023888034 loaded. 82 remaining
[Info   : Unity Log] Soundbank 3180914867 loaded. 81 remaining
[Info   : Unity Log] Soundbank 3843131111 loaded. 80 remaining
[Info   : Unity Log] Soundbank 499416135 loaded. 79 remaining
[Info   : Unity Log] Soundbank 1566663610 loaded. 78 remaining
[Info   : Unity Log] Soundbank 3247940604 loaded. 77 remaining
[Info   : Unity Log] Soundbank 2027800112 loaded. 76 remaining
[Info   : Unity Log] Soundbank 1087440883 loaded. 75 remaining
[Info   : Unity Log] Soundbank 2657263305 loaded. 74 remaining
[Info   : Unity Log] Soundbank 3483997436 loaded. 73 remaining
[Info   : Unity Log] Soundbank 4086344396 loaded. 72 remaining
[Info   : Unity Log] Soundbank 3813512022 loaded. 71 remaining
[Info   : Unity Log] Soundbank 962057104 loaded. 70 remaining
[Info   : Unity Log] Soundbank 3523761873 loaded. 69 remaining
[Info   : Unity Log] Soundbank 1529142338 loaded. 68 remaining
[Info   : Unity Log] Soundbank 120132577 loaded. 67 remaining
[Info   : Unity Log] Soundbank 4155162766 loaded. 66 remaining
[Info   : Unity Log] Soundbank 158907880 loaded. 65 remaining
[Info   : Unity Log] Soundbank 789966885 loaded. 64 remaining
[Info   : Unity Log] Soundbank 3196514094 loaded. 63 remaining
[Info   : Unity Log] Soundbank 3027377960 loaded. 62 remaining
[Info   : Unity Log] Soundbank 3508190176 loaded. 61 remaining
[Info   : Unity Log] Soundbank 949121333 loaded. 60 remaining
[Info   : Unity Log] Soundbank 1147567708 loaded. 59 remaining
[Info   : Unity Log] Soundbank 2062804905 loaded. 58 remaining
[Info   : Unity Log] Soundbank 1537627768 loaded. 57 remaining
[Info   : Unity Log] Soundbank 3915059737 loaded. 56 remaining
[Info   : Unity Log] Soundbank 1193528542 loaded. 55 remaining
[Info   : Unity Log] Soundbank 1184992503 loaded. 54 remaining
[Info   : Unity Log] Soundbank 487167457 loaded. 53 remaining
[Info   : Unity Log] Soundbank 2310155815 loaded. 52 remaining
[Info   : Unity Log] Soundbank 3370470011 loaded. 51 remaining
[Info   : Unity Log] Soundbank 3141563463 loaded. 50 remaining
[Info   : Unity Log] Soundbank 1011090874 loaded. 49 remaining
[Info   : Unity Log] Soundbank 2062467274 loaded. 48 remaining
[Info   : Unity Log] Soundbank 3832200916 loaded. 47 remaining
[Info   : Unity Log] Soundbank 1039323568 loaded. 46 remaining
[Info   : Unity Log] Soundbank 2126286228 loaded. 45 remaining
[Info   : Unity Log] Soundbank 1642391815 loaded. 44 remaining
[Info   : Unity Log] Soundbank 2032027227 loaded. 43 remaining
[Info   : Unity Log] Soundbank 873742569 loaded. 42 remaining
[Info   : Unity Log] Soundbank 2280204525 loaded. 41 remaining
[Info   : Unity Log] Soundbank 1518149405 loaded. 40 remaining
[Info   : Unity Log] Soundbank 3363815448 loaded. 39 remaining
[Info   : Unity Log] Soundbank 153869071 loaded. 38 remaining
[Info   : Unity Log] Soundbank 2103916950 loaded. 37 remaining
[Info   : Unity Log] Soundbank 2080865335 loaded. 36 remaining
[Info   : Unity Log] Soundbank 835443085 loaded. 35 remaining
[Info   : Unity Log] Soundbank 58800115 loaded. 34 remaining
[Info   : Unity Log] Soundbank 577000050 loaded. 33 remaining
[Info   : Unity Log] Soundbank 4082005121 loaded. 32 remaining
[Info   : Unity Log] Soundbank 1529670391 loaded. 31 remaining
[Info   : Unity Log] Soundbank 827742080 loaded. 30 remaining
[Info   : Unity Log] Soundbank 730325267 loaded. 29 remaining
[Info   : Unity Log] Soundbank 3880180457 loaded. 28 remaining
[Info   : Unity Log] Soundbank 4273639584 loaded. 27 remaining
[Info   : Unity Log] Soundbank 38761679 loaded. 26 remaining
[Info   : Unity Log] Soundbank 3574570002 loaded. 25 remaining
[Info   : Unity Log] Soundbank 128152461 loaded. 24 remaining
[Info   : Unity Log] Soundbank 3618799142 loaded. 23 remaining
[Info   : Unity Log] Soundbank 3663841528 loaded. 22 remaining
[Info   : Unity Log] Soundbank 1677239337 loaded. 21 remaining
[Info   : Unity Log] Soundbank 2181585632 loaded. 20 remaining
[Info   : Unity Log] Soundbank 1512115115 loaded. 19 remaining
[Info   : Unity Log] Soundbank 2380027467 loaded. 18 remaining
[Info   : Unity Log] Soundbank 3199335826 loaded. 17 remaining
[Info   : Unity Log] Soundbank 1765431557 loaded. 16 remaining
[Info   : Unity Log] Soundbank 1104208749 loaded. 15 remaining
[Info   : Unity Log] Soundbank 2603442700 loaded. 14 remaining
[Info   : Unity Log] Soundbank 3381863324 loaded. 13 remaining
[Info   : Unity Log] Soundbank 916174285 loaded. 12 remaining
[Info   : Unity Log] Soundbank 2095108018 loaded. 11 remaining
[Info   : Unity Log] Soundbank 2995251179 loaded. 10 remaining
[Info   : Unity Log] Soundbank 1139200105 loaded. 9 remaining
[Info   : Unity Log] Soundbank 811525985 loaded. 8 remaining
[Info   : Unity Log] Soundbank 4014481054 loaded. 7 remaining
[Info   : Unity Log] Soundbank 863098608 loaded. 6 remaining
[Info   : Unity Log] Soundbank 2995767272 loaded. 5 remaining
[Info   : Unity Log] Soundbank 4203513218 loaded. 4 remaining
[Info   : Unity Log] Soundbank 4203513217 loaded. 3 remaining
[Info   : Unity Log] Soundbank 4203513216 loaded. 2 remaining
[Info   : Unity Log] Soundbank 99581196 loaded. 1 remaining
[Info   : Unity Log] Soundbank 3072344347 loaded. 0 remaining
[Info   : Unity Log] Initializing Mod System...
[Info   : Unity Log] Mod System initialized.
[Warning: Unity Log] The referenced script on this Behaviour (Game Object '<null>') is missing!
[Warning: Unity Log] The referenced script (Rewired.Platforms.Switch.NintendoSwitchInputManager) on this Behaviour is missing!
[Warning: Unity Log] The referenced script on this Behaviour (Game Object 'Rewired Input Manager') is missing!
[Info   : Unity Log] SCANNING ASSEMBLIES (not using cached searchables/assemblies!)
[Debug  :RoR2BepInExPack] Not scanning R2API.Utils.EmbeddedResources for ConVars due to it containing delegate pointer field(s)
[Info   : Unity Log] "usesocialiconflag" is not a recognized ConCommand or ConVar.
[Info   : Unity Log] This command requires a network or game session.
[Warning: Unity Log] Network message MsgType.Highest + 22 is unregistered.
[Info   : Unity Log] Setting current language to "en"
[Info   : Unity Log] Begin Shader Loading
[Error  : Unity Log] UnityEngine.AddressableAssets.InvalidKeyException: Exception of type 'UnityEngine.AddressableAssets.InvalidKeyException' was thrown., Key=86212504c7e9f468db2300dc5932dc17, Type=UnityEngine.Shader
[Info   : Unity Log] loaded a shader!:  Shaders/Deferred/HGStandard / 48dca5b99d113b8d11006bab44295342
[Info   : Unity Log] Error loading shader: Hidden/ProBuilder/EdgePicker / 86212504c7e9f468db2300dc5932dc17
[Info   : Unity Log] loaded a shader!:  Hopoo Games/Deferred/Standard / 48dca5b99d113b8d11006bab44295342
[Info   : Unity Log] loaded a shader!:  TranslucentImage/FillCrop / d996ab4381100014d99e25e68e9aac84
[Info   : Unity Log] loaded a shader!:  TranslucentImage/EfficientBlur / 12e87e7c7fde8e74db9f75172456c5c3
[Info   : Unity Log] loaded a shader!:  Hidden/PostProcess/SceneTint / 14aa4a59af5f6d348a6ef6051b0aa7cb
[Info   : Unity Log] loaded a shader!:  Hidden/PostProcess/SobelRain / b7d96c4da51704442b8cfedb1872b45f
[Info   : Unity Log] loaded a shader!:  Hidden/PostProcess/SobelOutline / 7d6541fe01591cd418caf48d3c099e52
[Info   : Unity Log] loaded a shader!:  Hidden/PostProcess/RampFog / c14b6abb309b8c246add9e94a6920d7f
[Info   : Unity Log] loaded a shader!:  Hidden/PostProcessing/HopooSSR / c2c5464695b95d34c833a4c0642e7532
[Info   : Unity Log] loaded a shader!:  Hidden/NGSS_Directional / d42da1e69d53bc044ad4a55ce9a410c1
[Info   : Unity Log] loaded a shader!:  Hidden/NGSS_ContactShadows / ab1a11124b683ad44bcb86515a434172
[Info   : Unity Log] loaded a shader!:  UI/TranslucentImage / b1115addd36579a429d5e6b4ffae668d
[Info   : Unity Log] loaded a shader!:  Hidden/FillCrop / d996ab4381100014d99e25e68e9aac84
[Info   : Unity Log] loaded a shader!:  Hidden/EfficientBlur / 12e87e7c7fde8e74db9f75172456c5c3
[Info   : Unity Log] loaded a shader!:  Decalicious/Unlit Decal / 3f698aa8b620b764d962905c27b9f1ae
[Info   : Unity Log] loaded a shader!:  Decalicious/Deferred Decal / 45d0c9045cf6b7f44b2550a667173c5f
[Info   : Unity Log] loaded a shader!:  Hidden/Decalicious Game Object ID / af51ca0e06c840e4b9efd34efed7cc85
[Info   : Unity Log] loaded a shader!:  Projector/Caustics / 28260a8d98706fe40a3c99f2f781b3cb
[Info   : Unity Log] loaded a shader!:  CalmWater/Calm Water [Single Sided] / 5113337feed5cb641a5590972b07b9c1
[Info   : Unity Log] loaded a shader!:  CalmWater/Calm Water [DX11] / 2e5d60e33aa6da447bb69ec37053c5ba
[Info   : Unity Log] loaded a shader!:  CalmWater/Calm Water [DX11] [Double Sided] / c960b2fa5c6943a47bf5f9dbab77ea35
[Info   : Unity Log] loaded a shader!:  CalmWater/Calm Water [Double Sided] / 64cc94e88500a904084a18cf5fcb2991
[Info   : Unity Log] loaded a shader!:  Hidden/UnderWaterFog / 472da7a0e387cf14abf12a99c96998fd
[Info   : Unity Log] loaded a shader!:  TranslucentImage/DownTent / e4abd95960ed75f4a9f59cac7c40bd9d
[Info   : Unity Log] loaded a shader!:  TranslucentImage/QuickDumbBlur / b12e02e9a9b684543a286089a03cb04b
[Info   : Unity Log] loaded a shader!:  Shaders/UI/HGUIOverbrighten / cf1ebead4b1ec6c4591b2066773bffd7
[Info   : Unity Log] loaded a shader!:  Shaders/UI/HGUIIgnoreZ / 36643fea3e3d2a844ac2e6e777f5b249
[Info   : Unity Log] loaded a shader!:  Shaders/UI/HGUICustomBlend / 6ecc105be1511d240b11e32eb05f80e8
[Info   : Unity Log] loaded a shader!:  Shaders/UI/HGUIBlur / 84464b76c258972459d5ed8d5a875d0b
[Info   : Unity Log] loaded a shader!:  Shaders/UI/HGUIBarRemap / fc58d915158fa30429e09867bf1a1929
[Info   : Unity Log] loaded a shader!:  Shaders/UI/HGUIAnimateAlpha / de7d3aae599afd94491a236cc750f320
[Info   : Unity Log] loaded a shader!:  Shaders/Speedtree Override/TreeSoftOcclusionLeaves / 4420bd398c9be744082b5d2458829746
[Info   : Unity Log] loaded a shader!:  Shaders/Speedtree Override/TreeSoftOcclusionBark / cea4b848c6b05db458c71e5ad1a005b3
[Info   : Unity Log] loaded a shader!:  Shaders/Speedtree Override/SpeedTreeCustom / 85eebb34728e99141abefe9e3f234e55
[Info   : Unity Log] loaded a shader!:  Shaders/Speedtree Override/SpeedTreeBillboard / 72ed34a768bf9604bbfe1e9fe1edbbb2
[Info   : Unity Log] loaded a shader!:  Shaders/PostProcess/HGVisionLimit / 5e246b703e2090148a761d52af19744b
[Info   : Unity Log] loaded a shader!:  Shaders/PostProcess/HGSobelBuffer / 780f79b2a62a0df439dedf59c533eee6
[Info   : Unity Log] loaded a shader!:  Shaders/PostProcess/HGScreenDamage / 62decc840d5afaa49886d8b13165939e
[Info   : Unity Log] loaded a shader!:  Shaders/PostProcess/HGScopeShader / c59dc7ff2931ab5409d6ab6a95e504fb
[Info   : Unity Log] loaded a shader!:  Shaders/PostProcess/HGOutlineHighlight / 41e90cbc5ad198a438128b019f8d8553
[Info   : Unity Log] loaded a shader!:  Shaders/MobileBlur / f9d5fa183cd6b45eeb1491f74863cd91
[Info   : Unity Log] loaded a shader!:  Shaders/FX/HGVertexOnly / 3b2fa336b2f421746a875f53f075d95f
[Info   : Unity Log] loaded a shader!:  Shaders/FX/HGSolidParallax / 302e1057ea9d0e74dab5a0de5cbf611c
[Info   : Unity Log] loaded a shader!:  Shaders/FX/HGOpaqueCloudRemap / a035a371a79a19c468ec4e6dc40911c5
[Info   : Unity Log] loaded a shader!:  Shaders/FX/HGIntersectionCloudRemap / 43a6c7a9084ef9743ab45ee8d5f3c4e9
[Info   : Unity Log] loaded a shader!:  Shaders/FX/HGForwardPlanet / 94b2ede73cf555f4f8549dc24b957446
[Info   : Unity Log] loaded a shader!:  Shaders/FX/HGDistortion / f6bd449dcf2a4496da3d2ad0c3881450
[Info   : Unity Log] loaded a shader!:  Shaders/FX/HGDamageNumber / 7d8dad6ac5790494cafac7c5a3fcb748
[Info   : Unity Log] loaded a shader!:  Shaders/FX/HGCloudRemap / bbffe49749c91724d819563daf91445d
[Info   : Unity Log] loaded a shader!:  Shaders/Environment/HGWaterfall / 38f45f1c98f056f4ca78cb3f37bcc47d
[Info   : Unity Log] loaded a shader!:  Shaders/Environment/HGGrass / cb13e29f56673694cbaeb73c22d3cd1c
[Info   : Unity Log] loaded a shader!:  Shaders/Environment/HGDistantWater / d48a4aa52cd665f45a89801d053c38de
[Info   : Unity Log] loaded a shader!:  Shaders/Deferred/Internal-DeferredShadingCustom / 5b1b52b10f4dd5047b8400977ff4c0d7
[Info   : Unity Log] loaded a shader!:  Shaders/Deferred/Internal-DeferredReflectionsCustom / 40eea4bc126f35642be7798d96f9f7c1
[Info   : Unity Log] loaded a shader!:  Shaders/Deferred/Internal-DeferredReflections / 441e313ad6852fb47825253de68f351f
[Info   : Unity Log] loaded a shader!:  Shaders/Deferred/HGWavyCloth / 69d9da0a01c9f774e8e80f16ecd381b0
[Info   : Unity Log] loaded a shader!:  Shaders/Deferred/HGTriplanarTerrainBlend / cd44d5076b47fbc4d8872b2a500b78f8
[Info   : Unity Log] loaded a shader!:  Shaders/Deferred/HGStandardWithDistortion / 583195a0938e3d944b20d158ef28e3be
[Info   : Unity Log] loaded a shader!:  Shaders/Deferred/HGSnowTopped / ec2c273472427df41846b25c110155c2
[Info   : Unity Log] loaded a shader!:  Shaders/TMP_Sprite / cf81c85f95fe47e1a27f6ae460cf182c
[Info   : Unity Log] loaded a shader!:  Shaders/TMP_SDF-Surface-Mobile / 85187c2149c549c5b33f0cdb02836b17
[Info   : Unity Log] loaded a shader!:  Shaders/TMP_SDF-Surface / f7ada0af4f174f0694ca6a487b8f543d
[Info   : Unity Log] loaded a shader!:  Shaders/TMP_SDF-Mobile / fe393ace9b354375a9cb14cdbbc28be4
[Info   : Unity Log] loaded a shader!:  Shaders/TMP_SDF-Mobile Overlay / a02a7d8c237544f1962732b55a9aebf1
[Info   : Unity Log] loaded a shader!:  Shaders/TMP_SDF-Mobile Masking / bc1ede39bf3643ee8e493720e4259791
[Info   : Unity Log] loaded a shader!:  Shaders/TMP_SDF / 68e6db2ebdc24f95958faec2be5558d6
[Info   : Unity Log] loaded a shader!:  Shaders/TMP_SDF Overlay / dd89cf5b9246416f84610a006f916af7
[Info   : Unity Log] loaded a shader!:  Shaders/TMP_Bitmap-Mobile / 1e3b057af24249748ff873be7fafee47
[Info   : Unity Log] loaded a shader!:  Shaders/TMP_Bitmap-Custom-Atlas / 48bb5f55d8670e349b6e614913f9d910
[Info   : Unity Log] loaded a shader!:  Shaders/TMP_Bitmap / 128e987d567d4e2c824d754223b3f3b0
[Info   : Unity Log] loaded a shader!:  Hopoo Games/Internal/VisionLimit / 5e246b703e2090148a761d52af19744b
[Info   : Unity Log] loaded a shader!:  TextMeshPro/Mobile/Distance Field SSD / c8d12adcee749c344b8117cf7c7eb912
[Info   : Unity Log] loaded a shader!:  TextMeshPro/Distance Field SSD / 14eb328de4b8eb245bb7cea29e4ac00b
[Info   : Unity Log] loaded a shader!:  TextMeshPro/Sprite / cf81c85f95fe47e1a27f6ae460cf182c
[Info   : Unity Log] loaded a shader!:  TextMeshPro/Distance Field / 68e6db2ebdc24f95958faec2be5558d6
[Info   : Unity Log] loaded a shader!:  TextMeshPro/Distance Field (Surface) / f7ada0af4f174f0694ca6a487b8f543d
[Info   : Unity Log] loaded a shader!:  TextMeshPro/Mobile/Distance Field (Surface) / 85187c2149c549c5b33f0cdb02836b17
[Info   : Unity Log] loaded a shader!:  TextMeshPro/Mobile/Distance Field / fe393ace9b354375a9cb14cdbbc28be4
[Info   : Unity Log] loaded a shader!:  TextMeshPro/Mobile/Distance Field Overlay / a02a7d8c237544f1962732b55a9aebf1
[Info   : Unity Log] loaded a shader!:  TextMeshPro/Mobile/Distance Field - Masking / bc1ede39bf3643ee8e493720e4259791
[Info   : Unity Log] loaded a shader!:  TextMeshPro/Distance Field Overlay / dd89cf5b9246416f84610a006f916af7
[Info   : Unity Log] loaded a shader!:  TextMeshPro/Bitmap / 128e987d567d4e2c824d754223b3f3b0
[Info   : Unity Log] loaded a shader!:  TextMeshPro/Mobile/Bitmap / 1e3b057af24249748ff873be7fafee47
[Info   : Unity Log] loaded a shader!:  TextMeshPro/Bitmap Custom Atlas / 48bb5f55d8670e349b6e614913f9d910
[Info   : Unity Log] loaded a shader!:  Hopoo Games/UI/Default Overbrighten / cf1ebead4b1ec6c4591b2066773bffd7
[Info   : Unity Log] loaded a shader!:  Hopoo Games/UI/Debug Ignore Z / 36643fea3e3d2a844ac2e6e777f5b249
[Info   : Unity Log] loaded a shader!:  Hopoo Games/UI/Custom Blend / 6ecc105be1511d240b11e32eb05f80e8
[Info   : Unity Log] loaded a shader!:  Hopoo Games/UI/Masked UI Blur / 84464b76c258972459d5ed8d5a875d0b
[Info   : Unity Log] loaded a shader!:  Hopoo Games/UI/UI Bar Remap / fc58d915158fa30429e09867bf1a1929
[Info   : Unity Log] loaded a shader!:  Hopoo Games/UI/Animate Alpha / de7d3aae599afd94491a236cc750f320
[Info   : Unity Log] loaded a shader!:  Nature/Tree Soft Occlusion Leaves / 4420bd398c9be744082b5d2458829746
[Info   : Unity Log] loaded a shader!:  Nature/Tree Soft Occlusion Bark / cea4b848c6b05db458c71e5ad1a005b3
[Info   : Unity Log] loaded a shader!:  Nature/SpeedTree / 85eebb34728e99141abefe9e3f234e55
[Info   : Unity Log] loaded a shader!:  Nature/SpeedTree Billboard / 72ed34a768bf9604bbfe1e9fe1edbbb2
[Info   : Unity Log] loaded a shader!:  Hopoo Games/Internal/SobelBuffer / 780f79b2a62a0df439dedf59c533eee6
[Info   : Unity Log] loaded a shader!:  Hopoo Games/Post Process/Screen Damage / 62decc840d5afaa49886d8b13165939e
[Info   : Unity Log] loaded a shader!:  Hopoo Games/Post Process/Scope Distortion / c59dc7ff2931ab5409d6ab6a95e504fb
[Info   : Unity Log] loaded a shader!:  Hopoo Games/Internal/Outline Highlight / 41e90cbc5ad198a438128b019f8d8553
[Info   : Unity Log] loaded a shader!:  Hidden/FastBlur / f9d5fa183cd6b45eeb1491f74863cd91
[Info   : Unity Log] loaded a shader!:  Hopoo Games/FX/Vertex Colors Only / 3b2fa336b2f421746a875f53f075d95f
[Info   : Unity Log] loaded a shader!:  Hopoo Games/FX/Solid Parallax / 302e1057ea9d0e74dab5a0de5cbf611c
[Info   : Unity Log] loaded a shader!:  Hopoo Games/FX/Opaque Cloud Remap / a035a371a79a19c468ec4e6dc40911c5
[Info   : Unity Log] loaded a shader!:  Hopoo Games/FX/Cloud Intersection Remap / 43a6c7a9084ef9743ab45ee8d5f3c4e9
[Info   : Unity Log] loaded a shader!:  Hopoo Games/FX/Forward Planet / 94b2ede73cf555f4f8549dc24b957446
[Info   : Unity Log] loaded a shader!:  Hopoo Games/FX/Distortion / f6bd449dcf2a4496da3d2ad0c3881450
[Info   : Unity Log] loaded a shader!:  Hopoo Games/FX/Damage Number / 7d8dad6ac5790494cafac7c5a3fcb748
[Info   : Unity Log] loaded a shader!:  Hopoo Games/FX/Cloud Remap / bbffe49749c91724d819563daf91445d
[Info   : Unity Log] loaded a shader!:  Hopoo Games/Environment/Waterfall / 38f45f1c98f056f4ca78cb3f37bcc47d
[Info   : Unity Log] loaded a shader!:  Hopoo Games/Environment/Waving Grass / cb13e29f56673694cbaeb73c22d3cd1c
[Info   : Unity Log] loaded a shader!:  Hopoo Games/Environment/Distant Water / d48a4aa52cd665f45a89801d053c38de
[Info   : Unity Log] loaded a shader!:  Hidden/Internal-DeferredShading / 5b1b52b10f4dd5047b8400977ff4c0d7
[Info   : Unity Log] loaded a shader!:  Hidden/Internal-DeferredReflections / 40eea4bc126f35642be7798d96f9f7c1
[Info   : Unity Log] loaded a shader!:  Hopoo Games/Deferred/Wavy Cloth / 69d9da0a01c9f774e8e80f16ecd381b0
[Info   : Unity Log] loaded a shader!:  Hopoo Games/Deferred/Triplanar Terrain Blend / cd44d5076b47fbc4d8872b2a500b78f8
[Info   : Unity Log] loaded a shader!:  Hopoo Games/Deferred/Snow Topped / ec2c273472427df41846b25c110155c2
[Info   : Unity Log] loaded a shader!:  Custom/Fill-Linear / 4aa2a800cd44e904fbd41952f82c6370
[Info   : Unity Log] End Shader Loading
[Info   :R2API.ContentManagement] Generating a total of 2 ContentPacks...
[Warning: Unity Log] Internal: JobTempAlloc has allocations that are more than 4 frames old - this is not allowed and likely a leak
[Warning: Unity Log] EffectManager: Killing all pools
[Info   : Unity Log] <color=green>Triggered MusicController</color>
[Error  : Unity Log] ArgumentException: The Object you want to instantiate is null.
Stack trace:
UnityEngine.Object.Instantiate[T] (T original) (at <a20b3695b7ce4017b7981f9d06962bd1>:IL_003D)
RoR2.PostProcessing.DamageIndicator.Awake () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:IL_000C)

[Warning: Unity Log] Internal: deleting an allocation that is older than its permitted lifetime of 4 frames (age = 7)
[Warning: Unity Log] Internal: deleting an allocation that is older than its permitted lifetime of 4 frames (age = 7)
[Warning: Unity Log] EffectManager: Killing all pools
[Warning: Unity Log] Internal: JobTempAlloc has allocations that are more than 4 frames old - this is not allowed and likely a leak
[Warning: Unity Log] Failed to assign Items.LemurianHarness: Asset "LemurianHarness" not found.
[Warning: Unity Log] Failed to assign Buffs.TamperedHeart: Asset "bdTamperedHeart" not found.
[Warning: Unity Log] Failed to assign Buffs.FreeChest: Asset "bdFreeChest" not found.
[Warning: Unity Log] Failed to assign Buffs.Deafened: Asset "bdDeafened" not found.
[Warning: Unity Log] Failed to assign Items.SummonedEcho: Asset "SummonedEcho" not found.
[Warning: Unity Log] Failed to assign Equipment.AffixEcho: Asset "EliteEchoEquipment" not found.
[Warning: Unity Log] Failed to assign Buffs.AffixEcho: Asset "bdEliteEcho" not found.
[Warning: Unity Log] Failed to assign Elites.Echo: Asset "edEcho" not found.
[Info   :R2API.ContentManagement] Created a SerializableContentPack for mod com.TheTimesweeper.RedAlert
[Debug  :RoR2BepInExPack] OnHook R2API.PrefabAPI.SetProperAssetIdForDelayedPrefabs added by assembly: R2API.Prefab.dll for: RoR2.Networking.NetworkManagerSystem.OnStartClient
[Warning: Unity Log] The referenced script on this Behaviour (Game Object '<null>') is missing!
[Warning: Unity Log] The referenced script (Xft.XWeaponTrail) on this Behaviour is missing!
[Warning: Unity Log] The referenced script on this Behaviour (Game Object 'XTrail') is missing!
[Warning: Unity Log] The referenced script on this Behaviour (Game Object '<null>') is missing!
[Warning: Unity Log] The referenced script (Xft.XWeaponTrail) on this Behaviour is missing!
[Warning: Unity Log] The referenced script on this Behaviour (Game Object 'XTrail') is missing!
[Debug  : Red Alert] matTesla_Armor has already been loaded. returning cached
[Debug  : Red Alert] matTesla_Armor has already been loaded. returning cached
[Debug  : Red Alert] matTesla_Armor has already been loaded. returning cached
[Debug  : Red Alert] matTesla_Body has already been loaded. returning cached
[Debug  : Red Alert] matTesla_Armor has already been loaded. returning cached
[Debug  : Red Alert] matChronoArmor has already been loaded. returning cached
[Debug  : Red Alert] matChronoBackpack has already been loaded. returning cached
[Debug  : Red Alert] matChronoDecals is not unity standard shader. aborting material conversion
[Debug  : Red Alert] matChronoGun has already been loaded. returning cached
[Debug  : Red Alert] matChronoEmission has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 03 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 03 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 03 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 03 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 0fdsa - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 03 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 03 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 03 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 03 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 03 - Default has already been loaded. returning cached
[Debug  : Red Alert] bled0045_Specular 1 has already been loaded. returning cached
[Debug  : Red Alert] bl2 has already been loaded. returning cached
[Debug  : Red Alert] bled0045_Specular has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] matTesla_Armor has already been loaded. returning cached
[Debug  : Red Alert] matTesla_Armor has already been loaded. returning cached
[Debug  : Red Alert] matTesla_Body has already been loaded. returning cached
[Debug  : Red Alert] matTesla_Armor has already been loaded. returning cached
[Debug  : Red Alert] matTesla_Body has already been loaded. returning cached
[Debug  : Red Alert] matTesla_Armor has already been loaded. returning cached
[Debug  : Red Alert] matTesla_Armor has already been loaded. returning cached
[Debug  :RoR2BepInExPack] OnHook R2API.DamageAPI.WriteDamageInfo added by assembly: R2API.DamageType.dll for: RoR2.NetworkExtensions.Write
[Debug  :RoR2BepInExPack] OnHook R2API.DamageAPI.ReadDamageInfo added by assembly: R2API.DamageType.dll for: RoR2.NetworkExtensions.ReadDamageInfo
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.BulletAttackDefaultHitCallbackIL modifier by assembly: R2API.DamageType.dll for: RoR2.BulletAttack.DefaultHitCallbackImplementation
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.DamageOrbOnArrivalIL modifier by assembly: R2API.DamageType.dll for: RoR2.Orbs.DamageOrb.OnArrival
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.GenericDamageOrbOnArrivalIL modifier by assembly: R2API.DamageType.dll for: RoR2.Orbs.GenericDamageOrb.OnArrival
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.LightningOrbOnArrivalIL modifier by assembly: R2API.DamageType.dll for: RoR2.Orbs.LightningOrb.OnArrival
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.ChainGunOrbOnArrivalIL modifier by assembly: R2API.DamageType.dll for: RoR2.Orbs.ChainGunOrb.OnArrival
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.DeathProjectileFixedUpdateIL modifier by assembly: R2API.DamageType.dll for: RoR2.Projectile.DeathProjectile.FixedUpdate
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.ProjectileDotZoneResetOverlapIL modifier by assembly: R2API.DamageType.dll for: RoR2.Projectile.ProjectileDotZone.ResetOverlap
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.ProjectileExplosionDetonateServerIL modifier by assembly: R2API.DamageType.dll for: RoR2.Projectile.ProjectileExplosion.DetonateServer
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.ProjectileGrantOnKillOnDestroyOnDestroyIL modifier by assembly: R2API.DamageType.dll for: RoR2.Projectile.ProjectileGrantOnKillOnDestroy.OnDestroy
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.ProjectileIntervalOverlapAttackFixedUpdateIL modifier by assembly: R2API.DamageType.dll for: RoR2.Projectile.ProjectileIntervalOverlapAttack.FixedUpdate
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.ProjectileOverlapAttackStartIL modifier by assembly: R2API.DamageType.dll for: RoR2.Projectile.ProjectileOverlapAttack.Start
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.ProjectileOverlapAttackResetOverlapAttackIL modifier by assembly: R2API.DamageType.dll for: RoR2.Projectile.ProjectileOverlapAttack.ResetOverlapAttack
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.ProjectileProximityBeamControllerUpdateServerIL modifier by assembly: R2API.DamageType.dll for: RoR2.Projectile.ProjectileProximityBeamController.UpdateServer
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.ProjectileSingleTargetImpactOnProjectileImpactIL modifier by assembly: R2API.DamageType.dll for: RoR2.Projectile.ProjectileSingleTargetImpact.OnProjectileImpact
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.DotControllerEvaluateDotStacksForTypeIL modifier by assembly: R2API.DamageType.dll for: RoR2.DotController.EvaluateDotStacksForType
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.DotControllerAddPendingDamageEntryIL modifier by assembly: R2API.DamageType.dll for: RoR2.DotController.AddPendingDamageEntry
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.BlastAttackHandleHitsIL modifier by assembly: R2API.DamageType.dll for: RoR2.BlastAttack.HandleHits
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.BlastAttackPerformDamageServerIL modifier by assembly: R2API.DamageType.dll for: RoR2.BlastAttack.PerformDamageServer
[Debug  :RoR2BepInExPack] OnHook R2API.DamageAPI.BlastAttackDamageInfoWrite added by assembly: R2API.DamageType.dll for: RoR2.BlastAttack+BlastAttackDamageInfo.Write
[Debug  :RoR2BepInExPack] OnHook R2API.DamageAPI.BlastAttackDamageInfoRead added by assembly: R2API.DamageType.dll for: RoR2.BlastAttack+BlastAttackDamageInfo.Read
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.OverlapAttackProcessHitsIL modifier by assembly: R2API.DamageType.dll for: RoR2.OverlapAttack.ProcessHits
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.OverlapAttackPerformDamageIL modifier by assembly: R2API.DamageType.dll for: RoR2.OverlapAttack.PerformDamage
[Debug  :RoR2BepInExPack] OnHook R2API.DamageAPI.OverlapAttackMessageSerialize added by assembly: R2API.DamageType.dll for: RoR2.OverlapAttack+OverlapAttackMessage.Serialize
[Debug  :RoR2BepInExPack] OnHook R2API.DamageAPI.OverlapAttackMessageDeserialize added by assembly: R2API.DamageType.dll for: RoR2.OverlapAttack+OverlapAttackMessage.Deserialize
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.GlobalEventManagerOnHitAllIL modifier by assembly: R2API.DamageType.dll for: RoR2.GlobalEventManager.OnHitAllProcess
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.HealthComponentSendDamageDealtIL modifier by assembly: R2API.DamageType.dll for: RoR2.HealthComponent.SendDamageDealt
[Debug  :RoR2BepInExPack] OnHook R2API.DamageAPI.DamageDealtMessageSerialize added by assembly: R2API.DamageType.dll for: RoR2.DamageDealtMessage.Serialize
[Debug  :RoR2BepInExPack] OnHook R2API.DamageAPI.DamageDealtMessageDeserialize added by assembly: R2API.DamageType.dll for: RoR2.DamageDealtMessage.Deserialize
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.ContactDamageFireOverlapsIL modifier by assembly: R2API.DamageType.dll for: RoR2.ContactDamage.FireOverlaps
[Debug  :RoR2BepInExPack] ILHook R2API.DamageAPI.DelayBlastDetonateIL modifier by assembly: R2API.DamageType.dll for: RoR2.DelayBlast.Detonate
[Debug  :RoR2BepInExPack] OnHook R2API.DamageAPI.CrocoDamageTypeControllerGetDamageType added by assembly: R2API.DamageType.dll for: RoR2.CrocoDamageTypeController.GetDamageType
[Debug  :RoR2BepInExPack] OnHook R2API.DamageAPI.ProjectileManagerInitializeProjectile added by assembly: R2API.DamageType.dll for: RoR2.Projectile.ProjectileManager.InitializeProjectile
[Debug  :RoR2BepInExPack] ILHook R2API.ColorsAPI.GetColorIL modifier by assembly: R2API.Colors.dll for: RoR2.ColorCatalog.GetColor
[Debug  :RoR2BepInExPack] ILHook R2API.ColorsAPI.GetColorIL modifier by assembly: R2API.Colors.dll for: RoR2.ColorCatalog.GetColorHexString
[Debug  :RoR2BepInExPack] ILHook R2API.ColorsAPI.GetColorIL modifier by assembly: R2API.Colors.dll for: RoR2.DamageColor.FindColor
[Debug  :RoR2BepInExPack] ILHook R2API.DeployableAPI.GetDeployableSameSlotLimitIL modifier by assembly: R2API.Deployable.dll for: RoR2.CharacterMaster.GetDeployableSameSlotLimit
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Modules.Skins.DoNothing added by assembly: RA2Mod.dll for: RoR2.SkinDef.Awake
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Modules.Skins.DoNothing removed by assembly: RA2Mod.dll for: RoR2.SkinDef.Awake
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Modules.Skins.DoNothing added by assembly: RA2Mod.dll for: RoR2.SkinDef.Awake
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Modules.Skins.DoNothing removed by assembly: RA2Mod.dll for: RoR2.SkinDef.Awake
[Debug  : Red Alert] matMastery has already been loaded. returning cached
[Debug  : Red Alert] matMastery has already been loaded. returning cached
[Debug  : Red Alert] matMastery has already been loaded. returning cached
[Debug  : Red Alert] matMastery has already been loaded. returning cached
[Debug  : Red Alert] matMastery has already been loaded. returning cached
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Modules.Skins.DoNothing added by assembly: RA2Mod.dll for: RoR2.SkinDef.Awake
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Modules.Skins.DoNothing removed by assembly: RA2Mod.dll for: RoR2.SkinDef.Awake
[Debug  : Red Alert] matNod_Body has already been loaded. returning cached
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Survivors.Tesla.TeslaTrooperSurvivor.BaseAI_OnBodyDamaged added by assembly: RA2Mod.dll for: RoR2.CharacterAI.BaseAI.OnBodyDamaged
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Survivors.Tesla.TeslaTrooperSurvivor.CharacterMaster_AddDeployable added by assembly: RA2Mod.dll for: RoR2.CharacterMaster.AddDeployable
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Hooks.RoR2.HealthComponent+TakeDamageHook.HealthComponent_TakeDamage added by assembly: RA2Mod.dll for: RoR2.HealthComponent.TakeDamage
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Survivors.Tesla.TeslaTrooperSurvivor.LightningOrb_Begin added by assembly: RA2Mod.dll for: RoR2.Orbs.LightningOrb.Begin
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Survivors.Tesla.TeslaTrooperSurvivor.SetStateOnHurt_OnTakeDamageServer added by assembly: RA2Mod.dll for: RoR2.SetStateOnHurt.OnTakeDamageServer
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Survivors.Tesla.TeslaTrooperSurvivor.JitterBones_RebuildBones added by assembly: RA2Mod.dll for: RoR2.JitterBones.RebuildBones
[Debug  : Red Alert] matDesolatorArmor has already been loaded. returning cached
[Debug  : Red Alert] matDesolatorArmorColor has already been loaded. returning cached
[Debug  : Red Alert] matDesolatorBody has already been loaded. returning cached
[Debug  : Red Alert] matDesolatorCannon has already been loaded. returning cached
[Debug  : Red Alert] matDesolatorCannon (Instance) is not unity standard shader. aborting material conversion
[Debug  : Red Alert] matDesolatorCannon (Instance) is not unity standard shader. aborting material conversion
[Info   : R2API.Dot] Custom Dot (Index: 11) that uses Buff : DesolatorIrradiated added
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Modules.Skins.DoNothing added by assembly: RA2Mod.dll for: RoR2.SkinDef.Awake
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Modules.Skins.DoNothing removed by assembly: RA2Mod.dll for: RoR2.SkinDef.Awake
[Debug  : Red Alert] matDesolatorArmor has already been loaded. returning cached
[Debug  : Red Alert] matDesolatorBody has already been loaded. returning cached
[Debug  : Red Alert] matDesolatorCannon has already been loaded. returning cached
[Debug  : Red Alert] matDesolatorArmorColor has already been loaded. returning cached
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Modules.Skins.DoNothing added by assembly: RA2Mod.dll for: RoR2.SkinDef.Awake
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Modules.Skins.DoNothing removed by assembly: RA2Mod.dll for: RoR2.SkinDef.Awake
[Debug  : Red Alert] matDesoMasteryArmor has already been loaded. returning cached
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Survivors.Desolator.DesolatorSurvivor.BuffCatalog_Init added by assembly: RA2Mod.dll for: RoR2.BuffCatalog.Init
[Debug  : Red Alert] matChronoArmor has already been loaded. returning cached
[Debug  : Red Alert] matChronoArmor has already been loaded. returning cached
[Debug  : Red Alert] matChronoEmission has already been loaded. returning cached
[Debug  : Red Alert] matChronoBackpack has already been loaded. returning cached
[Debug  : Red Alert] matChronoBackpack has already been loaded. returning cached
[Debug  : Red Alert] matChronoBody has already been loaded. returning cached
[Debug  : Red Alert] matChronoDecals is not unity standard shader. aborting material conversion
[Debug  : Red Alert] matChronoGun has already been loaded. returning cached
[Debug  : Red Alert] matChronoGun has already been loaded. returning cached
[Debug  : Red Alert] matChronoEmission has already been loaded. returning cached
[Debug  :RoR2BepInExPack] OnHook Slipstream.ExtraHealthbarSegment.HealthBar_Awake added by assembly: RA2Mod.dll for: RoR2.UI.HealthBar.Awake
[Debug  :RoR2BepInExPack] OnHook Slipstream.ExtraHealthbarSegment.HealthBar_CheckInventory added by assembly: RA2Mod.dll for: RoR2.UI.HealthBar.CheckInventory
[Debug  :RoR2BepInExPack] OnHook Slipstream.ExtraHealthbarSegment.HealthBar_UpdateBarInfos added by assembly: RA2Mod.dll for: RoR2.UI.HealthBar.UpdateBarInfos
[Debug  :RoR2BepInExPack] ILHook Slipstream.ExtraHealthbarSegment.HealthBar_ApplyBars modifier by assembly: RA2Mod.dll for: RoR2.UI.HealthBar.ApplyBars
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Modules.Skins.DoNothing added by assembly: RA2Mod.dll for: RoR2.SkinDef.Awake
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Modules.Skins.DoNothing removed by assembly: RA2Mod.dll for: RoR2.SkinDef.Awake
[Warning: Red Alert] voicelinecontext is True
[Debug  :RoR2BepInExPack] ILHook RA2Mod.Survivors.Chrono.ChronoSurvivor.HealthComponent_TakeDamageIL modifier by assembly: RA2Mod.dll for: RoR2.HealthComponent.TakeDamageProcess
[Debug  :RoR2BepInExPack] ILHook RA2Mod.Survivors.Chrono.ChronoSurvivor.GenericCharacterDeath_OnEnter1 modifier by assembly: RA2Mod.dll for: EntityStates.GenericCharacterDeath.OnEnter
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Survivors.Chrono.ChronoSurvivor.CharacterBody_OnBuffFinalStackLost added by assembly: RA2Mod.dll for: RoR2.CharacterBody.OnBuffFinalStackLost
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 03 - Default has already been loaded. returning cached
[Debug  : Red Alert] bled0045_Specular 1 has already been loaded. returning cached
[Debug  : Red Alert] bl2 has already been loaded. returning cached
[Debug  : Red Alert] bled0045_Specular has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 0fdsa - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 03 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 03 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 03 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 03 - Default has already been loaded. returning cached
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Modules.Skins.DoNothing added by assembly: RA2Mod.dll for: RoR2.SkinDef.Awake
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Modules.Skins.DoNothing removed by assembly: RA2Mod.dll for: RoR2.SkinDef.Awake
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Survivors.GI.GISurvivor.Detonate_Explode added by assembly: RA2Mod.dll for: EntityStates.Engi.Mine.Detonate.Explode
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 03 - Default has already been loaded. returning cached
[Debug  : Red Alert] bled0045_Specular 1 has already been loaded. returning cached
[Debug  : Red Alert] bl2 has already been loaded. returning cached
[Debug  : Red Alert] bled0045_Specular has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 0fdsa - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 03 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 03 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 03 - Default has already been loaded. returning cached
[Debug  : Red Alert] 07 - Default has already been loaded. returning cached
[Debug  : Red Alert] 03 - Default has already been loaded. returning cached
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Modules.Skins.DoNothing added by assembly: RA2Mod.dll for: RoR2.SkinDef.Awake
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Modules.Skins.DoNothing removed by assembly: RA2Mod.dll for: RoR2.SkinDef.Awake
[Debug  : Red Alert] matTowerBlack has already been loaded. returning cached
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Modules.Skins.DoNothing added by assembly: RA2Mod.dll for: RoR2.SkinDef.Awake
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Modules.Skins.DoNothing removed by assembly: RA2Mod.dll for: RoR2.SkinDef.Awake
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Modules.Skins.DoNothing added by assembly: RA2Mod.dll for: RoR2.SkinDef.Awake
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Modules.Skins.DoNothing removed by assembly: RA2Mod.dll for: RoR2.SkinDef.Awake
[Debug  : Red Alert] matMastery has already been loaded. returning cached
[Debug  : Red Alert] matMastery has already been loaded. returning cached
[Debug  : Red Alert] matMastery has already been loaded. returning cached
[Debug  : Red Alert] matMastery has already been loaded. returning cached
[Debug  : Red Alert] matMastery has already been loaded. returning cached
[Debug  : Red Alert] WHITE has already been loaded. returning cached
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Modules.Skins.DoNothing added by assembly: RA2Mod.dll for: RoR2.SkinDef.Awake
[Debug  :RoR2BepInExPack] OnHook RA2Mod.Modules.Skins.DoNothing removed by assembly: RA2Mod.dll for: RoR2.SkinDef.Awake
[Debug  : Red Alert] matTowerNod has already been loaded. returning cached
[Debug  : Red Alert] matTowerNod has already been loaded. returning cached
[Debug  :R2API.ContentManagement] Content added from com.RiskySleeps.ClassicItemsReturns:
CLASSICITEMSRETURNS_BODY_RepairDroneBody (GameObject)
CLASSICITEMSRETURNS_MASTER_RepairDroneMaster (GameObject)
AtlasCannonInteractable (GameObject)
AtlasCannonTarget (GameObject)
AtlasCannonTeleporterVisual (GameObject)
CIR_RoyalMedallinPickup (GameObject)
CLASSICITEMSRETURNS_BODY_RepairDroneBody (GameObject)
CLASSICITEMSRETURNS_MASTER_RepairDroneMaster (GameObject)
CLASSICITEMSRETURNS_ITEM_56LEAFCLOVER (ItemDef)
CLASSICITEMSRETURNS_ITEM_ARCANEBLADES (ItemDef)
CLASSICITEMSRETURNS_ITEM_ARMSRACE (ItemDef)
CLASSICITEMSRETURNS_ITEM_ARMSRACEDRONEITEM (ItemDef)
CLASSICITEMSRETURNS_ITEM_BITTERROOT (ItemDef)
CLASSICITEMSRETURNS_ITEM_DRONEREPAIRKITDRONEITEM (ItemDef)
CLASSICITEMSRETURNS_ITEM_ENERGYCELL (ItemDef)
CLASSICITEMSRETURNS_ITEM_FIRESHIELD (ItemDef)
CLASSICITEMSRETURNS_ITEM_GOLDENGUN (ItemDef)
CLASSICITEMSRETURNS_ITEM_HITLIST (ItemDef)
CLASSICITEMSRETURNS_ITEM_HYPERTHREADER (ItemDef)
CLASSICITEMSRETURNS_ITEM_JEWEL (ItemDef)
CLASSICITEMSRETURNS_ITEM_LIFESAVINGS (ItemDef)
CLASSICITEMSRETURNS_ITEM_MUCONSTRUCT (ItemDef)
CLASSICITEMSRETURNS_ITEM_MYSTERIOUSVIAL (ItemDef)
CLASSICITEMSRETURNS_ITEM_PENNY (ItemDef)
CLASSICITEMSRETURNS_ITEM_PERMAFROST (ItemDef)
CLASSICITEMSRETURNS_ITEM_PRISONSHACKLES (ItemDef)
CLASSICITEMSRETURNS_ITEM_ROYALMEDALLION (ItemDef)
CLASSICITEMSRETURNS_ITEM_RUSTYJETPACK (ItemDef)
CLASSICITEMSRETURNS_ITEM_SMARTSHOPPER (ItemDef)
CLASSICITEMSRETURNS_ITEM_SNAKEEYES (ItemDef)
CLASSICITEMSRETURNS_ITEM_THALLIUM (ItemDef)
CLASSICITEMSRETURNS_ITEM_TOUGHTIMES (ItemDef)
CLASSICITEMSRETURNS_ITEM_USB (ItemDef)
CLASSICITEMSRETURNS_ITEM_WEAKENONCONTACT (ItemDef)
CLASSICITEMSRETURNS_ITEM_WICKEDRING (ItemDef)
CLASSICITEMSRETURNS_EQUIPMENT_CREATEGHOSTTARGETING (EquipmentDef)
CLASSICITEMSRETURNS_EQUIPMENT_DRONEREPAIRKIT (EquipmentDef)
CLASSICITEMSRETURNS_EQUIPMENT_DRUGS (EquipmentDef)
CLASSICITEMSRETURNS_EQUIPMENT_LOSTDOLL (EquipmentDef)
CLASSICITEMSRETURNS_EQUIPMENT_RESETSKILLCOOLDOWN (EquipmentDef)
CIR_BitterRoot (BuffDef)
CIR_Chilled (BuffDef)
CIR_DroneRepairBuff (BuffDef)
CIR_GoldenGun (BuffDef)
CIR_HitListEnemyMarker (BuffDef)
CIR_HitListPlayerBuff (BuffDef)
CIR_Prescriptions (BuffDef)
CIR_PrisonShackles (BuffDef)
CIR_RoyalMedallionBuff (BuffDef)
CIR_Snake1 (BuffDef)
CIR_Snake2 (BuffDef)
CIR_Snake3 (BuffDef)
CIR_Snake4 (BuffDef)
CIR_Snake5 (BuffDef)
CIR_Snake6 (BuffDef)
CIR_ThalliumBuff (BuffDef)
CIR_WeakenOnContact (BuffDef)
CLASSICITEMSRETURNS_ARTIFACT_CLOVER (ArtifactDef)
CIR_HyperThreaderOrbEffect (EffectDef)
CIR_RustyJetpackEffect (EffectDef)
HitListCheck (EffectDef)
ThalliumProcEffect (EffectDef)
 (NetworkSoundEventDef)
 (NetworkSoundEventDef)
 (NetworkSoundEventDef)
 (NetworkSoundEventDef)
 (NetworkSoundEventDef)
 (NetworkSoundEventDef)
 (NetworkSoundEventDef)
 (NetworkSoundEventDef)
 (NetworkSoundEventDef)
[Debug  :R2API.ContentManagement] Content added from iHarbHD.DebugToolkit:
DebugToolKitComponentsNetworked (GameObject)
[Warning:Silly Hitbox Viewer] No characters found to tone down hitboxes, make sure you've typed their names right in the config
[Message:SurvivorSortOrder] [Before Sort]
[Message:SurvivorSortOrder] CommandoBody sort position: 1
[Message:SurvivorSortOrder] HuntressBody sort position: 2
[Message:SurvivorSortOrder] Bandit2Body sort position: 3
[Message:SurvivorSortOrder] ToolbotBody sort position: 4
[Message:SurvivorSortOrder] EngiBody sort position: 5
[Message:SurvivorSortOrder] MageBody sort position: 6
[Message:SurvivorSortOrder] MercBody sort position: 7
[Message:SurvivorSortOrder] TreebotBody sort position: 8
[Message:SurvivorSortOrder] LoaderBody sort position: 9
[Message:SurvivorSortOrder] CrocoBody sort position: 10
[Message:SurvivorSortOrder] CaptainBody sort position: 11
[Message:SurvivorSortOrder] HereticBody sort position: 13
[Message:SurvivorSortOrder] RailgunnerBody sort position: 14
[Message:SurvivorSortOrder] VoidSurvivorBody sort position: 15
[Message:SurvivorSortOrder] SeekerBody sort position: 16
[Message:SurvivorSortOrder] FalseSonBody sort position: 17
[Message:SurvivorSortOrder] ChefBody sort position: 18
[Message:SurvivorSortOrder] TeslaTrooperBody sort position: 69.1
[Message:SurvivorSortOrder] DesolatorBody sort position: 69.2
[Message:SurvivorSortOrder] RA2ChronoBody sort position: 69.3
[Message:SurvivorSortOrder] RA2GIBody sort position: 69.4
[Message:SurvivorSortOrder] RA2MCVBody sort position: 69.5
[Message:SurvivorSortOrder] [After Sort]
[Message:SurvivorSortOrder] TeslaTrooperBody sort position: 0
[Message:SurvivorSortOrder] DesolatorBody sort position: 0.1
[Message:SurvivorSortOrder] RA2ChronoBody sort position: 0.2
[Message:SurvivorSortOrder] CommandoBody sort position: 1
[Message:SurvivorSortOrder] HuntressBody sort position: 2
[Message:SurvivorSortOrder] Bandit2Body sort position: 3
[Message:SurvivorSortOrder] ToolbotBody sort position: 4
[Message:SurvivorSortOrder] EngiBody sort position: 5
[Message:SurvivorSortOrder] MageBody sort position: 6
[Message:SurvivorSortOrder] MercBody sort position: 7
[Message:SurvivorSortOrder] TreebotBody sort position: 8
[Message:SurvivorSortOrder] LoaderBody sort position: 9
[Message:SurvivorSortOrder] CrocoBody sort position: 10
[Message:SurvivorSortOrder] CaptainBody sort position: 11
[Message:SurvivorSortOrder] HereticBody sort position: 13
[Message:SurvivorSortOrder] RailgunnerBody sort position: 14
[Message:SurvivorSortOrder] VoidSurvivorBody sort position: 15
[Message:SurvivorSortOrder] SeekerBody sort position: 25.016
[Message:SurvivorSortOrder] FalseSonBody sort position: 25.017
[Message:SurvivorSortOrder] ChefBody sort position: 25.018
[Message:SurvivorSortOrder] RA2GIBody sort position: 25.0694
[Message:SurvivorSortOrder] RA2MCVBody sort position: 25.0695
[Message:SurvivorSortOrder] Printed Sort Order:
TeslaTrooperBody:0, DesolatorBody:0.1, RA2ChronoBody:0.2, CommandoBody:1, HuntressBody:2, Bandit2Body:3, ToolbotBody:4, EngiBody:5, MageBody:6, MercBody:7, TreebotBody:8, LoaderBody:9, CrocoBody:10, CaptainBody:11, HereticBody:13, RailgunnerBody:14, VoidSurvivorBody:15, SeekerBody:25.016, FalseSonBody:25.017, ChefBody:25.018, RA2GIBody:25.0694, RA2MCVBody:25.0695
[Debug  :RoR2BepInExPack] Assembly.GetTypes() failed for RA2Mod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null (threw ReflectionTypeLoadException). System.Reflection.ReflectionTypeLoadException: Exception of type 'System.Reflection.ReflectionTypeLoadException' was thrown.
Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
Could not load type of field 'RA2Mod.Survivors.Chrono.VanishDriver:cachedWeaponDef' (0) due to: Could not load file or assembly 'DriverMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
  at (wrapper managed-to-native) System.Reflection.Assembly.GetTypes(System.Reflection.Assembly,bool)
  at (wrapper dynamic-method) System.Reflection.Assembly.DMD<System.Reflection.Assembly::GetTypes>(System.Reflection.Assembly)
  at (wrapper dynamic-method) MonoMod.Utils.DynamicMethodDefinition.Trampoline<System.Reflection.Assembly::GetTypes>?-624099166(System.Reflection.Assembly)
  at RoR2BepInExPack.ReflectionHooks.AutoCatchReflectionTypeLoadException.SaferGetTypes (System.Func`2[T,TResult] orig, System.Reflection.Assembly self) [0x00006] in <dcfd801117434c69ad117b0772e91f5f>:IL_0006 
System.TypeLoadException: Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
System.TypeLoadException: Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
System.TypeLoadException: Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
System.TypeLoadException: Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
System.TypeLoadException: Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
System.TypeLoadException: Could not load file or assembly 'Skills, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
System.TypeLoadException: Could not load type of field 'RA2Mod.Survivors.Chrono.VanishDriver:cachedWeaponDef' (0) due to: Could not load file or assembly 'DriverMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.
[Info   : Unity Log] ApplyEntityStateConfiguration(EntityStates.FalseSon.TheTamperedHeart (RoR2.EntityStateConfiguration)) failed: state type is null.
[Info   : Unity Log] Attempting to load user profile /UserProfiles/438f6934-15e3-4a18-8032-2200ba460c9b.xml
[Info   : Unity Log] stream.Length=448987
[Info   : Unity Log] Attempting to load user profile /UserProfiles/57edddd3-8c9d-49d1-8553-d2755907606e.xml
[Info   : Unity Log] stream.Length=274610
[Info   : Unity Log] Attempting to load user profile /UserProfiles/6866b93b-2206-419b-816b-ed76dc5e5b1f.xml
[Info   : Unity Log] stream.Length=255092
[Info   : Unity Log] Could not find body index for bodyName=AliemBody
[Info   : Unity Log] Attempting to load user profile /UserProfiles/6a42d473-f6b7-4b5d-9dc4-35be3458a6a6.xml
[Info   : Unity Log] stream.Length=236344
[Info   : Unity Log] Attempting to load user profile /UserProfiles/777cd811-3279-489e-8110-7b64412d2632.xml
[Info   : Unity Log] stream.Length=234352
[Info   : Unity Log] Could not find skill slot index for elementSkillFamilyName=EnforcerBodyPrimarySkillFamily elementSkillName=ENFORCER_PRIMARY_SHOTGUN_NAME
[Info   : Unity Log] Could not find skill slot index for elementSkillFamilyName=EnforcerBodySecondarySkillFamily elementSkillName=ENFORCER_SECONDARY_BASH_NAME
[Info   : Unity Log] Could not find skill slot index for elementSkillFamilyName=EnforcerBodyUtilitySkillFamily elementSkillName=ENFORCER_UTILITY_TEARGAS_NAME
[Info   : Unity Log] Could not find skill slot index for elementSkillFamilyName=EnforcerBodySpecialSkillFamily elementSkillName=ENFORCER_SPECIAL_SHIELDUP_NAME
[Info   : Unity Log] Attempting to load user profile /UserProfiles/d9cb65d7-f823-4c0a-9a3f-4eb115e8ce73.xml
[Info   : Unity Log] stream.Length=197283
[Info   : Unity Log] Attempting to load user profile /UserProfiles/f64c0a5f-44d6-4876-8646-4276c6eac263.xml
[Info   : Unity Log] stream.Length=234046
[Info   : Unity Log] Attempting to load user profile /UserProfiles/fd7ac372-0268-40b7-bd8f-bbaca422fe1d.xml
[Info   : Unity Log] stream.Length=299176
[Info   : Unity Log] loading RunReport /RunReports/History/925b4f00-d1c2-430e-aaa7-92ee8ec912ec.xml
[Info   : Unity Log] loading RunReport /RunReports/History/0b262c39-65f4-483c-80ef-56be8ee28639.xml
[Info   : Unity Log] loading RunReport /RunReports/History/c3f64499-3daf-4dd4-8fc4-d4a2b4911be0.xml
[Info   : Unity Log] loading RunReport /RunReports/History/a04d9e24-5522-4c47-b6a9-d7559322cbc4.xml
[Info   : Unity Log] loading RunReport /RunReports/History/db7bf4f1-c0a2-4181-84cc-c074bf9d520f.xml
[Info   : Unity Log] loading RunReport /RunReports/History/f22ffb36-ffac-40af-b627-9bd1f3042f9a.xml
[Info   : Unity Log] loading RunReport /RunReports/History/aa8a08ab-7d1a-4e89-b5e3-3b332b64dcf8.xml
[Info   : Unity Log] loading RunReport /RunReports/History/7b896c86-f3d5-421a-9334-0c8da128b9c8.xml
[Info   : Unity Log] loading RunReport /RunReports/History/0632a7aa-2ca7-4c24-b511-08c501e84446.xml
[Info   : Unity Log] loading RunReport /RunReports/History/0c4842ff-a1a7-4be9-be7f-53dfc219ee2e.xml
[Info   : Unity Log] loading RunReport /RunReports/History/0b26a227-a5b8-4275-918b-04e99b86d996.xml
[Info   : Unity Log] loading RunReport /RunReports/History/a7c779f0-7bb9-4d3b-9562-58b9502b5ca5.xml
[Info   : Unity Log] loading RunReport /RunReports/History/0f49ef82-dd44-4a56-9efe-8eb21d74b46d.xml
[Info   : Unity Log] loading RunReport /RunReports/History/d1dfaf82-3bc3-4f8a-ad15-a29b1f091feb.xml
[Info   : Unity Log] loading RunReport /RunReports/History/0fb9500c-0d49-4288-89d2-90c6803f845b.xml
[Info   : Unity Log] loading RunReport /RunReports/History/96d0847c-108c-4462-91a5-3d443c91e619.xml
[Info   : Unity Log] loading RunReport /RunReports/History/b146f98d-be02-4045-aaff-c32f1d7aa568.xml
[Info   : Unity Log] loading RunReport /RunReports/History/133d6001-5445-4ee0-8f8f-8a4713ca2a0c.xml
[Info   : Unity Log] loading RunReport /RunReports/History/7c2992ff-7767-49b2-bc70-000a7ece1975.xml
[Info   : Unity Log] loading RunReport /RunReports/History/05ef3bff-8de0-45e0-b9b8-d6a95ab5e8b0.xml
[Info   : Unity Log] loading RunReport /RunReports/History/b3ac8923-b940-4d90-911c-a470d121cf2d.xml
[Info   : Unity Log] loading RunReport /RunReports/History/17db0924-71f1-404c-9c45-d4fcd6c9be1f.xml
[Info   : Unity Log] loading RunReport /RunReports/History/5e0dfbbe-33de-4a71-a309-397af9f471f3.xml
[Info   : Unity Log] loading RunReport /RunReports/History/1eba6aef-6f78-4c3c-a8a1-6a6e85719dce.xml
[Info   : Unity Log] loading RunReport /RunReports/History/e6951d99-cbc8-434d-8c82-eff61d4dd260.xml
[Info   : Unity Log] loading RunReport /RunReports/History/e8010830-f29d-4aac-b5d2-1a1fac765076.xml
[Info   : Unity Log] loading RunReport /RunReports/History/82b53988-8778-495d-ba83-7a8caad97312.xml
[Info   : Unity Log] Setting offline scene to title
[Warning: Unity Log] EffectManager: Killing all pools
[Warning: Unity Log] EffectManager: Killing all pools
[Warning:     R2API] This version of R2API was built for build id "1.3.1", you are running "1.3.4".
[Warning:     R2API] Should any problems arise, please check for a new version before reporting issues.
[Warning: Unity Log] InputSourceFilter.Refresh: Null eventSystem on SettingsPanelTitle(Clone)
[Warning: Unity Log] InputSourceFilter.Refresh: Null eventSystem on FooterContainer
[Warning: Unity Log] InputSourceFilter.Refresh: Null eventSystem on FooterContainer
[Warning: Unity Log] InputSourceFilter.Refresh: Null eventSystem on TitleMenu
[Warning: Unity Log] InputSourceFilter.Refresh: Null eventSystem on TitleMenu
[Info   : Unity Log] BaseMainMenuScreen: OnEnter()
[Message:Classic Items Returns] Setting up language with 28 tokens.
[Debug  :RoR2BepInExPack] OnHook ClassicItemsReturns.Modules.LanguageOverrides.FinalizeLanguage removed by assembly: ClassicItemsReturns.dll for: RoR2.UI.MainMenu.MainMenuController.Start
[Info   : Unity Log] NetworkManagerSystem.desiredHost={ hostType=Self listen=False maxPlayers=4 }
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Error  :R2API.Prefab] mdlTeslaTrooper (UnityEngine.GameObject) don't have a NetworkIdentity Component but was marked for network registration.
[Error  :R2API.Prefab] mdlDesolator (UnityEngine.GameObject) don't have a NetworkIdentity Component but was marked for network registration.
[Error  :R2API.Prefab] mdlChrono (UnityEngine.GameObject) don't have a NetworkIdentity Component but was marked for network registration.
[Error  :R2API.Prefab] mdlJoe (UnityEngine.GameObject) don't have a NetworkIdentity Component but was marked for network registration.
[Error  :R2API.Prefab] mdlJoe (UnityEngine.GameObject) don't have a NetworkIdentity Component but was marked for network registration.
[Error  :R2API.Prefab] DesolatorLeapAcidGhost (UnityEngine.GameObject) don't have a NetworkIdentity Component but was marked for network registration.
[Error  :R2API.Prefab] mdlTeslaTower (UnityEngine.GameObject) don't have a NetworkIdentity Component but was marked for network registration.
[Warning: Unity Log] The prefab 'ArenaMissionController' has multiple NetworkIdentity components. There can only be one NetworkIdentity on a prefab, and it must be on the root object.
[Warning: Unity Log] The prefab 'Moon2DropshipZone' has multiple NetworkIdentity components. There can only be one NetworkIdentity on a prefab, and it must be on the root object.
[Warning: Unity Log] The prefab 'HOLDER_ Gauntlets' has multiple NetworkIdentity components. There can only be one NetworkIdentity on a prefab, and it must be on the root object.
[Warning: Unity Log] The prefab 'VoidCamp' has multiple NetworkIdentity components. There can only be one NetworkIdentity on a prefab, and it must be on the root object.
[Warning: Unity Log] The prefab 'ChefWok' has multiple NetworkIdentity components. There can only be one NetworkIdentity on a prefab, and it must be on the root object.
[Warning: Unity Log] The prefab 'MeridianEventTriggerCore' has multiple NetworkIdentity components. There can only be one NetworkIdentity on a prefab, and it must be on the root object.
[Warning: Unity Log] The prefab 'ShrineHalcyonite' has multiple NetworkIdentity components. There can only be one NetworkIdentity on a prefab, and it must be on the root object.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Warning: Unity Log] EffectManager: Killing all pools
[Warning:     R2API] This version of R2API was built for build id "1.3.1", you are running "1.3.4".
[Warning:     R2API] Should any problems arise, please check for a new version before reporting issues.
[Info   : Unity Log] "gamemode" = "ClassicRun"
Sets the specified game mode as the one to use in the next run.
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Error  : Unity Log] For iteration 0, out of 1
[Info   : Unity Log] Adding local player controller 0 on connection hostId: -1 connectionId: 0 isReady: True channel count: 0
[Info   : Unity Log] NetworkManagerSystem.AddPlayerInternal(conn=hostId: -1 connectionId: 0 isReady: True channel count: 0, playerControllerId=0, extraMessageReader=NetBuf sz:274 pos:274
[Info   : Unity Log] Could not load config /Config/server_pregame.cfg: Could not find file "C:\Program Files (x86)\Steam\steamapps\common\Risk of Rain 2\Risk of Rain 2_Data\Config\server_pregame.cfg"
[Info   : Unity Log] Attempting to generate PreGameVoteController for 
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Info   : Unity Log] Received vote from 
[Info   : Unity Log] Accepting vote from 
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] InputSourceFilter.Refresh: Null eventSystem on CharacterSelectUIMain(Clone)
[Warning: Unity Log] InputSourceFilter.Refresh: Null eventSystem on CharacterSelectUIMain(Clone)
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] Internal: JobTempAlloc has allocations that are more than 4 frames old - this is not allowed and likely a leak
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Warning: Unity Log] [DebuggingPlains] could not find Difficulty Deluge! ignoring
[Info   : Unity Log] Saved file "438f6934-15e3-4a18-8032-2200ba460c9b.xml" (448987 bytes)
[Debug  :RoR2BepInExPack] OnHook R2API.DifficultyAPI.GetExtendedDifficultyDef removed by assembly: R2API.Difficulty.dll for: RoR2.DifficultyCatalog.GetDifficultyDef
[Debug  :RoR2BepInExPack] OnHook R2API.DifficultyAPI.InitialiseRuleBookAndFinalizeList removed by assembly: R2API.Difficulty.dll for: RoR2.RuleDef.FromDifficulty
[Debug  :RoR2BepInExPack] OnHook R2API.DirectorAPI.ApplyChangesOnStart removed by assembly: R2API.Director.dll for: RoR2.ClassicStageInfo.Start
[Debug  :RoR2BepInExPack] OnHook R2API.DirectorAPI.InitStageEnumToSceneDefs removed by assembly: R2API.Director.dll for: RoR2.SceneCatalog.Init
[Debug  :RoR2BepInExPack] OnHook R2API.LanguageAPI.Language_GetLocalizedStringByToken removed by assembly: R2API.Language.dll for: RoR2.Language.GetLocalizedStringByToken
[Debug  :RoR2BepInExPack] OnHook R2API.LanguageAPI.Language_TokenIsRegistered removed by assembly: R2API.Language.dll for: RoR2.Language.TokenIsRegistered
[Debug  :RoR2BepInExPack] OnHook R2API.SkinIDRS.SetCustomIDRS removed by assembly: R2API.Skins.dll for: RoR2.ModelSkinController.ApplySkin
[Debug  :RoR2BepInExPack] OnHook R2API.SkinVFX.ApplyModifier removed by assembly: R2API.Skins.dll for: RoR2.EffectComponent.Start
[Debug  :RoR2BepInExPack] OnHook R2API.SkinVFX.ApplyReplacement removed by assembly: R2API.Skins.dll for: RoR2.EffectManager.SpawnEffect
[Debug  :RoR2BepInExPack] OnHook R2API.SkinVFX.ModifyGenericMelee removed by assembly: R2API.Skins.dll for: EntityStates.BasicMeleeAttack.BeginMeleeAttackEffect
[Debug  :RoR2BepInExPack] OnHook R2API.SkinLightReplacement.SetLightOverrides removed by assembly: R2API.Skins.dll for: RoR2.ModelSkinController.ApplySkin
[Debug  :RoR2BepInExPack] OnHook R2API.SoundAPI+Music.EnableCustomMusicSystems removed by assembly: R2API.Sound.dll for: RoR2.MusicController.StartIntroMusic
[Debug  :RoR2BepInExPack] OnHook R2API.SoundAPI+Music.IsGameMusicBankInUse removed by assembly: R2API.Sound.dll for: RoR2.MusicController.UpdateState
[Error  : Unity Log] NullReferenceException: Object reference not set to an instance of an object
Stack trace:
RoR2.UI.RuleChoiceController.FindNetworkUser () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)
RoR2.UI.RuleChoiceController.UpdateFromVotes () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)
RoR2.UI.RuleChoiceController+<>c.<.cctor>b__18_0 () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)
(wrapper dynamic-method) RoR2.PreGameRuleVoteController.DMD<RoR2.PreGameRuleVoteController::UpdateGameVotes>()
(wrapper dynamic-method) MonoMod.Utils.DynamicMethodDefinition.Trampoline<RoR2.PreGameRuleVoteController::UpdateGameVotes>?1184668712()
DebuggingPlains.DebuggingPlains.PreGameRuleVoteController_UpdateGameVotes (On.RoR2.PreGameRuleVoteController+orig_UpdateGameVotes orig) (at <6717db4003bf462a9529ba0fdf0e36a2>:0)
(wrapper dynamic-method) MonoMod.Utils.DynamicMethodDefinition.Hook<RoR2.PreGameRuleVoteController::UpdateGameVotes>?190922644()
RoR2.PreGameRuleVoteController+<>c.<.cctor>b__40_0 (RoR2.PreGameController controller) (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)
RoR2.PreGameController.RecalculateModifierAvailability () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)
(wrapper dynamic-method) RoR2.PreGameRuleVoteController.DMD<RoR2.PreGameRuleVoteController::UpdateGameVotes>()
(wrapper dynamic-method) MonoMod.Utils.DynamicMethodDefinition.Trampoline<RoR2.PreGameRuleVoteController::UpdateGameVotes>?1184668712()
DebuggingPlains.DebuggingPlains.PreGameRuleVoteController_UpdateGameVotes (On.RoR2.PreGameRuleVoteController+orig_UpdateGameVotes orig) (at <6717db4003bf462a9529ba0fdf0e36a2>:0)
(wrapper dynamic-method) MonoMod.Utils.DynamicMethodDefinition.Hook<RoR2.PreGameRuleVoteController::UpdateGameVotes>?190922644()
RoR2.PreGameRuleVoteController+<>c.<.cctor>b__40_0 (RoR2.PreGameController controller) (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)
RoR2.PreGameController.RecalculateModifierAvailability () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)
RoR2.PreGameController.OnNetworkUserLost (RoR2.NetworkUser networkUser) (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)
RoR2.NetworkUser.OnDisable () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)
UnityEngine.Object:DestroyImmediate(Object)
RoR2BepInExPack.RoR2BepInExPack:OnApplicationQuitting()
UnityEngine.Application:Internal_ApplicationQuit()

[Warning: Unity Log] Internal: deleting an allocation that is older than its permitted lifetime of 4 frames (age = 13)
[Warning: Unity Log] Internal: deleting an allocation that is older than its permitted lifetime of 4 frames (age = 13)
[Error  : Unity Log] NullReferenceException
Stack trace:
UnityEngine.Object.Instantiate (UnityEngine.Object original, UnityEngine.Transform parent, System.Boolean instantiateInWorldSpace) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
UnityEngine.Object.Instantiate[T] (T original, UnityEngine.Transform parent, System.Boolean worldPositionStays) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
UnityEngine.Object.Instantiate[T] (T original, UnityEngine.Transform parent) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
RoR2.UI.ViewableTag.Refresh () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)
RoR2.UI.ViewableTag.OnDisable () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)

[Error  : Unity Log] NullReferenceException
Stack trace:
UnityEngine.Object.Instantiate (UnityEngine.Object original, UnityEngine.Transform parent, System.Boolean instantiateInWorldSpace) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
UnityEngine.Object.Instantiate[T] (T original, UnityEngine.Transform parent, System.Boolean worldPositionStays) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
UnityEngine.Object.Instantiate[T] (T original, UnityEngine.Transform parent) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
RoR2.UI.ViewableTag.Refresh () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)
RoR2.UI.ViewableTag.OnDisable () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)

[Error  : Unity Log] NullReferenceException
Stack trace:
UnityEngine.Object.Instantiate (UnityEngine.Object original, UnityEngine.Transform parent, System.Boolean instantiateInWorldSpace) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
UnityEngine.Object.Instantiate[T] (T original, UnityEngine.Transform parent, System.Boolean worldPositionStays) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
UnityEngine.Object.Instantiate[T] (T original, UnityEngine.Transform parent) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
RoR2.UI.ViewableTag.Refresh () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)
RoR2.UI.ViewableTag.OnDisable () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)

[Error  : Unity Log] NullReferenceException
Stack trace:
UnityEngine.Object.Instantiate (UnityEngine.Object original, UnityEngine.Transform parent, System.Boolean instantiateInWorldSpace) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
UnityEngine.Object.Instantiate[T] (T original, UnityEngine.Transform parent, System.Boolean worldPositionStays) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
UnityEngine.Object.Instantiate[T] (T original, UnityEngine.Transform parent) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
RoR2.UI.ViewableTag.Refresh () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)
RoR2.UI.ViewableTag.OnDisable () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)

[Error  : Unity Log] NullReferenceException
Stack trace:
UnityEngine.Object.Instantiate (UnityEngine.Object original, UnityEngine.Transform parent, System.Boolean instantiateInWorldSpace) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
UnityEngine.Object.Instantiate[T] (T original, UnityEngine.Transform parent, System.Boolean worldPositionStays) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
UnityEngine.Object.Instantiate[T] (T original, UnityEngine.Transform parent) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
RoR2.UI.ViewableTag.Refresh () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)
RoR2.UI.ViewableTag.OnDisable () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)

[Error  : Unity Log] NullReferenceException
Stack trace:
UnityEngine.Object.Instantiate (UnityEngine.Object original, UnityEngine.Transform parent, System.Boolean instantiateInWorldSpace) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
UnityEngine.Object.Instantiate[T] (T original, UnityEngine.Transform parent, System.Boolean worldPositionStays) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
UnityEngine.Object.Instantiate[T] (T original, UnityEngine.Transform parent) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
RoR2.UI.ViewableTag.Refresh () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)
RoR2.UI.ViewableTag.OnDisable () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)

[Error  : Unity Log] NullReferenceException
Stack trace:
UnityEngine.Object.Instantiate (UnityEngine.Object original, UnityEngine.Transform parent, System.Boolean instantiateInWorldSpace) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
UnityEngine.Object.Instantiate[T] (T original, UnityEngine.Transform parent, System.Boolean worldPositionStays) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
UnityEngine.Object.Instantiate[T] (T original, UnityEngine.Transform parent) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
RoR2.UI.ViewableTag.Refresh () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)
RoR2.UI.ViewableTag.OnDisable () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)

[Error  : Unity Log] NullReferenceException
Stack trace:
UnityEngine.Object.Instantiate (UnityEngine.Object original, UnityEngine.Transform parent, System.Boolean instantiateInWorldSpace) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
UnityEngine.Object.Instantiate[T] (T original, UnityEngine.Transform parent, System.Boolean worldPositionStays) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
UnityEngine.Object.Instantiate[T] (T original, UnityEngine.Transform parent) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
RoR2.UI.ViewableTag.Refresh () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)
RoR2.UI.ViewableTag.OnDisable () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)

[Error  : Unity Log] NullReferenceException
Stack trace:
UnityEngine.Object.Instantiate (UnityEngine.Object original, UnityEngine.Transform parent, System.Boolean instantiateInWorldSpace) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
UnityEngine.Object.Instantiate[T] (T original, UnityEngine.Transform parent, System.Boolean worldPositionStays) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
UnityEngine.Object.Instantiate[T] (T original, UnityEngine.Transform parent) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
RoR2.UI.ViewableTag.Refresh () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)
RoR2.UI.ViewableTag.OnDisable () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)

[Error  : Unity Log] NullReferenceException
Stack trace:
UnityEngine.Object.Instantiate (UnityEngine.Object original, UnityEngine.Transform parent, System.Boolean instantiateInWorldSpace) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
UnityEngine.Object.Instantiate[T] (T original, UnityEngine.Transform parent, System.Boolean worldPositionStays) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
UnityEngine.Object.Instantiate[T] (T original, UnityEngine.Transform parent) (at <a20b3695b7ce4017b7981f9d06962bd1>:0)
RoR2.UI.ViewableTag.Refresh () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)
RoR2.UI.ViewableTag.OnDisable () (at <a149dbd7438a4e73b97d6600b3a2bbd4>:0)

[Info   :FixPluginTypesSerialization] Restored original AssemblyNames list

    //Taken from https://github.com/ToastedOven/CustomEmotesAPI/blob/main/CustomEmotesAPI/CustomEmotesAPI/CustomEmotesAPI.cs
    public static bool GetKeyPressed(ConfigEntry<KeyboardShortcut> entry)
    {
        foreach (var item in entry.Value.Modifiers)
        {
            if (!Input.GetKey(item))
            {
                return false;
            }
        }
        return Input.GetKeyDown(entry.Value.MainKey);
    }


    public static void ConfigureBody(CharacterBody bodyComponent, string section, string bodyInfoTitle = "")
    {
        if (string.IsNullOrEmpty(bodyInfoTitle))
        {
            bodyInfoTitle = bodyComponent.name;
        }

        bodyComponent.baseMaxHealth = BindAndOptions(
                section,
                $"{bodyInfoTitle} Base Max Health",
                bodyComponent.baseMaxHealth,
                0,
                1000,
                "levelMaxHealth will be adjusted accordingly (baseMaxHealth * 0.3)",
                true).Value;
        bodyComponent.levelMaxHealth = Mathf.Round(bodyComponent.baseMaxHealth * 0.3f);

        bodyComponent.baseRegen = BindAndOptions(
                section,
                $"{bodyInfoTitle} Base Regen",
                bodyComponent.baseRegen,
                "levelRegen will be adjusted accordingly (baseRegen * 0.2)",
                true).Value;
        bodyComponent.levelRegen = bodyComponent.baseRegen * 0.2f;

        bodyComponent.baseArmor = BindAndOptions(
                section,
                $"{bodyInfoTitle} Armor",
                bodyComponent.baseArmor,
                "",
                true).Value;

        bodyComponent.baseDamage = BindAndOptions(
                section,
                $"{bodyInfoTitle} Base Damage",
                bodyComponent.baseDamage,
                "pretty much all survivors are 12. If you want to change damage, change damage of the moves instead.\nlevelDamage will be adjusted accordingly (baseDamage * 0.2)",
                true).Value;
        bodyComponent.levelDamage = bodyComponent.baseDamage * 0.2f;

        bodyComponent.baseJumpCount = BindAndOptions(
                section,
                $"{bodyInfoTitle} Jump Count",
                bodyComponent.baseJumpCount,
                "",
                true).Value;
    }

    public static void ConfigureSkillDef(SkillDef skillDef, string section, string skillTitle, bool cooldown = true, bool maxStock = true, bool rechargeStock = false)
    {
        if (cooldown)
        {
            skillDef.baseRechargeInterval = BindAndOptions(
                section,
                $"{skillTitle} cooldown",
                skillDef.baseRechargeInterval,
                0,
                20,
                "",
                true).Value;
        }
        if (maxStock)
        {
            skillDef.baseMaxStock = BindAndOptions(
                section,
                $"{skillTitle} stocks",
                skillDef.baseMaxStock,
                0,
                100,
                "",
                true).Value;
        }
        if (rechargeStock)
        {
            skillDef.rechargeStock = BindAndOptions(
                section,
                $"{skillTitle} recharge stocks",
                skillDef.baseMaxStock,
                0,
                100,
                "",
                true).Value;
        }
    }

    internal static Sprite LoadSpriteFromModFolder(string fileName, bool pointFilter = false)
    {
        string fullFilePath = Path.Combine(Path.GetDirectoryName(SillyDeltaTimePlugin.instance.Info.Location), fileName);

        Texture2D texture2D = new Texture2D(2, 2);
        byte[] data = File.ReadAllBytes(fullFilePath);
        texture2D.LoadImage(data);
        texture2D.filterMode = (pointFilter ? FilterMode.Point : FilterMode.Bilinear);
        texture2D.Apply();
        if (pointFilter)
        {
            texture2D.filterMode = FilterMode.Point;
            texture2D.Apply();
        }

        texture2D.name = fileName;
        texture2D.filterMode = FilterMode.Point;
        texture2D.Apply();
        Rect rect = new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height);
        Sprite sprite = Sprite.Create(texture2D, rect, new Vector2(0.5f, 0.5f), 16f);
        sprite.name = fileName;
        return sprite;
    }
}