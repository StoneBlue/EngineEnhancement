using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineEnhancement
{
    class EngineHider:PartModule
    {
        [KSPField]
        public string hiddenEngine = "";

        private bool enginesFound = false;
        private MultiModeEngine mmeModule = null;
        private List<ModuleEngines> parentEngineModules = new List<ModuleEngines>();
        private List<ModuleEnginesFX> parentEngineFxModules = new List<ModuleEnginesFX>();

        private ModuleEngines myEngine = null;
        private ModuleEnginesFX myEngineFx = null;

        //--------------------------------------------------------------------
        // EngineIgnited
        private bool EngineIgnited()
        {
            bool engineIgnited = false;
            if (mmeModule != null)
            {
                string activeEngine = (mmeModule.runningPrimary) ? mmeModule.primaryEngineID : mmeModule.secondaryEngineID;

                foreach (ModuleEnginesFX engine in parentEngineFxModules)
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

                foreach (ModuleEnginesFX engine in parentEngineFxModules)
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
                    var mme = thatModule as MultiModeEngine;
                    if (mme != null)
                    {
                        mmeModule = mme;
                    }
                    var em = thatModule as ModuleEngines;
                    if (em != null)
                    {
                        if (em.thrustVectorTransformName == hiddenEngine)
                        {
                            myEngine = em;
                        }
                        else
                        {
                            parentEngineModules.Add(em);
                        }
                    }
                    var efm = thatModule as ModuleEnginesFX;
                    if (efm != null)
                    {
                        if (efm.thrustVectorTransformName == hiddenEngine)
                        {
                            myEngineFx = efm;
                        }
                        else
                        {
                            parentEngineFxModules.Add(efm);
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
            if(myEngine != null)
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
            else if(myEngineFx != null)
            {
                if (HighLogic.LoadedSceneIsFlight)
                {
                    myEngineFx.EngineIgnited = EngineIgnited();
                    myEngineFx.Events["Activate"].guiActive = false;
                    myEngineFx.Events["Shutdown"].guiActive = false;
                    myEngineFx.Fields["status"].guiActive = false;
                    myEngineFx.Fields["statusL2"].guiActive = false;
                    myEngineFx.Fields["realIsp"].guiActive = false;
                    myEngineFx.Fields["finalThrust"].guiActive = false;
                    myEngineFx.Fields["fuelFlowGui"].guiActive = false;
                }
                myEngineFx.Fields["thrustPercentage"].guiActive = false;
                myEngineFx.Fields["thrustPercentage"].guiActiveEditor = false;
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

    public class GimbalHider:PartModule
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
                    var gimbal = thatModule as ModuleGimbal;
                    if (gimbal != null)
                    {
                        gimbals.Add(gimbal);
                    }
                }

                gimbalsFound = true;
            }
        }

        //--------------------------------------------------------------------
        // HideMyMenu
        private void HideMyMenu()
        {
            for(int i=1; i<gimbals.Count; ++i)
            {
                gimbals[i].gimbalLock = gimbals[0].gimbalLock;
                gimbals[i].Events["FreeGimbal"].guiActive = false;
                gimbals[i].Events["FreeGimbal"].guiActiveEditor = false;
                gimbals[i].Events["LockGimbal"].guiActive = false;
                gimbals[i].Events["LockGimbal"].guiActiveEditor = false;
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
