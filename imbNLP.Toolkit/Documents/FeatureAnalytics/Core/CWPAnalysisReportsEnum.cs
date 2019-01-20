using imbNLP.Toolkit.Documents.Ranking;
using imbNLP.Toolkit.Documents.Ranking.Core;
using imbNLP.Toolkit.Documents.Ranking.Data;
using imbNLP.Toolkit.ExperimentModel;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Weighting;
using imbSCI.Core.files.folders;
using imbSCI.Core.reporting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace imbNLP.Toolkit.Documents.FeatureAnalytics.Core
{
[Flags]
    public enum CWPAnalysisReportsEnum
    {

        none = 0,
        reportDatasetStructure = 1,
        reportHTMLContentStructure = 4,
        reportDatasetMetrics = 8,
        reportGraphAnalytics = 16,

        reportCategoryOverlap = 32,
        reportTermDistribution = 64,

        reportCWPAnalytics = 128,


        all = reportCategoryOverlap | reportHTMLContentStructure | CWPAnalysisReportsEnum.reportDatasetMetrics | CWPAnalysisReportsEnum.reportDatasetStructure
            | CWPAnalysisReportsEnum.reportGraphAnalytics | CWPAnalysisReportsEnum.reportHTMLContentStructure | CWPAnalysisReportsEnum.reportTermDistribution | reportCWPAnalytics




    }
}