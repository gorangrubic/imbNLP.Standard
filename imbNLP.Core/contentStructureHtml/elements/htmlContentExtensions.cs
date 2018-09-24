// --------------------------------------------------------------------------------------------------------------------
// <copyright file="htmlContentExtensions.cs" company="imbVeles" >
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
    using imbSCI.Core.extensions.data;
    using imbSCI.Data.enums.reporting;
    using System.Collections.Generic;

    public static class htmlContentExtensions
    {
        public static List<IHtmlContentElement> getChildern(this IHtmlContentElement item, List<IHtmlContentElement> input, int index = 5)
        {
            List<IHtmlContentElement> output = new List<IHtmlContentElement>();
            if (input == null) input = new List<IHtmlContentElement>();

            foreach (IHtmlContentElement ch in input)
            {
                output.AddMulti(ch.items);
            }

            if (index > 0)
            {
                if (item.items.Count > 0) output.AddMulti(item.getChildern(output, index - 1));
            }

            output.AddMulti(input);

            return output;
        }

        /// <summary>
        /// Returns parent that has link tag in self
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static IHtmlContentElement linkRootParent(this IHtmlContentElement item)
        {
            if (item.parent is IHtmlContentElement)
            {
                IHtmlContentElement parent_IHtmlContentElement = (IHtmlContentElement)item.parent;
                if (parent_IHtmlContentElement == null)
                {
                    return null;
                }
                else if (parent_IHtmlContentElement.htmlTag.ToLower() == htmlTagName.a.ToString().ToLower())
                {
                    return parent_IHtmlContentElement;
                }
                else
                {
                    return parent_IHtmlContentElement.linkRootParent;
                }
            }
            else
            {
                return null;
            }
        }
    }
}