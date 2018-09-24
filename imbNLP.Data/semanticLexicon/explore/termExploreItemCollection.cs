// --------------------------------------------------------------------------------------------------------------------
// <copyright file="termExploreItemCollection.cs" company="imbVeles" >
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
namespace imbNLP.Data.semanticLexicon.explore
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class termExploreItemCollection : IList<termExploreItem>
    {
        public bool Contains(string term)
        {
            if (items.Any(x => x.inputForm == term))
            {
                return true;
            }
            return false;
        }

        /// <summary> </summary>
        protected List<termExploreItem> items { get; set; } = new List<termExploreItem>();

        public int Count
        {
            get
            {
                return ((IList<termExploreItem>)items).Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IList<termExploreItem>)items).IsReadOnly;
            }
        }

        public termExploreItem this[int index]
        {
            get
            {
                return ((IList<termExploreItem>)items)[index];
            }

            set
            {
                ((IList<termExploreItem>)items)[index] = value;
            }
        }

        public termExploreItem this[string instanceForm]
        {
            get
            {
                if (items.Any(x => x.inputForm == instanceForm))
                {
                    return items.First(x => x.inputForm == instanceForm);
                }
                return null;
            }
        }

        /// <summary>
        /// Adds the specified instance form, or if already exists just adds gram declaration
        /// </summary>
        /// <param name="instanceForm">The instance form.</param>
        /// <param name="declaration">The declaration.</param>
        /// <returns></returns>
        public termExploreItem Add(string instanceForm, string declaration)
        {
            if (!items.Any(x => x.inputForm == instanceForm))
            {
                termExploreItem tmp = new termExploreItem(instanceForm);
                items.Add(tmp);
            }

            this[instanceForm].gramSet.Add(declaration);

            return this[instanceForm];
        }

        public void Add(string inputString)
        {
            termExploreItem tei = new termExploreItem(inputString);
            Add(tei);
        }

        public void Add(termExploreItem tei)
        {
            if (!this.Any(x => x.inputForm.Equals(tei.inputForm, StringComparison.CurrentCultureIgnoreCase)))
            {
                items.Add(tei);
            }
        }

        public int IndexOf(termExploreItem item)
        {
            return ((IList<termExploreItem>)items).IndexOf(item);
        }

        public void Insert(int index, termExploreItem item)
        {
            ((IList<termExploreItem>)items).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<termExploreItem>)items).RemoveAt(index);
        }

        public void Clear()
        {
            ((IList<termExploreItem>)items).Clear();
        }

        public bool Contains(termExploreItem item)
        {
            return ((IList<termExploreItem>)items).Contains(item);
        }

        public void CopyTo(termExploreItem[] array, int arrayIndex)
        {
            ((IList<termExploreItem>)items).CopyTo(array, arrayIndex);
        }

        public bool Remove(termExploreItem item)
        {
            return ((IList<termExploreItem>)items).Remove(item);
        }

        public IEnumerator<termExploreItem> GetEnumerator()
        {
            return ((IList<termExploreItem>)items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<termExploreItem>)items).GetEnumerator();
        }

        public termExploreItemCollection()
        {
        }
    }
}