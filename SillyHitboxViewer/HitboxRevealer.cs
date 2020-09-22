using UnityEngine;
using UnityEngine.UI;

namespace SillyHitboxViewer {
    public class HitboxRevealer : MonoBehaviour {

        public static float cfg_BoxAlpha = 0.22f;
        public static bool MercSoften;

        [SerializeField]
        private Renderer rend;

        private MaterialPropertyBlock _matProperties;
        private Color _clr;
        private bool _isMerc;

        void Awake() {

            _matProperties = new MaterialPropertyBlock();

            rend.GetPropertyBlock(_matProperties);

            _clr = Random.ColorHSV(0.00f, 1.00f, 0.5f, 0.5f, 0.7f, 0.7f);
            
            _clr.a = _isMerc && MercSoften ? 0.1f : cfg_BoxAlpha;

            _matProperties.SetColor("_Color", _clr);

            rend.SetPropertyBlock(_matProperties);
        }

        public HitboxRevealer init(bool isMerc) {
            _isMerc = isMerc;
            return this;
        }

        public void show(bool active) {
            //Color color = active ? onColor : offColor;
            if (rend) {
                rend.enabled = active;
            }
        }
    }
}
