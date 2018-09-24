// --------------------------------------------------------------------------------------------------------------------
// <copyright file="htmlContentParagraph.cs" company="imbVeles" >
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
    using imbNLP.Core.textRetrive;
    using imbSCI.Core.attributes;

    #endregion imbVELES USING

    public class htmlContentParagraph : contentParagraph, IHtmlContentElement
    {
        #region -- FLAGGING OVERRIDE

        /// <summary>
        /// Primarno obelezavanje - od mikro ka makro nivoa
        /// </summary>
        /// <remarks>Obelezavanje se oslanja samo na sopstvene podatke i podatke svojih itema</remarks>
        /// <param name="resources"></param>
        public override void primaryFlaging(params object[] resources)
        {
            base.primaryFlaging(resources);

            //if (
            //    items.TrueForAll(
            //        x =>
            //        x.sentenceFlags.ContainsOneOrMore(contentSentenceFlag.navigationLink,
            //                                          contentSentenceFlag.titleForLink,
            //                                          contentSentenceFlag.navigationContainer)))
            //{
            //    flags |= contentParagraphFlag.navigation;
            //}

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

        #endregion -- FLAGGING OVERRIDE

        #region --- xpath ------- Bindable property

        // private String _xpath;

        public string htmlTag
        {
            get
            {
                if (htmlNode == null) return "";
                return htmlNode.Name;
            }
        }

        /// <summary>
        /// Bindable property
        /// </summary>
        public string xpath
        {
            get
            {
                if (htmlNode == null) return "";
                return htmlNode.XPath;
            }
            //set
            //{
            //    _xpath = value;
            //    OnPropertyChanged("xpath");
            //}
        }

        [imb(imbAttributeName.xmlEntityOutput, "htmlId")]
        public string htmlId
        {
            get
            {
                if (htmlNode == null) return "";
                return htmlNode.Id;
            }
        }

        [imb(imbAttributeName.xmlEntityOutput, "htmlClass")]
        public string htmlClass
        {
            get
            {
                if (htmlNode == null) return "";
                return htmlNode.GetAttributeValue("class", "");
                //throw new NotImplementedException();
            }
        }

        #endregion --- xpath ------- Bindable property

        public htmlContentParagraph() : base()
        {
        }

        /*
        public htmlContentParagraph():base()
        {
            htmlNode = leaf.value as HtmlNode;
            treeNode = leaf;

            content = htmlNode.InnerText; // extractContent(); // htmlNode.InnerHtml;
            sourceContent = htmlNode.InnerHtml;

            //content = __node.InnerText;
            //xpath = __node.XPath;

            //XPathNavigator nav = __node.CreateNavigator();
            //sourceContent = __node.InnerHtml;
            //content = nav.retriveText(htmlContentParagraph.textSetup);

            //imbStringBuilder output = new imbStringBuilder();
            //XPathNodeIterator itr = nav.SelectDescendants(XPathNodeType.Text, true);
            //while (itr.MoveNext())
            //{
            //    switch (itr.Current.NodeType)
            //    {
            //        case XPathNodeType.Text:
            //            String inner = itr.Current.Value;
            //            if (!String.IsNullOrEmpty(inner))
            //            {
            //                var subNav = itr.Current.CreateNavigator();

            //                if (subNav.MoveToParent())
            //                {
            //                    if (subNav.checkNode(settings))
            //                        output.AppendLine(deploySpacing(inner, subNav, settings));
            //                }
            //                else
            //                {
            //                    if (subNav.checkNode(settings)) output.AppendLine(inner);
            //                }
            //            }
            //            break;
            //        default:
            //            break;
            //    }
            //}
            //content = output.ToString();
        }*/

        #region Implementation of IHtmlContentElement

        /// <summary>
        /// HTML node od koga se formira element
        /// </summary>
        public HtmlNode htmlNode { get; set; }

        //#region -----------  treeNode  -------  [imbTreeNode objekat koji predstavlja izvor ovog paragrafa]

        //private imbTreeNode _treeNode; // = new imbTreeNode();

        ///// <summary>
        ///// imbTreeNode objekat koji predstavlja izvor ovog paragrafa
        ///// </summary>
        //// [XmlIgnore]
        //[Category("htmlContentParagraph")]
        //[DisplayName("treeNode")]
        //[Description("imbTreeNode objekat koji predstavlja izvor ovog paragrafa")]
        //public imbTreeNode treeNode
        //{
        //    get { return _treeNode; }
        //    set
        //    {
        //        // Boolean chg = (_treeNode != value);
        //        _treeNode = value;
        //        OnPropertyChanged("treeNode");
        //        // if (chg) {}
        //    }
        //}

        //#endregion

        #endregion Implementation of IHtmlContentElement

        public static textRetriveSetup textSetup
        {
            get
            {
                var _set = new textRetriveSetup();
                _set.doCompressNewLines = false;
                return _set;
            }
        }

        public IHtmlContentElement linkRootParent
        {
            get
            {
                return this.linkRootParent();
            }
        }
    }
}