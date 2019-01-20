using imbNLP.Toolkit.Classifiers.Core;
using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Documents.Ranking;
using imbNLP.Toolkit.Documents.Ranking.Core;
using imbNLP.Toolkit.ExperimentModel;
using imbNLP.Toolkit.Feature;

using imbNLP.Toolkit.Planes;
using imbNLP.Toolkit.Processing;
using imbSCI.Core.extensions.data;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Project.Operations
{

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Planes.MethodDesignBase" />
    /// <seealso cref="imbNLP.Toolkit.Documents.Ranking.IHasProceduralRequirements" />
    public class OperationClassificationEngine : MethodDesignBase, IHasProceduralRequirements
    {




        public OperationClassificationEngine()
        {

        }

        public OperationClassificationEngine(FeaturePlaneMethodSettings featureSettings, ToolkitExperimentNotes _notes, ILogBuilder logger)
        {

            DeploySettings(featureSettings, _notes, logger);
        }


        /// <summary>
        /// Prepares everything for operation
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="_notes">The notes.</param>
        /// <param name="logger">The logger.</param>
        public void DeploySettings(FeaturePlaneMethodSettings featureSettings, ToolkitExperimentNotes _notes, ILogBuilder logger)
        {
            DeploySettingsBase(_notes);


            classifier = featureSettings.classifierSettings.GetClassifier();

            featureSettings.Describe(notes);
            classifier.Describe(notes);

            CloseDeploySettingsBase();

        }




        /// <summary>
        /// Gets or sets the classifier.
        /// </summary>
        /// <value>
        /// The classifier.
        /// </value>
        public IClassifier classifier { get; set; }

        /// <summary>
        /// Queries factors for preprocessing requirements
        /// </summary>
        /// <param name="requirements">The requirements.</param>
        /// <returns></returns>
        public override ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null)
        {
            if (requirements == null) requirements = new ScoreModelRequirements();

            //requirements.MayUseFeatureSpace = true;
            requirements.MayUseTextRender = true;
            requirements.MayUseSelectedFeatures = true;

            //foreach (IScoreModelFactor factor in Factors)
            //{
            //    factor.CheckRequirements(requirements);

            //}

            return requirements;
        }


        //public ExperimentTruthTable ConstructTruthTable(FeatureSpace space, ILogBuilder log)
        //{

        //    ExperimentTruthTable output = new ExperimentTruthTable();

        //    output.Deploy(space, )

        //    List<String> labels = space.labelToDocumentAssociations.GetAllDistinctNames(true);

        //    List<FeatureVectorWithLabelID> dataset = new List<FeatureVectorWithLabelID>();

        //    foreach (FeatureVector vec in space.documents)
        //    {
        //        var associated = space.labelToDocumentAssociations.GetAllLinked(vec);

        //        Int32 lbi = -1;


        //        FeatureVectorWithLabelID fvl = null;

        //        if (associated.Any())
        //        {
        //            lbi = labels.IndexOf(associated.First().name);
        //        }
        //        else
        //        {
        //            lbi = labels.IndexOf(SpaceLabel.UNKNOWN);
        //        }

        //        fvl = new FeatureVectorWithLabelID(vec, lbi);
        //        dataset.Add(fvl);
        //    }

        //    output.Deploy(dataset, log);
        //    return output;

        //}


        public void DistributeTrainingAndTestSets(ClassificationDatasetSeparationEnum distributionRule, ExperimentTruthTable truthTable, FeatureSpace featureSpace, ILogBuilder log, List<FeatureVectorWithLabelID> testSet, List<FeatureVectorWithLabelID> trainingSet)
        {
            log.log("Spliting data [" + distributionRule.ToString() + "]");

            List<FeatureVectorWithLabelID> __testSet = new List<FeatureVectorWithLabelID>();
            List<FeatureVectorWithLabelID> __trainingSet = new List<FeatureVectorWithLabelID>();


            foreach (FeatureVector vec in featureSpace.documents)
            {
                var associated = featureSpace.labelToDocumentAssociations.GetAllLinked(vec);

                Int32 lbi = -1;


                FeatureVectorWithLabelID fvl = null;

                if (associated.Any())
                {
                    lbi = truthTable.labels_without_unknown.IndexOf(associated.First().name);
                }

                fvl = new FeatureVectorWithLabelID(vec, lbi);

                if (lbi == -1)
                {
                    __testSet.Add(fvl);
                }
                else
                {

                    __trainingSet.Add(fvl);
                }

            }



            if (!__testSet.Any())
            {
                notes.log("TEST SET IS EMPTY ---- APPLYING 1:1 EXPERIMENT SHEME: training and test set are the same");
                __trainingSet.ForEach(x => __testSet.Add(x));

            }
            else
            {
                if (distributionRule.HasFlag(ClassificationDatasetSeparationEnum.TestLabeled))
                {
                    testSet.AddRange(__trainingSet, true);
                }

                if (distributionRule.HasFlag(ClassificationDatasetSeparationEnum.TestUnlabeled))
                {
                    testSet.AddRange(__testSet, true);
                }

                if (distributionRule.HasFlag(ClassificationDatasetSeparationEnum.TrainingLabeled))
                {
                    trainingSet.AddRange(__trainingSet, true);
                }

                if (distributionRule.HasFlag(ClassificationDatasetSeparationEnum.TrainingUnlabeled))
                {
                    trainingSet.AddRange(__testSet, true);
                }

            }


            log.log("Training [" + trainingSet.Count + "] - Testing [" + testSet.Count + "]");

            //switch (distributionRule)
            //{
            //    case ClassificationDatasetSeparationEnum.TrainingAll_TestAll:
            //       
            //        break;
            //    case ClassificationDatasetSeparationEnum.TrainingAll_TestUnlabeled:

            //        break;
            //    case ClassificationDatasetSeparationEnum.TrainingLabeled_TestAll:
            //        trainingSet.ForEach(x => testSet.Add(x.vector));
            //        break;
            //    case ClassificationDatasetSeparationEnum.TrainingLabeled_TestUnlabeled:
            //        // just fine
            //        break;
            //}

        }


        public void PerformClassification(OperationContext context, ExperimentTruthTable truthTable, ClassificationDatasetSeparationEnum distributionRule, ILogBuilder log)
        {

            log.log("Performing classification");

            if (truthTable == null)
            {
                truthTable = new ExperimentTruthTable();
                notes.log(":: DEPLOYING IN-FOLD TRUTH TABLE ::");
                log.log(":: DEPLOYING IN-FOLD TRUTH TABLE ::");
                truthTable.Deploy(context.featureSpace, context.spaceModel.labels.Select(x => x.name).ToList(), log);
            }


            DistributeTrainingAndTestSets(distributionRule, truthTable, context.featureSpace, log, context.testSet, context.trainingSet);

            if (!context.trainingSet.Any())
            {

                notes.log("TRAINING SET EMPTY ---- APPLYING 1:1 EXPERIMENT SHEME: training and test set are the same");


            }
            else
            {

                notes.log("Training [" + classifier.name + "] with [" + context.trainingSet.Count + "] feature vectors.");
                classifier.DoTraining(context.trainingSet, log);

                log.log("Training [" + classifier.name + "] completed.");


                notes.log("Testing [" + classifier.name + "] with [" + context.testSet.Count + "] feature vectors.");

                context.testResults = new List<FeatureVectorWithLabelID>();

                var ts = context.testSet.Select(x => x.vector);

                List<Int32> distinctResults = new List<int>();


                foreach (FeatureVector fv in ts)
                {
                    Int32 result = classifier.DoSelect(fv, log);
                    if (!distinctResults.Contains(result)) distinctResults.Add(result);
                    FeatureVectorWithLabelID fvl = new FeatureVectorWithLabelID(fv, result);
                    context.testResults.Add(fvl);
                }

                if (distinctResults.Count < truthTable.labels_without_unknown.Count)
                {
                    List<String> no_match_labels = truthTable.labels_without_unknown.ToList();
                    foreach (Int32 d in distinctResults)
                    {
                        no_match_labels.Remove(truthTable.labels_without_unknown[d]);
                    }

                    log.log("WARNING --- [" + classifier.name + "] ONLY [" + distinctResults.Count + "] of [" + truthTable.labels_without_unknown.Count + "] were assigned by the classifier");
                    foreach (String l in no_match_labels)
                    {
                        log.log("Class [" + l + "] received no assigment");

                    }

                    foreach (var v in context.testSet)
                    {
                        var dist = v.GetDistinctValuesAtVector();
                        if (dist.Count < 2)
                        {
                            log.log("Test vector [" + v.name + "] has [" + dist.Count + "] distinct values at [" + v.dimensions.Length + "] dimensions!");
                        }
                    }

                    foreach (var v in context.trainingSet)
                    {
                        var dist = v.GetDistinctValuesAtVector();
                        if (dist.Count < 2)
                        {
                            log.log("Training vector [" + v.name + "] has [" + dist.Count + "] distinct values at [" + v.dimensions.Length + "] dimensions!");
                        }
                    }

                }

                log.log("Testing [" + classifier.name + "] completed.");






            }
        }
    }
}