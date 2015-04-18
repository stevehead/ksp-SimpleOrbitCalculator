using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleOrbitCalculator
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class OrbitCalculatorGUIController : UnityEngine.MonoBehaviour
    {
        private const string PluginName = "Simple Orbit Calculator";
        private const string PluginDirectoryName = "SimpleOrbitCalculator";
        private const string PluginIconButtonStock = "icon_button_stock";

        private bool windowOpen = false;
        private Rect windowPos = new Rect(100, 100, 800, 465);

        private List<CelestialBody> celestialBodies;
        private string[] celestialSelectValues;
        private int selectedCelestialIndex = 0;

        private ApplicationLauncherButton SOCButtonStock = null;

        public void Awake()
        {
            LoadCelestials();
            LoadCelestialSelectValues();

            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
        }

        void OnGUIAppLauncherReady()
        {
            if (ApplicationLauncher.Ready)
            {
                string pluginIconButtonStockPath = String.Format("{0}/Textures/{1}", PluginDirectoryName, PluginIconButtonStock);
                SOCButtonStock = ApplicationLauncher.Instance.AddModApplication(
                    OnAppLaunchToggleOn, OnAppLaunchToggleOff,
                    DummyVoid, DummyVoid,
                    DummyVoid, DummyVoid,
                    ApplicationLauncher.AppScenes.SPACECENTER,
                    (Texture)GameDatabase.Instance.GetTexture(pluginIconButtonStockPath, false));
            }
        }

        void OnAppLaunchToggleOn()
        {
            windowOpen = true;
        }

        void OnAppLaunchToggleOff()
        {
            windowOpen = false;
        }

        void DummyVoid() { }

        public void Start()
        {
            RenderingManager.AddToPostDrawQueue(0, new Callback(DrawGUI));
            print(PluginName + "GUI Loaded");
        }

        public void DrawGUI()
        {
            if (windowOpen)
            {
                windowPos = GUILayout.Window(PluginName.GetHashCode(), windowPos, MainWindow, PluginName,
                    GUILayout.Width(800), GUILayout.Height(465), GUILayout.ExpandWidth(false));
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

        private void OnDestroy()
        {
            GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
            if (SOCButtonStock != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(SOCButtonStock);
            }
        }

        /// <summary>
        /// Grabs all the celestials known to KSP and orders them based on reference body and SMA.
        /// </summary>
        private void LoadCelestials()
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
                if (HighLogic.LoadedSceneIsFlight && celestialBodies[i] == FlightGlobals.ActiveVessel.mainBody)
                {
                    initialSelectedIndex = i;
                }
                // Else set the initial index to Kerbin.
                else if (initialSelectedIndex < 0 && celestialBodies[i].name.Equals("Kerbin"))
                {
                    initialSelectedIndex = i;
                }
            }

            // If an initial selected index was found, set the selected index to it.
            if (initialSelectedIndex >= 0)
            {
                selectedCelestialIndex = initialSelectedIndex;
            }
        }
    }
}
