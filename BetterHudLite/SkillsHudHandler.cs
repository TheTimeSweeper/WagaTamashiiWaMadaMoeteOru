using RoR2;
using UnityEngine;
using UnityEngine.UI;


namespace BetterHudLite
{
    public class SkillsHudHandler : BaseHudHandler {

        protected override void MoveHud() {
            RectTransform bottomRightCluster = (RectTransform)_hud.mainUIPanel.transform.Find("SpringCanvas/BottomRightCluster");
            RectTransform bottomCenterCluster = (RectTransform)_hud.mainUIPanel.transform.Find("SpringCanvas/BottomCenterCluster");

            #region skills
            //skills
            RectTransform skillsScaler = (RectTransform)_hud.mainUIPanel.transform.Find("SpringCanvas/BottomRightCluster/Scaler");

            //separate sprint and tab screen icons, keeping them in their corner
            RectTransform sprint = (RectTransform)skillsScaler.Find("SprintCluster");
            sprint.SetParent(skillsScaler.parent);
            sprint.anchoredPosition = new Vector2(-100, 0);
            sprint.anchorMin = new Vector2(1, 0.5f);
            sprint.anchorMax = new Vector2(1, 0.5f);

            RectTransform inve = (RectTransform)skillsScaler.Find("InventoryCluster");
            inve.SetParent(skillsScaler.parent);
            inve.anchoredPosition = new Vector2(-146, 0);
            inve.anchorMin = new Vector2(1, 0.5f);
            inve.anchorMax = new Vector2(1, 0.5f);

            //move skills over
            skillsScaler.transform.SetParent(_hud.mainUIPanel.transform.Find("SpringCanvas/BottomCenterCluster"));// barRoots);
                                                                                                                  //skillsScaler.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
            skillsScaler.rotation = Quaternion.identity;
            skillsScaler.pivot = new Vector2(0.5f, 0.0f);

            skillsScaler.sizeDelta = new Vector2(-639, -234);
            float barsHeight = Confug.healthBarHeight - 1;
            float skillHeightFactor = 72 * 0.25f;
            skillsScaler.anchoredPosition = new Vector2(60, 80 + barsHeight * skillHeightFactor);
            //presto! I bet you've never seen so much magic in your life!

            //move up the skill slot fields to give the section room to move lower
            Transform rect;
            Image im;
            foreach (InputBindingDisplayController bindingDisplay in skillsScaler.GetComponentsInChildren<InputBindingDisplayController>()) {

                if (Confug.HideSkillKeys)
                {
                    bindingDisplay.transform.parent.gameObject.SetActive(false);
                    continue;
                }

                rect = bindingDisplay.transform.parent;
                Vector3 anch = rect.localPosition;
                anch.y += 9;
                rect.localPosition = anch;

                im = rect.GetComponent<Image>();
                Color col = im.color;
                col.a = 0.76f;
                im.color = col;
            }

            //move UtilityArea (where seeker has her lotus thingy display)
            skillsScaler.Find("UtilityArea").transform.localPosition = new Vector3(-363, 0, 0);

            //Fixing tooltips
            #region component migration

            GraphicRaycaster graphicRaycasterOld = bottomRightCluster.GetComponent<GraphicRaycaster>();
            GraphicRaycaster graphicRaycasterNew = bottomCenterCluster.gameObject.AddComponent<GraphicRaycaster>();
            graphicRaycasterNew.blockingObjects = graphicRaycasterOld.blockingObjects;
            graphicRaycasterNew.ignoreReversedGraphics = graphicRaycasterOld.ignoreReversedGraphics;
            graphicRaycasterNew.useGUILayout = graphicRaycasterOld.useGUILayout;
            #endregion

            #endregion skills

            //skills are now overlapping this so move it
            #region notif area (item blurbs)

            RectTransform notifArea = (RectTransform)_hud.mainUIPanel.transform.parent.Find("NotificationArea");
            notifArea.anchorMin = new Vector2(0.8f, 0.05f);
            notifArea.anchorMax = new Vector2(0.8f, 0.05f);
            #endregion

            #region spectator label

            RectTransform spec = (RectTransform)bottomCenterCluster.Find("SpectatorLabel");
            spec.anchoredPosition = new Vector2(0, 150);
            #endregion
        }
    }
}