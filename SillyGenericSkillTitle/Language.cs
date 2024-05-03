using System;
using System.Collections.Generic;
using System.IO;

namespace LoadoutSkillTitles.Modules
{
    internal static class Language
    {
        public static string TokensOutput = "";

        public static bool printingEnabled = false;

        public static void Add(string token, string text)
        {            
            if (!printingEnabled) return;

            //add a token formatted to language file
            TokensOutput += $"\n        \"{token}\" : \"{text.Replace(Environment.NewLine, "\\n").Replace("\n", "\\n")}\",";
        }

        public static void PrintOutput(string fileName = "")
        {
            if (!printingEnabled) return;

            //wrap all tokens in a properly formatted language file

            string strings =
$@"{{
    ""strings"": {{{TokensOutput}
    }},
    ""en"": {{{TokensOutput}
    }}
}}"; 
            //spit out language dump in console for copy paste if you want
            UnityEngine.Debug.LogWarning($"{fileName}: \n{strings}");

            //write a language file next to your mod. must have a folder called Language next to your mod dll with a folder called en under it.
            if (!string.IsNullOrEmpty(fileName))
            {
                string path = Path.Combine(Directory.GetParent(LoadoutSkillTitlesPlugin.instance.Info.Location).FullName, fileName);
                File.WriteAllText(path, strings);
            }

            //empty the output each time this is printed, so you can print multiple language files
            TokensOutput = "";
        }
    }
}