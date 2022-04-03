using BepInEx;
using BepInEx.Configuration;
using System.Linq;

namespace Zinal.Modding.ThePlanetCrafter.ModdingAPI
{
    [BepInPlugin("Zinal.Modding.ThePlanetCrafter.ModdingAPI", "Modding API", "1.0.1")]
    [BepInProcess("Planet Crafter.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static BepInEx.Logging.ManualLogSource PluginLogger { get; private set; }
        private static HarmonyLib.Harmony _Harmony;
        private static bool _PluginIncompabilityChecked = false;

        public Plugin()
        {
            PluginLogger = Logger;
            Configuration.Init(this);

            _Harmony = new HarmonyLib.Harmony("Zinal.Modding.ThePlanetCrafter.ModdingAPI");

            if (Configuration.PatchGameStateEvents.Value)
            {
                _Harmony.PatchAll(typeof(Patches.GameState_Patches));
                Logger.LogDebug("Patching GameState");
            }

            if (Configuration.PatchWeatherEvents.Value)
            {
                _Harmony.PatchAll(typeof(Patches.Weather_Patches));
                Logger.LogDebug("Patching Weather");
            }

            if (Configuration.PatchMachinesEvents.Value)
            {
                _Harmony.PatchAll(typeof(Patches.Machine_Patches));
                Logger.LogDebug("Patching Machines");
            }

            if (Configuration.PatchBuildingEvents.Value)
            {
                _Harmony.PatchAll(typeof(Patches.Building_Patches));
                Logger.LogDebug("Patching Building");
            }
        }

        private void Awake()
        {
            Logger.LogInfo("Modding API loaded successfully.");
        }

        private void FixedUpdate()
        {
            if(!_PluginIncompabilityChecked)
            {
                CheckIncompability();
                _PluginIncompabilityChecked = true;
            }
        }

        private void CheckIncompability()
        {
            int incompatibilities = 0;

            if (Configuration.PatchMachinesEvents.Value && BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("PlanetCrafterPlugins.OreExtractorTweaks_Plugin"))
            {
                Logger.LogWarning($"OreExtractorTweaks plugin found and {Configuration.PatchMachinesEvents.Definition.Key} is enabled! This will most likely cause OreExtractorTweaks to not function properly!");
                incompatibilities++;
            }

            Logger.LogDebug($"Found {incompatibilities} incompatibilities.");
        }

        private void OnDestroy()
        {
            _Harmony?.UnpatchSelf();
        }
    }
}
