// --------------------------------------------------------------------------------------------------------------------
// <copyright file="htmlContentSentence.cs" company="imbVeles" >
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
    using imbNLP.Data.enums.flags;
    using System.Linq;

    #endregion imbVELES USING

    /// <summary>
    ///
    /// </summary>
    public class htmlContentSentence : contentSentence, IHtmlContentElement
    {
        public IHtmlContentElement linkRootParent
        {
            get
            {
                return this.linkRootParent();
            }
        }

        #region -- FLAGGING OVERRIDE

        /// <summary>
        /// Primarno obelezavanje - od mikro ka makro nivoa
        /// </summary>
        /// <remarks>Obelezavanje se oslanja samo na sopstvene podatke i podatke svojih itema</remarks>
        /// <param name="resources"></param>
        public override void primaryFlaging(params object[] resources)
        {
            base.primaryFlaging(resources);
            // ovde ubaciti algoritam specifican za ovaj tip

            if (items.Count == 1)
            {
                IHtmlContentElement child = items.First() as IHtmlContentElement;
                if (child != null)
                {
                    if (child.htmlTag == "a")
                        sentenceFlags |= contentSentenceFlag.navigationContainer;
                }
            }

            if (htmlTag == "a") sentenceFlags |= contentSentenceFlag.navigationLink;

            if (parent is IHtmlContentElement)
            {
                IHtmlContentElement parent_IHtmlContentElement = (IHtmlContentElement)parent;
                if (parent_IHtmlContentElement.htmlTag == "a")
                    sentenceFlags |= contentSentenceFlag.titleForLink;
            }

            //flags.Add(contentStructure.flags.contentTokenFlag.)
        }

        /// <summary>
        /// Sekundarno obelezavanje -- od makro ka mikro novou
        /// </summary>
        /// <remarks>Obelezavanje se oslanja na podatke svog parenta i njegovog okruzenja</remarks>
        /// <param name="resources"></param>
        public override void secondaryFlaging(params object[] resources)
        {
            base.secondaryFlaging(resources);
            // ovde ubaciti algoritam specifican za ovaj tip
        }

        /// <summary>
        /// Sprovodi proces izvodjenja generalne semantike
        /// </summary>
        /// <param name="resources"></param>
        public override void generalSemanticsFlaging(params object[] resources)
        {
            base.generalSemanticsFlaging(resources);
            // ovde ubaciti algoritam specifican za ovaj tip
        }

        /// <summary>
        /// Izvodi specijalnu semantiku> u skladu sa zadatkom analize pravi triplete
        /// </summary>
        /// <param name="resources"></param>
        public override void specialSematicsFlaging(params object[] resources)
        {
            base.specialSematicsFlaging(resources);
            // ovde ubaciti algoritam specifican za ovaj tip
        }

        #endregion -- FLAGGING OVERRIDE

        public htmlContentSentence(HtmlNode __node, string __content)
        {
            htmlNode = __node;
            content = __content;
            sourceContent = __content;
        }

        #region IHtmlContentElement Members

        public HtmlNode htmlNode { get; set; }

        public string htmlTag
        {
            get
            {
                if (htmlNode != null)
                {
                    return htmlNode.Name;
                }
                return "";
            }
        }

        public string xpath
        {
            get
            {
                if (htmlNode != null)
                {
                    return htmlNode.XPath;
                }
                return "";
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

        #endregion IHtmlContentElement Members
    }
}