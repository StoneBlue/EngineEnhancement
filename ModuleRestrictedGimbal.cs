using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EngineEnhancement
{
    class ModuleMultiAxisGimbal:ModuleGimbal
    {
        [KSPField(isPersistant = false)]
        public float pitchRange = 0.0f;
        [KSPField(isPersistant = false)]
        public float yawRange = 0.0f;

        [KSPField(isPersistant = false)]
        public string pitchTransformName = null;
        [KSPField(isPersistant = false)]
        public string yawTransformName = null;

        private Transform pitchTransform = null;
        private Transform yawTransform = null;

        private Quaternion initPitch = Quaternion.identity;
        private Quaternion initYaw = Quaternion.identity;

        //--------------------------------------------------------------------
        // GetInfo
        // Tell the user how this gimbal is configured.
        public override string GetInfo()
        {
            return string.Format("Maximum Pitch: {0}°\nMaximum Yaw: {1}°\n", pitchRange, yawRange);
        }

        //--------------------------------------------------------------------
        // OnFixedUpdate
        public override void OnFixedUpdate()
        {
            if (!HighLogic.LoadedSceneIsFlight) return;

            // Override stock behavior, so don't call base.OnFixedUpdate()
            float toYaw;
            float toPitch;
            if(gimbalLock)
            {
                toYaw = 0.0f;
                toPitch = 0.0f;
            }
            else
            {
                FlightCtrlState ctrl = vessel.ctrlState;
                // MOARdV TODO: decompose the desired inputs, and compare our thrust position relative to the center of mass.
                // That way, we can provide roll authority if our thrust transform is off-axis.

                if(gimbalTransforms.Count != 1)
                {
                    Debug.Log("Unexpected gimbal count of " + gimbalTransforms.Count);
                }
                else
                {
                    Vector3 fwd = gimbalTransforms[0].TransformDirection(vessel.ReferenceTransform.forward);
                    Vector3 CoM = gimbalTransforms[0].InverseTransformPoint(vessel.CoM);
                    Vector3 center = gimbalTransforms[0].InverseTransformPoint(gimbalTransforms[0].position);
                    float angle = Vector3.Angle(fwd, gimbalTransforms[0].forward);
                    //Part.PartToVesselSpaceDir

                    Debug.Log("CoM = " + CoM + ", fwd = " + fwd + ", center = " + center + ", angle = " + angle);
                }

                toYaw = ctrl.yaw * yawRange;
                toPitch = ctrl.pitch * pitchRange;
            }


            if (gimbalAngleYaw != toYaw)
            {
                gimbalAngleYaw = Mathf.Lerp(gimbalAngleYaw, toYaw, Mathf.Min(1.0f, gimbalResponseSpeed * TimeWarp.deltaTime));
            }
            if (gimbalAnglePitch != toPitch)
            {
                gimbalAnglePitch = Mathf.Lerp(gimbalAnglePitch, toPitch, Mathf.Min(1.0f, gimbalResponseSpeed * TimeWarp.deltaTime));
            }

            if (pitchTransform != null)
            {
                pitchTransform.localRotation = initPitch * Quaternion.AngleAxis(gimbalAnglePitch, pitchTransform.InverseTransformDirection(vessel.ReferenceTransform.right));
            }
            if (yawTransform != null)
            {
                yawTransform.localRotation = initYaw * Quaternion.AngleAxis(gimbalAngleYaw, yawTransform.InverseTransformDirection(vessel.ReferenceTransform.forward));
            }
        }

        //--------------------------------------------------------------------
        // OnStart
        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);

            if (state == StartState.None)
            {
                return; // early
            }

            if (pitchTransformName != null)
            {
                pitchTransform = part.FindModelTransform(pitchTransformName);
                if (pitchTransform != null)
                {
                    initPitch = pitchTransform.localRotation;
                }
            }

            if (yawTransformName != null)
            {
                yawTransform = part.FindModelTransform(yawTransformName);
                if (yawTransform != null)
                {
                    initYaw = yawTransform.localRotation;
                }
            }
        }

        //--------------------------------------------------------------------
        // OnUpdate
        public override void OnUpdate()
        {
            // Override stock gimbal
        }
    }
}
