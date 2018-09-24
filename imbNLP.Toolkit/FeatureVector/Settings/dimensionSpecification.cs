using System;

namespace imbNLP.Toolkit.Feature.Settings
{

    /// <summary>
    /// Serializable specification for dimensions
    /// </summary>
    public class dimensionSpecification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="dimensionSpecification"/> class.
        /// </summary>
        public dimensionSpecification()
        {

        }


        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public FeatureVectorDimensionType type { get; set; }

        /// <summary>
        /// Dimension computation class name
        /// </summary>
        /// <value>
        /// The name of the function.
        /// </value>
        public String functionName { get; set; } = "";


    }
}
