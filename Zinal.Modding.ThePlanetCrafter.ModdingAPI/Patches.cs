using HarmonyLib;
using MijuTools;
using SpaceCraft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zinal.Modding.ThePlanetCrafter.ModdingAPI
{
    internal static class Patches
    {
        [HarmonyPatch(typeof(Localization), "LoadLocalization")]
        [HarmonyPostfix()]
        public static void Localization_LoadLocalization_Postfix(bool ___hasLoadedSuccesfully)
        {
            if (!LocalizationEx.LocalizationLoaded && ___hasLoadedSuccesfully)
            {
                LocalizationEx.LocalizationLoaded = true;
                LocalizationEx.TriggerLoaded();
            }

            LocalizationEx.LocalizationLoaded = ___hasLoadedSuccesfully;
        }

        internal static class GameState_Patches
        {
            [HarmonyPatch(typeof(SavedDataHandler), "LoadSavedData")]
            [HarmonyPrefix()]
            public static void SavedDataHandler_LoadSavedData_Prefix(SavedDataHandler __instance)
            {
                Events.GameState.Trigger_GameLoading(__instance, __instance.saveFileName);
            }

            [HarmonyPatch(typeof(SavedDataHandler), "LoadSavedData")]
            [HarmonyPostfix()]
            public static void SavedDataHandler_LoadSavedData_Postfix(SavedDataHandler __instance)
            {
                Events.GameState.Trigger_GameLoaded(__instance, __instance.saveFileName);
            }

            [HarmonyPatch(typeof(SavedDataHandler), "SaveWorldData")]
            [HarmonyPrefix()]
            public static void SavedDataHandler_SaveWorldData_Prefix(SavedDataHandler __instance, String _forceSaveFileName)
            {
                Events.GameState.Trigger_GameSaving(__instance, String.IsNullOrEmpty(_forceSaveFileName) ? __instance.saveFileName : _forceSaveFileName);
            }

            [HarmonyPatch(typeof(SavedDataHandler), "SaveWorldData")]
            [HarmonyPostfix()]
            public static void SavedDataHandler_SaveWorldData_Postfix(SavedDataHandler __instance, String _forceSaveFileName)
            {
                Events.GameState.Trigger_GameSaved(__instance, String.IsNullOrEmpty(_forceSaveFileName) ? __instance.saveFileName : _forceSaveFileName);
            }
        }

        internal static class Weather_Patches
        {
            [HarmonyPatch(typeof(MeteoHandler), "QueueMeteoEvent")]
            [HarmonyPrefix]
            public static bool MeteoHandler_QueueMeteoEvent_Prefix(MeteoHandler __instance, MeteoEventData _meteoEvent)
            {
                if (Events.Weather.Trigger_WeatherQueued(__instance, _meteoEvent))
                    return false;

                return true;
            }

            [HarmonyPatch(typeof(MeteoHandler), "LaunchSpecificMeteoEvent")]
            [HarmonyPrefix]
            public static bool MeteoHandler_LaunchSpecificMeteoEvent_Prefix(MeteoHandler __instance, MeteoEventData _meteoEvent)
            {
                if (Events.Weather.Trigger_WeatherChanged(__instance, _meteoEvent))
                    return false;

                return true;
            }
        }

        internal static class Machine_Patches
        {
            [HarmonyPatch(typeof(MachineGenerator), "GenerateAnObject")]
            [HarmonyPrefix]
            public static bool MachineGenerator_GenerateAnObject_Prefix(MachineGenerator __instance, Inventory ___inventory, WorldObject ___worldObject, bool ___hasEnergy)
            {
                WorldObject worldObject = WorldObjectsHandler.CreateNewWorldObject(GroupsHandler.GetGroupViaId(__instance.groupDatas[UnityEngine.Random.Range(0, __instance.groupDatas.Count)].id), 0);

                if (Events.Machines.Trigger_ObjectGenerated(__instance, ___hasEnergy, ___inventory, worldObject))
                {
                    WorldObjectsHandler.DestroyWorldObject(worldObject);
                    return false;
                }

                ___inventory.AddItem(worldObject);

                return false;
            }
        }

        internal static class Building_Patches
        {
            [HarmonyPatch(typeof(PlayerBuilder), "SetNewGhost")]
            [HarmonyPrefix]
            public static bool PlayerBuilder_SetNewGhost_Prefix(PlayerBuilder __instance, GroupConstructible groupConstructible)
            {
                if (Events.Building.Trigger_BuildModeActivated(__instance, groupConstructible))
                    return false;

                return true;
            }

            [HarmonyPatch(typeof(PlayerBuilder), "InputOnCancelAction")]
            [HarmonyPrefix]
            public static bool PlayerBuilder_InputOnCancelAction_Prefix(PlayerBuilder __instance, ConstructibleGhost ___ghost, GroupConstructible ___ghostGroupConstructible)
            {
                if (Events.Building.Trigger_BuildModeDeactivated(__instance, ___ghost, ___ghostGroupConstructible, true))
                    return false;

                return true;
            }

            [HarmonyPatch(typeof(PlayerBuilder), "InputOnAction")]
            [HarmonyPrefix]
            public static bool PlayerBuilder_InputOnAction_Prefix(PlayerBuilder __instance, float ___timeCreatedGhost, float ___timeCantBuildInterval, ref ConstructibleGhost ___ghost, GroupConstructible ___ghostGroupConstructible)
            {
                if (___ghost != null)
                {
                    if (Events.Building.Trigger_BuildModeDeactivated(__instance, ___ghost, ___ghostGroupConstructible, false))
                        return false;

                    if (UnityEngine.Time.time < ___timeCreatedGhost + ___timeCantBuildInterval && !Managers.GetManager<PlayModeHandler>().GetIsFreePlay())
                        return false;

                    UnityEngine.GameObject gameObject = ___ghost.Place();

                    if (gameObject != null)
                    {
                        UnityEngine.Quaternion ghostRotation = new UnityEngine.Quaternion(___ghost.transform.rotation.x, ___ghost.transform.rotation.y, ___ghost.transform.rotation.z, ___ghost.transform.rotation.w);
                        ___ghost = null;

                        __instance.GetComponent<PlayerAudio>().PlayBuildGhost();
                        __instance.GetComponent<PlayerAnimations>().AnimateConstruct(true);
                        __instance.Invoke("StopAnimation", 0.5f);
                        UnityEngine.Material constructionMaterial = Managers.GetManager<VisualsResourcesHandler>().GetConstructionMaterial();
                        gameObject.AddComponent<GhostFx>().StartDisolveAnimation(-0.015f, 0.7f, constructionMaterial);
                        Inventory inventory = __instance.GetComponent<PlayerBackpack>().GetInventory();

                        if (inventory.ContainsItems(new List<Group>() { ___ghostGroupConstructible }))
                            inventory.RemoveItems(new List<Group>() { ___ghostGroupConstructible }, true, true);
                        else
                            inventory.RemoveItems(___ghostGroupConstructible.GetRecipe().GetIngredientsGroupInRecipe(), true, true);

                        Events.Building.Trigger_BuildingBuilt(__instance, ghostRotation, ___ghostGroupConstructible, gameObject);
                    }
                }

                return false;
            }
        }
    }
}
