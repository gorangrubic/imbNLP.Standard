using imbNLP.Toolkit.Entity;
using imbNLP.Toolkit.Feature.Settings;
using imbNLP.Toolkit.Planes.Core;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Stemmers.Shaman;
using imbNLP.Toolkit.Weighting.Global;
using imbSCI.Core.reporting;
using System;

namespace imbNLP.Toolkit.Planes
{


    [Flags]
    public enum PlanesReportOptions
    {
        none = 0,
        report_corpusDictionary = 1,
        report_documentDictionary = 2,
        report_categoryDictionary = 4,

        report_selectedFeatures = 8,

        report_documentBoWModels = 16,
        report_categoryBoWModels = 32,

        report_featureVectors = 64,

        report_fold_stats = 128,
        report_fold_contentAnalysis = 256,
        report_fold_textrender = 512,
    }

    /// <summary>
    /// Settings covering the whole 
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Planes.Core.IPlaneSettings" />
    public class PlanesMethodSettings : IPlaneSettings
    {




        /// <summary>
        /// Gets default configuration
        /// </summary>
        /// <returns></returns>
        public static PlanesMethodSettings GetDefaultSettings()
        {
            PlanesMethodSettings output = new PlanesMethodSettings();

            output.entityMethod.instructions.Add(DocumentRenderInstruction.GetDescriptionInstruction());
            output.entityMethod.instructions.Add(DocumentRenderInstruction.GetTitleInstruction());
            output.entityMethod.instructions.Add(DocumentRenderInstruction.GetBodyTextInstruction());
            output.entityMethod.blenderOptions = DocumentBlenderFunctionOptions.binaryAggregation | DocumentBlenderFunctionOptions.pageLevel;
            output.entityMethod.filterFunctionName = ""; // nameof(DocumentEntropyFunction);
            output.entityMethod.filterLimit = 5;



            output.corpusMethod.stemmer = nameof(EnglishStemmer);
            output.corpusMethod.tokenizer = nameof(TokenizerBasic);
            output.corpusMethod.transliterationRuleSetId = "";

            #region PREPARE Weighting model
            var weightModel = new Corpora.FeatureWeightModel();
            weightModel.LocalFunction = new Weighting.Local.TermFrequencyFunction();

            var globalFactor = new Corpora.FeatureWeightFactor();
            globalFactor.Settings.functionName = nameof(IDFElement);
            weightModel.GlobalFactors.Add(globalFactor);

            output.corpusMethod.weightModel = weightModel;
            #endregion


            var featureFilter = new Corpora.FeatureFilter();
            featureFilter.limit = 8000;
            featureFilter.functionSettings = new GlobalFunctionSettings();
            featureFilter.functionSettings.functionName = nameof(CollectionTDPElement);
            featureFilter.functionSettings.weight = 1.0;
            featureFilter.functionSettings.tdpFactor = Weighting.Metrics.TDPFactor.chi;
            output.corpusMethod.filter = featureFilter;

            /*
            output.vectorMethod.constructor = new Feature.Settings.FeatureVectorConstructorSettings();
            dimensionSpecification dimSpec = new dimensionSpecification();
            dimSpec.functionName = nameof(CosineSimilarityFunction);
            dimSpec.type = FeatureVectorDimensionType.similarityFunction;
            output.vectorMethod.constructor.labelDimensions.Add(dimSpec);
            */

            output.vectorMethod.constructor = new Feature.Settings.FeatureVectorConstructorSettings();
            dimensionSpecification dimSpec = new dimensionSpecification();
            //dimSpec.functionName = nameof(CosineSimilarityFunction);
            dimSpec.type = FeatureVectorDimensionType.directTermWeight;
            output.vectorMethod.constructor.featureDimensions.Add(dimSpec);


            output.featureMethod.classifierSettings.type = Classifiers.ClassifierType.multiClassSVM;
            output.featureMethod.classifierSettings.lossFunctionForTraining = Accord.MachineLearning.VectorMachines.Learning.Loss.L2;


            /*
            output.featureMethod.classifierSettings.type = Classifiers.ClassifierType.kNearestNeighbors;
            output.featureMethod.classifierSettings.lossFunctionForTraining = Accord.MachineLearning.VectorMachines.Learning.Loss.L2;
            output.featureMethod.classifierSettings.kNN_k = 4;
            */

            return output;

        }

        public String signature_sufix { get; set; } = "";

        public void Describe(ILogBuilder logger)
        {

        }



        /// <summary>
        /// Initializes a new instance of the <see cref="PlanesMethodSettings"/> class.
        /// </summary>
        public PlanesMethodSettings()
        {

        }

        /// <summary>
        /// Gets or sets the entity method settings
        /// </summary>
        /// <value>
        /// The entity method.
        /// </value>
        public EntityPlaneMethodSettings entityMethod { get; set; } = new EntityPlaneMethodSettings();

        /// <summary>
        /// Gets or sets the corpus method settings
        /// </summary>
        /// <value>
        /// The corpus method.
        /// </value>
        public CorpusPlaneMethodSettings corpusMethod { get; set; } = new CorpusPlaneMethodSettings();

        /// <summary>
        /// Gets or sets the vector method settings
        /// </summary>
        /// <value>
        /// The vector method.
        /// </value>
        public VectorPlaneMethodSettings vectorMethod { get; set; } = new VectorPlaneMethodSettings();

        /// <summary>
        /// Gets or sets the feature method settings
        /// </summary>
        /// <value>
        /// The feature method.
        /// </value>
        public FeaturePlaneMethodSettings featureMethod { get; set; } = new FeaturePlaneMethodSettings();
        public String cachePath { get; set; } = "";
    }

}