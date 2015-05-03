using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleOrbitCalculator
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class OrbitCalculatorGUIController : UnityEngine.MonoBehaviour
    {
        private const string PluginName = "Simple Orbit Calculator";
        private const string PluginDirectoryName = "SimpleOrbitCalculator";
        private const string PluginIconButtonStock = "icon_button_stock";

        private static bool windowOpen = false;
        private bool isActivated = false;
        private Rect windowPos = new Rect(100, 100, 800, 465);

        private static ApplicationLauncherButton appLauncherButton = null;

        void OnAppLaunchToggleOn()
        {
            if (appLauncherButton == null)
            {
                Debug.LogError(PluginName + " :: OnAppLaunchToggleOn called without a button.");
                return;
            }

            windowOpen = true;
        }

        void OnAppLaunchToggleOff()
        {
            if (appLauncherButton == null)
            {
                Debug.LogError(PluginName + " :: OnAppLaunchToggleOff called without a button.");
                return;
            }
            windowOpen = false;
        }

        void RemoveFromAppLauncher()
        {
#if DEBUG
            Debug.Log(PluginName + " :: RemoveFromAppLauncher called.");
#endif
            if (appLauncherButton != null)
            {
                ApplicationLauncher.Instance.RemoveApplication(appLauncherButton);
                GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
                GameEvents.onGUIApplicationLauncherReady.Remove(OnGuiAppLauncherDestroyed);
                GameEvents.onGameSceneLoadRequested.Remove(OnGameSceneLoadRequestedForAppLauncher);
#if DEBUG
                Debug.Log(PluginName + " :: RemoveFromAppLauncher removal logic completed.");
#endif
            }
        }

        void OnGUIAppLauncherReady()
        {
#if DEBUG
            Debug.Log(PluginName + " :: OnGuiAppLauncherReady called.");
#endif
            if (ApplicationLauncher.Ready && appLauncherButton == null)
            {
                string pluginIconButtonStockPath = String.Format("{0}/Textures/{1}", PluginDirectoryName, PluginIconButtonStock);
                appLauncherButton = ApplicationLauncher.Instance.AddModApplication(
                    OnAppLaunchToggleOn, OnAppLaunchToggleOff,
                    null, null,
                    null, null,
                    ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW | ApplicationLauncher.AppScenes.SPACECENTER
                        | ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.TRACKSTATION | ApplicationLauncher.AppScenes.VAB,
                    (Texture)GameDatabase.Instance.GetTexture(pluginIconButtonStockPath, false));
#if DEBUG
                Debug.Log(PluginName + " :: AppLauncher menu item added.");
#endif
            }
        }

        void OnGuiAppLauncherDestroyed()
        {
#if DEBUG
            Debug.Log(PluginName + " :: OnGuiAppLauncherDestroyed called.");
#endif
            if (appLauncherButton != null)
            {
                RemoveFromAppLauncher();
            }
        }

        void OnGameSceneLoadRequestedForAppLauncher(GameScenes sceneToLoad)
        {
#if DEBUG
            Debug.Log(PluginName + " :: OnGameSceneLoadRequestedForAppLauncher called.");
#endif
            if (appLauncherButton != null)
            {
                RemoveFromAppLauncher();
            }
        }

        /// <summary>
        /// Determines if the current game scane is valid for the plugin.
        /// This plugin should be able to run in VAB/SPH, Flight, Space Center, and Tracking Station scenes.
        /// </summary>
        /// <returns>True if valid; false if not valid.</returns>
        Boolean IsValidScene()
        {
            return HighLogic.LoadedSceneIsEditor || HighLogic.LoadedSceneIsFlight || HighLogic.LoadedScene == GameScenes.SPACECENTER || HighLogic.LoadedScene == GameScenes.TRACKSTATION;
        }

        public void Awake()
        {
#if DEBUG
            Debug.Log(PluginName + " :: is now awake.");
#endif
            if (appLauncherButton == null)
            {
#if DEBUG
                Debug.Log(PluginName + " :: AppLauncher setup.");
#endif
                GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
                GameEvents.onGUIApplicationLauncherDestroyed.Add(OnGuiAppLauncherDestroyed);
                GameEvents.onGameSceneLoadRequested.Remove(OnGameSceneLoadRequestedForAppLauncher);
            }

            if (IsValidScene())
            {
                RenderingManager.AddToPostDrawQueue(3, new Callback(DrawGUI));
            }
        }

        public void DrawGUI()
        {
            if (IsValidScene())
            {
                if (windowOpen)
                {
                    windowPos = GUILayout.Window(PluginName.GetHashCode(), windowPos, MainWindow, PluginName, GUILayout.Width(800));
                }
                isActivated = windowOpen;
            }
        }

        private void MainWindow(int windowID)
        {
            CalculatorWindow.Instance.Render();
        }
    }
}
