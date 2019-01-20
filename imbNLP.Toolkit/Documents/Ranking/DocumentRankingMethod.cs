using imbNLP.Toolkit.Documents.Ranking.Core;
using imbNLP.Toolkit.Documents.Ranking.Data;
using imbNLP.Toolkit.Weighting;
using imbNLP.Toolkit.Weighting.Global;
using imbNLP.Toolkit.Weighting.Local;
using imbSCI.Core.files;
using imbSCI.Core.math;
using imbSCI.Core.math.range.finder;
using imbSCI.Core.reporting;
using imbSCI.Core.reporting.render;
using System;

namespace imbNLP.Toolkit.Documents.Ranking
{
    /// <summary>
    /// Method that ranks documents
    /// </summary>
    [Serializable]
    public class DocumentRankingMethod : IHasProceduralRequirements
    {

        public DocumentRankingMethod()
        {

        }

        /// <summary>
        /// Describes the specified logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public void Describe(ITextRender logger)
        {
            model.Describe(logger);
            query.Describe(logger);
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public ScoreModel model { get; set; } = new ScoreModel();

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>
        /// The query.
        /// </value>
        public DocumentSelectQuery query { get; set; } = new DocumentSelectQuery();


        //        public Boolean DoNormalizeOnDomainLevel { get; set; } = true;


        /// <summary>
        /// Gets the preset tfidf.
        /// </summary>
        /// <param name="documentLimit">The document limit.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public static DocumentRankingMethod GetPreset_TFIDF(Int32 documentLimit, ILogBuilder log)
        {
            DocumentRankingMethod output = new DocumentRankingMethod();

            ScoreModel scoreModel = new ScoreModel();

            ScoreModelTermWeightFactor twf = new ScoreModelTermWeightFactor();
            twf.TermWeightModel = new Weighting.FeatureWeightModel();
            twf.TermWeightModel.LocalFunction = new TermFrequencyFunction();

            FeatureWeightFactor featureWeightFactor = new FeatureWeightFactor();
            featureWeightFactor.Settings = new GlobalFunctionSettings();
            featureWeightFactor.Settings.functionName = nameof(IDFElement);
            featureWeightFactor.Settings.weight = 1.0;
            featureWeightFactor.Deploy(log);

            twf.TermWeightModel.GlobalFactors.Add(featureWeightFactor);
            twf.weight = 1.0;

            scoreModel.Factors.Add(twf);


            output.query = new DocumentSelectQuery();
            output.query.SizeLimit = documentLimit;

            output.model = scoreModel;
            return output;


        }

        /// <summary>
        /// Returns MD5 hash of XML serialized settings of this instance
        /// </summary>
        /// <returns></returns>
        public String GetSignature()
        {
            String xml = objectSerialization.ObjectToXML(this);
            return md5.GetMd5Hash(xml, false);
        }


        /// <summary>
        /// Queries factors for preprocessing requirements
        /// </summary>
        /// <param name="requirements">The requirements.</param>
        /// <returns></returns>
        public ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null)
        {
            if (requirements == null) requirements = new ScoreModelRequirements();

            model.Deploy();
            model.CheckRequirements(requirements);


            return requirements;
        }

        /*
        /// <summary>
        /// Prepares the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public DocumentSelectResult PrepareContext(OperationContext context, ILogBuilder log)
        {
            DocumentSelectResult selectContext = new DocumentSelectResult();
            selectContext.stemmingContext = context.stemmContext;
            selectContext.spaceModel = context.spaceModel;
            selectContext.query = query;

            selectContext.selectedFeatures = context.SelectedFeatures;

            foreach (KeyValuePair<string, WebSiteDocuments> pair in context.webSiteByDomain)
            {
                selectContext.domainNameToGraph.Add(pair.Key, pair.Value.extensions.graph);

                foreach (WebSiteDocument doc in pair.Value.documents)
                {
                    DocumentSelectResultEntry entry = new DocumentSelectResultEntry();
                    TextDocument text = context.textDocuments[doc.AssociatedID];
                    SpaceDocumentModel spaceDocument = context.spaceModel.documents.FirstOrDefault(x => x.name == doc.AssociatedID);

                    string dn = pair.Value.domain;
                    entry.SetEntry(dn, doc, spaceDocument, text);
                    selectContext.Add(entry);
                    //entry.SetEntry( context.context.webDocumentByAssignedID[pair.Key], webDocIDToDomain[aID], webDocumentRegistry[aID], spaceDocumentRegistry[aID], textDocumentRegistry[aID]);
                }

            }

            // PREPARATION OF MODEL

            model.Prepare(selectContext, log);

            return selectContext;

        }
        */
        /*
        /// <summary>
        /// Prepares the context.
        /// </summary>
        /// <param name="space">The space.</param>
        /// <param name="sites">The sites.</param>
        /// <param name="documents">The documents.</param>
        /// <param name="stemmingContext">The stemming context.</param>
        /// <returns></returns>
        public DocumentSelectResult PrepareContext(SpaceModel space, IEnumerable<WebSiteDocuments> sites, IEnumerable<TextDocument> documents, StemmingContext stemmingContext)
        {
            DocumentSelectResult context = new DocumentSelectResult();
            context.query = query;

            context.stemmingContext = stemmingContext;
            context.spaceModel = space;

            List<String> associatedIDs = new List<string>();

            Dictionary<String, TextDocument> textDocumentRegistry = new Dictionary<string, TextDocument>();
            foreach (TextDocument textDocument in documents)
            {
                textDocumentRegistry.Add(textDocument.name, textDocument);
            }

            Dictionary<String, SpaceDocumentModel> spaceDocumentRegistry = new Dictionary<string, SpaceDocumentModel>();
            foreach (var textDocument in space.documents)
            {
                spaceDocumentRegistry.Add(textDocument.name, textDocument);
            }


            Dictionary<String, String> webDocIDToDomain = new Dictionary<string, string>();

            Dictionary<String, WebSiteDocument> webDocumentRegistry = new Dictionary<string, WebSiteDocument>();

            foreach (WebSiteDocuments site in sites)
            {
                context.domainNameToGraph.Add(site.domain, site.extensions.graph);

                foreach (WebSiteDocument webDocument in site.documents)
                {
                    webDocumentRegistry.Add(webDocument.AssociatedID, webDocument);
                    associatedIDs.Add(webDocument.AssociatedID);
                    webDocIDToDomain.Add(webDocument.AssociatedID, site.domain);
                }
            }

            foreach (String aID in associatedIDs)
            {
                DocumentSelectResultEntry entry = new DocumentSelectResultEntry();
                entry.SetEntry(webDocIDToDomain[aID], webDocumentRegistry[aID], spaceDocumentRegistry[aID], textDocumentRegistry[aID]);
                context.Add(entry);
            }

            return context;
        }
        */


        public DocumentSelectResult ExecuteEvaluation(DocumentSelectResult context, ILogBuilder log)
        {


            // SCORE COMPUTATION
            foreach (IScoreModelFactor factor in model.Factors)
            {
                rangeFinder ranger = new rangeFinder();

                foreach (DocumentSelectResultEntry entry in context.items)
                {
                    Double score = factor.Score(entry, context, log);
                    entry.SetScore(factor, score);
                    if (score != Double.NaN)
                    {
                        if (factor.doNormalize)
                        {
                            ranger.Learn(score);
                        }
                    }
                }

                foreach (DocumentSelectResultEntry entry in context.items)
                {
                    Double score = entry.GetScore(factor);

                    if (ranger.Range != Double.NaN)
                    {
                        if (factor.doNormalize)
                        {

                            score = score - ranger.Minimum;

                            score = score / ranger.Range;
                        }
                    }
                    score = score * factor.weight;

                    entry.SetScore(factor, score, false);
                }
            }

            foreach (DocumentSelectResultEntry entry in context.items)
            {
                entry.SumFactorScores();
            }






            return context;
        }





    }
}
