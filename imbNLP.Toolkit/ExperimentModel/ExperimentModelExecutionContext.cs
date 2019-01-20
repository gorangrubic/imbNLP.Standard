using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.ExperimentModel.CrossValidation;
using imbNLP.Toolkit.Feature;
using imbNLP.Toolkit.Planes.Core;
using imbSCI.Core.data;
using imbSCI.Core.extensions.table;
using imbSCI.Core.files;
using imbSCI.Core.files.folders;
using imbSCI.Core.math.classificationMetrics;
using imbSCI.Core.reporting;
using imbSCI.Data;
using imbSCI.Data.collection.math;
using imbSCI.DataComplex.tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.ExperimentModel
{






    /// <summary>
    /// Main context for an experiment execution
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Planes.Core.IPlaneContext" />
    public class ExperimentModelExecutionContext : PlaneContextBase, IOperationExecutionContext, IPlaneContext
    {

        //public OperationReportEnum reportOptions { get; set; } = OperationReportEnum.reportRenderingLayers | OperationReportEnum.reportPreblendFilter
        //    | OperationReportEnum.reportBlendedRenders | OperationReportEnum.reportFeatures | OperationReportEnum.tableExportText | OperationReportEnum.saveSetupXML;

        public ExperimentResourceProvider resourceProvider { get; set; } = new ExperimentResourceProvider();

        public String description { get; set; } = "";

        public ProcedureSetupCommon procedureCommons { get; set; } = new ProcedureSetupCommon();


        public Int32 ParallelThreads { get; set; } = 0;


        public Int32 DictionaryReportLimit { get; set; } = 500;


        //  public String dataSetSource { get; set; } = "";


        public aceAuthorNotation signature { get; set; } = new aceAuthorNotation();

        public CrossValidationModel crossValidation { get; set; }

        public classificationMetricComputation averagingMethod { get; set; } = classificationMetricComputation.macroAveraging;

        public ExperimentModelExecutionContext()
        {

        }

        public ExperimentModelExecutionContext(String _runName)
        {
            runName = _runName;
        }

        /// <summary>
        /// Deploys the specified report options.
        /// </summary>
        /// <param name="_reportOptions">The report options.</param>
        /// <param name="_signature">The signature.</param>
        /// <param name="_averagingMethod">The averaging method.</param>
        /// <param name="_rootFolder">The root folder.</param>
        /// <param name="_experimentDescription">The experiment description.</param>
        /// <param name="logger">The logger.</param>
        public void Deploy(aceAuthorNotation _signature, classificationMetricComputation _averagingMethod, folderNode _rootFolder, String _experimentDescription, ILogBuilder logger)
        {
            //  reportOptions = _reportOptions;
            signature = _signature;
            averagingMethod = _averagingMethod;
            description = _experimentDescription;
            experimentRootFolder = _rootFolder;
            notes = new ToolkitExperimentNotes(experimentRootFolder, _experimentDescription);


            notes.log("Starting [" + runName + "]");
            notes.log("[" + description + "]:[" + signature + "]");
            //ToolkitExperimentNotes.[" + _experimentDescription + "]:[" + signature + "]");
            imbSCI.Core.screenOutputControl.logToConsoleControl.setAsOutput(notes, "Note");
        }

        ///// <summary>
        ///// Prepares the notes.
        ///// </summary>
        ///// <param name="_rootFolder">The root folder.</param>
        ///// <param name="_experimentDescription">The experiment description.</param>
        ///// <param name="logger">The logger.</param>
        //public void PrepareNotes()
        //{

        //}


        /// <summary>
        /// Prepares the dataset.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <param name="validationModel">The validation model.</param>
        public void PrepareDataset(IEnumerable<WebSiteDocumentsSet> __dataset, CrossValidationModel validationModel)
        {
            crossValidation = validationModel;
            if (validationModel != null)
            {
                validationModel.Describe(notes);
            }
            else
            {
                validationModel = new CrossValidationModel();
                validationModel.SingleFold = true;
            }

            truthTable = new ExperimentTruthTable();

            dataset = new ExperimentDataSetFold(__dataset.ToList(), runName);



            // ------------------ creation of Experiment Folds ------------------ //
            if (validationModel != null)
            {

                if (notes != null) notes.logStartPhase("[-] Creating k-fold crossvalidation datasets", "The input dataset with [" + dataset.Count + "] categories, is sliced into k=[" + validationModel.K + "] mutually exclusive folds, of ~equal size");
                folds = new ExperimentDataSetFolds();
                folds.Deploy(validationModel, dataset, notes);

            }
            else
            {


            }

            truthTable.Deploy(dataset, notes);

            /*
            testReportsByFold = new Dictionary<string, List<classificationReport>>();
            foreach (var fold in folds)
            {
                testReportsByFold.Add(fold.name, new List<classificationReport>());
            }*/

            if (notes != null) notes.logEndPhase();
        }

        /// <summary>
        /// Generates the final reports and read me files
        /// </summary>
        public void CloseExperiment(ILogBuilder logger, long startOfLog)
        {

            if (!testSummaries.Any())
            {
                logger.log("No experiment procedures performes");

                return;
            }
            DataTableTypeExtended<classificationReport> summaryTable = new DataTableTypeExtended<classificationReport>("Test results", "k-fold cross valudation results");

            classificationReport sumRow = new classificationReport(runName);

            sumRow.Comment = runName + ", " + description;


            //    classificationEvalMetricSet metric = new classificationEvalMetricSet("Total", truthTable.labels_without_unknown);

            foreach (classificationReport s in testSummaries)
            {
                summaryTable.AddRow(s);
                //metric = metric + s;

                if (sumRow.Classifier.isNullOrEmpty())
                {
                    sumRow.Classifier = s.Classifier;
                }

                sumRow.AddValues(s);

            }




            sumRow.DivideValues(testSummaries.Count);

            sumRow.SetReportDataFields(crossValidation, this);

            summaryTable.SetDescription(description);

            summaryTable.SetAdditionalInfoEntry("RunName", runName);
            summaryTable.SetAdditionalInfoEntry("Description", description);
            summaryTable.SetAdditionalInfoEntry("Averaging", averagingMethod.ToString());

            summaryTable.AddRow(sumRow);




            summaryTable.GetReportAndSave(notes.folder, signature);

            finalReport = sumRow;

            //sumRow.ReportToLog(logger);
            sumRow.ReportToLog(notes);

            objectSerialization.saveObjectToXML(sumRow, notes.folder.pathFor("results.xml", imbSCI.Data.enums.getWritableFileMode.overwrite, "Experiment results", true));


            logger.log("Experiment completed");

            notes.SaveNote("note");

            String logPrintout = logger.GetContent(startOfLog);
            String p = notes.folder.pathFor("log.txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "Log printout during experiment execution");

            File.WriteAllText(p, logPrintout);

            experimentRootFolder.generateReadmeFiles(signature);

        }

        /// <summary>
        /// Instance of the inter-fold average
        /// </summary>
        /// <value>
        /// The final report.
        /// </value>
        public classificationReport finalReport { get; set; }



        public classificationReport EvaluateTestResults(List<FeatureVectorWithLabelID> testResults, String _testName, ILogBuilder logger)
        {
            classificationReport cReport = truthTable.EvaluateTestResults(testResults, _testName, notes);

            //  testSummaries.Add(cReport);

            //testSummaries.Add(cReport.GetSummary("Sum of " + _testName));

            //classificationEval summary = metric.GetSummary("Sum of " + _testName);
            //testSummaries.Add(summary);
            //classificationReport report = new classificationReport()
            return cReport;
        }


        public Dictionary<String, List<classificationReport>> testReportsByFold { get; set; } = new Dictionary<string, List<classificationReport>>();

        public ConcurrentList<classificationReport> testSummaries { get; set; } = new ConcurrentList<classificationReport>();


        [XmlIgnore]
        public folderNode experimentRootFolder { get; set; }

        [XmlIgnore]
        public ToolkitExperimentNotes notes { get; set; }

        public ExperimentTruthTable truthTable { get; private set; }




        #region GLOBAL INPUT DATA 

        /// <summary>
        /// Input dataset, after initial filtration of empty sites and low-page count ones
        /// </summary>
        /// <value>
        /// The dataset.
        /// </value>
        public ExperimentDataSetFold dataset { get; set; }

        /// <summary>
        /// Dataset k-fold crossvalidation iterations, extracted from the input dataset
        /// </summary>
        /// <value>
        /// The folds.
        /// </value>
        public ExperimentDataSetFolds folds { get; set; }
        public string runName { get; internal set; }

        #endregion




    }
}
