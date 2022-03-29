using BepInEx;

namespace Zinal.Modding.ThePlanetCrafter.ModdingAPI
{
    [BepInPlugin("Zinal.Modding.ThePlanetCrafter.ModdingAPI", "Modding API", "1.0.0")]
    [BepInProcess("Planet Crafter.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static BepInEx.Logging.ManualLogSource PluginLogger { get; private set; }

        static Plugin()
        {
            HarmonyLib.Harmony.CreateAndPatchAll(typeof(Patches));
        }

        public Plugin()
        {
            PluginLogger = Logger;
        }

        private void Awake()
        {
            Logger.LogInfo("Modding API loaded successfully.");
        }
    }
}
