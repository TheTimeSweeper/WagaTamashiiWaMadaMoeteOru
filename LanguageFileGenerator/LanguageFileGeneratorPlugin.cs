using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using MonoMod.RuntimeDetour;
using R2API.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using UnityEngine;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace LanguageFileGenerator
{
    [BepInDependency("com.bepis.r2api.language")]
    [BepInPlugin("AAA.TheTimeSweeper.LanguageFileGenerator", "LanguageFileGenerator", "0.0.1")]
    public class LanguageFileGeneratorPlugin : BaseUnityPlugin
    {
        private Hook languageHook;
        public static LanguageFileGeneratorPlugin instance;
        public static ManualLogSource Log;

        public static Dictionary<Assembly, string> assemblyModNames = new Dictionary<Assembly, string>();

        void Awake()
        {
            instance = this;
            Log = Logger;
            languageHook = new Hook(typeof(R2API.LanguageAPI).GetMethod("Add", new Type[] { typeof(string), typeof(string), typeof(string) }/* (System.Reflection.BindingFlags)(-1)*/), typeof(LanguageFileGeneratorPlugin).GetMethod("OnLanguageHook"));
            RoR2.RoR2Application.onLoad += RoR2Application_OnLoad;
        }
        private void RoR2Application_OnLoad()
        {
            Modules.Language.PrintAll();
        }

        public static void OnLanguageHook(Action<string, string, string> orig, string token, string text, string language)
        {
            Modules.Language.Add(token, text, GetModName(GetAssembly()));
            //instance.Logger.LogWarning(token + " | " + text);

            orig(token, text, language);
        }

        //credit to R2API.ContentManagement
        private static Assembly GetAssembly()
        {
            bool returnNext = false;
            for (int i = 0; i < 99; i++)
            {//Empty frame will stop loop early when the stack runs out.
                var asm = new System.Diagnostics.StackFrame(i, false).GetMethod()?.DeclaringType.Assembly;

                if (returnNext)
                {
                    //instance.Logger.LogMessage("found assembly: " + asm.FullName);
                    return asm;
                }

                if (asm.FullName.Contains("R2API.Language"))
                {
                    returnNext = true;
                }
            }
            return null;
        }



        //credit to R2API.ContentManagement
        private static string GetModName(Assembly assembly)
        {
            //If the assembly that's adding the item has not been cached, find the GUID of the assembly and cache it.
            if (!assemblyModNames.TryGetValue(assembly, out string modName))
            {
                if(assembly == null)
                {
                    assemblyModNames[assembly] = "UNKNOWN";
                    return assemblyModNames[assembly];
                }

                var location = assembly.Location;
                modName = Chainloader.PluginInfos.FirstOrDefault(x => location == x.Value.Location).Key;
                if (modName == null)
                {
                    instance.Logger.LogWarning($"The assembly {assembly.FullName} is not a loaded BepInEx plugin, falling back to looking for attribute in assembly");

                    try
                    {
                        _ = Reflection.GetTypesSafe(assembly, out var types);
                        var infoAttribute = types.Select(x => x.GetCustomAttribute<BepInPlugin>()).First(x => x != null);
                        modName = infoAttribute.GUID;
                    }
                    catch
                    {
                        instance.Logger.LogWarning("Assembly did not have a BepInPlugin attribute or couldn't load its types, falling back to assembly name");
                        modName = assembly.GetName().Name;
                    }
                }

                assemblyModNames[assembly] = modName;
            }

            return assemblyModNames[assembly];
        }
    }
}
