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

                // Time will be displayed as hours, minutes and seconds.
                case SimpleOrbit.ScalerType.Time:
                    int seconds = (int)Math.Round(input);
                    TimeSpan span = new TimeSpan(0, 0, seconds);

                    int hours = span.Days * 24 + span.Hours;
                    string output = "";
                    if (hours > 0) output += hours + "h ";
                    if (span.Minutes > 0) output += span.Minutes + "m ";
                    if (span.Seconds > 0) output += span.Seconds + "s ";
                    return output.Trim();

                // Specific Energy will be 3 decimal and in joules per kilograms.
                case SimpleOrbit.ScalerType.SpecificEnergy:
                    return string.Format("{0:0.###} J/kg", input);

                // Will default to 3 decimals for unknown types.
                default:
                    return ParseOrbitElement(input);
            }
        }
    }
}
