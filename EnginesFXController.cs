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
        public List<int> predList;
        public List<ModuleEnginesFX> engineList;

        public override void OnAwake()
        {
            if (engineNames == null)
            {
                engineNames = new List<String>();
            }
            if (engineList == null)
            {
                engineList = new List<ModuleEnginesFX>();
            }
            if (predList == null)
            {
                predList = new List<int>();
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
            }
        }

        public override void OnStart(PartModule.StartState state)
        {
            if (engineList.Count == 0 && !(engineList.Count > 0))
            {
                ModuleEnginesFX[] engineSearch = part.GetComponents<ModuleEnginesFX>();

                for (int j = 0; j < engineNames.Count; j++)
                {
                    engineList.Add(new ModuleEnginesFX());
                }

                for (int i = 0; i < engineNames.Count; i++)
                {
                    for (int j = 0; j < engineSearch.Length; j++)
                    {
                        if (engineNames[i] == engineSearch[j].engineID)
                        {
                            engineList[predList[i]] = engineSearch[j];
                        }
                    }
                }

                engineNames.Clear();
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
            }
        }

        public override void OnUpdate()
        {
            float throttle = vessel.ctrlState.mainThrottle;

            if (engineList == null)
            {
                Debug.LogError("EnginesFXController - OnUpdate - Enginelist is Null, OnUpdate()");
            }
            else
            {
                for (int i = 1; i < engineList.Count; i++)
                {
                    if (engineList[i - 1].engineShutdown || engineList[i - 1].flameout)
                    {
                        engineList[i].isEnabled = true;
                        engineList[i].Activate();
                        //engineList[i].EngineIgnited = true;
                    }
                    else
                    {
                        engineList[i].Shutdown();
                        engineList[i].isEnabled = false;
                        //engineList[i].EngineIgnited = false;
                    }
                }
            }
        }
    }
}
