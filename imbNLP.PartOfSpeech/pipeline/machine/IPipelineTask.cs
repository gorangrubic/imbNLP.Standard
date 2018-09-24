// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPipelineTask.cs" company="imbVeles" >
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
using imbNLP.PartOfSpeech.pipeline.core;
using System;

namespace imbNLP.PartOfSpeech.pipeline.machine
{
    public interface IPipelineTask
    {
        String GetStringInfo(Boolean full = true);

        IPipelineModel model { get; }

        pipelineModelExecutionContext context { get; }

        /// <summary>
        /// Starts the task execution
        /// </summary>
        /// <param name="__model">The model.</param>
        void StartProcess(pipelineModelExecutionContext __context);

        /// <summary>
        /// How many iterations/node moves are left for this task
        /// </summary>
        /// <value>
        /// The life span left.
        /// </value>
        Int32 lifeSpanLeft { get; set; }

        /// <summary>
        /// Item that is subject of process
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        IPipelineTaskSubject subject { get; set; }

        /// <summary>
        /// Current state of the task
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        pipelineTaskStateEnum state
        {
            get;
        }

        /// <summary>
        /// Node that was executing before <see cref="currentNode"/>
        /// </summary>
        /// <value>
        /// The last node.
        /// </value>
        IPipelineNode lastNode { get; }

        /// <summary>
        /// Current node where the task is
        /// </summary>
        /// <value>
        /// The current node.
        /// </value>
        IPipelineNode currentNode { get; }

        /// <summary>
        /// Next node designated to traverse to
        /// </summary>
        /// <value>
        /// The next node.
        /// </value>
        IPipelineNode nextNode { get; }
    }
}