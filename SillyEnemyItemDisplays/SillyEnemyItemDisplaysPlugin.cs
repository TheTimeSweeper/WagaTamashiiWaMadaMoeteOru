using BepInEx;
using BepInEx.Configuration;
using System;
using System.Linq;
using SillyEnemyItemDisplays.Monsters;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace SillyEnemyItemDisplays {
    [BepInPlugin("com.TheTimeSweeper.SillyEnemyItemDisplays", "SillyEnemyItemDisplays", "0.1.0")]
    public class SillyEnemyItemDisplaysPlugin : BaseUnityPlugin {

        void Awake() {
            Log.Init(base.Logger);
            ItemDisplays.PopulateDisplays();

            On.RoR2.ItemCatalog.Init += ItemCatalog_Init;
;
        }

        private void ItemCatalog_Init(On.RoR2.ItemCatalog.orig_Init orig) {
            orig();

            new Beetle().Init();
            new BeetleQueen().Init();
            new ClayTemplar().Init();
            new GreaterWisp().Init();
            new GroveTender().Init();
            new Imp().Init();
            new ImpOverlord().Init();
            new JellyFish().Init();
            new Wisp().Init();
            new StoneTitan().Init();
            new Aurelionite().Init();
            new Gup().Init();
            new Geep().Init();
            new Gip().Init();
        }
    }
}

/* for custom copy format in keb's helper
{childName},
                                                                       {localPos}, 
                                                                       {localAngles},
                                                                       {localScale})
*/