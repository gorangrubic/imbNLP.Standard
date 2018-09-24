// --------------------------------------------------------------------------------------------------------------------
// <copyright file="lexicInflection.cs" company="imbVeles" >
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
namespace imbNLP.PartOfSpeech.lexicUnit
{
    using imbNLP.PartOfSpeech.resourceProviders.core;
    using imbSCI.Data.collection.graph;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Graph representing single lexic inflection
    /// </summary>
    /// <seealso cref="imbNLP.PartOfSpeech.lexicUnit.lexicGraph" />
    [Serializable]
    public class lexicInflection : lexicGraph
    {
        /// <summary>
        /// Gets or sets the lexical definition line.
        /// </summary>
        /// <value>
        /// The lexical definition line.
        /// </value>
        public String lexicalDefinitionLine { get; set; } = "";

        /// <summary>
        /// Lemma form for this inflection
        /// </summary>
        /// <value>
        /// The lemma form.
        /// </value>
        public String lemmaForm { get; set; } = "";

        public String inflectedForm { get; set; } = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="lexicInflection"/> class.
        /// </summary>
        public lexicInflection()
        {
            nodeType = lexicGraphNodeType.inflection;
        }

        public lexicInflection(String __line)
        {
            nodeType = lexicGraphNodeType.inflection;
            lexicalDefinitionLine = __line;
        }

        public lexicInflection(String __name, String __lemmaForm)
        {
            name = __name;
            lemmaForm = __lemmaForm;
            nodeType = lexicGraphNodeType.inflection;
        }

        /// <summary>
        /// Gets the tag from gram tags.
        /// </summary>
        /// <returns></returns>
        public List<Object> GetAllTagsFromGramTags()
        {
            List<Object> output = new List<Object>();
            foreach (lexicGrammarCase gcase in this)
            {
                output.AddRange(gcase.tags.GetTags());
            }
            return output;
        }

        /// <summary>
        /// Gets all tags of specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="def">The definition.</param>
        /// <returns></returns>
        public List<T> GetTagFromGramTags<T>(T def = default(T)) where T : IConvertible
        {
            List<T> output = new List<T>();
            foreach (lexicGrammarCase gcase in this)
            {
                if (gcase.tags == null)
                {
                    return output;  //throw new aceGeneralException("-- gram tags are not loaded for [" + gcase.name + "]", null, this, "Inflection [" + name + "]");
                }
                else
                {
                    output.Add(gcase.tags.Get<T>(def));
                }
            }
            return output;
        }

        private Object GrammarLock = new Object();

        /// <summary>
        /// Adds the grammar case under this inflection
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <returns></returns>
        public lexicGrammarCase AddGrammarCase(grammaticTagCollection tags)
        {
            String n = tags.ToString();

            lock (GrammarLock)
            {
                if (mychildren.ContainsKey(n))
                {
                    return mychildren[n] as lexicGrammarCase;
                }

                lexicGrammarCase child = new lexicGrammarCase();
                child.name = n;
                child.tags = tags;
                Add(child);
                return child;
            }
        }

        /// <summary>
        /// Creates new child item and sets the name, but still do not add it. Used internally by <see cref="!:Add(String pathWithName)" />
        /// </summary>
        /// <param name="nameForChild">The name for child.</param>
        /// <returns></returns>
        public override graphNodeCustom CreateChildItem(string nameForChild)
        {
            var output = new lexicGrammarCase();
            output.name = nameForChild;
            return output;
        }
    }
}