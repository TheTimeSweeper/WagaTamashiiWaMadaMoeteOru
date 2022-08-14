using System.Collections;
using UnityEngine;


namespace BetterHudLite {
    public class BaseHudHandler : MonoBehaviour {

        protected RoR2.UI.HUD _hud;

        public void Init(RoR2.UI.HUD hud) {
            _hud = hud;

            StartCoroutine(hackyDelayedMoveHud());
        }

        private IEnumerator hackyDelayedMoveHud() {
            yield return null;
            MoveHud();
        }

        void Update() {
            //if (Input.GetKeyDown(KeyCode.G)) {
            //    MoveHud();
            //}
        }

        protected virtual void MoveHud() { }
    }
}