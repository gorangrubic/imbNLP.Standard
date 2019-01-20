
using imbNLP.Toolkit.Classifiers.Core;
using imbNLP.Toolkit.Documents.Ranking.Core;
using imbNLP.Toolkit.ExperimentModel;
using imbNLP.Toolkit.Feature;
using imbNLP.Toolkit.Planes.Core;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Planes
{

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Planes.Core.IPlaneMethodDesign" />
    public class FeaturePlaneMethodDesign : MethodDesignBase, IPlaneMethodDesign
    {

        /// <summary>
        /// Gets or sets the classifier.
        /// </summary>
        /// <value>
        /// The classifier.
        /// </value>
        public IClassifier classifier { get; set; }

        public override ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Prepares everything for operation
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="_notes">The notes.</param>
        /// <param name="logger">The logger.</param>
        public void DeploySettings(IPlaneSettings settings, ToolkitExperimentNotes _notes, ILogBuilder logger)
        {
            DeploySettingsBase(_notes);

            FeaturePlaneMethodSettings featureSettings = settings as FeaturePlaneMethodSettings;

            classifier = featureSettings.classifierSettings.GetClassifier();

            featureSettings.Describe(notes);
            classifier.Describe(notes);

            CloseDeploySettingsBase();

        }

        /// <summary>
        /// Executes the plane method, invoking contained functions according to the settings
        /// </summary>
        /// <param name="inputContext">The input context - related to this plane.</param>
        /// <param name="generalContext">General execution context, attached to the <see cref="T:imbNLP.Toolkit.Planes.PlanesMethodDesign" /></param>
        /// <param name="logger">The logger.</param>
        /// <returns>
        /// Retur
        /// </returns>
        public IPlaneContext ExecutePlaneMethod(IPlaneContext inputContext, ExperimentModelExecutionContext generalContext, ILogBuilder logger)
        {
            notes.logStartPhase("[4] Feature Plane - execution", "");

            IFeaturePlaneContext context = inputContext as IFeaturePlaneContext;



            foreach (FeatureVector vec in context.featureSpace.documents)
            {
                var associated = context.featureSpace.labelToDocumentAssociations.GetAllLinked(vec);
                if (associated.Any())
                {
                    Int32 lbi = generalContext.truthTable.labels_without_unknown.IndexOf(associated.First().name);

                    FeatureVectorWithLabelID fvl = new FeatureVectorWithLabelID(vec, lbi);
                    context.trainingSet.Add(fvl);

                }
                else
                {
                    context.testSet.Add(vec);
                }
            }

            if (!context.testSet.Any())
            {
                notes.log("TEST SET IS EMPTY ---- APPLYING 1:1 EXPERIMENT SHEME: training and test set are the same");

                context.trainingSet.ForEach(x => context.testSet.Add(x.vector));
            }


            if ((!context.trainingSet.Any()))
            {

                notes.log("TRAINING SET EMPTY ---- APPLYING 1:1 EXPERIMENT SHEME: training and test set are the same");


            }
            else
            {

                notes.log("Training [" + classifier.name + "] with [" + context.trainingSet.Count + "] feature vectors.");
                classifier.DoTraining(context.trainingSet, logger);

                notes.log("Testing [" + classifier.name + "] with [" + context.testSet.Count + "] feature vectors.");

                context.testResults = new List<FeatureVectorWithLabelID>();
                foreach (FeatureVector fv in context.testSet)
                {
                    Int32 result = classifier.DoSelect(fv, logger);
                    FeatureVectorWithLabelID fvl = new FeatureVectorWithLabelID(fv, result);
                    context.testResults.Add(fvl);
                }




                /*
                Dictionary<string, List<FeatureVectorWithLabelID>> byCategory = generalContext.truthTable.GroupByTrueCategory(context.testResults);
                objectTable<classificationReport> tbl = new objectTable<classificationReport>(nameof(classificationReport.Name), "inclass_" + generalContext.runName);
                classificationReport macroAverage = new classificationReport("AVG-" + generalContext.runName);
                foreach (KeyValuePair<string, List<FeatureVectorWithLabelID>> pair in byCategory)
                {
                    var cReport = generalContext.EvaluateTestResults(pair.Value, pair.Key + "-" + generalContext.runName, logger);

                    cReport.Classifier = classifier.name;
                    cReport.Comment = "Tr/Ts [" + context.trainingSet.Count + "]:[" + context.testSet.Count + "]";
                    String path = notes.folder_classification.pathFor(pair.Key + "_result.xml", imbSCI.Data.enums.getWritableFileMode.overwrite, "Serialized evaluation result within category [" + pair.Key + "]", true);

                    macroAverage.AddValues(cReport);

                    tbl.Add(cReport);
                }
                //  macroAverage.DivideValues(byCategory.Keys.Count);

                tbl.Add(macroAverage);

                notes.SaveDataTable(tbl.GetDataTable(), notes.folder_classification);
                */


            }




            notes.logEndPhase();

            return context;
        }
    }

}