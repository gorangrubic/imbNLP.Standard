// --------------------------------------------------------------------------------------------------------------------
// <copyright file="htmlNodeProcessing.cs" company="imbVeles" >
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
namespace imbNLP.Core.contentStructureHtml.tokenizator
{
    #region imbVELES USING

    using HtmlAgilityPack;
    using imbCommonModels.structure;
    using imbNLP.Core.contentPreprocess;
    using imbNLP.Core.contentStructure.collections;
    using imbNLP.Core.contentStructure.interafaces;
    using imbNLP.Core.contentStructureHtml.elements;
    using imbNLP.Data.enums.flags;
    using imbSCI.Core.extensions.data;
    using imbSCI.Data;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    #endregion imbVELES USING

    /// <summary>
    /// Ekstenzije koje omogućavaju rad sa HtmlNode objektima u kontekstu htmlTokenizacije
    /// </summary>
    public static class htmlNodeProcessing
    {
        public static IEnumerable<string> extractContent(this IList<IContentParagraph> source)
        {
            List<string> output = new List<string>();
            var en = source.GetEnumerator();
            do
            {
                if (en.Current == null) return output;
                output.Add(en.Current.content);
            } while (en.MoveNext());

            return output;
        }

        public static IEnumerable<string> extractContent(this IList<IContentSentence> source)
        {
            List<string> output = new List<string>();
            var en = source.GetEnumerator();
            do
            {
                if (en.Current == null) return output;
                output.Add(en.Current.content);
            } while (en.MoveNext());

            return output;
        }

        /// <summary>
        /// Extracts the content.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IEnumerable<string> extractContent(this IList<IContentToken> source)
        {
            List<string> output = new List<string>();
            var en = source.GetEnumerator();
            do
            {
                if (en.Current == null) return output;
                output.Add(en.Current.content);
            } while (en.MoveNext());

            return output;
        }

        /// <summary>
        /// Da li je ovaj Tag dozvoljen?
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal static bool isTagAcceptable(HtmlNode node, node page = null)
        {
            string xPathOfTag = node.XPath;
            if (!node.Name.isNullOrEmptyString())
            {
                if (node.Name.Contains("comment"))
                {
                    return false;
                }
            }
            if (xPathOfTag.Contains("title")) return false;
            if (xPathOfTag.Contains("meta")) return false;
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

            switch (node.NodeType.toString().ToLower())
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

        //public static contentElementList tokenizeUrlAndTitle(String url, String title, String description="")
        //{
        //    contentElementList output = new contentStructure.collections.contentElementList();

        //}

        /// <summary>
        /// Pravi rečenice na osnovu HtmlNode-a i vraća kolekciju -- koristi se za glavne rečenice kao i za pod rečenice
        /// </summary>
        /// <param name="htmlNode">The HTML node.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="output">The output.</param>
        /// <param name="preprocessFlags">The preprocess flags.</param>
        /// <param name="flags">The flags.</param>
        /// <returns></returns>
        public static contentTokenCollection createSentencesFromNode(this HtmlNode htmlNode, IHtmlContentElement parent,
                                                                     contentTokenCollection output = null,
                                                                     contentPreprocessFlag preprocessFlags = contentPreprocessFlag.none,
                                                                     sentenceDetectionFlag flags = sentenceDetectionFlag.none)
        {
            if (output == null) output = new contentTokenCollection();
            // if (preprocessFlags == null) preprocessFlags = contentPreprocessFlags.getDefaultFlags();
            //            if (flags == null) flags = sentenceDetectionFlags.getDefaultFlags();

            List<HtmlNode> nodes = new List<HtmlNode>();
            if (htmlNode.HasChildNodes)
            {
                foreach (HtmlNode child in htmlNode.ChildNodes)
                {
                    if (child.isNodeAcceptable()) nodes.Add(child);
                }
            }
            else
            {
                nodes.Add(htmlNode);
            }

            foreach (HtmlNode child in nodes)
            {
                HtmlNode relNode = child;
                if (child.ChildNodes.Count > 0)
                {
                    htmlContentSentence htmlSentence = new htmlContentSentence(child, "");
                    contentTokenCollection subSentences = child.createSentencesFromNode(htmlSentence, null,
                                                                                        preprocessFlags, flags);
                    output.AddRange(subSentences);
                    output.Add(htmlSentence);
                    parent.setItem(htmlSentence);

                    //subSentences.ForEach(x=>htmlSentence.items.Add(x));
                }
                else
                {
                    //if (child.ChildNodes.Count == 1)
                    //{
                    //    relNode = child.FirstChild;
                    //}
                    //if (relNode.NodeType==HtmlNodeType.Text)
                    //{
                    //    relNode = relNode.ParentNode;
                    //}
                    string input = child.InnerText.Trim();

                    if (flags.HasFlag(sentenceDetectionFlag.preprocessParagraphContent))
                        input = preprocess.process(input, preprocessFlags);

                    List<string> inputSentences = splitContentToSentences(input);

                    foreach (string _inputSentece in inputSentences)
                    {
                        if (string.IsNullOrEmpty(_inputSentece))
                        {
                        }
                        else
                        {
                            htmlContentSentence newSentence = new htmlContentSentence(relNode, _inputSentece);
                            if (_select_sentenceTerminator.IsMatch(_inputSentece))
                            {
                                newSentence.sentenceFlags |= contentSentenceFlag.regular;
                                Match m = _select_sentenceTerminator.Match(_inputSentece);
                                if (m.Success)
                                {
                                    newSentence.spliter = m.Value;
                                    newSentence.content = _inputSentece.Substring(0,
                                                                                  _inputSentece.Length -
                                                                                  newSentence.spliter.Length);
                                    newSentence.content = newSentence.content.Trim();
                                }
                            }
                            else
                            {
                                newSentence.sentenceFlags |= contentSentenceFlag.inregular;
                            }
                            output.Add(newSentence);
                            parent.setItem(newSentence);
                        }
                    }
                }
            }

            return output;
        }
    }
}