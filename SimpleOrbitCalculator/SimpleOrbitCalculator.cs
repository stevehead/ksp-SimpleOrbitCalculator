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

        private bool lockPeriapsis = false;
        private bool lockApoapsis = false;
        private bool lockEccentricity = false;
        private bool lockSMA = false;
        private bool lockPeriod = false;
        private bool useAltitideAspides = true;

        private string periapsisText = "";
        private string apoapsisText = "";
        private string eccentricityText = "";
        private string smaText = "";
        private string periodText = "";
        private string errorText = "";

        private SimpleOrbit currentOrbit = null;

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
                    windowPos = GUILayout.Window(PluginName.GetHashCode(), windowPos, MainWindow, PluginName, GUILayout.Width(600));
                }
                isActivated = windowOpen;
            }
        }

        private void MainWindow(int windowID)
        {
            int countLocks = 0;
            bool lockLimitReached = false;
            if (lockPeriapsis) countLocks++;
            if (lockApoapsis) countLocks++;
            if (lockEccentricity) countLocks++;
            if (lockSMA) countLocks++;
            if (lockPeriod) countLocks++;

            if (countLocks >= 2) lockLimitReached = true;

            // Window Settings
            int celestialSelectColumns = (int)Math.Ceiling(celestialBodies.Count / 20.0);
            float minCelestialSelectWidth = 100f * celestialSelectColumns;
            float lockWidth = 150f;
            float inputWidth = 150f;
            float synchPeriodButtonWidth = 25f;
            float calculateButtonWidth = 100f;

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.MinWidth(minCelestialSelectWidth), GUILayout.ExpandWidth(true));
            GUILayout.Label("Select a Body:");
            selectedCelestialIndex = GUILayout.SelectionGrid(selectedCelestialIndex, celestialSelectValues, celestialSelectColumns);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            GUILayout.BeginHorizontal();
            GUILayout.Label("Current Body: " + celestialSelectValues[selectedCelestialIndex]);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (!lockPeriapsis && lockLimitReached) GUI.enabled = false;
            lockPeriapsis = GUILayout.Toggle(lockPeriapsis, "Periapsis (m)", GUILayout.Width(lockWidth), GUILayout.ExpandWidth(false));
            if (!lockPeriapsis) GUI.enabled = false;
            periapsisText = GUILayout.TextField(periapsisText, GUILayout.Width(inputWidth), GUILayout.ExpandWidth(false));
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (!lockApoapsis && lockLimitReached) GUI.enabled = false;
            lockApoapsis = GUILayout.Toggle(lockApoapsis, "Apoapsis (m)", GUILayout.Width(lockWidth), GUILayout.ExpandWidth(false));
            if (!lockApoapsis) GUI.enabled = false;
            apoapsisText = GUILayout.TextField(apoapsisText, GUILayout.Width(inputWidth), GUILayout.ExpandWidth(false));
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if ((!lockSMA && !lockPeriod) && lockLimitReached) GUI.enabled = false;
            lockSMA = GUILayout.Toggle(lockPeriod ? false : lockSMA, "Semi-Major Axis (m)", GUILayout.Width(lockWidth), GUILayout.ExpandWidth(false));
            if (!lockSMA) GUI.enabled = false;
            smaText = GUILayout.TextField(smaText, GUILayout.Width(inputWidth), GUILayout.ExpandWidth(false));
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (!lockEccentricity && lockLimitReached) GUI.enabled = false;
            lockEccentricity = GUILayout.Toggle(lockEccentricity, "Eccentricity", GUILayout.Width(lockWidth), GUILayout.ExpandWidth(false));
            if (!lockEccentricity) GUI.enabled = false;
            eccentricityText = GUILayout.TextField(eccentricityText, GUILayout.Width(inputWidth), GUILayout.ExpandWidth(false));
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if ((!lockSMA && !lockPeriod) && lockLimitReached) GUI.enabled = false;
            lockPeriod = GUILayout.Toggle(lockSMA ? false : lockPeriod, "Orbital Period (s)", GUILayout.Width(lockWidth), GUILayout.ExpandWidth(false));
            if (!lockPeriod) GUI.enabled = false;
            periodText = GUILayout.TextField(periodText, GUILayout.Width(inputWidth - synchPeriodButtonWidth - 4), GUILayout.ExpandWidth(false));
            if (GUILayout.Button("S", GUILayout.Width(synchPeriodButtonWidth), GUILayout.ExpandWidth(false)))
            {
                periodText = celestialBodies[selectedCelestialIndex].rotationPeriod.ToString();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Calculate", GUILayout.Width(calculateButtonWidth)))
            {
                CalculateAndParseOrbit();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(50);

            GUILayout.BeginHorizontal();
            if (errorText != "")
            {
                GUILayout.Label(errorText);
            }
            if (currentOrbit != null)
            {
                GUILayout.BeginVertical();
                GUILayout.Label("Periapsis Alt.: " + ParseElement(currentOrbit.PeriapsisAltitude, StringParseElementTypes.Distance));
                GUILayout.Label("Apoapsis Alt.: " + ParseElement(currentOrbit.ApoapsisAltitude, StringParseElementTypes.Distance));
                GUILayout.Label("S.Major Axis: " + ParseElement(currentOrbit.SemiMajorAxis, StringParseElementTypes.Distance));
                GUILayout.Label("Period: " + ParseElement(currentOrbit.OrbitalPeriod, StringParseElementTypes.Time));
                GUILayout.Label("Mean Orbit Speed: " + ParseElement(currentOrbit.MeanOrbitalSpeed, StringParseElementTypes.Velocity));
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                GUILayout.Label("Periapsis: " + ParseElement(currentOrbit.Periapsis, StringParseElementTypes.Distance));
                GUILayout.Label("Apoapsis: " + ParseElement(currentOrbit.Apoapsis, StringParseElementTypes.Distance));
                GUILayout.Label("Eccentricity: " + ParseElement(currentOrbit.Eccentricity, StringParseElementTypes.Default));
                GUILayout.Label("SOI Limit: " + ParseElement(currentOrbit.ParentBody.sphereOfInfluence, StringParseElementTypes.Distance));
                GUILayout.Label("Darkness Length: " + ParseElement(currentOrbit.MeanDarknessTime, StringParseElementTypes.Time));
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(50);
            GUILayout.Label("Options");

            GUILayout.BeginHorizontal();
            useAltitideAspides = GUILayout.Toggle(useAltitideAspides, "Use Altitudes for Apsides");
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        private void CalculateAndParseOrbit()
        {
            errorText = "";
            currentOrbit = null;
            try
            {
                currentOrbit = CalculateOrbit();
                ParseOrbit(currentOrbit);
            }
            catch (OrbitalElementExecption e)
            {
                errorText = e.Message;
            }
            catch (Exception e)
            {
                errorText = "Something unusual happened";
                Debug.LogError(e.Message);
            }
        }

        private void ParseOrbit(SimpleOrbit orbit)
        {
            if (!lockApoapsis)
            {
                if (useAltitideAspides)
                {
                    apoapsisText = orbit.ApoapsisAltitude.ToString();
                }
                else
                {
                    apoapsisText = orbit.Apoapsis.ToString();
                }
            }

            if (!lockPeriapsis)
            {
                if (useAltitideAspides)
                {
                    periapsisText = orbit.PeriapsisAltitude.ToString();
                }
                else
                {
                    periapsisText = orbit.Periapsis.ToString();
                }
            }

            if (!lockEccentricity)
            {
                eccentricityText = orbit.Eccentricity.ToString();
            }

            if (!lockSMA)
            {
                smaText = orbit.SemiMajorAxis.ToString();
            }

            if (!lockPeriod)
            {
                periodText = orbit.OrbitalPeriod.ToString();
            }
        }

        private SimpleOrbit CalculateOrbit()
        {
            SimpleOrbitBuilder orbitBuilder = new SimpleOrbitBuilder(celestialBodies[selectedCelestialIndex]);
            if (lockApoapsis)
            {
                if (useAltitideAspides)
                {
                    orbitBuilder.SetApoapsisAltitude(apoapsisText);
                }
                else
                {
                    orbitBuilder.SetApoapsis(apoapsisText);
                }
            }

            if (lockPeriapsis)
            {
                if (useAltitideAspides)
                {
                    orbitBuilder.SetPeriapsisAltitude(periapsisText);
                }
                else
                {
                    orbitBuilder.SetPeriapsis(periapsisText);
                }
            }

            if (lockEccentricity)
            {
                orbitBuilder.SetEccentricity(eccentricityText);
            }

            if (lockSMA)
            {
                orbitBuilder.SetSemiMajorAxis(smaText);
            }

            if (lockPeriod)
            {
                orbitBuilder.SetOrbitalPeriod(periodText);
            }

            return orbitBuilder.Build();
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

        enum StringParseElementTypes { Distance, Velocity, Time, Default };

        private static string ParseElement(double input, StringParseElementTypes parseType)
        {
            switch (parseType)
            {
                case StringParseElementTypes.Distance:
                    return string.Format("{0:0.###} km", input / 1000.0);
                case StringParseElementTypes.Velocity:
                    return string.Format("{0:0.###} m/s", input);
                case StringParseElementTypes.Time:
                    int seconds = (int)Math.Round(input);
                    TimeSpan span = new TimeSpan(0, 0, seconds);

                    int hours = span.Days * 24 + span.Hours;
                    string output = "";
                    if (hours > 0) output += hours + "h ";
                    if (span.Minutes > 0) output += span.Minutes + "m ";
                    if (span.Seconds > 0) output += span.Seconds + "s ";
                    return output.Trim();
                default:
                    return string.Format("{0:0.###}", input);
            }
        }
    }
}
