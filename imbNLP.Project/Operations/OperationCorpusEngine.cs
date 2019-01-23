using imbACE.Core.core;
using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Documents.Analysis;
using imbNLP.Toolkit.Documents.Ranking.Core;
using imbNLP.Toolkit.Entity;
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

namespace imbNLP.Project.Operations
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

        public Boolean DoKeepContentMetrics { get; set; } = true;

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
        /// Gets or sets the blender.
        /// </summary>
        /// <value>
        /// The blender.
        /// </value>
        public DocumentBlenderFunction blender { get; set; } = new DocumentBlenderFunction();

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
            filter.Describe(logger);

            blender = corpusSettings.blender.CloneViaXML(logger);
            blender.Describe(logger);

            weightModel = corpusSettings.WeightModel.CloneViaXML(logger);
            weightModel.Deploy(logger);
            weightModel.Describe(logger);

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
            context.tokenizer = tokenizer;

            context.entityMetrics = new Dictionary<String, ContentMetrics>();

            foreach (KeyValuePair<String, TextDocumentSet> pair in context.renderSiteByDomain)
            {
                SpaceLabel spaceLabel = context.spaceLabelsDomains[pair.Key];

                SpaceDocumentModel modelOfSite = new SpaceDocumentModel();
                modelOfSite.name = pair.Key;
                modelOfSite.labels.Add(spaceLabel.name);

                foreach (TextDocumentLayerCollection textLayer in pair.Value)
                {
                    SpaceDocumentModel modelOfPage = new SpaceDocumentModel(textLayer.name);

                    ContentMetrics metrics = null;
                    if (DoKeepContentMetrics)
                    {
                        metrics = new ContentMetrics(textLayer.name);
                    }

                    foreach (var renderLayer in textLayer)
                    {
                        SpaceDocumentModel modelOfLayer = new SpaceDocumentModel(modelOfPage.name + renderLayer.name);

                        modelOfLayer = spaceConstructor.ConstructDocument(renderLayer.content, modelOfPage.name + renderLayer.name,
                            context.spaceModel, context.stemmContext, tokenizer,
                            spaceLabel.name != SpaceLabel.UNKNOWN, metrics);

                        modelOfLayer.weight = renderLayer.layerWeight;

                        modelOfLayer.documentScope = DocumentBlenderFunctionOptions.layerLevel;

                        modelOfPage.Children.Add(modelOfLayer);
                    }

                    modelOfPage.documentScope = DocumentBlenderFunctionOptions.pageLevel;

                    if (DoKeepContentMetrics)
                    {
                        context.entityMetrics.Add(metrics.Name, metrics);
                    }

                    // modelOfPage.Flatten(false);

                    modelOfSite.Children.Add(modelOfPage);
                }

                modelOfSite.documentScope = DocumentBlenderFunctionOptions.siteLevel;

                context.spaceModel.documents.Add(modelOfSite);

                foreach (String label in modelOfSite.labels)
                {
                    SpaceLabel sLabel = null;
                    sLabel = context.spaceLabels[label];
                    context.spaceModel.LabelToDocumentLinks.Add(sLabel, modelOfSite, 1);
                }

                modelOfSite.Flatten(false);
                /*
                if (modelOfSite.labels.Contains(SpaceLabel.UNKNOWN))
                {
                    context.spaceModel.terms_unknown_label.MergeDictionary(modelOfSite.terms);
                }
                else
                {
                    context.spaceModel.terms_known_label.MergeDictionary(modelOfSite.terms);
                }*/

                modelOfSite.PropagateLabels();

                //    modelOfSite.SetLabel(spaceLabel, context.spaceModel);

                //context.spaceModel.LabelToDocumentLinks.Add(spaceLabel, modelOfSite, 1.0);
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
            log.log("Feature selection [" + EnableSelection.ToString() + "]");

            if (filter == null)
            {
                EnableSelection = false;
            }
            else if (filter.WeightModel == null)
            {
                EnableSelection = false;
            }
            else if (!filter.WeightModel.GlobalFactors.Any())
            {
                EnableSelection = false;
            }

            if (!context.spaceModel.IsModelReady)
            {
                log.log("-- Feature selection function shouldn't be called before creation of the space model.");
            }

            List<KeyValuePair<string, double>> filter_result = new List<KeyValuePair<string, double>>();

            if (EnableSelection)
            {
                filter_result = filter.SelectFeatures(context.spaceModel, log, notes.folder);
            }
            else
            {
            }

            builderForLog textBuilder = new builderForLog();
            filter.Describe(textBuilder);

            context.SelectedFeatures = new WeightDictionary("FS_" + context.name, "Features selected by " + filter.GetSignature() + ". Info: " + textBuilder.GetContent());

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
                String msg = "-- Feature selection function returned zero set. All features [" + context.spaceModel.terms_known_label.Count + "] are therefore accepted as selected.";

                context.SelectedFeatures.description += msg;
                log.log(msg);
                var tkns = context.spaceModel.GetTokens(true, false);
                foreach (var tkn in tkns)
                {
                    context.SelectedFeatures.AddEntry(tkn, 1);
                }
            }

            if (context.SelectedFeatures.Count < context.spaceModel.terms_known_label.Count)
            {
                context.spaceModel.FilterSpaceModelFeatures(context.SelectedFeatures, log);
            }
            else
            {
                context.spaceModel.terms_unknown_label.FilterTokens(context.SelectedFeatures.GetKeys());
            }

            if (filter.WeightModel != null)
            {
                filter.WeightModel.Dispose();
            }


        }

        /// <summary>
        /// Spaces the model categories.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        public void SpaceModelCategories(OperationContext context, ILogBuilder log)
        {
            log.log("Space model categories");
            foreach (SpaceLabel label in context.spaceModel.labels)
            {
                if (label.name != SpaceLabel.UNKNOWN)
                {
                    var docs = context.spaceModel.LabelToDocumentLinks.GetAllLinked(label);

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

            log.log("Preparing Weight model [" + weightModel.GetSignature() + "] - feature selection [" + FV.Count() + "] ");
            // preparing the model
            weightModel.PrepareTheModel(context.spaceModel, log);

            // blanking anything existing in vector space
            context.vectorSpace = new VectorSpace();

            List<SpaceDocumentModel> toBlendIntoVectors = DocumentBlenderFunctionExtension.GetDocumentToBlend(blender.options, context.spaceModel.documents, log);

            Int32 i = 0;
            Int32 s = toBlendIntoVectors.Count() / 5;


            Dictionary<String, List<VectorDocument>> labelToDocumentSets = new Dictionary<String, List<VectorDocument>>();


            foreach (SpaceCategoryModel catModel in context.spaceModel.categories)
            {
                labelToDocumentSets.Add(catModel.name, new List<VectorDocument>());
            }

            Int32 unlabeled = 0;

            foreach (SpaceDocumentModel model in toBlendIntoVectors)
            {
                VectorDocument docVec = model.BlendToVector<VectorDocument>(weightModel, context.spaceModel, FV);  //new VectorDocument(model.name);
                context.vectorSpace.documents.Add(docVec);

                if (constructCategories)
                {
                    String l = model.labels.FirstOrDefault();

                    if (!l.isNullOrEmpty())
                    {
                        if (labelToDocumentSets.ContainsKey(l))
                        {
                            labelToDocumentSets[l].Add(docVec);
                        }
                        else
                        {
                            unlabeled++;
                            // 
                        }
                    }
                }


                if (i % s == 0)
                {
                    Double r = i.GetRatio(context.spaceModel.documents.Count);
                    log.log("Blending primary vectors [" + r.ToString("P2") + "] : [" + i + "/" + toBlendIntoVectors.Count + "]");
                }
                i++;
            }

            if (constructCategories && (unlabeled > 0))
            {
                log.log("Vectors [" + unlabeled + "] are unlabeled");
            }

            if (constructCategories)
            {
                log.log(":: Creating VectorSpace instances for categories");
                // building category VSM
                foreach (SpaceCategoryModel catModel in context.spaceModel.categories)
                {
                    VectorLabel catVec = new VectorLabel(catModel.name);
                    foreach (var docVec in labelToDocumentSets[catModel.name])
                    {
                        catVec.terms.Merge(docVec.terms);
                    }

                    //= catModel.BlendToVector<VectorLabel>(weightModel, context.spaceModel, FV); //weightModel.GetWeights(FV, catModel, context.spaceModel);

                    context.vectorSpace.labels.Add(catVec);
                }
            }


            if (weightModel != null)
            {
                weightModel.Dispose();
            }

        }

        public SpaceLabel designateSpaceLabel(OperationContext context, IVector vector)
        {
            //SpaceLabel lab = context.spaceModel.label_unknown;

            SpaceLabel lab = context.spaceModel.LabelToDocumentLinks.GetAllLinkedA(vector.name).FirstOrDefault();

            if (context.spaceLabelByDocAssignedID.ContainsKey(vector.name))
            {
                lab = context.spaceLabelByDocAssignedID[vector.name];
            }
            else if (context.spaceLabelsDomains.ContainsKey(vector.name))
            {
                lab = context.spaceLabelsDomains[vector.name];
            }

            if (lab == null)
            {
                lab = context.spaceModel.label_unknown;
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
            // deploying feature vector space constructor
            featureSpaceConstructor.Deploy(constructorSettings, context.vectorSpace);
            featureSpaceConstructor.Deploy(constructorSettings, context.SelectedFeatures.GetKeys());

            Int32 i = 0;
            Int32 s = 100;

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
                    log.log("Building feature vectors [" + r.ToString("P2") + "] : [" + i + "/" + context.vectorSpace.documents.Count + "]");
                }
                i++;
            }

            log.log("Feature vector construction [" + context.featureSpace.documents.Count + "] done");

            //if (context.reportOptions.HasFlag(PlanesReportOptions.report_featureVectors))
            //{
            //    var dt = context.featureSpace.MakeTable(featureSpaceConstructor, "FeatureSpace", "Feature space");
            //    notes.SaveDataTable(dt, notes.folder_feature);
            //}
        }
    }
}