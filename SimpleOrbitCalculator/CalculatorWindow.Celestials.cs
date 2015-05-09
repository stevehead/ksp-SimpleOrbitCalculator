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

            // Start recursively adding the celestial bodies.
            LoadCelesitalChildren(theSun);
        }

        /// <summary>
        /// Recursively adds all celestial bodies to the list, ordered by parent body and SMA.
        /// </summary>
        /// <param name="celestialBody">the current celestial body</param>
        private void LoadCelesitalChildren(CelestialBody celestialBody)
        {
            List<CelestialBody> children = FlightGlobals.Bodies.Where(o => o.referenceBody == celestialBody && o != celestialBody)
                .OrderBy(o => o.orbit.semiMajorAxis).ToList();
            foreach (CelestialBody child in children)
            {
                celestialBodies.Add(child);
                LoadCelesitalChildren(child);
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
