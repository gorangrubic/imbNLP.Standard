using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Planes.Core;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Planes
{

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Planes.ICorpusPlaneContext" />
    /// <seealso cref="imbNLP.Toolkit.Planes.Core.IPlaneContext" />
    public class CorpusPlaneContext : PlaneContextBase, ICorpusPlaneContext, IPlaneContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CorpusPlaneContext"/> class.
        /// </summary>
        public CorpusPlaneContext()
        {

        }




        /// <summary>
        /// Gets or sets the corpus documents.
        /// </summary>
        /// <value>
        /// The corpus documents.
        /// </value>
        public List<TextDocument> corpus_documents { get; set; } = new List<TextDocument>();

        public WeightDictionary SelectedFeatures { get; set; } = new WeightDictionary();

        public WeightDictionary CorpusGlobalWeights { get; set; } = new WeightDictionary();

        [XmlIgnore]
        public StemmingContext stemmContext { get; set; }

        public SpaceModel space { get; set; } = new SpaceModel();

        /// <summary>
        /// Gets or sets the dataset.
        /// </summary>
        /// <value>
        /// The dataset.
        /// </value>
        public List<WebSiteDocumentsSet> dataset { get; set; } = new List<WebSiteDocumentsSet>();

    }

}