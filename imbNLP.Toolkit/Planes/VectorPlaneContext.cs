using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Planes.Core;
using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Vectors;

namespace imbNLP.Toolkit.Planes
{

    /// <summary>
    /// Vector Plane context
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Planes.IVectorPlaneContext" />
    /// <seealso cref="imbNLP.Toolkit.Planes.Core.IPlaneContext" />
    public class VectorPlaneContext : PlaneContextBase, IVectorPlaneContext, IPlaneContext
    {
        public VectorPlaneContext() { }



        public Relationships<SpaceLabel, SpaceDocumentModel> LabelToDocumentLinks { get; set; } = new Relationships<SpaceLabel, SpaceDocumentModel>();

        /// <summary>
        /// Gets or sets the vector space.
        /// </summary>
        /// <value>
        /// The vector space.
        /// </value>
        public VectorSpace vectorSpace { get; set; } = new VectorSpace();


    }

}