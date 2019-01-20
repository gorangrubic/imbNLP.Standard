using imbACE.Core.operations;
using imbACE.Services.consolePlugins;
using imbNLP.Project.Dataset;
using imbNLP.Project.Extensions;
using imbNLP.Project.Operations.Core;
using imbNLP.Project.Operations.Procedures;
using imbNLP.Project.Operations.Setups;
using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Documents.HtmlAnalysis;
using imbNLP.Toolkit.Documents.Ranking;
using imbNLP.Toolkit.Documents.Ranking.Data;
using imbNLP.Toolkit.Documents.WebExtensions;
using imbNLP.Toolkit.ExperimentModel;
using imbNLP.Toolkit.Feature;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Reporting;
using imbSCI.Core.enums;
using imbSCI.Core.files;
using imbSCI.Core.files.folders;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace imbNLP.Project.Operations
{
    /// <summary>
    /// Plugin for imbACE console - imbNLPProjectPlugin
    /// </summary>
    /// <seealso cref="imbACE.Services.consolePlugins.aceConsolePluginBase" />
    public class nlpOperationsPlugin : aceConsolePluginExtension
    {


        public static String XMLEXPORT_ClassificationResults { get; set; } = "results.xml";




        private becDocumentSelectionExtension _docSelection;
        public ExperimentModelExecutionContext mainContext { get; set; } = null;


        private becGlobalWeightTableConstructionExtension _wtConstructor;

        public becGlobalWeightTableConstructionExtension wtConstructor
        {
            get { return _wtConstructor; }
        }


        public becProjectionWeightTableExtension projectionConstructor
        {
            get { return _projectionConstructor; }

        }


        public becDataSetReportingExtension reporting
        {
            get { return _reporting; }
            set { _reporting = value; }
        }

        public becFeatureCWPAnalysisExtension analysis
        {
            get { return _analysis; }
            set { _analysis = value; }
        }


        [Display(GroupName = "Setup", Name = "ds", Description = "Document selection editor")]
        public becDocumentSelectionExtension docSelection
        {
            get { return _docSelection; }
        }

        private becClassificationExtension _docClassification;
        private becDataSetProviderExtension _dataProvider;
        private becProjectionWeightTableExtension _projectionConstructor;
        private becDataSetReportingExtension _reporting;
        private becFeatureCWPAnalysisExtension _analysis;

        [Display(GroupName = "Setup", Name = "dc", Description = "Classification settings editor")]
        public becClassificationExtension docClassification
        {
            get
            {
                return _docClassification;
            }
        }


        public ProceduralStack proceduralStack { get; set; } = new ProceduralStack();





        [Display(GroupName = "set", Name = "ResetContext", ShortName = "", Description = "Dumps current operation context")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will clear any operation contextual data")]
        /// <summary>Dumps current operation context</summary> 
        /// <remarks><para>It will clear any operation contextual data</para></remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setResetContext()
        {
            contextSet = null;
        }




        [Display(GroupName = "set", Name = "CopyModelsToAnalysis", ShortName = "", Description = "It will copy current classification weight and feature filter models into collection for comparative analysis")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "What it will do?")]
        /// <summary>It will copy current classification weight and feature filter models into collection for comparative analysis</summary> 
        /// <remarks><para>What it will do?</para></remarks>
        /// <param name="word">--</param>
        /// <param name="steps">--</param>
        /// <param name="debug">--</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setCopyModelsToAnalysis(
              [Description("--")] Boolean copyWeight = true,
              [Description("--")] Boolean copyFilter = true,
              [Description("--")] Boolean removeExisting =false)
        {
            if (removeExisting)
            {
                analysis.data.FilterModels.Clear();
                analysis.data.WeightModels.Clear();
            }

            if (copyFilter) analysis.data.FilterModels.Add(docClassification.data.corpusMethod.filter.CloneViaXML());

            if (copyWeight) analysis.data.WeightModels.Add(docClassification.data.corpusMethod.WeightModel.CloneViaXML());


        }




        [Display(GroupName = "run", Name = "DataSetReduction", ShortName = "", Description = "Performs dataset reduction by removing designated html nodes and attributes")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "What it will do?")]
        /// <summary>Performs dataset reduction by removing designated html nodes and attributes</summary> 
        /// <remarks><para>What it will do?</para></remarks>
        /// <param name="inputPath">Path leading to source dataset</param>
        /// <param name="outp">--</param>
        /// <param name="debug">--</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_runDataSetReduction(
              [Description("Path leading to source dataset")] String inputPath = @"G:\imbWBI\datasets\7sectors_2018b",
              [Description("Output for reduced dataset")] String outputPath = @"G:\imbWBI\datasets\7sectors_2018c",
              [Description("Filename of reduction setup")] String reductionSetup = "*",
              [Description("Sites to remove")] WebSiteGraphDiagnosticMark marksToRemove = WebSiteGraphDiagnosticMark.none,
              Int32 minPageLimit = -1,
              Int32 maxPageLimit = -1,
              Boolean removeEmptyPages = false)
        {

            var logPosition = output.Length;

            WebKBDatasetAdapter adapter = new WebKBDatasetAdapter();
            List<WebSiteDocumentsSet> dataset = null;


            WebDocumentsCategory category = adapter.LoadDataset(inputPath, WebDomainCategoryFormatOptions.normalizeDomainname, output);

            if (reductionSetup != "*")
            {
                reductionSetup = folder.pathMake(reductionSetup);
            }

            var settings = WebSiteDataSetReductionSettings.LoadOrDefault(reductionSetup, output); // HtmlDocumentReductionSettings.LoadOrDefault(reductionSetup, output);


            if (settings.LimitSettings.flattenCategoryHierarchy)
            {
                dataset = category.GetFirstLevelCategories();
            }
            else
            {
                dataset = category.GetAllCategories();
            }

            if (mainContext?.dataset != null)
            {
                mainContext.dataset.TransferExtensionsTo(dataset);
            }

            if (outputPath == "*") outputPath = inputPath;


            WebSiteDataSetReductionEngine engine = new WebSiteDataSetReductionEngine();

            //HtmlDocumentReductionEngine engine = new HtmlDocumentReductionEngine();
            engine.ReduceDataset(dataset, settings, output);


            String out_pathRoot = Path.GetDirectoryName(outputPath);
            String out_dataset = outputPath.Substring(out_pathRoot.Length);

            if (!out_dataset.isNullOrEmpty())
            {
                category.name = out_dataset;
            }


            WebDocumentsCategory output_category = new WebDocumentsCategory(out_dataset);

            output_category.description = "Version of dataset [" + inputPath + "], reduced in size [" + engine.reductionScore.ToString("F2") + "] by HTML node filtration and WebGraph consistency rules.";


            output_category.SetCategoryByDataset(dataset);

            adapter.SaveDataset(output_category, out_pathRoot, WebDomainCategoryFormatOptions.normalizeDomainname | WebDomainCategoryFormatOptions.saveDomainList
                | WebDomainCategoryFormatOptions.saveReadmeFile | WebDomainCategoryFormatOptions.saveAggregate, output);

            engine.SaveReport(output, out_pathRoot + output_category.path, settings);



        }




        [Display(GroupName = "run", Name = "ConstructProjection", ShortName = "", Description = "Constructs projection based weight table")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "What it will do?")]
        /// <summary>
        /// Constructs projection based weight table
        /// </summary>
        /// <param name="loadConfig">The load configuration.</param>
        /// <param name="outputName">Name of the output.</param>
        /// <param name="skipIfExists">if set to <c>true</c> [skip if exists].</param>
        /// <remarks>
        /// What it will do?
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase" />
        public void aceOperation_runConstructProjection(
              [Description("--")] String loadConfig = "*",
              [Description("--")] String outputName = "*",
              [Description("--")] Boolean skipIfExists = true)
        {
            if (loadConfig != "*") projectionConstructor.aceOperation_runLoad(loadConfig, true);

            var setup = projectionConstructor.data.CloneViaXML();

            setup.LearnFrom(mainContext.procedureCommons);

            if (outputName != "*") setup.OutputFilename = outputName;
            setup.skipIfExisting = skipIfExists;

            //setup.reportOptions = mainContext.reportOptions;

            ProceduralFolderFor<ProcedureProjectionWTConstruction, SetupProjectionWeightTableConstruction, OperationContext, ExperimentModelExecutionContext> procedures
                = new ProceduralFolderFor<ProcedureProjectionWTConstruction, SetupProjectionWeightTableConstruction, OperationContext, ExperimentModelExecutionContext>(mainContext.folds, setup, mainContext.notes, parent);

            //procedures.Execute(mainContext.ParallelThreads, output, null, mainContext);

            proceduralStack.Push(procedures);

        }



        [Display(GroupName = "run", Name = "Test", ShortName = "", Description = "What is purpose of this?")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "What it will do?")]
        /// <summary>What is purpose of this?</summary> 
        /// <remarks><para>What it will do?</para></remarks>
        /// <param name="word">--</param>
        /// <param name="steps">--</param>
        /// <param name="debug">--</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_runTest(
              [Description("--")] String word = "word",
              [Description("--")] Int32 steps = 5,
              [Description("--")] Boolean debug = true)
        {



        }



        [Display(GroupName = "run", Name = "CreateDSRanks", ShortName = "", Description = "Creates document ranking score table that afterwards may be consumed by document selection procedure")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will deploy current document selector, evaluate folds and create the ranking tables?")]
        /// <summary>
        /// Creates document ranking score table that afterwards may be consumed by document selection procedure
        /// </summary>
        /// <param name="loadConfig">The load configuration.</param>
        /// <param name="outputName">Name of the output.</param>
        /// <param name="skipIfExists">if set to <c>true</c> [skip if exists].</param>
        /// <remarks>
        /// It will deploy current document selector, evaluate folds and create the ranking tables?
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase" />
        public void aceOperation_runCreateDSRanks(
              [Description("--")] String loadConfig = "*",
              [Description("--")] String outputName = "*",
              [Description("--")] Boolean skipIfExists = true,
             [Description("--")]  String description = " ")
        {
            if (loadConfig != "*") docSelection.aceOperation_runLoad(loadConfig, true);
            SetupDocumentSelection setup = docSelection.data.CloneViaXML();
            setup.descriptionAppendix.Add(description);
            setup.LearnFrom(mainContext.procedureCommons);

            if (outputName != "*") setup.OutputFilename = outputName;
            setup.skipIfExisting = skipIfExists;

            //setup.reportOptions = mainContext.reportOptions;

            ProceduralFolderFor<ProcedureCreateScoreSet, SetupDocumentSelection, OperationContext, ExperimentModelExecutionContext> procedures
                = new ProceduralFolderFor<ProcedureCreateScoreSet, SetupDocumentSelection, OperationContext, ExperimentModelExecutionContext>(mainContext.folds, setup, mainContext.notes, parent);

            proceduralStack.Push(procedures);

            // contextSet = procedures.Execute(mainContext.ParallelThreads, output, contextSet, mainContext);
        }





        [Display(GroupName = "run", Name = "CWPAnalysis", ShortName = "", Description = "It will perform CWP analysis over specified dataset")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "What it will do?")]
        /// <summary>It will perform CWP analysis over specified dataset</summary> 
        /// <remarks><para>What it will do?</para></remarks>
        /// <param name="dataset">--</param>
        /// <param name="steps">--</param>
        /// <param name="debug">--</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_runCWPAnalysis(
              [Description("--")] String dataset = "word",
              [Description("--")] Int32 steps = 5,
              [Description("--")] Boolean debug = true)
        {

            SetupFeatureCWPAnalysis setup = analysis.data.CloneViaXML(); //docClassification.data.CloneViaXML();

            setup.LearnFrom(mainContext.procedureCommons);


            //setup.reportOptions = mainContext.reportOptions;

            ProceduralFolderFor<ProcedureFeatureCWPAnalysis, SetupFeatureCWPAnalysis, OperationContext, ExperimentModelExecutionContext> procedures
    = new ProceduralFolderFor<ProcedureFeatureCWPAnalysis, SetupFeatureCWPAnalysis, OperationContext, ExperimentModelExecutionContext>(mainContext.folds, setup, mainContext.notes, parent);

            //  contextSet = procedures.Execute(mainContext.ParallelThreads, output, contextSet, mainContext);

            proceduralStack.Push(procedures);
        }




        [Display(GroupName = "run", Name = "ReportOnResources", ShortName = "", Description = "Generates reports on precompiled resources for each fold")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "What it will do?")]
        /// <summary>Generates reports on precompiled resources for each fold</summary> 
        /// <remarks><para>What it will do?</para></remarks>
        /// <param name="word">--</param>
        /// <param name="steps">--</param>
        /// <param name="debug">--</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_runReportOnResources(
              [Description("--")] String word = "word",
              [Description("--")] Int32 steps = 5,
              [Description("--")] Boolean debug = true)
        {

            SetupDocumentSelection setup = docSelection.data.CloneViaXML();

            ProceduralFolderFor<ProcedureCreateScoreSet, SetupDocumentSelection, OperationContext, ExperimentModelExecutionContext> procedures
                = new ProceduralFolderFor<ProcedureCreateScoreSet, SetupDocumentSelection, OperationContext, ExperimentModelExecutionContext>(mainContext.folds, setup, mainContext.notes, parent);


            foreach (var p in procedures)
            {
                p.Open();


                //ExperimentDataSetFoldContextPair<OperationContext> o_pair = new ExperimentDataSetFoldContextPair<OperationContext>(p.fold, new OperationContext());
                //o_pair.context.DeployDataSet(p.fold, output);
                FeatureVectorDictionaryWithDimensions dictWithDim = DocumentRankingExtensions.MergeDSRankings(p.fold_notes.folder, "", output, "*_ranking.xml");

                var t = dictWithDim.Values.MakeTable(dictWithDim.dimensions, "Rankings", p.description, mainContext.truthTable.label_index);

                p.fold_notes.SaveDataTable(t);
                //t.GetReportAndSave(p.fold_notes.folder, imbACE.Core.application.)


                var dictFWT = WeightDictionaryTools.MergeWeightDictionaries(p.fold_notes.folder, "", output, "*_wt.xml");

                var tfw = dictFWT.Values.MakeTable(dictFWT.dimensions, "WeightTable", p.description);

                p.fold_notes.SaveDataTable(tfw);

                p.Close();
            }



        }








        [Display(GroupName = "make", Name = "CombinedDSRanks", ShortName = "", Description = "Combines two or more precompiled document selection ranks")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "What it will do?")]
        /// <summary>
        /// Combines two or more precompiled document selection ranks
        /// </summary>
        /// <param name="inputNames">comma separated list of DS rank file names, leave empty if search pattern is used</param>
        /// <param name="searchPattern">file search pattern to select source files, leave * if no file search should be performed</param>
        /// <param name="compression">vector dimensions compression operation, i.e. how scores should be combined into single dimension</param>
        /// <param name="outputName">Name of the output.</param>
        /// <param name="doRankingFusion">if set to <c>true</c> [do ranking fusion].</param>
        /// <remarks>
        /// What it will do?
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase" />
        public void aceOperation_makeCombineDSRanks(
                  [Description("Space separated list of DS rank file names, leave empty if search pattern is used")] String inputNames = " ",
                  [Description("vector dimensions compression operation, i.e. how scores should be combined into single dimension")] operation compression = operation.avg,
                  [Description("Name of output Document Selection Rank file. Leave * to assign name as combination of input files")] String outputName = "*",
                  [Description("If true, it will perform ranking fusion instead of simple score fusion")] Boolean doRankingFusion = true,
                  [Description("file search pattern to select source files, leave * if no file search should be performed")] String searchPattern = "*"
                  )
        {


            SetupDocumentSelection setup = docSelection.data.CloneViaXML();

            ProceduralFolderFor<ProcedureCreateScoreSet, SetupDocumentSelection, OperationContext, ExperimentModelExecutionContext> procedures
                = new ProceduralFolderFor<ProcedureCreateScoreSet, SetupDocumentSelection, OperationContext, ExperimentModelExecutionContext>(mainContext.folds, setup, mainContext.notes, parent);

            outputName = DocumentSelectResult.CheckAndMakeFilename(outputName);

            foreach (var p in procedures)
            {

                p.Open();


                DocumentSelectResult resultOut = new DocumentSelectResult();

                var fl = mainContext.resourceProvider.GetResourceFiles(inputNames, p.fold);

                List<DocumentSelectResult> results = DocumentRankingExtensions.LoadDSRankings(fl, p.notes);

                resultOut = results.Fusion(compression, doRankingFusion, true, p.notes);

                String pt = mainContext.resourceProvider.SetResourceFilePath(outputName, p.fold);

                resultOut.saveObjectToXML(pt);

                p.Close();
            }



        }




        [Display(GroupName = "run", Name = "ClassificationWithDS", ShortName = "", Description = "Classification procedure with document selection using precompiled ranking file")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "Loads external document selection score table, applies query filter over input dataset and performs classification evaluation")]
        /// <summary>Classification procedure with document selection using precompiled ranking file</summary> 
        /// <remarks><para>Loads external document selection score table, applies query filter over input dataset and performs classification evaluation</para></remarks>
        /// <param name="outputName">Name for result</param>
        /// <param name="DSFilename">Name of Document Selection ranking file</param>
        /// <param name="DSCount">Number of documents to be selected</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_runClassificationWithDS(
              [Description("Name for result")] String outputName = "word",
              [Description("Name of Document Selection ranking file")] String DSFilename = "",
              [Description("Number of documents to be selected")] Int32 DSCount = 5,
              [Description("Query options")]  DocumentSelectQueryOptions options = DocumentSelectQueryOptions.none,
              [Description("Skip existing experiment")]  Boolean skipExisting = true)
        {

            SetupDocumentClassification setup = docClassification.data.CloneViaXML();

            setup.LearnFrom(mainContext.procedureCommons);

            setup.OutputFilename = outputName;

            setup.documentSelectQuery = docSelection.data.ranking.query.CloneViaXML();

            //docClassification.data.documentSelectQuery.

            // setup.documentSelectQuery = docSelection.data.ranking.query.CloneViaXML();

            setup.documentSelectQuery.PrecompiledScoresFilename = DSFilename;

            setup.documentSelectQuery.SizeLimit = DSCount;

            if (options != DocumentSelectQueryOptions.none)
            {
                setup.documentSelectQuery.options = options;
            }

            //setup.reportOptions = mainContext.reportOptions;

            ProceduralFolderFor<ProcedureClassification, SetupDocumentClassification, OperationContext, ExperimentModelExecutionContext> procedures
    = new ProceduralFolderFor<ProcedureClassification, SetupDocumentClassification, OperationContext, ExperimentModelExecutionContext>(mainContext.folds, setup, mainContext.notes, parent);

            //  contextSet = procedures.Execute(mainContext.ParallelThreads, output, contextSet, mainContext);

            proceduralStack.Push(procedures);
        }





        [Display(GroupName = "run", Name = "WTConstruct", ShortName = "", Description = "Runs a custom weight table construction procedure over currently set main context")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will load weight table constructor configuration, check if there is already a table and rebuild it")]
        /// <summary>
        /// Runs a custom weight table construction procedure over currently set main context
        /// </summary>
        /// <param name="loadConfig">The load configuration.</param>
        /// <param name="outputName">Name of the output.</param>
        /// <param name="skipIfExists">if set to <c>true</c> [skip if exists].</param>
        /// <remarks>
        /// It will load weight table constructor configuration, check if there is already a table and rebuild it
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase" />
        public void aceOperation_runWTConstruct(
              [Description("--")] String loadConfig = "*",
              [Description("--")] String outputName = "*",
              [Description("--")] Boolean skipIfExists = true)
        {

            if (loadConfig != "*") wtConstructor.aceOperation_runLoad(loadConfig, true);

            SetupWeightTableConstruction setup = wtConstructor.data.CloneViaXML();
            setup.LearnFrom(mainContext.procedureCommons);

            if (outputName != "*") setup.OutputFilename = outputName;
            setup.skipIfExisting = skipIfExists;



            ProceduralFolderFor<ProcedureForWTConstruction, SetupWeightTableConstruction, OperationContext, ExperimentModelExecutionContext> procedures
                = new ProceduralFolderFor<ProcedureForWTConstruction, SetupWeightTableConstruction, OperationContext, ExperimentModelExecutionContext>(mainContext.folds, setup, mainContext.notes, parent);

            proceduralStack.Push(procedures);

            //contextSet = procedures.Execute(mainContext.ParallelThreads, output, contextSet, mainContext);

        }




        [Display(GroupName = "run", Name = "Execute", ShortName = "", Description = "Runs current procedural stack")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will execute prepared procedural stacks")]
        /// <summary>
        /// Runs current procedural stack
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="threads">The threads.</param>
        /// <remarks>
        /// It will execute prepared procedural stacks
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase" />
        public void aceOperation_runExecute(
              [Description("Execution options")] ProceduralStackOptions options = ProceduralStackOptions.none,
              [Description("Allowed number of threads")] Int32 threads = -1)
        {

            String prefix = "";
            Int32 i = 0;
            Int32 c = proceduralStack.Count;

            Dictionary<string, OperationContext> currentContextSet = contextSet;


            if (options.HasFlag(ProceduralStackOptions.skipExistingExperiment))
            {


                String path = mainContext.notes.folder.path;

                DirectoryInfo di = mainContext.notes.folder;
                // List<DirectoryInfo> enumDirs = di.EnumerateDirectories(mainContext.runName).ToList();

                if (di.Exists)
                {
                    var fi = di.GetFiles(XMLEXPORT_ClassificationResults).FirstOrDefault();

                    if (fi != null)
                    {

                        output.log("SKIP EXISTING _" + mainContext.runName + "_ experiment. Dumping [" + proceduralStack.Count + "] stacked procedures");

                        proceduralStack.Clear();
                    }
                    else
                    {

                    }
                }
                else
                {

                }
            }

            while (proceduralStack.Count > 0)
            {

                prefix = "[" + i + "/" + c + "]"; //  proceduralStack.Count 

                imbSCI.Core.screenOutputControl.logToConsoleControl.setAsOutput(output, prefix, ConsoleColor.Cyan);

                var procedures = proceduralStack.Pop();
                if (threads == -1) threads = mainContext.ParallelThreads;

                procedures.SetExecutionOptions(options);

                currentContextSet = procedures.Execute<OperationContext, ExperimentModelExecutionContext>(threads, mainContext.notes, currentContextSet, mainContext);

            }

            contextSet = currentContextSet;
        }





        public Dictionary<string, OperationContext> contextSet { get; set; } = new Dictionary<string, OperationContext>();

        public folderNode folder { get; set; }

        public nlpOperationsPlugin(folderNode _folder, IAceOperationSetExecutor __parent) : base(__parent)
        {
            folder = _folder;
            //_wtConstructor = new becGlobalWeightTableConstructionExtension(_folder, __parent);
            _docSelection = new becDocumentSelectionExtension(_folder, this);
            _wtConstructor = new becGlobalWeightTableConstructionExtension(_folder, this);
            _docClassification = new becClassificationExtension(_folder, this);
            _projectionConstructor = new becProjectionWeightTableExtension(_folder, this);

            _analysis = new becFeatureCWPAnalysisExtension(_folder, this);


        }


    }
}