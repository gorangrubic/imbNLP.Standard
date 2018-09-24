// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineTaskSubjectContentToken.cs" company="imbVeles" >
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
using imbMiningContext.MCDocumentStructure;
using imbNLP.PartOfSpeech.flags.token;
using imbNLP.PartOfSpeech.lexicUnit;
using imbNLP.PartOfSpeech.pipeline.machine;
using imbSCI.Core.extensions.data;
using imbSCI.Core.extensions.text;
using imbSCI.Core.files.fileDataStructure;
using imbSCI.Data.collection.graph;
using System;
using System.Collections.Generic;

namespace imbNLP.PartOfSpeech.pipelineForPos.subject
{
    /// <summary>
    /// Task subject for content token
    /// </summary>
    /// <seealso cref="imbNLP.PartOfSpeech.pipeline.machine.IPipelineTaskSubject" />
    [Serializable]
    public class pipelineTaskSubjectContentToken : graphNodeCustom, IPipelineTaskSubject
    {
        public pipelineTaskSubjectContentToken()
        {
        }

        private String _Label;

        /// <summary>
        /// String used for graph display
        /// </summary>
        public String Label
        {
            get
            {
                if (_Label.isNullOrEmpty())
                {
                    return name;
                }
                return _Label;
            }
            set { _Label = value; }
        }

        public virtual void SetLabel()
        {
            if (mcElement == null)
            {
                _Label = currentForm.or(initialForm, contentLevelType.keyToString());
            }
            else
            {
                String lb = mcElement.name.or(mcElement.content);
                _Label = lb;
            }
        }

        protected IFileDataStructure fileDataStructure { get; set; }

        public imbMCDocumentElement mcElement { get; set; }

        public cnt_level contentLevelType { get; set; } = cnt_level.none;

        /// <summary>
        /// Current form of the task subject
        /// </summary>
        /// <value>
        /// The current form.
        /// </value>
        public String currentForm { get; set; } = "";

        /// <summary>
        /// Initial form of task subject
        /// </summary>
        /// <value>
        /// The initial form.
        /// </value>
        public String initialForm { get; set; } = "";

        /// <summary>
        /// code identifier
        /// </summary>
        /// <value>
        /// The code identifier.
        /// </value>
        public String codeID { get; set; } = "";

        public void AddGraph(lexicInflection _graph)
        {
            graph = _graph;
            foreach (lexicGrammarCase gcase in graph)
            {
                flagBag.AddRange(gcase.tags.GetTags(), true);
            }
        }

        /// <summary>
        /// Gets or sets the graph.
        /// </summary>
        /// <value>
        /// The graph.
        /// </value>
        public lexicInflection graph { get; set; }

        /// <summary>
        /// Bag of various flags, associated to the task subject
        /// </summary>
        /// <value>
        /// The flag bag.
        /// </value>
        public List<Object> flagBag { get; set; } = new List<object>();

        protected override bool doAutorenameOnExisting { get { return true; } }

        protected override bool doAutonameFromTypeName { get { return true; } }
    }
}