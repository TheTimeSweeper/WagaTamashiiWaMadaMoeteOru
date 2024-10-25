using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;


namespace BetterHudLite
{
    public class BoxNukerHudHandler : BaseHudHandler
    {
        public static Sprite frame;

        protected override void MoveHud()
        {
            Image[] allImagesIAmSoSorry = _hud.mainUIPanel.GetComponentsInChildren<Image>();
            for (int i = 0; i < allImagesIAmSoSorry.Length; i++)
            {
                if(allImagesIAmSoSorry[i].sprite == frame)
                {
                    allImagesIAmSoSorry[i].enabled = false;
                }
            }
        }
    }
}