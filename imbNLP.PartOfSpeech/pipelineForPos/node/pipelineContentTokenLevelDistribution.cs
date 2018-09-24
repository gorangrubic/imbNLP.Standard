// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineContentTokenLevelDistribution.cs" company="imbVeles" >
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
namespace imbNLP.PartOfSpeech.pipelineForPos.node
{
    using imbNLP.PartOfSpeech.flags.token;
    using imbNLP.PartOfSpeech.pipeline.core;
    using imbNLP.PartOfSpeech.pipeline.machine;
    using imbNLP.PartOfSpeech.pipelineForPos.subject;

    /// <summary>
    /// Distributes the task according to <see cref="pipelineTaskSubjectContentToken.contentLevelType"/>
    /// </summary>
    /// <seealso cref="imbNLP.PartOfSpeech.pipeline.core.pipelineNodeRegular{imbNLP.PartOfSpeech.pipeline.machine.pipelineTaskSubjectContentToken}" />
    public class pipelineContentTokenLevelDistribution : pipelineNodeRegular<pipelineTaskSubjectContentToken>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="pipelineContentTokenLevelDistribution"/> class.
        /// </summary>
        public pipelineContentTokenLevelDistribution()
        {
            _nodeType = pipelineNodeTypeEnum.distributor;
            SetLabel();
            Label = "Distributor";
        }

        public IPipelineNode chunkPipeline { get; set; } = null;

        public IPipelineNode blockPipeline { get; set; } = null;

        public IPipelineNode streamPipeline { get; set; } = null;

        public IPipelineNode tokenPipeline { get; set; } = null;

        public IPipelineNode repoPipeline { get; set; } = null;

        public IPipelineNode sitePipeline { get; set; } = null;

        public IPipelineNode pagePipeline { get; set; } = null;

        /// <summary>
        /// Redirects the task by <see cref="cnt_level"/> to (if not null) corresponding pipeline
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns>pipeline to direct to</returns>
        public override IPipelineNode process(IPipelineTask task)
        {
            //pipelineTask<pipelineTaskSubjectContentToken> realTask = task as pipelineTask<pipelineTaskSubjectContentToken>;
            pipelineTaskSubjectContentToken realSubject = task.subject as pipelineTaskSubjectContentToken;

            if (realSubject == null)
            {
                if (task.context.RunInDebugMode)
                {
                    task.context.logger.log("Node " + name + " received a task [" + task.GetType().Name + "] with non compatibile task subject [" + task.subject.GetType().Name + "]");
                }
            }

            switch (realSubject.contentLevelType)
            {
                case cnt_level.mcBlock:
                    if (blockPipeline != null) return blockPipeline;
                    break;

                case cnt_level.mcChunk:
                    if (chunkPipeline != null) return chunkPipeline;
                    break;

                case cnt_level.mcPage:
                    if (pagePipeline != null) return pagePipeline;
                    break;

                case cnt_level.mcRepo:
                    if (repoPipeline != null) return repoPipeline;
                    break;

                case cnt_level.mcSite:
                    if (sitePipeline != null) return sitePipeline;
                    break;

                default:
                case cnt_level.none:
                case cnt_level.mcSubtoken:
                    return next;
                    break;

                case cnt_level.mcToken:
                    if (tokenPipeline != null) return tokenPipeline;
                    break;

                case cnt_level.mcTokenStream:
                    if (streamPipeline != null) return streamPipeline;
                    break;
            }

            // <---- tagging code

            return forward;
        }
    }
}