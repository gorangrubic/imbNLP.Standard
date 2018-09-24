using imbNLP.Toolkit.Corpora;
using imbNLP.Toolkit.Planes.Core;
using imbSCI.Core.extensions.data;
using imbSCI.Core.reporting;
using System;

namespace imbNLP.Toolkit.Planes
{

    public class CorpusPlaneMethodSettings : IPlaneSettings
    {
        public CorpusPlaneMethodSettings()
        {

        }

        public String stemmer { get; set; } = "";

        public String tokenizer { get; set; } = "";

        public String transliterationRuleSetId { get; set; } = "";

        public FeatureFilter filter { get; set; } = new FeatureFilter();

        public FeatureWeightModel weightModel { get; set; } = new FeatureWeightModel();


        public void Describe(ILogBuilder logger)
        {
            logger.AppendPair("Stemmer", stemmer, true, "\t\t\t");

            logger.AppendPair("Tokenizer", tokenizer, true, "\t\t\t");

            logger.AppendPair("Transliteration", !transliterationRuleSetId.isNullOrEmpty(), true, "\t\t\t");

            filter.Describe(logger);

            weightModel.Describe(logger);


        }
    }

}