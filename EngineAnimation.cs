using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EngineEnhancement
{
    public class EngineAnimation : PartModule
    {
        private class SpinnerTransformation
        {
            public readonly Transform transform;
            public readonly string transformName;
            public readonly float spinRPM;
            private readonly int layer;

            public SpinnerTransformation(ConfigNode node, Part part, int _layer)
            {
                layer = _layer;
                if (node.HasValue("transformName"))
                {
                    transformName = node.GetValue("transformName").Trim();
                }
                else
                {
                    transformName = null;
                }

                spinRPM = 0.0f;
                if (node.HasValue("spinRPM"))
                {
                    float.TryParse(node.GetValue("spinRPM").Trim(), out spinRPM);
                }

                if (transformName != null)
                {
                    transform = part.FindModelTransform(transformName);
                    if(transform == null)
                    {
                        Debug.LogError("Did not find spinner transform " + transformName);
                    }
                }
                else
                {
                    transform = null;
                }
            }

            public void UpdateThrottle(float throttle)
            {
                // How many degrees did we turn?
                float rotation = spinRPM * throttle * TimeWarp.deltaTime * 360.0f;
                transform.Rotate(Vector3.forward * rotation);
            }
        }

        // Are we going to play the animation in reverse?
        [KSPField]
        public string onActivateAnimation = null;

        [KSPField]
        private bool reverseActivationAnimation = false;

        [KSPField]
        public float customActivationAnimationSpeed = 1.0f;

        [KSPField]
        public string onThrottleAnimation = null;

        [KSPField]
        private bool smoothThrottleAnimation = true;

        [KSPField]
        public float throttleResponseSpeed = 1.0f;

        private bool playReverseActivationAnimation = false;
        private bool enginesFound = false;
        private bool initialized = false;
        private Animation engineActivationAnimation = null;
        private Animation engineThrottleAnimation = null;

        private MultiModeEngine mmeModule = null;
        private List<ModuleEngines> engineModules = new List<ModuleEngines>();
        private List<ModuleEnginesFX> engineFxModules = new List<ModuleEnginesFX>();

        private List<SpinnerTransformation> spinnerAnimations = new List<SpinnerTransformation>();

        //--- Hacktastic!  In order to keep the child ConfigNodes around so I
        // can use them when I need them, I have to stuff a copy of my config
        // node in here.
        private static Dictionary<string, ConfigNode> configNodes = new Dictionary<string, ConfigNode>();

        //--------------------------------------------------------------------
        // FindEngines
        private void FindEngines()
        {
            if (!enginesFound && part != null)
            {
                foreach (PartModule thatModule in part.Modules)
                {
                    var mme = thatModule as MultiModeEngine;
                    if (mme != null)
                    {
                        mmeModule = mme;
                    }
                    var em = thatModule as ModuleEngines;
                    if(em != null)
                    {
                        engineModules.Add(em);
                    }
                    var efm = thatModule as ModuleEnginesFX;
                    if(efm != null)
                    {
                        engineFxModules.Add(efm);
                    }
                }

                enginesFound = true;
            }
        }

        //--------------------------------------------------------------------
        // NeedUpdateAnimation
        private bool NeedUpdateAnimation()
        {
            bool engineIgnited = false;
            if (mmeModule != null)
            {
                string activeEngine = (mmeModule.runningPrimary) ? mmeModule.primaryEngineID : mmeModule.secondaryEngineID;

                foreach (ModuleEnginesFX engine in engineFxModules)
                {
                    if (engine.engineID == activeEngine && engine.EngineIgnited)
                    {
                        engineIgnited = true;
                    }
                }
            }
            else
            {
                // What to do if there are multiple engine modules?  For now,
                // treat it as "any engine that's on".
                foreach (ModuleEngines engine in engineModules)
                {
                    if (engine.EngineIgnited)
                    {
                        engineIgnited = true;
                    }
                }

                foreach (ModuleEnginesFX engine in engineFxModules)
                {
                    if (engine.EngineIgnited)
                    {
                        engineIgnited = true;
                    }
                }
            }

            return (playReverseActivationAnimation == (reverseActivationAnimation ^ engineIgnited));
        }

        //--------------------------------------------------------------------
        // OnLoad
        public override void OnLoad(ConfigNode cfg)
        {
            base.OnLoad(cfg);

            if (!configNodes.ContainsKey(part.name))
            {
                // MOARdV: Hacktastic!  This method is called once, and it
                // does not seem to allow you to preserve variables you
                // initialize here, so when it comes time to instantiate a part
                // in the VAB, anything you set up here is lost.  But, you can
                // preserve the ConfigNode for later.
                configNodes.Add(part.name, cfg);
            }
        }

        //--------------------------------------------------------------------
        // OnStart
        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);

            Debug.Log("EE - OnStart");

            if (!configNodes.ContainsKey(part.name))
            {
                Debug.LogError("EE - myConfigNode is unset");
                return;
            }
            ConfigNode myConfigNode = configNodes[part.name];

            if (!initialized)
            {
                FindEngines();
                if (enginesFound)
                {
                    if (!string.IsNullOrEmpty(onActivateAnimation))
                    {
                        engineActivationAnimation = part.FindModelAnimators(onActivateAnimation).FirstOrDefault();
                        if (engineActivationAnimation == null)
                        {
                            Debug.LogError("EE - did not find animation " + onActivateAnimation);
                        }
                    }

                    if (!string.IsNullOrEmpty(onThrottleAnimation))
                    {
                        engineThrottleAnimation = part.FindModelAnimators(onThrottleAnimation).FirstOrDefault();
                        if (engineThrottleAnimation == null)
                        {
                            Debug.LogError("EE - did not find animation " + onThrottleAnimation);
                        }
                    }

                    var spinnerNodes = myConfigNode.GetNodes("ThrottleSpinner");
                    int layer = 4;
                    foreach (ConfigNode spinnerNode in spinnerNodes)
                    {
                        SpinnerTransformation spinner = new SpinnerTransformation(spinnerNode, part, layer);
                        if(spinner.transform != null)
                        {
                            ++layer;
                            spinnerAnimations.Add(spinner);
                        }
                    }

                    initialized = true;
                }
            }

            if (HighLogic.LoadedSceneIsFlight)
            {
                if (engineActivationAnimation != null)
                {
                    // Set the initial state of the animation
                    playReverseActivationAnimation = (engineActivationAnimation[onActivateAnimation].normalizedTime < 0.001f) ^ reverseActivationAnimation;
                    // Unfortunately, the side effect is that the animation plays on start if it's out of whack.
                    engineActivationAnimation[onActivateAnimation].layer = 2;
                }

                if (engineThrottleAnimation != null)
                {
                    engineThrottleAnimation[onThrottleAnimation].normalizedTime = vessel.ctrlState.mainThrottle;
                    engineThrottleAnimation[onThrottleAnimation].speed = 0f;
                    engineThrottleAnimation[onThrottleAnimation].wrapMode = WrapMode.ClampForever;
                    engineThrottleAnimation[onThrottleAnimation].layer = 3;
                }
            }
        }

        //--------------------------------------------------------------------
        // OnUpdate
        public override void OnUpdate()
        {
            base.OnUpdate();
            if (!HighLogic.LoadedSceneIsFlight)
            {
                return;
            }

            UpdateAnimation();
        }

        //--------------------------------------------------------------------
        // UpdateAnimation
        private void UpdateAnimation()
        {
            if (engineActivationAnimation != null && NeedUpdateAnimation())
            {
                playReverseActivationAnimation = !playReverseActivationAnimation;

                if (playReverseActivationAnimation)
                {
                    engineActivationAnimation[onActivateAnimation].speed = -1f * customActivationAnimationSpeed;
                    if (engineActivationAnimation[onActivateAnimation].normalizedTime == 0f || engineActivationAnimation[onActivateAnimation].normalizedTime == 1f)
                    {
                        engineActivationAnimation[onActivateAnimation].normalizedTime = 1f;
                    }
                }
                else
                {
                    engineActivationAnimation[onActivateAnimation].speed = 1f * customActivationAnimationSpeed;
                    if (engineActivationAnimation[onActivateAnimation].normalizedTime == 0f || engineActivationAnimation[onActivateAnimation].normalizedTime == 1f)
                    {
                        engineActivationAnimation[onActivateAnimation].normalizedTime = 0f;
                    }
                }

                engineActivationAnimation.Play(onActivateAnimation);
            }

            if(engineThrottleAnimation != null)
            {
                float goalThrottleAnimation = vessel.ctrlState.mainThrottle;
                float throttlePosition;
                if(smoothThrottleAnimation && engineThrottleAnimation[onThrottleAnimation].normalizedTime != goalThrottleAnimation)
                {
                    throttlePosition = Mathf.Lerp(engineThrottleAnimation[onThrottleAnimation].normalizedTime, goalThrottleAnimation, throttleResponseSpeed);
                }
                else
                {
                    throttlePosition = goalThrottleAnimation;
                }

                if (!engineThrottleAnimation.IsPlaying(onThrottleAnimation))
                {
                    engineThrottleAnimation.Play(onThrottleAnimation);
                }
                engineThrottleAnimation[onThrottleAnimation].normalizedTime = throttlePosition;
            }

            foreach(var spinner in spinnerAnimations)
            {
                spinner.UpdateThrottle(vessel.ctrlState.mainThrottle);
            }
        }
    }
}
