using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EngineEnhancement
{
    public class EngineAnimation : PartModule
    {
        // Are we going to play the animation in reverse?
        [KSPField(isPersistant = true)]
        private bool reverseAnimation = false;

        [KSPField(isPersistant = true)]
        public string onActivateAnimation = null;

        [KSPField(isPersistant = true)]
        public float customAnimationSpeed = 1.0f;

        private bool playReverseAnimation = false;
        private bool enginesFound = false;
        private bool initialized = false;
        private Animation engineAnimation = null;

        private MultiModeEngine mmeModule = null;
        private List<ModuleEngines> engineModules = new List<ModuleEngines>();
        private List<ModuleEnginesFX> engineFxModules = new List<ModuleEnginesFX>();

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

        private bool NeedUpdateAnimation()
        {
            bool engineIgnited = false;
            if (mmeModule != null)
            {
                string activeEngine = (mmeModule.runningPrimary) ? mmeModule.primaryEngineID : mmeModule.secondaryEngineID;

                foreach (ModuleEnginesFX engine in engineFxModules)
                {
                    if (engine.name == activeEngine && engine.EngineIgnited)
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
                    //Debug.Log("EMME - Need Update: engineShutdown = " + engine.engineShutdown + );
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

            return (playReverseAnimation == (reverseAnimation ^ engineIgnited));
        }

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);

            FindEngines();

            if (enginesFound && !initialized)
            {
                initialized = true;
                if (onActivateAnimation != null)
                {
                    engineAnimation = part.FindModelAnimators(onActivateAnimation).FirstOrDefault();
                }
            }

            if (engineAnimation != null)
            {
                // Set the initial state of the animation
                playReverseAnimation = (engineAnimation[onActivateAnimation].normalizedTime < 0.001f) ^ reverseAnimation;
                // Unfortunately, the side effect is that the animation plays on start if it's out of whack.
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            FindEngines();

            if (enginesFound && engineAnimation != null && NeedUpdateAnimation())
            {
                playReverseAnimation = !playReverseAnimation;

                UpdateAnimation();
            }
        }

        private void UpdateAnimation()
        {
            if (playReverseAnimation)
            {
                engineAnimation[onActivateAnimation].speed = -1f * customAnimationSpeed;
                if (engineAnimation[onActivateAnimation].normalizedTime == 0f || engineAnimation[onActivateAnimation].normalizedTime == 1f)
                {
                    engineAnimation[onActivateAnimation].normalizedTime = 1f;
                }
            }
            else
            {
                engineAnimation[onActivateAnimation].speed = 1f * customAnimationSpeed;
                if (engineAnimation[onActivateAnimation].normalizedTime == 0f || engineAnimation[onActivateAnimation].normalizedTime == 1f)
                {
                    engineAnimation[onActivateAnimation].normalizedTime = 0f;
                }
            }

            engineAnimation.Play(onActivateAnimation);
        }
    }
}
