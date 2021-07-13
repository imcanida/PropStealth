using Jotunn.Configs;
using Jotunn.Managers;
using Jotunn.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using MonoMod.Utils;
using System.Reflection;

namespace PropStealth
{
    public static class AssetManager
    {
        public static List<AssetBundleToLoad> AssetBundles = new List<AssetBundleToLoad>();

        public static PrefabToLoad<bool> GrabPrefab(string bundleName, string prefab)
        {
            var assetBundle = AssetBundles.FirstOrDefault(i => i.AssetBundleName.Equals(bundleName));
            var prefabFound = assetBundle.Prefabs.FirstOrDefault(i => i.AssetPath.Contains(prefab));
            return prefabFound;
        }

        public static GameObject GrabZNetPrefab(string assetBundle, string prefabName)
        {
            var assetbundle = AssetBundles.FirstOrDefault(i => i.AssetBundleName.Equals(assetBundle));
            var prefabToUse = assetbundle.Prefabs.FirstOrDefault(i => i.AssetPath.Contains(prefabName));
            return ZNetScene.instance.GetPrefab(prefabToUse.LoadedPrefab.name);
        }

        public static void LoadConfigs()
        {
            // Prefabs
            AssetBundles.Add(Config.AssetConfig.Assets);
        }

        public static void RegisterLocalization()
        {
            List<LocalizationConfig> localizationConfigs = new List<LocalizationConfig>();
            foreach (var assetBundle in AssetBundles)
            {
                foreach (var prefab in assetBundle.Pieces)
                {
                    // Grab all the localization configs that exist on peices.
                    localizationConfigs.AddRange(prefab.LocalizationConfigs);
                }

                foreach (var prefab in assetBundle.Items)
                {
                    // Grab all the localization configs that exist on items.
                    localizationConfigs.AddRange(prefab.LocalizationConfigs);
                }

                foreach (var prefab in assetBundle.Prefabs)
                {
                    // Grab all the localization configs that exist on prefabs? Prob none?.
                    localizationConfigs.AddRange(prefab.LocalizationConfigs);
                }
            }

            var localizationByLanguage = new Dictionary<string, LocalizationConfig>();
            // Aggregate all the Localizations by Lauguage
            foreach (var localizationConfig in localizationConfigs)
            {
                if (localizationByLanguage.ContainsKey(localizationConfig.Language))
                {
                    // Add it to this langauge.
                    localizationByLanguage[localizationConfig.Language].Translations.AddRange(localizationConfig.Translations);
                }
                else
                {
                    // No laguage is registered yet with this name add it in.
                    localizationByLanguage.Add(localizationConfig.Language, localizationConfig);
                }
            }
            Jotunn.Logger.LogDebug("Loading Localization: ");
            foreach (var localizations in localizationByLanguage)
            {
                var allKeys = localizations.Value.Translations.Select(i => i.Key).ToArray();
                var allValues = localizations.Value.Translations.Select(i => i.Value).ToArray();
                Debug.Log($"{localizations.Key}");
                Debug.Log($"{string.Join(", ", allKeys)}");
                Debug.Log($"{string.Join(", ", allValues)}");
            }

            Jotunn.Logger.LogDebug(localizationByLanguage);

            // We have our aggregated terms by language -- Add the localizations configs.
            foreach (var localizationConfig in localizationByLanguage)
            {
                LocalizationManager.Instance.AddLocalization(localizationConfig.Value);
            }
        }

        public static void RegisterPrefabs()
        {
            // Aggregate all assetbundles by name so it easier to search through things.
            var distinctBundles = new Dictionary<string, AssetBundleToLoad>();
            foreach (var assetBundle in AssetBundles)
            {
                if (distinctBundles.ContainsKey(assetBundle.AssetBundleName))
                {
                    distinctBundles[assetBundle.AssetBundleName].Prefabs.AddRange(assetBundle.Prefabs);
                    distinctBundles[assetBundle.AssetBundleName].Pieces.AddRange(assetBundle.Pieces);
                    distinctBundles[assetBundle.AssetBundleName].Items.AddRange(assetBundle.Items);
                    distinctBundles[assetBundle.AssetBundleName].AnimationOverrideControllers.AddRange(assetBundle.AnimationOverrideControllers);
                }
                else
                {
                    distinctBundles.Add(assetBundle.AssetBundleName, assetBundle);
                }
            }

            AssetBundles = distinctBundles.Values.ToList();

            foreach (var assetBundle in AssetBundles)
            {
                assetBundle.Load();
            }

            UnloadAssetBundles();
        }

        /// <summary>
        /// Creates a streamified embedded resource into a byte Array.
        /// </summary>
        /// <param name="input">Name of asset</param>
        /// <returns></returns>
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Creates an Embbedded Resources into a Stream.
        /// </summary>
        /// <param name="assetName">Name of asset.</param>
        /// <returns></returns>
        public static Stream StreamifyEmbeddedResource(string assetName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(assetName);
        }

        public static Texture2D Get2DTexture(string assetName)
        {
            var iconStream = StreamifyEmbeddedResource(assetName);
            var iconByteArray = ReadFully(iconStream);
            Texture2D iconAsTexture = new Texture2D(2, 2);
            iconAsTexture.LoadImage(iconByteArray);
            return iconAsTexture;
        }

        // function to get an embeded icon
        private static Dictionary<string, Sprite> _loadedSprites = new Dictionary<string, Sprite>();
        private static Dictionary<string, Material> _loadedMaterials = new Dictionary<string, Material>();

        public static Sprite GetResourceSprite(string resourceName)
        {
            if (_loadedSprites.ContainsKey(resourceName))
            {
                return _loadedSprites[resourceName];
            }

            var iconAsTexture = Get2DTexture(resourceName);
            var sprite = Sprite.Create(iconAsTexture, new Rect(0f, 0f, iconAsTexture.width, iconAsTexture.height), Vector2.zero);
            _loadedSprites.Add(resourceName, sprite);
            return sprite;
        }

        public static string LoadText(string path)
        {
            // Just wrapping it to be semi easier..
            return Jotunn.Utils.AssetUtils.LoadText(path);
        }

        // So we don't access the same bundle multiple times.
        private static Dictionary<string, AssetBundle> _assetBundlesByName = new Dictionary<string, AssetBundle>();
        public static AssetBundle LoadAssetBundle(string assetBundle)
        {
            AssetBundle assetBundleToUse;
            if (_assetBundlesByName.ContainsKey(assetBundle))
            {
                assetBundleToUse = _assetBundlesByName[assetBundle];
            }
            else
            {
                assetBundleToUse = AssetUtils.LoadAssetBundleFromResources(assetBundle, Assembly.GetExecutingAssembly());
                _assetBundlesByName.Add(assetBundle, assetBundleToUse);
            }
            if (assetBundleToUse == null) return null;
            return assetBundleToUse;
        }

        public static Texture LoadTextureFromBundle(string assetBundle, string assetPath)
        {
            AssetBundle assetBundleToUse;
            if (_assetBundlesByName.ContainsKey(assetBundle))
            {
                assetBundleToUse = _assetBundlesByName[assetBundle];
            }
            else
            {
                assetBundleToUse = AssetUtils.LoadAssetBundleFromResources(assetBundle, Assembly.GetExecutingAssembly());
                _assetBundlesByName.Add(assetBundle, assetBundleToUse);
            }
            if (assetBundleToUse == null) return null;
            return assetBundleToUse.LoadAsset<Texture>(assetPath);
        }

        public static Sprite LoadSpriteFromBundle(string assetBundle, string assetPath)
        {
            var uiPathKey = $"{assetBundle}_{assetPath}";
            if (_loadedSprites.ContainsKey(uiPathKey))
            {
                return _loadedSprites[uiPathKey];
            }

            AssetBundle assetBundleToUse;
            if (_assetBundlesByName.ContainsKey(assetBundle))
            {
                assetBundleToUse = _assetBundlesByName[assetBundle];
            }
            else
            {
                assetBundleToUse = AssetUtils.LoadAssetBundleFromResources(assetBundle, Assembly.GetExecutingAssembly());
                _assetBundlesByName.Add(assetBundle, assetBundleToUse);
            }
            if (assetBundleToUse == null) return null;
            Texture texture = assetBundleToUse.LoadAsset<Texture>(assetPath);
            if (texture == null) return null;

            var sprite = Sprite.Create((Texture2D)texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero);
            _loadedSprites.Add(uiPathKey, sprite);
            return sprite;
        }

        public static Material LoadMaterialFromBundle(string assetBundle, string assetPath)
        {
            var uiPathKey = $"{assetBundle}_{assetPath}";
            if (_loadedMaterials.ContainsKey(uiPathKey))
            {
                return _loadedMaterials[uiPathKey];
            }

            AssetBundle assetBundleToUse;
            if (_assetBundlesByName.ContainsKey(assetBundle))
            {
                assetBundleToUse = _assetBundlesByName[assetBundle];
            }
            else
            {
                assetBundleToUse = AssetUtils.LoadAssetBundleFromResources(assetBundle, Assembly.GetExecutingAssembly());
                _assetBundlesByName.Add(assetBundle, assetBundleToUse);
            }
            if (assetBundleToUse == null) return null;
            Material material = assetBundleToUse.LoadAsset<Material>(assetPath);
            if (material == null) return null;
            _loadedMaterials.Add(uiPathKey, material);
            return material;
        }

        public static void UnloadAssetBundles()
        {
            try
            {
                if (_assetBundlesByName == null) return;
                if (_assetBundlesByName.Count == 0) return;

                foreach (var item in _assetBundlesByName)
                {
                    if (item.Value == null) continue;
                    item.Value?.Unload(false);
                }
                _assetBundlesByName = new Dictionary<string, AssetBundle>();
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }
    }
}
