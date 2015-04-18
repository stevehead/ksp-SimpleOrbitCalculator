using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleOrbitCalculator
{
    class SimpleOrbit
    {
        private readonly CelestialBody parentBody;
        private readonly double semiMajorAxis;
        private readonly double eccentricity;
        private readonly double apoapsis;
        private readonly double periapsis;
        private readonly double orbitalPeriod;
        private readonly double apoapsisAltitude;
        private readonly double periapsisAltitude;
        private readonly double meanOrbitalSpeed;
        private readonly double meanDarknessTime;
        private readonly double specificOrbitalEnergy;

        public double SemiMajorAxis { get { return semiMajorAxis; }}
        public double Eccentricity { get { return eccentricity; } }
        public double Apoapsis { get { return apoapsis; } }
        public double Periapsis { get { return periapsis; } }
        public double OrbitalPeriod { get { return orbitalPeriod; } }
        public double ApoapsisAltitude { get { return apoapsisAltitude; } }
        public double PeriapsisAltitude { get { return periapsisAltitude; } }
        public double MeanOrbitalSpeed { get { return meanOrbitalSpeed; } }
        public double MeanDarknessTime { get { return meanDarknessTime; } }
        public double SpecificOrbitalEnergy { get { return specificOrbitalEnergy; } }

        private SimpleOrbit(CelestialBody parentBody, double semiMajorAxis, double eccentricity, double apoapsis,
            double periapsis, double orbitalPeriod, double apoapsisAltitude, double periapsisAltitude)
        {
            this.parentBody = parentBody;
            this.semiMajorAxis = semiMajorAxis;
            this.eccentricity = eccentricity;
            this.apoapsis = apoapsis;
            this.periapsis = periapsis;
            this.orbitalPeriod = orbitalPeriod;
            this.apoapsisAltitude = apoapsisAltitude;
            this.periapsisAltitude = periapsisAltitude;
            this.meanOrbitalSpeed = Math.Sqrt(parentBody.gravParameter / semiMajorAxis) * (
                1.0 - 1.0 / 4.0 * Math.Pow(eccentricity, 2.0)
                - 3.0 / 64.0 * Math.Pow(eccentricity, 4.0)
                - 5.0 / 256.0 * Math.Pow(eccentricity, 6.0)
                - 175.0 / 16384.0 * Math.Pow(eccentricity, 8.0));
            this.meanDarknessTime = orbitalPeriod * Math.Asin(parentBody.Radius / semiMajorAxis) / Math.PI;
            this.specificOrbitalEnergy = -parentBody.gravParameter / (2.0 * semiMajorAxis);
        }

        public static class Builder
        {
            private readonly CelestialBody parentBody;
            private double semiMajorAxis;
            private double eccentricity;
            private double apoapsis;
            private double periapsis;
            private double orbitalPeriod;
            private double apoapsisAltitude;
            private double periapsisAltitude;

            public Builder(CelestialBody parentBody)
            {
                this.parentBody = parentBody;
            }

            public SimpleOrbit Build()
            {
                ValidateElements();
                CalculateRemainingElements();
                return new SimpleOrbit(parentBody, semiMajorAxis, eccentricity, apoapsis, periapsis, orbitalPeriod, apoapsisAltitude, periapsis);
            }

            public Builder SetSemiMajorAxis(double semiMajorAxis)
            {
                this.semiMajorAxis = semiMajorAxis;
                return this;
            }

            public Builder SetEccentricity(double eccentricity)
            {
                this.eccentricity = eccentricity;
                return this;
            }

            public Builder SetApoapsis(double apoapsis)
            {
                this.apoapsis = apoapsis;
                return this;
            }

            public Builder SetPeriapsis(double periapsis)
            {
                this.periapsis = periapsis;
                return this;
            }

            public Builder SetOrbitalPeriod(double orbitalPeriod)
            {
                this.orbitalPeriod = orbitalPeriod;
                return this;
            }

            public Builder SetApoapsisAltitude(double apoapsisAltitude)
            {
                this.apoapsisAltitude = apoapsisAltitude;
                return this;
            }

            public Builder SetPeriapsisAltitude(double periapsisAltitude)
            {
                this.periapsisAltitude = periapsisAltitude;
                return this;
            }

            private int CountSetElements()
            {
                int count = 0;
                if (semiMajorAxis != null) count++;
                if (eccentricity != null) count++;
                if (apoapsis != null) count++;
                if (periapsis != null) count++;
                if (orbitalPeriod != null) count++;
                if (apoapsisAltitude != null) count++;
                if (periapsisAltitude != null) count++;
                return count;
            }

            private void ValidateElements()
            {
                if (CountSetElements() != 2)
                {
                    throw new OrbitalElementExecption("Two orbital elements are required to complete the calculation.");
                }

                if (apoapsis != null && apoapsisAltitude != null)
                {
                    throw new OrbitalElementExecption("Apoapsis and apoapsis altitude are both set.");
                }

                if (periapsis != null && periapsisAltitude != null)
                {
                    throw new OrbitalElementExecption("Periapsis and periapsis altitude are both set.");
                }

                if (semiMajorAxis != null && orbitalPeriod != null)
                {
                    throw new OrbitalElementExecption("Semi-major-axis and orbital period creates an ambiguous case.");
                }
            }

            private void CalculateRemainingElements()
            {
                SetApsides();

                double gravParam = parentBody.gravParameter;

                if (orbitalPeriod == null)
                {
                    if (eccentricity == null)
                    {
                        // Apoapsis and Periapsis
                        if (apoapsis != null && periapsis != null)
                        {
                            semiMajorAxis = (apoapsis + periapsis) / 2.0;
                        }
                        // Apoapsis and Semi-Major-Axis
                        else if (apoapsis != null && semiMajorAxis != null)
                        {
                            periapsis = 2.0 * semiMajorAxis - apoapsis;
                        }
                        // Periapsis and Semi-Major-Axis
                        else if (periapsis != null && semiMajorAxis != null)
                        {
                            apoapsis = 2.0 * semiMajorAxis - periapsis;
                        }

                        eccentricity = (apoapsis - periapsis) / (apoapsis + periapsis);
                    }
                    else
                    {
                        // Eccentricity and Apoapsis
                        if (apoapsis != null)
                        {
                            periapsis = apoapsis * (1.0 - eccentricity) / (1.0 + eccentricity);
                        }
                        // Eccentricity and Periapsis
                        else if (periapsis != null)
                        {
                            apoapsis = periapsis * (1.0 + eccentricity) / (1.0 - eccentricity);
                        }

                        semiMajorAxis = (apoapsis + periapsis) / 2.0;
                    }
                    
                    orbitalPeriod = 2.0 * Math.PI * Math.Sqrt(Math.Pow(semiMajorAxis, 3.0) / gravParam);
                }
                else
                {
                    semiMajorAxis = Math.Pow(gravParam * Math.Pow(orbitalPeriod / (2.0 * Math.PI), 2.0), 1.0 / 3.0);

                    // Period and Eccentricity
                    if (eccentricity != null)
                    {
                        periapsis = semiMajorAxis * (1.0 - eccentricity);
                        apoapsis = semiMajorAxis * (1.0 + eccentricity);
                    }
                    else
                    {
                        // Period and Apoapsis
                        if (apoapsis != null)
                        {
                            periapsis = 2.0 * semiMajorAxis - apoapsis;
                        }
                        // Period and Periapsis
                        else if (periapsis != null)
                        {
                            apoapsis = 2.0 * semiMajorAxis - periapsis;
                        }

                        eccentricity = (apoapsis - periapsis) / (apoapsis + periapsis);
                    }
                }

                SetApsides();
            }

            private void SetApsides()
            {
                if (apoapsis != null && apoapsisAltitude == null)
                {
                    apoapsisAltitude = apoapsis - parentBody.Radius;
                }
                else if (apoapsis == null && apoapsisAltitude != null)
                {
                    apoapsis = apoapsisAltitude + parentBody.Radius;
                }

                if (periapsis != null && periapsisAltitude == null)
                {
                    periapsisAltitude = periapsis - parentBody.Radius;
                }
                else if (periapsis == null && periapsisAltitude != null)
                {
                    periapsis = periapsisAltitude + parentBody.Radius;
                }
            }
        }
    }

    public class OrbitalElementExecption : Exception
    {
        public OrbitalElementExecption() : base("Unknown issue with the orbital elements.") { }
        public OrbitalElementExecption(string message) : base(message) { }
        public OrbitalElementExecption(string message, Exception inner) : base(message, inner) { }
    }
}
