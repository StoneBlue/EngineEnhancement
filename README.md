EngineEnhancement
=================

A small plugin for Kerbal Space Program to augment stock engine behaviors.

##OVERVIEW
The EngineEnhancement plugin provides small PartModules that can be added to an engine part in KSP.

###EngineAnimation
Allows animations to be played when an engine is activated or shut down, and when throttle settings change.  This can be used, for instance, to extend an exhaust nozzle on upper stage engines, spin pumps, and move other details on the engine.

- `onActivateAnimation`: (Optional, string) The name of the animation to play.
- `reverseAnimation`: (Optional, bool, default false) Should the animation be played in reverse?
- `customAnimationSpeed`: (Optional, float, default 1.0) How quickly should the animation play?  1.0 is the default speed.
- `onThrottleAnimation`: (Optional, string) The name of an animation that changes based on throttle position.
- `smoothThrottleAnimation`: (Optional, bool, default true) Whether the onThrottleAnimation should smoothly move between positions (when true), or snap to position (when false).
- `throttleResponseSpeed`: (Optional, float, default 1.0) Controls how quickly the onThrottleAnimation moves to its new position.

####ThrottleSpinner
An optional child node in EngineAnimation, a SpinnerTransformation rotates a transform based on throttle position and a designated speed.  More than one SpinnerTransformation may be in a single EngineAnimation module.  The designated transform is spun around its Y axis.
- `transformName`: (Required, string) The name of the transform to spin.
- `spinRPM`: (Required, float) The number of rotations per minute for the designated spinner.

>     MODULE
>     {
>        name = EngineAnimation
>     
>        onActivateAnimation = extend_nozzle
>        customAnimationSpeed = 1.0
>        reverseAnimation = false
>        
>        onThrottleAnimation = exhaust_feathers
>
>        ThrottleSpinner
>        {
>          transformName = fuel_pump_shaft
>          spinRPM = 125.0
>        }
>     }

###AxisRotationCopy
The AxisRotationCopy module copies one of the local Euler angles of a transformation to another transform (or transforms).  This allows model components to be moved based on gimbal positions, for instance.  Multiple transforms can be listed, and all transforms with the destination transform's name in the model will be updated.  The axis from the source transform is identified by adding `#pitch`, `#yaw`, or `#roll` to the end of the transform's name (eg. `engine_gimbal#pitch`).

-`transformName`: (Required, string) A comma-delimited list of (source transform, destination transform) pairs.

>     MODULE
>     {
>       name = AxisRotationCopy
>     
>       transformName = engine_gimbal#pitch, pitch_arms,engine_gimbal#yaw, yaw_arms
>     }

###EngineHider
The EngineHider module hides a ModuleEngines or ModuleEnginesFX's tweakable menu entries.  Engine settings (shutdown/active) are copied from a different specified engine.  This allows for two separate engine modules in a part without cluttering the tweakable menu.

- `hiddenEngine`: (Required, string) The `thrustVectorTransformName` of the engine to hide.

###GimbalHider
The GimbalHider module hides all ModuleGimbal instances in a part except for the first one.  All hidden gimbals are locked/freed when the visible gimbal is locked/freed.

##LICENSE:
This work is licensed under a [Creative Commons Attribution-Share Alike 4.0 International License](http://creativecommons.org/licenses/by-sa/4.0/).

##SOURCE CODE:
Source code and bug tracking can be found at GitHub:
[https://github.com/MOARdV/EngineEnhancement](https://github.com/MOARdV/EngineEnhancement)

Pull requests welcome.

##KNOWN ISSUES
* None at this time

##CHANGES

###(unreleased) - v0.3

###(unreleased) - v0.2

###(unreleased) - v0.1
