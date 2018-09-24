// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineMachineSettings.cs" company="imbVeles" >
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

namespace imbNLP.PartOfSpeech.pipeline.machine
{
    public class pipelineMachineSettings
    {
        public pipelineMachineSettings()
        {
        }

        /// <summary> If true it will do something </summary>
        [Category("Switch")]
        [DisplayName("doSharePipelineCollectionInTheSession")]
        [Description("If true it will do something")]
        public Boolean doSharePipelineCollectionInTheSession { get; set; } = true;

        /// <summary> number of parallel task to take and run in one iterations </summary>
        [Category("Count")]
        [DisplayName("TaskTake")]
        [imb(imbAttributeName.measure_letter, "TC")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("number of parallel task to take and run in one iterations")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public Int32 TaskTake { get; set; } = 1;

        /// <summary> Number of seconds for status report </summary>
        [Category("Count")]
        [DisplayName("StatusReportPeriod")]
        [imb(imbAttributeName.measure_letter, "R_t")]
        [imb(imbAttributeName.measure_setUnit, "s")]
        [Description("Number of seconds for status report")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public Int32 StatusReportPeriod { get; set; } = 60;

        /// <summary> Miliseconds for delay tick before status check  </summary>
        [Category("Count")]
        [DisplayName("TickForCheck")]
        [imb(imbAttributeName.measure_letter, "C_t")]
        [imb(imbAttributeName.measure_setUnit, "ms")]
        [Description("Miliseconds for delay tick before status check ")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public Int32 TickForCheck { get; set; } = 25;

        /// <summary> If true it will do something </summary>
        [Category("Switch")]
        [DisplayName("doUseParallelExecution")]
        [Description("If true it will do something")]
        public Boolean doUseParallelExecution { get; set; } = false;
    }
}