// --------------------------------------------------------------------------------------------------------------------
// <copyright file="grammaticTagCollection.cs" company="imbVeles" >
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
namespace imbNLP.PartOfSpeech.resourceProviders.core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Collection of POS enumeration flags, accessible by Type of Enum.
    /// </summary>
    /// <remarks>
    /// <para>The tag collection is new imbNLP API structure for morphosyntactic context of a token</para>
    /// <para>The collection is not designed to be serialized, it should be converted to string by a <see cref="resourceConverterForGramaticTags"/> instance before saved.</para>
    /// </remarks>
    [Serializable]
    public class grammaticTagCollection
    {
        public Boolean HasError { get; set; } = false;
        public String comment { get; set; } = "";

        public grammaticTagCollection()
        {
        }

        /// <summary>
        /// Returns flag value for the specified <c>T</c>
        /// </summary>
        /// <typeparam name="T">Enum type that we look for</typeparam>
        /// <returns>Typed enum flag, if exists</returns>
        public T Get<T>(T defOutput) where T : IConvertible
        {
            if (items.ContainsKey(typeof(T)))
            {
                Object output = items[typeof(T)];
                if (output == null) return defOutput;
                return (T)output;
            }
            return defOutput;
        }

        /// <summary>
        /// Gets the flag for specified <c>t</c> <see cref="Type"/>
        /// </summary>
        /// <param name="t">The type to get flag for</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">t</exception>
        public object Get(Type t)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t));
            }
            if (items.ContainsKey(t))
            {
                object o = null;
                if (items.TryGetValue(t, out o))
                {
                    return o;
                }
            }
            return null;
        }

        /// <summary>
        /// Adds the specified <c>tag</c> to the collection
        /// </summary>
        /// <param name="tag">The tag to be added into collection</param>
        /// <exception cref="ArgumentNullException">tag</exception>
        public void Add(object tag)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));
            Type t = tag.GetType();

            if (items.ContainsKey(t))
            {
                items[t] = tag;
            }
            else
            {
                items.GetOrAdd(t, tag);
            }
        }

        /// <summary>
        /// Returns a string representation of the tags contained
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public String ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<Type, Object> tg in items)
            {
                sb.Append(tg.Key.Name);
                sb.Append(".");
                sb.Append(tg.Value.ToString());
                sb.Append(";");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns list with all tags, with distinctive <see cref="Type"/>. Optionally: returns only tags for types that are specified by <c>forTypes</c>
        /// </summary>
        /// <param name="forTypes">If specified, returns only tags for types that are specified</param>
        /// <returns>List with tags</returns>
        public List<object> GetTags(IEnumerable<Type> forTypes = null)
        {
            if (forTypes == null)
            {
                return items.Values.ToList();
            }
            else
            {
                var output = new List<object>();
                foreach (Type t in forTypes)
                {
                    var o = Get(t);
                    if (o != null) output.Add(o);
                }
                return output;
            }
        }

        /// <summary>
        /// Grammatic tags by POS enumeration type
        /// </summary>
        protected ConcurrentDictionary<Type, object> items { get; set; } = new ConcurrentDictionary<Type, object>();
    }
}