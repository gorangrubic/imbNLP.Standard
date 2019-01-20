using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Weighting.Global;
using imbSCI.Core.reporting;

namespace imbNLP.Toolkit.Weighting.Local
{
    public abstract class LocalElementBase : WeightingElementBase, IDescribe, ILocalElement
    {


        public abstract void PrepareTheModel(SpaceModel space, ILogBuilder log);


        // public abstract WeightDictionaryEntry GetElementFactor(string term, SpaceDocumentModel document);

        public abstract double GetElementFactor(string term, SpaceDocumentModel document);


        public abstract WeightDictionaryEntry GetElementFactorEntry(string term, SpaceDocumentModel document);

        public abstract void Describe(ILogBuilder logger);



        // protected SpaceModel space { get; set; } = null;
    }

}