using RoR2;
//using R2API.Utils;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using System.Reflection;
using System;

namespace DimmaBandit {
    public class DimmaBanditHatGrower : MonoBehaviour {

        public static bool AfterLoop;

        private float _size = 0;

        private Transform _hat;
        private HatRescale _hatRescale;

        private CharacterModel _model;

        private bool _bandit1;
        private bool _subscribed;

        public DimmaBanditHatGrower init(CharacterModel model, bool one) {

            _model = model;
            _bandit1 = one;

            _hat = Instantiate(one? DougDimmadomeOwnerOfTheDimmsdaleDimmadome.dimmadomePrefab : DougDimmadomeOwnerOfTheDimmsdaleDimmadome.dimmadomePrefab2, transform)
                .transform;
            _hatRescale = _hat.GetComponent<HatRescale>();

            SetRendererInfos();
            //SetHatRagdoll();

            subscribeToEvents(true);

            grow(0);

            return this;
        }

        void OnDestroy() {
            subscribeToEvents(false);
        }

        private void subscribeToEvents(bool subscribe) {

            if (subscribe == _subscribed)
                return;
            _subscribed = subscribe;

            if (subscribe) {

                DougDimmadomeOwnerOfTheDimmsdaleDimmadome.Instance.onRagdoll += onRagdoll;
                DougDimmadomeOwnerOfTheDimmsdaleDimmadome.Instance.onHatGrow += grow;
            } else {

                DougDimmadomeOwnerOfTheDimmsdaleDimmadome.Instance.onRagdoll -= onRagdoll;
                DougDimmadomeOwnerOfTheDimmsdaleDimmadome.Instance.onHatGrow -= grow;
            }
        }

        private void SetRendererInfos() {

            CharacterModel.RendererInfo[] modelInfos = _model.baseRendererInfos;
            CharacterModel.RendererInfo[] hatInfos = new CharacterModel.RendererInfo[_hatRescale.hatRenderers.Length];

            Material banditMaterial = modelInfos[_bandit1 ? 0 : 3].defaultMaterial;//Reflection.GetFieldValue<SkinnedMeshRenderer>(model, "mainSkinnedMeshRenderer").material;

            //adding them in reverse so the tall part of the hat is last rendererinfo
            for (int i = _hatRescale.hatRenderers.Length - 1; i >= 0; i--) {

                hatInfos[i] = new CharacterModel.RendererInfo {
                    defaultMaterial = banditMaterial,
                    renderer = _hatRescale.hatRenderers[i],
                    defaultShadowCastingMode = ShadowCastingMode.On,
                    ignoreOverlays = false
                };
            }
            _model.baseRendererInfos = modelInfos.Concat(hatInfos).ToArray();
        }

        //why this didn't work i got no clu
        private void SetHatRagdoll() {

            RagdollController ragdollController = _model.GetComponent<RagdollController>();

            System.Collections.Generic.List<Transform> bonse = ragdollController.bones.ToList();
            bonse.AddRange(_hatRescale.hatColliders);
            ragdollController.bones = bonse.ToArray();
        }

        private void grow(float growScale) {

            _size += Mathf.Max(1, Mathf.FloorToInt(growScale));
            float growAdjusted = _bandit1 ? 1 : 1.4f;
            _hat.localScale = new Vector3(1, 1, 1 + _size * DougDimmadomeOwnerOfTheDimmsdaleDimmadome.growAmount * growAdjusted);            

            _hatRescale.RescaleTop();
        }

        private void onRagdoll(GameObject bod) {

            if (bod == _model.gameObject) {
                for (int i = 0; i < _hatRescale.hatColliders.Length; i++) {
                    _hatRescale.hatColliders[i].GetComponent<Collider>().enabled = true;
                }
            }
        }
    }
}
