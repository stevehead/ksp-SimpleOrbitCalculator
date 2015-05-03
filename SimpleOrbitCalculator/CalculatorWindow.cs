using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleOrbitCalculator
{
    class CalculatorWindow
    {
        private static CalculatorWindow instance;

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
        private SimpleOrbit savedOrbit1 = null;
        private SimpleOrbit savedOrbit2 = null;

        public static CalculatorWindow Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CalculatorWindow();
                }
                return instance;
            }
        }

        private CalculatorWindow()
        {
            LoadAllCelestialInformation();
        }

        public void Render()
        {
            // Need to deterime how many "locks" were used on the GUI inputs. We should not have more than 2.
            int countLocks = 0;
            bool lockLimitReached = false;
            if (lockPeriapsis) countLocks++;
            if (lockApoapsis) countLocks++;
            if (lockEccentricity) countLocks++;
            if (lockSMA) countLocks++;
            if (lockPeriod) countLocks++;
            if (countLocks >= 2) lockLimitReached = true;

            // Window Settings - TODO: make as class constants?
            int celestialSelectColumns = (int)Math.Ceiling(celestialBodies.Count / 20.0);
            float minCelestialSelectWidth = 100f * celestialSelectColumns;
            float lockWidth = 150f;
            float inputWidth = 200f;
            float synchPeriodButtonWidth = 25f;
            float calculateButtonWidth = 100f;
            float saveOrbitButtonWidth = 125f;

            #region MainWindow : Main Area
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.MinWidth(minCelestialSelectWidth), GUILayout.ExpandWidth(true));
            GUILayout.Label("Select a Body:");
            selectedCelestialIndex = GUILayout.SelectionGrid(selectedCelestialIndex, celestialSelectValues, celestialSelectColumns);
            GUILayout.EndVertical();

            GUILayout.Space(20);

            #region MainWindow : Main Input Area
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

            GUILayout.Space(25);

            GUILayout.BeginHorizontal();
            if (errorText != "")
            {
                GUILayout.Label(errorText);
            }
            if (currentOrbit != null)
            {
                GUILayout.BeginVertical();
                GUILayout.Label("Periapsis Alt.: " + SOCUtilis.ParseOrbitElement(currentOrbit.PeriapsisAltitude, SimpleOrbit.ScalerType.Distance));
                GUILayout.Label("Periapsis: " + SOCUtilis.ParseOrbitElement(currentOrbit.Periapsis, SimpleOrbit.ScalerType.Distance));
                GUILayout.Label("Periapsis Speed: " + SOCUtilis.ParseOrbitElement(currentOrbit.PeriapsisSpeed, SimpleOrbit.ScalerType.Speed));
                GUILayout.Label("S.Major Axis: " + SOCUtilis.ParseOrbitElement(currentOrbit.SemiMajorAxis, SimpleOrbit.ScalerType.Distance));
                GUILayout.Label("Period: " + SOCUtilis.ParseOrbitElement(currentOrbit.OrbitalPeriod, SimpleOrbit.ScalerType.Time));
                GUILayout.Label("SOI Limit: " + SOCUtilis.ParseOrbitElement(currentOrbit.ParentBody.sphereOfInfluence, SimpleOrbit.ScalerType.Distance));
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                GUILayout.Label("Apoapsis Alt.: " + SOCUtilis.ParseOrbitElement(currentOrbit.ApoapsisAltitude, SimpleOrbit.ScalerType.Distance));
                GUILayout.Label("Apoapsis: " + SOCUtilis.ParseOrbitElement(currentOrbit.Apoapsis, SimpleOrbit.ScalerType.Distance));
                GUILayout.Label("Apoapsis Speed: " + SOCUtilis.ParseOrbitElement(currentOrbit.ApoapsisSpeed, SimpleOrbit.ScalerType.Speed));
                GUILayout.Label("Eccentricity: " + SOCUtilis.ParseOrbitElement(currentOrbit.Eccentricity));
                GUILayout.Label("Mean Orbit Speed: " + SOCUtilis.ParseOrbitElement(currentOrbit.MeanOrbitalSpeed, SimpleOrbit.ScalerType.Speed));
                GUILayout.Label("Max. Darkness Length: " + SOCUtilis.ParseOrbitElement(currentOrbit.MaxDarknessTime, SimpleOrbit.ScalerType.Time));
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            #region MainWindow : Hohmann Transfer Calculator
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            if (currentOrbit != null)
            {
                GUILayout.Space(25);
            }

            GUILayout.BeginHorizontal();
            if (currentOrbit == null)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Save as Orbit 1", GUILayout.Width(saveOrbitButtonWidth)))
            {
                savedOrbit1 = currentOrbit;
            }
            GUI.enabled = true;
            if (savedOrbit1 != null)
            {
                if (GUILayout.Button("C", GUILayout.Width(synchPeriodButtonWidth)))
                {
                    savedOrbit1 = null;
                }
                GUILayout.Label(savedOrbit1.ToString(useAltitideAspides));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (currentOrbit == null)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Save as Orbit 2", GUILayout.Width(saveOrbitButtonWidth)))
            {
                savedOrbit2 = currentOrbit;
            }
            GUI.enabled = true;
            if (savedOrbit2 != null)
            {
                if (GUILayout.Button("C", GUILayout.Width(synchPeriodButtonWidth)))
                {
                    savedOrbit2 = null;
                }
                GUILayout.Label(savedOrbit2.ToString(useAltitideAspides));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (savedOrbit1 != null && savedOrbit2 != null)
            {
                if (savedOrbit1.ParentBody == savedOrbit2.ParentBody)
                {
                    try
                    {
                        double hohmannTransferDeltaV = SimpleOrbit.CalculateHohmannTransferDeltaV(savedOrbit1, savedOrbit2);
                        GUILayout.Label("Transfer Delta-V: " + SOCUtilis.ParseOrbitElement(hohmannTransferDeltaV, SimpleOrbit.ScalerType.Speed));
                    }
                    catch (ArgumentException e)
                    {
                        GUILayout.Label("Error: " + e.Message);
                    }
                }
                else
                {
                    GUILayout.Label("Parent bodies of saved orbits do not match.");
                }

            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            #endregion

            GUILayout.EndVertical();
            #endregion

            #region MainWindow : Options Area
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Label("Options");

            GUILayout.BeginHorizontal();
            useAltitideAspides = GUILayout.Toggle(useAltitideAspides, "Use Altitudes for Apsides");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            #endregion

            GUILayout.EndHorizontal();
            #endregion

            GUI.DragWindow();
        }

        /// <summary>
        /// Will set the GUI text fields based on the input orbit.
        /// </summary>
        /// <param name="orbit">An orbit that was calculated.</param>
        private void ParseOrbit(SimpleOrbit orbit)
        {
            if (!lockApoapsis)
            {
                apoapsisText = useAltitideAspides ? orbit.ApoapsisAltitude.ToString() : orbit.Apoapsis.ToString();
            }

            if (!lockPeriapsis)
            {
                periapsisText = useAltitideAspides ? orbit.PeriapsisAltitude.ToString() : orbit.Periapsis.ToString();
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

        /// <summary>
        /// Will attempt to create the orbit from the user's inputs, and then parse the output orbit or error.
        /// </summary>
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

        /// <summary>
        /// Will create the orbit object from the user's input.
        /// </summary>
        /// <returns>The calculated orbit.</returns>
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
        /// This should be compatible with RSS scales and Planet Factory like mods.
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
