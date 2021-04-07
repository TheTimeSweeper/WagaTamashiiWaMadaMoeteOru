using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SillyHitboxViewer {
    public class HitboxGroupRevealer: MonoBehaviour {

        public HitBoxGroup hitboxGroup { get; set; }
        private HitboxRevealer _hitboxPrefab;

        private HitboxRevealer[] _revealers;

        private bool _isMerc;

        private int _revealBufferCount;
        private bool _revealed;

        public void init(HitBoxGroup hitboxGroup_, HitboxRevealer hitboxPrefab_, GameObject attacker) {
            hitboxGroup = hitboxGroup_;
            _hitboxPrefab = hitboxPrefab_;

            if (attacker) {
                CharacterBody bod = attacker.GetComponent<CharacterBody>();
                if (bod) {
                    _isMerc = checkMerc((int)bod.bodyIndex);
                }
            }

            initVisaulizinators();
        }

        private bool checkMerc(int index) {

            return Utils.cfg_softenedCharacters.Contains(index);
        }

        private void initVisaulizinators() {

            HitboxRevealer rev;
            _revealers = new HitboxRevealer[hitboxGroup.hitBoxes.Length];
            for (int i = 0; i < hitboxGroup.hitBoxes.Length; i++) {

                //rev = HitboxViewerMod.instance.requestPooledHitboxRevealer();
                rev = Instantiate(_hitboxPrefab);
                rev.init(hitboxGroup.hitBoxes[i].transform, _isMerc);
                _revealers[i] = rev;
            }
        }

        public void reveal(bool active) {

            if (active) {
                _revealBufferCount = 3;
            }

            if (_revealed == active)
                return;

            _revealed = active;
            for (int i = 0; i < _revealers.Length; i++) {
                _revealers[i]?.show(active);
            }
        }

        void FixedUpdate() {
            if (_revealBufferCount == 0)
                reveal(false);
            _revealBufferCount -= 1;
        }

        void OnDestroy() {

            reveal(false);
            for (int i = 0; i < _revealers.Length; i++) {
                Destroy(_revealers[i].gameObject);

            }
            return; //rip pool

            reveal(false);
            _revealBufferCount = -1;

            HitboxViewerMod.instance.removeHitBoxGroupRevealer(this);
            HitboxViewerMod.instance.returnPooledHitboxRevealers(_revealers);
        }
    }
}
