using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EngineEnhancement
{
    class APAS69Fix:PartModule
    {
        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);

            if (HighLogic.LoadedSceneIsFlight)
            {
                string nodeTransformName = null;
                foreach (PartModule thatModule in part.Modules)
                {
                    ModuleDockingNode dockNode = thatModule as ModuleDockingNode;
                    if(dockNode != null)
                    {
                        nodeTransformName = dockNode.nodeTransformName;
                    }
                }

                if(nodeTransformName != null)
                {
                    Transform baseTransform = part.transform.FindChild("model");
                    //Debug.Log("Prefix:");
                    //WalkTransformsDown(baseTransform, "+-");

                    Vector3 partUp = baseTransform.up;

                    //Transform dock = part.transform.FindChild(nodeTransformName);
                    Transform dock = FindChildTransform(baseTransform, nodeTransformName);
                    if(dock != null)
                    {
                        Vector3 dockFwd = dock.forward;
                        Vector3 upL = dock.InverseTransformDirection(partUp);
                        Vector3 dF = dock.InverseTransformDirection(dockFwd);
                        float angle = Vector3.Angle(upL, dF);
                        //Debug.Log("Angle between reference UP and local FWD is "+angle);

                        Quaternion rotation = Quaternion.FromToRotation(dF, upL);
                        dock.localRotation = dock.localRotation * rotation;

                        //Debug.Log("Postfix:");
                        //WalkTransformsDown(baseTransform, "+-");
                    }
                    else
                    {
                        Debug.LogWarning("APAS69Fix: Did not find a transform named " + nodeTransformName + " in the part");
                    }
                }
                else
                {
                    Debug.LogWarning("APAS69Fix: Did not find nodeTransformName in ModuleDockingNode");
                }
            }
        }

        private static Transform FindChildTransform(Transform transform, string nodeTransformName)
        {
            Transform child = null;
            if (transform != null)
            {
                for (int i = 0; i < transform.childCount; ++i)
                {
                    if(transform.GetChild(i).name == nodeTransformName)
                    {
                        child = transform.GetChild(i);
                        break;
                    }
                    else if(transform.GetChild(i).childCount > 0)
                    {
                        child = FindChildTransform(transform.GetChild(i), nodeTransformName);
                        if(child != null)
                        {
                            break;
                        }
                    }
                }
            }

            return child;
        }

        //--------------------------------------------------------------------
        // WalkTransformsDown
        // From the given transform, do a traversal of all children of the
        // transform.  Debug.
        private static void WalkTransformsDown(Transform t, string prefix)
        {
            for (int i = 0; i < t.childCount; ++i)
            {
                Transform child = t.GetChild(i);
                Debug.Log(prefix + child + ", up = " + child.up);
                if (child.childCount > 0)
                {
                    WalkTransformsDown(child, " " + prefix);
                }
            }
        }
    }
}
