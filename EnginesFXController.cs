using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EngineEnhancement
{
    // EnginesFXController by CardBoardBoxProcessor
    class EnginesFXController : PartModule
    {
        public List<string> engineNames;
        public List<string> childEngineNames;
        public List<int> predList;
        public List<ModuleEnginesFX> engineList;
        public List<ModuleEnginesFX> childEngineList;

        public override void OnAwake()
        {
            if (engineNames == null)
            {
                engineNames = new List<string>();
            }
            if (childEngineNames == null)
            {
                childEngineNames = new List<string>();
            }
            if (engineList == null)
            {
                engineList = new List<ModuleEnginesFX>();
            }
            if (predList == null)
            {
                predList = new List<int>();
            }
            if(childEngineList == null)
            {
                childEngineList = new List<ModuleEnginesFX>();
            }
        }

        public override void OnLoad(ConfigNode mainNode)
        {
            foreach (ConfigNode n in mainNode.GetNodes("Engine_Data"))
            {
                if (n.GetValue("engineName") != null)
                {
                    engineNames.Add(n.GetValue("engineName"));
                }
                if (n.GetValue("precedence") != null)
                {
                    predList.Add(int.Parse(n.GetValue("precedence")));
                }
                if(n.GetValue("childEngineName") != null)
                {
                    childEngineNames.Add(n.GetValue("childEngineName"));
                }
                else
                {
                    childEngineNames.Add(null);
                }
            }
        }

        public override void OnStart(PartModule.StartState state)
        {
            if (engineList.Count == 0 /*&& !(engineList.Count > 0)*/)
            {
                ModuleEnginesFX[] engineSearch = part.GetComponents<ModuleEnginesFX>();

                for (int j = 0; j < engineNames.Count; j++)
                {
                    engineList.Add(new ModuleEnginesFX());
                    childEngineList.Add(new ModuleEnginesFX());
                }

                for (int i = 0; i < engineNames.Count; i++)
                {
                    for (int j = 0; j < engineSearch.Length; j++)
                    {
                        if (engineNames[i] == engineSearch[j].engineID)
                        {
                            engineList[predList[i]] = engineSearch[j];
                        }
                        if(childEngineNames[i] == engineSearch[j].engineID)
                        {
                            childEngineList[predList[i]] = engineSearch[j];
                        }
                    }
                }

                engineNames.Clear();
                childEngineNames.Clear();
                predList.Clear();
            }
            else
            {
                Debug.LogWarning("EnginesFXController - OnStart - did it already: " + engineList.Count);
            }

            for (int i = 0; i < engineList.Count; i++)
            {
                engineList[i].isEnabled = false;
                engineList[i].Shutdown();
                childEngineList[i].isEnabled = false;
                childEngineList[i].Shutdown();
            }
        }

        public override void OnUpdate()
        {
            float throttle = vessel.ctrlState.mainThrottle;

            if (engineList == null)
            {
                Debug.LogError("EnginesFXController - OnUpdate - Enginelist is Null");
            }
            else
            {
                //Debug.Log("EC - OnUpdate");
                // TEST THIS WITH RD-0440 / Multi-mode engine switching
                for (int i = 1; i < engineList.Count; i++)
                {
                    if (engineList[i - 1].engineShutdown || engineList[i - 1].flameout)
                    {
                        //Debug.Log("Engine " + i + " Activate");
                        //engineList[i].isEnabled = true;
                        //engineList[i].Activate();
                        engineList[i].EngineIgnited = true;
                        //childEngineList[i].isEnabled = true;
                        //childEngineList[i].Activate();
                        childEngineList[i].EngineIgnited = true;
                    }
                    else
                    {
                        //Debug.Log("Engine " + i + " Shutdown");
                        engineList[i].EngineIgnited = false;
                        //engineList[i].Shutdown();
                        //engineList[i].isEnabled = false;
                        //engineList[i].EngineIgnited = false;
                        //childEngineList[i].Shutdown();
                        //childEngineList[i].Shutdown();
                        childEngineList[i].EngineIgnited = false;
                    }
                }
            }
        }
    }
}
