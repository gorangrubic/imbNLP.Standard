using System;
using System.Linq;
using System.Collections.Generic;
using imbNLP.Toolkit.Entity;
using imbNLP.Toolkit.Feature.Settings;
using imbNLP.Toolkit.Planes.Core;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Stemmers.Shaman;
using imbNLP.Toolkit.Weighting.Global;
using imbSCI.Core.reporting;

namespace imbNLP.Toolkit.Planes
{

    [Flags]
    public enum PlanesReportOptions
    {
        none = 0,
        report_corpusDictionary = 1,
        report_documentDictionary = 2,
        report_categoryDictionary = 4,

        report_selectedFeatures = 8,

        report_documentBoWModels = 16,
        report_categoryBoWModels = 32,

        report_featureVectors = 64,

        report_fold_stats = 128,
        report_fold_contentAnalysis = 256,
        report_fold_textrender = 512,
    }

}