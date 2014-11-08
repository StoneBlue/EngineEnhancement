using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EngineEnhancement
{
    class ModuleVernierThruster:PartModule
    {
        [KSPField]
        public string thrustVectorTransformName = null;
        [KSPField]
        public string runningEffectName = null;

        [KSPField]
	    public float minThrust = 0.0f;
        [KSPField]
	    public float maxThrust = 0.0f;

        [KSPField]
        public float heatProduction = 0.0f;

        private float thrustNormalized = 0.0f;
        private float currentThrustNormalized = -1.0f;

        private Transform thrustTransform = null;

        private List<Propellant> propellants = new List<Propellant>();
        private FloatCurve ispCurve = null;
        // MOARdV TODO: Make this module detect MultiModeEngine, ModuleEngine
        private ModuleEnginesFX engine = null;

        //--- Hacktastic!  In order to keep the child ConfigNodes around so I
        // can use them when I need them, I have to stuff a copy of my config
        // node in here.
        public ConfigNode myConfigNode;

        //--------------------------------------------------------------------
        // GetInfo
        public override string GetInfo()
        {
            string infoString = string.Format("Max Thrust: {0:f1}kN", maxThrust);

            return infoString;
        }

        //--------------------------------------------------------------------
        // OnCenterOfThrustQuery
        // Let the editor know where our thrust transform is, so it can
        // adjust CoT.
        public virtual void OnCenterOfThrustQuery(CenterOfThrustQuery CoTquery)
        {
            if (thrustTransform != null)
            {
                CoTquery.pos = thrustTransform.position;
                CoTquery.dir = thrustTransform.forward;
                CoTquery.thrust = maxThrust;
            }
            else
            {
                CoTquery.pos = part.transform.position;
                CoTquery.dir = part.transform.forward;
                CoTquery.thrust = 0.0f;
            }
        }
        
        //--------------------------------------------------------------------
        // OnFixedUpdate
        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            if (!HighLogic.LoadedSceneIsFlight || !vessel.isActiveVessel || thrustTransform == null)
            {
                return; // early
            }

            float netThrust = thrustNormalized * maxThrust;

            // Early stab at it
            rigidbody.AddForceAtPosition(-thrustTransform.forward * netThrust, thrustTransform.position);

            // MOARdV TODO: Consume resources
            // MOARdV TODO: Apply heat
        }

        //--------------------------------------------------------------------
        // OnLoad
        public override void OnLoad(ConfigNode cfg)
        {
            base.OnLoad(cfg);

            if (myConfigNode == null)
            {
                // MOARdV: Hacktastic!  This method is called once, and it
                // does not seem to allow you to preserve variables you
                // initialize here, so when it comes time to instantiate a part
                // in the VAB, anything you set up here is lost.  But, you can
                // preserve the ConfigNode for later.
                myConfigNode = cfg;
            }
        }

        //--------------------------------------------------------------------
        // OnStart
        // Parse our config stuff.
        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);

            if(state == StartState.None)
            {
                return; // early
            }

            var propNodes = myConfigNode.GetNodes("PROPELLANT");
            foreach(ConfigNode node in propNodes)
            {
                Propellant p = new Propellant();
                p.Load(node);
                p.UpdateConnectedResources(part);
                propellants.Add(p);
            }

            var ispNode = myConfigNode.GetNode("atmosphereCurve");
            if(ispNode != null)
            {
                ispCurve = new FloatCurve();
                ispCurve.Load(ispNode);
            }

            foreach (PartModule thatModule in part.Modules)
            {
                ModuleEnginesFX engineFX = thatModule as ModuleEnginesFX;
                if(engineFX != null)
                {
                    engine = engineFX;
                    break;
                }
            }

            if (thrustVectorTransformName != null)
            {
                thrustTransform = part.FindModelTransform(thrustVectorTransformName);
                if(thrustTransform != null)
                {
                    Debug.Log("MVT - OnStart - found thrustTransform " + thrustVectorTransformName);
                }
            }
        }

        //--------------------------------------------------------------------
        // OnUpdate
        // Update our throttle effects
        public override void OnUpdate()
        {
            base.OnUpdate();
            if (!HighLogic.LoadedSceneIsFlight || engine == null)
            {
                return; //early
            }

            thrustNormalized = (this.engine.EngineIgnited) ? engine.normalizedThrustOutput : 0.0f;
            if (runningEffectName != null && thrustNormalized != currentThrustNormalized)
            {
                part.Effect(runningEffectName, thrustNormalized);
                currentThrustNormalized = thrustNormalized;
            }
        }
    }
}
