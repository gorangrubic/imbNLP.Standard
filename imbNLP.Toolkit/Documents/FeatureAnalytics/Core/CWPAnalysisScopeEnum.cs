using System;

namespace imbNLP.Toolkit.Documents.FeatureAnalytics.Core
{
    [Flags]
    public enum CWPAnalysisScopeEnum
    {
        none = 0,
        siteLevel = 1,
        categoryLevel = 2,
        flatSiteLevel = 4,
        unitaryLevel = categoryLevel | 8,

        mainLevel = categoryLevel | 16,

        rawLevel = 32,
        globalLevel = 64,

        all = siteLevel | categoryLevel | flatSiteLevel | unitaryLevel | mainLevel | rawLevel | globalLevel,
        nothing = 128
    }
}