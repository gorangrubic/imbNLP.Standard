// --------------------------------------------------------------------------------------------------------------------
// <copyright file="morphMachine.cs" company="imbVeles" >
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
namespace imbNLP.Data.semanticLexicon.morphology
{
    using imbNLP.PartOfSpeech.flags.basic;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.search;
    using imbSCI.Core.files.unit;
    using imbSCI.Core.reporting;
    using imbSCI.DataComplex.extensions.text;
    using System.Collections.Generic;
    using System.IO;

    public abstract class morphMachine
    {
        public abstract void SetRuleSets();

        public fileTextOperater inputOperator { get; protected set; }

        public fileunit inputFile { get; protected set; }

        public fileunit ignoreFile { get; protected set; }

        public morphMachine(folderNode node)
        {
            folder = node;

            string file = folder.findFile("lexiconCache_negatives.txt", SearchOption.AllDirectories, false);

            inputFile = new fileunit(file, true);

            string ignorePath = folder.pathFor("morphMachine_ignore.txt");
            ignoreFile = new fileunit(ignorePath, true);

            SetRuleSets();
        }

        /// <summary>
        /// Gets the next token - gets next not ingored file
        /// </summary>
        /// <returns></returns>
        public string GetNextToken()
        {
            foreach (string inStr in inputFile.contentLines)
            {
                if (!ignoreFile.contentLines.Contains(inStr))
                {
                    return inStr;
                }
            }
            return "";
        }

        public void SetResolved(IEnumerable<string> tokens)
        {
            inputFile.contentLines.removeRange(tokens);

            foreach (string tkn in tokens)
            {
                if (tkn.isNonDosChars())
                {
                    inputFile.contentLines.Remove(tkn);
                }
            }

            inputFile.Save();
        }

        public void SetIgnore(IEnumerable<string> tokens)
        {
            ignoreFile.Append(tokens, false);
            foreach (string tkn in tokens)
            {
                if (tkn.isNonDosChars())
                {
                    ignoreFile.Append(tkn.toDosCleanDirect());
                }
            }

            ignoreFile.Save();
        }

        public void SetIgnore(string token)
        {
            ignoreFile.Append(token, false);
            if (token.isNonDosChars())
            {
                ignoreFile.Append(token.toDosCleanDirect());
            }
            ignoreFile.Save();
        }

        public void Save()
        {
            ignoreFile.Save();
        }

        //public morphMachine(fileunit input)
        //{
        //    inputOperator = input.getOperater(true);
        //}

        public folderNode folder { get; protected set; }

        public morphRuleSet Add<T>(string __regex, pos_type __type) where T : morphRuleSet, new()
        {
            morphRuleSet output = new T();

            output.setup(__regex, __type);
            //output.SetRules();

            rules.Add(output);

            return output;
        }

        /// <summary>
        /// Explores the specified token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="loger">The loger.</param>
        /// <returns></returns>
        public morphRuleMatchSet Explore(string token, ILogBuilder loger)
        {
            morphRuleMatchSet output = new morphRuleMatchSet();
            foreach (morphRuleSet ruleSet in rules)
            {
                output.Add(ruleSet.Match(token, loger));
            }

            return output;
        }

        /// <summary> </summary>
        public List<morphRuleSet> rules { get; protected set; } = new List<morphRuleSet>();
    }
}