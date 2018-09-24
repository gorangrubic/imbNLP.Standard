// --------------------------------------------------------------------------------------------------------------------
// <copyright file="multitextIndexedResolver.cs" company="imbVeles" >
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
using System;

namespace imbNLP.PartOfSpeech.resourceProviders.core
{
    using System.Text.RegularExpressions;

    [Serializable]
    public class multitextIndexedResolver : textResourceIndexBase
    {
        public multitextIndexedResolver()
        {
        }

        protected Regex LineEntryParser = new Regex(@"^([\w]+)[\s]*([\w]+)[\s]*([\w\d\-]+)[\s]*([\d]+)[\s]*([\d\.]+)");

        protected Regex LineEntryShort = new Regex(@"^([\w]+)[\s]*([\w]+)[\s]*([\w\d\-]+)");

        /// <summary>
        /// Parser for text resource lexic unit definition
        /// </summary>
        /// <param name="line">The lexic unit definition line</param>
        /// <param name="inflectForm">The inflect form of a word</param>
        /// <param name="lemmaForm">The lemma form of a word</param>
        /// <param name="gramTag">String representation of the grammatic information</param>
        protected override void SelectFromLine(string line, out string inflectForm, out string lemmaForm, out string gramTag)
        {
            Match m = LineEntryShort.Match(line);
            if (m.Success)
            {
                inflectForm = m.Groups[1].Value;
                lemmaForm = m.Groups[2].Value;
                gramTag = m.Groups[3].Value;
            }
            else
            {
                inflectForm = "";
                lemmaForm = "";
                gramTag = "";
            }
        }

        protected override string RenderToLine(string inflectForm, string lemmaForm, string gramTag)
        {
            String output = inflectForm + "\t" + lemmaForm + "\t" + gramTag;
            return output;
        }
    }
}