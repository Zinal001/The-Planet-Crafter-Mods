using BepInEx;
using System.Linq;
using Zinal.Modding.ThePlanetCrafter.ModdingAPI;

namespace Zinal.Modding.ThePlanetCrafter.OreExtractorFullWarning
{
    [BepInPlugin("Zinal.Modding.ThePlanetCrafter.OreExtractorFullWarning", "Ore Extractor Full Warning", PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("Zinal.Modding.ThePlanetCrafter.ModdingAPI", BepInDependency.DependencyFlags.HardDependency)]
    [BepInProcess("Planet Crafter.exe")]
    public class Plugin : BaseUnityPlugin
    {
        private static readonly string[] MachineGenerator_Ignore = new string[] { "WaterBottle1", "TreeRoot" };
        private static readonly string[] MachineGenerator_Ores = new string[] { "Super Alloy", "Osmium", "Sulfur", "Aluminium", "Iridium", "Uranium", "Iron" };

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
            Logger.LogInfo("Plugin was loaded correctly");

            LocalizationEx.OnLocalizationLoaded += LocalizationEx_OnLocalizationLoaded;
            Events.Machines.ObjectGenerated += Machines_ObjectGenerated;
        }

        private void Machines_ObjectGenerated(object sender, Events.Machines.MachineGeneratorEventArgs e)
        {
            if(sender is SpaceCraft.MachineGenerator generator)
            {
                if(e.Inventory.GetInsideWorldObjects().Count == e.Inventory.GetSize() - 1)
                {
                    if (generator.groupDatas.Any(g => MachineGenerator_Ignore.Contains(g.id)))
                        return;

                    string oreName = "";

                    foreach (string ore in MachineGenerator_Ores)
                    {
                        if (generator.groupDatas.Any(g => g.id == ore))
                        {
                            oreName = ore;
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(oreName))
                        oreName = "Iron";

                    string pos = $"{generator.transform.position.x}:{generator.transform.position.y}:{generator.transform.position.z}";
                    Plugin.PluginLogger.LogDebug($"Ore extractor mining {oreName} at {pos} is full");
                    MijuTools.Managers.GetManager<SpaceCraft.BaseHudHandler>().DisplayCursorText("ORE_EXTRACTOR_FULL", 10f, $"Mining {oreName} at {pos}");
                }
            }
        }

        private void LocalizationEx_OnLocalizationLoaded(object sender, System.EventArgs e)
        {
            Logger.LogDebug("LocalizationEx has been loaded!");
            LocalizationEx.SetLocalizedText("ORE_EXTRACTOR_FULL", "Ore Extractor Full - ");
        }
    }
}
