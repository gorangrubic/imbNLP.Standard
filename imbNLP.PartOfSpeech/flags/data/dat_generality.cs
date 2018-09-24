using System;

namespace imbNLP.PartOfSpeech.flags.data
{
    /// <summary>
    /// Describes generality as found by method of combined (2D) corpus analysis
    /// </summary>
    [Flags]
    public enum dat_generality
    {
        none = 0,

        /// <summary>
        /// The particular outside domain: y+,x-
        /// </summary>
        particularOutsideDomain = 1,

        /// <summary>
        /// The particular inside domain: x-,y-
        /// </summary>
        particularInsideDomain = 2,

        /// <summary>
        /// The general inside domain: x+,y-
        /// </summary>
        generalInsideDomain = 4,

        /// <summary>
        /// The general for web: x+,y+
        /// </summary>
        generalOutsideDomain = 8,

        generalForDomain = 5,

        insideDomain = 3,

        outsideDomain = 10,

        generalForWeb = 12,

        /// <summary>
        /// The totally general for domain: x=1
        /// </summary>
        totallyGeneralForDomain = 16,

        /// <summary>
        /// The totally particular for domain: x=0
        /// </summary>
        totallyParticularForDomain = 32,

        /// <summary>
        /// The totally general outside domain: y=1
        /// </summary>
        totallyGeneralOutsideDomain = 64,

        /// <summary>
        /// The totally particular outside domain: y=0
        /// </summary>
        totallyParticularOutsideDomain = 128,
    }
}