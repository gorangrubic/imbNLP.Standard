using imbNLP.Toolkit.Documents.FeatureAnalytics.Core;
using System;

namespace imbNLP.Toolkit.Documents.FeatureAnalytics.Data
{
    public static class FeatureCWPAnalysisExtensions
    {
        public static String GetSignature(this CWPAnalysusScoreOutput computation)
        {
            switch (computation)
            {
                case CWPAnalysusScoreOutput.binaryCWParticularity:
                    return "BCP";
                    break;

                case CWPAnalysusScoreOutput.siteParticularity:
                    return "SP";
                    break;

                case CWPAnalysusScoreOutput.categoryParticularity:
                    return "CP";
                    break;

                case CWPAnalysusScoreOutput.flatSiteParticularity:
                    return "FSP";
                    break;

                case CWPAnalysusScoreOutput.siteCommonality:
                    return "SC";
                    break;

                case CWPAnalysusScoreOutput.categoryCommonality:
                    return "CC";
                    break;

                case CWPAnalysusScoreOutput.frequencyDensity:
                    return "FD";
                    break;

                case CWPAnalysusScoreOutput.globalMinDensity:
                    return "GMD";
                    break;

                case CWPAnalysusScoreOutput.IDFIWFICF:
                case CWPAnalysusScoreOutput.IWFICF:
                    return computation.ToString();
                    break;
            }
            return computation.ToString().Replace(",", "");
        }

        public static CWPAnalysisScopeEnum GetScope(this CWPAnalysusScoreOutput computation)
        {
            CWPAnalysisScopeEnum RequiredScopes = CWPAnalysisScopeEnum.nothing;

            switch (computation)
            {
                case CWPAnalysusScoreOutput.categoryCommonality:
                case CWPAnalysusScoreOutput.categoryParticularity:
                    RequiredScopes = CWPAnalysisScopeEnum.globalLevel;
                    break;
                case CWPAnalysusScoreOutput.globalMinDensity:
                case CWPAnalysusScoreOutput.frequencyDensity:
                    RequiredScopes = CWPAnalysisScopeEnum.globalLevel;
                    break;

                case CWPAnalysusScoreOutput.flatSiteParticularity:
                    RequiredScopes = CWPAnalysisScopeEnum.flatSiteLevel;
                    break;

                case CWPAnalysusScoreOutput.siteCommonality:
                case CWPAnalysusScoreOutput.siteParticularity:
                    RequiredScopes = CWPAnalysisScopeEnum.unitaryLevel;
                    break;

                case CWPAnalysusScoreOutput.binaryCWParticularity:
                    RequiredScopes = CWPAnalysisScopeEnum.mainLevel;
                    break;

                case CWPAnalysusScoreOutput.IWFICF:
                case CWPAnalysusScoreOutput.IDFIWFICF:
                    RequiredScopes = CWPAnalysisScopeEnum.rawLevel;
                    break;
            }

            if (computation.HasFlag(CWPAnalysusScoreOutput.category))
            {
                RequiredScopes |= CWPAnalysisScopeEnum.categoryLevel;
            }

            if (computation.HasFlag(CWPAnalysusScoreOutput.site))
            {
                RequiredScopes |= CWPAnalysisScopeEnum.siteLevel;
            }

            if (computation.HasFlag(CWPAnalysusScoreOutput.flat))
            {
                RequiredScopes |= CWPAnalysisScopeEnum.flatSiteLevel;
            }

            return RequiredScopes;
        }
    }
}