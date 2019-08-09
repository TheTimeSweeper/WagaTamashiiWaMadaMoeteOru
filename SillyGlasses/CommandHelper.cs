using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using R2API.Utils;
using RoR2;

namespace SillyGlasses
{
    public class CommandHelper
    {
        /*
         * This code belongs to Wildbook.
         * https://github.com/wildbook/R2Mods/blob/develop/Utilities/CommandHelper.cs
         * Credit goes to Wildbook.  
         * 
         * Also this Credit belongs to Fluffy
         * https://github.com/JackPendarvesRead/RoR2FluffyMods/
         * Credit for credit goes to Fluffy
         */

        public static void RegisterCommands(RoR2.Console self)
        {
            var types = typeof(CommandHelper).Assembly.GetTypes();
            var catalog = self.GetFieldValue<IDictionary>("concommandCatalog");

            foreach (var methodInfo in types.SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)))
            {
                var customAttributes = methodInfo.GetCustomAttributes(false);
                foreach (var attribute in customAttributes.OfType<ConCommandAttribute>())
                {
                    var conCommand = Reflection.GetNestedType<RoR2.Console>("ConCommand").Instantiate();

                    conCommand.SetFieldValue("flags", attribute.flags);
                    conCommand.SetFieldValue("helpText", attribute.helpText);
                    conCommand.SetFieldValue("action", (RoR2.Console.ConCommandDelegate)Delegate.CreateDelegate(typeof(RoR2.Console.ConCommandDelegate), methodInfo));

                    catalog[attribute.commandName.ToLower()] = conCommand;
                }
            }
        }
    }
}