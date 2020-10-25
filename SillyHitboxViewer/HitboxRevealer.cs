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
        private float setAlpha;

        void Awake() {

            _matProperties = new MaterialPropertyBlock();

            rend.GetPropertyBlock(_matProperties);

            rend.enabled = false;

        }

        public HitboxRevealer init(Transform transform, bool isMerc) {

            transform.parent = transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            setAlpha = cfg_BoxAlpha;
            if (isMerc && MercSoften) {
                setAlpha = 0.1f;
            }

            _clr = Random.ColorHSV(0.00f, 1.00f, 0.5f, 0.5f, 0.7f, 0.7f);

            _clr.a = setAlpha;

            _matProperties.SetColor("_Color", _clr);

            rend.SetPropertyBlock(_matProperties);

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
