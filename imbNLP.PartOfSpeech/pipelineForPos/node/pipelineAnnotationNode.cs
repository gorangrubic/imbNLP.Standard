// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineAnnotationNode.cs" company="imbVeles" >
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

namespace imbNLP.PartOfSpeech.pipelineForPos.node
{
    using imbNLP.PartOfSpeech.pipeline.core;
    using imbNLP.PartOfSpeech.pipeline.machine;
    using imbNLP.PartOfSpeech.pipelineForPos.subject;
    using imbNLP.PartOfSpeech.resourceProviders.core;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.reporting;

    /// <summary>
    /// Annotation pipeline node
    /// </summary>
    /// <seealso cref="imbNLP.PartOfSpeech.pipeline.core.pipelineNodeRegular{imbNLP.PartOfSpeech.pipeline.machine.pipelineTaskSubjectContentToken}" />
    public class pipelineTableAnnotationNode : pipelineNodeRegular<pipelineTaskSubjectContentToken>
    {
        protected tableAnnotationResolver resolver { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="pipelineTableAnnotationNode" /> class.
        /// </summary>
        /// <param name="resourcePath">The resource path.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="hostType">Type of the host.</param>
        public pipelineTableAnnotationNode(String resourcePath, ILogBuilder logger = null, Type hostType = null)
        {
            _nodeType = pipelineNodeTypeEnum.transformer;

            resolver = new tableAnnotationResolver(resourcePath, logger, hostType);
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

            // <---- tagging code

            var flags = resolver.process(realSubject.currentForm);
            realSubject.flagBag.AddRange(flags, true);

            return forward;
        }
    }
}