using imbNLP.Toolkit.Processing;
using imbSCI.Data.interfaces;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Feature
{

    /// <summary>
    /// Final product of the model, fector to be fed into classification algorithm
    /// </summary>
    /// <seealso cref="imbSCI.Data.interfaces.IObjectWithName" />
    public class FeatureVector : IObjectWithName, IVectorDimensions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureVector"/> class.
        /// </summary>
        public FeatureVector()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureVector"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public FeatureVector(String _name)
        {
            name = _name;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public String name { get; set; } = "";

        /// <summary>
        /// Gets or sets the dimensions.
        /// </summary>
        /// <value>
        /// The dimensions.
        /// </value>
        public Double[] dimensions { get; set; }

        public bool Equals(IVectorDimensions other)
        {
            return name == other.name;
        }
    }

}