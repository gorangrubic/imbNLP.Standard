// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPipelineNode.cs" company="imbVeles" >
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
using imbSCI.Data.collection.graph;
using System;

namespace imbNLP.PartOfSpeech.pipeline.core
{
    /// <summary>
    /// Interface to a pipeline node
    /// </summary>
    public interface IPipelineNode : IGraphNode
    {
        /// <summary>
        /// Gives to node auto-unique name variation, adds it to children and returns newly added node, or optionally returns this node
        /// </summary>
        /// <param name="node">The node to add, and auto-rename if required.</param>
        /// <param name="returnHost">if set to <c>true</c> it will return this node, if set to <c>false</c> it will return newly created node for fluid operation</param>
        /// <returns>this node (<c>returnHost</c> is true) or newly added node</returns>
        IPipelineNode AddNode(IPipelineNode node, Boolean returnHost = false);

        /// <summary>
        /// Display name, used for reporting purposes
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        String Label { get; }

        /// <summary>
        /// Automatically sets label
        /// </summary>
        void SetLabel();

        /// <summary>
        /// Processes the specified task and returns the next node to go into
        /// </summary>
        /// <param name="task">The task.</param>
        IPipelineNode process(IPipelineTask task);

        /// <summary>
        /// Default next node to go, for negative results of the test
        /// </summary>
        /// <value>
        /// The next.
        /// </value>
        IPipelineNode next { get; }

        /// <summary>
        /// Default next node to go, for positive result of the test
        /// </summary>
        /// <value>
        /// The forward.
        /// </value>
        IPipelineNode forward { get; }

        /// <summary>
        /// Regerence to the model root
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        IPipelineModel model { get; }

        /// <summary>
        /// Type of node designator
        /// </summary>
        /// <value>
        /// The type of the node.
        /// </value>
        pipelineNodeTypeEnum nodeType { get; }
    }
}