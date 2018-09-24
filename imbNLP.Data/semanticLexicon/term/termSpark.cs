// --------------------------------------------------------------------------------------------------------------------
// <copyright file="termSpark.cs" company="imbVeles" >
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
    using imbNLP.Data.semanticLexicon.core;
    using imbSCI.Core.enums;
    using imbSCI.Core.extensions.io;
    using imbSCI.DataComplex;
    using imbSCI.DataComplex.extensions.data.modify;
    using imbSCI.DataComplex.extensions.data.schema;
    using System;
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// A term in the query
    /// </summary>
    /// <seealso cref="aceCommonTypes.collection.tf_idf.weightTable" />
    /// <seealso cref="imbSCI.DataComplex.IWeightTableTerm" />
    public class termSpark : weightTable<termSparkArm>, IWeightTableTerm, IEquatable<termSpark>
    {
        public termSpark()
        {
        }

        /// <summary>
        /// Constructs unexpanded, single term spark
        /// </summary>
        /// <param name="term">The term.</param>
        public termSpark(string term)
        {
            lemma = new TempLemma(term);
            Add(new termSparkArm(term));
        }

        public void Define(string __name, string __nominalForm)
        {
            name = __name;
            // nominalForm = __nominalForm;
        }

        public termSpark(IWeightTableSet __parent, string __name) : base(__parent, __name)
        {
            lemma = new TempLemma(__name);
            parent = __parent;
        }

        public override string name
        {
            get
            {
                if (lemma == null) return base.name;

                return lemma.name; // + "_" + lemma.type;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public ITermLemma lemma { get; set; }

        /// <summary>
        /// Automatically calculated as cumulative weight
        /// </summary>
        public double weight { get; set; } = 1;

        /// <summary>
        ///
        /// </summary>
        public int AFreqPoints { get; set; } = 1;

        public override bool termSingleAddAllowed
        {
            get
            {
                return true;
            }
        }

        public override string ToString()
        {
            return nominalForm;
        }

        public string nominalForm
        {
            get
            {
                if (lemma != null)
                {
                    return lemma.name;
                }
                else
                {
                    return base.name;
                }
            }
        }

        int IWeightTableTerm.Count
        {
            get
            {
                return Count();
            }
        }

        public override DataRow buildTableRow(DataRow dr, termSparkArm t)
        {
            dr.SetData(termTableColumns.termName, t.name);
            dr.SetData(termTableColumns.termLemma, t?.lexItem?.name);
            //dr.SetData(termTableColumns.freqAbs, termsAFreq[t.name]);
            //dr.SetData(termTableColumns.freqNorm, GetNFreq(t.name));
            //dr.SetData(termTableColumns.idf, GetIDF(t.name));
            //dr.SetData(termTableColumns.tf_idf, GetTF_IDF(t.name));
            //dr.SetData(termTableColumns.df, GetBDFreq(t.name));
            dr.SetData(termTableColumns.semanticDistance, GetWeight(t.name));
            dr.SetData(termTableColumns.normalizedSemanticDistance, GetNWeight(t.name));
            return dr;
        }

        public override DataTable buildTableShema(DataTable output)
        {
            output.Add(termTableColumns.termName, "Dictionary form of the term", "T_nd", typeof(string), dataPointImportance.normal);
            output.Add(termTableColumns.termLemma, "Normal form", "T_n", typeof(string), dataPointImportance.normal);
            //output.Add(termTableColumns.freqAbs, "Absolute frequency - number of occurences", "T_af", typeof(Int32), dataPointImportance.normal, "Abs. freq.");
            //output.Add(termTableColumns.freqNorm, "Normalized frequency - abs. frequency divided by the maximum", "T_nf", typeof(Double), dataPointImportance.important, "#0.00000");
            //output.Add(termTableColumns.df, "Document frequency - number of documents containing the term", "T_df", typeof(Int32), dataPointImportance.normal);
            //output.Add(termTableColumns.idf, "Inverse document frequency - logaritmicly normalized T_df", "T_idf", typeof(Double), dataPointImportance.normal, "#0.00000");
            //output.Add(termTableColumns.tf_idf, "Term frequency Inverse document frequency - calculated as TF-IDF", "T_tf-idf", typeof(Double), dataPointImportance.important, "#0.00000");
            output.Add(termTableColumns.semanticDistance, "Semantic distance from spark central term and this term", "T_sr", typeof(double), dataPointImportance.normal, "#0.00000", "Distance (nominal)");
            output.Add(termTableColumns.normalizedSemanticDistance, "Normalized semantic relevance - divided by max(T_sr)", "T_nsr", typeof(double), dataPointImportance.important, "#0.00000", "Normalized semantic distance");
            return output;
        }

        public void SetOtherForms(IEnumerable<string> instances)
        {
            foreach (string inst in instances)
            {
                Add(new termSparkArm(inst));
            }
        }

        public bool Equals(termSpark other)
        {
            return (nominalForm == other.nominalForm);
        }

        public List<string> GetAllForms(bool includingNominalForm = true)
        {
            // List<String> output = new List<string>();

            var output = GetAllTermString();
            if (includingNominalForm) output.Add(nominalForm);
            return output;
        }

        internal DataTable GetDataTable()
        {
            throw new NotImplementedException();
        }
    }
}