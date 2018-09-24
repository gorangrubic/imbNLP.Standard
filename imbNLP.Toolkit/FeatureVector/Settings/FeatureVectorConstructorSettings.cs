using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Vectors;
using imbSCI.Core.reporting;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 
/// </summary>
namespace imbNLP.Toolkit.Feature.Settings
{

    public class FeatureVectorConstructorSettings : IDescribe
    {
        public FeatureVectorConstructorSettings()
        {

        }

        /// <summary>
        /// Dimensions computed as function between document and topics in the <see cref="VectorSpace"/>
        /// </summary>
        /// <value>
        /// The topic dimensions.
        /// </value>
        public List<dimensionSpecification> topicDimensions { get; set; } = new List<dimensionSpecification>();

        /// <summary>
        /// Dimensions based on direct funneling of the feature weights
        /// </summary>
        /// <value>
        /// The feature dimensions.
        /// </value>
        public List<dimensionSpecification> featureDimensions { get; set; } = new List<dimensionSpecification>();

        /// <summary>
        /// Dimensions to be computed for each category / class
        /// </summary>
        /// <value>
        /// The category dimensions.
        /// </value>
        public List<dimensionSpecification> labelDimensions { get; set; } = new List<dimensionSpecification>();


        //  public List<dimensionSpecification> dimensions { get; set; } = new List<dimensionSpecification>();
        public void AddDimensionSpecification(dimensionSpecification dimension)
        {

            switch (dimension.type)
            {
                case FeatureVectorDimensionType.directTermWeight:
                    featureDimensions.Add(dimension);
                    break;
                case FeatureVectorDimensionType.similarityFunction:
                    labelDimensions.Add(dimension);
                    break;
                case FeatureVectorDimensionType.topicWeight:
                    topicDimensions.Add(dimension);
                    break;
            }
        }


        protected void DescribeDimensions(ILogBuilder logger, List<dimensionSpecification> dimensions, string heading)
        {
            if (dimensions.Any())
            {
                logger.AppendLine(heading);
                logger.nextTabLevel();

                for (int i = 0; i < labelDimensions.Count; i++)
                {
                    dimensionSpecification ds = labelDimensions[i];
                    logger.AppendPair("[" + i.ToString("D2") + "]", ds.functionName, true, "\t\t\t");
                }

                logger.prevTabLevel();
            }
        }

        public void Describe(ILogBuilder logger)
        {
            DescribeDimensions(logger, featureDimensions, "Feature weight based dimensions");
            DescribeDimensions(logger, topicDimensions, "Document vs Topic based dimensions");
            DescribeDimensions(logger, labelDimensions, "Document vs Category based dimensions");
        }
    }

}