using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.ExperimentModel.CrossValidation;
using imbNLP.Toolkit.Feature;
using imbNLP.Toolkit.Planes;
using imbNLP.Toolkit.Planes.Core;
using imbSCI.Core.data;
using imbSCI.Core.files.folders;
using imbSCI.Core.math.classificationMetrics;
using imbSCI.Core.reporting;
using imbSCI.Data.collection.math;
using imbSCI.DataComplex.tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.ExperimentModel
{


    /// <summary>
    /// Main context for an experiment execution
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Planes.Core.IPlaneContext" />
    public class ExperimentModelExecutionContext : PlaneContextBase, IPlaneContext
    {

        public PlanesReportOptions reportOptions { get; set; } = PlanesReportOptions.report_categoryDictionary
          | PlanesReportOptions.report_corpusDictionary | PlanesReportOptions.report_documentDictionary
         | PlanesReportOptions.report_selectedFeatures | PlanesReportOptions.report_fold_textrender;


        public Int32 DictionaryReportLimit { get; set; } = 500;

        public aceAuthorNotation signature { get; set; } = new aceAuthorNotation();

        public classificationMetricComputation averagingMethod { get; set; } = classificationMetricComputation.macroAveraging;



        public ExperimentModelExecutionContext(String _runName)
        {
            runName = _runName;
        }

        /// <summary>
        /// Prepares the notes.
        /// </summary>
        /// <param name="_rootFolder">The root folder.</param>
        /// <param name="_experimentDescription">The experiment description.</param>
        /// <param name="logger">The logger.</param>
        public void PrepareNotes(folderNode _rootFolder, String _experimentDescription, ILogBuilder logger)
        {
            experimentRootFolder = _rootFolder;
            notes = new ToolkitExperimentNotes(experimentRootFolder, _experimentDescription);

            imbSCI.Core.screenOutputControl.logToConsoleControl.setAsOutput(notes, "Note");
        }

        /// <summary>
        /// Prepares the dataset.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <param name="validationModel">The validation model.</param>
        public void PrepareDataset(List<WebSiteDocumentsSet> dataset, CrossValidationModel validationModel, Int32 pageLimit)
        {
            validationModel.Describe(notes);

            truthTable = new ExperimentTruthTable();

            notes.logStartPhase("[-] Creating k-fold crossvalidation datasets", "The input dataset with [" + dataset.Count + "] categories, is sliced into k=[" + validationModel.K + "] mutually exclusive folds, of ~equal size");

            // vetting the dataset
            foreach (var ds in dataset)
            {
                ds.RemoveEmptyDocuments(notes, pageLimit);
            }

            // ------------------ creation of Experiment Folds ------------------ //
            folds = new ExperimentDataSetFolds();
            folds.Deploy(validationModel, dataset, notes);

            truthTable.Deploy(dataset, notes);

            /*
            testReportsByFold = new Dictionary<string, List<classificationReport>>();
            foreach (var fold in folds)
            {
                testReportsByFold.Add(fold.name, new List<classificationReport>());
            }*/

            notes.logEndPhase();
        }

        /// <summary>
        /// Generates the final reports and read me files
        /// </summary>
        public void CloseExperiment(ILogBuilder logger, long startOfLog)
        {
            DataTableTypeExtended<classificationReport> summaryTable = new DataTableTypeExtended<classificationReport>("Test results", "k-fold cross valudation results");

            classificationReport sumRow = new classificationReport(runName);

            //    classificationEvalMetricSet metric = new classificationEvalMetricSet("Total", truthTable.labels_without_unknown);

            foreach (classificationReport s in testSummaries)
            {
                summaryTable.AddRow(s);
                //metric = metric + s;


                sumRow.AddValues(s);

            }

            sumRow.DivideValues(testSummaries.Count);
            summaryTable.AddRow(sumRow);


            summaryTable.GetReportAndSave(notes.folder, signature);

            finalReport = sumRow;

            sumRow.ReportToLog(logger);
            sumRow.ReportToLog(notes);

            logger.log("Experiment completed");

            notes.SaveNote("note.txt");

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


        /// <summary>
        /// Truth table - document to class associations for the complete dataset
        /// </summary>
        /// <value>
        /// The truth table.
        /// </value>
        public ExperimentTruthTable truthTable { get; set; }

        #region GLOBAL INPUT DATA 

        /// <summary>
        /// Complete input dataset
        /// </summary>
        /// <value>
        /// The dataset.
        /// </value>
        public List<WebSiteDocumentsSet> dataset { get; set; }

        /// <summary>
        /// Dataset k-fold crossvalidation iterations
        /// </summary>
        /// <value>
        /// The folds.
        /// </value>
        public ExperimentDataSetFolds folds { get; set; }
        public string runName { get; internal set; }

        #endregion




    }
}
