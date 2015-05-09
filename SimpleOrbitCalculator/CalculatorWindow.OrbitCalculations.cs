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

            if (!lockEccentricity) eccentricityText = orbit.Eccentricity.ToString();
            if (!lockSMA) smaText = orbit.SemiMajorAxis.ToString();
            if (!lockPeriod) periodText = orbit.OrbitalPeriod.ToString();
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
            catch (FormatException)
            {
                errorText = "An input is not a valid number.";
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
                if (useAltitideAspides) orbitBuilder.SetApoapsisAltitude(apoapsisText);
                else orbitBuilder.SetApoapsis(apoapsisText);
            }

            if (lockPeriapsis)
            {
                if (useAltitideAspides) orbitBuilder.SetPeriapsisAltitude(periapsisText);
                else orbitBuilder.SetPeriapsis(periapsisText);
            }

            if (lockEccentricity) orbitBuilder.SetEccentricity(eccentricityText);
            if (lockSMA) orbitBuilder.SetSemiMajorAxis(smaText);
            if (lockPeriod) orbitBuilder.SetOrbitalPeriod(periodText);

            return orbitBuilder.Build();
        }

        /// <summary>
        /// Loads, calculates and parses the current vessel's orbit.
        /// </summary>
        private void LoadActiveVesselOrbit()
        {
            LoadOrbit(FlightGlobals.ActiveVessel.orbit);
        }

        /// <summary>
        /// Load, calculates and parses the current target's orbit.
        /// </summary>
        private void LoadTargetOrbit()
        {
            LoadOrbit(FlightGlobals.ActiveVessel.targetObject.GetOrbit());
        }

        /// <summary>
        /// Load, calculates and parses the input orbit.
        /// </summary>
        /// <param name="orbit">The orbit to use.</param>
        private void LoadOrbit(Orbit orbit)
        {
            // Reset all user inputs.
            ResetInputs();

            // Find the index of the orbit's parent body.
            selectedCelestialIndex = celestialBodies.IndexOf(orbit.referenceBody);

            // Get the eccentricity and semi-major axis.
            eccentricityText = orbit.eccentricity.ToString();
            smaText = orbit.semiMajorAxis.ToString();

            // Lock the eccentricity and semi-major axis.
            lockEccentricity = true;
            lockSMA = true;

            // Calculate and parse the orbit.
            CalculateAndParseOrbit();
        }
    }
}
