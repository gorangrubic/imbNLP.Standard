// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineModel.cs" company="imbVeles" >
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
using imbACE.Core.core;
using imbNLP.PartOfSpeech.pipeline.machine;
using imbSCI.Core.extensions.data;
using imbSCI.Core.reporting;
using imbSCI.Data.collection.graph;
using System;
using System.Collections.Generic;

namespace imbNLP.PartOfSpeech.pipeline.core
{
    /// <summary>
    /// Model
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="imbNLP.PartOfSpeech.pipeline.core.pipelineNode{T}" />
    /// <seealso cref="imbNLP.PartOfSpeech.pipeline.core.IPipelineModel" />
    public abstract class pipelineModel<T> : pipelineNode<T>, IPipelineModel where T : class, IPipelineTaskSubject
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public String description { get; set; } = "";

        /// <summary>
        /// Process of self construction
        /// </summary>
        public abstract void constructionProcess();

        /// <summary>
        /// It will be called by <see cref="pipelineMachine.run(IPipelineModel)"/> method to get initial tasks to run
        /// </summary>
        /// <param name="resources">Arbitrary resources that might be used for task creation</param>
        /// <returns></returns>
        public abstract List<IPipelineTask> createPrimaryTasks(Object[] resources);

        public override pipelineNodeTypeEnum nodeType
        {
            get
            {
                return pipelineNodeTypeEnum.model;
            }
        }

        /// <summary>
        /// Process call -- just forwards the task to its first child
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        public override IPipelineNode process(IPipelineTask task)
        {
            return forward;
        }

        protected pipelineModel(ILogBuilder _logger = null, String __name = "") : base()
        {
            if (!__name.isNullOrEmpty()) { name = __name; }

            exitBin = new pipelineNodeBin<T>("ExitBin", this);
            trashBin = new pipelineNodeBin<T>("TrashBin", this);

            if (_logger == null)
            {
                builderForLog __logger = new builderForLog();
                logger = __logger;
                imbSCI.Core.screenOutputControl.logToConsoleControl.setAsOutput(__logger, name);

                logger = _logger;
            }
            else
            {
                logger = _logger;
            }
        }

        public ILogBuilder logger { get; set; } = null;

        public override graphNodeCustom CreateChildItem(string nameForChild)
        {
            return null;
        }

        public override IPipelineNode forward
        {
            get
            {
                return getFirst() as IPipelineNode;
            }
        }

        /// <summary>
        /// Default next pipeline node
        /// </summary>
        /// <value>
        /// The next.
        /// </value>
        public override IPipelineNode next => exitBin;

        /// <summary>
        /// Default exit bin
        /// </summary>
        /// <value>
        /// The exit bin.
        /// </value>
        public pipelineNodeBin<T> exitBin { get; protected set; }

        /// <summary>
        /// Trashed
        /// </summary>
        /// <value>
        /// The trash bin.
        /// </value>
        public pipelineNodeBin<T> trashBin { get; protected set; }

        /// <summary>
        /// Initial life for new task at this model
        /// </summary>
        /// <value>
        /// The task initial life.
        /// </value>
        public int taskInitialLife { get; set; } = 100;

        /// <summary>
        /// Default exit bin
        /// </summary>
        /// <value>
        /// The exit bin.
        /// </value>
        IPipelineNodeBin IPipelineModel.exitBin => exitBin;

        /// <summary>
        /// Trashed
        /// </summary>
        /// <value>
        /// The trash bin.
        /// </value>
        IPipelineNodeBin IPipelineModel.trashBin => trashBin;
    }
}