using imbNLP.Project.Operations.Core;
using imbNLP.Project.Operations.Setups;
using imbNLP.Project.Operations.Tools;
using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Documents.FeatureAnalytics;
using imbNLP.Toolkit.Documents.Ranking;
using imbNLP.Toolkit.Documents.Ranking.Data;
using imbNLP.Toolkit.ExperimentModel;
using imbSCI.Core.files;
using imbSCI.Core.reporting;
using System;
using System.IO;

namespace imbNLP.Project.Operations.Procedures
{


    public class ProcedureCreateScoreSet : ProcedureBaseFor<SetupDocumentSelection, OperationContext, ExperimentModelExecutionContext>
    {
        OperationEntityEngine entityOperation { get; set; } // = new OperationEntityEngine(entityMethod, subnotes, output);

        OperationCorpusEngine corpusOperation { get; set; } // = new OperationCorpusEngine(corpusMethod, featureMethod.constructor, subnotes, output);

        DocumentRankingMethod ranking { get; set; }

        public override void DeployCustom()
        {
            entityOperation = new OperationEntityEngine(setup.renderForEvaluation, notes, notes);
            corpusOperation = new OperationCorpusEngine(setup.corpusForEvaluation, null, notes, notes);

            ranking = setup.ranking.CloneViaXML(notes);

            ranking.model.Deploy();

            ranking.Describe(notes);




            componentsWithRequirements.Add(entityOperation);
            componentsWithRequirements.Add(corpusOperation);
            componentsWithRequirements.Add(ranking);

            requirements = CheckRequirements();
        }

        public override ExperimentDataSetFoldContextPair<OperationContext> Execute(ILogBuilder logger, OperationContext executionContextMain = null, ExperimentModelExecutionContext executionContextExtra = null)
        {
            ExperimentDataSetFoldContextPair<OperationContext> output = new ExperimentDataSetFoldContextPair<OperationContext>(fold, executionContextMain);

            Open();

            Boolean skip = false;

            String fn = setup.OutputFilename;
            // String p_m = fold_notes.folder.pathFor(fn.ensureEndsWith("_ranking.xml"), imbSCI.Data.enums.getWritableFileMode.none);



            if (setup.skipIfExisting)
            {
                String f_n = DocumentSelectResult.CheckAndMakeFilename(fn);

                f_n = executionContextExtra.resourceProvider.GetResourceFile(f_n, fold); // .folder.findFile(f_n, SearchOption.AllDirectories);
                skip = DocumentRankingExtensions.EvaluateSavedDSRanking(f_n, logger, 0.01);
            }

            if (!skip)
            {


                output.context.DeployDataSet(fold, logger);


                //if (!output.context.IsTextRendered)
                //{
                entityOperation.TextRendering(output.context, notes);

                //entityOperation.TextPreblendFilter(output.context, notes);

                //entityOperation.TextBlending(output.context, notes);
                // }

                //if (!output.context.spaceModel.IsModelReady)
                //{

                corpusOperation.SpaceModelPopulation(output.context, notes);

                corpusOperation.SpaceModelCategories(output.context, notes);


                corpusOperation.FeatureSelection(output.context, notes, requirements.MayUseSelectedFeatures);

                corpusOperation.VectorSpaceConstruction(output.context, notes, requirements.MayUseSpaceModelCategories);







                //    }

                logger.log("Document selection computation");


                DocumentSelectResult drmContext = output.context.PrepareContext(ranking, fold_notes.folder, logger);
                drmContext = ranking.ExecuteEvaluation(drmContext, logger);




                foreach (String l in setup.descriptionAppendix)
                {
                    drmContext.description += Environment.NewLine + l;
                }



                fn = DocumentSelectResult.CheckAndMakeFilename(fn);
                fn = executionContextExtra.resourceProvider.SetResourceFilePath(fn, fold);
                // f_n = executionContextExtra.resourceProvider.folder.pathFor(f_n, imbSCI.Data.enums.getWritableFileMode.overwrite, "");
                String xmlModel = objectSerialization.ObjectToXML(drmContext);
                File.WriteAllText(fn, xmlModel);


                //corpusOperation.weightModel.PrepareTheModel(output.context.spaceModel);


                /*
                var dataset = corpusOperation.weightModel.SaveModelDataSet();


                String fn = setup.OutputFilename;
                String p_m = notes.folder.pathFor(fn.ensureEndsWith("_model.xml"), imbSCI.Data.enums.getWritableFileMode.autoRenameThis);

                String p_d = notes.folder.pathFor(fn.ensureEndsWith("_data.xml"), imbSCI.Data.enums.getWritableFileMode.autoRenameThis);


                String xmlModel = objectSerialization.ObjectToXML(setup.corpusMethod.weightModel);


                String xmlData = objectSerialization.ObjectToXML(dataset);


                File.WriteAllText(p_m, xmlModel);

                File.WriteAllText(p_d, xmlData);

                /*
                corpusOperation.weightModel.saveObjectToXML(
                    notes.folder.pathFor(setup.OutputFilename.ensureEndsWith("_model.xml"), imbSCI.Data.enums.getWritableFileMode.autoRenameThis, "Weight model [" + corpusOperation.weightModel.shortName + "]"));

                dataset.saveObjectToXML(notes.folder.pathFor(setup.OutputFilename.ensureEndsWith("_data.xml"), imbSCI.Data.enums.getWritableFileMode.autoRenameThis, "Weight model [" + corpusOperation.weightModel.shortName + "]"));
                */




                OperationContextReport reportOperation = new OperationContextReport();
                reportOperation.DeploySettingsBase(notes);

                reportOperation.GenerateReports(output.context, setup.reportOptions, notes);
            }

            Close();

            return output;
        }
    }
}