using PropStealth.Models;
using System.Collections.Generic;
using UnityEngine;

namespace PropStealth.Hooks
{
    internal class PlayerSetCrouch
    {
        public class CrouchEffectSpawner
        {
            public int index { get; set; } = 0;
            public GameObject SpawnedObject { get; set; }
            public GameObject PlayersMeshObject { get; set; }
            public List<AssetToLoad> CrouchHideAssets = new List<AssetToLoad>()
            {
                new AssetToLoad()
                {
                        assetBundle = "propstealth",
                        assetPrefab = "CrouchRock"
                },
                new AssetToLoad()
                {
                        assetBundle = "propstealth",
                        assetPrefab = "CrouchBarrel"
                },
                new AssetToLoad()
                {
                        assetBundle = "propstealth",
                        assetPrefab = "CrouchAnvil"
                },
                new AssetToLoad()
                {
                        assetBundle = "propstealth",
                        assetPrefab = "CrouchDeerRug"
                },
                new AssetToLoad()
                {
                        assetBundle = "propstealth",
                        assetPrefab = "CrouchForgeBucket"
                },
                new AssetToLoad()
                {
                        assetBundle = "propstealth",
                        assetPrefab = "CrouchGuardStone"
                },
            };

            internal void HideIt()
            {
                if (SpawnedObject != null)
                {
                    SpawnedObject
                    .AssureComponent<TimedDestruction>()
                    .DestroyNow();

                    SpawnedObject = null;
                }
                if (PlayersMeshObject != null && !PlayersMeshObject.activeSelf)
                {
                    PlayersMeshObject.SetActive(true);
                }
            }

            internal void ShowIt()
            {
                if (CrouchHideAssets == null) return;
                if (CrouchHideAssets.Count > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, CrouchHideAssets.Count);
                    var asset = CrouchHideAssets[randomIndex];

                    Player player = Player.m_localPlayer;
                    if (PlayersMeshObject == null)
                    {
                        PlayersMeshObject = player.transform.RecursiveFind("body").gameObject;
                    }
                    GameObject assetToMaintain = AssetManager.GrabZNetPrefab(asset.assetBundle, asset.assetPrefab);
                    SpawnedObject = UnityEngine.Object.Instantiate<GameObject>(
                        assetToMaintain,
                        player.transform.position,
                        Quaternion.identity,
                        parent: player.transform
                    );

                    PlayersMeshObject.SetActive(false);
                }
            }
        }

        public static CrouchEffectSpawner SpawnMaintainer { get; set; }

        internal static void Invoke(On.Player.orig_SetCrouch orig, Player self, bool crouch)
        {
            // Prefix
            if (crouch)
            {
                if (SpawnMaintainer == null)
                {
                    SpawnMaintainer = new CrouchEffectSpawner();
                }
                // Show it.
                SpawnMaintainer.ShowIt();
            }
            else
            {
                // Hide it.
                SpawnMaintainer.HideIt();
            }

            orig(self, crouch);

            // Postfix

        }
    }
}