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
    [BepInPlugin("AAA.TheTimeSweeper.LanguageFileGenerator", "LanguageFileGenerator", "1.0.2")]
    public class LanguageFileGeneratorPlugin : BaseUnityPlugin
    {
        private Hook languageHook;
        public static LanguageFileGeneratorPlugin instance;
        public static ManualLogSource Log;

        public static Dictionary<Assembly, string> assemblyModNames = new Dictionary<Assembly, string>();
        public const string nullMod = "UNKNOWN";

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

        private static Assembly GetAssembly()
        {
            Assembly asm = new System.Diagnostics.StackFrame(3, false).GetMethod()?.DeclaringType.Assembly;

            if (asm != null && asm.FullName.Contains("R2API.Language"))
            {
                asm = new System.Diagnostics.StackFrame(4, false).GetMethod()?.DeclaringType.Assembly;
            }

            return asm;
        }



        //credit to R2API.ContentManagement
        private static string GetModName(Assembly assembly)
        {
            if(assembly == null)
            {
                return nullMod;
            }

            //If the assembly that's adding the item has not been cached, find the GUID of the assembly and cache it.
            if (!assemblyModNames.TryGetValue(assembly, out string modName))
            {
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
