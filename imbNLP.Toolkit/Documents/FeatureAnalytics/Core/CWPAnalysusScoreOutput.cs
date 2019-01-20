using System;

namespace imbNLP.Toolkit.Documents.FeatureAnalytics.Core
{
    [Flags]
    public enum CWPAnalysusScoreOutput
    {
        none = 0,
        frequency = 1,
        minimum = 2,
        maximum = 4,
        particularity = 8,
        commonality = 16,
        inverse = 32,
        page = 64,
        site = 128,
        category = 256,
        flat = 512,
        density = 1024,

        binaryCWParticularity = 2024,

        siteParticularity = site | particularity,
        categoryParticularity = category | particularity,
        flatSiteParticularity = flat | site | particularity,
        siteCommonality = site | commonality,
        categoryCommonality = category | commonality,
        frequencyDensity = density | category,
        globalMinDensity = density | category | minimum,

        IDFIWFICF = inverse | frequency | page | site | category,

        IWFICF = inverse | frequency | site | category,
    }
}