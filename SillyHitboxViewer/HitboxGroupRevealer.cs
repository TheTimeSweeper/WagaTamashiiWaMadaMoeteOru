using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace SillyHitboxViewer {
    public class HitboxGroupRevealer: MonoBehaviour {

        public HitBoxGroup hitboxGroup { get; set; }
        private List<HitboxGroupRevealer> _hitboxesList;
        private HitboxRevealer _hitboxPrefab;

        private List<HitboxRevealer> _revealers = new List<HitboxRevealer>();

        private int revealCount;

        void FixedUpdate() {
            if (revealCount == 0)
                reveal(false);
            revealCount -= 1;
        }

        public void init(HitBoxGroup hitboxGroup_, List<HitboxGroupRevealer> hitboxesList, HitboxRevealer hitboxPrefab) {
            hitboxGroup = hitboxGroup_;
            _hitboxesList = hitboxesList;
            _hitboxPrefab = hitboxPrefab;

            addVisaulizinators();
        }

        private void addVisaulizinators() {
            for (int i = 0; i < hitboxGroup.hitBoxes.Length; i++) {
                _revealers.Add (Instantiate(_hitboxPrefab, hitboxGroup.hitBoxes[i].transform, false));
            }
        }

        public void reveal(bool active) {
            revealCount = 3;
            for (int i = 0; i < _revealers.Count; i++) {
                _revealers[i].show(active);
            }
        }

        void OnDestroy() {
            reveal(false);
            _hitboxesList.Remove(this);
        }
    }
}
