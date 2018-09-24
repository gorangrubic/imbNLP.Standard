// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineHTMLTagDetection.cs" company="imbVeles" >
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

namespace imbNLP.PartOfSpeech.pipelineForPos.node
{
    using HtmlAgilityPack;
    using imbNLP.PartOfSpeech.decomposing.html;
    using imbNLP.PartOfSpeech.flags.token;
    using imbNLP.PartOfSpeech.pipeline.core;
    using imbNLP.PartOfSpeech.pipeline.machine;
    using imbNLP.PartOfSpeech.pipelineForPos.subject;
    using imbSCI.Core.extensions.data;

    /// <summary>
    /// Pipeline transformer node
    /// </summary>
    /// <seealso cref="imbNLP.PartOfSpeech.pipeline.core.pipelineNodeRegular{imbNLP.PartOfSpeech.pipeline.machine.pipelineTaskSubjectContentToken}" />
    public class pipelineHTMLTagDetection : pipelineNodeRegular<pipelineTaskSubjectContentToken>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="pipelineHTMLTagDetection"/> class.
        /// </summary>
        public pipelineHTMLTagDetection()
        {
            _nodeType = pipelineNodeTypeEnum.transformer;
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
            if (realTask == null) return next;

            pipelineTaskSubjectContentToken realSubject = realTask.subject;

            if (realSubject.mcElement != null)
            {
                HtmlNode node = realSubject.mcElement.htmlNode;

                List<String> tags = new List<string>();

                if (node != null)
                {
                    tags = node.GetTagNames();

                    foreach (var tag in tags)
                    {
                        switch (tag)
                        {
                            case "a":
                                realSubject.flagBag.AddUnique(cnt_containerType.link);
                                break;

                            case "title":
                                realSubject.flagBag.AddUnique(cnt_containerType.title);
                                break;

                            case "h":
                            case "h1":
                            case "h2":
                            case "h3":
                            case "h4":
                            case "h5":
                            case "h6":
                                realSubject.flagBag.AddUnique(cnt_containerType.title);
                                break;
                        }
                    }
                }
            }

            // <---- tagging code

            return forward;
        }
    }
}