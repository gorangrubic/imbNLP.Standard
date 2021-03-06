// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineStreamTaskBuilderNode.cs" company="imbVeles" >
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
using System.Collections.Generic;

namespace imbNLP.PartOfSpeech.pipeline.mcRepoNodes
{
    using imbMiningContext.MCDocumentStructure;
    using imbNLP.PartOfSpeech.decomposing.token;
    using imbNLP.PartOfSpeech.pipeline.core;
    using imbNLP.PartOfSpeech.pipeline.machine;
    using imbNLP.PartOfSpeech.pipelineForPos.subject;

    /// <summary>
    /// Pipeline transformer node
    /// </summary>
    /// <seealso cref="imbNLP.PartOfSpeech.pipeline.core.pipelineNodeRegular{imbNLP.PartOfSpeech.pipeline.machine.pipelineTaskSubjectContentToken}" />
    public class pipelineStreamTaskBuilderNode : pipelineNodeRegular<pipelineTaskSubjectContentToken>
    {
        public ITokenComposer tokenComposer { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="pipelineStreamTaskBuilderNode"/> class.
        /// </summary>
        public pipelineStreamTaskBuilderNode(ITokenComposer composer)
        {
            _nodeType = pipelineNodeTypeEnum.taskBuilder;
            tokenComposer = composer;
            SetLabel();
        }

        /// <summary>
        /// Processes the specified task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        public override IPipelineNode process(IPipelineTask task)
        {
            pipelineTask<pipelineTaskSubjectContentToken> realTask = task as pipelineTask<pipelineTaskSubjectContentToken>;

            if (realTask == null)
            {
                return next;
            }

            pipelineTaskSubjectContentToken realSubject = realTask.subject;

            if (realSubject.contentLevelType != flags.token.cnt_level.mcTokenStream)
            {
                return next;
            }

            List<imbMCToken> mcTokens = tokenComposer.process(realSubject.mcElement as imbMCStream);

            foreach (imbMCToken token in mcTokens)
            {
                pipelineTaskSubjectContentToken tokenSubject = new pipelineTaskSubjectContentToken();
                tokenSubject.mcElement = token;

                realSubject.mcElement.Add(token);

                tokenSubject.name = token.name;
                tokenSubject.contentLevelType = flags.token.cnt_level.mcToken;
                tokenSubject.parent = realSubject;
                tokenSubject.currentForm = token.content;

                realSubject.Add(tokenSubject);

                pipelineTask<pipelineTaskSubjectContentToken> newTask = new pipelineTask<pipelineTaskSubjectContentToken>(tokenSubject);

                task.context.scheduledTasks.Push(newTask);
            }

            // <---- tagging code

            return forward;
        }
    }
}