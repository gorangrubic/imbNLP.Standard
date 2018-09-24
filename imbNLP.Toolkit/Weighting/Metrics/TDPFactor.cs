namespace imbNLP.Toolkit.Weighting.Metrics
{
    /// <summary>
    /// Computation mode for Term Discriminating Power
    /// </summary>
    public enum TDPFactor
    {
        none,

        /// <summary>
        /// Inverse Document Frequency
        /// </summary>
        idf,

        /// <summary>
        /// Probabilistic Inverse Document Frequency
        /// </summary>
        idf_prob,

        /// <summary>
        /// Chi Square
        /// </summary>
        chi,

        /// <summary>
        /// Information Gain
        /// </summary>
        ig,

        /// <summary>
        /// Gain Ratio
        /// </summary>
        gr,

        /// <summary>
        /// Odds Ratio
        /// </summary>
        or,

        /// <summary>
        /// Relevance frequency
        /// </summary>
        rf,
    }
}