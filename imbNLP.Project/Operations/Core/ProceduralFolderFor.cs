using imbACE.Core.operations;
using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.ExperimentModel;
using imbSCI.Core.data.cache;
using imbSCI.Core.files;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace imbNLP.Project.Operations.Core
{


    public interface IProceduralFolderFor
    {

        CacheServiceProvider CacheProvider
        {
            get;
        }

        void DeployFolder<T>(ExperimentDataSetFold _completeFold, IList<ExperimentDataSetFold> folds, T setup, ToolkitExperimentNotes _notes) where T : ProcedureSetupBase, new();


        IAceOperationSetExecutor executor { get; set; }

        Dictionary<String, TCMAIN> Execute<TCMAIN, TCEXTRA>(Int32 parallelThreads, ILogBuilder logger, Dictionary<String, TCMAIN> executionContextDict = null, TCEXTRA executionContextExtra = null)
            where TCMAIN : class, IOperationExecutionContext, new()
        where TCEXTRA : class, IOperationExecutionContext, new();



        ///// <summary>
        ///// Finalization execution step, called after parallel execution of the folds is done
        ///// </summary>
        ///// <typeparam name="TCMAIN">The type of the cmain.</typeparam>
        ///// <typeparam name="TCEXTRA">The type of the cextra.</typeparam>
        ///// <param name="executionContextDict">The execution context dictionary.</param>
        ///// <param name="executionContextExtra">The execution context extra.</param>
        ///// <param name="logger">The logger.</param>
        //void ExecuteFinal<TCMAIN, TCEXTRA>(Dictionary<String, TCMAIN> executionContextDict, TCEXTRA executionContextExtra, ILogBuilder logger)
        //    where TCMAIN : class, IOperationExecutionContext, new()
        //where TCEXTRA : class, IOperationExecutionContext, new();



        void SetExecutionOptions(ProceduralStackOptions _options);

    }





    /// <summary>
    /// Folder for procedural execution
    /// </summary>
    /// <typeparam name="TP">The type of the p.</typeparam>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TC">The type of the c.</typeparam>
    /// <seealso cref="System.Collections.Generic.List{TP}" />
    public class ProceduralFolderFor<TP, T, TCMAIN, TCEXTRA> : List<TP>, IProceduralFolderFor
        where TP : ProcedureBaseFor<T, TCMAIN, TCEXTRA>, new()
        where T : ProcedureSetupBase, new()
        where TCMAIN : class, IOperationExecutionContext, new()
        where TCEXTRA : class, IOperationExecutionContext, new()
    {



        /// <summary>
        /// Sets the execution options.
        /// </summary>
        /// <param name="_options">The options.</param>
        public void SetExecutionOptions(ProceduralStackOptions _options)
        {
            foreach (TP p in this)
            {
                p.executionOptions = _options;
            }
        }


        public CacheServiceProvider CacheProvider { get; protected set; } = new CacheServiceProvider();


        public string name { get; set; } = "DEF";

        CacheServiceProvider IProceduralFolderFor.CacheProvider => throw new NotImplementedException();

        public static Boolean EnableParallel = true;


        /// <summary>
        /// Initializes a new instance of the <see cref="ProceduralFolderFor{TP, T, TCMAIN, TCEXTRA}"/> class.
        /// </summary>
        /// <param name="folds">The folds.</param>
        /// <param name="setup">The setup.</param>
        /// <param name="_notes">The notes.</param>
        public ProceduralFolderFor(IList<ExperimentDataSetFold> folds, T setup, ToolkitExperimentNotes _notes, IAceOperationSetExecutor __executor)
        {
            executor = __executor;
            DeployFolder(folds, setup, _notes);

#if DEBUG
            EnableParallel = false;
#endif
        }


        /// <summary>
        /// Deploys the folder.
        /// </summary>
        /// <param name="folds">The folds.</param>
        /// <param name="setup">The setup.</param>
        /// <param name="_notes">The notes.</param>
        public void DeployFolder(IList<ExperimentDataSetFold> folds, T setup, ToolkitExperimentNotes _notes)
        {
            name = _notes.folder.name;
            notes = _notes;
            if (folds is ExperimentDataSetFolds experiment_folds)
            {
                completeFold = experiment_folds.dataset;
            }

            CacheProvider.Deploy(imbACE.Core.appManager.Application.folder_cache.Add(name, name, "CacheFolder"));
            Int32 i = 0;
            foreach (ExperimentDataSetFold fold in folds)
            {
                TP procedure = new TP();
                procedure.index = i;
                procedure.SetCacheProvider(CacheProvider);
                procedure.name += setup.OutputFilename;
                procedure.Deploy(setup.CloneViaXML(_notes), fold, _notes);

                Add(procedure);
                i++;
            }

            procedureForFinalStep = new TP();
            procedureForFinalStep.index = -1;
            procedureForFinalStep.name += setup.OutputFilename;
            procedureForFinalStep.IsFinalStepInstance = true;
            procedureForFinalStep.Deploy(setup, completeFold, notes);
        }


        public TP procedureForFinalStep { get; set; }


        /// <summary>
        /// Procedure execution method
        /// </summary>
        /// <param name="procedure">The procedure.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="executionContextDict">The execution context dictionary.</param>
        /// <param name="executionContextExtra">The execution context extra.</param>
        /// <returns></returns>
        protected ExperimentDataSetFoldContextPair<TCMAIN> Execute(TP procedure, ILogBuilder logger, Dictionary<String, TCMAIN> executionContextDict = null, TCEXTRA executionContextExtra = null)
        {
            TCMAIN input = null;


            if (executionContextDict != null)
            {
                if (executionContextDict.ContainsKey(procedure.fold.name))
                {

                    input = executionContextDict[procedure.fold.name];

                    if (procedure.executionOptions.HasFlag(ProceduralStackOptions.clearContextOnStart))
                    {
                        executionContextDict.Remove(procedure.fold.name);
                        input = null;
                    }
                }
            }


            ExperimentDataSetFoldContextPair<TCMAIN> result = null;

            try
            {
                result = procedure.Execute(logger, input, executionContextExtra);
            }
            catch (Exception ex)
            {

                notes.LogException("Procedure [" + name + "]:[" + procedure.name + ":" + procedure.GetType().Name + "] ", ex, "", false);

                if (executor is IAceCommandConsole commandConsole)
                {
                    var exeContext = commandConsole.ScriptExecutionContext;

                    if (exeContext != null)
                    {
                        notes.log("S[" + exeContext.script.path + "][" + exeContext.currentLine + "][" + commandConsole.ScriptExecutionContext.lastExecutedLine + "]");
                    }
                }
                notes.SaveNote();
                throw;
            }

            if (procedure.executionOptions.HasFlag(ProceduralStackOptions.clearContextOnFinish))
            {
                result.context = null;

            }



            return result;
        }



        /// <summary>
        /// Executes the specified parallel threads.
        /// </summary>
        /// <param name="parallelThreads">The parallel threads.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="executionContextDict">The execution context dictionary.</param>
        /// <param name="executionContextExtra">The execution context extra.</param>
        /// <returns></returns>
        public Dictionary<String, TCMAIN> Execute(Int32 parallelThreads, ILogBuilder logger, Dictionary<String, TCMAIN> executionContextDict = null, TCEXTRA executionContextExtra = null)
        {

            Dictionary<String, TCMAIN> output = new Dictionary<String, TCMAIN>();
            List<ExperimentDataSetFoldContextPair<TCMAIN>> list = new List<ExperimentDataSetFoldContextPair<TCMAIN>>();


            if (EnableParallel && (parallelThreads > 1))
            {
                ParallelOptions parallelOptions = new ParallelOptions();
                parallelOptions.MaxDegreeOfParallelism = parallelThreads;

                Parallel.ForEach(this, parallelOptions, (procedure) =>
                {

                    if (procedure.executionOptions.HasFlag(ProceduralStackOptions.delayOneSecond))
                    {
                        Thread.Sleep(procedure.index * 1000);
                    }

                    if (procedure.executionOptions.HasFlag(ProceduralStackOptions.delayFiveSeconds))
                    {
                        Thread.Sleep(procedure.index * 5000);
                    }


                    var result = Execute(procedure, logger, executionContextDict, executionContextExtra);

                    list.Add(result);

                    //procedure.Execute(mainContext.notes, output, mainContext.truthTable, mainContext.testSummaries, mainContext.runName);

                });


            }
            else
            {
                foreach (TP procedure in this)
                {

                    var result = Execute(procedure, logger, executionContextDict, executionContextExtra);  //procedure.Execute(logger, input, executionContextExtra);

                    list.Add(result);

                }
            }

            list.ForEach(x => output.Add(x.fold.name, x.context));

            ExecuteFinal(output, executionContextExtra, logger);

            notes.SaveNote();

            return output;
        }

        public ToolkitExperimentNotes notes { get; set; } = null;

        /// <summary>
        /// Reference to the complete dataset sample
        /// </summary>
        /// <value>
        /// The complete fold.
        /// </value>
        public ExperimentDataSetFold completeFold { get; set; } = null;

        public IAceOperationSetExecutor executor { get; set; } = null;

        void IProceduralFolderFor.DeployFolder<T1>(ExperimentDataSetFold _completeFold, IList<ExperimentDataSetFold> folds, T1 setup, ToolkitExperimentNotes _notes)
        {
            completeFold = _completeFold;
            notes = _notes;
            DeployFolder(folds, setup as T, _notes);


        }


        protected void ExecuteFinal(Dictionary<String, TCMAIN> dict, TCEXTRA executionContextExtra, ILogBuilder logger)
        {
            logger.log("Finalization of [" + name + "]");

            CacheProvider.Describe(logger);

            procedureForFinalStep.ExecuteFinal(dict, executionContextExtra, logger);
        }


        Dictionary<string, TCMAIN1> IProceduralFolderFor.Execute<TCMAIN1, TCEXTRA1>(int parallelThreads, ILogBuilder logger, Dictionary<string, TCMAIN1> executionContextDict, TCEXTRA1 executionContextExtra)
        {
            return Execute(parallelThreads, logger, executionContextDict as Dictionary<String, TCMAIN>, executionContextExtra as TCEXTRA) as Dictionary<string, TCMAIN1>;
        }



        //void IProceduralFolderFor.ExecuteFinal<TCMAIN1, TCEXTRA1>(Dictionary<string, TCMAIN1> executionContextDict, TCEXTRA1 executionContextExtra, ILogBuilder logger)
        //{
        //    ExecuteFinal(executionContextDict as Dictionary<String, TCMAIN>, executionContextExtra as TCEXTRA, logger);
        //}
    }
}