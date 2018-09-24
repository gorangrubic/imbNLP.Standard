// --------------------------------------------------------------------------------------------------------------------
// <copyright file="wlfConstructorSettings.cs" company="imbVeles" >
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
using imbNLP.PartOfSpeech.flags.basic;
using imbSCI.Core.attributes;

// // using imbMiningContext.TFModels.WLF_ISF;
using imbSCI.Core.extensions.data;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace imbNLP.PartOfSpeech.TFModels.webLemma
{
    public class wlfConstructorSettings
    {
        public wlfConstructorSettings()
        {
            //  allowedLemmaTypes.AddUnique(pos_type.N);
            // allowedLemmaTypes.AddUnique(pos_type.A);
        }

        /// <summary>
        /// Creates multi-line description of current configuration
        /// </summary>
        /// <returns>List of description lines</returns>
        public List<string> DescribeSelf()
        {
            List<String> output = new List<string>();

            output.Add("### Configuration of the TF-IDF weight computation procedure");

            if (allowedLemmaTypes.Any())
            {
                String ln = "";
                allowedLemmaTypes.ForEach(x => ln = ln.add(x.ToString(), ","));
                output.Add(" > Only POS elements flagged with one of the following pos_types [" + ln + "] are allowed into TF-IDF table");
            }
            else
            {
                output.Add(" > POS elements of any pos_type are allowed into TF-IDF table");
            }

            output.Add(" > Initial TF-IDF weights for each POS element are adjusted by the following factors, depending on type of HTML node hosting the element:");
            output.Add(" > > Title and headings:            " + titleTextFactor.ToString("F2"));
            output.Add(" > > Link (anchor) text:            " + anchorTextFactor.ToString("F2"));
            output.Add(" > > Other textual content:         " + contentTextFactor.ToString("F2"));

            if (doUseIDF)
            {
                if (doUseNaturalLog)
                {
                    output.Add(" > IDF is computed using natural logaritam: Log_e.");
                }
                else
                {
                    output.Add(" > IDF is computed using logaritam of decade base - Log_10.");
                }

                if (documentFrequencyMaxFactor != 1)
                {
                    output.Add(" > Before TF-IDF weights computation, the Max. Document Frequency (used in the equation) is adjusted by factor (x): " + documentFrequencyMaxFactor.ToString("F2"));
                    if (documentFrequencyMaxCorrection > 0)
                    {
                        output.Add(" > and adjusted for the following value (+): " + documentFrequencyMaxFactor);
                    }
                }
                else if (documentFrequencyMaxCorrection > 0)
                {
                    output.Add(" > Before TF-IDF weights computation, the Max. Document Frequency (used in the equation) is adjusted for the following value (+): " + documentFrequencyMaxFactor.ToString("F2"));
                }
                else
                {
                    output.Add(" > Max. Document Frequency is used without adjustments.");
                }

                if (doAdjustIDFForCase)
                {
                    output.Add(" > The IDF computation adjustment (via Max. DF correction) is applied in both class and case TF-IDFs.");
                }
                else
                {
                    output.Add(" > The IDF computation adjustment (via Max. DF correction) is applied only to class TF-IDFs, not for cases.");
                }
            }
            else
            {
                output.Add(" ---- IDF is disabled in this configuration --- ");
            }

            if (doUseDocumentSet)
            {
                output.Add(" > Computed TF-IDF weights are adjusted by function of DocumentSetFrequency (number of web sites containing the term/element).");
            }
            else
            {
                output.Add(" > DocumentSetFrequency (number of web sites containing the term/element) is not used by this configuration.");
            }

            return output;
        }

        /// <summary> If <c>true</c> it will use documentSet frequency to boost weights of terms that are common to the complete category </summary>
        [Category("Flag")]
        [DisplayName("doUseDocumentSet")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will use documentSet frequency to boost weights of terms that are common to the complete category")]
        [XmlAttribute]
        public Boolean doUseDocumentSet { get; set; } = false;

        [XmlAttribute]
        public Boolean doComputeTFIDF { get; set; } = true;

        [XmlAttribute]
        public Boolean doRenormalizeWeights { get; set; } = false;

        [XmlAttribute]
        public Boolean doUseIDF { get; set; } = true;

        /// <summary>
        /// If true it will exclude any POS type that may be anything other then <see cref="allowedLemmaTypes"/>
        /// </summary>
        /// <value>
        ///   <c>true</c> if [strict position type policy]; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public Boolean strictPosTypePolicy { get; set; } = false;

        private List<pos_type> _allowedTypes = new List<pos_type>(); //

        private Boolean _allowedTypesChecked = false;

        public List<pos_type> allowedLemmaTypes
        {
            get
            {
                if (!_allowedTypesChecked)
                {
                    _allowedTypesChecked = true;
                    List<pos_type> tmp = new List<pos_type>();

                    _allowedTypes.ForEach(x => tmp.AddUnique(x));
                    _allowedTypes = tmp;
                }
                return _allowedTypes;
            }
            set
            {
                if (!_allowedTypesChecked)
                {
                    _allowedTypesChecked = true;
                    List<pos_type> tmp = new List<pos_type>();

                    value.ForEach(x => tmp.AddUnique(x));
                    _allowedTypes = tmp;
                }
                else
                {
                    _allowedTypes = value;
                }
            }
        }

        /// <summary> If true it will adjust IDF computation also for DocumentSetCase </summary>
        [Category("Switch")]
        [DisplayName("doAdjustIDFForCase")]
        [Description("If true it will adjust IDF computation also for DocumentSetCase")]
        [XmlAttribute]
        public Boolean doAdjustIDFForCase { get; set; } = true;

        /// <summary>
        /// Factor to multiply max. DF in IDF calculation
        /// </summary>
        /// <value>
        /// The document frequency maximum factor.
        /// </value>

        public Double documentFrequencyMaxFactor { get; set; } = 1.1;

        /// <summary>
        /// Absolute correction to max. df in IDF calculation
        /// </summary>
        /// <value>
        /// The document frequency maximum correction.
        /// </value>
        [XmlAttribute]
        public Int32 documentFrequencyMaxCorrection { get; set; } = 0;

        [XmlAttribute]
        public Double anchorTextFactor { get; set; } = 0.75;

        [XmlAttribute]
        public Double titleTextFactor { get; set; } = 1.0;

        [XmlAttribute]
        public Double contentTextFactor { get; set; } = 0.5;

        [XmlAttribute]
        public bool doUseNaturalLog { get; set; } = true;
    }
}