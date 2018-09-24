// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineTaskTFDFContentToken.cs" company="imbVeles" >
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
using imbNLP.PartOfSpeech.pipeline.machine;
using imbSCI.DataComplex.tf_idf;

namespace imbNLP.PartOfSpeech.pipelineForPos.subject
{
    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="imbSCI.Data.collection.graph.graphNodeCustom" />
    /// <seealso cref="imbNLP.PartOfSpeech.pipeline.machine.IPipelineTaskSubject" />
    public class pipelineTaskTFDFContentSubject : pipelineTaskSubjectContentToken, IPipelineTaskSubject
    {
        public pipelineTaskTFDFContentSubject()
        {
        }

        public pipelineTaskTFDFContentSubject(TFDFContainer container)
        {
            tfdf = container;
            currentForm = tfdf.indexForm;
            initialForm = tfdf.indexForm;
            contentLevelType = cnt_level.mcToken;
        }

        public TFDFContainer tfdf { get; set; } = null;

        //public string currentForm { get; set; } = "";
        //public string initialForm { get; set; } = "";
        //public string codeID { get; set; } = "";
        //public List<object> flagBag { get; set; } = new List<object>();

        //protected override bool doAutorenameOnExisting { get { return true; } }

        //protected override bool doAutonameFromTypeName { get { return true; } }
    }
}