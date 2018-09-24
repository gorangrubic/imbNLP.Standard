using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Planes.Core;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Planes
{
    /// <summary>
    /// Context for Entity Plane
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Planes.IEntityPlaneContext" />
    /// <seealso cref="imbNLP.Toolkit.Planes.Core.IPlaneContext" />
    public class EntityPlaneContext : PlaneContextBase, IEntityPlaneContext, IPlaneContext
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityPlaneContext"/> class.
        /// </summary>
        public EntityPlaneContext()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityPlaneContext"/> class.
        /// </summary>
        /// <param name="_dataset">The dataset.</param>
        public EntityPlaneContext(List<WebSiteDocumentsSet> _dataset)
        {
            dataset = _dataset;
        }


        /// <summary>
        /// Gets or sets the dataset.
        /// </summary>
        /// <value>
        /// The dataset.
        /// </value>
        public List<WebSiteDocumentsSet> dataset { get; set; } = new List<WebSiteDocumentsSet>();


    }
}
