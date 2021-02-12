using RoR2;
using R2API.Utils;
using System.Linq;
using UnityEngine;
using System.Reflection;

namespace DimmaBandit {
    public class DimmaBanditHatGrower : MonoBehaviour {

        private float _size = 0;

        private Transform _hat;
        private HatRescale _hatRescale;

        public static bool AfterLoop;

        public DimmaBanditHatGrower init(CharacterModel model) {

            DougDimmadomeOwnerOfTheDimmsdaleDimmadome.Instance.onHatGrow += grow;

            _hat = Instantiate(DougDimmadomeOwnerOfTheDimmsdaleDimmadome.dimmadomePrefab, transform).transform;
            _hatRescale = _hat.GetComponent<HatRescale>();

            SetRendererInfos(model);

            grow(0);

            return this;
        }

        private void SetRendererInfos(CharacterModel model) {
            CharacterModel.RendererInfo[] modelInfos = model.baseRendererInfos;

            CharacterModel.RendererInfo[] hatInfos = new CharacterModel.RendererInfo[2];

            Material banditMaterial = Reflection.GetFieldValue<SkinnedMeshRenderer>(model, "mainSkinnedMeshRenderer").material;

            for (int i = 0; i < _hatRescale.hatRenderers.Length; i++) {
                hatInfos[i] = new CharacterModel.RendererInfo {
                    defaultMaterial = banditMaterial,
                    renderer = _hatRescale.hatRenderers[i],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false
                };
            }
            model.baseRendererInfos = modelInfos.Concat(hatInfos).ToArray();
        }

        private void grow(float growScale) {
            _size += Mathf.Max(1, Mathf.FloorToInt(growScale));
            _hat.localScale = new Vector3(1, 1, _size * DougDimmadomeOwnerOfTheDimmsdaleDimmadome.growAmount);            

            _hatRescale.RescaleTop();
        }

        void OnDestroy() {
            DougDimmadomeOwnerOfTheDimmsdaleDimmadome.Instance.onHatGrow -= grow;
        }
    }
}
