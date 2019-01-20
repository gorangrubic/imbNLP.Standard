using HtmlAgilityPack;
using imbNLP.Toolkit.Documents.HtmlAnalysis;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbSCI.Core.extensions.data;
using imbSCI.Core.files.folders;
using imbSCI.Core.reporting;
using imbSCI.Data;
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

        //    public EntityPlaneMethodDesign entityMethod { get; set; } = new EntityPlaneMethodDesign();

        //   public CorpusPlaneMethodDesign corpusMethod { get; set; } = new CorpusPlaneMethodDesign();

        //  public VectorPlaneMethodDesign vectorMethod { get; set; } = new VectorPlaneMethodDesign();

        //public EntityPlaneMethodSettings entityMethodSettings { get; set; } = new EntityPlaneMethodSettings();

        //public CorpusPlaneMethodSettings corpusMethodSettings { get; set; } = new CorpusPlaneMethodSettings();

        //public VectorPlaneMethodSettings vectorMethodSettings { get; set; } = new VectorPlaneMethodSettings();

        public DataSetComparison CompareDatasets(String runName, IEnumerable<WebSiteDocumentsSet> datasetA, IEnumerable<WebSiteDocumentsSet> datasetB, OperationContext context, ILogBuilder logger)
        {


            DataSetComparison output = new DataSetComparison();

            output.analyticA = ProduceMetrics(runName + "_A", datasetA, context, logger);
            output.analyticB = ProduceMetrics(runName + "_B", datasetB, context, logger);

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

        public HtmlTagCategoryTree imbTagCategoryTree { get; set; } = HtmlTagCategoryTree.GetIMBStandardCategoryTree();


        

        /// <summary>
        /// Produces metrics for the data set specified
        /// </summary>
        /// <param name="runName">Name of the run.</param>
        /// <param name="dataset">The dataset.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public ContentAnalyticsContext ProduceMetrics(String runName, IEnumerable<WebSiteDocumentsSet> dataset, OperationContext context, ILogBuilder logger)
        {
            ContentAnalyticsContext output = new ContentAnalyticsContext();

            SpaceModel space = context.spaceModel;


            output.DictinctStems = context.stemmContext.GetDictinctStems();
            output.DictinctTokens = context.stemmContext.GetDistinctTokens();
            output.StemToTokens = context.stemmContext.GetStemToWords();

            //ExperimentModelExecutionContext mainContext = new ExperimentModelExecutionContext(runName);
            //mainContext.PrepareDataset(dataset, null, 0, -1); //.PrepareNotes(folder, "", logger);

            output.GlobalCategoryTree = imbTagCategoryTree;//HtmlTagCategoryTree.GetIMBStandardCategoryTree(); //new HtmlTagCategoryTree(runName, "HTML metrics for the complete dataset");

            //entityMethod.DeploySettings(entityMethodSettings, mainContext.notes, logger);
            //corpusMethod.DeploySettings(corpusMethodSettings, mainContext.notes, logger);
            //vectorMethod.DeploySettings(vectorMethodSettings, mainContext.notes, logger);

            Dictionary<String, ContentMetrics> categoryMetrics = new Dictionary<string, ContentMetrics>();
            Dictionary<String, ContentMetrics> siteMetrics = new Dictionary<string, ContentMetrics>();


            foreach (WebSiteDocumentsSet classSet in dataset)
            {
                output.categoryNameVsHtmlTag.Add(classSet.name, imbTagCategoryTree);

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
                        output.pages.Add(webdocument.AssignedID);

                        siteMetrics[website.domain].SourceLength += webdocument.HTMLSource.Length;

                        var docMetric = new ContentMetrics(webdocument.AssignedID);

                        if (context.entityMetrics.ContainsKey(webdocument.AssignedID))
                        {
                            docMetric.Plus(context.entityMetrics[webdocument.AssignedID]);
                        }


                        siteMetrics.Add(webdocument.AssignedID, docMetric);


                        HtmlDocument htmlDoc = HtmlDocumentCache.DefaultDocumentCache.GetDocument(webdocument.AssignedID, webdocument.HTMLSource);  //new HtmlDocument();
                        ///htmlDoc.LoadHtml(webdocument.HTMLSource);

                        output.categoryNameVsHtmlTag[classSet.name].CountTags(htmlDoc.DocumentNode.ChildNodes);
                        output.GlobalCategoryTree.CountTags(htmlDoc.DocumentNode.ChildNodes);

                        docMetric.Documents = 1;
                        docMetric.SourceLength = webdocument.HTMLSource.Length;
                        // processing of single document
                    }
                }
            }

            Dictionary<String, List<SpaceDocumentModel>> categoryNameVsDocumentModel = output.categoryNameVsDocumentModel;
            
            Dictionary<String, TokenDictionary> categoryNameVsTerms = output.categoryNameVsTerms;


            Dictionary<String, SpaceDocumentModel> documentVsModel = new Dictionary<string, SpaceDocumentModel>();

            // modelling the documents 
            foreach (SpaceDocumentModel doc in space.documents)
            {
                ContentMetrics docMetrics = siteMetrics[doc.name];



                docMetrics.RenderLength = doc.Length; // doc.content.Length;
                                                      // docMetrics.UniqueTokensDoc = doc.terms.GetSumFrequency();

                

                // SpaceDocumentModel model = corpusMethod.spaceConstructor.ConstructDocument(doc.content, doc.name, corpusContext.space, corpusContext.stemmContext, corpusMethod.tokenizer, docMetrics);
                var labels = doc.labels; //corpusMethod.spaceConstructor.GetLabels(doc.labels, corpusContext.space);

                foreach (String label in labels)
                {
                    if (!categoryNameVsDocumentModel.ContainsKey(label))
                    {
                        categoryNameVsDocumentModel.Add(label, new List<SpaceDocumentModel>());
                        //  categoryNameVsDocumentText.Add(label, new List<TextDocument>());
                        categoryNameVsTerms.Add(label, new TokenDictionary());
                    }
                    categoryNameVsDocumentModel[label].Add(doc);



                    categoryNameVsTerms[label].MergeDictionary(doc.GetTerms(true, true, true));

                    categoryMetrics[label].Plus(docMetrics);


                }

                documentVsModel.Add(doc.name, doc);
            }

            ContentMetrics interclassAvg = new ContentMetrics("Average");
            ContentMetrics interclassSum = new ContentMetrics("Sum");
            foreach (KeyValuePair<string, TokenDictionary> pair in categoryNameVsTerms)
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