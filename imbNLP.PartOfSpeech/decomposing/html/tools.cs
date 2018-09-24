// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tools.cs" company="imbVeles" >
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
using HtmlAgilityPack;
using imbCommonModels.contentBlock;
using imbCommonModels.structure;
using imbMiningContext.MCWebPage;
using imbSCI.Core.math;
using imbSCI.Data;
using imbSCI.Data.collection.graph;
using imbSCI.DataComplex.extensions.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace imbNLP.PartOfSpeech.decomposing.html
{
    public static class tools
    {
        /// <summary>
        /// Gets the textual extract.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static String GetTextualExtract(this imbMCWebPage source)
        {
            String extract = source.TextContent.Replace(Environment.NewLine, "").Replace(" ", "").ToLower();
            return extract;
        }

        /// <summary>
        /// Gets the unique pages.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="orderBySize">if set to <c>true</c> [order by size].</param>
        /// <returns></returns>
        public static List<imbMCWebPage> GetUniquePages(this IEnumerable<imbMCWebPage> source)
        {
            List<imbMCWebPage> output = new List<imbMCWebPage>();

            List<String> hashes = new List<string>();

            foreach (imbMCWebPage page in source)
            {
                String extr = md5.GetMd5Hash(page.GetTextualExtract());

                if (!hashes.Contains(extr))
                {
                    hashes.Add(extr);
                    output.Add(page);
                }
            }

            return output;
        }

        /// <summary>
        /// Gets the tag and its parent nodes names, until the limit is reached
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="limit">The limit.</param>
        /// <returns></returns>
        public static List<String> GetTagNames(this HtmlNode node, Int32 limit = 5)
        {
            List<String> output = new List<string>();

            HtmlNode head = node;
            Int32 i = 0;

            while (head != null)
            {
                output.Add(head.OriginalName.ToLower());
                head = head.ParentNode;

                i++;

                if (i > limit)
                {
                    break;
                }
            }

            return output;
        }

        /// <summary>
        /// Da li je ovaj Tag dozvoljen?
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal static bool isTagAcceptable(HtmlNode node, node page = null, Boolean allowTitle = false, Boolean allowMeta = false)
        {
            string xPathOfTag = node.XPath;
            if (!node.Name.isNullOrEmptyString())
            {
                if (node.Name.Contains("comment"))
                {
                    return false;
                }
            }
            if (xPathOfTag.Contains("title")) return allowTitle;
            if (xPathOfTag.Contains("meta")) return allowMeta;
            if (xPathOfTag.Contains("style")) return false;
            if (xPathOfTag.Contains("script")) return false;
            if (xPathOfTag.Contains("link")) return false;
            if (xPathOfTag.Contains("#comment")) return false;
            if (xPathOfTag.Contains("#script")) return false;
            if (node.Name.StartsWith("a"))
            {
                string href = node.GetAttributeValue("href", "");
                string key = node.Name.ToString() + "[" + href + "]";
                if (page != null)
                {
                    return true;
                    //if (page.links.byScope[linkScope.outer].Any(x => x.Key == key))
                    //{
                    //    return false;
                    //}
                    //else
                    //{
                    //    return true;
                    //}
                }
                //lnk
            }
            return true;
        }

        internal static bool isNodeAcceptable(this HtmlNode node)
        {
            if (node == null) return false;

            switch (node.NodeType.ToString().ToLower())
            {
                //case "img":
                case "script":
                case "style":
                case "br":

                    return false;
                    break;

                default:
                    return true;
                    break;
            }
        }

        /// <summary>
        /// Regex select sentenceTerminator : ([\.;!\?]+)$
        /// </summary>
        /// <remarks>
        /// <para>Selektuje karaktere koji predstavljaju kraj recenice</para>
        /// <para></para>
        /// </remarks>
        public static Regex _select_sentenceTerminator = new Regex(@"([\.;!\?]+)$", RegexOptions.Compiled);

        /// <summary>
        /// Regex select sentenceSpliter : (?=[\.;!\?])\s*(?=[A-ZČŠĆŽĐ\d])
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_sentenceSpliter = new Regex(@"(?<=[\.;!\?])\s*(?=[A-ZČŠĆŽĐ\d])",
                                                                RegexOptions.Compiled);

        /// <summary>
        /// Interni algoritam za razbijanje na recenice
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static List<string> splitContentToSentences(string input)
        {
            List<string> inputSentences = new List<string>();

            if (_select_sentenceSpliter.IsMatch(input))
            {
                // ima vise recenica
                var _ins = _select_sentenceSpliter.Split(input).ToList();
                _ins.ForEach(x => inputSentences.Add(x.Trim()));
            }
            else
            {
                // postoji samo jedna recenica
                inputSentences.Add(input.Trim());
            }
            return inputSentences;
        }

        public static bool checkTextHtmlConsistensy(this HtmlNode htmlNode)
        {
            if (htmlNode.InnerHtml != htmlNode.InnerText)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Builds the content tree out of <see cref="HtmlDocument"/>
        /// </summary>
        /// <param name="htmlDoc">The HTML document</param>
        /// <param name="__name">The name of the root</param>
        /// <returns></returns>
        public static nodeTree buildTree(this HtmlDocument htmlDoc, string __name, Boolean allowTitle = false, Boolean allowMeta = false)
        {
            nodeTree output = new nodeTree(__name, htmlDoc);

            XPathNodeIterator iterator = htmlDoc.CreateNavigator().Select("//*[text()][count(*)=0]");

            //XPathNodeIterator iterator = htmlDoc.CreateNavigator().SelectDescendants(System.Xml.XPath.XPathNodeType.Text, false);
            while (iterator.MoveNext())
            {
                XPathNavigator current = iterator.Current;
                HtmlNodeNavigator htmlNavigator = current as HtmlNodeNavigator;
                string sp = htmlNavigator.CurrentNode.XPath.Replace("/", "\\");
                string cn = "";

                if (htmlNavigator.CurrentNode.Name.ToLower() == "title")
                {
                }

                if (isTagAcceptable(htmlNavigator.CurrentNode, null, allowTitle, allowMeta))
                {
                    cn = "";
                    cn = htmlNavigator.CurrentNode.InnerText;

                    //if (htmlNavigator.CurrentNode.checkTextHtmlConsistensy())
                    //{
                    //}

                    cn = cn.htmlContentProcess().Trim();
                    if (!cn.isNullOrEmptyString())
                    {
                        graphWrapNode<htmlWrapper> nn = output.Add(sp, htmlNavigator.CurrentNode.Clone());
                        nn.item.content = cn;
                        nn.item.xPath = sp;
                        nn.item.path = nn.path;
                        // nodesWithText.AddNewLeaf(sp, htmlNavigator.CurrentNode.Clone(), report, cn);
                    }
                    else
                    {
                    }
                }
                else
                {
                }
            }

            return output;
        }
    }
}