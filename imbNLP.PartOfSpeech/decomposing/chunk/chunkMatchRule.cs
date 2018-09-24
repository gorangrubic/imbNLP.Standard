// --------------------------------------------------------------------------------------------------------------------
// <copyright file="chunkMatchRule.cs" company="imbVeles" >
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
using imbNLP.PartOfSpeech.flags.basic;
using imbNLP.PartOfSpeech.flags.token;
using imbNLP.PartOfSpeech.pipelineForPos.render;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace imbNLP.PartOfSpeech.decomposing.chunk
{
    /// <summary>
    /// Singe regex-based rule to build part-of-speech quring using different <see cref="imbMCTokenStream"/> renderings
    /// </summary>
    public class chunkMatchRule
    {
        public chunkMatchRule()
        {
        }

        [XmlAttribute]
        public pos_type chunkType { get; set; } = pos_type.N;

        public cnt_level[] contentLevel { get; set; } = new cnt_level[] { cnt_level.mcToken, cnt_level.mcChunk };

        /// <summary>
        /// Names of enumeration types whose values have to match in order to confirm prior regex match
        /// </summary>
        /// <value>
        /// The flag types to match names.
        /// </value>
        public List<String> flagTypesToMatchNames { get; set; } = new List<String>();

        [XmlIgnore]
        public List<Type> flagTypesToMatch { get; set; } = new List<Type>();

        [XmlIgnore]
        public String _regexPattern = "";

        [XmlElement("CDataElement")]
        public XmlCDataSection regexPattern
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                return doc.CreateCDataSection(_regexPattern);
            }
            set
            {
                _regexPattern = value.Value;
            }
        }

        private Regex _regex { get; set; } = null;

        [XmlIgnore]
        public Regex regex
        {
            get
            {
                if (_regex == null)
                {
                    _regex = new Regex(regexPattern.Value, RegexOptions.IgnoreCase);
                }
                return _regex;
            }
        }

        [XmlAttribute]
        public contentTokenSubjectRenderMode renderMode { get; set; } = contentTokenSubjectRenderMode.posTypeTagForm;

        public Int32 priority { get; set; } = 100;
    }
}