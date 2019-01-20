using imbNLP.Project.Operations.Core;
using imbNLP.Project.Operations.Setups;
using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Documents.Analysis;
using imbNLP.Toolkit.Documents.DatasetStructure;
using imbNLP.Toolkit.Documents.FeatureAnalytics;
using imbNLP.Toolkit.Documents.FeatureAnalytics.Core;
using imbNLP.Toolkit.ExperimentModel;
using imbNLP.Toolkit.Processing;
using imbSCI.Core.math.range.histogram;
using imbSCI.Core.reporting;
using imbSCI.DataComplex.tables;
using imbSCI.Graph.Graphics;
using imbSCI.Graph.Graphics.HeatMap;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;

namespace imbNLP.Project.Operations.Procedures
{
    public class ProcedureFeatureCWPAnalysis : ProcedureBaseFor<SetupFeatureCWPAnalysis, OperationContext, ExperimentModelExecutionContext>
    {
        OperationEntityEngine entityOperation { get; set; } // = new OperationEntityEngine(entityMethod, subnotes, output);

        OperationCorpusEngine corpusOperation { get; set; }

        public ProcedureFeatureCWPAnalysis()
        {

        }


        FeatureCWPAnalysis analysis { get; set; }

        public override void DeployCustom()
        {
            name = setup.OutputFilename;

            entityOperation = new OperationEntityEngine(setup.renderForEvaluation, notes, notes);
            corpusOperation = new OperationCorpusEngine(setup.corpusForEvaluation, setup.featureMethod.constructor, notes, notes);


            componentsWithRequirements.Add(entityOperation);
            componentsWithRequirements.Add(corpusOperation);

            analysis = new FeatureCWPAnalysis(setup.analysisSettings);
            

            requirements = CheckRequirements();
            requirements.MayUseSelectedFeatures = true;
            requirements.MayUseVectorSpaceCategories = true;
        }

        public override ExperimentDataSetFoldContextPair<OperationContext> Execute(ILogBuilder logger, OperationContext executionContextMain = null, ExperimentModelExecutionContext executionContextExtra = null)
        {

            ExperimentDataSetFoldContextPair<OperationContext> output = new ExperimentDataSetFoldContextPair<OperationContext>(fold, executionContextMain);

            Open();


            output.context.DeployDataSet(fold, logger);

            entityOperation.TextRendering(output.context, notes, requirements.MayUseTextRender);

            corpusOperation.SpaceModelPopulation(output.context, notes);

            corpusOperation.SpaceModelCategories(output.context, notes);


            FeatureFilterAndWeightModelAnalysis fwmAnalysis = new FeatureFilterAndWeightModelAnalysis(output.context.spaceModel, setup.WeightModels, setup.FilterModels);



            corpusOperation.FeatureSelection(output.context, notes);


            corpusOperation.VectorSpaceConstruction(output.context, notes, true);

            corpusOperation.FeatureVectorConstruction(output.context, notes);




            fwmAnalysis.ExecuteAnalysis(output.context, logger, fold_notes.folder_feature);


            //if (setup.tasks.HasFlag(CWPAnalysisReportsEnum.reportTermDistribution))
            //{

            //    var model = output.context.spaceModel.categories.GetHeatMapMatrix();
            //    HeatMapRender heatMapRender = new HeatMapRender();
            //    heatMapRender.RenderAndSave(model, fold_notes.folder_feature.pathFor("category_overlap_beforeFS", imbSCI.Data.enums.getWritableFileMode.overwrite, "Heat map showing overlaping terms and their frequencies, before feature selection"));

            //}



            if (setup.tasks.HasFlag(CWPAnalysisReportsEnum.reportDatasetStructure))
            {

                DatasetStructureReport datasetStructureReport = DatasetStructureReport.MakeStructureReport(fold, fold.name);

                datasetStructureReport.Compute();
                datasetStructureReport.Publish(fold_notes.folder, true, true, true);

            }

            if (setup.tasks.HasFlag(CWPAnalysisReportsEnum.reportDatasetMetrics))
            {
                ContentAnalytics contentAnalytics = new ContentAnalytics(fold_notes.folder_entity);

                var Metrics = contentAnalytics.ProduceMetrics(fold.name, fold, output.context, logger);
                Metrics.ReportHTMLTags(fold_notes.folder_entity, fold.name);
                Metrics.ReportSample(fold_notes.folder_entity, fold.name, 1000);
                Metrics.ReportTokens(fold_notes.folder_corpus, fold.name, 1000);
                Metrics.GetDataTable(fold_notes.name).GetReportAndSave(fold_notes.folder_entity, null, "Dataset");
            }


            if (setup.tasks.HasFlag(CWPAnalysisReportsEnum.reportTermDistribution))
            {
                imbSCI.Core.math.range.matrix.HeatMapModel model = output.context.spaceModel.categories.GetHeatMapMatrix();


                model.GetDataTable("CategoryFreqOverlap", "Overlaping terms and their frequencies").GetReportAndSave(fold_notes.folder, null, "CategoryOverlap");

                try
                {

                    HeatMapRender heatMapRender = new HeatMapRender();
                    heatMapRender.style.accronimLength = 3;
                    heatMapRender.style.BaseColor = Color.Black;
                    heatMapRender.style.fieldHeight = 50;
                    heatMapRender.style.fieldWidth = 50;
                    var svg = heatMapRender.Render(model);
                    svg.Save(fold_notes.folder_feature.pathFor("category_overlap_afterFS.svg", imbSCI.Data.enums.getWritableFileMode.overwrite, "Heat map showing overlaping terms and their frequencies"));
                    svg.SaveJPEG(fold_notes.folder_feature.pathFor("category_overlap_afterFS.jpg", imbSCI.Data.enums.getWritableFileMode.overwrite, "Heat map showing overlaping terms and their frequencies"));

                }
                catch (Exception ex)
                {
                    logger.log(ex.Message);
                }

                List<histogramModel> models = new List<histogramModel>();

                foreach (var cat in output.context.spaceModel.categories)
                {
                    var hist = cat.GetHistogram(20);
                    models.Add(hist);
                    DataTable dt_hist = hist.GetDataTableForFrequencies();

                    dt_hist.GetReportAndSave(fold_notes.folder, null, "histogram_table_" + cat.name);

                    string h_p = fold_notes.folder_feature.pathFor(cat.name + "_term_distribution.svg", imbSCI.Data.enums.getWritableFileMode.overwrite, "Histogram with term distribution for category [" + cat.name + "]");


                    // File.WriteAllText(h_p, hist.GetSVGChart());
                }

                models.BlendHistogramModels(fold.name).GetReportAndSave(fold_notes.folder, null, "histogram_all");

            }

            if (setup.tasks.HasFlag(CWPAnalysisReportsEnum.reportCWPAnalytics))
            {

                analysis.Prepare(output.context.spaceModel, logger);

                analysis.Analysis(fold_notes);

            }


            Close();

            return output;
        }
    }
}