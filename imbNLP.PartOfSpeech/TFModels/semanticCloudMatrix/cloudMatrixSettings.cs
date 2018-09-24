// --------------------------------------------------------------------------------------------------------------------
// <copyright file="cloudMatrixSettings.cs" company="imbVeles" >
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
// using imbNLP.PartOfSpeech.TFModels.semanticCloud.core;
using imbSCI.Core.attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace imbNLP.PartOfSpeech.TFModels.semanticCloudMatrix
{
    /// <summary>
    /// Settings for semantic cloud transformation
    /// </summary>
    public class cloudMatrixSettings
    {
        public cloudMatrixSettings()
        {
        }

        public List<string> DescribeSelf()
        {
            List<String> output = new List<string>();

            output.Add("### Semantic Cloud Lemma Frequency Matrix -- isActive[" + isActive + "]");

            output.Add(" > > Matrix of common lemmas in the DocumentSetClass semantic clouds");
            output.Add(" > > The Matrix is used for increase of constrasts between DocumentSetClass semantic clouds");

            if (doDivideWeightWithCloudFrequency)
            {
                if (doUseSquareFunctionOfCF)
                {
                    output.Add(" > Lemma weight is divided by square power of its cloud frequency (i.e. number of clouds it was detected in).");
                }
                else
                {
                    output.Add(" > Lemma weight is divided by its cloud frequency (i.e. number of clouds it was detected in).");
                }
            }

            if (doCutOffByCloudFrequency)
            {
                output.Add(" > Cut off low pass filter is applied:");
                if (isFilterInAdaptiveMode)
                {
                    output.Add(" > > In adaptive mode - all lemmas with cloud frequency above [(min(CF)-1) + " + lowPassFilter + "] are removed from all clouds");
                }
                else
                {
                    output.Add(" > > In adaptive mode - all lemmas with cloud frequency above [" + lowPassFilter + "] are removed from all clouds");
                }

                if (doAssignMicroWeightInsteadOfRemoval)
                {
                    output.Add(" > > > Filter behaviour is changed (by doAssignMicroWeightInsteadOfRemoval) to assign micro-weight filter limit instead of Term removal.");
                    output.Add(" > > > This option will keep, otherwise removed, terms in the cloud but minimize their direct effect in the similarity calculation.");
                    output.Add(" > > > Although, their presence in the cloud allows greater term expansion, once the cloud is used.");
                }
            }

            if (doRemoveAnyRepeatingPrimaryTerm)
            {
                output.Add(" > > It will remove any primary term found to have CF above 0");
            }

            if (doDemoteAnyRepeatingSecondaryTerm)
            {
                output.Add(" > > It will demote any secondary term found to have CF above 0");
            }

            output.Add(" > Micro-weight high-pass filter set to: " + microWeightNoiseGate.ToString("F8"));

            return output;
        }

        /// <summary> If true it will remove any primary term that is found to repeat among clouds </summary>
        [Category("Switch")]
        [DisplayName("doRemoveAnyRepeatingPrimaryTerm")]
        [Description("If true it will remove any primary term that is found to repeat among clouds")]
        [XmlAttribute]
        public Boolean doRemoveAnyRepeatingPrimaryTerm { get; set; } = true;

        [XmlAttribute]
        public Boolean doDemoteAnyRepeatingPrimaryTerm { get; set; } = false;

        /// <summary> If true it will demote secondary term to reserve if found repeating </summary>
        [Category("Switch")]
        [DisplayName("doDemoteAnyRepeatingSecondaryTerm")]
        [Description("If true it will demote secondary term to reserve if found repeating")]
        [XmlAttribute]
        public Boolean doDemoteAnyRepeatingSecondaryTerm { get; set; } = true;

        /// <summary>
        /// It will pass only terms with less then specified number of clouds found. Max. possible overlap is: n-1, where n is size of square matrix
        /// </summary>
        public Int32 lowPassFilter { get; set; } = 2;

        /// <summary> If <c>true</c> it will use the semantic cloud matrix for noise removal </summary>
        [Category("Flag")]
        [DisplayName("isActive")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will use the semantic cloud matrix for noise removal")]
        [XmlAttribute]
        public Boolean isActive { get; set; } = false;

        /// <summary> If true it divide each term weight with number of common clouds being detected in </summary>
        [Category("Switch")]
        [DisplayName("doDivideWeightWithCloudFrequency")]
        [Description("If true it divide each term weight with number of common clouds being detected in")]
        public Boolean doDivideWeightWithCloudFrequency { get; set; } = true;

        /// <summary> If true it will divide weight with square power of cloud frequency </summary>
        [Category("Switch")]
        [DisplayName("doUseSquareFunctionOfCF")]
        [Description("If true it will divide weight with square power of cloud frequency")]
        public Boolean doUseSquareFunctionOfCF { get; set; } = true;

        /// <summary> If true it will remove terms having cloud frequency above specified lowPassFilter value </summary>
        [Category("Switch")]
        [DisplayName("doCutOffByCloudFrequency")]
        [Description("If true it will remove terms having cloud frequency above specified lowPassFilter value")]
        [XmlAttribute]
        public Boolean doCutOffByCloudFrequency { get; set; } = true;

        /// <summary> If true it will automatically adjust lowPassFilter value according to the top edge of cloud frequencies </summary>
        [Category("Switch")]
        [DisplayName("isFilterInAdaptiveMode")]
        [Description("If true it will automatically adjust lowPassFilter value according to the top edge of cloud frequencies")]
        [XmlAttribute]
        public Boolean isFilterInAdaptiveMode { get; set; } = true;

        /// <summary> If true it assign the microWeightNoiseGate value to term, set to be removed by the filter (this prevents the removal) </summary>
        [Category("Switch")]
        [DisplayName("doAssignMicroWeightInsteadOfRemoval")]
        [Description("If true it assign the microWeightNoiseGate value to term, set to be removed by the filter (this prevents the removal)")]
        public Boolean doAssignMicroWeightInsteadOfRemoval { get; set; } = true;

        /// <summary> Weight lower limit for term removal </summary>
        [Category("Ratio")]
        [DisplayName("microWeightNoiseGate")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "")]
        [Description("Weight lower limit for term removal")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        [XmlAttribute]
        public Double microWeightNoiseGate { get; set; } = 0.000001;
    }
}