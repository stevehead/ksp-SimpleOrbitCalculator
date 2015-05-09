using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleOrbitCalculator
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class SimpleOrbitCalculatorController : MonoBehaviour
    {
        /// <summary>
        /// The plugin's name.
        /// </summary>
        public const string PluginName = "Simple Orbit Calculator";

        /// <summary>
        /// The plugin's directory under GameData.
        /// </summary>
        public const string PluginDirectoryName = "SimpleOrbitCalculator";

        /// <summary>
        /// The stock toolbar icon.
        /// </summary>
        private const string PluginIconButtonStock = "icon_button_stock";

        /// <summary>
        /// The Blizzy's toolbar icon.
        /// </summary>
        private const string PluginIconButtonBlizzy = "icon_button_blizzy";

        /// <summary>
        /// Tells plugin to use Blizzy's toolbar if it is available. Will change to an option.
        /// </summary>
        private const bool UseBlizzyToolbar = true;

        /// <summary>
        /// The application launcher button that is created.
        /// </summary>
        private static ApplicationLauncherButton appLauncherButton = null;

        /// <summary>
        /// The Blizzy's Toolbar button that is created.
        /// </summary>
        private static IButton blizzyToolbarButton = null;

        /// <summary>
        /// The calculator object that is created.
        /// </summary>
        private static GameObject calculator;

        /// <summary>
        /// Is the window open? Really only used for Blizzy's toolbar.
        /// </summary>
        private static bool isWindowOpen = false;

        /// <summary>
        /// The scenes the button will be visible in.
        /// </summary>
        private static ApplicationLauncher.AppScenes ScenesApplicationVisibleIn
        {
            get
            {
                return ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW | ApplicationLauncher.AppScenes.SPACECENTER
                        | ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.TRACKSTATION | ApplicationLauncher.AppScenes.VAB;
            }
        }
        
        /// <summary>
        /// Called when the script is loaded.
        /// </summary>
        public void Awake()
        {
            if (UseBlizzyToolbar && ToolbarManager.ToolbarAvailable)
            {
                blizzyToolbarButton = ToolbarManager.Instance.add("SimpleOrbitCalculator", "mainWindow");
                blizzyToolbarButton.Visibility = new GameScenesVisibility(GameScenes.EDITOR, GameScenes.SPACECENTER, GameScenes.TRACKSTATION, GameScenes.FLIGHT);
                blizzyToolbarButton.TexturePath = String.Format("{0}/Textures/{1}", PluginDirectoryName, PluginIconButtonBlizzy);
                blizzyToolbarButton.ToolTip = PluginName;
                blizzyToolbarButton.OnClick += (e) =>
                {
                    if (isWindowOpen) OnAppLaunchToggleOff();
                    else OnAppLaunchToggleOn();
                };
            }
            else
            {
                GameEvents.onGUIApplicationLauncherReady.Add(AddAppLauncherButton);
                GameEvents.onGUIApplicationLauncherDestroyed.Add(RemoveAppLauncherButton);
            }
        }

        /// <summary>
        /// Called to add the application to the stock toolbar.
        /// </summary>
        private void AddAppLauncherButton()
        {
            if (ApplicationLauncher.Ready && appLauncherButton == null)
            {
                string pluginIconButtonStockPath = String.Format("{0}/Textures/{1}", PluginDirectoryName, PluginIconButtonStock);
                appLauncherButton = ApplicationLauncher.Instance.AddModApplication(
                    OnAppLaunchToggleOn, OnAppLaunchToggleOff,
                    null, null,
                    null, null,
                    ScenesApplicationVisibleIn,
                    (Texture)GameDatabase.Instance.GetTexture(pluginIconButtonStockPath, false));
            }
        }

        /// <summary>
        /// Removes the application from the stock toolbar.
        /// </summary>
        private void RemoveAppLauncherButton()
        {
            ApplicationLauncher.Instance.RemoveApplication(appLauncherButton);
        }

        /// <summary>
        /// Sets the window as open. Should only be called by a button.
        /// </summary>
        private void OnAppLaunchToggleOn()
        {
            calculator = new GameObject("SOCWindow", typeof(CalculatorWindow));
            isWindowOpen = true;
        }

        /// <summary>
        /// Sets the windows as closed. Should only be called by a button.
        /// </summary>
        private void OnAppLaunchToggleOff()
        {
            Destroy(calculator);
            isWindowOpen = false;
        }

        /// <summary>
        /// Allows outside objects to turn the toolbar icon off.
        /// </summary>
        public static void SetApplauncherButtonFalse()
        {
            if (appLauncherButton != null)
            {
                appLauncherButton.SetFalse();
            }
            isWindowOpen = false;
        }
    }
}
