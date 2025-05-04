using UnityEngine;


namespace BetterHudLite {
    public class HealthHudHandler : BaseHudHandler {

        protected override void MoveHud() {

            RectTransform barRoots = (RectTransform)_hud.mainUIPanel.transform.Find("SpringCanvas/BottomLeftCluster/BarRoots");

            Transform buffs = barRoots.Find("LevelDisplayCluster/BuffDisplayRoot");
            buffs.SetParent(barRoots.parent);

            barRoots.SetParent(_hud.mainUIPanel.transform.Find("SpringCanvas/BottomCenterCluster"));
            
            barRoots.rotation = Quaternion.identity;
            barRoots.pivot = new Vector2(0.5f, 0.25f);
            barRoots.anchoredPosition = new Vector2(Confug.healthBarX.Value, Confug.healthBarY.Value);

            barRoots.sizeDelta = new Vector2(-432f + -432 * (-(Confug.healthBarWidth.Value - 1)), Confug.healthBarHeight.Value * 72f);

            //Transform lev = barRoots.Find("LevelDisplayCluster");
            //lev.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
        }
    }
}