using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleOrbitCalculator
{
    internal partial class CalculatorWindow
    {
        /// <summary>
        /// The title of the calculator's window.
        /// </summary>
        private const string WindowTitle = SimpleOrbitCalculatorController.PluginName;

        /// <summary>
        /// The total number of celestials per column in select area.
        /// </summary>
        private const int MaximumCelestialsPerColumn = 20;

        /// <summary>
        /// The width of the celestial individual select buttons.
        /// </summary>
        private const float CelestialSelectWidth = 100F;

        /// <summary>
        /// The width of the use current orbit button.
        /// </summary>
        private const float UseCurrentOrbitButtonWidth = 175F;

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
        /// The unique ID of the window.
        /// </summary>
        private int id;

        /// <summary>
        /// Is the window open?
        /// </summary>
        private bool isWindowOpen = true;

        /// <summary>
        /// Position of the calculator's window.
        /// </summary>
        private Rect windowPosition;

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
    }
}
