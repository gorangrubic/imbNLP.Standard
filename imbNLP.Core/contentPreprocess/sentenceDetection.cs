// --------------------------------------------------------------------------------------------------------------------
// <copyright file="sentenceDetection.cs" company="imbVeles" >
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
namespace imbNLP.Core.contentPreprocess
{
    using imbNLP.Core.contentStructure.interafaces;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public static class sentenceDetection
    {
        /// <summary>
        /// Regex select sentenceTerminator : ([\.;!\?]+)$
        /// </summary>
        /// <remarks>
        /// <para>Selektuje karaktere koji predstavljaju kraj recenice</para>
        /// <para></para>
        /// </remarks>
        public static Regex _select_sentenceTerminator = new Regex(@"([\.;!\?]+)$", RegexOptions.Compiled);

        /// <summary>
        /// Regex select sentenceSpliter : (?<=[\.;!\?])\s*(?=[A-ZČŠĆŽĐ\d])
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_sentenceSpliter = new Regex(@"(?<=[\.;!\?])\s*(?=[A-ZČŠĆŽĐ\d])", RegexOptions.Compiled);

        /// <summary>
        /// Match Evaluation for sentenceSpliter : _select_sentenceSpliter
        /// </summary>
        /// <param name="m">Match with value to process</param>
        /// <returns>For m.value "something" returns "SOMETHING"</returns>
        private static string _replace_sentenceSpliter(Match m)
        {
            string output = m.Value.Replace(".", "");
            output = output.Replace(" ", "");

            return output.ToUpper();
        }

        /// <summary>
        /// Secka string u recenice
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static List<string> splitContentToSentences(string input)
        {
            List<string> inputSentences = new List<string>();

            if (_select_sentenceSpliter.IsMatch(input))
            {
                // ima vise recenica
                string[] _ins = _select_sentenceSpliter.Split(input);
                foreach (string s in _ins) inputSentences.Add(s.Trim());
            }
            else
            {
                // postoji samo jedna recenica
                inputSentences.Add(input.Trim());
            }
            return inputSentences;
        }

        /// <summary>
        /// vrsi osnovno obelezavanje recenica na osnovu sadrzaja
        /// </summary>
        /// <param name="sentences"></param>
        internal static void basicFlaging(IEnumerable<IContentSentence> sentences)
        {
            foreach (IContentSentence newSentence in sentences)
            {
                newSentence.primaryFlaging();

                //if (newSentence.content.isUpperCase())
                //{
                //    newSentence.sentenceFlags.Add(contentSentenceFlag.caseCapital);
                //} else if (newSentence.content.isSentenceCase())
                //{
                //    newSentence.sentenceFlags.Add(contentSentenceFlag.caseSentence);
                //}
                //else if (newSentence.content.isSentenceFragmentCase())
                //{
                //    newSentence.sentenceFlags.Add(contentSentenceFlag.caseFragment);
                //}

                //switch (newSentence.spliter)
                //{
                //    case sentenceEnd_listStart2:
                //    case sentenceEnd_listStart:
                //    case sentenceEnd_arrowRight:
                //    case sentenceEnd_arrowLeft:
                //        newSentence.sentenceFlags.Add(contentSentenceFlag.pointing);
                //        break;
                //    case sentenceEnd_notFinished2:
                //    case sentenceEnd_notFinished:
                //        newSentence.sentenceFlags.Add(contentSentenceFlag.unfinished);
                //        break;
                //    case sentenceEnd_question:
                //        newSentence.sentenceFlags.Add(contentSentenceFlag.question);
                //        break;
                //    case sentenceEnd_normal:
                //        newSentence.sentenceFlags.Add(contentSentenceFlag.normal);
                //        break;
                //    case sentenceEnd_listItemEnd_listEnd:
                //    case sentenceEnd_listItemEnd:
                //        newSentence.sentenceFlags.Add(contentSentenceFlag.itemInListLast);
                //        break;
                //    case sentenceEnd_exclamation:
                //        newSentence.sentenceFlags.Add(contentSentenceFlag.exclamation);
                //        break;
                //    case "":
                //        if (newSentence.sentenceFlags.Contains(contentSentenceFlag.caseCapital))
                //        {
                //            newSentence.sentenceFlags.Add(contentSentenceFlag.titleStrongCase, contentSentenceFlag.title);
                //        } else if (newSentence.sentenceFlags.Contains(contentSentenceFlag.caseSentence))
                //        {
                //            newSentence.sentenceFlags.Add(contentSentenceFlag.titleSoftCase, contentSentenceFlag.title);
                //        }
                //        else if (newSentence.sentenceFlags.Contains(contentSentenceFlag.caseFragment))
                //        {
                //            newSentence.sentenceFlags.Add(contentSentenceFlag.item);
                //        }
                //        break;
                //    default:

                //            if (!String.IsNullOrEmpty(newSentence.spliter))
                //            {
                //                devNoteManager.note(newSentence, devNoteType.nlp, "Unknown spliter for sentence: [" + newSentence.spliter +
                //                                    "] - add support for it in> cs constants and sentenceAnalysis()",
                //                                    "blokCategorization");
                //            }
                //        break;

                //}
            }
        }

        /*
        /// <summary>
        /// Pravi recenice od prosledjenog String sadrzaja. Poziva preprocess, secka recenice na delove, kreira objekte sa recenicom, izvrsava odnovno flagovanje i povezuje recenice sa paragrafom ako je prosledjen
        /// </summary>
        /// <typeparam name="TSentence"></typeparam>
        /// <typeparam name="TToken"></typeparam>
        /// <param name="paragraph"></param>
        /// <param name="resources">Razliciti flagovi i drugi objekti koje moze koristiti pri obradi sadrzaja</param>
        /// <returns></returns>
        internal static contentSentenceCollection setSentencesFromContent<TSentence>(IContentParagraph paragraph, params object[] resources) where TSentence:IContentSentence,new()
        {/*
            String input = paragraph.content;

            sentenceDetectionFlags flags = new sentenceDetectionFlags(resources);
            contentPreprocessFlags preprocessFlags = new contentPreprocessFlags(resources);
            contentSentenceCollection output = new contentSentenceCollection();

            // preuzima parent page ako je prosledjen
            IContentPage parentPage = resources.getOfType<IContentPage>();

            if (flags.Contains(sentenceDetectionFlag.preprocessParagraphContent)) input = contentPreprocess.preprocess.process(input, preprocessFlags.ToArray());

            List<String> inputSentences = splitContentToSentences(input);

            foreach (String _inputSentece in inputSentences)
            {
                TSentence newSentence = new TSentence();
                newSentence.sourceContent = _inputSentece;
                newSentence.content = _inputSentece;
                if (_select_sentenceTerminator.IsMatch(_inputSentece))
                {
                    newSentence.sentenceFlags.Add(contentSentenceFlag.regular);
                    Match m = _select_sentenceTerminator.Match(_inputSentece);
                    if (m.Success)
                    {
                        newSentence.spliter = m.Value;
                        newSentence.content = _inputSentece.Substring(0, _inputSentece.Length - newSentence.spliter.Length);
                    }
                } else
                {
                    newSentence.sentenceFlags.Add(contentSentenceFlag.inregular);
                }
                output.Add(newSentence);
            }

            if (flags.Contains(sentenceDetectionFlag.performBasicFlaging)) basicFlaging(output);

            return output;
        }
    */
    }
}