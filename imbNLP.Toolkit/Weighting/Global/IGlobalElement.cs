using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Weighting.Global
{
    public interface IGlobalElement : IWeightingElementBase
    {
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