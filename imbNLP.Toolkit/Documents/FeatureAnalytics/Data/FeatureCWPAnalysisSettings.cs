using imbNLP.Toolkit.Documents.FeatureAnalytics.Core;
using System;

namespace imbNLP.Toolkit.Documents.FeatureAnalytics.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class FeatureCWPAnalysisSettings
    {
        /// <summary>
        /// 
        /// </summary>
        public enum AnalysisPurpose
        {
            /// <summary>
            /// The application - it will make perform minimal computations, only required for application to feature selection and weighting
            /// </summary>
            application,

            /// <summary>
            /// The exploration - it will perform all computations
            /// </summary>
            exploration
        }

        public CWPAnalysusScoreOutput Computation { get; set; } = CWPAnalysusScoreOutput.siteParticularity;

        public CWPAnalysisScopeEnum RequiredScopes { get; set; } = CWPAnalysisScopeEnum.siteLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureCWPAnalysisSettings"/> class.
        /// </summary>
        /// <param name="computation">The computation.</param>
        /// <param name="purpose">The purpose.</param>
        public FeatureCWPAnalysisSettings(CWPAnalysusScoreOutput computation=CWPAnalysusScoreOutput.flatSiteParticularity, AnalysisPurpose purpose=AnalysisPurpose.application)
        {
            if (purpose == AnalysisPurpose.exploration)
            {
                RequiredScopes = CWPAnalysisScopeEnum.all;
            }
            else
            {
                RequiredScopes = computation.GetScope();
            }
            Computation = computation;
        }

        /// <summary>
        /// Gets or sets the purpose.
        /// </summary>
        /// <value>
        /// The purpose of analysis
        /// </value>
        public AnalysisPurpose purpose { get; set; } = AnalysisPurpose.application;

        // public Boolean DoMakeDataSetAnalysis { get; set; } = true;

        public Double Particularity { get; set; } = 0.75;
        public Double Commonality { get; set; } = 0.05;
        public double HighFrequency { get; set; } = 0.50;
        public double LowFrequency { get; set; } = 0.25;
    }
}