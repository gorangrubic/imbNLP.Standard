using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Feature.Settings
{


    /// <summary>
    /// Serializable specification for dimensions
    /// </summary>
    public class dimensionSpecification
    {

        /// <summary>
        /// Human oriented dimension specification
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public String name { get; set; } = "";


        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public String description { get; set; } = "";


        /// <summary>
        /// Initializes a new instance of the <see cref="dimensionSpecification"/> class.
        /// </summary>
        public dimensionSpecification()
        {

        }


        public void Deploy(String _name, String _description, FeatureVectorDimensionType _type, String _functionName = "")
        {
            name = _name;
            description = _description;
            type = _type;
            functionName = _functionName;
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
