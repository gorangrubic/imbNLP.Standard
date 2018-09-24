// --------------------------------------------------------------------------------------------------------------------
// <copyright file="multitextResourceParser.cs" company="imbVeles" >
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
namespace imbNLP.PartOfSpeech.resourceProviders.multitext
{
    using imbNLP.PartOfSpeech.resourceProviders.core;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Resource parser for MULTITEXT v5 text-format of morpho-syntactic dictionary
    /// </summary>
    /// <seealso cref="imbNLP.PartOfSpeech.resourceProviders.core.textResourceResolverBase" />
    public class multitextResourceParser : textResourceResolverBase
    {
        /// <summary>
        /// Setups the text resource resolver for MULTITEXT format
        /// </summary>
        /// <param name="resourceFilePath">File path to the actual lexic resource used</param>
        /// <param name="grammSpecFilename">Gram Tags converter specification</param>
        /// <param name="logger">The logger.</param>
        public multitextResourceParser(string resourceFilePath, string grammSpecFilename, ILogBuilder logger = null)
        {
            Setup(resourceFilePath, grammSpecFilename, logger);
        }

        protected Regex LineEntryParser = new Regex(@"^([\w]+)[\s]*([\w]+)[\s]*([\w\d\-]+)[\s]*([\d]+)[\s]*([\d\.]+)");

        /// <summary>
        /// Parser for text resource lexic unit definition
        /// </summary>
        /// <param name="line">The lexic unit definition line</param>
        /// <param name="inflectForm">The inflect form of a word</param>
        /// <param name="lemmaForm">The lemma form of a word</param>
        /// <param name="gramTag">String representation of the grammatic information</param>
        protected override void SelectFromLine(string line, out string inflectForm, out string lemmaForm, out string gramTag)
        {
            Match m = LineEntryParser.Match(line);
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

        /// <summary>
        /// Builds the search regex pattern for inflected form needle, optionally for lemma needle and gram tag needle
        /// </summary>
        /// <param name="inflectFormNeedle">The inflect form needle.</param>
        /// <param name="lemmaNeedle">The lemma needle.</param>
        /// <param name="gramTagNeedle">The gram tag needle.</param>
        /// <param name="allowPartialInflectedForms">if set to <c>true</c> [allow partial inflected forms].</param>
        /// <param name="allowPartialLemmaForms">if set to <c>true</c> [allow partial lemma forms].</param>
        /// <returns></returns>
        protected override string GetSearchRegex(string inflectFormNeedle, string lemmaNeedle = "", string gramTagNeedle = "", Boolean allowPartialInflectedForms = false, Boolean allowPartialLemmaForms = false)
        {
            string reg = "^";

            if (inflectFormNeedle == "")
            {
                reg += "[\\w]+";
            }
            else
            {
                if (allowPartialInflectedForms)
                {
                    reg += inflectFormNeedle + "[\\w]*";
                }
                else
                {
                    reg += inflectFormNeedle + "";
                }
            }

            reg += "[\\s]+";

            if (!lemmaNeedle.isNullOrEmpty())
            {
                if (allowPartialInflectedForms)
                {
                    reg += lemmaNeedle + "[\\w]*";
                }
                else
                {
                    reg += lemmaNeedle + "";
                }
            }
            else
            {
                reg += "[\\w]+";
            }

            reg += "[\\s]+";

            if (gramTagNeedle.isNullOrEmpty())
            {
                reg += "[\\w\\d-]+";
            }
            else
            {
                reg += gramTagNeedle + "[\\w\\d-]*";
            }

            return reg;
        }
    }
}