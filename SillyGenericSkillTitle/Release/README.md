# Misc
Fixes extra GenericSkill slots from "Misc" to more fitting names, primarily loadout passives;

## Config
- Default a non-primary skill in the first slot to say "Passive", otherwise just keep as "misc" (on by default)
- Fall back to internal name of the skill on the component and hope characters put something nice there, otherwise just keep as "misc" (off by default)

## Compat
If you just want the first skill to say "Passive", this mod will do that by default.

For your character, set your generic skill's skillName field to begin with `LOADOUT` and this mod will automatically read that as a language token.

If your character already exists and you don't want to mess with existing components (I wouldn't if I were you), you can manually add your character's bodyname and skill slot index to this mod as a soft dependency:
```csharp
if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TheTimeSweeper.LoadoutSkillTitles"))
{
    AddLoadoutSkillTitle();
}
```
```csharp
[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
public static void AddLoadoutSkillTitle()
{
    LoadoutSkillTitles.LoadoutSkillTitlesPlugin.AddTitleToken("MySurvivorBodyName", 0, "TITLE_LANGUAGE_TOKEN");
}
```
skill slot index being in order of where it appears on the loadout. for example if you have an extra skill in between secondary and utility, it would be 2

## changelog
`1.0.1`
 - fix incompatibility with dragon's dbz characters

`1.0.0`
 - c: