// --------------------------------------------------------------------------------------------------------------------
// <copyright file="blokCategorization.cs" company="imbVeles" >
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
namespace imbNLP.Core.contentStructure.categorization
{
    #region imbVELES USING

    using imbNLP.Core.contentStructure.interafaces;
    using imbNLP.Core.contentStructure.tokenizator;
    using imbNLP.Data.basic;

    #endregion imbVELES USING

    /// <summary>
    /// PREVAZIDJENO --- INTEGRISANO U contentStructure elemente - 2017
    /// </summary>
    internal static class blokCategorization
    {
        //public static nlpSentenceBasicType getBasicType(nlpSentenceGenericType input)
        //{
        //    String name = input.ToString();
        //    if (name.StartsWith("normal")) return nlpSentenceBasicType.normal;
        //    if (name.StartsWith("open")) return nlpSentenceBasicType.open;
        //    if (name.StartsWith("role")) return nlpSentenceBasicType.role;
        //    return nlpSentenceBasicType.unknown;
        //}

        internal static void sentenceAnalysis(IContentPage content, nlpTokenizatorSettings settings,
                                              basicLanguage language)
        {
            return;
            /*
            foreach (IContentSentence sentence in content.sentences)
            {
                if (sentence.genericType == nlpSentenceGenericType.unknown)
                {
                    String spliter = sentence.spliter.Trim();

                    Boolean firstCaseOk = (sentence.items.First().letterCase == nlpTextCase.firstUpperRestLower);

                    var prevSentence = sentence.prev as IContentSentence;
                    if (prevSentence == null) prevSentence = sentence;

                    if (prevSentence.genericType == nlpSentenceGenericType.list_startSentence)
                    {
                        sentence.genericType = nlpSentenceGenericType.list_item;
                    } else
                    {
                        switch (spliter)
                        {
                            case tokenization.sentenceEnd_arrowRight:
                            case tokenization.sentenceEnd_arrowLeft:
                                sentence.genericType = nlpSentenceGenericType.role_title;
                                break;

                            case tokenization.sentenceEnd_notFinished2:
                            case tokenization.sentenceEnd_notFinished:
                                if (firstCaseOk)
                                {
                                    sentence.genericType = nlpSentenceGenericType.normal_unfinished;
                                }
                                break;

                            case tokenization.sentenceEnd_question:
                                if (firstCaseOk)
                                {
                                    sentence.genericType = nlpSentenceGenericType.normal_question;
                                }

                                break;

                            case tokenization.sentenceEnd_normal:
                                if (firstCaseOk)
                                {
                                    sentence.genericType = nlpSentenceGenericType.normal;
                                }
                                break;

                            case tokenization.sentenceEnd_listStart2:
                            case tokenization.sentenceEnd_listStart:
                                sentence.genericType = nlpSentenceGenericType.list_startSentence;
                                break;

                            case tokenization.sentenceEnd_listItemEnd_listEnd:
                            case tokenization.sentenceEnd_listItemEnd:
                                sentence.genericType = nlpSentenceGenericType.list_item;
                                break;

                            case tokenization.sentenceEnd_exclamation:
                                if (firstCaseOk)
                                {
                                    sentence.genericType = nlpSentenceGenericType.normal_exclamation;
                                }
                                break;

                            default:
                                if (prevSentence.genericType == nlpSentenceGenericType.list_item)
                                {
                                    sentence.genericType = nlpSentenceGenericType.list_item;
                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty(spliter))
                                    {
                                        content.note(devNoteType.nlp,
                                                            "Unknown spliter for sentence: [" + spliter +
                                                            "] - add support for it in> tokenization.cs constants and sentenceAnalysis()",
                                                            "blokCategorization");
                                    }
                                }
                                break;
                        }
                    }

                    /*
                    if (sentence.genericType==nlpSentenceGenericType.unknown)
                    {
                        if (firstCaseOk)
                        {
                            sentence.genericType = nlpSentenceGenericType.normal_unknown;
                        } else
                        {
                            if (sentence.items.All(x => x.letterCase == nlpTextCase.upperCase))
                            {
                                sentence.genericType = nlpSentenceGenericType.role_title;
                            } else
                            {
                                sentence.genericType = nlpSentenceGenericType.role_simpleText;
                            }
                        }
                    }
                }
            }
        }*/
        }

        internal static void blockAnalysis(IContentPage content, nlpTokenizatorSettings settings, basicLanguage language)
        {
            if (content.items == null) return;
            foreach (IContentBlock block in content.items)
            {
            }
        }

        /// <summary>
        /// Vrsi analizu paragrafa - za svaki paragraf unfreeze kolekciju recenica, pokrenuti obavezno posle kategorizacije recenica
        /// </summary>
        /// <param name="content"></param>
        /// <param name="settings"></param>
        /// <param name="language"></param>
        internal static void paragraphAnalysis(IContentPage content, nlpTokenizatorSettings settings,
                                               basicLanguage language)
        {
            if (content.paragraphs == null) return;
            /*
            foreach (IContentParagraph paragraph in content.paragraphs)
            {
                //paragraph.items.unfreeze();
                var firstSentence = paragraph.items.First();

                if (paragraph.items.Count == 1)
                {
                    switch (firstSentence.genericType)
                    {
                        case nlpSentenceGenericType.normal:
                        case nlpSentenceGenericType.normal_exclamation:
                        case nlpSentenceGenericType.normal_question:
                        case nlpSentenceGenericType.normal_unfinished:
                        case nlpSentenceGenericType.normal_unknown:
                            paragraph.genericType = nlpParagraphGenericType.textual_single;
                            break;

                        case nlpSentenceGenericType.role_title:
                            paragraph.genericType = nlpParagraphGenericType.textual_title;
                            break;

                        case nlpSentenceGenericType.role_simpleText:
                            paragraph.genericType = nlpParagraphGenericType.data_single;
                            break;

                        default:
                            paragraph.genericType = nlpParagraphGenericType.data_single;
                            break;
                    }
                } else
                {
                    //var stats = paragraph.items.getRankedStats(false);

                    //nlpSentenceBasicType first = stats.First().Key.convertToBasicEnum<nlpSentenceBasicType>();

                    nlpSentenceBasicType first = firstSentence.basicType;

                    switch (first)
                    {
                        case nlpSentenceBasicType.normal:
                            switch (firstSentence.genericType)
                            {
                                case nlpSentenceGenericType.role_simpleText:
                                case nlpSentenceGenericType.normal_unknown:
                                case nlpSentenceGenericType.role_title:
                                    paragraph.genericType = nlpParagraphGenericType.textual_article;
                                    break;

                                default:
                                    paragraph.genericType = nlpParagraphGenericType.textual;
                                    break;
                            }
                            break;

                        case nlpSentenceBasicType.role:
                            paragraph.genericType = nlpParagraphGenericType.data_simple;
                            break;

                        default:
                        case nlpSentenceBasicType.unknown:
                            paragraph.genericType = nlpParagraphGenericType.unknown;
                            break;

                        case nlpSentenceBasicType.list:
                            paragraph.genericType = nlpParagraphGenericType.data_listed;
                            break;
                    }
                }
            }*/
        }
    }
}