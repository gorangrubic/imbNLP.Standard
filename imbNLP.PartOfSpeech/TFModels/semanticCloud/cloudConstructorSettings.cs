// --------------------------------------------------------------------------------------------------------------------
// <copyright file="cloudConstructorSettings.cs" company="imbVeles" >
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
using imbNLP.PartOfSpeech.TFModels.semanticCloudWeaver;
using imbSCI.Core.attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

// // using imbMiningContext.TFModels.WLF_ISF;
// using imbNLP.PartOfSpeech.TFModels.semanticCloud.core;
using System.Xml.Serialization;

namespace imbNLP.PartOfSpeech.TFModels.semanticCloud
{
    /// <summary>
    /// Configuration parameters, driving <see cref="lemmaSemanticCloud" /> construction
    /// </summary>
    public class cloudConstructorSettings
    {
        public cloudConstructorSettings()
        {
        }

        public List<string> DescribeSelf()
        {
            List<String> output = new List<string>();

            output.Add("### Configuration of the Semantic Cloud construction");

            if (documentSetFreqLowLimit > 0)
            {
                output.Add(" > > Only chunks with DocumentSetFrequency above [" + documentSetFreqLowLimit + "] are selected to the Primary Chunks set.");
            }
            else
            {
                output.Add(" > > All extracted chunks are used in the process of term category detection.");
            }

            if (assignTermTableWeightToNode)
            {
                output.Add(" > > Term weight (at Semantic Distance = 1) is inherited from the TF-IDF weight of the Web Lemma table");
            }
            else
            {
                output.Add(" > > Term weight (at Semantic Distance = 1) is set to 1 for all nodes");
            }

            if (assignChunkTableWeightToLink)
            {
                output.Add(" > > Link weight is inherited from the TF-IDF weight of POS chunks containing linked terms");
                if (doSumExistingLinkWeights)
                {
                    output.Add("------- (resulting weight is sum of the TF-IDF weights)");
                }
            }
            else if (doAdjustLinkWeightByChunkSize)
            {
                output.Add(" > > Link weight is reduced for chunk having more then 2 tokens: w = 1 / (T_c-1)");
                if (doSumExistingLinkWeights)
                {
                    output.Add("------- (resulting weight for repeating pairs is sum of weights)");
                }
            }
            else
            {
                output.Add(" > > Link weight is set to 1 for all nodes");
            }

            output.Add(" > Algorithm variation used: " + algorithm.ToString());

            if (algorithm == cloudConstructorAlgorithm.complex)
            {
                output.Add(" > > The complex algorithm in first step detect the Primary and Secondary Terms");
                output.Add(" > > by counting term occurance in the selected set of the chunks (having DocumentSetFrequency > " + documentSetFreqLowLimit + ")");
                //output.Add(" > > The terms having frequency above 1 are Primary, others are Secondary.");

                if (doFactorToCaseClouds || doFactorToClassClouds)
                {
                    output.Add(" > > Primary term weight is multiplied by factor [" + PrimaryTermWeightFactor.ToString("F2") + "]");
                    output.Add(" > > Secondary term weight is multiplied by factor [" + SecondaryTermWeightFactor.ToString("F2") + "]");
                    output.Add(" > > Other (Reserve) term weight is multiplied by factor [" + ReserveTermWeightFactor.ToString("F2") + "]");

                    if (doFactorToCaseClouds)
                    {
                        output.Add(" > > the factors are applied to DocumentSetCase clouds.");
                    }
                    else
                    {
                        output.Add(" > > the factors are not applied to DocumentSetCase clouds.");
                    }

                    if (doFactorToClassClouds)
                    {
                        output.Add(" > > the factors are applied to DocumentSetClass clouds.");
                    }
                    else
                    {
                        output.Add(" > > the factors are not applied to DocumentSetClass clouds.");
                    }
                }
                else
                {
                    output.Add("Term weight per category factors are disabled - the weights are not affected by assigned Term Category");
                }

                if (termInChunkLowerLimit > 0)
                {
                    output.Add("Only terms apearing in more then [" + termInChunkLowerLimit + "] will be allowed into the cloud");
                }

                output.Add("> Lower target count for Primary Category set to [" + primaryTermLowTargetCount + "], the optimization procedure will try to reduce Primary Terms until the count is even or less then the target.");

                if (doReserveTermsForClass)
                {
                    output.Add(" > > Reserve terms are used in DocumentSetClass cloud construction.");
                }
                else
                {
                    output.Add(" > > Reserve terms are not used in DocumentSetClass cloud construction.");
                }
            }

            output.Add(cloudWeaver.ToString());

            return output;
        }

        public lemmaSemanticWeaver cloudWeaver { get; set; } = new lemmaSemanticWeaver();

        public cloudConstructorAlgorithm algorithm { get; set; } = cloudConstructorAlgorithm.complex;

        /// <summary> If <c>true</c> it will reduce link weight when chunk contains more then 2 tokens </summary>
        [Category("Flag")]
        [DisplayName("doAdjustLinkWeightByChunkSize")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will reduce link weight when chunk contains more then 2 tokens")]
        public Boolean doAdjustLinkWeightByChunkSize { get; set; } = true;

        /// <summary>
        /// Gets or sets the term in chunk lower limit.
        /// </summary>
        /// <value>
        /// The term in chunk lower limit.
        /// </value>
        public Int32 termInChunkLowerLimit { get; set; } = 0;

        [XmlAttribute]
        public Double PrimaryTermWeightFactor { get; set; } = 2;

        [XmlAttribute]
        public Double SecondaryTermWeightFactor { get; set; } = 1;

        [XmlAttribute]
        public Double ReserveTermWeightFactor { get; set; } = 0.5;

        /// <summary> Do apply factors to class clouds </summary>
        [Category("Switch")]
        [DisplayName("doFactorToClassClouds")]
        [Description("Do apply factors to class clouds")]
        [XmlAttribute]
        public Boolean doFactorToClassClouds { get; set; } = true;

        /// <summary> If true it will do something </summary>
        [Category("Switch")]
        [DisplayName("doFactorToCaseClouds")]
        [Description("If true it will do something")]
        [XmlAttribute]
        public Boolean doFactorToCaseClouds { get; set; } = false;

        /// <summary> If true it will build links to the reserve terms </summary>
        [Category("Switch")]
        [DisplayName("doReserveTermsForClass")]
        [Description("If true it will build links to the reserve terms")]
        [XmlAttribute]
        public Boolean doReserveTermsForClass { get; set; } = true;

        /// <summary>
        /// Minimal document set frequency required for chunk entry to be included in the semantic cloud
        /// </summary>
        /// <value>
        /// The document set freq low limit.
        /// </value>
        [XmlAttribute]
        public Int32 documentSetFreqLowLimit { get; set; } = 0;

        [XmlAttribute]
        public Boolean assignTermTableWeightToNode { get; set; } = true;

        [XmlAttribute]
        public Boolean assignChunkTableWeightToLink { get; set; } = false;

        /// <summary>
        /// In case the link already exists, it will summarize its weight with new link
        /// </summary>
        /// <value>
        ///   <c>true</c> if [do sum existing link weights]; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public Boolean doSumExistingLinkWeights { get; set; } = true;

        public int primaryTermOptimizationIterationLimit { get; set; } = 10;
        public int primaryTermLowTargetCount { get; set; } = 5;
    }
}