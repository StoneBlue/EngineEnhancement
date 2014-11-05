using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EngineEnhancement
{
    class ModuleRestrictedGimbal:ModuleGimbal
    {
        [KSPField(isPersistant = false)]
        public float pitchRange = 90.0f;

        [KSPField(isPersistant = false)]
        public float rollRange = 90.0f;

        [KSPField(isPersistant = false)]
        public float yawRange = 90.0f;

        private bool showedError = false;

        // ModuleGimbal
        //public float gimbalAnglePitch;
        //public float gimbalAngleRoll;
        //public float gimbalAngleYaw;
        //[KSPField(isPersistant = true)]
        //public bool gimbalLock;
        //[KSPField(isPersistant = false)]
        //public float gimbalRange;
        //[KSPField(isPersistant = false)]
        //public float gimbalResponseSpeed;
        //[KSPField(isPersistant = false)]
        //public string gimbalTransformName;
        //public List<UnityEngine.Transform> gimbalTransforms;
        //public List<UnityEngine.Quaternion> initRots;
        //[KSPField(isPersistant = false)]
        //public bool useGimbalResponseSpeed;

        public override string GetInfo()
        {
            return string.Format("Maximum Pitch: {0}°\nMaximum Yaw: {1}°", pitchRange, yawRange);
        }

        public override void OnFixedUpdate()
        {
            if (!HighLogic.LoadedSceneIsFlight || !vessel.isActiveVessel) return;

            if (gimbalTransforms.Count > initRots.Count && !showedError)
            {
                Debug.LogError(this.name + " - too many gimbal transforms.  Disabling module.");
                showedError = true;
                return;
            }

            FlightCtrlState ctrl = vessel.ctrlState;

            if (useGimbalResponseSpeed)
            {
                float toYaw = ctrl.yaw * yawRange;
                float toPitch = ctrl.pitch * pitchRange;
                float toRoll = ctrl.roll * rollRange;
                gimbalAngleYaw = Mathf.Lerp(gimbalAngleYaw, toYaw, gimbalResponseSpeed * TimeWarp.deltaTime);
                gimbalAnglePitch = Mathf.Lerp(gimbalAnglePitch, toPitch, gimbalResponseSpeed * TimeWarp.deltaTime);
                gimbalAngleRoll = Mathf.Lerp(gimbalAngleRoll, toRoll, gimbalResponseSpeed * TimeWarp.deltaTime);
            }
            else
            {
                gimbalAngleYaw = ctrl.yaw * yawRange;
                gimbalAnglePitch = ctrl.pitch * pitchRange;
                gimbalAngleRoll = ctrl.roll * rollRange;
            }

            for(int i=0; i<gimbalTransforms.Count; ++i)
            {
                gimbalTransforms[i].localRotation = initRots[i] * Quaternion.AngleAxis(this.gimbalAnglePitch, gimbalTransforms[i].InverseTransformDirection(vessel.ReferenceTransform.right)) * Quaternion.AngleAxis(gimbalAngleYaw, gimbalTransforms[i].InverseTransformDirection(vessel.ReferenceTransform.forward));
            }
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            pitchRange = Math.Max(0.0f, Math.Min(pitchRange, gimbalRange));
            rollRange = Math.Max(0.0f, Math.Min(rollRange, gimbalRange));
            yawRange = Math.Max(0.0f, Math.Min(yawRange, gimbalRange));
        }
    }
}
