// --------------------------------------------------------------------------------------------------------------------
// <copyright file="morphRuleSet.cs" company="imbVeles" >
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
// Project: imbNLP.Data
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------

//using aceCommonTypes.extensions.text;
//using aceCommonTypes.extensions.enumworks;
//using aceCommonTypes.extensions.typeworks;
//using aceCommonTypes.extensions.io;

namespace imbNLP.Data.semanticLexicon.morphology
{
    using imbNLP.Data.semanticLexicon.explore;
    using imbNLP.Data.semanticLexicon.posCase;
    using imbNLP.PartOfSpeech.flags.basic;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public abstract class morphRuleSet
    {
        public morphRuleSet()
        {
        }

        public List<termExploreItem> GetItems(morphRuleMatch match, string excludeForm)
        {
            Dictionary<string, gramCaseSet> output = new Dictionary<string, gramCaseSet>();
            List<termExploreItem> items = new List<termExploreItem>();
            foreach (morphRule rule in rules)
            {
                string form = rule.GetForm(match.root, match.stem, SharpSufix);
                if (excludeForm != form)
                {
                    if (!output.ContainsKey(form))
                    {
                        output.Add(form, new gramCaseSet());
                    }
                    output[form].Add(rule.gramEntry);
                }
            }

            foreach (string form in output.Keys)
            {
                termExploreItem item = new termExploreItem(form);
                item.gramSet = output[form];
                items.Add(item);
            }
            return items;
        }

        public morphRule GetRule(params Enum[] grams)
        {
            foreach (morphRule rule in rules)
            {
                if (rule.gramEntry.ToList().ContainsAll(grams))
                {
                    return rule;
                }
            }
            return rules.First();
        }

        public List<morphRule> GetRules(Enum[] grams)
        {
            List<morphRule> output = new List<morphRule>();
            foreach (morphRule rule in rules)
            {
                if (rule.gramEntry.ToList().ContainsAll(grams))
                {
                    output.Add(rule);
                }
            }
            return output;
        }

        public gramCaseSet GetGramSet(string sufix)
        {
            gramCaseSet output = new gramCaseSet();
            foreach (morphRule rule in rules)
            {
                if (rule.sufix == sufix)
                {
                    output.Add(rule.gramEntry);
                }
            }

            return output;
        }

        public gramCaseSet GetGramSet(morphRuleMatch match)
        {
            gramCaseSet output = new gramCaseSet();
            foreach (morphRule rule in rules)
            {
                if (rule.sufix == match.sufix)
                {
                    output.Add(rule.gramEntry);
                }
            }

            return output;
        }

        public string SharpSufix { get; set; }

        /// <summary>
        /// Adds the specified sufix: $ is last letter of nominative, # is n, t or s
        /// </summary>
        /// <param name="sufix">The sufix.</param>
        /// <param name="gram">The gram.</param>
        /// <returns></returns>
        public morphRule Add(string sufix, gramFlags gram)
        {
            gram.type = type;

            morphRule output = new morphRule(sufix, gram);

            rules.Add(output);

            return output;
        }

        public void setup(string __regex, pos_type __type)
        {
            regexCriteria = __regex;
            regex = new Regex(__regex);
            type = __type;
        }

        public abstract void SetRules(params Enum[] gramEnums);

        public morphRuleMatch Match(string token, ILogBuilder loger)
        {
            if (regex.IsMatch(token))
            {
                morphRuleMatch match = new morphRuleMatch();
                match.ruleSet = this;
                match.item = match.createExploreItem(token);

                return match;
            }
            else
            {
                return null;
            }
        }

        public string regexCriteria { get; protected set; }

        public pos_type type { get; protected set; }

        internal Regex regex { get; set; }

        /// <summary> </summary>
        public List<morphRule> rules { get; protected set; } = new List<morphRule>();
    }
}