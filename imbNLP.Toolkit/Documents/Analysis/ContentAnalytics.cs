using HtmlAgilityPack;
using imbNLP.Toolkit.Documents.HtmlAnalysis;
using imbNLP.Toolkit.ExperimentModel;
using imbNLP.Toolkit.Planes;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbSCI.Core.extensions.data;
using imbSCI.Core.files.folders;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Documents.Analysis
{

    public class ContentAnalytics : AnalyticsBase
    {
        public ContentAnalytics()
        {

        }

        public ContentAnalytics(folderNode _folder)
        {
            folder = _folder;

        }

        public EntityPlaneMethodDesign entityMethod { get; set; } = new EntityPlaneMethodDesign();

        public CorpusPlaneMethodDesign corpusMethod { get; set; } = new CorpusPlaneMethodDesign();

        public VectorPlaneMethodDesign vectorMethod { get; set; } = new VectorPlaneMethodDesign();

        public EntityPlaneMethodSettings entityMethodSettings { get; set; } = new EntityPlaneMethodSettings();

        public CorpusPlaneMethodSettings corpusMethodSettings { get; set; } = new CorpusPlaneMethodSettings();

        public VectorPlaneMethodSettings vectorMethodSettings { get; set; } = new VectorPlaneMethodSettings();

        public DataSetComparison CompareDatasets(String runName, IEnumerable<WebSiteDocumentsSet> datasetA, IEnumerable<WebSiteDocumentsSet> datasetB, ILogBuilder logger)
        {


            DataSetComparison output = new DataSetComparison();

            output.analyticA = ProduceMetrics(runName + "_A", datasetA, logger);
            output.analyticB = ProduceMetrics(runName + "_B", datasetB, logger);

            output.tknA = output.analyticA.terms.GetTokens().OrderBy(x => x).ToList();
            output.tknB = output.analyticB.terms.GetTokens().OrderBy(x => x).ToList();

            output.tknC = output.tknA.GetCrossSection<String>(new List<String>[] { output.tknB }).OrderBy(x => x).ToList();

            output.tknA_u = output.tknA.GetDifference(output.tknC).OrderBy(x => x).ToList();
            output.tknB_u = output.tknB.GetDifference(output.tknC).OrderBy(x => x).ToList();

            foreach (String t in output.tknC) output.TermsInCommon.AddToken(t, output.analyticA.terms.GetTokenFrequency(t) + output.analyticB.terms.GetTokenFrequency(t));

            foreach (String t in output.tknA_u) output.TermsUniqueForA.AddToken(t, output.analyticA.terms.GetTokenFrequency(t));
            foreach (String t in output.tknB_u) output.TermsUniqueForB.AddToken(t, output.analyticB.terms.GetTokenFrequency(t));

            output.DocumentSetsUniqueForA.AddRange(output.analyticA.domains.GetDifference(output.analyticB.domains));
            output.DocumentSetsInCommonByName.AddRange(output.analyticA.domains.GetCrossSection(new List<String>[] { output.analyticB.domains }));
            output.DocumentSetsUniqueForB.AddRange(output.analyticB.domains.GetDifference(output.analyticA.domains));

            List<String> domains = new List<string>();
            foreach (var pair in output.analyticA.categoryNameVsDocumentModel)
            {
                domains.AddRange(pair.Value.Select(x => x.name));
            }

            return output;

        }



        /// <summary>
        /// Produces metrics for the data set specified
        /// </summary>
        /// <param name="runName">Name of the run.</param>
        /// <param name="dataset">The dataset.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public ContentAnalyticsContext ProduceMetrics(String runName, IEnumerable<WebSiteDocumentsSet> dataset, ILogBuilder logger)
        {
            ContentAnalyticsContext output = new ContentAnalyticsContext();
            ExperimentModelExecutionContext mainContext = new ExperimentModelExecutionContext(runName);
            mainContext.PrepareNotes(folder, "", logger);

            output.GlobalCategoryTree = HtmlTagCategoryTree.GetIMBStandardCategoryTree(); //new HtmlTagCategoryTree(runName, "HTML metrics for the complete dataset");

            entityMethod.DeploySettings(entityMethodSettings, mainContext.notes, logger);
            corpusMethod.DeploySettings(corpusMethodSettings, mainContext.notes, logger);
            vectorMethod.DeploySettings(vectorMethodSettings, mainContext.notes, logger);

            Dictionary<String, ContentMetrics> categoryMetrics = new Dictionary<string, ContentMetrics>();
            Dictionary<String, ContentMetrics> siteMetrics = new Dictionary<string, ContentMetrics>();


            foreach (WebSiteDocumentsSet classSet in dataset)
            {
                output.categoryNameVsHtmlTag.Add(classSet.name, HtmlTagCategoryTree.GetIMBStandardCategoryTree());

                categoryMetrics.Add(classSet.name, new ContentMetrics());
                categoryMetrics[classSet.name].DocumentSets = classSet.Count;
                foreach (WebSiteDocuments website in classSet)
                {
                    output.domains.Add(website.domain);
                    siteMetrics.Add(website.domain, new ContentMetrics());
                    siteMetrics[website.domain].Documents = website.documents.Count;
                    // categoryMetrics[classSet.name].Documents += website.documents.Count;
                    foreach (WebSiteDocument webdocument in website.documents)
                    {
                        siteMetrics[website.domain].SourceLength += webdocument.HTMLSource.Length;

                        HtmlDocument htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(webdocument.HTMLSource);
                        output.categoryNameVsHtmlTag[classSet.name].CountTags(htmlDoc.DocumentNode.ChildNodes);
                        output.GlobalCategoryTree.CountTags(htmlDoc.DocumentNode.ChildNodes);
                        // processing of single document
                    }
                }
            }

            Dictionary<String, List<SpaceDocumentModel>> categoryNameVsDocumentModel = output.categoryNameVsDocumentModel;
            Dictionary<String, List<TextDocument>> categoryNameVsDocumentText = output.categoryNameVsDocumentText; // new Dictionary<String, List<TextDocument>>();

            Dictionary<String, TokenDictionary> categoryNameVsTerms = output.categoryNameVsTerms;



            EntityPlaneContext context = new EntityPlaneContext();
            context.dataset = dataset.ToList();

            CorpusPlaneContext corpusContext = new CorpusPlaneContext();
            VectorPlaneContext vectorContext = new VectorPlaneContext();

            corpusContext = entityMethod.ExecutePlaneMethod(context, mainContext, logger) as CorpusPlaneContext;
            corpusContext.stemmContext = new StemmingContext(corpusMethod.stemmer);

            Dictionary<String, SpaceDocumentModel> documentVsModel = new Dictionary<string, SpaceDocumentModel>();

            // modelling the documents 
            foreach (TextDocument doc in corpusContext.corpus_documents)
            {
                ContentMetrics docMetrics = siteMetrics[doc.name];
                docMetrics.RenderLength = doc.content.Length;

                SpaceDocumentModel model = corpusMethod.spaceConstructor.ConstructDocument(doc.content, doc.name, corpusContext.space, corpusContext, corpusMethod.tokenizer, docMetrics);
                var labels = corpusMethod.spaceConstructor.GetLabels(doc.labels, corpusContext.space);

                foreach (SpaceLabel label in labels)
                {
                    if (!categoryNameVsDocumentModel.ContainsKey(label.name))
                    {
                        categoryNameVsDocumentModel.Add(label.name, new List<SpaceDocumentModel>());
                        categoryNameVsDocumentText.Add(label.name, new List<TextDocument>());
                        categoryNameVsTerms.Add(label.name, new TokenDictionary());
                    }
                    categoryNameVsDocumentModel[label.name].Add(model);
                    categoryNameVsDocumentText[label.name].Add(doc);
                    categoryNameVsTerms[label.name].MergeDictionary(model.terms);
                    categoryMetrics[label.name].Plus(docMetrics);

                    //corpusContext.space.LabelToDocumentLinks.Add(label, model, 1);
                }

                corpusContext.space.documents.Add(model);
                corpusContext.space.terms.MergeDictionary(model.terms);

                documentVsModel.Add(doc.name, model);
            }

            ContentMetrics interclassAvg = new ContentMetrics("Average");
            ContentMetrics interclassSum = new ContentMetrics("Sum");
            foreach (var pair in categoryNameVsTerms)
            {
                categoryMetrics[pair.Key].Terms = pair.Value.Count;
                categoryMetrics[pair.Key].Class = 1;
                interclassAvg.Plus(categoryMetrics[pair.Key]);
                interclassSum.Plus(categoryMetrics[pair.Key]);
                output.ClassMetrics.Add(categoryMetrics[pair.Key]);
                categoryMetrics[pair.Key].Name = pair.Key;
                output.terms.MergeDictionary(pair.Value);
            }

            interclassAvg.Divide(Convert.ToDouble(categoryNameVsTerms.Count));

            output.avgMetrics = interclassAvg;
            output.sumMetrics = interclassSum;

            return output;
        }

    }

}