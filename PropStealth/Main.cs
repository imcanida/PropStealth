using BepInEx;
using Jotunn.Utils;

namespace PropStealth
{
    [BepInPlugin("github.imcanida.propstealth", "PropStealth", "1.0.0")]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    public class ModMain : BaseUnityPlugin
    {
        private void Awake()
        {
            AssetManager.LoadConfigs();
            AssetManager.RegisterPrefabs();
            AssetManager.RegisterLocalization();

            On.Player.SetCrouch += Hooks.PlayerSetCrouch.Invoke;
        }
    }
}