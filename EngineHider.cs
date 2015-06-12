using System;
using System.Collections.Generic;

namespace EngineEnhancement
{
    //--- EngineHider: Hide multiple ModuleEngines on the same part ----------
    class EngineHider : PartModule
    {
        [KSPField]
        public string hiddenEngine = "";

        private bool enginesFound = false;
        private MultiModeEngine mmeModule = null;
        private List<ModuleEngines> parentEngineModules = new List<ModuleEngines>();

        private ModuleEngines myEngine = null;

        //--------------------------------------------------------------------
        // EngineIgnited
        private bool EngineIgnited()
        {
            bool engineIgnited = false;
            if (mmeModule != null)
            {
                string activeEngine = (mmeModule.runningPrimary) ? mmeModule.primaryEngineID : mmeModule.secondaryEngineID;

                foreach (ModuleEngines engine in parentEngineModules)
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
                foreach (ModuleEngines engine in parentEngineModules)
                {
                    if (engine.EngineIgnited)
                    {
                        engineIgnited = true;
                    }
                }
            }

            return engineIgnited;
        }

        //--------------------------------------------------------------------
        // FindEngines
        private void FindEngines()
        {
            if (!enginesFound && part != null)
            {
                foreach (PartModule thatModule in part.Modules)
                {
                    if (thatModule is MultiModeEngine)
                    {
                        mmeModule = thatModule as MultiModeEngine;
                    }
                    else if (thatModule is ModuleEngines)
                    {
                        var em = thatModule as ModuleEngines;
                        if (em.thrustVectorTransformName == hiddenEngine)
                        {
                            myEngine = em;
                        }
                        else
                        {
                            parentEngineModules.Add(em);
                        }
                    }
                }

                enginesFound = true;
            }
        }

        //--------------------------------------------------------------------
        // HideMyMenu
        private void HideMyMenu()
        {
            try
            {
                if (myEngine != null)
                {
                    if (HighLogic.LoadedSceneIsFlight)
                    {
                        myEngine.EngineIgnited = EngineIgnited();
                        myEngine.Events["Activate"].guiActive = false;
                        myEngine.Events["Shutdown"].guiActive = false;
                        myEngine.Fields["status"].guiActive = false;
                        myEngine.Fields["statusL2"].guiActive = false;
                        myEngine.Fields["realIsp"].guiActive = false;
                        myEngine.Fields["finalThrust"].guiActive = false;
                        myEngine.Fields["fuelFlowGui"].guiActive = false;
                    }
                    myEngine.Fields["thrustPercentage"].guiActive = false;
                    myEngine.Fields["thrustPercentage"].guiActiveEditor = false;
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("EngineEnhancement.EngineHider.HideMyMenu triggered an exception: " + e);
                // self-destruct so we don't spam
                Destroy(this);
            }
        }

        //--------------------------------------------------------------------
        // OnStart
        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            FindEngines();
        }

        //--------------------------------------------------------------------
        // OnFixedUpdate
        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            FindEngines();

            HideMyMenu();
        }

        //--------------------------------------------------------------------
        // OnUpdate
        public override void OnUpdate()
        {
            base.OnUpdate();
            FindEngines();

            HideMyMenu();
        }
    }

    //--- GimbalHider: hide multiple gimbals on the same part ----------------
    public class GimbalHider : PartModule
    {
        private bool gimbalsFound = false;
        private List<ModuleGimbal> gimbals = new List<ModuleGimbal>();

        //--------------------------------------------------------------------
        // FindGimbals
        private void FindGimbals()
        {
            if (!gimbalsFound && part != null)
            {
                foreach (PartModule thatModule in part.Modules)
                {
                    if (thatModule is ModuleGimbal)
                    {
                        gimbals.Add(thatModule as ModuleGimbal);
                    }
                }

                gimbalsFound = true;
            }
        }

        //--------------------------------------------------------------------
        // HideMyMenu
        private void HideMyMenu()
        {
            try
            {
                for (int i = 1; i < gimbals.Count; ++i)
                {
                    gimbals[i].gimbalLock = gimbals[0].gimbalLock;
                    gimbals[i].Actions["FreeAction"].active = false;
                    gimbals[i].Actions["LockAction"].active = false;
                    gimbals[i].Actions["ToggleAction"].active = false;
                    gimbals[i].Fields["gimbalLimiter"].guiActive = false;
                    gimbals[i].Fields["gimbalLimiter"].guiActiveEditor = false;
                    gimbals[i].Fields["gimbalLock"].guiActive = false;
                    gimbals[i].Fields["gimbalLock"].guiActiveEditor = false;
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("EngineEnhancement.GimbalHider.HideMyMenu triggered an exception: " + e);
                // self-destruct so we don't spam
                Destroy(this);
            }
        }

        //--------------------------------------------------------------------
        // OnStart
        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            FindGimbals();
        }

        //--------------------------------------------------------------------
        // OnFixedUpdate
        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            FindGimbals();

            HideMyMenu();
        }

        //--------------------------------------------------------------------
        // OnUpdate
        public override void OnUpdate()
        {
            base.OnUpdate();
            FindGimbals();

            HideMyMenu();
        }
    }
}
