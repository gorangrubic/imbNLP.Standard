// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineModelForContentToken.cs" company="imbVeles" >
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
using imbNLP.PartOfSpeech.pipeline.machine;
using imbNLP.PartOfSpeech.pipelineForPos.subject;
using System;
using System.Collections.Generic;

namespace imbNLP.PartOfSpeech.pipeline.core
{
    /// <summary>
    /// Base class for dynamically built models and prebuilt models
    /// </summary>
    /// <seealso cref="imbNLP.PartOfSpeech.pipeline.core.pipelineModel{imbNLP.PartOfSpeech.pipeline.machine.pipelineTaskSubjectContentToken}" />
    public class pipelineModelForContentToken : pipelineModel<pipelineTaskSubjectContentToken>
    {
        public override void constructionProcess()
        {
            // <--- no construction process
        }

        public override List<IPipelineTask> createPrimaryTasks(object[] resources)
        {
            var output = new List<IPipelineTask>();
            // <--------- creation of the primary tasks

            return output;
        }

        public pipelineModelForContentToken(String modelName) : base(null, modelName)
        {
            Label = "CP Pipeline Model";
        }
    }
}