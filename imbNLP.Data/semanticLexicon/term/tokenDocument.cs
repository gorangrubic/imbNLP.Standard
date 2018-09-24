// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tokenDocument.cs" company="imbVeles" >
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
// Project: imbNLP.Data
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
namespace imbNLP.Data.semanticLexicon.term
{
    using imbSCI.Core.enums;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.DataComplex;
    using imbSCI.DataComplex.extensions.data.modify;
    using imbSCI.DataComplex.extensions.data.schema;
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// Token Document - providing tf-idf without Lexicon
    /// </summary>
    /// <seealso cref="imbSCI.DataComplex.weightTable{TWeightTableTerm}.collection.tf_idf.weightTableGenericTerm}" />
    public class tokenDocument : weightTable<weightTableGenericTerm>
    {
        public override bool termSingleAddAllowed
        {
            get
            {
                return false;
            }
        }

        public void AddTokens(List<string> tokens, ILogBuilder response = null)
        {
            //            var sparks = tokens.getSparks(1, response);
            string number = "";
            foreach (string sp in tokens)
            {
                if (sp.isNumber())
                {
                    number = number.add(sp, "-");
                }
                else
                {
                    if (!number.isNullOrEmptyString())
                    {
                        Add((IWeightTableTerm)new weightTableGenericTerm(number, 1), -1);
                        number = "";
                    }
                    var gt = new weightTableGenericTerm(sp, 1);
                    Add((IWeightTableTerm)gt, -1);
                }
            }
        }

        public override DataRow buildTableRow(DataRow dr, weightTableGenericTerm t)
        {
            dr.SetData(termTableColumns.termName, t.name);
            dr.SetData(termTableColumns.freqAbs, termsAFreq[t.name]);
            dr.SetData(termTableColumns.freqNorm, GetNFreq(t.name));
            dr.SetData(termTableColumns.df, GetBDFreq(t.name));
            dr.SetData(termTableColumns.idf, GetIDF(t.name));
            dr.SetData(termTableColumns.tf_idf, GetTF_IDF(t.name));
            return dr;
        }

        public override DataTable buildTableShema(DataTable output)
        {
            output.Add(termTableColumns.termName, "Nominal form of the term", "T_n", typeof(string), dataPointImportance.normal);
            output.Add(termTableColumns.freqAbs, "Absolute frequency - number of occurences", "T_af", typeof(int), dataPointImportance.normal, "D", "Abs. freq.");
            output.Add(termTableColumns.freqNorm, "Normalized frequency - abs. frequency divided by the maximum", "T_nf", typeof(double), dataPointImportance.important, "#0.00000");
            output.Add(termTableColumns.df, "Document frequency - number of documents containing the term", "T_df", typeof(int), dataPointImportance.normal);
            output.Add(termTableColumns.idf, "Inverse document frequency - logaritmicly normalized T_df", "T_idf", typeof(double), dataPointImportance.normal, "#0.00000");
            output.Add(termTableColumns.tf_idf, "Term frequency Inverse document frequency - calculated as TF-IDF", "T_tf-idf", typeof(double), dataPointImportance.important, "#0.00000");
            return output;
        }
    }
}