using UnityEngine;
using UnityEngine.UI;

namespace SillyHitboxViewer {

    public class HitboxRevealer : MonoBehaviour {

        public static float cfg_BoxAlpha = 0.22f;
        public static float cfg_HurtAlpha = 0.22f;
        public static bool cfg_mercSoften;
        public static bool showingBoxes = true;
        public static bool showingHitBoxes = true;
        public static bool showingHurtBoxes = true;

        [SerializeField] 
        private Renderer rend;

        private MaterialPropertyBlock _matProperties;

        void Awake() {
            _matProperties = new MaterialPropertyBlock();

            rend.GetPropertyBlock(_matProperties);
            
            rend.enabled = false;
        }

        public HitboxRevealer init(Transform boxTransform, bool isMerc, bool isHurtbox = false) {

            transform.parent = boxTransform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            float setAlpha = isHurtbox ? cfg_HurtAlpha : cfg_BoxAlpha; 
            float setLum = 0.6f;
            float setHue = 0.00f;
            float setHueHue = 1.00f;

            if (isMerc && cfg_mercSoften) { 
                //low alpha, and colors in cool blue-ish range
                setAlpha = 0.12f;
                setLum = 0.4f;
                setHue = 0.35f;
                setHueHue = 0.90f;
            }

            Color clr = Random.ColorHSV(setHue, setHueHue, 0.5f, 0.5f, setLum, setLum);  

            clr.a = setAlpha;
            Utils.Log($"init box alpha {setAlpha}");

            _matProperties.SetColor("_Color", clr);

            rend.SetPropertyBlock(_matProperties);

            return this;
        }

        public void show(bool active) {

            active &= showingHitBoxes &= showingBoxes;
            //Color color = active ? onColor : offColor;
            if (rend) {
                rend.enabled = active;
            }
        }

        public HitboxRevealer initHurtbox(Transform capsuleTransform, CapsuleCollider capsuleCollider) {
            init(capsuleTransform, false, true);

            transform.localPosition = capsuleCollider.center;
            transform.localScale = new Vector3(capsuleCollider.radius * 2, capsuleCollider.height / 2, capsuleCollider.radius * 2);
            switch (capsuleCollider.direction) {
                case 0:
                    transform.localEulerAngles = new Vector3(0, 0, 90);
                    break;
                case 1:
                    break;
                case 2:
                    transform.localEulerAngles = new Vector3(90, 0, 0);
                    break;
            }

            showHurtboxes(true);
            return this;
        }

        public HitboxRevealer initHurtbox(Transform sphereTransform, SphereCollider sphereCollider) {
            init(sphereTransform, false, true);

            transform.localPosition = sphereCollider.center;
            transform.localScale = Vector3.one * sphereCollider.radius * 2;

            showHurtboxes(true);
            return this;
        }

        public HitboxRevealer initHurtbox(Transform boxTransform, BoxCollider boxCollider) {
            init(boxTransform, false, true);

            transform.localPosition = boxCollider.center;
            transform.localScale = boxCollider.size;

            showHurtboxes(true);
            return this;
        }

        public void showHurtboxes(bool active) {

            active &= showingHurtBoxes &= showingBoxes;
            //Color color = active ? onColor : offColor;
            if (rend) {
                rend.enabled = active;
            }
        }
    }
}
