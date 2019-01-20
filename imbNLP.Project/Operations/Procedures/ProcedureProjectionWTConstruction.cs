using imbNLP.Project.Operations.Core;
using imbNLP.Project.Operations.Setups;
using imbNLP.Project.Operations.Tools;
using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Documents.Ranking;
using imbNLP.Toolkit.Documents.Ranking.Data;
using imbNLP.Toolkit.ExperimentModel;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Processing.Relations;
using imbSCI.Core.files;
using imbSCI.Core.reporting;
using System;
using System.IO;

namespace imbNLP.Project.Operations.Procedures
{
    public class ProcedureProjectionWTConstruction : ProcedureBaseFor<SetupProjectionWeightTableConstruction, OperationContext, ExperimentModelExecutionContext>
    {

        OperationEntityEngine primaryEntityOperation { get; set; }

        OperationEntityEngine secondaryEntityOperation { get; set; }

        OperationCorpusEngine corpusOperation { get; set; } // = new OperationCorpusEngine(corpusMethod, featureMethod.constructor, subnotes, output);

        DocumentRankingMethod rankingOperation { get; set; }


        public override void DeployCustom()
        {

            primaryEntityOperation = new OperationEntityEngine(setup.primaryRender, notes, notes);
            secondaryEntityOperation = new OperationEntityEngine(setup.secondaryRender, notes, notes);
            corpusOperation = new OperationCorpusEngine(setup.secondaryModel.corpusForEvaluation, null, notes, notes);
            rankingOperation = setup.secondaryModel.ranking.CloneViaXML(notes);
            rankingOperation.model.Deploy();
            rankingOperation.Describe(notes);

            componentsWithRequirements.Add(primaryEntityOperation);
            componentsWithRequirements.Add(secondaryEntityOperation);
            componentsWithRequirements.Add(corpusOperation);
            componentsWithRequirements.Add(rankingOperation);

            requirements = CheckRequirements();

        }

        public override ExperimentDataSetFoldContextPair<OperationContext> Execute(ILogBuilder logger, OperationContext executionContextMain = null, ExperimentModelExecutionContext executionContextExtra = null)
        {
            ExperimentDataSetFoldContextPair<OperationContext> output = new ExperimentDataSetFoldContextPair<OperationContext>(fold, executionContextMain);

            Open();

            Boolean skip = false;

            //  String fn = setup.OutputFilename;

            String p_m = WeightDictionary.GetDictionaryFilename(setup.OutputFilename, fold_notes.folder);  //FeatureWeightModel.GetModelDefinitionFilename(setup.OutputFilename, fold_notes.folder);

            //String p_d = FeatureWeightModel.GetModelDataFilename(setup.OutputFilename, fold_notes.folder);


            if (setup.skipIfExisting)
            {
                if (File.Exists(p_m))
                {
                    logger.log("WeightTable [" + p_m + "] found, skipping the operation");
                    skip = true;
                }
            }



            if (!skip)
            {

                notes.log("Rendering primary view");

                // ------------------- PRIMARY CONTEXT

                output.context.DeployDataSet(fold, logger);

                primaryEntityOperation.TextRendering(output.context, notes);

                //primaryEntityOperation.TextPreblendFilter(output.context, notes);

                //primaryEntityOperation.TextBlending(output.context, notes);


                corpusOperation.SpaceModelPopulation(output.context, notes);

                corpusOperation.SpaceModelCategories(output.context, notes);

                corpusOperation.FeatureSelection(output.context, notes, requirements.MayUseSelectedFeatures);



                OperationContext primaryContext = output.context;

                // ------------------- SECONDARY CONTEXT

                output.context = new OperationContext();

                notes.log("Rendering secondary view");

                output.context.DeployDataSet(fold, logger);

                secondaryEntityOperation.TextRendering(output.context, notes);

                //  secondaryEntityOperation.TextPreblendFilter(output.context, notes);

                // secondaryEntityOperation.TextBlending(output.context, notes);

                corpusOperation.SpaceModelPopulation(output.context, notes);

                corpusOperation.SpaceModelCategories(output.context, notes);

                corpusOperation.FeatureSelection(output.context, notes, requirements.MayUseSelectedFeatures);


                OperationContext secondaryContext = output.context;



                ProjectionDictionary projectionPairs = DocumentRankingTools.ConstructPairDictionary(primaryContext.spaceModel.documents, secondaryContext.spaceModel.documents);

                DocumentSelectResult drmContext = output.context.PrepareContext(rankingOperation, fold_notes.folder, logger);
                drmContext = rankingOperation.ExecuteEvaluation(drmContext, logger);
                drmContext.description = "Document score assigned to the primary text render" + name;
                drmContext.saveObjectToXML(fold_notes.folder.pathFor("DS_" + name + "_projection_score.xml", imbSCI.Data.enums.getWritableFileMode.overwrite, "Projection within [" + name + "] operation"));

                TokenFrequencyAndScoreDictionary tokenFrequencyAndScoreDictionary = ProjectionTools.ProjectPrimaryTermsToScores(projectionPairs, drmContext, logger);

                WeightDictionary wt = tokenFrequencyAndScoreDictionary.ConstructWeightDictionary();
                wt.name = setup.OutputFilename;
                wt.description = "Projected PrimaryView to ScoreTable - WeightTable, constructed from [" + projectionPairs.Count + "] render pairs. Document ranking: " + drmContext.description;

                wt.Save(fold_notes.folder, logger, setup.OutputFilename);

                //                wt.saveObjectToXML(p_m);




            }


            Close();

            return output;
        }
    }
}