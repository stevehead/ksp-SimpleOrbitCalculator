using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleOrbitCalculator
{
    class CalculatorWindow
    {
        /// <summary>
        /// The singleton instance.
        /// </summary>
        private static CalculatorWindow instance;

        /// <summary>
        /// The total number of celestials per column in select area.
        /// </summary>
        private const int MaximumCelestialsPerColumn = 20;

        /// <summary>
        /// The width of the celestial individual select buttons.
        /// </summary>
        private const float CelestialSelectWidth = 100f;

        /// <summary>
        /// The width of the toggle for the lockeable inputs.
        /// </summary>
        private const float LockWidth = 150F;

        /// <summary>
        /// The width of the text input fields.
        /// </summary>
        private const float InputWidth = 200F;

        /// <summary>
        /// The width of single char buttons.
        /// </summary>
        private const float CharButtonWidth = 25F;

        /// <summary>
        /// The width of the calculate button.
        /// </summary>
        private const float CalculateButtonWidth = 100F;

        /// <summary>
        /// The width of the save orbit buttons.
        /// </summary>
        private const float SaveOrbitButtonWidth = 125F;

        /// <summary>
        /// List of known celestial bodies in the solar system.
        /// </summary>
        private List<CelestialBody> celestialBodies;

        /// <summary>
        /// The array of celestial's names for the celestial select area.
        /// </summary>
        private string[] celestialSelectValues;

        /// <summary>
        /// The current selected celestial index.
        /// </summary>
        private int selectedCelestialIndex = 0;

        /// <summary>
        /// Is periapsis locked?
        /// </summary>
        private bool lockPeriapsis = false;

        /// <summary>
        /// Is apoapsis locked?
        /// </summary>
        private bool lockApoapsis = false;

        /// <summary>
        /// Is eccentricity locked?
        /// </summary>
        private bool lockEccentricity = false;

        /// <summary>
        /// Is semi-major axis locked?
        /// </summary>
        private bool lockSMA = false;

        /// <summary>
        /// Is orbital period locked?
        /// </summary>
        private bool lockPeriod = false;

        /// <summary>
        /// Is altitude-based apsides being used?
        /// </summary>
        private bool useAltitideAspides = true;

        /// <summary>
        /// Periapsis input.
        /// </summary>
        private string periapsisText = "";

        /// <summary>
        /// Apoapsis input.
        /// </summary>
        private string apoapsisText = "";

        /// <summary>
        /// Eccentricity input.
        /// </summary>
        private string eccentricityText = "";

        /// <summary>
        /// Semi-major axis input.
        /// </summary>
        private string smaText = "";

        /// <summary>
        /// Orbital period input.
        /// </summary>
        private string periodText = "";

        /// <summary>
        /// Calculate orbit error text.
        /// </summary>
        private string errorText = "";

        /// <summary>
        /// The currently caclulated orbit.
        /// </summary>
        private SimpleOrbit currentOrbit = null;

        /// <summary>
        /// The current saved orbit 1.
        /// </summary>
        private SimpleOrbit savedOrbit1 = null;

        /// <summary>
        /// The current saved orbit 2.
        /// </summary>
        private SimpleOrbit savedOrbit2 = null;

        /// <summary>
        /// Singleton instance.
        /// </summary>
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

        /// <summary>
        /// The number of columns the celestial bodies will be in.
        /// </summary>
        private int CelestialSelectColumns
        {
            get { return (int)Math.Ceiling(celestialBodies.Count / (1.0 * MaximumCelestialsPerColumn)); }
        }

        /// <summary>
        /// The minimum width of the celestial select area.
        /// </summary>
        private float MinCelestialSelectAreaWidth
        {
            get { return CelestialSelectWidth * CelestialSelectColumns; }
        }

        /// <summary>
        /// Main constructor.
        /// </summary>
        private CalculatorWindow()
        {
            LoadAllCelestialInformation();
        }

        /// <summary>
        /// The main method that renders the window.
        /// </summary>
        public void Render()
        {
            GUILayout.BeginHorizontal();

            // Left Column
            GUILayout.BeginVertical(GUILayout.MinWidth(MinCelestialSelectAreaWidth), GUILayout.ExpandWidth(true));
            RenderCelestialSelectArea();
            GUILayout.EndVertical();

            GUILayout.Space(20);

            // Center Column
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            RenderMainInputArea();
            GUILayout.Space(25);
            RenderMainOutputArea();
            GUILayout.Space(25);
            RenderHohmannTransferArea();
            GUILayout.EndVertical();

            // Right Column
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            RenderOptionsArea();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        /// <summary>
        /// Renders the celestial select area.
        /// </summary>
        private void RenderCelestialSelectArea()
        {
            GUILayout.Label("Select a Body:");
            selectedCelestialIndex = GUILayout.SelectionGrid(selectedCelestialIndex, celestialSelectValues, CelestialSelectColumns);
        }

        /// <summary>
        /// Redners all of the user's input form.
        /// </summary>
        private void RenderMainInputArea()
        {
            // If the blank form fields are disabled.
            bool lockLimitReached = IsLockLimitReached();

            // Displays the current celestial.
            GUILayout.BeginHorizontal();
            GUILayout.Label("Current Body: " + celestialSelectValues[selectedCelestialIndex]);
            GUILayout.EndHorizontal();

            // Periapsis input.
            GUILayout.BeginHorizontal();
            if (!lockPeriapsis && lockLimitReached) GUI.enabled = false;
            lockPeriapsis = GUILayout.Toggle(lockPeriapsis, "Periapsis (m)", GUILayout.Width(LockWidth), GUILayout.ExpandWidth(false));
            if (!lockPeriapsis) GUI.enabled = false;
            periapsisText = GUILayout.TextField(periapsisText, GUILayout.Width(InputWidth), GUILayout.ExpandWidth(false));
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            // Apoapsis input.
            GUILayout.BeginHorizontal();
            if (!lockApoapsis && lockLimitReached) GUI.enabled = false;
            lockApoapsis = GUILayout.Toggle(lockApoapsis, "Apoapsis (m)", GUILayout.Width(LockWidth), GUILayout.ExpandWidth(false));
            if (!lockApoapsis) GUI.enabled = false;
            apoapsisText = GUILayout.TextField(apoapsisText, GUILayout.Width(InputWidth), GUILayout.ExpandWidth(false));
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            // Semi-major axis input.
            GUILayout.BeginHorizontal();
            if ((!lockSMA && !lockPeriod) && lockLimitReached) GUI.enabled = false;
            lockSMA = GUILayout.Toggle(lockPeriod ? false : lockSMA, "Semi-Major Axis (m)", GUILayout.Width(LockWidth), GUILayout.ExpandWidth(false));
            if (!lockSMA) GUI.enabled = false;
            smaText = GUILayout.TextField(smaText, GUILayout.Width(InputWidth), GUILayout.ExpandWidth(false));
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            // Eccentricity input.
            GUILayout.BeginHorizontal();
            if (!lockEccentricity && lockLimitReached) GUI.enabled = false;
            lockEccentricity = GUILayout.Toggle(lockEccentricity, "Eccentricity", GUILayout.Width(LockWidth), GUILayout.ExpandWidth(false));
            if (!lockEccentricity) GUI.enabled = false;
            eccentricityText = GUILayout.TextField(eccentricityText, GUILayout.Width(InputWidth), GUILayout.ExpandWidth(false));
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            // Orbital period input.
            GUILayout.BeginHorizontal();
            if ((!lockSMA && !lockPeriod) && lockLimitReached) GUI.enabled = false;
            lockPeriod = GUILayout.Toggle(lockSMA ? false : lockPeriod, "Orbital Period (s)", GUILayout.Width(LockWidth), GUILayout.ExpandWidth(false));
            if (!lockPeriod) GUI.enabled = false;
            periodText = GUILayout.TextField(periodText, GUILayout.Width(InputWidth - CharButtonWidth - 4), GUILayout.ExpandWidth(false));
            if (GUILayout.Button("S", GUILayout.Width(CharButtonWidth), GUILayout.ExpandWidth(false)))
            {
                periodText = celestialBodies[selectedCelestialIndex].rotationPeriod.ToString();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            // Calculate orbit button.
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Calculate", GUILayout.Width(CalculateButtonWidth))) CalculateAndParseOrbit();
            GUILayout.EndHorizontal();
        }

        private void RenderMainOutputArea()
        {
            GUILayout.BeginHorizontal();

            // Display error if it exists.
            if (errorText != "") GUILayout.Label(errorText);

            // Parse the caclulated orbit.
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
        }

        private void RenderHohmannTransferArea()
        {
            // The Orbit 1 Save button.
            GUILayout.BeginHorizontal();
            if (currentOrbit == null) GUI.enabled = false;
            if (GUILayout.Button("Save as Orbit 1", GUILayout.Width(SaveOrbitButtonWidth))) savedOrbit1 = currentOrbit;
            GUI.enabled = true;
            if (savedOrbit1 != null)
            {
                if (GUILayout.Button("C", GUILayout.Width(CharButtonWidth))) savedOrbit1 = null;
                GUILayout.Label(savedOrbit1.ToString(useAltitideAspides));
            }
            GUILayout.EndHorizontal();

            // The Orbit 2 Save button.
            GUILayout.BeginHorizontal();
            if (currentOrbit == null) GUI.enabled = false;
            if (GUILayout.Button("Save as Orbit 2", GUILayout.Width(SaveOrbitButtonWidth))) savedOrbit2 = currentOrbit;
            GUI.enabled = true;
            if (savedOrbit2 != null)
            {
                if (GUILayout.Button("C", GUILayout.Width(CharButtonWidth))) savedOrbit2 = null;
                GUILayout.Label(savedOrbit2.ToString(useAltitideAspides));
            }
            GUILayout.EndHorizontal();

            // Calculate the delta-V of transfer if valid.
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
        }

        /// <summary>
        /// Renders the calculator's options.
        /// </summary>
        private void RenderOptionsArea()
        {
            GUILayout.Label("Options");

            // The Apsides altitude option.
            GUILayout.BeginHorizontal();
            useAltitideAspides = GUILayout.Toggle(useAltitideAspides, "Use Altitudes for Apsides");
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Determines if the lock limit has been reached for the inputs.
        /// </summary>
        /// <returns>True is lock limit reached, false if not.</returns>
        private bool IsLockLimitReached()
        {
            int countLocks = 0;
            bool lockLimitReached = false;
            if (lockPeriapsis) countLocks++;
            if (lockApoapsis) countLocks++;
            if (lockEccentricity) countLocks++;
            if (lockSMA) countLocks++;
            if (lockPeriod) countLocks++;
            if (countLocks >= 2) lockLimitReached = true;
            return lockLimitReached;
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
