
using imbNLP.Toolkit.Functions;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Vectors;
using System;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Feature
{

    /// <summary>
    /// Dimension constructor based on similarity functions.
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Feature.FeatureSpaceDimensionBase" />
    public class FeatureSpaceDimensionSimilarity : FeatureSpaceDimensionBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureSpaceDimensionSimilarity"/> class.
        /// </summary>
        public FeatureSpaceDimensionSimilarity()
        {

        }

        ///// <summary>
        ///// Label associated with this domension i.e. class 
        ///// </summary>
        ///// <value>
        ///// The label.
        ///// </value>
        //public SpaceLabel label { get; set; }

        /// <summary>
        /// Similarity function used to compute the dimension between document and class FeatureVectors
        /// </summary>
        /// <value>
        /// The similarity function.
        /// </value>
        public IVectorSimilarityFunction similarityFunction { get; set; }


        /// <summary>
        /// Reference to a FeatureVector that represents a class (in multi-class classification problem)
        /// </summary>
        /// <value>
        /// The class vector.
        /// </value>
        [XmlIgnore]
        public IVector classVector { get; set; }


        public override double ComputeDimension(WeightDictionary vector, Int32 d = 0)
        {
            var entry = similarityFunction.ComputeSimilarity(vector, classVector.terms);  //vector.GetValue(term, d);
            return entry;
        }

        /// <summary>
        /// Computes the dimension value for the given vector
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns></returns>
        public override double ComputeDimension(IVector vector, Int32 d = 0)
        {
            if (similarityFunction == null)
            {
                throw new System.Exception("Similarity function instance not set for a FeatureSpace dimension! [" + classVector.name + "]");
            }
            return similarityFunction.ComputeSimilarity(vector.terms, classVector.terms);
        }
    }

}