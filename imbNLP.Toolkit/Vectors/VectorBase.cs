using imbNLP.Toolkit.Processing;

namespace imbNLP.Toolkit.Vectors
{


    /// <summary>
    /// Common base for elements of <see cref="VectorSpace"/>
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Vectors.IVector" />
    public abstract class VectorBase : IVector
    {


        /// <summary>
        /// Weighted terms
        /// </summary>
        /// <value>
        /// The terms.
        /// </value>
        public WeightDictionary terms { get; set; } = new WeightDictionary();

        /// <summary>
        /// ID of the vector
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string name { get; set; } = "";
    }

}