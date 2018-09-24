using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Planes.Core;
using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Vectors;

namespace imbNLP.Toolkit.Planes
{

    /// <summary>
    /// Vector plane context interface
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Planes.Core.IPlaneContext" />
    public interface IVectorPlaneContext : IPlaneContext
    {
        /// <summary>
        /// Gets or sets the vector space.
        /// </summary>
        /// <value>
        /// The vector space.
        /// </value>
        VectorSpace vectorSpace { get; set; }


        Relationships<SpaceLabel, SpaceDocumentModel> LabelToDocumentLinks { get; set; }
    }

}