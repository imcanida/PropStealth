using Jotunn.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PropStealth
{
    public class PrefabToLoad<T>
    {
        public int SortOrder { get; set; } = 0;

        public string AbilityName { get; set; }

        public string AssetPath { get; set; }

        public bool Craftable { get; set; } = true;
        public bool Loaded { get; set; } = false;
        public GameObject LoadedPrefab { get; set; }
        public bool IsProjectile { get; set; }

        public List<AddComponentToTransform> ComponentsToAdd { get; set; } = new List<AddComponentToTransform>();

        public class AddComponentToTransform
        {
            public string name { get; set; }
            public Component componentToAdd { get; set; }
            public Action SetupComponent { get; set; }
        }

        public T Config { get; set; }

        public List<LocalizationConfig> LocalizationConfigs = new List<LocalizationConfig>();

        public void AddTranslation(string languageType, string objectKey, string value)
        {
            var localizationConfig = LocalizationConfigs.FirstOrDefault(i => i.Language == languageType);
            
            if(localizationConfig != null)
            {
                if (localizationConfig.Translations.ContainsKey(objectKey))
                {
                    return;
                }
                localizationConfig.Translations.Add(objectKey, value);
            }
            else
            {
                localizationConfig = new LocalizationConfig(languageType);
                localizationConfig.Translations.Add(objectKey, value);
                LocalizationConfigs.Add(localizationConfig);

            }
        }
    }
}
