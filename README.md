# Simple Orbit Calculator
Simple Orbit Calculator (aka SOC) is an in-game calculator for Kerbal Space Program to calculate basic orbital elements from pre-determined inputs.

### Compatibilities
The plugin pulls all celestial bodies known to KSP during runtime of the game; therefore, no hardcoding of the bodies and their physical/orbital characteristics are used. This allows the plugin to work with mods that alter the Solar System on load, such as Real Solar System or plantary packs like Outer Planets Mod.

### Incompatibilities
This plugin could possibly give false results when using a mod that can alter the celestial bodies during gameplay, such as HyperEdit. Use with caution.

### How to use
1. Select the celestial body on the left.
2. To the right of the celestial bodies, select two orbital elements (you cannot select both semi-major axis and orbital period; it would create an ambiguous case). The other elements will be disabled from input at this point.
3. As an option, you can use either apoapsis/periapsis as altitudes above sea level of parent body or as distances from the center of the parent body. Just toggle the *Use Altitudes for Apsides* option near the bottom under Options.
4. Enter your target values.
5. Click the *Calculate* button, which will fill in the other elements and display below more in-depth information of the calculated orbit. Any errors will be reported.

Note: Because the calculation will fill in other elements that you did not choose, you will be free to use those easily in other calculations. Just de-select the elements you do not need, and select the new ones. This is very useful for calculating Hohmann transfers.

### Current features
* Will automatically detect all celestial bodies known to KSP and their properties, including changes/additions by RSS or Planet Factory like mods.
* Allowed inputs include: Apoapsis, Periapsis, Semi-Major Axis, Eccentricity, Orbital Period.
* Other information displayed after calculation: Orbital Speeds at Apoapsis and Periapsis, Mean Orbital Speed, Sphere of Influence of Parent Body, Mean Darkness Length (useful for Remote Tech satellite electric charge calculations).

### Upcoming features
* Possibly more input values.
* Ability to save two orbits around the same parent body, and calculate the delta-V required for the Hohmann transfer.

### Will not implement
* Interplanetary transfers (there already exists such a mod: [Transfer Window Planner](http://forum.kerbalspaceprogram.com/threads/93115)).
* Plane change calculations: somewhat out of scope of what the mod is intended for.
