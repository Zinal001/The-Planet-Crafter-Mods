using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zinal.Modding.ThePlanetCrafter.ModdingAPI
{
    public static class Configuration
    {
        private static ConfigFile _Config;
        public static bool IsConfigurationLoaded { get; private set; } = false;

        public static ConfigEntry<bool> PatchGameStateEvents { get; private set; }
        public static ConfigEntry<bool> PatchWeatherEvents { get; private set; }
        public static ConfigEntry<bool> PatchMachinesEvents { get; private set; }
        public static ConfigEntry<bool> PatchBuildingEvents { get; private set; }

        public static event EventHandler ConfigurationLoaded;

        internal static void Init(Plugin plugin)
        {
            _Config = plugin.Config;

            PatchGameStateEvents = _Config.Bind("General", "Patch GameState Events", true, "Should the API patch methods related to the gamestate?");
            PatchWeatherEvents = _Config.Bind("General", "Patch Weather Events", true, "Should the API patch methods related to weather?");
            PatchMachinesEvents = _Config.Bind("General", "Patch Machines Events", true, "Should the API patch methods related to machines?");
            PatchBuildingEvents = _Config.Bind("General", "Patch Building Events", true, "Should the API patch methods related to building and buildings?");
            IsConfigurationLoaded = true;

            ConfigurationLoaded?.Invoke(null, EventArgs.Empty);
        }
    
        

    }
}
