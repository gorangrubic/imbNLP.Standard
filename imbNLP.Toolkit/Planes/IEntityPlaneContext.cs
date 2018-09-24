using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Feature;
using imbNLP.Toolkit.Planes.Core;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Vectors;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Planes
{

    public interface IEntityPlaneContext : IPlaneContext
    {
        List<WebSiteDocumentsSet> dataset { get; set; }
    }

}