using BepInEx;
using UnityEngine;
using System.Collections.Generic;

namespace ThiccifyMutiny
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.TheTimeSweeper.ThiccifyMutiny", "Thiccify Mutiny", "0.2.0")]
    public class Main : BaseUnityPlugin
    {

        private bool isThicc = false;

        private bool setupThicc = false;

        private bool removeScarfAndSkirt = false;

        private bool applyRuntimeMeshModification = true;

        private bool mageMeshThicc = false;

        private bool huntressMeshThicc = false;

        private Vector3 thighPositionLeft = Vector3.zero;
        
        private Vector3 thighPositionRight = Vector3.zero;
        
        private Vector3 calfPositionLeft = Vector3.zero;
        
        private Vector3 calfPositionRight = Vector3.zero;
        
        private Vector3 pelvisPosition = Vector3.zero;

        // Token: 0x06000001 RID: 1 RVA: 0x0000208C File Offset: 0x0000028C
        public void Awake()
        {

            On.RoR2.CharacterBody.Start += CharacterBody_StartHook; 
        }

        private void CharacterBody_StartHook(On.RoR2.CharacterBody.orig_Start orig, RoR2.CharacterBody self)
        {
            orig(self);
            bool isPlayerControlled = self.isPlayerControlled;
            if (isPlayerControlled)
            {
                Transform modelTransform = self.modelLocator.modelTransform;
                bool flag = modelTransform.Find("MageArmature");
                Vector3 vector = new Vector3(1.25f, 1.25f, 1.25f);
                Vector3 vector2 = new Vector3(1f, 1f, 1f);
                vector2 = Vector3.Scale(vector2, new Vector3(1f, 1f / vector.y, 1f));
                Vector3 localScale = new Vector3(1.1f, 1f, 1.1f);
                Vector3 vector3 = flag ? new Vector3(0f, 0f, 0.05f) : Vector3.zero;
                Vector3 zero = Vector3.zero;
                Vector3 vector4 = Vector3.Scale(zero, new Vector3(0f, -1f, 0f));
                Vector3 vector5 = new Vector3(-1f, 1f, 1f);
                Transform transform = null;
                for (int i = 0; i < modelTransform.childCount; i++)
                {
                    Transform child = modelTransform.GetChild(i);
                    bool flag2 = child.Find("ROOT");
                    if (flag2)
                    {
                        transform = child;
                        break;
                    }
                }
                bool flag3 = transform;
                if (flag3)
                {
                    Vector3 vector6 = new Vector3(vector.x, 1f, vector.z);
                    Transform transform2 = transform.Find("ROOT/IKLegTarget.l");
                    bool flag4 = transform2;
                    if (flag4)
                    {
                        transform2.localPosition = Vector3.Scale(transform2.localPosition, vector6);
                    }
                    Transform transform3 = transform.Find("ROOT/IKLegTarget.r");
                    bool flag5 = transform3;
                    if (flag5)
                    {
                        transform3.localPosition = Vector3.Scale(transform3.localPosition, vector6);
                    }
                    Transform transform4 = transform.Find("ROOT/base/pelvis");
                    bool flag6 = transform4;
                    if (flag6)
                    {
                        bool flag7 = !this.setupThicc;
                        if (flag7)
                        {
                            this.pelvisPosition = transform4.localPosition;
                        }
                        transform4.localScale = vector;
                        transform4.localPosition = this.pelvisPosition + vector3;
                        Transform transform5 = transform4.Find("thigh.r");
                        Transform transform6 = transform4.Find("thigh.l");
                        bool flag8 = transform5;
                        if (flag8)
                        {
                            transform5.localScale = vector2;
                            Transform calfR = transform5.Find("calf.r");
                            bool flag9 = calfR;
                            if (flag9)
                            {
                                calfR.localScale = localScale;
                                bool flag10 = !this.setupThicc;
                                if (flag10)
                                {
                                    this.thighPositionRight = transform5.localPosition;
                                    this.calfPositionRight = calfR.localPosition;
                                }
                                calfR.localPosition = this.calfPositionRight + vector4;
                            }
                            transform5.localPosition = this.thighPositionRight + zero;
                        }
                        bool flag11 = transform6;
                        if (flag11)
                        {
                            transform6.localScale = vector2;
                            Transform transform8 = transform6.Find("calf.l");
                            bool flag12 = transform8;
                            if (flag12)
                            {
                                transform8.localScale = localScale;
                                bool flag13 = !this.setupThicc;
                                if (flag13)
                                {
                                    this.thighPositionLeft = transform6.localPosition;
                                    this.calfPositionLeft = transform8.localPosition;
                                }
                                transform8.localPosition = this.calfPositionLeft + vector4;
                            }
                            transform6.localPosition = this.thighPositionLeft + Vector3.Scale(zero, vector5);
                        }
                        this.setupThicc = true;
                    }
                }
                this.MakeThicc(self);
            }
        }

        // Token: 0x06000002 RID: 2 RVA: 0x000020A4 File Offset: 0x000002A4
        private void MakeThicc(RoR2.CharacterBody self)
        {
            Transform modelTransform = self.modelLocator.modelTransform;
            Transform transform = null;
            for (int i = 0; i < modelTransform.childCount; i++)
            {
                Transform child = modelTransform.GetChild(i);
                bool flag = child.Find("ROOT");
                if (flag)
                {
                    transform = child;
                    break;
                }
            }
            bool flag2 = transform;
            if (flag2)
            {
                Transform transform2 = transform.Find("ROOT/base/stomach/chest");
                Transform transform3 = transform.Find("ROOT/base/stomach/chest/neck/head");
                Transform transform4 = transform.Find("ROOT/base/stomach/pelvis");
                Vector3 vector = new Vector3(float.NaN, float.NaN, float.NaN);
                bool flag3 = modelTransform.Find("MageArmature");
                if (flag3)
                {
                    bool flag4 = this.removeScarfAndSkirt;
                    if (flag4)
                    {
                        modelTransform.Find("MageCapeMesh").gameObject.SetActive(false);
                    }

                    modelTransform.Find("MageCapeMesh").localScale.Scale(new Vector3(1, 0.4f, 1));

                    bool flag5 = this.applyRuntimeMeshModification;
                    if (flag5)
                    {
                        //for (int j = 0; j < transform2.childCount; j++)
                        //{
                        //    Transform child2 = transform2.GetChild(j);
                        //    bool flag6 = child2.name == "Jets";
                        //    if (flag6)
                        //    {
                        //        child2.localScale.Scale(new Vector3(0.7f, 0.5f, 0.7f));
                        //    }
                        //}
                        bool flag7 = !this.mageMeshThicc;
                        if (flag7)
                        {
                            SkinnedMeshRenderer component = modelTransform.Find("MageMesh").GetComponent<SkinnedMeshRenderer>();
                            Vector3[] vertices = component.sharedMesh.vertices;
                            BoneWeight[] boneWeights = component.sharedMesh.boneWeights;
                            List<Vector2> list = new List<Vector2>();
                            component.sharedMesh.GetUVs(0, list);
                            float[] array = new float[]
                            {
                                0.155523732f,
                                0.161421865f,
                                0.04235405f,
                                0.00210663863f,
                                0.115523428f,
                                0.169746473f,
                                0.160440609f,
                                0.547309041f,
                                0.208297536f,
                                0.175094485f,
                                0.5473088f,
                                0.5701668f,
                                0.46927318f,
                                0.171110779f,
                                0.172749758f,
                                0.9341732f,
                                0.790172338f
                            };
                            for (int k = 0; k < vertices.Length; k++)
                            {
                                Vector2 vector2 = list[k];
                                BoneWeight boneWeight = boneWeights[k];
                                //bool flag8 = boneWeight.boneIndex0 == 3;
                                //if (flag8)
                                //{
                                //    for (int l = 0; l < array.Length; l++)
                                //    {
                                //        bool flag9 = vector2.y < 0.3f || Mathf.Abs(vector2.x - array[l]) < 0.0005f || Mathf.Abs(vector2.y - array[l]) < 0.0005f;
                                //        if (flag9)
                                //        {
                                //            vertices[k] = vector;
                                //            break;
                                //        }
                                //    }
                                //}
                            }
                            component.sharedMesh.vertices = vertices;
                            this.mageMeshThicc = true;
                        }
                    }
                }
                else
                {
                    bool flag10 = modelTransform.Find("HuntressArmature");
                    flag10 = false;
                    if (flag10)
                    {
                        bool flag11 = this.removeScarfAndSkirt;
                        if (flag11)
                        {
                            modelTransform.Find("HuntressScarfMesh").gameObject.SetActive(false);
                        }
                        bool flag12 = !this.huntressMeshThicc && this.applyRuntimeMeshModification;
                        if (flag12)
                        {
                            SkinnedMeshRenderer component2 = modelTransform.Find("HuntressMesh").GetComponent<SkinnedMeshRenderer>();
                            Vector3[] vertices2 = component2.sharedMesh.vertices;
                            BoneWeight[] boneWeights2 = component2.sharedMesh.boneWeights;
                            List<Vector2> list2 = new List<Vector2>();
                            List<Color32> list3 = new List<Color32>();
                            Color32 color = new Color32(0, 0, 0, byte.MaxValue);
                            component2.sharedMesh.GetColors(list3);
                            component2.sharedMesh.GetUVs(0, list2);
                            Vector2 vector3 = new Vector3(0.7746712f, 0.6335765f);
                            Vector3 vector4= new Vector3(0f, -0.044645f, 1.419872f);
                            Vector3 vector5 = new Vector3(0.071101f, -0.146735f, 1.416376f);
                            for (int m = 0; m < vertices2.Length; m++)
                            {
                                Vector2 vector6 = list2[m];
                                BoneWeight boneWeight2 = boneWeights2[m];
                                bool flag13 = boneWeight2.boneIndex0 == 39 & (vector6.x > 0.6699667f || Mathf.Abs(vector6.x - 0.59991926f) < 0.0001f);
                                if (flag13)
                                {
                                    vertices2[m] = vector;
                                }
                                else
                                {
                                    bool flag14 = boneWeight2.boneIndex0 == 3;
                                    if (flag14)
                                    {
                                        Vector3 vector7 = vertices2[m];
                                        float num = Vector2.Distance(vector6, vector3);
                                        bool flag15 = num < 0.05f;
                                        if (flag15)
                                        {
                                            vertices2[m] = vector;
                                            vector7 = vector;
                                        }
                                        else
                                        {
                                            bool flag16 = vector6.x == 0.5669036f || vector6.x == 0.583716333f || vector6.x == 0.6001201f || vector6.x == 0.6146834f || vector6.x == 0.6304007f;
                                            if (flag16)
                                            {
                                                list2[m] = new Vector2(0.636950552f, 0.638931155f);
                                            }
                                        }
                                        bool flag17 = Mathf.Abs(vector7.x) < 0.0765f && vector7.y < -0.0875f;
                                        if (flag17)
                                        {
                                            bool flag18 = vector7.x == 0f;
                                            bool flag19 = flag18;
                                            if (flag19)
                                            {
                                                float num2 = 1f - Mathf.Min(1f, Mathf.Abs(vector7.z - 1.317341f) / 0.22f);
                                                vertices2[m] = Vector3.Lerp(vector7, vector4, num2 * 0.4f);
                                                list3[m] = Color32.Lerp(list3[m], color, num2);
                                            }
                                        }
                                    }
                                }
                            }
                            component2.sharedMesh.vertices = vertices2;
                            component2.sharedMesh.SetUVs(0, list2);
                            component2.sharedMesh.SetColors(list3);
                            component2.sharedMesh.RecalculateNormals();
                            this.huntressMeshThicc = true;
                        }
                    }
                }
                this.isThicc = true;
            }
        }
    }
}


