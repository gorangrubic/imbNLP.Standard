// --------------------------------------------------------------------------------------------------------------------
// <copyright file="htmlLinkNodeCollection.cs" company="imbVeles" >
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
namespace imbNLP.Core.contentStructureHtml.elements
{
    using imbNLP.Core.contentStructure.collections;
    using imbSCI.Core.extensions.data;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{imbSemanticEngine.contentStructureHtml.elements.htmlLinkNode}" />
    public class htmlLinkNodeCollection : List<htmlLinkNode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="htmlLinkNodeCollection"/> class.
        /// </summary>
        /// <param name="tkns">The TKNS.</param>
        public htmlLinkNodeCollection(contentTokenCollection tkns)
        {
            foreach (IHtmlContentElement tkn in tkns)
            {
                if (!scoped.Contains(tkn))
                {
                    var lnk = tkn.linkRootParent;
                    if (lnk != null)
                    {
                        Add(lnk);
                    }
                }
            }
        }

        public int Compare(htmlLinkNode a, htmlLinkNode b)
        {
            return a.score.CompareTo(b.score);
        }

        public IEnumerable<htmlLinkNode> getSorted(int take = -1)
        {
            Sort(Compare);
            if (take == -1) return this;
            return this.Take<htmlLinkNode>(take);
        }

        /// <summary>
        ///
        /// </summary>
        public Dictionary<string, htmlLinkNode> byUrl { get; set; } = new Dictionary<string, htmlLinkNode>();

        /// <summary>
        ///
        /// </summary>
        public htmlTokenList scoped { get; set; } = new htmlTokenList();

        public new htmlLinkNode Add(IHtmlContentElement rootLink)
        {
            htmlLinkNode tmp = new htmlLinkNode(rootLink);
            Add(tmp);
            scoped.AddMulti(tmp.scoped);
            if (!byUrl.ContainsKey(tmp.url))
            {
                byUrl.Add(tmp.url, tmp);
            }
            return tmp;
        }
    }
}