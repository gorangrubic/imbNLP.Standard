// --------------------------------------------------------------------------------------------------------------------
// <copyright file="htmlContentToken.cs" company="imbVeles" >
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
    using imbSCI.Core.attributes;
    using imbSCI.DataComplex;
    using System;
    using System.Collections.Generic;

    #endregion imbVELES USING

    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="contentToken" />
    /// <seealso cref="IHtmlContentElement" />
    public class htmlContentToken : contentToken, IHtmlContentElement, IWeightTableTerm
    {
        /// <summary>
        /// if this HtmlToken is contained inside <c>a</c> tag - this property will return <see cref="IHtmlContentElement"/>. If it is not part of <c>a</c> tag it will return <c>null</c>
        /// </summary>
        /// <value>
        /// Returns parent that is associated to <c>a</c> htmltag
        /// </value>
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

        public bool isMatch(IWeightTableTerm other)
        {
            throw new NotImplementedException();
        }

        public void Define(string __name, string __nominalForm)
        {
            name = __name;
            // nominalForm = __nominalForm;
        }

        //public List<string> GetAllForms()
        //{
        //    return new List<String>() { content };
        //}

        public List<string> GetAllForms(bool includingNominalForm = true)
        {
            if (includingNominalForm)
            {
                return new List<string>() { content };
            }
            else
            {
                return new List<string>();
            }
        }

        public void SetOtherForms(IEnumerable<string> instances)
        {
            throw new NotImplementedException();
        }

        #endregion -- FLAGGING OVERRIDE

        #region Implementation of IHtmlContentElement

        public HtmlNode htmlNode { get; set; }

        [imb(imbAttributeName.xmlEntityOutput)]
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

        [imb(imbAttributeName.xmlEntityOutput)]
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

        [imb(imbAttributeName.xmlEntityOutput)]
        public string htmlId
        {
            get
            {
                if (htmlNode == null) return "";
                return htmlNode.Id;
            }
        }

        [imb(imbAttributeName.xmlEntityOutput)]
        public string htmlClass
        {
            get
            {
                if (htmlNode == null) return "";
                return htmlNode.GetAttributeValue("class", "");
                //throw new NotImplementedException();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public int AFreqPoints { get; set; } = 1;

        /// <summary>
        ///
        /// </summary>
        public double weight { get; set; } = 0;

        public string nominalForm
        {
            get
            {
                return content;
            }
        }

        int IWeightTableTerm.Count
        {
            get
            {
                return 1;
            }
        }

        #endregion Implementation of IHtmlContentElement
    }
}