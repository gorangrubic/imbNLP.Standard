using System;

namespace imbNLP.Toolkit.Documents.Ranking.Core
{

    public enum GraphFactorAlgorithm
    {
        customFunction,
        PageRank,
        HITS
    }


    [Flags]
    public enum GraphFactorFunctionEnum
    {
        none = 0,
        count_inbound = 1,
        count_outbound = 2,
        /// <summary>
        /// Divides score by count of inbound and outbound links
        /// </summary>
        divide_by_linkCount = 4,
        divide_by_inbound = 8,
        divide_by_outbound = 16,
        divide_by_graphnodes = 32,
        divide_by_graphlinks = 64,


        count_in_and_out = count_inbound | count_outbound






    }
}