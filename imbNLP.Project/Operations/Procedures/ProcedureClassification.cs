using imbNLP.Project.Operations.Core;
using imbNLP.Project.Operations.Data;
using imbNLP.Project.Operations.Setups;
using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Documents.DatasetStructure;
using imbNLP.Toolkit.Documents.Ranking;
using imbNLP.Toolkit.Documents.Ranking.Data;
using imbNLP.Toolkit.ExperimentModel;
using imbSCI.Core.extensions.text;
using imbSCI.Core.files;
using imbSCI.Core.math.classificationMetrics;
using imbSCI.Core.reporting;
using imbSCI.Data;
using imbSCI.DataComplex.tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Project.Operations.Procedures
{

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="imbNLP.Project.Operations.Core.ProcedureBaseFor{imbNLP.Project.Operations.Setups.SetupDocumentClassification, imbNLP.Project.Operations.Data.OperationContext, imbNLP.Toolkit.ExperimentModel.ExperimentModelExecutionContext}" />
    public class ProcedureClassification : ProcedureBaseFor<SetupDocumentClassification, OperationContext, ExperimentModelExecutionContext>
    {
        OperationEntityEngine entityOperation { get; set; } // = new OperationEntityEngine(entityMethod, subnotes, output);

        OperationCorpusEngine corpusOperation { get; set; }

        OperationClassificationEngine classificationOperation { get; set; }

        public override void DeployCustom()
        {
            name = setup.OutputFilename;

            entityOperation = new OperationEntityEngine(setup.entityMethod, notes, notes);
            corpusOperation = new OperationCorpusEngine(setup.corpusMethod, setup.featureMethod.constructor, notes, notes);

            classificationOperation = new OperationClassificationEngine(setup.featureMethod, notes, notes);

            componentsWithRequirements.Add(entityOperation);
            componentsWithRequirements.Add(corpusOperation);
            componentsWithRequirements.Add(classificationOperation);



            requirements = CheckRequirements();
            requirements.MayUseSelectedFeatures = true;
            //requirements.MayUseVectorSpaceCategories = true;
        }


        public override void ExecuteFinal(Dictionary<string, OperationContext> executionContextDict, ExperimentModelExecutionContext executionContextExtra, ILogBuilder logger)
        {
            executionContextExtra.CloseExperiment(logger, 0);



        }

        public override ExperimentDataSetFoldContextPair<OperationContext> Execute(ILogBuilder logger, OperationContext executionContextMain = null, ExperimentModelExecutionContext executionContextExtra = null)
        {
            ExperimentDataSetFoldContextPair<OperationContext> output = new ExperimentDataSetFoldContextPair<OperationContext>(fold, executionContextMain);

            Open();


            if (!setup.documentSelectQuery.PrecompiledScoresFilename.Trim().isNullOrEmpty())
            {

                String precompFile = DocumentSelectResult.CheckAndMakeFilename(setup.documentSelectQuery.PrecompiledScoresFilename);

                var p = executionContextExtra.resourceProvider.GetResourceFile(precompFile, fold);

                //var p = executionContextExtra.resourceProvider.folder.findFile(precompFile, SearchOption.AllDirectories);

                DocumentSelectResult scores = DocumentSelectResult.LoadFromFile(p, logger);  // objectSerialization.loadObjectFromXML<DocumentSelectResult>(path, logger);

                if (scores != null)
                {
                    scores.SaveReport(fold_notes.folder.pathFor("DSScores_loaded.txt", imbSCI.Data.enums.getWritableFileMode.overwrite));

                    scores = setup.documentSelectQuery.ExecuteLimit(scores, logger);

                    IEnumerable<string> assignedIDs = scores.items.Select(x => x.AssignedID);

                    scores.SaveReport(fold_notes.folder.pathFor("DSScores_applied.txt", imbSCI.Data.enums.getWritableFileMode.overwrite));

                    fold.DataSetSubSet(assignedIDs.ToList(), true, true);
                }
                else
                {
                    throw new ArgumentException("DSelection file failed: " + setup.documentSelectQuery.PrecompiledScoresFilename);

                    logger.log(" _ DocumentSelect failed for [" + name + "]");
                }

            }

            classificationReport tmpReport = new classificationReport();

            String dsReportName = fold.name + setup.documentSelectQuery.PrecompiledScoresFilename + setup.documentSelectQuery.SizeLimit;


            DatasetStructureReport dsReport = DatasetStructureReport.MakeStructureReport(fold, dsReportName);
            dsReport.Publish(fold_notes.folder, true, true);

            tmpReport.SetReportDataFields(dsReport);

            if (!output.context.IsDatasetDeployed)
            {

                output.context.DeployDataSet(fold, logger);

                entityOperation.TextRendering(output.context, notes, requirements.MayUseTextRender);


                corpusOperation.SpaceModelPopulation(output.context, notes);

                if (requirements.MayUseSpaceModelCategories)
                {
                    corpusOperation.SpaceModelCategories(output.context, notes);
                }

            }

            tmpReport.SetReportDataFields(output.context, false);

            corpusOperation.FeatureSelection(output.context, notes);


            corpusOperation.VectorSpaceConstruction(output.context, notes, requirements.MayUseVectorSpaceCategories);

            corpusOperation.FeatureVectorConstruction(output.context, notes);


            if (setup.reportOptions.HasFlag(OperationReportEnum.randomSampledDemo))
            {
                logger.log("-- generating random sample report");
                var data_wm = imbNLP.Toolkit.Reporting.ReportGenerators.MakeWeightModelDemoTable(output.context.spaceModel, corpusOperation.weightModel, output.context.SelectedFeatures, 5, "DemoForWeightModel", "Diagnostic report for picked sample");
                data_wm.GetReportAndSave(fold_notes.folder);
                var data_fs = imbNLP.Toolkit.Reporting.ReportGenerators.MakeWeightModelDemoTable(output.context.spaceModel, corpusOperation.filter.WeightModel, output.context.SelectedFeatures, 5, "DemoForFeatureSelection", "Diagnostic report for feature selection filter sample");
                data_fs.GetReportAndSave(fold_notes.folder);
            }

            classificationOperation.PerformClassification(output.context, executionContextExtra.truthTable, setup.dataSetMode, notes);


            corpusOperation.weightModel.DiagnosticDump(fold_notes.folder, logger);

            //classificationOperation.classifier.

            classificationEvalMetricSet evaluationMetrics = executionContextExtra.truthTable.EvaluateTestResultsToMetricSet(output.context.testResults, setup.OutputFilename + "-" + notes.folder.name, logger);

            if (setup.ExportEvaluationAsDocumentSelectionResult)
            {
                Toolkit.Feature.FeatureVectorDictionaryWithDimensions dict = executionContextExtra.truthTable.GetEvaluationAsFeatureVectorDictionary(output.context.testResults, setup.OutputFilename, logger, setup.ExportEvaluationCorrectScore, setup.ExportEvaluationIncorrectScore);
                String out_ds = setup.ExportEvaluationToFilename.Replace("*", "");
                dict.Save(fold_notes.folder, out_ds.or(setup.OutputFilename), logger);
                //executionContextExtra.resourceProvider.folder
                dict.Save(notes.folder, out_ds.or(setup.OutputFilename), logger);
            }


            DataTableTypeExtended<classificationEval> inclassEvalTable = new DataTableTypeExtended<classificationEval>("inclass_evaluation", "Test results, per class");
            evaluationMetrics.GetAllEntries().ForEach(x => inclassEvalTable.AddRow(x));
            inclassEvalTable.AddRow(evaluationMetrics.GetSummary("Sum"));
            notes.SaveDataTable(inclassEvalTable, notes.folder_classification);

            classificationReport averagedReport = new classificationReport(evaluationMetrics, setup.averagingMethod);

            averagedReport.Classifier = classificationOperation.classifier.GetSignature(); // featureMethod.classifierSettings.name; // FeatureMethod.classifier.name;
            averagedReport.saveObjectToXML(notes.folder_classification.pathFor(averagedReport.Name + ".xml", imbSCI.Data.enums.getWritableFileMode.overwrite, "Serialized classification evaluation results summary"));
            averagedReport.ReportToLog(notes);

            averagedReport.SetReportDataFields(output.context, true);
            averagedReport.data.Merge(tmpReport.data);

            averagedReport.SetReportDataFields(classificationOperation.classifier, corpusOperation.filter, corpusOperation.weightModel);



            executionContextExtra.testSummaries.Add(averagedReport);




            OperationContextReport reportOperation = new OperationContextReport();
            reportOperation.DeploySettingsBase(notes);

            reportOperation.GenerateReports(output.context, setup.reportOptions, notes);

            /*
            if (setup.reportOptions.HasFlag(OperationReportEnum.reportClassification))
            {

                Dictionary<string, List<FeatureVectorWithLabelID>> byCategory = executionContextExtra.truthTable.GroupByTrueCategory(executionContextMain.testResults);

                objectTable<classificationReport> tbl = new objectTable<classificationReport>(nameof(classificationReport.Name), "inclass_" + executionContextExtra.runName);
                classificationReport macroAverage = new classificationReport("AVG-" + executionContextExtra.runName);
                foreach (KeyValuePair<string, List<FeatureVectorWithLabelID>> pair in byCategory)
                {
                    var cReport = executionContextExtra.EvaluateTestResults(pair.Value, pair.Key + "-" + executionContextExtra.runName, logger);

                    cReport.Classifier = classificationOperation.classifier.GetSignature(); // classifier.name;
                    cReport.Comment = "Tr/Ts [" + executionContextMain.trainingSet.Count + "]:[" + executionContextMain.testSet.Count + "]";
                    String path = notes.folder_classification.pathFor(pair.Key + "_result.xml", imbSCI.Data.enums.getWritableFileMode.overwrite, "Serialized evaluation result within category [" + pair.Key + "]", true);

                    macroAverage.AddValues(cReport);

                    tbl.Add(cReport);
                }
                //  macroAverage.DivideValues(byCategory.Keys.Count);

                tbl.Add(macroAverage);

                notes.SaveDataTable(tbl.GetDataTable(), notes.folder_classification);

            }*/

            Close();

            return output;
        }
    }
}