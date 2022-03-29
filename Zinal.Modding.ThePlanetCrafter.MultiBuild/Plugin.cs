using BepInEx;
using System.Collections.Generic;

namespace Zinal.Modding.ThePlanetCrafter.MultiBuild
{
    [BepInPlugin("Zinal.Modding.ThePlanetCrafter.MultiBuild", "MultiBuild", "1.0.1")]
    [BepInDependency("Zinal.Modding.ThePlanetCrafter.ModdingAPI", BepInDependency.DependencyFlags.HardDependency)]
    [BepInProcess("Planet Crafter.exe")]
    public class Plugin : BaseUnityPlugin
    {
        private static readonly System.Reflection.FieldInfo ghostField = typeof(SpaceCraft.PlayerBuilder).GetField("ghost", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        private void Awake()
        {
            ModdingAPI.Events.Building.BuildingBuilt += Building_BuildingBuilt;
        }

        private void Building_BuildingBuilt(object sender, ModdingAPI.Events.Building.BuildingBuiltEventArgs e)
        {
            SpaceCraft.PlayerBuilder playerBuilder = sender as SpaceCraft.PlayerBuilder;

            if(UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.LeftShift].isPressed || UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.RightShift].isPressed)
            {
                List<SpaceCraft.Group> ingredients = e.GroupConstructible.GetRecipe().GetIngredientsGroupInRecipe();
                if(MijuTools.Managers.GetManager<SpaceCraft.PlayersManager>().GetActivePlayerController().GetPlayerBackpack().GetInventory().ContainsItems(ingredients) || MijuTools.Managers.GetManager<SpaceCraft.PlayModeHandler>().GetFreeCraft())
                {
                    playerBuilder.SetNewGhost(e.GroupConstructible);

                    SpaceCraft.ConstructibleGhost newGhost = (SpaceCraft.ConstructibleGhost)ghostField.GetValue(playerBuilder);
                    if (newGhost != null)
                        newGhost.gameObject.transform.rotation = e.GhostRotation;
                }
            }
        }
    }
}
