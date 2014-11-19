using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EngineEnhancement
{
    class AxisRotationCopy:PartModule
    {
        private class TransformGroup
        {
            public readonly Transform sourceTransform = null;
            public readonly Transform[] destinationTransform = null;
            public readonly Vector3 eulerAngles = Vector3.zero;
            private Quaternion lastRotation = Quaternion.identity;
            private float angleRate = 0.0f;

            public TransformGroup(Part part, string source, string destination, float _angleRate)
            {
                if(!(source == null || destination == null))
                {
                    angleRate = _angleRate;

                    string[] sources = source.Split('#');
                    if(sources.Length == 2)
                    {
                        Transform st = part.FindModelTransform(sources[0].Trim());
                        string axis = sources[1].Trim().ToLower();
                        if (st != null)
                        {
                            if(axis == "pitch")
                            {
                                eulerAngles.x = 1.0f;
                            }
                            else if(axis == "yaw")
                            {
                                eulerAngles.y = 1.0f;
                            }
                            else if(axis == "roll")
                            {
                                eulerAngles.z = 1.0f;
                            }

                            if (eulerAngles != Vector3.zero)
                            {
                                Transform[] dt = part.FindModelTransforms(destination.Trim());
                                if (dt != null && dt.Length > 0)
                                {
                                    sourceTransform = st;
                                    destinationTransform = dt;

                                    Vector3 sourceAngles = sourceTransform.localEulerAngles;
                                    Vector3 destAngles = new Vector3(sourceAngles.x * eulerAngles.x, sourceAngles.y * eulerAngles.y, sourceAngles.z * eulerAngles.z);
                                    Quaternion destRot = Quaternion.Euler(destAngles);
                                    lastRotation = destRot;
                                    //Debug.Log("ARC - Init lastRotation = " + lastRotation);
                                }
                            }
                        }
                    }
                }
            }

            public void FixedUpdate(float dT)
            {
                Vector3 sourceAngles = sourceTransform.localEulerAngles;
                Vector3 destAngles = new Vector3(sourceAngles.x * eulerAngles.x, sourceAngles.y * eulerAngles.y, sourceAngles.z * eulerAngles.z);
                Quaternion destRot = Quaternion.Euler(destAngles);
                Quaternion resultQuat;
                if (angleRate > 0.0f)
                {
                    float angle = Quaternion.Angle(lastRotation, destRot);
                    if(Mathf.Abs(angle) > angleRate*dT)
                    {
                        float slerp = Mathf.Clamp01((dT * angleRate) / angle);
                        resultQuat = Quaternion.Slerp(lastRotation, destRot, slerp);
                    }
                    else
                    {
                        resultQuat = destRot;
                    }
                }
                else
                {
                    resultQuat = destRot;
                }

                lastRotation = resultQuat;
                for (int i = 0; i < destinationTransform.Length; ++i)
                {
                    destinationTransform[i].localRotation = resultQuat;
                }
            }
        }

        [KSPField]
        public string transformName = null;

        [KSPField]
        public float axisResponseRate = 1.0f;

        private List<TransformGroup> transforms = new List<TransformGroup>();

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            if (!HighLogic.LoadedSceneIsFlight || !vessel.isActiveVessel)
            {
                return; // early
            }

            foreach(var group in transforms)
            {
                group.FixedUpdate(TimeWarp.deltaTime);
            }
        }

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            if (!HighLogic.LoadedSceneIsFlight || !vessel.isActiveVessel || transformName == null)
            {
                return; // early
            }

            string[] inits = transformName.Split(',');
            if(inits != null)
            {
                for(int i=0; i<inits.Length/2; ++i)
                {
                    TransformGroup tg = new TransformGroup(part, inits[i * 2], inits[i * 2 + 1], axisResponseRate);
                    if(tg.sourceTransform != null)
                    {
                        transforms.Add(tg);
                    }
                }
            }
        }
    }
}
