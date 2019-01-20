using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Weighting.Global;
using imbSCI.Core.reporting;
using System;

namespace imbNLP.Toolkit.Weighting.Local
{

    public interface ILocalElement : IWeightingElementBase
    {
        void PrepareTheModel(SpaceModel space, ILogBuilder log);

        Double GetElementFactor(string term, SpaceDocumentModel document);

        WeightDictionaryEntry GetElementFactorEntry(string term, SpaceDocumentModel document);


    }

}