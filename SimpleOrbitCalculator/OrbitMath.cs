using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleOrbitCalculator
{
    abstract class OrbitMath
    {
        public static double FocalParameter(double semiMajorAxis, double eccentricity)
        {
            double semiLatusRectum = SemiLatusRectum(semiMajorAxis, eccentricity);
            return semiLatusRectum / eccentricity;
        }

        public static double LinearEccentricity(double semiMajorAxis, double eccentricity)
        {
            return semiMajorAxis * eccentricity;
        }

        public static double MeanOrbitalSpeed(double semiMajorAxis, double eccentricity, double gravParameter)
        {
            return Math.Sqrt(gravParameter / semiMajorAxis) * (
                1.0 - 1.0 / 4.0 * Math.Pow(eccentricity, 2.0)
                - 3.0 / 64.0 * Math.Pow(eccentricity, 4.0)
                - 5.0 / 256.0 * Math.Pow(eccentricity, 6.0)
                - 175.0 / 16384.0 * Math.Pow(eccentricity, 8.0));
        }

        public static double SemiLatusRectum(double semiMajorAxis, double eccentricity)
        {
            double semiMinorAxis = SemiMinorAxis(semiMajorAxis, eccentricity);
            return Math.Pow(semiMinorAxis, 2.0) / semiMajorAxis;
        }

        public static double SemiMinorAxis(double semiMajorAxis, double eccentricity)
        {
            return semiMajorAxis * Math.Sqrt(1.0 - Math.Pow(eccentricity, 2.0));
        }
    }
}
