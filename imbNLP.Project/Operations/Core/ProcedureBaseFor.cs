using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Documents.Ranking;
using imbNLP.Toolkit.Documents.Ranking.Core;
using imbNLP.Toolkit.ExperimentModel;
using imbSCI.Core.data.cache;
using imbSCI.Core.files;
using imbSCI.Core.reporting;
using imbSCI.Data;
using System;
using System.Collections.Generic;

namespace imbNLP.Project.Operations.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TCMAIN">The type of the cmain.</typeparam>
    /// <typeparam name="TCEXTRA">The type of the cextra.</typeparam>
    /// <seealso cref="imbNLP.Project.Operations.Core.ProcedureBase" />
    /// <seealso cref="imbSCI.Core.data.cache.IHasCacheProvider" />
    public abstract class ProcedureBaseFor<T, TCMAIN, TCEXTRA> : ProcedureBase, IHasCacheProvider
            where T : ProcedureSetupBase
            where TCMAIN : class, IOperationExecutionContext, new()
            where TCEXTRA : class, IOperationExecutionContext, new()
    {

        protected T setup { get; set; }

        public ToolkitExperimentNotes notes { get; set; }
        public ToolkitExperimentNotes fold_notes { get; set; }

        public Boolean IsFinalStepInstance { get; set; } = false;

        protected ScoreModelRequirements requirements { get; set; }

        protected List<IHasProceduralRequirements> componentsWithRequirements { get; set; } = new List<IHasProceduralRequirements>();

        protected CacheServiceProvider CacheProvider { get; set; }

        public void SetCacheProvider(CacheServiceProvider _CacheProvider)
        {
            CacheProvider = _CacheProvider;
        }




        /// <summary>
        /// Checks the requirements and connects the <see cref="CacheProvider"/>
        /// </summary>
        /// <param name="requirements">The requirements.</param>
        /// <returns></returns>
        public ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null)
        {
            ScoreModelRequirements output = new ScoreModelRequirements();
            foreach (IHasProceduralRequirements component in componentsWithRequirements)
            {
                output = component.CheckRequirements(output);

                if (setup.useCacheProvider)
                {
                    if (component is IHasCacheProvider componentCached)
                    {

                        componentCached.SetCacheProvider(CacheProvider);
                    }
                }

            }

            output.Describe(notes);

            return output;
        }


        protected ProcedureBaseFor(T _setup, ExperimentDataSetFold _fold, ToolkitExperimentNotes _notes)
        {

            Deploy(_setup, _fold, _notes);
        }

        protected ProcedureBaseFor() : base()
        {

        }


        public void Deploy(T _setup, ExperimentDataSetFold _fold, ToolkitExperimentNotes _notes)
        {
            setup = _setup;
            fold = _fold;
            notes = _notes;

            if (!IsFinalStepInstance)
            {
                fold_notes = fold.StartNote(_notes, name);
                notes = fold_notes;
            }
            else
            {
                fold_notes = notes;
            }




            DeployCustom();
        }

        public void Open()
        {
            if (!IsFinalStepInstance)
            {
                notes = fold_notes.StartSubnotes(name, description);
            }
            //notes.logStartPhase(name, description);

            if (setup.reportOptions.HasFlag(OperationReportEnum.saveSetupXML))
            {
                setup.saveObjectToXML(notes.folder.pathFor(name.ensureEndsWith("_setup.xml"), imbSCI.Data.enums.getWritableFileMode.overwrite, "Operation configuration [" + description + "]"));
            }
        }

        public void Close()
        {
            //notes.logEndPhase();

            notes.SaveNote();
            fold_notes.SaveNote();

            //fold.EndNote();
            //  fold.notes.SaveNote();


        }


        public abstract void DeployCustom();

        public abstract ExperimentDataSetFoldContextPair<TCMAIN> Execute(ILogBuilder logger, TCMAIN executionContextMain = null, TCEXTRA executionContextExtra = null);

        /// <summary>
        /// Executes the final step, over complete execution result set
        /// </summary>
        /// <param name="executionContextDict">The execution context dictionary.</param>
        /// <param name="executionContextExtra">The execution context extra.</param>
        /// <param name="logger">The logger.</param>
        public virtual void ExecuteFinal(Dictionary<String, TCMAIN> executionContextDict, TCEXTRA executionContextExtra, ILogBuilder logger)
        {

        }



    }
}