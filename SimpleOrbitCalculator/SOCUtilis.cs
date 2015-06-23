using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleOrbitCalculator
{
    /// <summary>
    /// GUIUtilities is a static class of helper methods for various GUI based functions.
    /// </summary>
    internal static class SOCUtilis
    {
        /// <summary>
        /// Converts an unitless orbit element into an easy to read string.
        /// </summary>
        /// <param name="input">The orbit element value.</param>
        /// <returns>The string representation.</returns>
        public static string ParseOrbitElement(double input)
        {
            // Default to 3 decimals.
            return string.Format("{0:0.###}", input);
        }

        public static string ParseOrbitElement(double input, string unit)
        {
            // Default to 3 decimals.
            return string.Format("{0:0.###} " + unit, input);
        }

        /// <summary>
        /// Converts orbit element scalers into an easy to read string.
        /// </summary>
        /// <param name="input">The orbit element value.</param>
        /// <param name="elementType">The scaler type.</param>
        /// <returns>The string representation.</returns>
        public static string ParseOrbitElement(double input, SimpleOrbit.ScalerType elementType)
        {
            switch (elementType)
            {
                // Distance will be 3 decimals and in kilometers.
                case SimpleOrbit.ScalerType.Distance:
                    return string.Format("{0:0.###} km", input / 1000.0);

                // Area will be 3 decimals and in km^2.
                case SimpleOrbit.ScalerType.Area:
                    return string.Format("{0:0.###} km²", input / 1000000.0);

                // Volume will be 3 decimals, sci notation and in km^3.
                case SimpleOrbit.ScalerType.Volume:
                    return string.Format("{0:0.0##e0} km³", input / 1000000000.0);

                // Mass will be 3 decimals, sci notation and in kg.
                case SimpleOrbit.ScalerType.Mass:
                    return string.Format("{0:0.0##e0} kg", input);

                // Density will be 3 decimals, and in g/cm^3.
                case SimpleOrbit.ScalerType.Density:
                    return string.Format("{0:0.###} g/cm³", input / 1000.0);

                // Speed will be 1 decimal and in meters per seconds.
                case SimpleOrbit.ScalerType.Speed:
                    return string.Format("{0:0.#} m/s", input);

                // Time will be displayed as days, hours, minutes and seconds.
                case SimpleOrbit.ScalerType.Time:
                    string output = (input < 0) ? "-" : "";
                    int seconds = (int)Math.Round(Math.Abs(input));
                    TimeSpan span = new TimeSpan(0, 0, seconds);

                    if (span.Days > 0) output += span.Days + "d ";
                    if (span.Hours > 0) output += span.Hours + "h ";
                    if (span.Minutes > 0) output += span.Minutes + "m ";
                    if (span.Seconds > 0) output += span.Seconds + "s ";
                    return output.Trim();

                // Specific Energy will be 3 decimal and in joules per kilograms.
                case SimpleOrbit.ScalerType.SpecificEnergy:
                    return string.Format("{0:0.###} J/kg", input);

                // Degrees will be 3 decimals
                case SimpleOrbit.ScalerType.Degrees:
                    return string.Format("{0:0.###}°", input);

                // Radians will be converted to degrees and be 3 decimals
                case SimpleOrbit.ScalerType.Radians:
                    input *= 180.0 / Math.PI;
                    return string.Format("{0:0.###}°", input);

                // Will default to 3 decimals for unknown types.
                default:
                    return ParseOrbitElement(input);
            }
        }

        /// <summary>
        /// Finds the primary planet of the system the celestial body is part of.
        /// Example: Kerbin is part of the Kerbin system, Ike is part of the Duna system, etc.
        /// </summary>
        /// <param name="celestialBody">the celestial body in question</param>
        /// <returns>the primary celestial body</returns>
        public static CelestialBody PlanetOfCelesital(CelestialBody celestialBody)
        {
            // Do not perform logic if input is the Sun
            if (celestialBody.referenceBody != celestialBody || celestialBody.referenceBody == null)
            {
                CelestialBody planet = celestialBody;
                // Loop until the celestial body is not the Sun
                while (true)
                {
                    if (planet.referenceBody.referenceBody == planet.referenceBody || planet.referenceBody.referenceBody == null)
                    {
                        return planet;
                    }
                    else
                    {
                        planet = planet.referenceBody;
                    }
                }
            }
            else
            {
                return null;
            }
        }
    }
}
