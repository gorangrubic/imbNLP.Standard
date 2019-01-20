using imbNLP.Toolkit.Documents.Ranking.Data;
using imbSCI.Core.enums;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Documents.Ranking
{


    public static class DocumentRankingScoreFusion
    {
        public static int Compare(List<DocumentSelectResultEntry> x, List<DocumentSelectResultEntry> y)
        {

            Int32 l = Math.Min(x.Count, y.Count);

            Int32 comparison = 0;

            for (int i = 0; i < l; i++)
            {
                if (x[i].score == y[i].score)
                {
                    comparison = 0;
                }
                else if (x[i].score > y[i].score)
                {
                    comparison = 1;
                }
                else
                {

                    comparison = -1;
                }
                if (comparison != 0)
                {
                    break;
                }
            }

            return comparison;

        }

        public static int Compare(KeyValuePair<string, List<DocumentSelectResultEntry>> x, KeyValuePair<string, List<DocumentSelectResultEntry>> y)
        {

            Int32 l = Math.Min(x.Value.Count, y.Value.Count);

            Int32 comparison = 0;

            for (int i = 0; i < l; i++)
            {
                if (x.Value[i].score == y.Value[i].score)
                {
                    comparison = 0;
                }
                else if (x.Value[i].score > y.Value[i].score)
                {
                    comparison = 1;
                }
                else
                {

                    comparison = -1;
                }
                if (comparison != 0)
                {
                    break;
                }
            }

            return comparison;

        }



        /// <summary>
        /// Sorts the entries comparing one by one score
        /// </summary>
        /// <param name="scoreSet">The score set.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public static List<DocumentSelectResultEntry> rankFusion(this IEnumerable<IEnumerable<DocumentSelectResultEntry>> scoreSet, ILogBuilder log)
        {
            Dictionary<string, List<DocumentSelectResultEntry>> aligned = scoreSet.GetAlignedByAssignedID(log);

            var list = aligned.ToList();

            list.Sort(Compare);
            list.Reverse();

            List<DocumentSelectResultEntry> output = new List<DocumentSelectResultEntry>();

            foreach (KeyValuePair<string, List<DocumentSelectResultEntry>> p in list)
            {
                output.Add(p.Value.First());
            }

            return output;

        }

        /// <summary>
        /// Fusions the specified operation.
        /// </summary>
        /// <param name="scoreSet">The score set.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="doRankingFusion">if set to <c>true</c> [do ranking fusion].</param>
        /// <param name="doDomainNormalization">if set to <c>true</c> [do domain normalization].</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public static DocumentSelectResult Fusion(this IEnumerable<DocumentSelectResult> scoreSet, operation operation, Boolean doRankingFusion, Boolean doDomainNormalization, ILogBuilder log)
        {
            List<DocumentSelectResultEntry> fusioned = null;


            if (doDomainNormalization)
            {
                log.log("Performing domain-level normalization over [" + scoreSet.Count() + "] document score sets");
                foreach (DocumentSelectResult set in scoreSet)
                {
                    DocumentRankingExtensions.NormalizeWithinDomain(set.items, log);
                }

            }



            if (doRankingFusion)
            {
                log.log("Ranking fusion over [" + scoreSet.Count() + "] document score sets");

                fusioned = rankFusion(scoreSet.Select(x => x.items), log);
            }
            else
            {
                log.log("Score fusion over [" + scoreSet.Count() + "] document score sets");

                fusioned = ScoreFusion(scoreSet.Select(x => x.items), operation, log);
            }


            DocumentSelectResult output = new DocumentSelectResult(); // scoreSet.First();

            output.name = "ScoreFusionBy" + operation.ToString();
            output.description = "Sets fusioned: ";

            foreach (var s in scoreSet)
            {
                output.description = s.name + " ";
            }

            output.items.AddRange(fusioned);


            return output;
        }

        public static List<DocumentSelectResultEntry> ScoreFusion(this IEnumerable<IEnumerable<DocumentSelectResultEntry>> scoreSet, operation operation, ILogBuilder log)
        {

            Dictionary<string, List<DocumentSelectResultEntry>> aligned = scoreSet.GetAlignedByAssignedID(log);
            List<DocumentSelectResultEntry> output = new List<DocumentSelectResultEntry>();

            //List<DocumentSelectResultEntry> output = new List<DocumentSelectResultEntry>();

            foreach (var pair in aligned)
            {
                var entry = pair.Value.First();
                //output.Add(pair.Key, entry);

                entry.score = pair.Value.Select(x => x.score).ToArray().CompressNumericVector(operation);

                output.Add(entry);

            }

            return output;
        }


    }
}
