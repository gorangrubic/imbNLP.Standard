// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineNodeBin.cs" company="imbVeles" >
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
using imbSCI.Core.extensions.text;
using imbSCI.Data.collection.graph;

using System;
using System.Collections;
using System.Collections.Concurrent;

namespace imbNLP.PartOfSpeech.pipeline.core
{
    /// <summary>
    /// End node
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="imbNLP.PartOfSpeech.pipeline.core.pipelineNode{T}" />
    /// <seealso cref="imbNLP.PartOfSpeech.pipeline.core.IPipelineNode" />
    public class pipelineNodeBin<T> : pipelineNode<T>, IPipelineNodeBin where T : class, IPipelineTaskSubject
    {
        public pipelineNodeBin(String __name, IPipelineNode __parent = null)
        {
            name = __name;
            if (__parent != null) parent = parent;
        }

        public void SetLabel()
        {
            String ln = this.GetType().Name.Replace("pipeline", "");
            ln = ln.Replace("Node", "");
            ln = ln.Replace("Transformer", "");
            ln = ln + " " + name;
            Label = ln.imbTitleCamelOperation(true); // + " [" + nodeType.ToString() + "]";
        }

        /// <summary>
        /// Gets a value indicating whether this instance is running in routed counter mode.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is running in routed counter mode; otherwise, <c>false</c>.
        /// </value>
        public Boolean isRunningInRoutedCounterMode
        {
            get
            {
                return (routedContainer != null);
            }
        }

        /// <summary>
        /// Repository for self contained bin mode
        /// </summary>
        protected ConcurrentBag<IPipelineTaskSubject> selfContainer { get; set; } = new ConcurrentBag<IPipelineTaskSubject>();

        /// <summary>
        /// Gets or sets the routed container.
        /// </summary>
        /// <value>
        /// The routed container.
        /// </value>
        protected ConcurrentBag<IPipelineTaskSubject> routedContainer { get; set; } = null;

        /// <summary>
        /// Task Subjects that are contained
        /// </summary>
        /// <value>
        /// The contained.
        /// </value>
        public ConcurrentBag<IPipelineTaskSubject> container
        {
            get
            {
                if (routedContainer != null) return routedContainer;
                return selfContainer;
            }
        }

        public void SetRoute(ConcurrentBag<IPipelineTaskSubject> __routedContainer)
        {
            routedContainer = __routedContainer;
        }

        public override IPipelineNode next { get { return null; } }

        public override IPipelineNode forward { get { return null; } }

        public override pipelineNodeTypeEnum nodeType
        {
            get
            {
                return pipelineNodeTypeEnum.bin;
            }
        }

        bool IPipelineNodeBin.isRunningInRoutedCounterMode => throw new NotImplementedException();

        public override graphNodeCustom CreateChildItem(string nameForChild)
        {
            return null;
        }

        public override IPipelineNode process(IPipelineTask task)
        {
            container.Add(task.subject as T);
            return null;
        }

        void IPipelineNodeBin.SetRoute(IEnumerable __routedContainer)
        {
            SetRoute((ConcurrentBag<IPipelineTaskSubject>)__routedContainer);
        }
    }
}