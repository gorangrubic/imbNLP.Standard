// --------------------------------------------------------------------------------------------------------------------
// <copyright file="htmlContentSubSentence.cs" company="imbVeles" >
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
    #region imbVELES USING

    using HtmlAgilityPack;
    using imbNLP.Core.contentStructure.elements;
    using imbNLP.Core.contentStructure.interafaces;
    using imbSCI.Core.attributes;

    #endregion imbVELES USING

    public class htmlContentSubSentence : contentSubSentence, IHtmlContentElement, IContentSubSentence
    {
        /// <summary>
        /// Gets the link root parent.
        /// </summary>
        /// <value>
        /// The link root parent.
        /// </value>
        public IHtmlContentElement linkRootParent
        {
            get
            {
                return this.linkRootParent();
            }
        }

        #region Implementation of IHtmlContentElement

        public HtmlNode htmlNode { get; set; }

        [imb(imbAttributeName.xmlEntityOutput)]
        public string htmlTag
        {
            get
            {
                if (htmlNode == null) return "";
                return htmlNode.Name;
            }
        }

        public string xpath
        {
            get
            {
                if (htmlNode == null) return "";
                return htmlNode.XPath;
            }
        }

        public string htmlId
        {
            get
            {
                if (htmlNode == null) return "";
                return htmlNode.Id;
            }
        }

        public string htmlClass
        {
            get
            {
                if (htmlNode == null) return "";
                return htmlNode.GetAttributeValue("class", "");
                //throw new NotImplementedException();
            }
        }

        #endregion Implementation of IHtmlContentElement
    }
}