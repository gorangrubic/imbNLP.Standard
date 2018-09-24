// --------------------------------------------------------------------------------------------------------------------
// <copyright file="webLemmaTerm.cs" company="imbVeles" >
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
using imbNLP.PartOfSpeech.TFModels.vectorSpace;
using imbSCI.Core.attributes;
using System.ComponentModel;
using System.Linq;

namespace imbNLP.PartOfSpeech.TFModels.webLemma.table
{
    /// <summary>
    /// Lemma term - atomic unit of tokenized content
    /// </summary>
    /// <seealso cref="imbMiningContext.TFModels.core.termLemmaBase" />
    public class webLemmaTerm : termLemmaBase, ISpaceModelEntity
    {
        /// <summary>
        /// List of additional relationships
        /// </summary>
        /// <value>
        /// The relation list.
        /// </value>
        public spaceModelRelationList relationList { get; set; } = new spaceModelRelationList();

        /// <summary>
        /// Gets a clone with same absolute frequencies, nominalForm, and other forms
        /// </summary>
        /// <returns></returns>
        public webLemmaTerm GetAbsoluteClone()
        {
            webLemmaTerm output = new webLemmaTerm();
            output.nominalForm = nominalForm;
            output.otherForms = otherForms;
            output.otherForms = otherForms.ToList();
            output.AFreqPoints = AFreqPoints;
            output.documentFrequency = documentFrequency;
            output.documentSetFrequency = documentSetFrequency;
            return output;
        }

        /// <summary>
        /// Sums absolute values (abs. frequency, document frequency and document set frequency)
        /// </summary>
        /// <param name="b">The b.</param>
        public void AddAbsoluteValues(webLemmaTerm b)
        {
            AFreqPoints += b.AFreqPoints;
            documentFrequency += b.documentFrequency;
            documentSetFrequency += b.documentSetFrequency;
        }

        /// <summary> Ratio </summary>
        [Category("Frequencies")]
        [DisplayName("Document Set Freq")]
        [imb(imbAttributeName.measure_letter, "DSF")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Number of Document Sets (web sites) containing the lemma form")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double documentSetFrequency { get; set; } = 0;

        [Category("Frequencies")]
        [DisplayName("Document Freq")]
        [imb(imbAttributeName.measure_letter, "DF")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Number of Documents (web pages) containing the lemma form")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double documentFrequency { get; set; } = 0;

        [Category("Frequencies")]
        [DisplayName("Term Frequency")]
        [imb(imbAttributeName.measure_letter, "TF")]
        [imb(imbAttributeName.measure_setUnit, "w")]
        [imb(imbAttributeName.reporting_valueformat, "F5")]
        [Description("Normalized weight of the lemma form")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_escapeoff)]
        public double termFrequency { get; set; } = 0;

        /// <summary>
        /// IDF factor for weight
        /// </summary>
        /// <value>
        /// The factor.
        /// </value>
        [Category("Factors")]
        [DisplayName("IDF factor")]
        [imb(imbAttributeName.measure_letter, "IDF")]
        [imb(imbAttributeName.measure_setUnit, "r")]
        [Description("The IDF factor applied to normalized lemma form frequency")]
        [imb(imbAttributeName.reporting_valueformat, "F5")]
        public double documentFactor { get; set; } = 0;

        public webLemmaTerm()
        {
        }
    }
}