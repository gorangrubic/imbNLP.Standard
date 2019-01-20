using imbNLP.Toolkit.Entity;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Stemmers.Shaman;
using imbNLP.Toolkit.Weighting;
using imbSCI.Core.extensions.data;
using imbSCI.Core.reporting;
using System;

namespace imbNLP.Toolkit.Planes
{

    public class CorpusPlaneMethodSettings : PlaneSettingsBase
    {
        private FeatureFilter _filter = new FeatureFilter();


        private FeatureWeightModel _weightModel = new FeatureWeightModel();

        public CorpusPlaneMethodSettings()
        {

        }

        public String stemmer { get; set; } = nameof(EnglishStemmer);

        public String tokenizer { get; set; } = nameof(TokenizerBasic);

        public String transliterationRuleSetId { get; set; } = "";

        /// <summary>
        /// Gets or sets the blender.
        /// </summary>
        /// <value>
        /// The blender.
        /// </value>
        public DocumentBlenderFunction blender { get; set; } = new DocumentBlenderFunction();



        public FeatureFilter filter
        {
            get { return _filter; }
            set
            {

                _filter = value;
                OnPropertyChange(nameof(filter));
            }
        }

        /// <summary>
        /// Gets or sets the weight model.
        /// </summary>
        /// <value>
        /// The weight model.
        /// </value>
        public FeatureWeightModel WeightModel
        {
            get { return _weightModel; }
            set
            {

                _weightModel = value;
                OnPropertyChange(nameof(WeightModel));
            }
        }


        public override void Describe(ILogBuilder logger)
        {
            if (logger != null)
            {
                logger.AppendPair("Stemmer", stemmer, true, "\t\t\t");

                logger.AppendPair("Tokenizer", tokenizer, true, "\t\t\t");

                logger.AppendPair("Transliteration", !transliterationRuleSetId.isNullOrEmpty(), true, "\t\t\t");

                filter.Describe(logger);

                WeightModel.Describe(logger);
            }

        }
    }

}