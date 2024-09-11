using BepInEx;
using System;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace CursorFreedom2
{
    [BepInPlugin("com.TheTimeSweeper.CursorFreedom2", "CursorFreedom2", "0.0.1")]
    public class CursorFreedom2Plugin : BaseUnityPlugin
    {
        void Awake()
        {
            On.RoR2.RoR2Application.UpdateCursorState += RoR2Application_UpdateCursorState;

            On.RoR2.MPEventSystemManager.Update += MPEventSystemManager_Update;
        }

        private void MPEventSystemManager_Update(On.RoR2.MPEventSystemManager.orig_Update orig, RoR2.MPEventSystemManager self)
        {
            orig(self);

            Mutinize();
        }


        private void RoR2Application_UpdateCursorState(On.RoR2.RoR2Application.orig_UpdateCursorState orig)
        {
            orig();

            Mutinize();
        }

        private void Mutinize()
        {
            if(UnityEngine.Cursor.lockState == UnityEngine.CursorLockMode.Confined)
            {
                UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.None;
            }
        }
    }
}
