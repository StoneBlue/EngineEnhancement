EngineEnhancement
=================

A small plugin for Kerbal Space Program to augment stock engine behaviors.

##OVERVIEW
The EngineEnhancement plugin provides a small PartModule that can be added to an engine part in KSP.

###EngineAnimation
Allows animations to be played when an engine is activated or shut down, and when throttle settings change.  This can be used, for instance, to extend an exhaust nozzle on upper stage engines, spin pumps, and move other details on the engine.

- `onActivateAnimation`: (Optional, string) The name of the animation to play.
- `reverseActivationAnimation`: (Optional, bool, default false) Should the animation be played in reverse?
- `customActivationAnimationSpeed`: (Optional, float, default 1.0) How quickly should the animation play?  1.0 is the default speed.

##LICENSE:
This work is licensed under a [Creative Commons Attribution-Share Alike 4.0 International License](http://creativecommons.org/licenses/by-sa/4.0/).

##SOURCE CODE:
Source code and bug tracking can be found at GitHub:
[https://github.com/MOARdV/EngineEnhancement](https://github.com/MOARdV/EngineEnhancement)

Pull requests welcome.

##KNOWN ISSUES
* None at this time

##CHANGES

###14 April 2016 - v0.7
* KSP 1.1 release.  Removed all functionality except the `onActivateAnimation` behavior.

###29 April 2015 - v0.5
* KSP 1.0 bandage release (GimbalHider feature is broken).

###3 December 2014 - v0.4
* Initial public alpha release.

###Older
* Internal development versions
