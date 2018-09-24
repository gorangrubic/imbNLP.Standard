using System;

namespace imbNLP.PartOfSpeech.flags.data
{
    [Flags]
    public enum dat_emailType
    {
        none = 0,

        general = 1,

        /// <summary>
        /// Personal> contains name and/or lastname
        /// </summary>
        personal = 2,

        departmant = 4,

        companyDomain = 1 << 10,

        companyAltDomain = 1 << 11,

        /// <summary>
        /// The public service: like gmail, hotmail, yahoo etc
        /// </summary>
        publicService = 1 << 12,
    }
}