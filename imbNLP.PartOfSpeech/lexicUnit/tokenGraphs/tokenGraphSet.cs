// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tokenGraphSet.cs" company="imbVeles" >
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
namespace imbNLP.PartOfSpeech.lexicUnit.tokenGraphs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IDictionary{System.String, imbLanguageFramework.extended.tokenGraphs.tokenGraph}" />
    public class tokenGraphSet : IDictionary<String, tokenGraph>
    {
        private Dictionary<String, tokenGraph> _items = new Dictionary<String, tokenGraph>();

        public void Add(IEnumerable<KeyValuePair<String, String>> source, tokenGraphNodeType typeForValue)
        {
            foreach (var pair in source)
            {
                if (!ContainsKey(pair.Key))
                {
                    tokenGraph tg = new tokenGraph(pair.Key);
                    Add(pair.Key, tg);
                }

                this[pair.Key].Add(pair.Value, typeForValue);
            }
        }

        public tokenGraph this[string key]
        {
            get
            {
                return ((IDictionary<string, tokenGraph>)items)[key];
            }

            set
            {
                ((IDictionary<string, tokenGraph>)items)[key] = value;
            }
        }

        public int Count
        {
            get
            {
                return ((IDictionary<string, tokenGraph>)items).Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IDictionary<string, tokenGraph>)items).IsReadOnly;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return ((IDictionary<string, tokenGraph>)items).Keys;
            }
        }

        public ICollection<tokenGraph> Values
        {
            get
            {
                return ((IDictionary<string, tokenGraph>)items).Values;
            }
        }

        /// <summary>
        ///
        /// </summary>
        protected Dictionary<String, tokenGraph> items
        {
            get
            {
                //if (_items == null)_items = new Dictionary<String, tokenGraph>();
                return _items;
            }
            set { _items = value; }
        }

        public void Add(KeyValuePair<string, tokenGraph> item)
        {
            ((IDictionary<string, tokenGraph>)items).Add(item);
        }

        public void Add(string key, tokenGraph value)
        {
            ((IDictionary<string, tokenGraph>)items).Add(key, value);
        }

        public void Clear()
        {
            ((IDictionary<string, tokenGraph>)items).Clear();
        }

        public bool Contains(KeyValuePair<string, tokenGraph> item)
        {
            return ((IDictionary<string, tokenGraph>)items).Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return ((IDictionary<string, tokenGraph>)items).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, tokenGraph>[] array, int arrayIndex)
        {
            ((IDictionary<string, tokenGraph>)items).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, tokenGraph>> GetEnumerator()
        {
            return ((IDictionary<string, tokenGraph>)items).GetEnumerator();
        }

        public bool Remove(KeyValuePair<string, tokenGraph> item)
        {
            return ((IDictionary<string, tokenGraph>)items).Remove(item);
        }

        public bool Remove(string key)
        {
            return ((IDictionary<string, tokenGraph>)items).Remove(key);
        }

        public bool TryGetValue(string key, out tokenGraph value)
        {
            return ((IDictionary<string, tokenGraph>)items).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<string, tokenGraph>)items).GetEnumerator();
        }

        public static implicit operator tokenGraph(tokenGraphSet gs)
        {
            return gs.items[gs.items.Keys.First()]; // .First().Value;
        }
    }
}