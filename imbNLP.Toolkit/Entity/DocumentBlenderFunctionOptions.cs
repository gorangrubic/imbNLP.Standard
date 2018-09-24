using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace imbNLP.Toolkit.Entity
{

    [Flags]
    public enum DocumentBlenderFunctionOptions
    {
        none = 0,

        /// <summary>
        /// an content part can be excluded or included, no fuzzy option
        /// </summary>
        binaryAggregation = 1,

        /// <summary>
        /// an content parts are associated with their specific weights
        /// </summary>
        weightedAggregation = 2,




        /// <summary>
        /// base content unit is page/document
        /// </summary>
        pageLevel = 16,
        /// <summary>
        /// base content unit is block of content
        /// </summary>
        blockLevel = 32,
        /// <summary>
        /// base content unit is sentence
        /// </summary>
        sentenceLevel = 64,

        /// <summary>
        /// Only content units that are DocumentSet-wide unique will be included in the blended output
        /// </summary>
        uniqueContentUnitsOnly = 256

    }

}