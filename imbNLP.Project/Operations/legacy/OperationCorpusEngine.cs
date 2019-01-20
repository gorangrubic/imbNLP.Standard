using imbACE.Core.core;
using imbNLP.Project.Operations.Data;
using imbNLP.Toolkit.Documents.Ranking.Core;
using imbNLP.Toolkit.ExperimentModel;
using imbNLP.Toolkit.Feature;
using imbNLP.Toolkit.Feature.Settings;

using imbNLP.Toolkit.Planes;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Stemmers;
using imbNLP.Toolkit.Typology;
using imbNLP.Toolkit.Vectors;
using imbNLP.Toolkit.Weighting;
using imbNLP.Transliteration.ruleSet;
using imbSCI.Core.extensions.data;
using imbSCI.Core.files;
using imbSCI.Core.math;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Project.Operations.legacy
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Planes.MethodDesignBase" />
    public class OperationCorpusEngine : MethodDesignBase
    {

        //    public OperationReportEnum reportOptions { get; set; } = OperationReportEnum.reportSpaceModel;

        /// <summary>
        /// Gets or sets the tokenizer.
        /// </summary>
        /// <value>
        /// The tokenizer.
        /// </value>
        public ITokenizer tokenizer { get; set; }

        /// <summary>
        /// Gets or sets the stemmer.
        /// </summary>
        /// <value>
        /// The stemmer.
        /// </value>
        public IStemmer stemmer { get; set; }

        /// <summary>
        /// Gets or sets the transliteration.
        /// </summary>
        /// <value>
        /// The transliteration.
        /// </value>
        public transliterationPairSet transliteration { get; set; }

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        public FeatureFilter filter { get; set; }

        /// <summary>
        /// Gets or sets the weight model.
        /// </summary>
        /// <value>
        /// The weight model.
        /// </value>
        public FeatureWeightModel weightModel { get; set; }

        /// <summary>
        /// Gets or sets the space constructor.
        /// </summary>
        /// <value>
        /// The space constructor.
        /// </value>
        public SpaceModelConstructor spaceConstructor { get; set; } = new SpaceModelConstructor();

        /// <summary>
        /// Gets or sets the feature space constructor.
        /// </summary>
        /// <value>
        /// The feature space constructor.
        /// </value>
        public FeatureVectorConstructor featureSpaceConstructor { get; set; } = new FeatureVectorConstructor();

        /// <summary>
        /// Gets or sets the constructor settings.
        /// </summary>
        /// <value>
        /// The constructor settings.
        /// </value>
        public FeatureVectorConstructorSettings constructorSettings { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="OperationCorpusEngine"/> class.
        /// </summary>
        public OperationCorpusEngine()
        {

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="OperationCorpusEngine"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="_notes">The notes.</param>
        /// <param name="logger">The logger.</param>
        public OperationCorpusEngine(CorpusPlaneMethodSettings corpusSettings, FeatureVectorConstructorSettings __constructorSettings, ToolkitExperimentNotes _notes, ILogBuilder logger)
        {
            DeploySettings(corpusSettings, __constructorSettings, _notes, logger);
        }

        /// <summary>
        /// Queries factors for preprocessing requirements
        /// </summary>
        /// <param name="requirements">The requirements.</param>
        /// <returns></returns>
        public override ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null)
        {
            if (requirements == null) requirements = new ScoreModelRequirements();

            if (filter != null)
            {
                if (filter.IsEnabled)
                {
                    requirements.MayUseTextRender = true;
                    //requirements.MayUseFeatureSpace = true;
                }
            }
            //requirements.MayUseFeatureSpace = true;
            //requirements.MayUseTextRender = true;
            //requirements.MayUseSelectedFeatures = true;

            //foreach (IScoreModelFactor factor in Factors)
            //{
            //    factor.CheckRequirements(requirements);

            //}

            return requirements;
        }

        /// <summary>
        /// Prepares everything for operation
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="_notes">The notes.</param>
        /// <param name="logger">The logger.</param>
        public void DeploySettings(CorpusPlaneMethodSettings corpusSettings, FeatureVectorConstructorSettings __constructorSettings, ToolkitExperimentNotes _notes, ILogBuilder logger)
        {
            DeploySettingsBase(_notes);


            stemmer = TypeProviders.stemmerTypes.GetInstance(corpusSettings.stemmer);
            tokenizer = TypeProviders.tokenizerTypes.GetInstance(corpusSettings.tokenizer);
            if (!corpusSettings.transliterationRuleSetId.isNullOrEmpty())
            {
                transliteration = Transliteration.ruleSet.transliteration.GetTransliterationPairSet(corpusSettings.transliterationRuleSetId);
            }

            filter = corpusSettings.filter.CloneViaXML(logger);
            filter.Deploy(logger);


            weightModel = corpusSettings.WeightModel.CloneViaXML(logger);
            weightModel.Deploy(logger);

            if (__constructorSettings != null)
            {
                constructorSettings = __constructorSettings.CloneViaXML(logger);
            }


            //   Describe(_notes);

            CloseDeploySettingsBase();

        }

        public void Describe(ILogBuilder logger)
        {
            if (logger != null)
            {
                logger.AppendPair("Stemmer", stemmer.GetType().Name, true, "\t\t\t");

                logger.AppendPair("Tokenizer", tokenizer.GetType().Name, true, "\t\t\t");

                if (transliteration != null)
                {
                    logger.AppendPair("Transliteration", transliteration.lang_A_id + "-" + transliteration.lang_B_id, true, "\t\t\t");
                }
                else
                {
                    logger.AppendPair("Transliteration", "Disabled", true, "\t\t\t");
                }

                filter.Describe(logger);

                weightModel.Describe(logger);

                if (constructorSettings != null)
                {
                    constructorSettings.Describe(logger);
                }
                else
                {
                    logger.AppendLine("Vector model constructor - not set");
                }
            }

        }

        /// <summary>
        /// Spaces the model population.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        public void SpaceModelPopulation(OperationContext context, ILogBuilder log)
        {
            log.log("Space model population");
            context.stemmContext = new StemmingContext(stemmer);


            // modelling the documents 
            foreach (var pair in context.textDocuments)
            {
                var doc = pair.Value;

                SpaceDocumentModel model = spaceConstructor.ConstructDocument(doc.content, doc.name, context.spaceModel, context.stemmContext, tokenizer);

                foreach (String label in doc.labels)
                {


                    SpaceLabel sLabel = null;
                    sLabel = context.spaceLabels[label];

                    context.spaceModel.LabelToDocumentLinks.Add(sLabel, model, 1);
                }

                context.spaceModel.documents.Add(model);



                if (doc.labels.Contains(SpaceLabel.UNKNOWN))
                {
                    context.spaceModel.terms_unknown_label.MergeDictionary(model.terms);
                }
                else
                {
                    context.spaceModel.terms.MergeDictionary(model.terms);
                }


            }

            log.log("Space model -- documents created [" + context.spaceModel.documents.Count + "]");

        }






        /// <summary>
        /// Features the selection.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        public void FeatureSelection(OperationContext context, ILogBuilder log, Boolean EnableSelection = true)
        {



            if (filter == null)
            {
                EnableSelection = false;
            }
            //else if (filter. == null)
            //{
            //    EnableSelection = false;
            //}


            if (!context.spaceModel.IsModelReady)
            {
                log.log("-- Feature selection function shouldn't be called before creation of the space model.");
            }

            List<KeyValuePair<string, double>> filter_result = new List<KeyValuePair<string, double>>();


            if (EnableSelection)
            {
                filter_result = filter.SelectFeatures(context.spaceModel,log);
            }
            else
            {

            }

            builderForLog textBuilder = new builderForLog();
            filter.Describe(textBuilder);


            context.SelectedFeatures = new WeightDictionary("FS_" + context.name, "Features selected by " + filter.GetSignature() + ". Info: " + textBuilder.GetContent());

            //if (filter.IsEnabled)
            //{


            //}

            if (filter_result.Any())
            {
                foreach (var pair in filter_result)
                {
                    context.SelectedFeatures.AddEntry(pair.Key, pair.Value);
                }

                //if (generalContext.reportOptions.HasFlag(PlanesReportOptions.report_selectedFeatures))
                //{
                //    notes.SaveDataTable(context.SelectedFeatures.MakeTable("selected_features", "Features selected for BoW construction", new List<string>() { filter.function.shortName }, generalContext.DictionaryReportLimit), notes.folder_corpus);
                //}
            }
            else
            {
                String msg = "-- Feature selection function returned zero set. All features [" + context.spaceModel.terms.Count + "] are therefore accepted as selected.";

                context.SelectedFeatures.description += msg;
                log.log(msg);
                var tkns = context.spaceModel.terms.GetTokens();
                foreach (var tkn in tkns)
                {
                    context.SelectedFeatures.AddEntry(tkn, 1);
                }
            }

        }

        /// <summary>
        /// Spaces the model categories.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        public void SpaceModelCategories(OperationContext context, ILogBuilder log)
        {
            context.vectorSpace = new VectorSpace();

            log.log("Space model categories");
            foreach (SpaceLabel label in context.spaceModel.labels)
            {
                var docs = context.spaceModel.LabelToDocumentLinks.GetAllLinked(label);
                if (label.name != SpaceLabel.UNKNOWN)
                {
                    SpaceCategoryModel categoryModel = new SpaceCategoryModel(label, docs);
                    context.spaceModel.LabelToCategoryLinks.Add(label, categoryModel, 1);

                    context.spaceModel.categories.Add(categoryModel);

                    // notes.log("Class [" + categoryModel.name + "] BoW model created - terms[" + categoryModel.terms.Count + "] ");
                }

            }

        }

        /// <summary>
        /// Builds vectors from selected features and feature weighting model
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        public void VectorSpaceConstruction(OperationContext context, ILogBuilder log, Boolean constructCategories = false)
        {


            List<string> FV = context.SelectedFeatures.GetKeys(); //.entries.Select(x => x.name).ToList();
            //FV.AddRange();

            log.log("Preparing Weight model [" + weightModel.GetSignature() + "] - feature selection [" + FV.Count() + "]");
            // preparing the model
            weightModel.PrepareTheModel(context.spaceModel,log);


            Int32 i = 0;
            Int32 s = context.spaceModel.documents.Count / 10;

            // building document VSM
            foreach (SpaceDocumentModel docModel in context.spaceModel.documents)
            {
                var wd = weightModel.GetWeights(FV, docModel, context.spaceModel);
                VectorDocument docVec = new VectorDocument(docModel.name);
                docVec.terms = wd;

                context.vectorSpace.documents.Add(docVec);
                if (i % s == 0)
                {
                    Double r = i.GetRatio(context.spaceModel.documents.Count);
                    log.log("[" + r.ToString("F2") + "]");
                }
                i++;
            }

            if (constructCategories)
            {

                // logger.log(":: Creating VectorSpace instances for categories");
                // building category VSM
                foreach (SpaceCategoryModel catModel in context.spaceModel.categories)
                {

                    var wd = weightModel.GetWeights(FV, catModel, context.spaceModel);
                    VectorLabel catVec = new VectorLabel(catModel.name);
                    catVec.terms = wd;


                    context.vectorSpace.labels.Add(catVec);
                }
            }
        }


        public SpaceLabel designateSpaceLabel(OperationContext context, IVector vector)
        {
            SpaceLabel lab = context.spaceModel.label_unknown;

            if (context.spaceLabelByDocAssignedID.ContainsKey(vector.name))
            {
                lab = context.spaceLabelByDocAssignedID[vector.name];
            }
            else if (context.spaceLabelsDomains.ContainsKey(vector.name))
            {
                lab = context.spaceLabelsDomains[vector.name];
            }

            return lab;
        }


        /// <summary>
        /// Features the vector construction.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        public void FeatureVectorConstruction(OperationContext context, ILogBuilder log)
        {

            //log.log("Feature vector construction");

            // deploying feature vector space constructor
            featureSpaceConstructor.Deploy(constructorSettings, context.vectorSpace);
            featureSpaceConstructor.Deploy(constructorSettings, context.SelectedFeatures.GetKeys());


            Int32 i = 0;
            Int32 s = context.vectorSpace.documents.Count / 10;

            foreach (IVector vector in context.vectorSpace.documents)
            {

                var lab = designateSpaceLabel(context, vector);


                FeatureVector fv = featureSpaceConstructor.ConstructFeatureVector(vector);

                context.featureVectorByName.Add(vector.name, fv);

                context.featureSpace.documents.Add(fv);
                context.featureSpace.labelToDocumentAssociations.Add(fv, lab, 1);
                if (i % s == 0)
                {
                    Double r = i.GetRatio(context.spaceModel.documents.Count);
                    log.log("[" + r.ToString("F2") + "]");
                }
                i++;
            }

            log.log("Feature vector construction [" + context.featureSpace.documents.Count + "]");

            //if (context.reportOptions.HasFlag(PlanesReportOptions.report_featureVectors))
            //{
            //    var dt = context.featureSpace.MakeTable(featureSpaceConstructor, "FeatureSpace", "Feature space");
            //    notes.SaveDataTable(dt, notes.folder_feature);
            //}

        }

    }
}