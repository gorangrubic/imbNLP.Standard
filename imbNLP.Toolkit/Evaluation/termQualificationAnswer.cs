namespace imbNLP.Toolkit.Evaluation
{
    /// <summary>
    /// Response or value association, for a <see cref="termQualification"/>
    /// </summary>
    public enum termQualificationAnswer
    {
        /// <summary>
        /// The term is irrelevant, for both approaches
        /// </summary>
        irrelevant = -1,

        /// <summary>
        /// The term is not accepted, nor denied
        /// </summary>
        neutral = 0,

        /// <summary>
        /// The term is inclusivly acceptable, but not in exclusive approach
        /// </summary>
        inclusive = 1,

        /// <summary>
        /// The term is suitable in exclusive sense, making it acceptable in inclusive too
        /// </summary>
        exclusive = 2,
    }
}