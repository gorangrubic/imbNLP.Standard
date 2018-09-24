using imbNLP.Toolkit.Corpora;
using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.ExperimentModel;
using imbNLP.Toolkit.Planes.Core;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Reporting;
using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Stemmers;
using imbNLP.Toolkit.Typology;
using imbNLP.Toolkit.Vectors;
using imbNLP.Transliteration.ruleSet;
using imbSCI.Core.extensions.io;
using imbSCI.Core.reporting;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace imbNLP.Toolkit.Planes
{


    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Planes.Core.IPlaneMethodDesign" />
    public class CorpusPlaneMethodDesign : MethodDesignBase, IPlaneMethodDesign
    {



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
        /// Prepares everything for operation
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="_notes">The notes.</param>
        /// <param name="logger">The logger.</param>
        public void DeploySettings(IPlaneSettings settings, ToolkitExperimentNotes _notes, ILogBuilder logger)
        {
            DeploySettingsBase(_notes);

            CorpusPlaneMethodSettings corpusSettings = settings as CorpusPlaneMethodSettings;
            stemmer = TypeProviders.stemmerTypes.GetInstance(corpusSettings.stemmer);
            tokenizer = TypeProviders.tokenizerTypes.GetInstance(corpusSettings.tokenizer);
            if (!corpusSettings.transliterationRuleSetId.isNullOrEmpty())
            {
                transliteration = Transliteration.ruleSet.transliteration.GetTransliterationPairSet(corpusSettings.transliterationRuleSetId);
            }

            filter = corpusSettings.filter;
            // filter.limit = corpusSettings.filterLimit;
            filter.Deploy(logger);

            weightModel = corpusSettings.weightModel;
            weightModel.Deploy(logger);

            corpusSettings.Describe(notes);

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

            notes.logStartPhase("[2] Corpus Plane - execution", "");

            ICorpusPlaneContext context = (ICorpusPlaneContext)inputContext;
            VectorPlaneContext outputContext = new VectorPlaneContext();
            outputContext.provider.StoreAndReceive(context);

            context.stemmContext = new StemmingContext(stemmer);

            Dictionary<String, SpaceDocumentModel> documentVsModel = new Dictionary<string, SpaceDocumentModel>();

            // modelling the documents 
            foreach (TextDocument doc in context.corpus_documents)
            {
                var model = spaceConstructor.ConstructDocument(doc.content, doc.name, context.space, context, tokenizer);
                var labels = spaceConstructor.GetLabels(doc.labels, context.space);
                Boolean isUnknownLabel = true;
                foreach (SpaceLabel label in labels)
                {
                    if (label.name != SpaceLabel.UNKNOWN)
                    {
                        isUnknownLabel = false;
                    }
                    context.space.LabelToDocumentLinks.Add(label, model, 1);
                }
                context.space.documents.Add(model);
                if (!isUnknownLabel)
                {
                    context.space.terms.MergeDictionary(model.terms);
                }

                documentVsModel.Add(doc.name, model);
            }

            if (generalContext.reportOptions.HasFlag(PlanesReportOptions.report_fold_textrender))
            {
                foreach (TextDocument doc in context.corpus_documents)
                {
                    String prefix = doc.labels.FirstOrDefault();
                    if (prefix.isNullOrEmpty()) prefix = SpaceLabel.UNKNOWN;

                    String fn = prefix + "_" + doc.name;
                    String pth = notes.folder_entity.pathFor(fn.getFilename("txt"), imbSCI.Data.enums.getWritableFileMode.overwrite, "Textual representation of website [" + doc.name + "], produced by rendering and blending settings", true);
                    doc.content.saveStringToFile(pth, imbSCI.Data.enums.getWritableFileMode.overwrite);
                }
            }

            if (generalContext.reportOptions.HasFlag(PlanesReportOptions.report_fold_stats))
            {


                foreach (WebSiteDocumentsSet ds in context.dataset)
                {
                    DataTable dt = ds.MakeTable(documentVsModel);
                    notes.SaveDataTable(dt, notes.folder_entity);
                }



                var dt_vsm = context.space.LabelToDocumentLinks.MakeTable("LabelToDocument", "Relationships between labels and documents in the primary Vector Space Model");
                notes.SaveDataTable(dt_vsm, notes.folder_corpus);
            }

            if (generalContext.reportOptions.HasFlag(PlanesReportOptions.report_corpusDictionary))
            {
                notes.SaveDataTable(context.space.terms.MakeTable("corpus_stats", "Training set dictionary, after stemming", generalContext.DictionaryReportLimit), notes.folder_corpus);
            }



            #region SELECTING THE FEATURES 
            // forming corpus global weight
            context.SelectedFeatures = new WeightDictionary();
            List<KeyValuePair<string, double>> filter_result = filter.SelectFeatures(context.space);
            List<string> FV = new List<string>();
            FV.AddRange(filter_result.Select(x => x.Key));

            if (filter_result.Any())
            {
                foreach (var pair in filter_result)
                {
                    context.SelectedFeatures.AddEntry(pair.Key, pair.Value);
                }

                if (generalContext.reportOptions.HasFlag(PlanesReportOptions.report_selectedFeatures))
                {
                    notes.SaveDataTable(context.SelectedFeatures.MakeTable("selected_features", "Features selected for BoW construction", new List<string>() { filter.function.shortName }, generalContext.DictionaryReportLimit), notes.folder_corpus);
                }
            }
            else
            {
                logger.log("-- Feature selection function returned zero set. All features [" + context.space.terms.Count + "] are therefore accepted as selected.");
                var tkns = context.space.terms.GetTokens();
                foreach (var tkn in tkns)
                {
                    context.SelectedFeatures.AddEntry(tkn, 1);
                }
            }
            #endregion


            notes.log("Selected features [" + context.SelectedFeatures.entries.Count + "] by [" + filter.functionSettings.functionName + "]");




            //context.space = 
            //weightModel.Deploy();

            outputContext.vectorSpace = new Vectors.VectorSpace();


            foreach (SpaceLabel label in context.space.labels)
            {
                var docs = context.space.LabelToDocumentLinks.GetAllLinked(label);
                if (label.name != SpaceLabel.UNKNOWN)
                {
                    SpaceCategoryModel categoryModel = new SpaceCategoryModel(label, docs);
                    context.space.LabelToCategoryLinks.Add(label, categoryModel, 1);

                    context.space.categories.Add(categoryModel);

                    notes.log("Class [" + categoryModel.name + "] BoW model created - terms[" + categoryModel.terms.Count + "] ");
                }

            }

            outputContext.LabelToDocumentLinks = context.space.LabelToDocumentLinks;

            // preparing the model
            weightModel.PrepareTheModel(context.space);

            // logger.log(":: Creating VectorSpace instances for documents");
            // building document VSM
            foreach (SpaceDocumentModel docModel in context.space.documents)
            {
                var wd = weightModel.GetWeights(FV, docModel, context.space);
                VectorDocument docVec = new VectorDocument(docModel.name);
                docVec.terms = wd;

                if (generalContext.reportOptions.HasFlag(PlanesReportOptions.report_documentBoWModels))
                {
                    DataTable dt = wd.MakeTable("docVec_" + docModel.name, "Document vector model", null, 10000);
                    notes.SaveDataTable(dt, notes.folder_vector);
                }
                outputContext.vectorSpace.documents.Add(docVec);
            }

            // logger.log(":: Creating VectorSpace instances for categories");
            // building category VSM
            foreach (SpaceCategoryModel catModel in context.space.categories)
            {

                var wd = weightModel.GetWeights(FV, catModel, context.space);
                VectorLabel catVec = new VectorLabel(catModel.name);
                catVec.terms = wd;

                if (generalContext.reportOptions.HasFlag(PlanesReportOptions.report_documentBoWModels))
                {
                    DataTable dt = wd.MakeTable("catVec_" + catModel.name, "Document vector model", null, 10000);
                    notes.SaveDataTable(dt, notes.folder_vector);
                }

                outputContext.vectorSpace.labels.Add(catVec);
            }

            if (generalContext.reportOptions.HasFlag(PlanesReportOptions.report_documentBoWModels))
            {

                foreach (SpaceCategoryModel catModel in context.space.categories)
                {
                    var dt = catModel.terms.MakeTable("cat_" + catModel.name, "Vector Space BoW weighted model, representing a category");
                    notes.SaveDataTable(dt, notes.folder_vector);
                }
            }


            notes.logEndPhase();

            return outputContext;
        }
    }

}