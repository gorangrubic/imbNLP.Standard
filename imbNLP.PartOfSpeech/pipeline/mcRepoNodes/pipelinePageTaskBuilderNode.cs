// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelinePageTaskBuilderNode.cs" company="imbVeles" >
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
using System.Linq;

namespace imbNLP.PartOfSpeech.pipeline.mcRepoNodes
{
    using HtmlAgilityPack;
    using imbMiningContext.MCDocumentStructure;
    using imbMiningContext.MCRepository;
    using imbNLP.PartOfSpeech.decomposing.block;
    using imbNLP.PartOfSpeech.pipeline.core;
    using imbNLP.PartOfSpeech.pipeline.machine;
    using imbNLP.PartOfSpeech.pipelineForPos.subject;

    /// <summary>
    /// Task builder node. If the task is not for it, it will forward it to <see cref="IPipelineNode.next"/>,
    /// </summary>
    /// <remarks>
    /// <para>if task is processed and new tasks were fed into <see cref="pipelineModelExecutionContext"/> it will forward the processed task to the <see cref="IPipelineNode.forward"/></para>
    /// </remarks>
    /// <seealso cref="imbNLP.PartOfSpeech.pipeline.core.pipelineNodeRegular{imbNLP.PartOfSpeech.pipeline.machine.pipelineTaskSubjectContentToken}" />
    public class pipelinePageTaskBuilderNode : pipelineNodeRegular<pipelineTaskMCRepoSubject>
    {
        protected IBlockComposer blockComposer { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="pipelinePageTaskBuilderNode"/> class.
        /// </summary>
        public pipelinePageTaskBuilderNode(IBlockComposer __composer)
        {
            _nodeType = pipelineNodeTypeEnum.taskBuilder;
            SetLabel();
            blockComposer = __composer;
        }

        /// <summary>
        /// Task builder for <see cref="imbMCRepository"/> level of subject. Sends to next if task is not with <see cref="pipelineTaskMCRepoSubject"/>
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        public override IPipelineNode process(IPipelineTask task)
        {
            pipelineTask<pipelineTaskMCPageSubject> realTask = task as pipelineTask<pipelineTaskMCPageSubject>;
            if (realTask == null) return next;

            pipelineTaskMCPageSubject realSubject = realTask.subject;

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(realSubject.MCPage.HtmlSourceCode);

            pipelineTaskMCSiteSubject siteSubject = realSubject.parent as pipelineTaskMCSiteSubject;

            realSubject.htmlDocument = html;

            List<imbMCBlock> blocks = blockComposer.process(html, realSubject.name);

            if (!blocks.Any())
            {
                task.context.logger.log("Block composer returned zero blocks for [" + siteSubject.name + "]");
            }

            foreach (imbMCBlock block in blocks)
            {
                pipelineTaskSubjectContentToken tokenSubject = new pipelineTaskSubjectContentToken();
                tokenSubject.name = block.name;
                tokenSubject.contentLevelType = flags.token.cnt_level.mcBlock;
                tokenSubject.mcElement = block;
                tokenSubject.currentForm = block.content;
                realSubject.mcElement.Add(tokenSubject.mcElement);
                realSubject.Add(tokenSubject);

                pipelineTask<pipelineTaskSubjectContentToken> taskForElement = new pipelineTask<pipelineTaskSubjectContentToken>(tokenSubject);

                task.context.scheduledTasks.Push(taskForElement);
            }

            return forward;
        }
    }
}