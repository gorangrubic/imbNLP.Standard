namespace imbNLP.PartOfSpeech.flags.data
{
    /// <summary>
    /// Legal form of the organization
    /// </summary>
    public enum dat_legalForm
    {
        none = 0,

        /// <summary>
        /// The limited: limited liability, owner is not legaly bound to liabilities. LTD, doo
        /// </summary>
        limited = 1,

        /// <summary>
        /// The corporation: ownership is splitted to stocks, traded or not traded on stock exchange: inc, ad
        /// </summary>
        corporation = 2,

        /// <summary>
        /// The entherprise: full liability, owner is legaly bound to liabilities of the legal entity, sr, str
        /// </summary>
        entherprise = 4,

        nonProfit = 8,

        /// <summary>
        /// The public service company: State owned company providing public services
        /// </summary>
        publicServiceCompany = 16,
    }
}