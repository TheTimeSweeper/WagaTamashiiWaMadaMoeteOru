using UnityEngine;
using UnityEngine.UI;

namespace SillyHitboxViewer {
    public class HitboxRevealer : MonoBehaviour {

        public static float cfg_BoxAlpha = 0.23f;

        [SerializeField]
        private Renderer rend;

        private MaterialPropertyBlock matProperties;
        private Color clr;

        void Awake() {

            matProperties = new MaterialPropertyBlock();

            clr = Random.ColorHSV(0.00f, 1.00f, 1, 1, 0.7f, 0.7f);

            rend.GetPropertyBlock(matProperties);

            clr.a = cfg_BoxAlpha;

            matProperties.SetColor("_Color", clr);

            rend.SetPropertyBlock(matProperties);
        }

        public void show(bool active) {
            //Color color = active ? onColor : offColor;
            rend.enabled = active;
        }
    }
}
