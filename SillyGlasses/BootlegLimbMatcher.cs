using System;
using RoR2;
using UnityEngine;
//using R2API.Utils;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

//[assembly: ManualNetworkRegistration]
namespace SillyGlasses
{
    public class BootlegLimbMatcher : MonoBehaviour
    {
        // Token: 0x04002C79 RID: 11385
        public bool scaleLimbs = true;

        // Token: 0x04002C7A RID: 11386
        private bool valid;

        // Token: 0x04002C7B RID: 11387
        public LimbMatcher.LimbPair[] limbPairs;

        // Token: 0x02000795 RID: 1941
        [Serializable]
        public struct LimbPair
        {
            // Token: 0x04002C7C RID: 11388
            public Transform originalTransform;

            // Token: 0x04002C7D RID: 11389
            public string targetChildLimb;

            // Token: 0x04002C7E RID: 11390
            public float originalLimbLength;

            // Token: 0x04002C7F RID: 11391
            [NonSerialized]
            public Transform targetTransform;
        }
        private float distance;


        public void Init(LimbMatcher limbMatcher_, ChildLocator childLocator_, float distance_)
        {
            limbPairs = limbMatcher_.limbPairs;
            scaleLimbs = limbMatcher_.scaleLimbs;

            distance = distance_;

            SetChildLocator(childLocator_);
        }

        // Token: 0x06002906 RID: 10502 RVA: 0x000B2160 File Offset: 0x000B0360
        public void SetChildLocator(ChildLocator childLocator)
        {
            this.valid = true;
            for (int i = 0; i < this.limbPairs.Length; i++)
            {
                LimbMatcher.LimbPair limbPair = this.limbPairs[i];
                Transform transform = childLocator.FindChild(limbPair.targetChildLimb);
                if (!transform)
                {
                    this.valid = false;
                    return;
                }
                this.limbPairs[i].targetTransform = transform;
            }
        }

        // Token: 0x06002907 RID: 10503 RVA: 0x000B21C3 File Offset: 0x000B03C3
        private void LateUpdate()
        {
            this.UpdateLimbs();
        }

        // Token: 0x06002908 RID: 10504 RVA: 0x000B21CC File Offset: 0x000B03CC
        private void UpdateLimbs()
        {
            if (!this.valid)
            {
                return;
            }
            for (int i = 0; i < this.limbPairs.Length; i++)
            {
                LimbMatcher.LimbPair limbPair = this.limbPairs[i];
                Transform targetTransform = limbPair.targetTransform;
                if (targetTransform && limbPair.originalTransform)
                {
                    //copypaste an entire comopnent just for one line changed...
                    //less stinky ways to do this is actually more stinky with like a getcomponent every frame or something.
                    limbPair.originalTransform.position = targetTransform.position + targetTransform.forward * distance;
                    limbPair.originalTransform.rotation = targetTransform.rotation;
                    if (i < this.limbPairs.Length - 1)
                    {
                        float num = Vector3.Magnitude(this.limbPairs[i + 1].targetTransform.position - targetTransform.position);
                        float originalLimbLength = limbPair.originalLimbLength;
                        if (this.scaleLimbs)
                        {
                            Vector3 localScale = limbPair.originalTransform.localScale;
                            localScale.y = num / originalLimbLength;
                            limbPair.originalTransform.localScale = localScale;
                        }
                    }
                }
            }
        }
    }
}