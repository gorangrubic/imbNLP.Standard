using imbNLP.Toolkit.Feature.Settings;
using imbNLP.Toolkit.Functions;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Typology;
using imbNLP.Toolkit.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Feature
{

    /// <summary>
    /// Creates Feature vectors with dimensions specified 
    /// </summary>
    public class FeatureVectorConstructor
    {

        public FeatureVectorConstructor()
        {

        }


        public void Deploy(FeatureVectorConstructorSettings settings, WeightDictionary selectedFeatures)
        {
            foreach (dimensionSpecification ld in settings.featureDimensions)
            {

                foreach (var entry in selectedFeatures.entries)
                {
                    FeatureSpaceDimensionTerm dimensionTerm = new FeatureSpaceDimensionTerm(entry.name);


                    dimensionFunctionSet.Add(dimensionTerm);
                }
            }
        }

        /// <summary>
        /// Deploys the specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="vectorSpace">The vector space.</param>
        public void Deploy(FeatureVectorConstructorSettings settings, VectorSpace vectorSpace)
        {
            // dimensionFunctionSet = new List<FeatureSpaceDimensionBase>();

            // creates instances of dimension value providers - for each label in the vector space
            foreach (dimensionSpecification ld in settings.labelDimensions)
            {
                foreach (VectorLabel label in vectorSpace.labels)
                {
                    FeatureSpaceDimensionSimilarity dimensionInstance = new FeatureSpaceDimensionSimilarity();
                    IVectorSimilarityFunction functionInstance = (IVectorSimilarityFunction)TypeProviders.similarityFunctions.GetInstance(ld.functionName);

                    IVector classVector = vectorSpace.labels.First(x => x.name == label.name);
                    dimensionInstance.similarityFunction = functionInstance;
                    dimensionInstance.classVector = classVector;

                    dimensionFunctionSet.Add(dimensionInstance);
                }
            }

        }

        /// <summary>
        /// Dimension providers, instantiated 
        /// </summary>
        /// <value>
        /// The document vs cat dimension set.
        /// </value>
        public List<FeatureSpaceDimensionBase> dimensionFunctionSet { get; protected set; } = new List<FeatureSpaceDimensionBase>();

        /// <summary>
        /// Constructs a feature vector - having dimension values set by <see cref="dimensionFunctionSet"/>
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns></returns>
        public FeatureVector ConstructFeatureVector(IVector vector)
        {
            FeatureVector fv = new FeatureVector(vector.name);

            Int32 c = 0;
            Int32 d = vector.terms.nDimensions;
            fv.dimensions = new double[dimensionFunctionSet.Count * d];

            foreach (var dimension in dimensionFunctionSet)
            {
                for (int i = 0; i < d; i++)
                {
                    fv.dimensions[c] = dimension.ComputeDimension(vector, i);
                    c++;
                }
            }



            return fv;

        }



    }

}