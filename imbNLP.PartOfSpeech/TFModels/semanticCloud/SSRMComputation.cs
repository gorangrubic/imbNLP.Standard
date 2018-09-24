// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SSRMComputation.cs" company="imbVeles" >
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
using System;

// // using imbMiningContext.TFModels.WLF_ISF;
// using imbNLP.PartOfSpeech.TFModels.semanticCloud.core;
using System.Text;

namespace imbNLP.PartOfSpeech.TFModels.semanticCloud
{
    /// <summary>
    /// Diagnostic SSRM computation artifact
    /// </summary>
    public class SSRMComputation
    {
        public String GetFilename()
        {
            return "SSRM_" + query + "_" + document + ".txt";
        }

        public Double upper { get; set; } = 0;
        public Double lower { get; set; } = 0;
        public Double similarity { get; set; } = 0;

        public Int32 terms { get; set; } = 0;
        public StringBuilder sb { get; set; } = new StringBuilder();

        public String document { get; set; } = "";
        public String query { get; set; } = "";

        public SSRMComputation(String _document, String _query)
        {
            document = _document;
            query = _query;
            sb.AppendLine(String.Format("SSRM :: Sim(d, c) = d->[{0,20}]  t->[{1,20}]", document, query));
        }

        /// <summary>
        /// Prints the term.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="term">The term.</param>
        /// <param name="w_d">The w d.</param>
        /// <param name="w_t">The w t.</param>
        /// <param name="w_c">The w c.</param>
        /// <param name="up">Up.</param>
        /// <param name="lw">The lw.</param>
        public void printTerm(Int32 i, String term, Double w_d, Double w_t, Double w_c, Double up, Double lw)
        {
            sb.AppendLine(String.Format("{0,-5} : {1,-15} Wd[{2,7:F5}] Wt{3,7:F5} Wc{4,7:F5}  U{5,7:F5} L{6,7:F5}", i, term, w_d, w_t, w_c, up, lw));
        }

        public void printFinale()
        {
            sb.AppendLine(String.Format("Sim(d, c) = {0,-7:F5} / {1, -7:F5} = {2, -10:F5} (terms:{3,-5})", upper, lower, similarity, terms));
        }
    }
}