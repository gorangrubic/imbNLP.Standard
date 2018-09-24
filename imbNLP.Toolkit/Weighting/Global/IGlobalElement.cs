using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Weighting.Global
{

    public interface IGlobalElement
    {
        void PrepareTheModel(SpaceModel space);

        WeightDictionaryEntry GetElementFactorEntry(string term, SpaceModel space, SpaceLabel label = null);

        WeightDictionary GetElementFactors(IEnumerable<String> terms, SpaceModel space, SpaceLabel label = null);

        Double GetElementFactor(string term, SpaceModel space, SpaceLabel label = null);

        Boolean IsEnabled { get; set; }

        void Describe(ILogBuilder logger);

        String shortName { get; set; }

        FunctionResultTypeEnum resultType { get; }


    }

}