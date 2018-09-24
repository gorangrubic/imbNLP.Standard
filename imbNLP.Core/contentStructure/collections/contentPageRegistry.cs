// --------------------------------------------------------------------------------------------------------------------
// <copyright file="contentPageRegistry.cs" company="imbVeles" >
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
// Project: imbNLP.Core
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
namespace imbNLP.Core.contentStructure.collections
{
    using imbNLP.Core.contentStructure.interafaces;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public class contentPageRegistry
    {
        public bool ContainsKey(string key) => items.ContainsKey(key);

        public IEnumerator<KeyValuePair<string, IContentPage>> GetEnumerator() => items.GetEnumerator();

        public int Count() => items.Count;

        public IContentPage Get(string key, bool autocreate)
        {
            if (!allowUseExisting) return null;
            if (!items.ContainsKey(key)) items.TryAdd(key, null);
            return items[key];
        }

        public void Add(string key, IContentPage item)
        {
            if (!allowUseExisting) return;
            if (!items.ContainsKey(key)) items.TryAdd(key, item);
        }

        /// <summary>
        ///
        /// </summary>
        protected ConcurrentDictionary<string, IContentPage> items { get; set; } = new ConcurrentDictionary<string, IContentPage>();

        public bool allowUseExisting { get; set; }
    }
}