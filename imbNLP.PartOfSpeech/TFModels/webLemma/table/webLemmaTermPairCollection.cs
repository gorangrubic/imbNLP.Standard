// --------------------------------------------------------------------------------------------------------------------
// <copyright file="webLemmaTermPairCollection.cs" company="imbVeles" >
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
using imbSCI.Core.extensions.table;
using imbSCI.Core.math;
using imbSCI.Core.reporting;
using imbSCI.DataComplex.extensions.data.schema;
using System;
using System.Collections.Generic;
using System.Data;

namespace imbNLP.PartOfSpeech.TFModels.webLemma.table
{
    /// <summary>
    /// Set of matched <see cref="webLemmaTerm"/>s
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{imbMiningContext.TFModels.WLF_ISF.webLemmaTermPair}" />
    public class webLemmaTermPairCollection : List<webLemmaTermPair>
    {
        /// <summary>
        /// Gets the data table with complete pair collection
        /// </summary>
        /// <returns></returns>
        public DataTable GetDataTable()
        {
            DataTable output = new DataTable("MatchedTerms");

            output.Add("T", "Matched Term", "T", typeof(String), imbSCI.Core.enums.dataPointImportance.normal, "", "Term");
            output.Add("IDFq", "Term IDF at case/query", "IDF_qi", typeof(Double), imbSCI.Core.enums.dataPointImportance.normal, "F2", "IDF qi");
            output.Add("IDFi", "Term IDF at at class/document", "IDF_ci", typeof(Double), imbSCI.Core.enums.dataPointImportance.normal, "F2", "IDF ci");
            output.Add("Wqi", "Term weight at case/query", "W_qi", typeof(Double), imbSCI.Core.enums.dataPointImportance.normal, "F5", "Term W_qi");
            output.Add("Wci", "Term weight at class/document", "W_ci", typeof(Double), imbSCI.Core.enums.dataPointImportance.normal, "F5", "Term W_ci");
            output.Add("Pw", "Term pair weight-factor", "P_w", typeof(Double), imbSCI.Core.enums.dataPointImportance.normal, "F5", "Pair W");

            foreach (webLemmaTermPair pair in this)
            {
                var dr = output.NewRow();
                dr["T"] = pair.entryA.nominalForm;
                dr["IDFq"] = pair.entryA.documentFrequency;
                dr["IDFi"] = pair.entryA.documentFrequency;
                dr["Wqi"] = pair.entryA.weight;
                dr["Wci"] = pair.entryB.weight;
                dr["Pw"] = pair.factor;
                output.Rows.Add(dr);
            }

            output.AddExtra("Total pairs: " + this.Count);

            return output;
        }

        public void Add(webLemmaTerm entryA, webLemmaTerm entryB)
        {
            Add(new webLemmaTermPair(entryA, entryB));
        }

        public webLemmaTermPair GetPair(String nominalForm)
        {
            foreach (webLemmaTermPair pair in this)
            {
                if (pair.entryA.nominalForm == nominalForm)
                {
                    return pair;
                }
            }
            return null;
        }

        public void DeployFactors(List<webLemmaTermPairCollection> otherCrossSections)
        {
            foreach (webLemmaTermPair pair in this)
            {
                Int32 divisor = 1;

                foreach (webLemmaTermPairCollection crossSection in otherCrossSections)
                {
                    var p = crossSection.GetPair(pair.entryA.nominalForm);
                    if (p != null)
                    {
                        divisor++;
                    }
                }

                pair.factor = pair.factor / divisor;
                if (divisor == 2)
                {
                }
                if (divisor == 1)
                {
                }
            }
        }

        /// <summary>
        /// Gets the cosine similarity for contained pairs collection
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public double GetCosineSimilarity(ILogBuilder logger = null)
        {
            Double upper = 0;
            Double lowerA = 0;
            Double lowerB = 0;

            foreach (webLemmaTermPair pair in this)
            {
                upper += pair.entryA.weight * pair.entryB.weight * pair.factor;
                lowerA += pair.entryA.weight * pair.entryA.weight * pair.factor;
                lowerB += pair.entryB.weight * pair.entryB.weight * pair.factor;
            }

            Double output = upper.GetRatio(Math.Sqrt(lowerA * lowerB));

            if (output == 0)
            {
                logger.log("Cosine similarity returned 0 score!");
            }

            return output;
        }
    }
}