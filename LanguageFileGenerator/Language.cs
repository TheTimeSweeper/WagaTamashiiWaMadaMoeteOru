using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace LanguageFileGenerator.Modules
{
    internal static class Language
    {
        public static Dictionary<string, string> tokensOutputs = new Dictionary<string, string>();

        public static void Add(string token, string text, string modName)
        {          
            if (!tokensOutputs.ContainsKey(modName))
            {
                tokensOutputs[modName] = "";
            }

            //add a token formatted to language file
            tokensOutputs[modName] += $"\n        \"{token}\" : \"{text.Replace(Environment.NewLine, "\\n").Replace("\n", "\\n")}\",";
        }

        public static void PrintAll()
        {
            foreach (KeyValuePair<string, string> kvp in tokensOutputs)
            {
                PrintOutput(kvp.Value, kvp.Key);
            }
        }

        private static void PrintOutput(string tokensOutput, string fileName = "")
        {
            //wrap all tokens in a properly formatted language file

            string strings =
$@"{{
    ""en"": {{{tokensOutput}
    }}
}}";
            //spit out language dump in console for copy paste if you want
            //UnityEngine.Debug.LogWarning($"{fileName}: \n{strings}");

            //write a language file next to your mod.
            fileName += ".language";
            string path = Path.Combine(Directory.GetParent(LanguageFileGeneratorPlugin.instance.Info.Location).FullName, fileName);
            File.WriteAllText(path, strings);
            LanguageFileGeneratorPlugin.Log.LogMessage($"writing file to {path}");
        }
    }
}