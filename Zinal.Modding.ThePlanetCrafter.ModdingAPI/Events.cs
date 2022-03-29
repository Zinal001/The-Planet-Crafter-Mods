using SpaceCraft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zinal.Modding.ThePlanetCrafter.ModdingAPI
{
    public static class Events
    {
        public static class GameState
        {
            /// <summary>
            /// <para>Triggered whenever the game starts to load a save.</para>
            /// Note: Nothing has been loaded yet when this event is triggered.
            /// </summary>
            public static event EventHandler<GameStateEventArgs> GameLoading;

            /// <summary>
            /// <para>Triggered whenever the game has successfully loaded a save.</para>
            /// </summary>
            public static event EventHandler<GameStateEventArgs> GameLoaded;

            /// <summary>
            /// <para>Triggered whenever the game saves.</para>
            /// Note: Nothing has been saved yet when this event is triggered.
            /// </summary>
            public static event EventHandler<GameStateEventArgs> GameSaving;

            /// <summary>
            /// <para>Triggered whenever the game has successfully saved the game.</para>
            /// </summary>
            public static event EventHandler<GameStateEventArgs> GameSaved;

            internal static void Trigger_GameLoading(SavedDataHandler savedDataHandler, String filename) => GameLoading?.Invoke(savedDataHandler, new GameStateEventArgs(filename));
            internal static void Trigger_GameLoaded(SavedDataHandler savedDataHandler, String filename) => GameLoaded?.Invoke(savedDataHandler, new GameStateEventArgs(filename));

            internal static void Trigger_GameSaving(SavedDataHandler savedDataHandler, String filename) => GameSaving?.Invoke(savedDataHandler, new GameStateEventArgs(filename));
            internal static void Trigger_GameSaved(SavedDataHandler savedDataHandler, String filename) => GameSaved?.Invoke(savedDataHandler, new GameStateEventArgs(filename));


            public class GameStateEventArgs : EventArgs
            {
                public String SaveFileName { get; private set; }

                internal GameStateEventArgs(String saveFileName)
                {
                    SaveFileName = saveFileName;
                }
            }
        }

        public static class Weather
        {
            /// <summary>
            /// <para>Triggered when a weather (Meteo) has been added to the queue.</para>
            /// <para>Check the field <see cref="MeteoEventData.asteroidEventData"/> to see if this event will spawn asteroids.</para>
            /// <para>This event can be cancelled by setting Cancel to true.</para>
            /// </summary>
            public static event EventHandler<WeatherStateEventArgs> WeatherQueued;

            /// <summary>
            /// <para>Triggered when a weather (Meteo) is triggered.</para>
            /// <para>Check the field <see cref="MeteoEventData.asteroidEventData"/> to see if this event will spawn asteroids.</para>
            /// <para>This event can be cancelled by setting Cancel to true.</para>
            /// </summary>
            public static event EventHandler<WeatherStateEventArgs> WeatherChanged;

            internal static bool Trigger_WeatherQueued(MeteoHandler meteoHandler, MeteoEventData meteoEventData)
            {
                WeatherStateEventArgs args = new WeatherStateEventArgs(meteoEventData, true, false, meteoEventData.asteroidEventData != null);
                WeatherQueued?.Invoke(meteoHandler, args);
                return args.Cancel;
            }

            internal static bool Trigger_WeatherChanged(MeteoHandler meteoHandler, MeteoEventData meteoEventData)
            {
                WeatherStateEventArgs args = new WeatherStateEventArgs(meteoEventData, false, true, meteoEventData.asteroidEventData != null);
                WeatherChanged?.Invoke(meteoHandler, args);
                return args.Cancel;
            }

            public class WeatherStateEventArgs : CancelableEventArgs
            {
                public MeteoEventData Data { get; private set; }
                public bool Queued { get; private set; }
                public bool LaunchingNow { get; private set; }
                public bool HasAsteroids { get; private set; }

                internal WeatherStateEventArgs(MeteoEventData data, bool queued, bool launchingNow, bool hasAsteroids)
                {
                    Data = data;
                    Queued = queued;
                    LaunchingNow = launchingNow;
                    HasAsteroids = hasAsteroids;
                }
            }
        }

        public class CancelableEventArgs : EventArgs
        {
            public bool Cancel { get; set; } = false;

            internal CancelableEventArgs() { }
        }

        public static class Machines
        {
            /// <summary>
            /// Triggered when an object is generated in a Generator-type Machine (Ore Extractor, Atmospheric Water Collector, etc)
            /// <para>This event can be cancelled by setting Cancel to true.</para>
            /// </summary>
            public static event EventHandler<MachineGeneratorEventArgs> ObjectGenerated;


            internal static bool Trigger_ObjectGenerated(MachineGenerator machineGenerator, bool hasEnergy, Inventory inventory, WorldObject generatedObject)
            {
                MachineGeneratorEventArgs args = new MachineGeneratorEventArgs(inventory.IsFull(), hasEnergy, inventory, generatedObject);
                ObjectGenerated?.Invoke(machineGenerator, args);
                return args.Cancel;
            }

            public class MachineGeneratorEventArgs : CancelableEventArgs
            {
                public bool IsFull { get; private set; }
                public bool HasEnergy { get; private set; }
                public Inventory Inventory { get; private set; }
                public WorldObject GeneratedObject { get; private set; }

                internal MachineGeneratorEventArgs(bool isFull, bool hasEnergy, Inventory inventory, WorldObject generatedObject)
                {
                    IsFull = isFull;
                    HasEnergy = hasEnergy;
                    Inventory = inventory;
                    GeneratedObject = generatedObject;
                }
            }
        }

        public static class Building
        {
            /// <summary>
            /// Triggered when a player has selected a blueprint to build.
            /// <para>Check the field <see cref="BuildingEventArgs.GroupConstructible"/> to find information about what is being built.</para>
            /// <para>This event can be cancelled by setting Cancel to true.</para>
            /// </summary>
            public static event EventHandler<BuildingEventArgs> BuildModeActivated;

            /// <summary>
            /// Triggered when a player either cancels building a blueprint or just before an object is built.
            /// <para>This event can be cancelled by setting Cancel to true.</para>
            /// </summary>
            public static event EventHandler<BuildingDeactivatedEventArgs> BuildModeDeactivated;

            /// <summary>
            /// Triggered when a building is built.
            /// </summary>
            public static event EventHandler<BuildingBuiltEventArgs> BuildingBuilt;

            internal static bool Trigger_BuildModeActivated(PlayerBuilder builder, GroupConstructible groupConstructible)
            {
                BuildingEventArgs args = new BuildingEventArgs(groupConstructible, null);
                BuildModeActivated?.Invoke(builder, args);
                return args.Cancel;
            }

            internal static bool Trigger_BuildModeDeactivated(PlayerBuilder builder, ConstructibleGhost ghost, GroupConstructible ghostGroupConstructible, bool isBuildingCancelled)
            {
                BuildingDeactivatedEventArgs args = new BuildingDeactivatedEventArgs(ghostGroupConstructible, ghost, isBuildingCancelled);
                BuildModeDeactivated?.Invoke(builder, args);
                return args.Cancel;
            }

            internal static void Trigger_BuildingBuilt(PlayerBuilder builder, UnityEngine.Quaternion ghostRotation, GroupConstructible groupConstructible, UnityEngine.GameObject builtObject)
            {
                BuildingBuiltEventArgs args = new BuildingBuiltEventArgs(ghostRotation, groupConstructible, builtObject);
                BuildingBuilt?.Invoke(builder, args);
            }

            public class BuildingEventArgs : CancelableEventArgs
            {
                public GroupConstructible GroupConstructible { get; private set; }
                public ConstructibleGhost BuildingGhost { get; private set; }

                internal BuildingEventArgs(GroupConstructible groupConstructible, ConstructibleGhost ghost)
                {
                    GroupConstructible = groupConstructible;
                    BuildingGhost = ghost;
                }
            }

            public class BuildingDeactivatedEventArgs : BuildingEventArgs
            {
                public bool IsBuildingCancelled { get; private set; }

                internal BuildingDeactivatedEventArgs(GroupConstructible groupConstructible, ConstructibleGhost ghost, bool isBuildingCancelled) : base(groupConstructible, ghost)
                {
                    IsBuildingCancelled = isBuildingCancelled;
                }
            }

            public class BuildingBuiltEventArgs : EventArgs
            {
                public UnityEngine.Quaternion GhostRotation { get; private set; }
                public GroupConstructible GroupConstructible { get; private set; }
                public UnityEngine.GameObject BuiltObject { get; private set; }

                internal BuildingBuiltEventArgs(UnityEngine.Quaternion ghostRotation, GroupConstructible groupConstructible, UnityEngine.GameObject builtObject)
                {
                    GhostRotation = ghostRotation;
                    GroupConstructible = groupConstructible;
                    BuiltObject = builtObject;
                }
            }
        }
    }
}
