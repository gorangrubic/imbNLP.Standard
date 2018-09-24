using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using System;

namespace imbNLP.Toolkit.Weighting.Local
{

    public interface ILocalElement
    {
        void PrepareTheModel(SpaceModel space);

        Double GetElementFactor(string term, SpaceDocumentModel document);

        WeightDictionaryEntry GetElementFactorEntry(string term, SpaceDocumentModel document);

        Boolean IsEnabled { get; set; }
    }

}