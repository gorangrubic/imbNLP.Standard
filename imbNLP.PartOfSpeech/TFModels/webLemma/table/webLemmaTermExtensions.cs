// --------------------------------------------------------------------------------------------------------------------
// <copyright file="webLemmaTermExtensions.cs" company="imbVeles" >
//
// Copyright (C) 2018 imbVeles
//
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Project: imbNLP.PartOfSpeech
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
using imbMiningContext.TFModels.core;
using imbSCI.Core.extensions.table;
using imbSCI.Core.math;
using imbSCI.Core.reporting;
using imbSCI.DataComplex.extensions.data.operations;
using imbSCI.DataComplex.extensions.data.schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace imbNLP.PartOfSpeech.TFModels.webLemma.table
{
    /// <summary>
    /// Extension methods for <see cref="webLemmaTermTable"/>
    /// </summary>
    public static class webLemmaTermExtensions
    {
        /// <summary>
        /// Gets the lemmas, matching <c>queryTerms</c>, sorted by weight (in the <c>table</c>)
        /// </summary>
        /// <param name="table">webLemmaTermTable to be queried</param>
        /// <param name="queryTerms">The query terms, strings to query <c>table</c></param>
        /// <param name="takeTopN">Number of top ranked lemmas, by <see cref="termLemmaBase.weight"/>, descending.</param>
        /// <returns>List of matched webLemmaTerms</returns>
        public static List<String> GetLemmasInStringSorted(this webLemmaTermTable table, IEnumerable<String> queryTerms, Int32 takeTopN = -1)
        {
            var list = table.GetLemmasSorted(queryTerms, takeTopN);
            List<String> output = new List<string>();
            list.ForEach(x => output.Add(x.lemmaForm));
            return output;
        }

        /// <summary>
        /// Gets the lemmas, matching <c>queryTerms</c>, sorted by weight (in the <c>table</c>)
        /// </summary>
        /// <param name="table">webLemmaTermTable to be queried</param>
        /// <param name="queryTerms">The query terms, strings to query <c>table</c></param>
        /// <param name="takeTopN">Number of top ranked lemmas, by <see cref="termLemmaBase.weight"/>, descending.</param>
        /// <returns>List of matched webLemmaTerms</returns>
        public static List<webLemmaTerm> GetLemmasSorted(this webLemmaTermTable table, IEnumerable<String> queryTerms, Int32 takeTopN = -1)
        {
            // ------------ selection of key terms
            List<webLemmaTerm> terms = new List<webLemmaTerm>();
            foreach (String tkn in queryTerms)
            {
                var lm = table.ResolveLemmaForTerm(tkn);
                if (lm != null)
                {
                    terms.Add(lm);
                }
            }
            terms.Sort((x, y) => y.weight.CompareTo(x.weight));
            if (takeTopN == -1) takeTopN = terms.Count;
            var list = terms.Take(Math.Min(takeTopN, terms.Count)).ToList();
            return list;
        }

        /// <summary>
        /// Recomputes the term frequencies.
        /// </summary>
        /// <param name="lemmas">The lemmas.</param>
        /// <param name="logger">The logger.</param>
        public static void RecomputeTermFrequencies(this IEnumerable<webLemmaTerm> lemmas, ILogBuilder logger)
        {
            Int32 TFMax = lemmas.Max(x => x.AFreqPoints);

            foreach (var lemma in lemmas)
            {
                lemma.termFrequency = lemma.AFreqPoints.GetRatio(TFMax);
            }
        }

        public static Dictionary<String, webLemmaTerm> GetMergedLemmaDictionary(this List<webLemmaTermTable> tables, ILogBuilder logger)
        {
            Dictionary<String, webLemmaTerm> lemmaSummary = new Dictionary<string, webLemmaTerm>();

            foreach (webLemmaTermTable t in tables)
            {
                var lemmas = t.GetList();
                foreach (webLemmaTerm lemma in lemmas)
                {
                    if (lemmaSummary.ContainsKey(lemma.name))
                    {
                        lemmaSummary[lemma.name].AddAbsoluteValues(lemma);
                    }
                    else
                    {
                        lemmaSummary.Add(lemma.name, lemma.GetAbsoluteClone());
                    }
                }
            }

            lemmaSummary.Values.RecomputeTermFrequencies(logger);

            return lemmaSummary;
        }

        /// <summary>
        /// Merges web lemma term tables into single table that should be recomputed afterwards because only absolute values are set for the lemmas
        /// </summary>
        /// <param name="tables">The tables.</param>
        /// <param name="name">The name.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static webLemmaTermTable GetMergedLemmaTable(this List<webLemmaTermTable> tables, String name, ILogBuilder logger)
        {
            logger.log("Merging [" + tables.Count + "]");

            Dictionary<String, webLemmaTerm> lemmaSummary = GetMergedLemmaDictionary(tables, logger);

            webLemmaTermTable output = new webLemmaTermTable(name);

            foreach (var pair in lemmaSummary)
            {
                output.Add(pair.Value);
            }

            logger.log("Merged lemma table created [" + output.name + "] with [" + output.Count + "] entries.");

            return output;
        }

        /// <summary>
        /// Gets the data table sorted.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="limit">The limit.</param>
        /// <returns></returns>
        public static DataTable GetDataTableSorted(this webLemmaTermTable table, Int32 limit = -1)
        {
            DataTable wlt = table.GetDataTable();
            wlt.DefaultView.Sort = "termFrequency desc";
            var sorted = wlt.DefaultView.ToTable();

            DataTable elt = wlt.GetClonedShema<DataTable>(true);

            elt.CopyRowsFrom(sorted, 0, limit);

            if (limit > 0) elt.AddExtra("The report contains first [" + limit.ToString("D5") + "] rows");
            return elt;
        }

        public static Dictionary<String, webLemmaTerm> GetLemmaDictionary(this IEnumerable<webLemmaTerm> lemmas)
        {
            Dictionary<String, webLemmaTerm> output = new Dictionary<string, webLemmaTerm>();

            foreach (webLemmaTerm term in lemmas)
            {
                if (!output.ContainsKey(term.name))
                {
                    output.Add(term.name, term);
                }
            }

            return output;
        }
    }
}