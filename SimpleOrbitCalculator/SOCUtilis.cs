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

                // Speed will be 1 decimal and in meters per seconds.
                case SimpleOrbit.ScalerType.Speed:
                    return string.Format("{0:0.#} m/s", input);

                // Time will be displayed as days, hours, minutes and seconds.
                case SimpleOrbit.ScalerType.Time:
                    int seconds = (int)Math.Round(input);
                    TimeSpan span = new TimeSpan(0, 0, seconds);
                    string output = "";

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
    }
}
