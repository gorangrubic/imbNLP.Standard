using imbNLP.Project.Operations.Core;
using imbNLP.Project.Operations.Setups;
using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.ExperimentModel;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Weighting;
using imbSCI.Core.reporting;
using System;
using System.IO;

namespace imbNLP.Project.Operations.Procedures
{
    /// <summary>
    /// Constructs Weight Table according to settings
    /// </summary>
    /// <seealso cref="imbNLP.Project.Operations.Core.ProcedureBaseFor{imbNLP.Project.Operations.Setups.SetupWeightTableConstruction, imbNLP.Project.Operations.Data.OperationContext, imbNLP.Project.Operations.Data.OperationContext}" />
    public class ProcedureForWTConstruction : ProcedureBaseFor<SetupWeightTableConstruction, OperationContext, ExperimentModelExecutionContext>
    {
        private OperationEntityEngine entityOperation { get; set; } // = new OperationEntityEngine(entityMethod, subnotes, output);

        private OperationCorpusEngine corpusOperation { get; set; } // = new OperationCorpusEngine(corpusMethod, featureMethod.constructor, subnotes, output);

        public override void DeployCustom()
        {
            entityOperation = new OperationEntityEngine(setup.entityMethod, notes, notes);
            corpusOperation = new OperationCorpusEngine(setup.corpusMethod, null, notes, notes);

            componentsWithRequirements.Add(entityOperation);
            componentsWithRequirements.Add(corpusOperation);

            requirements = CheckRequirements();
            requirements.MayUseSelectedFeatures = true;
        }

        public override ExperimentDataSetFoldContextPair<OperationContext> Execute(ILogBuilder logger, OperationContext executionContextMain = null, ExperimentModelExecutionContext executionContextExtra = null)
        {
            ExperimentDataSetFoldContextPair<OperationContext> output = new ExperimentDataSetFoldContextPair<OperationContext>(fold, executionContextMain);

            Open();

            String p_m = FeatureWeightModel.GetModelDefinitionFilename(setup.OutputFilename, fold_notes.folder);

            String p_d = FeatureWeightModel.GetModelDataFilename(setup.OutputFilename, fold_notes.folder);

            String w_t = WeightDictionary.GetDictionaryFilename(setup.OutputFilename, fold_notes.folder);

            Boolean skip = false;

            if (setup.skipIfExisting)
            {
                if (File.Exists(p_m) && File.Exists(p_d) && File.Exists(w_t))
                {
                    logger.log("WeightTable [" + p_d + "] found, skipping the operation");
                    skip = true;
                }
            }

            if (!skip)
            {
                output.context.DeployDataSet(fold, logger);

                entityOperation.TextRendering(output.context, notes);

                /*
                entityOperation.TextPreblendFilter(output.context, notes);

                entityOperation.TextBlending(output.context, notes);
                */

                corpusOperation.SpaceModelPopulation(output.context, notes);

                corpusOperation.SpaceModelCategories(output.context, notes);

                corpusOperation.FeatureSelection(output.context, notes, requirements.MayUseSelectedFeatures);

                output.context.SelectedFeatures.Save(fold_notes.folder, notes, setup.OutputFilename + "_fs");

                //corpusOperation.weightModel.

                corpusOperation.weightModel.PrepareTheModel(output.context.spaceModel, logger);

                var wt_s = corpusOperation.weightModel.GetElementFactors(output.context.SelectedFeatures.GetKeys(), output.context.spaceModel);

                wt_s.Save(fold_notes.folder, notes, setup.OutputFilename);

                corpusOperation.weightModel.Save(setup.OutputFilename, fold_notes.folder, notes);

                OperationContextReport reportOperation = new OperationContextReport();
                reportOperation.DeploySettingsBase(notes);

                reportOperation.GenerateReports(output.context, setup.reportOptions, notes);
            }

            Close();

            return output;
        }
    }
}