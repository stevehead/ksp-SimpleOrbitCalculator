using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleOrbitCalculator
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class OrbitCalculatorGUIController : MonoBehaviour
    {
        /// <summary>
        /// The plugin's name.
        /// </summary>
        private const string PluginName = "Simple Orbit Calculator";

        /// <summary>
        /// The plugin's directory under GameData.
        /// </summary>
        private const string PluginDirectoryName = "SimpleOrbitCalculator";

        /// <summary>
        /// The stock toolbar's icon.
        /// </summary>
        private const string PluginIconButtonStock = "icon_button_stock";

        /// <summary>
        /// Is calculator open?
        /// </summary>
        private static bool windowOpen = false;

        /// <summary>
        /// Location of the calculator's window.
        /// </summary>
        private Rect windowPos = new Rect(100, 100, 800, 465);

        /// <summary>
        /// The application launcher button that is created.
        /// </summary>
        private static ApplicationLauncherButton appLauncherButton = null;

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
            // If the application has not been registered yet.
            if (appLauncherButton == null)
            {
                GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
                GameEvents.onGUIApplicationLauncherDestroyed.Add(OnGuiAppLauncherDestroyed);
                GameEvents.onGameSceneLoadRequested.Add(OnGameSceneLoadRequestedForAppLauncher);
            }

            // If this is a valid scene, add the GUI callback.
            if (IsValidScene())
            {
                RenderingManager.AddToPostDrawQueue(3, new Callback(DrawGUI));
            }
        }

        /// <summary>
        /// Sets the window as open. Should only be called by a button.
        /// </summary>
        private void OnAppLaunchToggleOn()
        {
            if (appLauncherButton == null)
            {
                Debug.LogError(PluginName + " :: OnAppLaunchToggleOn called without a button.");
            }

            windowOpen = true;
        }

        /// <summary>
        /// Sets the windows as closed. Should only be called by a button.
        /// </summary>
        private void OnAppLaunchToggleOff()
        {
            if (appLauncherButton == null)
            {
                Debug.LogError(PluginName + " :: OnAppLaunchToggleOff called without a button.");
            }

            windowOpen = false;
        }

        /// <summary>
        /// Removes the application from launcher.
        /// </summary>
        private void RemoveFromAppLauncher()
        {
            if (appLauncherButton != null)
            {
                ApplicationLauncher.Instance.RemoveApplication(appLauncherButton);
            }
            GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
            GameEvents.onGUIApplicationLauncherReady.Remove(OnGuiAppLauncherDestroyed);
            GameEvents.onGameSceneLoadRequested.Remove(OnGameSceneLoadRequestedForAppLauncher);
        }

        /// <summary>
        /// Called when the AppLauncher is ready to receive this application.
        /// </summary>
        private void OnGUIAppLauncherReady()
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
        /// Called when the App Launcher toolbar is destroyed.
        /// </summary>
        private void OnGuiAppLauncherDestroyed()
        {
            RemoveFromAppLauncher();
        }

        /// <summary>
        /// Called when the game scene changes.
        /// </summary>
        /// <param name="sceneToLoad">The new game scene.</param>
        private void OnGameSceneLoadRequestedForAppLauncher(GameScenes sceneToLoad)
        {
            RemoveFromAppLauncher();
        }

        /// <summary>
        /// Handles the GUI rendering.
        /// </summary>
        private void DrawGUI()
        {
            if (IsValidScene() && windowOpen)
            {
                if (windowOpen)
                {
                    windowPos = GUILayout.Window(PluginName.GetHashCode(), windowPos, MainWindow, PluginName, GUILayout.Width(800));
                }
            }
        }

        /// <summary>
        /// The rendering of the calculator.
        /// </summary>
        /// <param name="windowID">Window ID</param>
        private void MainWindow(int windowID)
        {
            CalculatorWindow.Instance.Render();
        }

        /// <summary>
        /// Determines if the current game scane is valid for the plugin.
        /// This plugin should be able to run in VAB/SPH, Flight, Space Center, and Tracking Station scenes.
        /// </summary>
        /// <returns>True if valid; false if not valid.</returns>
        private static bool IsValidScene()
        {
            return HighLogic.LoadedSceneIsEditor || HighLogic.LoadedSceneIsFlight || HighLogic.LoadedScene == GameScenes.SPACECENTER
                || HighLogic.LoadedScene == GameScenes.TRACKSTATION;
        }
    }
}
