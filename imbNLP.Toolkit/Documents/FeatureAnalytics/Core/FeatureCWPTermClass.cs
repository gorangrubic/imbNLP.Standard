using imbNLP.Toolkit.ExperimentModel;
using imbNLP.Toolkit.Processing;
using imbSCI.Core.files.folders;
using imbSCI.Data.collection.nested;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Documents.FeatureAnalytics.Core
{
public enum FeatureCWPTermClass
    {
        normal,
        /// <summary>
        /// Term is frequent for particular entry within the set
        /// </summary>
        particularForEntry,
        /// <summary>
        /// Term is present at few pages within each entry within the set
        /// </summary>
        particularForAspect,

        /// <summary>
        /// Term is common for the most of entries and has inner freq above half
        /// </summary>
        commonHighFrequency,

        /// <summary>
        /// Term is common for the most of entries and has inner freq below half
        /// </summary>
        commonLowFrequency,
        unevaluated,
    }
}