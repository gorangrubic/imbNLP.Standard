using imbNLP.Toolkit.Documents.FeatureAnalytics;
using imbNLP.Toolkit.Documents.FeatureAnalytics.Core;
using imbNLP.Toolkit.Documents.FeatureAnalytics.Data;
using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Weighting.Data;
using imbSCI.Core.extensions.enumworks;
using imbSCI.Core.extensions.text;
using imbSCI.Core.reporting;
using System;
using System.Linq;
using static imbNLP.Toolkit.Weighting.Global.IDFElement;

namespace imbNLP.Toolkit.Weighting.Global
{

    public enum GeneralComputationOptionEnum
    {
        inverse,
        log
    }

    public class CWPElement : GlobalElementBase
    {



        public CWPElement()
        {
            shortName = "CWP";

            description = "Class-Website-Page density frenquency";
        }




        public String property { get; set; } = nameof(FeatureCWPAnalysisSiteMetrics.particularity_score);


        public CWPAnalysusScoreOutput computation { get; set; } = CWPAnalysusScoreOutput.siteParticularity;

        public IDFComputation normalization { get; set; } = IDFComputation.DF;

        public GeneralComputationOptionEnum generalOption { get; set; } = GeneralComputationOptionEnum.inverse;

        protected FeatureCWPAnalysis CWPAnalysis { get; set; }

        public override void DeploySettings(GlobalFunctionSettings settings)
        {
            computation = imbEnumExtendBase.GetEnumFromStringFlags<CWPAnalysusScoreOutput>(settings.flags, computation).FirstOrDefault();
            normalization = imbEnumExtendBase.GetEnumFromStringFlags<IDFComputation>(settings.flags, normalization).FirstOrDefault();

            FeatureCWPAnalysisSettings CWPSettings = new FeatureCWPAnalysisSettings(computation, FeatureCWPAnalysisSettings.AnalysisPurpose.application);
            CWPAnalysis = new FeatureCWPAnalysis(CWPSettings);

            shortName = "CWP";
            shortName += computation.ToString().imbGetAbbrevation(3, true);
            shortName += normalization.ToString().imbGetAbbrevation(2, true);

        }



        protected double GetScore(string term)
        {

            Double score = CWPAnalysis.GetScore(term, computation);

            switch (generalOption)
            {
                case GeneralComputationOptionEnum.inverse:
                    score = 1 - score;
                    break;

            }



            switch (normalization)
            {
                case IDFComputation.DF:

                    break;
                case IDFComputation.logPlus:

                    score = Math.Log(score) + 1;
                    break;
                default:

                    break;
            }

            if (score == Double.NegativeInfinity) score = 0;
            return score;
        }


        public override double GetElementFactor(string term, SpaceModel space, SpaceLabel label = null)
        {
            Double score = 0;
            if (index.ContainsKey(term))
            {
                score = index[term];
            }
            else
            {
                score = GetScore(term);
            }

            if (!DistinctReturns.ContainsKey(score))
            {
                DistinctReturns.Add(score, term);
            }

            return score;


        }

        public override void LoadModelData(WeightingModelData data)
        {
            LoadModelDataBase(data);
        }

        public override void PrepareTheModel(SpaceModel space, ILogBuilder log)
        {
            CWPAnalysis.Prepare(space, null);

            CWPAnalysis.Analysis(null, null);

            foreach (String term in space.GetTokens(true, false))
            {
                index.Add(term, GetScore(term));
            }



        }

        public override WeightingModelData SaveModelData()
        {
            return SaveModelDataBase();
        }
    }
}