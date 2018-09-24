// --------------------------------------------------------------------------------------------------------------------
// <copyright file="chunkMatchRuleSet.cs" company="imbVeles" >
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
using imbNLP.PartOfSpeech.map;
using imbNLP.PartOfSpeech.pipelineForPos.render;
using imbSCI.Core.extensions.data;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.PartOfSpeech.decomposing.chunk
{
    public class chunkMatchRuleSet : List<chunkMatchRule>
    {
        public chunkMatchRuleSet()
        {
        }

        public void SaveTypeNames()
        {
            foreach (chunkMatchRule rule in this)
            {
                rule.flagTypesToMatchNames.Clear();
                foreach (Type t in rule.flagTypesToMatch)
                {
                    rule.flagTypesToMatchNames.Add(t.Name);
                }
            }
        }

        /// <summary>
        /// Describes its rules
        /// </summary>
        /// <returns></returns>
        public List<string> DescribeSelf()
        {
            List<String> output = new List<string>();
            Int32 i = 0;

            output.Add("Contains [" + Count + "] POS token matching rules");

            foreach (chunkMatchRule rule in this)
            {
                i++;
                output.Add("Rule [" + i.ToString("D2") + "] creates chunk of POS type: " + rule.chunkType + ")");
                if (!rule.regexPattern.isNullOrEmpty())
                {
                    output.Add(" > Based on Regex query [" + rule.regexPattern + "]");
                    output.Add(" > The Regex is evaluated against mapped textual POS-flags rendering [" + rule.renderMode + "]");
                }
                if (rule.flagTypesToMatchNames.Any())
                {
                    output.Add(" > Rule requires from evaluated POS tokens to have the same values on the following flag types:");
                    String flagTypeLine = "";
                    rule.flagTypesToMatchNames.ForEach(x => flagTypeLine = flagTypeLine.add(x, ","));
                    output.Add(" >  [" + flagTypeLine + "]");
                }
                output.Add(" > Execution priority: [" + rule.priority + "]");
                output.Add("    ");
            }

            output.Add("    ");

            return output;
        }

        public const String itemFormat = @"([\w\s]+,[\s]*{0}[\s]*,[\w\s]+|[\w\s]+,[\s]*{0}[\s]*|[\s]*{0}[\s]*|[\s]*{0}[\s]*,[\w\s]+)";

        public chunkMatchRule AddTypeAndFlagRule(pos_type[] posTypePattern, Type[] flagTypesToMatch, pos_type chunkType)
        {
            chunkMatchRule rule = new chunkMatchRule();
            rule.renderMode = contentTokenSubjectRenderMode.posTypeTagForm;
            rule.chunkType = chunkType;
            String rgx = "";

            for (int i = 0; i < posTypePattern.Length; i++)
            {
                var ps = posTypePattern[i];

                rgx = rgx + textMapBase.SEPARATOR + String.Format(itemFormat, ps.toString());
            }
            rgx = rgx + textMapBase.SEPARATOR;
            rule._regexPattern = rgx;
            rule.flagTypesToMatch.AddRange(flagTypesToMatch);
            rule.flagTypesToMatch.ForEach(x => rule.flagTypesToMatchNames.AddUnique(x.Name));

            Add(rule);
            return rule;
        }

        public void prepare(Dictionary<string, Type> _pos_enum_types)
        {
            foreach (chunkMatchRule rule in this)
            {
                foreach (String ft in rule.flagTypesToMatchNames)
                {
                    if (_pos_enum_types.ContainsKey(ft))
                    {
                        rule.flagTypesToMatch.Add(_pos_enum_types[ft]);
                    }
                }
            }

            Sort((x, y) => x.priority.CompareTo(y.priority));
        }
    }
}