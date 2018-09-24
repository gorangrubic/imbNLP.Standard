// --------------------------------------------------------------------------------------------------------------------
// <copyright file="gramCaseSet.cs" company="imbVeles" >
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
namespace imbNLP.Data.semanticLexicon.posCase
{
    using imbNLP.PartOfSpeech.flags.basic;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    [XmlInclude(typeof(KeyValuePair<string, gramFlags>))]
    public class gramCaseSet : IEnumerable<KeyValuePair<string, gramFlags>>
    {
        public gramCaseSet()
        {
        }

        /// <summary>
        /// Creates inline string representation of the case set
        /// </summary>
        /// <returns></returns>
        public string GetAll()
        {
            string output = "";
            foreach (var it in items)
            {
                output = output.add(it.Key, "|");
            }
            return output;
        }

        /// <summary>
        /// Reads the inline string representation and populates the content
        /// </summary>
        public void SetAll(string declaration)
        {
            if (declaration.isNullOrEmptyString()) return;

            List<string> keys = declaration.SplitSmart("|");
            items.Clear();
            foreach (var it in keys)
            {
                Add(it);
            }
        }

        public pos_type getPosType()
        {
            if (items.Any())
            {
                return items.First().Value.type;
            }
            else
            {
                return pos_type.none;
            }
        }

        /// <summary>
        /// Logs a multiline description of the gramCaseSet
        /// </summary>
        /// <param name="log">The log.</param>
        public void ToString(ILogBuilder log, bool expanded = false)
        {
            //StringBuilder sb = new StringBuilder();
            log.AppendLine("Grammatical sets");
            int c = 0;
            foreach (KeyValuePair<string, gramFlags> gf in items)
            {
                log.AppendLine("[" + c + "] " + gf.Key);
                if (expanded)
                {
                    log.AppendLine(gf.Value.ToString(!expanded));
                    log.AppendLine("");
                }
                c++;
            }
        }

        /// <summary>
        /// Returns a string that represents the set in inline format
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            string output = "[";

            foreach (var gf in items)
            {
                output = output + gf.Key;
            }
            return output + "] ";
        }

        /// <summary>
        /// Add gramFlags into the set, defined by the specified declaration. If the same declaration exists it is not added.
        /// </summary>
        /// <param name="declaration">The declaration.</param>
        /// <returns>Created or existing <see cref="gramFlags"/></returns>
        public gramFlags Add(string declaration, gramFlags gram = null)
        {
            if (!items.ContainsKey(declaration))
            {
                if (gram == null)
                {
                    gramFlags tmp = new gramFlags();
                    tmp.SetAll(declaration);
                    gram = tmp;
                }
                items.Add(declaration, gram);
            }

            return items[declaration];
        }

        public void Add(object gram)
        {
            if (gram is gramFlags)
            {
                gramFlags gramFlags_gramIObjectWithName = (gramFlags)gram;
                string declaration = gramFlags_gramIObjectWithName.GetAll();
                Add(declaration, gramFlags_gramIObjectWithName);
            }
        }

        /// <summary>
        ///
        /// </summary>
        protected Dictionary<string, gramFlags> items { get; set; } = new Dictionary<string, gramFlags>();

        IEnumerator<KeyValuePair<string, gramFlags>> IEnumerable<KeyValuePair<string, gramFlags>>.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, gramFlags>>)items).GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, gramFlags>>)items).GetEnumerator();
        }
    }
}