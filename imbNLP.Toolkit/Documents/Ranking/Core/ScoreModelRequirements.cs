using imbSCI.Core.extensions.text;
using imbSCI.Core.reporting.render;
using System;

namespace imbNLP.Toolkit.Documents.Ranking.Core
{
    public class ScoreModelRequirements
    {
        private Boolean _mayUseTextRender = false;
        private Boolean _mayUseSelectedFeatures = false;
        private Boolean _mayUseGraph = false;

        public ScoreModelRequirements()
        {

        }

        private void DescribeProperty(ITextRender logger, String pn, Object pv)
        {
            pn = pn.Replace("May", "");
            pn = pn.imbTitleCamelOperation();
            logger.AppendPair(pn, pv, true, ": ");
        }

        /// <summary>
        /// Describes the specified logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public void Describe(ITextRender logger)
        {
            DescribeProperty(logger, nameof(MayUseFeatureSpace), MayUseFeatureSpace);
            DescribeProperty(logger, nameof(MayUseTextRender), MayUseTextRender);
            DescribeProperty(logger, nameof(MayUseSelectedFeatures), MayUseSelectedFeatures);
            DescribeProperty(logger, nameof(MayUseGraph), MayUseGraph);

        }

        public Boolean MayUseSelectedFeatures
        {
            get { return _mayUseSelectedFeatures; }
            set { _mayUseSelectedFeatures = value; }
        }
        public Boolean MayUseTextRender
        {
            get
            {
                if (_mayUseSelectedFeatures)
                {
                    return true;
                }
                return _mayUseTextRender;
            }
            set { _mayUseTextRender = value; }
        }

        public Boolean MayUseGraph
        {
            get { return _mayUseGraph; }
            set { _mayUseGraph = value; }
        }

        public bool MayUseFeatureSpace { get; internal set; }
        public bool MayUseSpaceModel { get; internal set; }
        public bool MayUseSpaceModelCategories { get; internal set; }
        public bool MayUseResourceProvider { get; internal set; }
        public bool MayUseVectorSpaceCategories { get; set; }
    }
}