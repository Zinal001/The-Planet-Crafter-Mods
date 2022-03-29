using HarmonyLib;
using MijuTools;
using SpaceCraft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Zinal.Modding.ThePlanetCrafter.OreExtractorFullWarning
{
    internal static class Patches
    {
        /*private static readonly string[] MachineGenerator_Ignore = new string[] { "WaterBottle1", "TreeRoot" };
        private static readonly string[] MachineGenerator_Ores = new string[] { "Super Alloy", "Osmium", "Sulfur", "Aluminium", "Iridium", "Uranium", "Iron" };

        [HarmonyPatch(typeof(MachineGenerator), "GenerateAnObject")]
        [HarmonyPostfix()]
        public static void MachineGenerator_GenerateAnObject_Postfix(MachineGenerator __instance, Inventory ___inventory)
        {
            if(___inventory.IsFull())
            {
                if (__instance.groupDatas.Any(g => MachineGenerator_Ignore.Contains(g.id)))
                    return;

                string oreName = "";

                foreach(String ore in MachineGenerator_Ores)
                {
                    if(__instance.groupDatas.Any(g => g.id == ore))
                    {
                        oreName = ore;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(oreName))
                    oreName = "Iron";

                Vector3 position = __instance.transform.position;
                Plugin.PluginLogger.LogDebug($"Ore extractor mining {oreName} at {Mathf.Round(position.x)}:{Mathf.Round(position.y)}:{Mathf.Round(position.z)} is full");
                Managers.GetManager<BaseHudHandler>().DisplayCursorText("ORE_EXTRACTOR_FULL", 10f, $"Mining {oreName} at {Mathf.Round(position.x)}:{Mathf.Round(position.y)}:{Mathf.Round(position.z)}");
            }
        }*/

    }
}
