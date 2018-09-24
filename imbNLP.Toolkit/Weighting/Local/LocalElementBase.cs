using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbSCI.Core.reporting;
using System;

namespace imbNLP.Toolkit.Weighting.Local
{
    public abstract class LocalElementBase : IDescribe, ILocalElement
    {
        public String shortName { get; set; } = "local";

        public abstract void PrepareTheModel(SpaceModel space);


        // public abstract WeightDictionaryEntry GetElementFactor(string term, SpaceDocumentModel document);

        public abstract double GetElementFactor(string term, SpaceDocumentModel document);


        public abstract WeightDictionaryEntry GetElementFactorEntry(string term, SpaceDocumentModel document);

        public abstract void Describe(ILogBuilder logger);

        public Boolean IsEnabled { get; set; } = true;

        protected SpaceModel space { get; set; } = null;
    }

}