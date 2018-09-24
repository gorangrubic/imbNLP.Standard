using imbNLP.Toolkit.Processing;
using imbSCI.Data.interfaces;

namespace imbNLP.Toolkit.Vectors
{

    /// <summary>
    /// Vector in the <see cref="VectorSpace"/>
    /// </summary>
    /// <seealso cref="imbSCI.Data.interfaces.IObjectWithName" />
    public interface IVector : IObjectWithName
    {
        /// <summary>
        /// ID of the vector
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string name { get; set; }

        /// <summary>
        /// Weighted terms
        /// </summary>
        /// <value>
        /// The terms.
        /// </value>
        WeightDictionary terms { get; set; }

    }

}