using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Weighting.Metrics;
using System;

namespace imbNLP.Toolkit.Weighting.Global
{

    /// <summary>
    /// Global function type
    /// </summary>
    public enum GlobalFunctionType
    {
        /// <summary>
        /// The icf - <see cref="ICFElement"/>
        /// </summary>
        ICF,
        /// <summary>
        /// The ic sd f - <see cref="ICSdFElement"/>
        /// </summary>
        ICSdF,
        /// <summary>
        /// The idf - <see cref="IDFElement"/>
        /// </summary>
        IDF,
        /// <summary>
        /// The igm - <see cref="IGMElement"/>
        /// </summary>
        IGM,
        /// <summary>
        /// The TDP - <see cref="CollectionTDPElement"/>
        /// </summary>
        TDP,
    }

}