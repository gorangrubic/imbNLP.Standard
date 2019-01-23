using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Weighting.Global
{

    public interface ISharedDataPool
    {
        /// <summary>
        /// name of function produced the data
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        String name { get; set; }

        /// <summary>
        /// Checks if something should be recomputed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settings">The settings.</param>
        void CheckRequirements<T>(T __settings) where T : class;

    }

    public interface IGlobalElementWithSharedData : IGlobalElement
    {

        ISharedDataPool GetSharedDataStructure();

        void SetSharedDataStructure(ISharedDataPool data);


    }

    

    public interface IGlobalElement : IWeightingElementBase
    {
        void DoIndexNormalization(ILogBuilder log);

        void PrepareTheModel(SpaceModel space, ILogBuilder log);

        WeightDictionaryEntry GetElementFactorEntry(string term, SpaceModel space, SpaceLabel label = null);

        WeightDictionary GetElementFactors(IEnumerable<String> terms, SpaceModel space, SpaceLabel label = null);

        Double GetElementFactor(string term, SpaceModel space, SpaceLabel label = null);

        Dictionary<Double, String> DistinctReturns { get; }

        void Describe(ILogBuilder logger);

        void DeploySettings(GlobalFunctionSettings settings);

        FunctionResultTypeEnum resultType { get; }
    }
}