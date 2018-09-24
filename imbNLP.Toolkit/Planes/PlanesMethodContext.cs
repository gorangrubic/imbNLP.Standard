using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Feature;
using imbNLP.Toolkit.Planes.Core;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Vectors;
using imbSCI.Core.files.folders;
using imbSCI.Core.reporting;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Planes
{




    /// <summary>
    /// Context of the planes procedure
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Planes.IEntityPlaneContext" />
    /// <seealso cref="imbNLP.Toolkit.Planes.ICorpusPlaneContext" />
    /// <seealso cref="imbNLP.Toolkit.Planes.IVectorPlaneContext" />
    /// <seealso cref="imbNLP.Toolkit.Planes.IFeaturePlaneContext" />
    /// <seealso cref="imbNLP.Toolkit.Planes.Core.IPlaneContext" />
    public class PlanesMethodContext : PlaneContextBase, IEntityPlaneContext, ICorpusPlaneContext, IVectorPlaneContext, IFeaturePlaneContext, IPlaneContext
    {

        public PlanesMethodContext() { }

        #region experiment log-out

        public string name { get; set; } = "";

        public ILogBuilder logger { get; set; }


        public folderNode folder { get; set; }

        #endregion








        #region Entity Plane Context

        /// <summary>
        /// Reference to the fold
        /// </summary>
        /// <value>
        /// The dataset.
        /// </value>
        public List<WebSiteDocumentsSet> dataset { get; set; } = new List<WebSiteDocumentsSet>();

        #endregion

        #region Corpus Plane Context
        public List<TextDocument> corpus_documents { get; set; } = new List<TextDocument>();

        public WeightDictionary SelectedFeatures { get; set; } = new WeightDictionary();

        public WeightDictionary CorpusGlobalWeights { get; set; } = new WeightDictionary();

        [XmlIgnore]
        public StemmingContext stemmContext { get; set; }

        public SpaceModel space { get; set; } = new SpaceModel();
        #endregion


        #region Vector Plane

        public VectorSpace vectorSpace { get; set; } = new VectorSpace();

        public Relationships<SpaceLabel, SpaceDocumentModel> LabelToDocumentLinks { get; set; } = new Relationships<SpaceLabel, SpaceDocumentModel>();


        #endregion


        #region Feature Plane

        public FeatureSpace featureSpace { get; set; } = new FeatureSpace();

        public List<FeatureVectorWithLabelID> trainingSet { get; set; } = new List<FeatureVectorWithLabelID>();

        public List<FeatureVector> testSet { get; set; } = new List<FeatureVector>();

        public List<FeatureVectorWithLabelID> testResults { get; set; } = new List<FeatureVectorWithLabelID>();


        #endregion

    }

}