using UnityEngine;

namespace DimmaBandit {
    public class HatRescale : MonoBehaviour {

        [SerializeField]
        private Transform _hatTop;

        [SerializeField]
        private Renderer[] _hatRenderers;
        public Renderer[] hatRenderers { get => _hatRenderers; }

        //script's now taking on more responsibility than hatrescale but fuck it I already named it that
        [SerializeField]
        private Transform[] _hatColliders;
        public Transform[] hatColliders { get => _hatColliders; }
        
        public void RescaleTop() {

            for (int i = 0; i < _hatRenderers.Length; i++) {
                _hatRenderers[i].enabled = true;
            }

            _hatTop.localScale = new Vector3(1, 1, 1 / transform.localScale.z);
        }
    }
}
