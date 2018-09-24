// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineNodeRegular.cs" company="imbVeles" >
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
using imbSCI.Core.extensions.data;
using imbSCI.Core.extensions.text;
using imbSCI.Data.collection.graph;
using System;

namespace imbNLP.PartOfSpeech.pipeline.core
{
    /// <summary>
    /// Regular pipeline node with forward and next node directions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="imbNLP.PartOfSpeech.pipeline.core.pipelineNode{T}" />
    /// <seealso cref="imbNLP.PartOfSpeech.pipeline.core.IPipelineNode" />
    public abstract class pipelineNodeRegular<T> : pipelineNode<T>, IPipelineNodeRegular where T : class, IPipelineTaskSubject
    {
        /// <summary>
        /// Adds the nodes as <see cref="forwardPredefined"/> and <see cref="nextPredefined"/>, and returns node according to <c>returnHost</c> and <c>returnNextNotForward</c>
        /// </summary>
        /// <param name="__forwardNode">The forward node.</param>
        /// <param name="__nextNode">The next node.</param>
        /// <param name="returnHost">if set to <c>true</c> [return host].</param>
        /// <param name="returnNextNotForward">if set to <c>true</c> [return next not forward].</param>
        /// <returns></returns>
        public IPipelineNode AddNodes(IPipelineNode __forwardNode, IPipelineNode __nextNode, bool returnHost = false, bool returnNextNotForward = false)
        {
            if (__forwardNode != null)
            {
                forwardPredefined = NestNode(__forwardNode, false);
            }

            if (__nextNode != null)
            {
                nextPredefined = NestNode(__forwardNode, false);
            }

            if (returnHost) return this;

            if (returnNextNotForward)
            {
                return __nextNode;
            }
            else
            {
                return __forwardNode;
            }
        }

        /// <summary>
        /// Automatically sets label
        /// </summary>
        public void SetLabel()
        {
            String ln = this.GetType().Name.Replace("pipeline", "");
            ln = ln.Replace("Node", "");
            ln = ln.Replace("Transformer", "");
            Label = ln.imbTitleCamelOperation(true); // + " [" + nodeType.ToString() + "]";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="pipelineNodeRegular{T}"/> class.
        /// </summary>
        protected pipelineNodeRegular()
        {
        }

        protected pipelineNodeTypeEnum _nodeType = pipelineNodeTypeEnum.distributor;

        public override pipelineNodeTypeEnum nodeType
        {
            get
            {
                return _nodeType;
            }
        }

        public IPipelineNode forwardPredefined { get; set; }

        /// <summary>
        /// Default next node to go, for positive result of the test
        /// </summary>
        /// <value>
        /// The forward.
        /// </value>
        public override IPipelineNode forward
        {
            get
            {
                if (forwardPredefined != null) return forwardPredefined;

                if (children.Count > 0)
                {
                    return getFirst() as IPipelineNode;
                }
                return next;
            }
        }

        public IPipelineNode nextPredefined { get; set; }

        /// <summary>
        /// Default next pipeline node: if there is a child, then child -- if there is a sibling next to this node> then sibling, if this was the last child of parent then <see cref="IPipelineModel.exitBin"/>
        /// </summary>
        /// <value>
        /// The next.
        /// </value>
        public override IPipelineNode next
        {
            get
            {
                if (nextPredefined != null) return nextPredefined;

                Boolean breakNext = false;
                String nextName = "";

                if (parent != null)
                {
                    foreach (String ch in parent.getChildNames())
                    {
                        if (breakNext)
                        {
                            nextName = ch;
                            break;
                        }
                        if (ch == name) breakNext = true;
                    }
                }

                IPipelineNode pNode = parent as IPipelineNode;
                IPipelineNode nNode = null;

                if (nextName.isNullOrEmpty())
                {
                    nNode = model.exitBin;
                }
                else
                {
                    nNode = pNode[nextName] as IPipelineNode;
                }

                return nNode;
            }
        }

        public override graphNodeCustom CreateChildItem(string nameForChild)
        {
            return null;
        }

        public abstract override IPipelineNode process(IPipelineTask task);
    }
}