using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleOrbitCalculator
{
    public class SimpleOrbit
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

        internal SimpleOrbit(CelestialBody parentBody, double semiMajorAxis, double eccentricity, double apoapsis,
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

        
    }

    public class SimpleOrbitBuilder
    {
        private readonly CelestialBody parentBody;
        private List<string> inputElements = new List<string>();
        private double semiMajorAxis;
        private double eccentricity;
        private double apoapsis;
        private double periapsis;
        private double orbitalPeriod;
        private double apoapsisAltitude;
        private double periapsisAltitude;

        public SimpleOrbitBuilder(CelestialBody parentBody)
        {
            this.parentBody = parentBody;
        }

        public SimpleOrbit Build()
        {
            ValidateElements();
            CalculateRemainingElements();
            return new SimpleOrbit(parentBody, semiMajorAxis, eccentricity, apoapsis, periapsis, orbitalPeriod, apoapsisAltitude, periapsis);
        }

        public SimpleOrbitBuilder SetSemiMajorAxis(double semiMajorAxis)
        {
            this.semiMajorAxis = semiMajorAxis;
            inputElements.Add("semiMajorAxis");
            return this;
        }

        public SimpleOrbitBuilder SetEccentricity(double eccentricity)
        {
            this.eccentricity = eccentricity;
            inputElements.Add("eccentricity");
            return this;
        }

        public SimpleOrbitBuilder SetApoapsis(double apoapsis)
        {
            this.apoapsis = apoapsis;
            inputElements.Add("apoapsis");
            return this;
        }

        public SimpleOrbitBuilder SetPeriapsis(double periapsis)
        {
            this.periapsis = periapsis;
            inputElements.Add("periapsis");
            return this;
        }

        public SimpleOrbitBuilder SetOrbitalPeriod(double orbitalPeriod)
        {
            this.orbitalPeriod = orbitalPeriod;
            inputElements.Add("orbitalPeriod");
            return this;
        }

        public SimpleOrbitBuilder SetApoapsisAltitude(double apoapsisAltitude)
        {
            this.apoapsis = apoapsisAltitude + parentBody.Radius;
            inputElements.Add("apoapsis");
            return this;
        }

        public SimpleOrbitBuilder SetPeriapsisAltitude(double periapsisAltitude)
        {
            this.periapsis = periapsisAltitude + parentBody.Radius;
            inputElements.Add("periapsis");
            return this;
        }

        private void ValidateElements()
        {
            if (inputElements.Count != 2)
            {
                throw new OrbitalElementExecption("Two orbital elements are required to complete the calculation.");
            }

            if (inputElements.Contains("semiMajorAxis") && inputElements.Contains("orbitalPeriod"))
            {
                throw new OrbitalElementExecption("Semi-major-axis and orbital period creates an ambiguous case.");
            }
        }

        private void CalculateRemainingElements()
        {
            double gravParam = parentBody.gravParameter;

            if (!inputElements.Contains("orbitalPeriod"))
            {
                if (!inputElements.Contains("eccentricity"))
                {
                    // Apoapsis and Periapsis
                    if (inputElements.Contains("apoapsis") && inputElements.Contains("periapsis"))
                    {
                        semiMajorAxis = (apoapsis + periapsis) / 2.0;
                    }
                    // Apoapsis and Semi-Major-Axis
                    else if (inputElements.Contains("apoapsis") && inputElements.Contains("semiMajorAxis"))
                    {
                        periapsis = 2.0 * semiMajorAxis - apoapsis;
                    }
                    // Periapsis and Semi-Major-Axis
                    else if (inputElements.Contains("periapsis") && inputElements.Contains("semiMajorAxis"))
                    {
                        apoapsis = 2.0 * semiMajorAxis - periapsis;
                    }

                    eccentricity = (apoapsis - periapsis) / (apoapsis + periapsis);
                }
                else
                {
                    // Eccentricity and Apoapsis
                    if (inputElements.Contains("apoapsis"))
                    {
                        periapsis = apoapsis * (1.0 - eccentricity) / (1.0 + eccentricity);
                    }
                    // Eccentricity and Periapsis
                    else if (inputElements.Contains("periapsis"))
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
                if (inputElements.Contains("eccentricity"))
                {
                    periapsis = semiMajorAxis * (1.0 - eccentricity);
                    apoapsis = semiMajorAxis * (1.0 + eccentricity);
                }
                else
                {
                    // Period and Apoapsis
                    if (inputElements.Contains("apoapsis"))
                    {
                        periapsis = 2.0 * semiMajorAxis - apoapsis;
                    }
                    // Period and Periapsis
                    else if (inputElements.Contains("periapsis"))
                    {
                        apoapsis = 2.0 * semiMajorAxis - periapsis;
                    }

                    eccentricity = (apoapsis - periapsis) / (apoapsis + periapsis);
                }
            }

            apoapsisAltitude = apoapsis - parentBody.Radius;
            periapsisAltitude = periapsis - parentBody.Radius;
        }
    }

    public class OrbitalElementExecption : Exception
    {
        public OrbitalElementExecption() : base("Unknown issue with the orbital elements.") { }
        public OrbitalElementExecption(string message) : base(message) { }
        public OrbitalElementExecption(string message, Exception inner) : base(message, inner) { }
    }
}
