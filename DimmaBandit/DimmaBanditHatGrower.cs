using RoR2;
//using R2API.Utils;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using System.Reflection;

namespace DimmaBandit {
    public class DimmaBanditHatGrower : MonoBehaviour {

        public static bool AfterLoop;

        private float _size = 0;

        private Transform _hat;
        private HatRescale _hatRescale;

        private bool _bandit1;

        public DimmaBanditHatGrower init(CharacterModel model, bool one) {

            DougDimmadomeOwnerOfTheDimmsdaleDimmadome.Instance.onHatGrow += grow;

            _bandit1 = one;

            _hat = Instantiate(one? DougDimmadomeOwnerOfTheDimmsdaleDimmadome.dimmadomePrefab : DougDimmadomeOwnerOfTheDimmsdaleDimmadome.dimmadomePrefab2, transform).transform;
            _hatRescale = _hat.GetComponent<HatRescale>();

            SetRendererInfos(model);

            grow(0);

            return this;
        }

        private void SetRendererInfos(CharacterModel model) {

            CharacterModel.RendererInfo[] modelInfos = model.baseRendererInfos;
            CharacterModel.RendererInfo[] hatInfos = new CharacterModel.RendererInfo[2];

            Material banditMaterial = modelInfos[_bandit1 ? 0 : 3].defaultMaterial;//Reflection.GetFieldValue<SkinnedMeshRenderer>(model, "mainSkinnedMeshRenderer").material;

            for (int i = 0; i < _hatRescale.hatRenderers.Length; i++) {
                hatInfos[i] = new CharacterModel.RendererInfo {
                    defaultMaterial = banditMaterial,
                    renderer = _hatRescale.hatRenderers[i],
                    defaultShadowCastingMode = ShadowCastingMode.On,
                    ignoreOverlays = false
                };
            }
            model.baseRendererInfos = modelInfos.Concat(hatInfos).ToArray();
        }

        private void grow(float growScale) {

            _size += Mathf.Max(1, Mathf.FloorToInt(growScale));
            float growAdjusted = _bandit1 ? 1 : 1.4f;
            _hat.localScale = new Vector3(1, 1, 1 + _size * DougDimmadomeOwnerOfTheDimmsdaleDimmadome.growAmount * growAdjusted);            

            _hatRescale.RescaleTop();
        }

        void OnDestroy() {
            DougDimmadomeOwnerOfTheDimmsdaleDimmadome.Instance.onHatGrow -= grow;
        }
    }
}
