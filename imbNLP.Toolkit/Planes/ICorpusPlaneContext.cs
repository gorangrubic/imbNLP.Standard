using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Planes.Core;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Planes
{

    /// <summary>
    /// Corpus plane context
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Planes.Core.IPlaneContext" />
    public interface ICorpusPlaneContext : IPlaneContext
    {
        List<TextDocument> corpus_documents { get; set; }

        WeightDictionary SelectedFeatures { get; set; }

        WeightDictionary CorpusGlobalWeights { get; set; }

        StemmingContext stemmContext { get; set; }

        SpaceModel space { get; set; }

        List<WebSiteDocumentsSet> dataset { get; set; }
    }

}