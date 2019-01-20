using HtmlAgilityPack;
using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Documents.Analysis;
using imbNLP.Toolkit.ExperimentModel;
using imbNLP.Toolkit.Feature;

using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Vectors;
using imbSCI.Core.extensions.data;
using imbSCI.Core.reporting;
using imbSCI.Data.collection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Documents
{
    public class OperationContext : IOperationExecutionContext
    {

        public Double rateOfEmptyRenders { get; set; } = 0;

        /// <summary>
        /// If true the rendering phase was already done
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is text rendered; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsTextRendered
        {
            get
            {
                return renderSiteByDomain.Any();
            }
        }

        /// <summary>
        /// If true: dataset is already deployed
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is dataset deployed; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsDatasetDeployed
        {
            get
            {
                if (webSiteByDomain.Any()) return true;
                if (spaceLabelsDomains.Any()) return true;

                return webDocumentByAssignedID.Any();
            }
        }


        public WebSiteDocumentsSet UnlabeledSites { get; set; }


        public Dictionary<String, WebSiteDocumentsSet> dataset { get; set; } = new Dictionary<string, WebSiteDocumentsSet>();
        public String dataSetSignature { get; set; } = "";
        public Dictionary<String, WebSiteDocuments> webSiteByDomain = new Dictionary<string, WebSiteDocuments>();
        public Dictionary<String, WebSiteDocument> webDocumentByAssignedID = new Dictionary<string, WebSiteDocument>();

        public Dictionary<String, HtmlDocument> htmlDocumentByAssignedID = new Dictionary<string, HtmlDocument>();

        public Dictionary<String, SpaceLabel> spaceLabelByDocAssignedID = new Dictionary<string, SpaceLabel>();

        public SpaceModel spaceModel { get; set; } = new SpaceModel();
        public Dictionary<String, SpaceLabel> spaceLabels = new Dictionary<string, SpaceLabel>();
        public Dictionary<String, SpaceLabel> spaceLabelsDomains = new Dictionary<string, SpaceLabel>();

        public Dictionary<String, TextDocumentSet> renderSiteByDomain = new Dictionary<string, TextDocumentSet>();
        public Dictionary<String, TextDocumentLayerCollection> renderLayersByAssignedID = new Dictionary<string, TextDocumentLayerCollection>();


        /// <summary>
        /// Rendered documents by AssignedID
        /// </summary>
    //    public Dictionary<String, TextDocument> textDocuments = new Dictionary<string, TextDocument>();


        public StemmingContext stemmContext { get; set; }

        public WeightDictionary SelectedFeatures { get; set; }



        public List<FeatureVectorWithLabelID> trainingSet { get; set; } = new List<FeatureVectorWithLabelID>();

        public List<FeatureVectorWithLabelID> testSet { get; set; } = new List<FeatureVectorWithLabelID>();

        public List<FeatureVectorWithLabelID> testResults { get; set; } = new List<FeatureVectorWithLabelID>();

        public FeatureSpace featureSpace { get; set; } = new FeatureSpace();

        public Dictionary<string, FeatureVector> featureVectorByName = new Dictionary<string, FeatureVector>();


        public Relationships<SpaceLabel, SpaceDocumentModel> LabelToDocumentLinks { get; set; } = new Relationships<SpaceLabel, SpaceDocumentModel>();

        /*
        public HtmlDocument GetHtmlDocument(WebSiteDocument doc)
        {
            String AssignedID = doc.AssociatedID;
            if (!htmlDocumentByAssignedID.ContainsKey(AssignedID))
            {

                var htmlRelDoc = new HtmlDocument();
                htmlRelDoc.OptionOutputUpperCase = true;


                htmlRelDoc.LoadHtml(doc.HTMLSource);
                htmlDocumentByAssignedID.Add(AssignedID, htmlRelDoc);
            }
            return htmlDocumentByAssignedID[AssignedID];
        }

        public HtmlDocument GetHtmlDocument(String AssignedID)
        {
            if (!htmlDocumentByAssignedID.ContainsKey(AssignedID))
            {

                var htmlRelDoc = new HtmlDocument();
                htmlRelDoc.OptionOutputUpperCase = true;


                htmlRelDoc.LoadHtml(webDocumentByAssignedID[AssignedID].HTMLSource);
                htmlDocumentByAssignedID.Add(AssignedID, htmlRelDoc);
            }
            return htmlDocumentByAssignedID[AssignedID];
        }
        */

        /// <summary>
        /// Gets or sets the vector space.
        /// </summary>
        /// <value>
        /// The vector space.
        /// </value>
        public VectorSpace vectorSpace { get; set; } = new VectorSpace();


        /// <summary>
        /// Initializes a new instance of the <see cref="OperationContext"/> class.
        /// </summary>
        public OperationContext()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationContext"/> class.
        /// </summary>
        /// <param name="_dataset">The dataset.</param>
        /// <param name="log">The log.</param>
        public OperationContext(IEnumerable<WebSiteDocumentsSet> _dataset, ILogBuilder log)
        {
            DeployDataSet(_dataset, log);
        }



        private void DeployCategory(WebSiteDocumentsSet set)
        {
            String labelName = set.name;

            if (labelName.isNullOrEmpty())
            {
                labelName = SpaceLabel.UNKNOWN;
            }
            SpaceLabel lab = new SpaceLabel(labelName);
            if (labelName == SpaceLabel.UNKNOWN)
            {
                spaceModel.label_unknown = lab;
            }
            else
            {

                spaceModel.labels.Add(lab);
            }

            spaceLabels.Add(lab.name, lab);
            dataset.Add(labelName, set);

            foreach (WebSiteDocuments site in set)
            {
                spaceLabelsDomains.Add(site.domain, lab);

                webSiteByDomain.Add(site.domain, site);


                List<WebSiteDocument> toRemove = new List<WebSiteDocument>();

                foreach (WebSiteDocument doc in site.documents)
                {
                    if (webDocumentByAssignedID.ContainsKey(doc.AssignedID))
                    {
                        toRemove.Add(doc);
                    }
                    else
                    {
                        webDocumentByAssignedID.Add(doc.AssignedID, doc);
                        spaceLabelByDocAssignedID.Add(doc.AssignedID, lab);
                    }
                }

                toRemove.ForEach(x => site.documents.Remove(x));
            }
        }

        public String name { get; set; } = "";
        public ITokenizer tokenizer { get; set; }
        public Dictionary<string, ContentMetrics> entityMetrics { get; set; } = new Dictionary<string, ContentMetrics>();

        public void DeployDataSet(IEnumerable<WebSiteDocumentsSet> _dataset, ILogBuilder log)
        {
            if (IsDatasetDeployed)
            {
                log.log("DataSet deployement called on already deployed context!");
            }

            dataset = new Dictionary<string, WebSiteDocumentsSet>();
            webSiteByDomain = new Dictionary<string, WebSiteDocuments>();
            webDocumentByAssignedID = new Dictionary<string, WebSiteDocument>();
            spaceLabelsDomains = new Dictionary<string, SpaceLabel>();

            spaceModel = new SpaceModel();
            spaceLabels = new Dictionary<string, SpaceLabel>();

            dataSetSignature = _dataset.GetDataSetSignature();

            if (_dataset is ExperimentDataSetFold _fold)
            {
                name = _fold.name;
            }
            else
            {
                name = dataSetSignature;
            }

            UnlabeledSites = _dataset.GetUnlabeledDataSet();

            List<WebSiteDocumentsSet> inputSets = _dataset.ToList();
            if (UnlabeledSites != null)
            {
                inputSets.Remove(UnlabeledSites);
                inputSets.Insert(0, UnlabeledSites);
            }


            foreach (WebSiteDocumentsSet set in inputSets)
            {
                DeployCategory(set);
            }

        }

    }
}
