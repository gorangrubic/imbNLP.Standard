using System;

namespace imbNLP.Toolkit.Documents.Ranking.Data
{
    //public class DocumentSelectQueryPrecompiled 

    [Flags]
    public enum DocumentSelectQueryOptions
    {
        none = 0,
        /// <summary>
        /// The apply domain level limits
        /// </summary>
        ApplyDomainLevelLimits = 1,

        /// <summary>
        /// The force home page
        /// </summary>
        ForceHomePage = 2,

        /// <summary>
        /// Performs domain level normalization before applying treshold filter
        /// </summary>
        DomainLevelNormalization = 4,

        /// <summary>
        /// The iterative selection
        /// </summary>
        IterativeSelection = 8
    }
}