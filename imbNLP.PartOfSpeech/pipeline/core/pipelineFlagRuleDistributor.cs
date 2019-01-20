// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineFlagRuleDistributor.cs" company="imbVeles" >
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
using imbSCI.Core.extensions.data;
using imbSCI.Data;
using System;
using System.Collections.Generic;

namespace imbNLP.PartOfSpeech.pipeline.core
{
    /// <summary>
    /// Flag set rule based distributor
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="imbNLP.PartOfSpeech.pipeline.core.pipelineNode{T}" />
    public class pipelineFlagRuleDistributor<T> : pipelineNodeRegular<T> where T : class, IPipelineTaskSubject
    {
        public override pipelineNodeTypeEnum nodeType
        {
            get
            {
                return pipelineNodeTypeEnum.distributor;
            }
        }

        protected List<Object> flags { get; set; }
        protected containsQueryTypeEnum queryType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="pipelineFlagRuleDistributor{T}"/> class.
        /// </summary>
        /// <param name="_ruleType">Type of the rule.</param>
        /// <param name="_flags">The flags.</param>
        public pipelineFlagRuleDistributor(containsQueryTypeEnum _ruleType, Object[] _flags)
        {
            queryType = _ruleType;
            flags = _flags.toList();
        }

        public override IPipelineNode process(IPipelineTask task)
        {
            pipelineTask<T> realTask = task as pipelineTask<T>;

            if (realTask.subject.flagBag.ContainsByEnum(flags.ToArray(), queryType))
            {
                return forward;
            }

            return next;
        }
    }
}