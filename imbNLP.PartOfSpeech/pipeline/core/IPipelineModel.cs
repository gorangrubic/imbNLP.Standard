// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPipelineModel.cs" company="imbVeles" >
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
using System;
using System.Collections.Generic;

namespace imbNLP.PartOfSpeech.pipeline.core
{
    /// <summary>
    /// Interface for pipeline model
    /// </summary>
    public interface IPipelineModel : IPipelineNode
    {
        /// <summary>
        /// Constructions the process.
        /// </summary>
        void constructionProcess();

        /// <summary>
        /// Creates the primary tasks, called by machine's run method
        /// </summary>
        /// <param name="resources">The resources.</param>
        /// <returns></returns>
        List<IPipelineTask> createPrimaryTasks(Object[] resources);

        /// <summary>
        /// Initial life for new task at this model
        /// </summary>
        /// <value>
        /// The task initial life.
        /// </value>
        Int32 taskInitialLife { get; }

        /// <summary>
        /// Default exit bin
        /// </summary>
        /// <value>
        /// The exit bin.
        /// </value>
        IPipelineNodeBin exitBin { get; }

        /// <summary>
        /// Trashed
        /// </summary>
        /// <value>
        /// The trash bin.
        /// </value>
        IPipelineNodeBin trashBin { get; }
    }
}