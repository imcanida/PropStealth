using Jotunn.Configs;
using System.Collections.Generic;

namespace PropStealth.Config
{
    public static class AssetConfig
    {
        public static AssetBundleToLoad Assets = new AssetBundleToLoad()
        {
            AssetBundleName = "propstealth",
            Prefabs = new List<PrefabToLoad<bool>>()
            {
                new PrefabToLoad<bool>()
                {
                    AssetPath = "Assets/PropStealth/CrouchBarrel.prefab"
                },
                new PrefabToLoad<bool>()
                {
                    AssetPath = "Assets/PropStealth/CrouchRock.prefab"
                },
                new PrefabToLoad<bool>()
                {
                    AssetPath = "Assets/PropStealth/CrouchDeerRug.prefab"
                },
                new PrefabToLoad<bool>()
                {
                    AssetPath = "Assets/PropStealth/CrouchForgeBucket.prefab"
                },
                new PrefabToLoad<bool>()
                {
                    AssetPath = "Assets/PropStealth/CrouchGuardStone.prefab"
                },
                new PrefabToLoad<bool>()
                {
                    AssetPath = "Assets/PropStealth/CrouchAnvil.prefab"
                }
            }
        };
    }
}
