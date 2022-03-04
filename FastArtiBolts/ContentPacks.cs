using RoR2;
using RoR2.ContentManagement;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Modules {

    public class ContentPacks : IContentPackProvider
    {
        public ContentPack contentPack = new ContentPack();
        public string identifier => FastArtiBolts.FastBoltsMod.MODUID;

        public static List<GameObject> projectilePrefabs = new List<GameObject>();

        public static List<SkillDef> skillDefs = new List<SkillDef>();
        public static List<Type> entityStates = new List<Type>();

        //public static List<EffectDef> effectDefs = new List<EffectDef>();

        public void Initialize()
        {
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
        }
        
        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(this);
        }

        public System.Collections.IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            this.contentPack.identifier = this.identifier;

            //Debug.LogWarning("added" + (projectilePrefabs[0] != null));
            contentPack.projectilePrefabs.Add(projectilePrefabs.ToArray());

            contentPack.skillDefs.Add(skillDefs.ToArray());
            contentPack.entityStateTypes.Add(entityStates.ToArray());

            //contentPack.effectDefs.Add(effectDefs.ToArray());

            args.ReportProgress(1f);
            yield break;
        }

        public System.Collections.IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(this.contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public System.Collections.IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }
    }
}