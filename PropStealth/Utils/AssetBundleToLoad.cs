using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;
using Logger = Jotunn.Logger;
using System.Linq;

namespace PropStealth
{
    public class ItemConfigExt : ItemConfig
    {
        public string AssetBundle { get; set; }
        public string AssetName { get; set; }
    }


    public class AssetBundleToLoad
    {
        public string AssetBundleName { get; set; }
        public bool Loaded { get; set; } = false;

        public List<PrefabToLoad<ItemConfigExt>> Items = new List<PrefabToLoad<ItemConfigExt>>();
        public List<PrefabToLoad<PieceConfig>> Pieces = new List<PrefabToLoad<PieceConfig>>();
        public List<PrefabToLoad<bool>> Prefabs = new List<PrefabToLoad<bool>>();

        public List<PrefabToLoad<RuntimeAnimatorController>> AnimationOverrideControllers = new List<PrefabToLoad<RuntimeAnimatorController>>();

        public void Load()
        {
            try
            {
                //Load embedded resources and extract gameobject(s)
                var assetBundle = AssetManager.LoadAssetBundle(AssetBundleName);

                foreach (var prefabToLoad in Pieces)
                {
                    try
                    {
                        prefabToLoad.LoadedPrefab = assetBundle.LoadAsset<GameObject>(prefabToLoad.AssetPath);

                        // add the piece with jotunn and unload the bundle
                        var customPiece = new CustomPiece(prefabToLoad.LoadedPrefab, prefabToLoad.Config);
                        PieceManager.Instance.AddPiece(customPiece);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Failed to Load {prefabToLoad.AssetPath}: {ex}");
                    }
                }

                foreach (var prefabToLoad in Items)
                {
                    try
                    {
                        prefabToLoad.LoadedPrefab = assetBundle.LoadAsset<GameObject>(prefabToLoad.AssetPath);

                        var itemDropScript = prefabToLoad.LoadedPrefab.GetComponent<ItemDrop>();
                        if (itemDropScript != null)
                        {
                            if (!string.IsNullOrEmpty(prefabToLoad.Config.AssetBundle) && !string.IsNullOrEmpty(prefabToLoad.Config.AssetName))
                            {
                                itemDropScript.m_itemData.m_shared.m_icons = new Sprite[]
                                {
                                    AssetManager.LoadSpriteFromBundle(prefabToLoad.Config.AssetBundle, prefabToLoad.Config.AssetName)
                                };
                            }
                        }
                        var customItem = new CustomItem(prefabToLoad.LoadedPrefab, false, prefabToLoad.Config);
                        ItemManager.Instance.AddItem(customItem);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Failed to Load {prefabToLoad.Config.Name}: {ex}");
                    }
                }

                foreach (var prefabToLoad in Prefabs.OrderBy(i => i.SortOrder))
                {
                    try
                    {
                        prefabToLoad.LoadedPrefab = assetBundle.LoadAsset<GameObject>(prefabToLoad.AssetPath);
                        PrefabManager.Instance.AddPrefab(prefabToLoad.LoadedPrefab);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Failed to Load {prefabToLoad.AssetPath}: {ex}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
    }
}
