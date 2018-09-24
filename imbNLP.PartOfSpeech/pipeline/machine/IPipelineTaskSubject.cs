// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPipelineTaskSubject.cs" company="imbVeles" >
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
using imbNLP.PartOfSpeech.flags.token;
using imbSCI.Data.collection.graph;
using System;
using System.Collections.Generic;

namespace imbNLP.PartOfSpeech.pipeline.machine
{
    /// <summary>
    /// Subject of pipeline processing
    /// </summary>
    public interface IPipelineTaskSubject : IGraphNode
    {
        /// <summary>
        /// Auto-sets the label.
        /// </summary>
        void SetLabel();

        /// <summary>
        /// Display label
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        String Label { get; set; }

        cnt_level contentLevelType { get; set; }

        /// <summary>
        /// Current form of the task subject
        /// </summary>
        /// <value>
        /// The current form.
        /// </value>
        String currentForm { get; set; }

        /// <summary>
        /// Initial form of task subject
        /// </summary>
        /// <value>
        /// The initial form.
        /// </value>
        String initialForm { get; set; }

        /// <summary>
        /// code identifier
        /// </summary>
        /// <value>
        /// The code identifier.
        /// </value>
        String codeID { get; set; }

        /// <summary>
        /// Bag of various flags, associated to the task subject
        /// </summary>
        /// <value>
        /// The flag bag.
        /// </value>
        List<Object> flagBag { get; set; }
    }
}