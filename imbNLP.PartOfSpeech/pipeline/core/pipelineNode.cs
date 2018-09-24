// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineNode.cs" company="imbVeles" >
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
using imbSCI.Data;
using imbSCI.Data.collection.graph;
using System;

namespace imbNLP.PartOfSpeech.pipeline.core
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="imbSCI.Data.collection.graph.graphNodeCustom" />
    /// <seealso cref="imbNLP.PartOfSpeech.pipeline.core.IPipelineNode" />
    public abstract class pipelineNode<T> : graphNodeCustom, IPipelineNode where T : IPipelineTaskSubject
    {
        protected override bool doAutorenameOnExisting { get { return false; } }

        protected override bool doAutonameFromTypeName { get { return false; } }

        public void SetLabel()
        {
            String ln = this.GetType().Name.Replace("pipeline", "");
            ln = ln.Replace("Node", "");
            ln = ln.Replace("Transformer", "");
            if (ln.isNullOrEmpty()) ln = name;
            Label = ln.imbTitleCamelOperation(true); // + " [" + nodeType.ToString() + "]";
        }

        protected pipelineNode()
        {
            if (name.isNullOrEmpty())
            {
                name = GetType().Name + "_" + typeof(T).Name;
                name = name.Replace("Node", "");
                name = name.Replace("pipeline", "");
                name = name.Replace("Task", "");
                name = name.Replace("Subject", "");
                name = name.imbTitleCamelOperation(true).imbGetAbbrevation(5, true);
            }
        }

        private String _Label;

        /// <summary>
        /// Title of the node, to be displayed in graphs
        /// </summary>
        public String Label
        {
            get
            {
                if (_Label.isNullOrEmpty()) return name;
                return _Label;
            }
            set { _Label = value; }
        }

        /// <summary>
        /// Process call
        /// </summary>
        /// <param name="task">The task.</param>
        public abstract IPipelineNode process(IPipelineTask task);

        public const Int32 AUTORENAME_LIMIT = 99;

        /// <summary>
        /// Gives to node auto-unique name variation, adds it to children and returns newly added node, or optionally returns this node
        /// </summary>
        /// <param name="node">The node to add, and auto-rename if required.</param>
        /// <param name="returnHost">if set to <c>true</c> it will return this node, if set to <c>false</c> it will return newly created node for fluid operation</param>
        /// <returns>
        /// this node (<c>returnHost</c> is true) or newly added node
        /// </returns>
        public IPipelineNode AddNode(IPipelineNode node, bool returnHost = false)
        {
            return NestNode(node, returnHost);
        }

        /// <summary>
        /// Nests the node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="returnHost">if set to <c>true</c> [return host].</param>
        /// <returns></returns>
        protected IPipelineNode NestNode(IPipelineNode node, bool returnHost = false)
        {
            if (node.parent == null)
            {
                node.name = this.MakeUniqueChildName(node.name, AUTORENAME_LIMIT, Count());
                children.Add(node.name, node);
                node.parent = this;
            }

            if (returnHost) return this;

            return node;
        }

        /// <summary>
        /// Adds the stealth link.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="returnHost">if set to <c>true</c> [return host].</param>
        /// <returns></returns>
        public IPipelineNode AddStealthLink(IPipelineNode node, bool returnHost = false)
        {
            node.name = this.MakeUniqueChildName(node.name, AUTORENAME_LIMIT, Count());
            node.parent = this;

            if (returnHost) return this;

            return node;
        }

        /// <summary>
        /// Default next pipeline node
        /// </summary>
        /// <value>
        /// The next.
        /// </value>
        public abstract IPipelineNode next { get; }

        /// <summary>
        /// Default next node to go, for positive result of the test
        /// </summary>
        /// <value>
        /// The forward.
        /// </value>
        public abstract IPipelineNode forward { get; }

        /// <summary>
        /// Type of node designator
        /// </summary>
        /// <value>
        /// The type of the node.
        /// </value>
        public abstract pipelineNodeTypeEnum nodeType { get; }

        /// <summary>
        /// Regerence to the model root
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public IPipelineModel model
        {
            get
            {
                if (this is IPipelineModel) return this as IPipelineModel;

                IPipelineNode mRoot = root as IPipelineNode;
                if (mRoot is IPipelineModel) return mRoot as IPipelineModel;
                return null;
            }
        }
    }
}