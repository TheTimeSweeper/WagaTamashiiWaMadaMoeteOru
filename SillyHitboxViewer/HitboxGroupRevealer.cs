using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace SillyHitboxViewer {
    public class HitboxGroupRevealer: MonoBehaviour {

        public HitBoxGroup hitboxGroup { get; set; }

        private List<HitboxGroupRevealer> _modHitboxesList;
        private bool _isMerc;

        private HitboxRevealer[] _revealers;

        private int revealBufferCount;

        public void init(HitBoxGroup hitboxGroup_, List<HitboxGroupRevealer> hitboxesList) {
            hitboxGroup = hitboxGroup_;
            _modHitboxesList = hitboxesList;

            var bod = GetComponent<CharacterBody>();
            Utils.Log($"bod: {bod}");
            if (bod) {
                _isMerc = GetComponent<CharacterBody>().bodyIndex == BodyCatalog.FindBodyIndex("Mercenary");
            }

            addVisaulizinators();
        }

        private void addVisaulizinators() {

            HitboxRevealer rev;
            _revealers = new HitboxRevealer[hitboxGroup.hitBoxes.Length];
            for (int i = 0; i < hitboxGroup.hitBoxes.Length; i++) {

                rev = HitboxViewerMod.requestPooledRevealer();
                rev.init(hitboxGroup.hitBoxes[i].transform, _isMerc);
                _revealers[i] = rev;
            }
        }

        public void reveal(bool active) {
            revealBufferCount = 3;
            for (int i = 0; i < _revealers.Length; i++) {
                _revealers[i].show(active);
            }
        }

        void FixedUpdate() {
            if (revealBufferCount == 0)
                reveal(false);
            revealBufferCount -= 1;
        }

        void OnDestroy() {
            reveal(false);
            revealBufferCount = -1;

            _modHitboxesList.Remove(this);
            HitboxViewerMod.returnPooledRevealers(_revealers);
        }
    }
}
