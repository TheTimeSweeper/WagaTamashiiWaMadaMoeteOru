using BepInEx;
using System;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace SillyMod2
{
    [BepInPlugin("com.TheTimeSweeper.SillyMod2", "SillyMod2", "0.0.1")]
                          //add plugin
    public class SillyMod2 : BaseUnityPlugin
    {
        void Awake()
        {

        }
    }
}
