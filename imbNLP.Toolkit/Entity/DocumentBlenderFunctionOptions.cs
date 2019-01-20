using System;

namespace imbNLP.Toolkit.Entity
{

    /// <summary>
    /// Options that control how documents are blended into single web site representation
    /// </summary>
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


        //  keepPageSeparated = 4,


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


        siteLevel = 128,

        /// <summary>
        /// Only content units that are DocumentSet-wide unique will be included in the blended output
        /// </summary>
        uniqueContentUnitsOnly = 256,
        separatePages = 512,
        keepLayersInMemory = 1024,


        categoryLevel = 2048,

        datasetLevel = 4092,

        layerLevel = 8192
    }

}