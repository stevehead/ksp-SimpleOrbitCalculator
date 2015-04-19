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
        private readonly double apoapsisSpeed;
        private readonly double periapsisSpeed;
        private readonly double meanOrbitalSpeed;
        private readonly double meanDarknessTime;
        private readonly double specificOrbitalEnergy;
        
        public CelestialBody ParentBody { get { return parentBody; }}
        public double SemiMajorAxis { get { return semiMajorAxis; }}
        public double Eccentricity { get { return eccentricity; } }
        public double Apoapsis { get { return apoapsis; } }
        public double Periapsis { get { return periapsis; } }
        public double OrbitalPeriod { get { return orbitalPeriod; } }
        public double ApoapsisAltitude { get { return apoapsisAltitude; } }
        public double PeriapsisAltitude { get { return periapsisAltitude; } }
        public double ApoapsisSpeed { get { return apoapsisSpeed; } }
        public double PeriapsisSpeed { get { return periapsisSpeed; } }
        public double MeanOrbitalSpeed { get { return meanOrbitalSpeed; } }
        public double MeanDarknessTime { get { return meanDarknessTime; } }
        public double SpecificOrbitalEnergy { get { return specificOrbitalEnergy; } }

        /// <summary>
        /// ScalerType are the various scalers used by the orbit elements.
        /// This is mostly used for string formatting purposes.
        /// </summary>
        public enum ScalerType { Distance, Speed, Time, SpecificEnergy };

        internal SimpleOrbit(CelestialBody parentBody, double semiMajorAxis, double eccentricity, double apoapsis, double periapsis, double orbitalPeriod)
        {
            this.parentBody = parentBody;
            this.semiMajorAxis = semiMajorAxis;
            this.eccentricity = eccentricity;
            this.apoapsis = apoapsis;
            this.periapsis = periapsis;
            this.orbitalPeriod = orbitalPeriod;
            this.apoapsisAltitude = apoapsis - parentBody.Radius;
            this.periapsisAltitude = periapsis - parentBody.Radius;
            this.apoapsisSpeed = Math.Sqrt(parentBody.gravParameter * (2.0 / apoapsis - 1.0 / semiMajorAxis));
            this.periapsisSpeed = Math.Sqrt(parentBody.gravParameter * (2.0 / periapsis - 1.0 / semiMajorAxis));
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
        private const double ApsidesMargin = 0.00001;

        private readonly CelestialBody parentBody;
        private List<string> inputElements = new List<string>();
        private double semiMajorAxis;
        private double eccentricity;
        private double apoapsis;
        private double periapsis;
        private double orbitalPeriod;

        public SimpleOrbitBuilder(CelestialBody parentBody)
        {
            this.parentBody = parentBody;
        }

        public SimpleOrbit Build()
        {
            ValidateInputElements();
            CalculateRemainingElements();
            CleanPrecisions();
            ValidateAllElements();
            return new SimpleOrbit(parentBody, semiMajorAxis, eccentricity, apoapsis, periapsis, orbitalPeriod);
        }

        public SimpleOrbitBuilder SetSemiMajorAxis(double semiMajorAxis)
        {
            this.semiMajorAxis = semiMajorAxis;
            inputElements.Add("semiMajorAxis");
            return this;
        }

        public SimpleOrbitBuilder SetSemiMajorAxis(string semiMajorAxis)
        {
            return SetSemiMajorAxis(Convert.ToDouble(semiMajorAxis));
        }

        public SimpleOrbitBuilder SetEccentricity(double eccentricity)
        {
            this.eccentricity = eccentricity;
            inputElements.Add("eccentricity");
            return this;
        }

        public SimpleOrbitBuilder SetEccentricity(string eccentricity)
        {
            return SetEccentricity(Convert.ToDouble(eccentricity));
        }

        public SimpleOrbitBuilder SetApoapsis(double apoapsis)
        {
            this.apoapsis = apoapsis;
            inputElements.Add("apoapsis");
            return this;
        }

        public SimpleOrbitBuilder SetApoapsis(string apoapsis)
        {
            return SetApoapsis(Convert.ToDouble(apoapsis));
        }

        public SimpleOrbitBuilder SetPeriapsis(double periapsis)
        {
            this.periapsis = periapsis;
            inputElements.Add("periapsis");
            return this;
        }

        public SimpleOrbitBuilder SetPeriapsis(string periapsis)
        {
            return SetPeriapsis(Convert.ToDouble(periapsis));
        }

        public SimpleOrbitBuilder SetOrbitalPeriod(double orbitalPeriod)
        {
            this.orbitalPeriod = orbitalPeriod;
            inputElements.Add("orbitalPeriod");
            return this;
        }

        public SimpleOrbitBuilder SetOrbitalPeriod(string orbitalPeriod)
        {
            return SetOrbitalPeriod(Convert.ToDouble(orbitalPeriod));
        }

        public SimpleOrbitBuilder SetApoapsisAltitude(double apoapsisAltitude)
        {
            this.apoapsis = apoapsisAltitude + parentBody.Radius;
            inputElements.Add("apoapsis");
            return this;
        }

        public SimpleOrbitBuilder SetApoapsisAltitude(string apoapsisAltitude)
        {
            return SetApoapsisAltitude(Convert.ToDouble(apoapsisAltitude));
        }

        public SimpleOrbitBuilder SetPeriapsisAltitude(double periapsisAltitude)
        {
            this.periapsis = periapsisAltitude + parentBody.Radius;
            inputElements.Add("periapsis");
            return this;
        }

        public SimpleOrbitBuilder SetPeriapsisAltitude(string periapsisAltitude)
        {
            return SetPeriapsisAltitude(Convert.ToDouble(periapsisAltitude));
        }

        private void ValidateInputElements()
        {
            if (inputElements.Count != 2)
            {
                throw new OrbitalElementExecption("Two orbital elements are required to complete the calculation.");
            }

            if (inputElements.Contains("semiMajorAxis") && inputElements.Contains("orbitalPeriod"))
            {
                throw new OrbitalElementExecption("Semi-major axis and orbital period creates an ambiguous case.");
            }
        }

        private void ValidateAllElements()
        {
            if (apoapsis < periapsis)
            {
                throw new OrbitalElementExecption("Apoapsis must be greater than or equal to the periapsis");
            }

            if (apoapsis < 0 || periapsis < 0)
            {
                throw new OrbitalElementExecption("Apsides must be greater than 0.");
            }

            if (eccentricity < 0 || eccentricity > 1)
            {
                throw new OrbitalElementExecption("Eccentricity must be between 0 and 1.");
            }

            if (semiMajorAxis <= 0)
            {
                throw new OrbitalElementExecption("Semi-major axis must be greater than 0.");
            }

            if (orbitalPeriod <= 0)
            {
                throw new OrbitalElementExecption("Orbital period must be greater than 0.");
            }

            if (apoapsis > parentBody.sphereOfInfluence)
            {
                throw new OrbitalElementExecption("Apoapsis located outside of SOI.");
            }
        }

        private void CleanPrecisions()
        {
            double allowedDifference = Math.Abs(periapsis * ApsidesMargin);

            // If apoapsis and periapsis are almost equal, treat them as equal
            if (Math.Abs(apoapsis - periapsis) <= allowedDifference)
            {
                double average = (periapsis + apoapsis) / 2;
                periapsis = average;
                apoapsis = average;
                semiMajorAxis = average;
                eccentricity = 0.0;
                orbitalPeriod = 2.0 * Math.PI * Math.Sqrt(Math.Pow(semiMajorAxis, 3.0) / parentBody.gravParameter);
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
                    // Apoapsis and Semi-Major Axis
                    else if (inputElements.Contains("apoapsis") && inputElements.Contains("semiMajorAxis"))
                    {
                        periapsis = 2.0 * semiMajorAxis - apoapsis;
                    }
                    // Periapsis and Semi-Major Axis
                    else if (inputElements.Contains("periapsis") && inputElements.Contains("semiMajorAxis"))
                    {
                        apoapsis = 2.0 * semiMajorAxis - periapsis;
                    }

                    eccentricity = (apoapsis - periapsis) / (apoapsis + periapsis);
                }
                else
                {
                    if (!inputElements.Contains("semiMajorAxis"))
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
                    // Eccentricity and Semi-Major Axis
                    else
                    {
                        periapsis = semiMajorAxis * (1.0 - eccentricity);
                        apoapsis = semiMajorAxis * (1.0 + eccentricity);
                    }
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
        }
    }

    public class OrbitalElementExecption : Exception
    {
        public OrbitalElementExecption() : base("Unknown issue with the orbital elements.") { }
        public OrbitalElementExecption(string message) : base(message) { }
        public OrbitalElementExecption(string message, Exception inner) : base(message, inner) { }
    }
}
