using imbNLP.Toolkit.Documents.Ranking.Core;
using imbNLP.Toolkit.ExperimentModel;
using imbNLP.Toolkit.Feature;
using imbNLP.Toolkit.Feature.Settings;
using imbNLP.Toolkit.Planes.Core;
using imbNLP.Toolkit.Reporting;
using imbNLP.Toolkit.Vectors;
using imbSCI.Core.reporting;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Planes
{

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Planes.MethodDesignBase" />
    /// <seealso cref="imbNLP.Toolkit.Planes.Core.IPlaneMethodDesign" />
    public class VectorPlaneMethodDesign : MethodDesignBase, IPlaneMethodDesign
    {
        public FeatureVectorConstructor featureSpaceConstructor { get; set; } = new FeatureVectorConstructor();

        public FeatureVectorConstructorSettings constructorSettings { get; set; }

        public override ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null)
        {
            throw new System.NotImplementedException();
        }

        public void DeploySettings(IPlaneSettings settings, ToolkitExperimentNotes _notes, ILogBuilder logger)
        {
            DeploySettingsBase(_notes);

            VectorPlaneMethodSettings vectorPlaneSettings = (VectorPlaneMethodSettings)settings;
           // constructorSettings = vectorPlaneSettings.constructor;

            if (notes != null) vectorPlaneSettings.Describe(notes);

            CloseDeploySettingsBase();

        }

        /// <summary>
        /// Generates feature vectors 
        /// </summary>
        /// <param name="inputContext">The input context - related to this plane.</param>
        /// <param name="generalContext">General execution context, attached to the <see cref="T:imbNLP.Toolkit.Planes.PlanesMethodDesign" /></param>
        /// <param name="logger">The logger.</param>
        /// <returns>
        /// Retur
        /// </returns>
        public IPlaneContext ExecutePlaneMethod(IPlaneContext inputContext, ExperimentModelExecutionContext generalContext, ILogBuilder logger)
        {
            notes.logStartPhase("[3] Vector Plane - execution", "");

            IVectorPlaneContext context = (IVectorPlaneContext)inputContext;
            FeaturePlaneContext finalContext = new FeaturePlaneContext();
            finalContext.provider.StoreAndReceive(context);

            ICorpusPlaneContext corpusContext = finalContext.provider.GetContext<CorpusPlaneContext>();

            // deploying feature vector space constructor
            featureSpaceConstructor.Deploy(constructorSettings, context.vectorSpace);
            featureSpaceConstructor.Deploy(constructorSettings, corpusContext.SelectedFeatures);

            Dictionary<string, FeatureVector> docByName = new Dictionary<string, FeatureVector>();

            notes.log(":: Constructing feature vectors");
            // constructing the feature vectors
            foreach (IVector vector in context.vectorSpace.documents)
            {
                var fv = featureSpaceConstructor.ConstructFeatureVector(vector);
                docByName.Add(fv.name, fv);
                finalContext.featureSpace.documents.Add(fv);
            }

            foreach (var link in context.LabelToDocumentLinks.links)
            {
                finalContext.featureSpace.labelToDocumentAssociations.Add(docByName[link.NodeB.name], link.NodeA, 1);
            }

            if (generalContext.reportOptions.HasFlag(PlanesReportOptions.report_featureVectors))
            {
                var dt = finalContext.featureSpace.MakeTable(featureSpaceConstructor, "FeatureSpace", "Feature space");
                notes.SaveDataTable(dt, notes.folder_feature);
            }

            notes.logEndPhase();

            return finalContext;
        }
    }

}