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

            public TransformGroup(Part part, string source, string destination)
            {
                if(!(source == null || destination == null))
                {
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
                                }
                            }
                        }
                    }
                }
            }

            public void FixedUpdate()
            {
                Vector3 sourceAngles = sourceTransform.localEulerAngles;
                Vector3 destAngles = new Vector3(sourceAngles.x * eulerAngles.x, sourceAngles.y * eulerAngles.y, sourceAngles.z * eulerAngles.z);
                for (int i = 0; i < destinationTransform.Length; ++i)
                {
                    destinationTransform[i].localEulerAngles = destAngles;
                }
            }
        }

        [KSPField]
        public string transformName = null;

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
                group.FixedUpdate();
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
                    TransformGroup tg = new TransformGroup(part, inits[i*2],inits[i*2+1]);
                    if(tg.sourceTransform != null)
                    {
                        transforms.Add(tg);
                    }
                }
            }
        }
    }
}
