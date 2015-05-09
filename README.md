# Simple Orbit Calculator
Simple Orbit Calculator (aka SOC) is an in-game calculator for Kerbal Space Program to calculate basic orbital elements from pre-determined inputs.

### Compatibilities
The plugin pulls all celestial bodies known to KSP during runtime of the game; therefore, no hardcoding of the bodies and their physical/orbital characteristics are used. This allows the plugin to work with mods that alter the Solar System on load, such as Real Solar System or plantary packs like Outer Planets Mod.

### Incompatibilities
This plugin could possibly give false results when using a mod that can alter the celestial bodies during gameplay, such as HyperEdit. Use with caution.

### How to use the orbit calculator
1. Select the celestial body on the left.
2. To the right of the celestial bodies, select two orbital elements (you cannot select both semi-major axis and orbital period; it would create an ambiguous case). The other elements will be disabled from input at this point.
3. As an option, you can use either apoapsis/periapsis as altitudes above sea level of parent body or as distances from the center of the parent body. Just toggle the *Use Altitudes for Apsides* option.
4. Enter your target values. The *S* button on the same row of the orbital period will set the orbital period to the parent body's rotation period (useful for synchronous orbits).
5. Click the *Calculate* button, which will fill in the other elements and display below more in-depth information of the calculated orbit. Any errors will be reported.
6. Optionally, if you are currently in a flight scene, you can click the *Use Current Orbit* button to use your current orbit or click the *Use Target Orbit* to use your target's orbit.

Note: Because the calculation will fill in other elements that you did not choose, you will be free to use those easily in other calculations. Just de-select the elements you do not need, and select the new ones. This is very useful for calculating Hohmann transfers.

### How to use the Hohmann transfer calculator
1. Once you have calculated an orbit above, click either *Save as Orbit 1* or *Save as Orbit 2*.
2. When two orbits are saved that have the same parent body, the delta-v for the transfer will be calculated and displayed below.
3. Click the *C* buttons to clear their respective orbits.

### Current features
* Will automatically detect all celestial bodies known to KSP and their properties, including changes/additions by RSS or Planet Factory like mods.
* Allowed inputs include: Apoapsis, Periapsis, Semi-Major Axis, Eccentricity, Orbital Period.
* Other information displayed after calculation: Orbital Speeds at Apoapsis and Periapsis, Mean Orbital Speed, Sphere of Influence of Parent Body, Max Darkness Length (useful for Remote Tech satellite electric charge calculations).
* Can use your current vessel's or target's orbit as input.
* Can save two orbits to calculate the delta-v required for a Hohmann transfer.
* Blizzy's Toolbar support.

### Upcoming features
* A better UI!... still.
* Possibly more input values.
* Ability to save orbits for re-use across saves/installs.
* Setting to force stock toolbar over Blizzy's.

### Will not implement
* Interplanetary transfers (there already exists such a mod: [Transfer Window Planner](http://forum.kerbalspaceprogram.com/threads/93115)).
* Plane change calculations: somewhat out of scope of what the mod is intended for.

### License
This plugin is released under the GNU General Public License: http://www.gnu.org/licenses/.

### Changelog
5/9/2015 - **v.1.3.0** *Dangerous Doughnut* - While in flight scenes, can use your target's orbit as input. Blizzy's Toolbar support. Calculations, inputs and window location now persist between window closings and scene changes.

5/3/2015 - **v1.2.0** *Cowardly Cheddar* - Can save calculated orbits to calculate the Hohmann transfer delta-v. While in flight scenes, can use your active vessel's orbit as input. Code cleanup for the AppLauncher stuff (thanks for the help stupid_chris!). Code cleanup overall.

4/27/2015 - **v1.1.1** - KSP 1.0 compatibility update.

4/23/2015 - **v1.1.0** *Beautiful Biscuit* - Darkness time is now the longest amount of time that could be possibly spent in darkness (was originally assuming a circular orbit). Removed restrictions regarding hyperbolic trajectories (may result in some weird output numbers). Some code cleanup to prepare for future input values.

4/19/2105 - **v1.0.0** *Allergic Applesauce* - Initial release for KSP 0.90.
