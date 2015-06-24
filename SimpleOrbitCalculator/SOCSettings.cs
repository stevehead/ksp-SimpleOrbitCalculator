using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SimpleOrbitCalculator
{
    internal class SOCSettings
    {
        /// <summary>
        /// Config node name for settings.
        /// </summary>
        private const string ConfigNodeName = "SIMPLEORBITCALCULATOR_SETTINGS";

        /// <summary>
        /// Name of settings file.
        /// </summary>
        private const string LocalSettingsFile = "SOC_Settings.cfg";

        /// <summary>
        /// The plugin's PluginData directory.
        /// </summary>
        private static string LocalPluginDataPath
        {
            get
            {
                return String.Format("GameData/{0}/Plugins/PluginData", SimpleOrbitCalculatorController.PluginDirectoryName);
            }
        }

        /// <summary>
        /// The local path to the settings file.
        /// </summary>
        private static string LocalSettingsPath
        {
            get { return Path.Combine(LocalPluginDataPath, LocalSettingsFile); }
        }

        /// <summary>
        /// The absolute path to the settings file.
        /// </summary>
        private static string AbsoluteSettingsPath
        {
            get { return Path.Combine(KSPUtil.ApplicationRootPath, LocalSettingsPath); }
        }

        // Default values for settings
        private bool forceStockToolbar = false;

        /// <summary>
        /// Determines if the stock toolbar is to be used if Blizzy's Toolbar is installed.
        /// </summary>
        public bool ForceStockToolbar
        {
            get { return forceStockToolbar; }
        }

        /// <summary>
        /// Singleton instance.
        /// </summary>
        private static SOCSettings instance;

        /// <summary>
        /// Settings instance.
        /// </summary>
        public static SOCSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SOCSettings();
                }
                return instance;
            }
        }

        /// <summary>
        /// Constructor loads settings from file and creates one if it does not exist or invalid.
        /// </summary>
        private SOCSettings()
        {
            if (File.Exists(AbsoluteSettingsPath))
            {
                if (!LoadSettings())
                {
                    SaveSettings();
                }
            }
        }

        /// <summary>
        /// Loads the settings from file.
        /// </summary>
        /// <returns></returns>
        private bool LoadSettings()
        {
            bool validSettings = true;
            ConfigNode settings = ConfigNode.Load(AbsoluteSettingsPath);
            if (!settings.HasNode(ConfigNodeName)) { return false; }
            ConfigNode node = settings.GetNode(ConfigNodeName);
            validSettings &= LoadSetting(node, "forceStockToolbar", ref forceStockToolbar);
            return validSettings;
        }

        /// <summary>
        /// Loads a boolean-type setting.
        /// </summary>
        /// <param name="node">The config node.</param>
        /// <param name="name">The setting name.</param>
        /// <param name="value">The setting value to be set.</param>
        /// <returns></returns>
        private bool LoadSetting(ConfigNode node, string name, ref bool value)
        {
            bool result = false;
            if (node.HasValue(name) && bool.TryParse(node.GetValue(name), out result))
            {
                value = result;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Saves the settings to a file.
        /// </summary>
        private void SaveSettings()
        {
            // Create directory if needed.
            string settingsDirectory = Path.GetDirectoryName(AbsoluteSettingsPath);
            if (!Directory.Exists(settingsDirectory))
            {
                Directory.CreateDirectory(settingsDirectory);
            }
            ConfigNode settings = new ConfigNode();
            ConfigNode node = new ConfigNode(ConfigNodeName);
            node.AddValue("forceStockToolbar", forceStockToolbar);
            settings.AddNode(node);
            settings.Save(AbsoluteSettingsPath);
        }
    }
}
