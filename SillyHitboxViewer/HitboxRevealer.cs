using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SillyHitboxViewer {

    public enum boxType {
        HIT,
        BLAST,
        BULLET,
        BULLET_THIN,
        BULLET_POINT,
        HURT,
    }

    public class HitboxRevealer : MonoBehaviour {

        public static bool cfg_MercSoften;
        public static float cfg_BoxAlpha = 0.22f;
        public static float cfg_BulletAlpha = 0.32f;

        public static float cfg_HurtAlpha = 0.22f;

        public static float cfg_BlastShowTime = 0.2f;
        public static float cfg_BulletShowTime = 0.5f;
        public static bool cfg_ColorfulBullets = false;
        public static bool cfg_UniformBullets = false;

        //incremented in plugin update
        public static float randomTimer;

        public static bool showingBoxes = true;
        public static bool showingHitBoxes = true;
        public static bool showingHurtBoxes = true;

        public static bool bulletModeEnabled = false;


        [SerializeField] 
        private Renderer rend;

        private MaterialPropertyBlock _matProperties;
        private Color _matColor;

        void Awake() {
            _matProperties = new MaterialPropertyBlock();

            rend.GetPropertyBlock(_matProperties);
            
            rend.enabled = false;
        }

        private float getBoxAlpha(boxType box) {

            switch (box) {
                default:
                case boxType.HIT:
                    return cfg_BoxAlpha;
                case boxType.BLAST:
                    return cfg_BoxAlpha;
                case boxType.BULLET:
                    return cfg_BulletAlpha;
                case boxType.BULLET_THIN:
                    return cfg_BulletAlpha * 1.5f;
                case boxType.BULLET_POINT:
                    return cfg_BulletAlpha * 3f;
                case boxType.HURT:
                    return cfg_HurtAlpha;
            }
        }

        #region hitbox

        public HitboxRevealer init(boxType box, Transform boxParentTransform, bool isMerc = false) {

            transform.parent = boxParentTransform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            float setAlpha = getBoxAlpha(box); 
            float setLum = 0.6f;
            float setHue = 0.00f;
            float setHueHue = 1.00f;

            if (isMerc && cfg_MercSoften) { 
                //low alpha, and colors in cool blue-ish range
                setAlpha = 0.12f;
                setLum = 0.4f;
                setHue = 0.35f;
                setHueHue = 0.90f;
            }
            if ((box == boxType.BULLET || box == boxType.BULLET_THIN || box == boxType.BULLET_POINT)) {

                if (!cfg_ColorfulBullets) {

                    setHue = randomTimer * 10000 - (int)(randomTimer * 10000);
                    setHueHue = setHue;

                    //Utils.LogReadout($"{setHue} | {Time.time}/{randomTimer}");
                }

                if(cfg_UniformBullets) {
                    setHue = 0.5f;
                    setHueHue = setHue;
                }
            }

            _matColor = Random.ColorHSV(setHue, setHueHue, 0.5f, 0.5f, setLum, setLum);  

            _matColor.a = setAlpha;
            //Utils.LogReadout($"init box. alpha: {setAlpha}");

            _matProperties.SetColor("_Color", _matColor);

            rend.SetPropertyBlock(_matProperties);

            return this;
        }

        public void show(bool active) {

            active &= showingBoxes && showingHitBoxes;
            //Color color = active ? onColor : offColor;
            if (rend) {
                rend.enabled = active;
            }
        }
        #endregion

        #region hurtbox
        public HitboxRevealer initHurtbox(Transform capsuleTransform, CapsuleCollider capsuleCollider) {
            init(boxType.HURT, capsuleTransform);

            transform.localPosition = capsuleCollider.center;
            transform.localScale = new Vector3(capsuleCollider.radius * 2, capsuleCollider.height / 2, capsuleCollider.radius * 2);
            switch (capsuleCollider.direction) {
                case 0:
                    transform.localEulerAngles = new Vector3(0, 0, 90);
                    break;
                case 1:
                    //stay
                    break;
                case 2:
                    transform.localEulerAngles = new Vector3(90, 0, 0);
                    break;
            }

            hurtboxShow(true);
            return this;
        }

        public HitboxRevealer initHurtbox(Transform sphereTransform, SphereCollider sphereCollider) {
            init(boxType.HURT, sphereTransform);

            transform.localPosition = sphereCollider.center;
            transform.localScale = Vector3.one * sphereCollider.radius * 2;

            hurtboxShow(true);
            return this;
        }

        public HitboxRevealer initHurtbox(Transform boxTransform, BoxCollider boxCollider) {
            init(boxType.HURT, boxTransform);

            transform.localPosition = boxCollider.center;
            transform.localScale = boxCollider.size;

            hurtboxShow(true);
            return this;
        }

        public void hurtboxShow(bool active) {

            active &= showingBoxes && showingHurtBoxes;
            //Color color = active ? onColor : offColor;
            if (rend) {
                rend.enabled = active;
            }
        }
        #endregion

        #region blast

        public HitboxRevealer initBlastBox(Vector3 blastPosition, float radius) {
            init(boxType.BLAST, null);

            transform.position = blastPosition;
            transform.localScale = Vector3.one * radius * 2;

            blastboxShow(true, cfg_BlastShowTime);
            return this;
        }

        public void blastboxShow(bool active, float showTime) {

            active &= showingBoxes && showingHitBoxes;
            //Color color = active ? onColor : offColor;
            if (rend) {
                rend.enabled = active;
            }
            StartCoroutine(timedRemoveBlast(showTime));
        } 

        private IEnumerator timedRemoveBlast(float killTime) {

            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            _matColor *= 0.69f;
            _matColor.a *= 1.449f;

            _matProperties.SetColor("_Color", _matColor);
            rend.SetPropertyBlock(_matProperties);

            yield return new WaitForSeconds(killTime);

            if (gameObject) {
                //show(false);
                //HitboxViewerMod.instance.returnPooledBlastRevealer(this);
                Destroy(gameObject);
            }
        }
        #endregion

        #region bullet

        //bullet cyinder
        public HitboxRevealer initBulletBox(Vector3 origin, Vector3 normal, float distance, float radius) {

            init(radius == 0 ? boxType.BULLET_THIN : boxType.BULLET, null);
            radius = Mathf.Max(radius, 0.05f);

            transform.position = origin;

            transform.LookAt(transform.TransformPoint(normal), Vector3.up);
            // transform.LookAt(origin + normal, Vector3.up); slightly less retarded version of doing exactly the same as above
            // transform.rotation = Quaternion.LookRotation(normal); is the not retarded thing to do
            // i'm picking the retarded version anyway cause it's special to me

            transform.localScale = new Vector3(radius * 2f, radius * 2f, distance);

            blastboxShow(true, cfg_BulletShowTime);
            return this;
        }

        //bullet hit point
        public HitboxRevealer initBulletPoint(Vector3 position, float radius) {
            init(boxType.BULLET_POINT, null);

            transform.position = position;
            transform.localScale = Vector3.one * radius * 2;

            bulletPointBoxShow(true, bulletModeEnabled ? 100 : cfg_BulletShowTime * 2.5f, cfg_BulletShowTime * 1.5f);

            return this;
        }

        public void bulletPointBoxShow(bool active, float showTime, float shrinkTime) {

            active &= showingBoxes && showingHitBoxes;
            //Color color = active ? onColor : offColor;
            if (rend) {
                rend.enabled = active;
            }
            StartCoroutine(timedRemoveBulletPoint(showTime, shrinkTime));
        }

        private IEnumerator timedRemoveBulletPoint(float killTime, float shrinkTime) {

            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            _matColor *= 0.69f;
            _matColor.a *= 1.449f;

            _matProperties.SetColor("_Color", _matColor);
            rend.SetPropertyBlock(_matProperties);

            yield return new WaitForSeconds(shrinkTime);

            float shrink = transform.localScale.x * 0.4f;

            transform.localScale = Vector3.one * Mathf.Max(shrink, 0.1f);

            if(killTime < shrinkTime) 
                Utils.LogError("can't wait for a negative time retard");

            yield return new WaitForSeconds(killTime - shrinkTime);

            if (gameObject) {
                
                Destroy(gameObject);
            }
        }

        public void kill() {
            if (gameObject) {
                this.StopAllCoroutines();
                Destroy(gameObject);
            }
        }

        #endregion
    }
}