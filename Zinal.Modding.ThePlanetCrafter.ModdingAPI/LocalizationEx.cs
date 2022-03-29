using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zinal.Modding.ThePlanetCrafter.ModdingAPI
{
    /// <summary>
    /// <para>An extended version of MijuTools localization class.</para>
    /// See <see cref="MijuTools.Localization"/> for the default localization class.
    /// </summary>
    public static class LocalizationEx
    {
        private static readonly System.Reflection.FieldInfo localizationDictionaryField;
        private static readonly System.Reflection.FieldInfo hasLoadedSuccesfullyField;
        private static readonly System.Reflection.FieldInfo currentLangageField;
        private static readonly System.Reflection.FieldInfo availableLanguagesField;

        /// <summary>
        /// Is this localization in-game loaded?
        /// </summary>
        public static bool LocalizationLoaded { get; internal set; } = false;

        /// <summary>
        /// This event is triggered when the localization in game is loaded. Do not call any methods in this class before this is called at least once!
        /// </summary>
        public static event EventHandler OnLocalizationLoaded;

        static LocalizationEx()
        {
            localizationDictionaryField = typeof(MijuTools.Localization).GetField("localizationDictionary", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            hasLoadedSuccesfullyField = typeof(MijuTools.Localization).GetField("hasLoadedSuccesfully", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            currentLangageField = typeof(MijuTools.Localization).GetField("currentLangage", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            availableLanguagesField = typeof(MijuTools.Localization).GetField("availableLanguages", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);


            if ((bool)hasLoadedSuccesfullyField.GetValue(null) && !LocalizationLoaded)
            {
                LocalizationLoaded = true;
                TriggerLoaded();
            }
        }

        internal static void TriggerLoaded()
        {
            OnLocalizationLoaded?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Get the current language name (Example: english, which is default for the game).
        /// </summary>
        /// <returns></returns>
        public static String GetCurrentLanguage()
        {
            return currentLangageField.GetValue(null) as String;
        }

        /// <summary>
        /// Add a new language to the game.
        /// </summary>
        /// <param name="languageName">The name of the language. (Example: english)</param>
        /// <returns></returns>
        public static bool AddLanguage(String languageName, String localizedName = null)
        {
            if (!LocalizationLoaded || string.IsNullOrEmpty(languageName))
            {
                Plugin.PluginLogger.LogWarning("Tried to add language before localization was loaded.");
                return false;
            }

            if (string.IsNullOrEmpty(localizedName))
                localizedName = languageName;

            Dictionary<string, Dictionary<string, string>> localizationDictionary = (Dictionary<string, Dictionary<string, string>>)localizationDictionaryField.GetValue(null);
            if (!localizationDictionary.ContainsKey(languageName))
            {
                _AddLanguage(ref localizationDictionary, languageName, localizedName);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Add a new localized string to a language.
        /// </summary>
        /// <param name="languageName">The name of the language. (Example: english)</param>
        /// <param name="stringCode">The name/key.</param>
        /// <param name="text">The localized string.</param>
        /// <returns></returns>
        public static bool AddLocalizedText(String languageName, String stringCode, String text)
        {
            if (!LocalizationLoaded || string.IsNullOrEmpty(languageName))
            {
                Plugin.PluginLogger.LogWarning("Tried to add localized text before localization was loaded.");
                return false;
            }

            Dictionary<string, Dictionary<string, string>> localizationDictionary = (Dictionary<string, Dictionary<string, string>>)localizationDictionaryField.GetValue(null);
            if (!localizationDictionary.ContainsKey(languageName))
                _AddLanguage(ref localizationDictionary, languageName, languageName);

            if (!localizationDictionary[languageName].ContainsKey(stringCode))
            {
                localizationDictionary[languageName][stringCode] = text;
                localizationDictionaryField.SetValue(null, localizationDictionary);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Add a new localized string to the current language.
        /// </summary>
        /// <param name="stringCode">The name/key.</param>
        /// <param name="text">The localized string.</param>
        /// <returns></returns>
        public static bool AddLocalizedText(String stringCode, String text) => AddLocalizedText(GetCurrentLanguage(), stringCode, text);

        /// <summary>
        /// Adds or replaces a localized string to a language.
        /// </summary>
        /// <param name="languageName">The name of the language. (Example: english)</param>
        /// <param name="stringCode">The name/key.</param>
        /// <param name="text">The localized string.</param>
        /// <returns></returns>
        public static bool SetLocalizedText(String languageName, String stringCode, String text)
        {
            if (!LocalizationLoaded || string.IsNullOrEmpty(languageName))
            {
                Plugin.PluginLogger.LogWarning("Tried to set localized text before localization was loaded.");
                return false;
            }

            Dictionary<string, Dictionary<string, string>> localizationDictionary = (Dictionary<string, Dictionary<string, string>>)localizationDictionaryField.GetValue(null);
            if (!localizationDictionary.ContainsKey(languageName))
                _AddLanguage(ref localizationDictionary, languageName, languageName);

            localizationDictionary[languageName][stringCode] = text;
            localizationDictionaryField.SetValue(null, localizationDictionary);

            return true;
        }

        /// <summary>
        /// Adds or replaces a localized string to the current language.
        /// </summary>
        /// <param name="stringCode">The name/key.</param>
        /// <param name="text">The localized string.</param>
        /// <returns></returns>
        public static bool SetLocalizedText(String stringCode, String text) => SetLocalizedText(GetCurrentLanguage(), stringCode, text);



        private static void _AddLanguage(ref Dictionary<string, Dictionary<string, string>> localizationDictionary, String languageName, String localizedName)
        {
            if (!localizationDictionary.ContainsKey(languageName))
            {
                localizationDictionary[languageName] = new Dictionary<string, string>
                {
                    ["LANGUAGE"] = localizedName
                };
                localizationDictionaryField.SetValue(null, localizationDictionary);

                List<string> availableLanguages = (List<string>)availableLanguagesField.GetValue(null);
                if (!availableLanguages.Contains(languageName))
                {
                    availableLanguages.Add(languageName);
                    availableLanguagesField.SetValue(null, availableLanguages);
                }
            }
        }
    }
}
