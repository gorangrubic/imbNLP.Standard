// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineSiteTaskBuilderNode.cs" company="imbVeles" >
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
using System;
using System.Collections.Generic;

namespace imbNLP.PartOfSpeech.pipeline.mcRepoNodes
{
    using imbMiningContext.MCDocumentStructure;
    using imbMiningContext.MCRepository;
    using imbMiningContext.MCWebPage;
    using imbNLP.PartOfSpeech.decomposing.html;
    using imbNLP.PartOfSpeech.pipeline.core;
    using imbNLP.PartOfSpeech.pipeline.machine;
    using imbNLP.PartOfSpeech.pipelineForPos.subject;
    using imbSCI.Data.data.sample;

    /// <summary>
    /// Task builder node. If the task is not for it, it will forward it to <see cref="IPipelineNode.next"/>,
    /// </summary>
    /// <remarks>
    /// <para>if task is processed and new tasks were fed into <see cref="pipelineModelExecutionContext"/> it will forward the processed task to the <see cref="IPipelineNode.forward"/></para>
    /// </remarks>
    public class pipelineSiteTaskBuilderNode : pipelineNodeRegular<pipelineTaskMCSiteSubject>
    {
        protected Boolean doSortPagesByTextSize { get; set; } = false;
        protected Boolean doFilterOutDuplicates { get; set; } = true;

        protected samplingSettings takeSetup { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="pipelineSiteTaskBuilderNode"/> class.
        /// </summary>
        public pipelineSiteTaskBuilderNode(samplingSettings __takeSetup, Boolean _doSortPagesByTextSize, Boolean _doFilterOutDuplicates)
        {
            _nodeType = pipelineNodeTypeEnum.taskBuilder;
            doFilterOutDuplicates = _doFilterOutDuplicates;
            doSortPagesByTextSize = _doSortPagesByTextSize;
            takeSetup = __takeSetup;
            SetLabel();
        }

        public int SortByPageSize(imbMCWebPage page1, imbMCWebPage page2)
        {
            return page1.TextContent.Length.CompareTo(page2.TextContent.Length);
        }

        /// <summary>
        /// Task builder for <see cref="imbMCRepository"/> level of subject. Sends to next if task is not with <see cref="pipelineTaskMCRepoSubject"/>
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        public override IPipelineNode process(IPipelineTask task)
        {
            var realTask = task as pipelineTask<pipelineTaskMCSiteSubject>;
            if (realTask == null) return next;

            pipelineTaskMCSiteSubject realSubject = realTask.subject;

            var repoSubject = realSubject.parent as pipelineTaskMCRepoSubject;

            var repo = repoSubject.MCRepo;

            if (repo == null)
            {
                task.context.logger.log("MCRepo is null at [" + task.GetStringInfo() + "]");
            }

            List<imbMCWebPage> listPages = repo.GetAllWebPages(realSubject.MCSite, null, takeSetup);

            if (doFilterOutDuplicates) listPages = listPages.GetUniquePages();

            if (doSortPagesByTextSize) listPages.Sort(SortByPageSize);

            foreach (imbMCWebPage page in listPages)
            {
                var mCPageSubject = new pipelineTaskMCPageSubject();

                imbMCDocument doc = new imbMCDocument();
                doc.webPage = page;
                realSubject.mcElement.Add(doc);

                mCPageSubject.mcElement = doc;
                mCPageSubject.MCPage = page;
                // mCPageSubject.name = page.entry.HashCode;
                mCPageSubject.parent = realSubject;

                realSubject.Add(mCPageSubject);

                pipelineTask<pipelineTaskMCPageSubject> taskForPage = new pipelineTask<pipelineTaskMCPageSubject>(mCPageSubject);

                task.context.scheduledTasks.Push(taskForPage);
            }

            return forward;
        }
    }
}