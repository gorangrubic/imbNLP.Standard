using System;
using System.Linq;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Documents.TextExtraction
{

    /// <summary>
    /// Kako se ponaša prema: div, span, li, ul, headings, td, tr
    /// </summary>
    public enum textExtraction_structure
    {
        ignore,

        /// <summary>
        /// prepiše vrednost i lupi enter
        /// </summary>
        normal,

        /// <summary>
        /// doda još jedan newLine gore i dole
        /// </summary>
        newLine,

        /// <summary>
        /// Ostane u liniji, samo doda space
        /// </summary>
        spaceInline,
    }

}