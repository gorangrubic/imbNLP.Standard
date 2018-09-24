// --------------------------------------------------------------------------------------------------------------------
// <copyright file="itmConstructorSettings.cs" company="imbVeles" >
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
using imbSCI.Core.attributes;
using System;
using System.ComponentModel;

namespace imbNLP.PartOfSpeech.TFModels.industryLemma
{
    public class itmConstructorSettings
    {
        public itmConstructorSettings()
        {
        }

        /// <summary> Ratio </summary>
        [Category("Ratio")]
        [DisplayName("PrimaryTermFactor")]
        [imb(imbAttributeName.measure_letter, "T_p")]
        [Description("Ratio")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public Double PrimaryTermFactor { get; set; } = 1;

        /// <summary> Ratio </summary>
        [Category("Ratio")]
        [DisplayName("SecondaryTermFactor")]
        [imb(imbAttributeName.measure_letter, "T_s")]
        [imb(imbAttributeName.measure_setUnit, "")]
        [Description("Ratio")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public Double SecondaryTermFactor { get; set; } = 0.5;

        /// <summary> Ratio </summary>
        [Category("Ratio")]
        [DisplayName("ReserveTermFactor")]
        [imb(imbAttributeName.measure_letter, "T_r")]
        [imb(imbAttributeName.measure_setUnit, "")]
        [Description("Ratio")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public Double ReserveTermFactor { get; set; } = 0.25;

        /// <summary> Ratio </summary>
        [Category("Ratio")]
        [DisplayName("OtherTermFactor")]
        [imb(imbAttributeName.measure_letter, "T_o")]
        [imb(imbAttributeName.measure_setUnit, "")]
        [Description("Ratio")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public Double OtherTermFactor { get; set; } = 0.1;
    }
}