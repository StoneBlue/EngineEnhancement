EngineEnhancement
=================

A small plugin for Kerbal Space Program to augment stock engine behaviors.

##OVERVIEW
The EngineEnhancement plugin provides small PartModules that can be added to an engine part in KSP.

###EngineAnimation
Allows an animation to be played when an engine is activated or shut down.  This can be used, for instance, to extend an exhaust nozzle on upper stage engines.

- `onActivateAnimation`: (Required, string) The name of the animation to play.
- `reverseAnimation`: (Optional, bool, default false) Should the animation be played in reverse?
- `customAnimationSpeed`: (Optional, float, default 1.0) How quickly should the animation play?  1.0 is the default speed.

>     MODULE
>     {
>        name = EngineAnimation
>     
>        onActivateAnimation = extend_nozzle
>        customAnimationSpeed = 1.0
>        reverseAnimation = false
>     }

###ModuleRestrictedGimbal
The ModuleRestrictedGimbal module is a superset of the stock ModuleGimbal.  This module adds the ability to restrict gimbal movement by allowing the config file editor to set independent values for pitchRange, yawRange, and rollRange.  When this module runs, the gimbal effect along each axis is controlled by the smaller of `gimbalRange` or the appropriate restricted range.  Note that this means you must set `gimbalRange` in addition to these values.

-`pitchRange`: (Optional, float, default 90.0) Used to limit pitch gimbal.
-`rollRange`: (Optional, float, default 90.0) Used to limit roll gimbal.
-`yawRange`: (Optional, float, default 90.0) Used to limit yaw gimbal.

>     MODULE
>     {
>       name = ModuleRestrictedGimbal
>     
>       gimbalTransformName = engine_gimbal
>       gimbalRange = 4
>       useGimbalResponseSpeed = True
>       gimbalResponseSpeed = 1
>       
>       pitchRange = 2
>       rollRange = 0
>       yawRange = 4
>     }

##LICENSE:
This work is licensed under a [Creative Commons Attribution-Share Alike 4.0 International License](http://creativecommons.org/licenses/by-sa/4.0/).

##SOURCE CODE:
Source code and bug tracking can be found at GitHub:
[https://github.com/MOARdV/EngineEnhancement](https://github.com/MOARdV/EngineEnhancement)

Pull requests welcome.

##KNOWN ISSUES
* None at this time

##CHANGES

###(pending) - v0.2
* ModuleRestrictedGimbal added

###(unreleased) - v0.1
* EngineAnimator module added
 
