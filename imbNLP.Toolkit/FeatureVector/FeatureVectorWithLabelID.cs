using System;
namespace imbNLP.Toolkit.Feature
{





    /// <summary>
    /// 
    /// </summary>
    public class FeatureVectorWithLabelID
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureVectorWithLabelID"/> class.
        /// </summary>
        public FeatureVectorWithLabelID()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureVectorWithLabelID"/> class.
        /// </summary>
        /// <param name="_vector">The vector.</param>
        /// <param name="_labelID">The label identifier.</param>
        public FeatureVectorWithLabelID(FeatureVector _vector, Int32 _labelID)
        {
            vector = _vector;
            labelID = _labelID;
        }

        /// <summary>
        /// Gets or sets the vector.
        /// </summary>
        /// <value>
        /// The vector.
        /// </value>
        public FeatureVector vector { get; set; } = null;
        /// <summary>
        /// Gets or sets the label identifier.
        /// </summary>
        /// <value>
        /// The label identifier.
        /// </value>
        public Int32 labelID { get; set; } = -1;

    }

}