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
        private readonly double semiMinorAxis;
        private readonly double semiLatusRectum;
        private readonly double focalParameter;
        private readonly double linearEccentricity;
        private readonly double apoapsis;
        private readonly double periapsis;
        private readonly double apoapsisAltitude;
        private readonly double periapsisAltitude;
        private readonly double orbitalPeriod;
        private readonly double apoapsisSpeed;
        private readonly double periapsisSpeed;
        private readonly double meanOrbitalSpeed;
        private readonly double specificOrbitalEnergy;
        private readonly double specificAngularMomentum;
        private readonly double meanDarknessTime;
        private readonly double maxDarknessTime;
        
        public CelestialBody ParentBody { get { return parentBody; }}
        public double SemiMajorAxis { get { return semiMajorAxis; }}
        public double Eccentricity { get { return eccentricity; } }
        public double SemiMinorAxis { get { return semiMinorAxis; } }
        public double SemiLatusRectum { get { return semiLatusRectum; } }
        public double FocalParameter { get { return focalParameter; } }
        public double LinearEccentricity { get { return linearEccentricity; } }
        public double Apoapsis { get { return apoapsis; } }
        public double Periapsis { get { return periapsis; } }
        public double ApoapsisAltitude { get { return apoapsisAltitude; } }
        public double PeriapsisAltitude { get { return periapsisAltitude; } }
        public double OrbitalPeriod { get { return orbitalPeriod; } }
        public double ApoapsisSpeed { get { return apoapsisSpeed; } }
        public double PeriapsisSpeed { get { return periapsisSpeed; } }
        public double MeanOrbitalSpeed { get { return meanOrbitalSpeed; } }
        public double SpecificOrbitalEnergy { get { return specificOrbitalEnergy; } }
        public double SpecificAngularMomentum { get { return specificAngularMomentum; } }
        public double MaxDarknessTime { get { return maxDarknessTime; } }
        public double MeanDarknessTime { get { return meanDarknessTime; } }

        /// <summary>
        /// ScalerType are the various scalers used by the orbit elements.
        /// This is mostly used for string formatting purposes.
        /// </summary>
        public enum ScalerType { Distance, Speed, Time, SpecificEnergy };

        internal SimpleOrbit(CelestialBody parentBody, double semiMajorAxis, double eccentricity)
        {
            this.parentBody = parentBody;
            this.semiMajorAxis = semiMajorAxis;
            this.eccentricity = eccentricity;

            // Ellipse features
            semiMinorAxis = semiMajorAxis * Math.Sqrt(1.0 - Math.Pow(eccentricity, 2.0));
            semiLatusRectum = Math.Pow(semiMinorAxis, 2.0) / semiMajorAxis;
            focalParameter = semiLatusRectum / eccentricity;
            linearEccentricity = semiMajorAxis * eccentricity;

            // Apside calculations
            apoapsis = semiMajorAxis * (1.0 + eccentricity);
            periapsis = semiMajorAxis * (1.0 - eccentricity);
            apoapsisAltitude = apoapsis - parentBody.Radius;
            periapsisAltitude = periapsis - parentBody.Radius;

            // Time calculations
            orbitalPeriod = 2.0 * Math.PI * Math.Sqrt(Math.Pow(semiMajorAxis, 3.0) / parentBody.gravParameter);

            // Speed calculations
            apoapsisSpeed = Math.Sqrt(parentBody.gravParameter * (2.0 / apoapsis - 1.0 / semiMajorAxis));
            periapsisSpeed = Math.Sqrt(parentBody.gravParameter * (2.0 / periapsis - 1.0 / semiMajorAxis));
            meanOrbitalSpeed = Math.Sqrt(parentBody.gravParameter / semiMajorAxis) * (
                1.0 - 1.0 / 4.0 * Math.Pow(eccentricity, 2.0)
                - 3.0 / 64.0 * Math.Pow(eccentricity, 4.0)
                - 5.0 / 256.0 * Math.Pow(eccentricity, 6.0)
                - 175.0 / 16384.0 * Math.Pow(eccentricity, 8.0));
            
            // Misc. calculations
            specificOrbitalEnergy = -parentBody.gravParameter / (2.0 * semiMajorAxis);
            specificAngularMomentum = Math.Sqrt(semiLatusRectum * parentBody.gravParameter);
            meanDarknessTime = orbitalPeriod * Math.Asin(parentBody.Radius / semiMajorAxis) / Math.PI;
            maxDarknessTime = 2.0 * semiMajorAxis * semiMinorAxis * (Math.Asin(parentBody.Radius / semiMinorAxis) + eccentricity * parentBody.Radius / semiMinorAxis) / specificAngularMomentum;
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool userApsideAltitude)
        {
            string periapsisText, apoapsisText;
            if (userApsideAltitude)
            {
                periapsisText = GUIUtilities.ParseOrbitElement(PeriapsisAltitude, ScalerType.Distance);
                apoapsisText = GUIUtilities.ParseOrbitElement(ApoapsisAltitude, ScalerType.Distance);
            }
            else
            {
                periapsisText = GUIUtilities.ParseOrbitElement(Periapsis, ScalerType.Distance);
                apoapsisText = GUIUtilities.ParseOrbitElement(Apoapsis, ScalerType.Distance);
            }
            return parentBody.name + ": " + periapsisText + " x " + apoapsisText;
        }

        public static double CalculateHohmannTransferDeltaV(SimpleOrbit orbit1, SimpleOrbit orbit2)
        {
            SimpleOrbit lowerEnergyOrbit, higherEnergyOrbit, transferOrbit;
            double deltaV = 0.0;

            if (orbit1.ParentBody != orbit2.ParentBody)
            {
                throw new ArgumentException("Input orbits must have same parent body.");
            }

            if (orbit1.SpecificOrbitalEnergy < orbit2.SpecificOrbitalEnergy)
            {
                lowerEnergyOrbit = orbit1;
                higherEnergyOrbit = orbit2;
            }
            else
            {
                lowerEnergyOrbit = orbit2;
                higherEnergyOrbit = orbit1;
            }

            SimpleOrbitBuilder transferOrbitBuilder = new SimpleOrbitBuilder(orbit1.ParentBody);
            transferOrbitBuilder.SetPeriapsis(lowerEnergyOrbit.Periapsis);
            transferOrbitBuilder.SetApoapsis(higherEnergyOrbit.Periapsis);
            transferOrbit = transferOrbitBuilder.Build();

            deltaV += Math.Abs(transferOrbit.PeriapsisSpeed - lowerEnergyOrbit.PeriapsisSpeed);
            deltaV += Math.Abs(higherEnergyOrbit.PeriapsisSpeed - transferOrbit.ApoapsisSpeed);

            return deltaV;
        }
    }

    public class SimpleOrbitBuilder
    {
        private const double EccentricityEpsilon = 0.00001;

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
            CalculateOneToOneElements();
            CalculateRemainingElements();
            CleanEccentricityPrecision();
            ValidateAllElements();
            return new SimpleOrbit(parentBody, semiMajorAxis, eccentricity);
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
            if (eccentricity < 0)
            {
                throw new OrbitalElementExecption("Resulting eccentricity cannot be less than 0.");
            }

            /*if (apoapsis > parentBody.sphereOfInfluence)
            {
                throw new OrbitalElementExecption("Apoapsis located outside of SOI.");
            }*/
        }

        private void CleanEccentricityPrecision()
        {
            if (Math.Abs(eccentricity) <= EccentricityEpsilon)
            {
                eccentricity = 0.0;
            }
        }

        private void CalculateOneToOneElements()
        {
            if (inputElements.Contains("orbitalPeriod"))
            {
                semiMajorAxis = Math.Pow(parentBody.gravParameter * Math.Pow(orbitalPeriod / (2.0 * Math.PI), 2.0), 1.0 / 3.0);
                inputElements.Add("semiMajorAxis");
            }
        }

        private void CalculateRemainingElements()
        {
            // We already have what we need if semi-major axis and eccentricity are set
            if (inputElements.Contains("semiMajorAxis") && inputElements.Contains("eccentricity"))
            {
                return;
            }

            // Apoapsis and Periapsis
            if (inputElements.Contains("apoapsis") && inputElements.Contains("periapsis"))
            {
                semiMajorAxis = (apoapsis + periapsis) / 2.0;
                eccentricity = (apoapsis - periapsis) / (apoapsis + periapsis);
            }
            // Apoapsis and Semi-Major Axis
            else if (inputElements.Contains("apoapsis") && inputElements.Contains("semiMajorAxis"))
            {
                eccentricity = apoapsis / semiMajorAxis - 1.0;
            }
            // Periapsis and Semi-Major Axis
            else if (inputElements.Contains("periapsis") && inputElements.Contains("semiMajorAxis"))
            {
                eccentricity = 1.0 - periapsis / semiMajorAxis;
            }
            // Eccentricity and Apoapsis
            else if (inputElements.Contains("apoapsis") && inputElements.Contains("eccentricity"))
            {
                semiMajorAxis = apoapsis / (1.0 + eccentricity);
            }
            // Eccentricity and Periapsis
            else if (inputElements.Contains("periapsis") && inputElements.Contains("eccentricity"))
            {
                semiMajorAxis = periapsis / (1.0 - eccentricity);
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
