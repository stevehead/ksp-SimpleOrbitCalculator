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
        /// The main method that renders the window.
        /// </summary>
        private void RenderWindow(int windowId)
        {
            // Begin Main Window
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

            // Begin Left Column
            GUILayout.BeginVertical(GUILayout.MinWidth(MinCelestialSelectAreaWidth), GUILayout.ExpandWidth(true));
            RenderCelestialSelectArea();
            GUILayout.EndVertical();
            // End Left Column

            GUILayout.Space(20);

            if (!inCelestialBodyInfoMode)
            {
                // Begin Right Column
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

                // Begin Right Column, Top Row
                GUILayout.BeginHorizontal();

                // Begin Right Column, Top Row, Left Column
                GUILayout.BeginVertical();
                RenderMainInputArea();
                GUILayout.EndVertical();
                // End Right Column, Top Row, Left Column

                // Begin Right Column, Top Row, Right Column
                GUILayout.BeginVertical();
                RenderOptionsArea();
                GUILayout.EndVertical();
                // End Right Column, Top Row, Right Column

                GUILayout.EndHorizontal();
                // End Right Column, Top Row

                GUILayout.Space(25);

                // Begin Right Column, Middle Row
                GUILayout.BeginVertical();
                RenderMainOutputArea();
                GUILayout.EndVertical();
                // End Right Column, Middle Row

                GUILayout.Space(25);

                // Begin Right Column, Bottom Row
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                RenderHohmannTransferArea();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                // End Right Column, Bottom Row

                GUILayout.EndVertical();
                // End Right Column
            }
            else
            {
                // Begin Center Column
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                RenderCelestialInfoArea();
                GUILayout.EndVertical();
                // End Center Column

                // Begin Left Column
                GUILayout.BeginVertical();
                RenderOptionsArea();
                GUILayout.EndVertical();
                // End left Column
            }

            GUILayout.EndHorizontal();
            // End MainWindow

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
            GUILayout.Label("Current Body: " + celestialSelectValues[selectedCelestialIndex]);

            // Use current active vessel's orbit button.
            if (HighLogic.LoadedSceneIsFlight)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Use Current Orbit", GUILayout.Width(UseCurrentOrbitButtonWidth))) LoadActiveVesselOrbit();

                if (FlightGlobals.ActiveVessel.targetObject != null)
                {
                    if (GUILayout.Button("Use Target Orbit", GUILayout.Width(UseCurrentOrbitButtonWidth))) LoadTargetOrbit();
                }

                GUILayout.EndHorizontal();
            }

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
            if (GUILayout.Button("Calculate", GUILayout.Width(CalculateButtonWidth))) CalculateAndParseOrbit();
        }

        private void RenderMainOutputArea()
        {
            // Display error if it exists.
            if (errorText != "") GUILayout.Label(errorText);

            // Parse the caclulated orbit.
            if (currentOrbit != null)
            {
                if (currentOrbit.IsOutsideSOI)
                {
                    GUILayout.Label("Warning: Orbit is outside of the SOI.");
                }
                if (currentOrbit.Periapsis < currentOrbit.ParentBody.Radius)
                {
                    GUILayout.Label("Warning: Orbit intersects surface.");
                }

                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                GUILayout.Label("Periapsis Alt.: " + SOCUtilis.ParseOrbitElement(currentOrbit.PeriapsisAltitude, SimpleOrbit.ScalerType.Distance));
                GUILayout.Label("Periapsis: " + SOCUtilis.ParseOrbitElement(currentOrbit.Periapsis, SimpleOrbit.ScalerType.Distance));
                GUILayout.Label("Periapsis Speed: " + SOCUtilis.ParseOrbitElement(currentOrbit.PeriapsisSpeed, SimpleOrbit.ScalerType.Speed));
                GUILayout.Label("S.Major Axis: " + SOCUtilis.ParseOrbitElement(currentOrbit.SemiMajorAxis, SimpleOrbit.ScalerType.Distance));
                GUILayout.Label("Period: " + SOCUtilis.ParseOrbitElement(currentOrbit.OrbitalPeriod, SimpleOrbit.ScalerType.Time));
                try
                {
                    string dragText = (currentOrbit.ParentBody.atmosphere) ? " + drag" : "";
                    GUILayout.Label("Launch Δv: " + SOCUtilis.ParseOrbitElement(currentOrbit.CalculateEquatorialLandingDeltaV(), SimpleOrbit.ScalerType.Speed) + dragText);
                }
                catch (OrbitalElementExecption)
                {
                    GUILayout.Label("Launch Δv: n/a");
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                GUILayout.Label("Apoapsis Alt.: " + SOCUtilis.ParseOrbitElement(currentOrbit.ApoapsisAltitude, SimpleOrbit.ScalerType.Distance));
                GUILayout.Label("Apoapsis: " + SOCUtilis.ParseOrbitElement(currentOrbit.Apoapsis, SimpleOrbit.ScalerType.Distance));
                GUILayout.Label("Apoapsis Speed: " + SOCUtilis.ParseOrbitElement(currentOrbit.ApoapsisSpeed, SimpleOrbit.ScalerType.Speed));
                GUILayout.Label("Eccentricity: " + SOCUtilis.ParseOrbitElement(currentOrbit.Eccentricity));
                GUILayout.Label("Mean Orbit Speed: " + SOCUtilis.ParseOrbitElement(currentOrbit.MeanOrbitalSpeed, SimpleOrbit.ScalerType.Speed));
                GUILayout.Label("Max. Darkness Length: " + SOCUtilis.ParseOrbitElement(currentOrbit.MaxDarknessTime, SimpleOrbit.ScalerType.Time));
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
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
            if (savedOrbit1 != null && savedOrbit2 != null)
            {
                if (savedOrbit1.ParentBody == savedOrbit2.ParentBody)
                {
                    try
                    {
                        double hohmannTransferDeltaV = SimpleOrbit.CalculateHohmannTransferDeltaV(savedOrbit1, savedOrbit2);
                        GUILayout.Label("Transfer Δv: " + SOCUtilis.ParseOrbitElement(hohmannTransferDeltaV, SimpleOrbit.ScalerType.Speed));
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
        }

        /// <summary>
        /// Renders the calculator's options.
        /// </summary>
        private void RenderOptionsArea()
        {
            GUILayout.Label("Options");

            // The celestial body info mode option.
            inCelestialBodyInfoMode = GUILayout.Toggle(inCelestialBodyInfoMode, "Celestial Body Info Mode");

            if (!inCelestialBodyInfoMode)
            {
                // The Apsides altitude option.
                useAltitideAspides = GUILayout.Toggle(useAltitideAspides, "Use Altitudes for Apsides");
            }
        }

        /// <summary>
        /// Renders the celestial info area.
        /// </summary>
        private void RenderCelestialInfoArea()
        {
            // The current celestial.
            CelestialBody celestialBody = celestialBodies[selectedCelestialIndex];

            // Displays the current celestial.
            GUILayout.BeginHorizontal();
            GUILayout.Label("Selected Body: " + celestialSelectValues[selectedCelestialIndex]);
            GUILayout.EndHorizontal();

            // Main info area
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

            // Orbital Characteristics
            if (celestialBody.orbit != null)
            {
                // Calculations
                double meanOrbitalSpeed = OrbitMath.MeanOrbitalSpeed(celestialBody.orbit.semiMajorAxis, celestialBody.orbit.eccentricity, celestialBody.referenceBody.gravParameter);

                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                GUILayout.Label("ORBITAL CHARACTERISTICS");
                GUILayout.Label("Apoapsis: " + SOCUtilis.ParseOrbitElement(celestialBody.orbit.ApR, SimpleOrbit.ScalerType.Distance));
                GUILayout.Label("Periapsis: " + SOCUtilis.ParseOrbitElement(celestialBody.orbit.PeR, SimpleOrbit.ScalerType.Distance));
                GUILayout.Label("Semi-major axis: " + SOCUtilis.ParseOrbitElement(celestialBody.orbit.semiMajorAxis, SimpleOrbit.ScalerType.Distance));
                GUILayout.Label("Eccentricity: " + SOCUtilis.ParseOrbitElement(celestialBody.orbit.eccentricity));
                GUILayout.Label("Orbital period: " + SOCUtilis.ParseOrbitElement(celestialBody.orbit.period, SimpleOrbit.ScalerType.Time));
                GUILayout.Label("Average orbital speed: " + SOCUtilis.ParseOrbitElement(meanOrbitalSpeed, SimpleOrbit.ScalerType.Speed));
                GUILayout.Label("Mean anomaly at epoch: " + SOCUtilis.ParseOrbitElement(celestialBody.orbit.meanAnomalyAtEpoch, SimpleOrbit.ScalerType.Radians));
                GUILayout.Label("Inclination: " + SOCUtilis.ParseOrbitElement(celestialBody.orbit.inclination, SimpleOrbit.ScalerType.Degrees));
                GUILayout.Label("Longitude of ascending node: " + SOCUtilis.ParseOrbitElement(celestialBody.orbit.LAN, SimpleOrbit.ScalerType.Degrees));
                GUILayout.Label("Argument of periapsis: " + SOCUtilis.ParseOrbitElement(celestialBody.orbit.argumentOfPeriapsis, SimpleOrbit.ScalerType.Degrees));
                GUILayout.EndVertical();
            }

            // Physical Characteristics
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Label("PHYSICAL CHARACTERISTICS");
            GUILayout.Label("Radius: " + SOCUtilis.ParseOrbitElement(celestialBody.Radius, SimpleOrbit.ScalerType.Distance));
            GUILayout.Label("Circumference: " + SOCUtilis.ParseOrbitElement(celestialBody.Radius * 2.0 * Math.PI, SimpleOrbit.ScalerType.Distance));
            GUILayout.Label("Surface area: " + SOCUtilis.ParseOrbitElement(celestialBody.SurfaceArea, SimpleOrbit.ScalerType.Area));
            GUILayout.Label("Volume: " + SOCUtilis.ParseOrbitElement(celestialBody.Mass / celestialBody.Density, SimpleOrbit.ScalerType.Volume));
            GUILayout.Label("Mass: " + SOCUtilis.ParseOrbitElement(celestialBody.Mass, SimpleOrbit.ScalerType.Mass));
            GUILayout.Label("Density: " + SOCUtilis.ParseOrbitElement(celestialBody.Density, SimpleOrbit.ScalerType.Density));
            GUILayout.Label("Surface gravity: " + SOCUtilis.ParseOrbitElement(celestialBody.GeeASL, "g"));
            GUILayout.Label("Escape velocity: " + SOCUtilis.ParseOrbitElement(Math.Sqrt(2.0 * celestialBody.gravParameter / celestialBody.Radius), SimpleOrbit.ScalerType.Speed));
            GUILayout.Label("Sphere of influence: " + SOCUtilis.ParseOrbitElement(celestialBody.sphereOfInfluence, SimpleOrbit.ScalerType.Distance));
            GUILayout.Label("Sidereal rotation period: " + SOCUtilis.ParseOrbitElement(celestialBody.rotationPeriod, SimpleOrbit.ScalerType.Time));

            CelestialBody planet = SOCUtilis.PlanetOfCelesital(celestialBody);
            if (planet != null)
            {
                double solarDay = Math.Abs(celestialBody.rotationPeriod / (1.0 - celestialBody.rotationPeriod / planet.orbit.period));
                GUILayout.Label("Solar day: " + SOCUtilis.ParseOrbitElement(solarDay, SimpleOrbit.ScalerType.Time));
                if (celestialBody.atmosphere)
                {
                    GUILayout.Label("Surface pressure: " + SOCUtilis.ParseOrbitElement(celestialBody.atmospherePressureSeaLevel, "kPa"));
                }
            }

            GUILayout.EndVertical();
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
        /// Resets all user inputs.
        /// </summary>
        private void ResetInputs()
        {
            lockPeriapsis = false;
            lockApoapsis = false;
            lockEccentricity = false;
            lockSMA = false;
            lockPeriod = false;
            periapsisText = "";
            apoapsisText = "";
            eccentricityText = "";
            smaText = "";
            periodText = "";
            errorText = "";
            currentOrbit = null;
        }
    }
}
