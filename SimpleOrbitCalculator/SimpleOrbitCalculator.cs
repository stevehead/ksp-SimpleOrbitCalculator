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

        private List<CelestialBody> celestialBodies;
        private string[] celestialSelectValues;
        private int selectedCelestialIndex = 0;

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
            if (appLauncherButton != null)
            {
                ApplicationLauncher.Instance.RemoveApplication(appLauncherButton);
                appLauncherButton = null;
                GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
                GameEvents.onGUIApplicationLauncherReady.Remove(OnGuiAppLauncherDestroyed);
                GameEvents.onGameSceneLoadRequested.Remove(OnGameSceneLoadRequestedForAppLauncher);
            }
        }

        void OnGUIAppLauncherReady()
        {
            if (ApplicationLauncher.Ready && appLauncherButton == null)
            {
                string pluginIconButtonStockPath = String.Format("{0}/Textures/{1}", PluginDirectoryName, PluginIconButtonStock);
                appLauncherButton = ApplicationLauncher.Instance.AddModApplication(
                    OnAppLaunchToggleOn, OnAppLaunchToggleOff,
                    null, null,
                    null, null,
                    ApplicationLauncher.AppScenes.ALWAYS,
                    (Texture)GameDatabase.Instance.GetTexture(pluginIconButtonStockPath, false));
            }
        }

        void OnGuiAppLauncherDestroyed()
        {
            if (appLauncherButton != null)
            {
                RemoveFromAppLauncher();
            }
        }

        void OnGameSceneLoadRequestedForAppLauncher(GameScenes sceneToLoad)
        {
            if (appLauncherButton != null)
            {
                RemoveFromAppLauncher();
            }
        }

        Boolean IsValidScene()
        {
            return HighLogic.LoadedSceneIsEditor || HighLogic.LoadedSceneIsFlight || HighLogic.LoadedScene == GameScenes.SPACECENTER || HighLogic.LoadedScene == GameScenes.TRACKSTATION;
        }

        public void Awake()
        {
            if (appLauncherButton == null)
            {
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
                    if (!isActivated)
                    {
                        LoadAllCelestialInformation();
                    }
                    windowPos = GUILayout.Window(PluginName.GetHashCode(), windowPos, MainWindow, PluginName,
                        GUILayout.Width(800), GUILayout.Height(465), GUILayout.ExpandWidth(false));
                }
                isActivated = windowOpen;
            }
        }

        private void MainWindow(int windowID)
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Label("Select a Body:");
            selectedCelestialIndex = GUILayout.SelectionGrid(selectedCelestialIndex, celestialSelectValues, 1);
            GUILayout.BeginVertical();

            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        /// <summary>
        /// Loads all the information for the celestial bodies.
        /// </summary>
        private void LoadAllCelestialInformation()
        {
            LoadKnownCelestials();
            LoadCelestialSelectValues();
        }

        /// <summary>
        /// Grabs all the celestials known to KSP and orders them based on reference body and SMA.
        /// </summary>
        private void LoadKnownCelestials()
        {
            celestialBodies = new List<CelestialBody>();

            // The Sun is the only body that has a reference body of itself.
            CelestialBody theSun = FlightGlobals.Bodies.Where(o => o.referenceBody == o).First();

            // Planets are all bodies with the Sun as its reference body, except for the Sun itself.
            List<CelestialBody> thePlanets = FlightGlobals.Bodies.Where(o => o.referenceBody == theSun && o != theSun)
                .OrderBy(o => o.orbit.semiMajorAxis).ToList();

            // Need to add the Sun first.
            celestialBodies.Add(theSun);

            // Loop through each planet.
            foreach (CelestialBody planet in thePlanets)
            {
                // Add the planet.
                celestialBodies.Add(planet);

                // Gather all the moons of this planet and add to the list.
                celestialBodies.AddRange(FlightGlobals.Bodies.Where(o => o.referenceBody == planet)
                    .OrderBy(o => o.orbit.semiMajorAxis).ToList());
            }
        }

        /// <summary>
        /// Creates the array of strings that represent the select values.
        /// </summary>
        private void LoadCelestialSelectValues()
        {
            celestialSelectValues = new string[celestialBodies.Count];
            int initialSelectedIndex = -1;

            for (int i = 0; i < celestialBodies.Count; i++)
            {
                // Add name to array.
                celestialSelectValues[i] = celestialBodies[i].name;

                // If the body is the same as the current vessel's main body, set this as the initial selected index.
                /*if (HighLogic.LoadedSceneIsFlight && celestialBodies[i] == FlightGlobals.ActiveVessel.mainBody)
                {
                    initialSelectedIndex = i;
                }
                // Else set the initial index to Kerbin.
                else if (initialSelectedIndex < 0 && celestialBodies[i].name.Equals("Kerbin"))
                {
                    initialSelectedIndex = i;
                }*/
            }

            // If an initial selected index was found, set the selected index to it.
            if (initialSelectedIndex >= 0)
            {
                selectedCelestialIndex = initialSelectedIndex;
            }
        }
    }
}
