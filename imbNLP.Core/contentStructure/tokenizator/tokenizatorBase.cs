// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tokenizatorBase.cs" company="imbVeles" >
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
namespace imbNLP.Core.contentStructure.tokenizator
{
    #region imbVELES USING

    using imbACE.Core.core;
    using imbNLP.Core.contentStructure.interafaces;
    using imbSCI.Data.data;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;

    // using imbSemanticEngine.concluding.semanticModels;

    //using imbSemanticEngine.nlp.context.structure;

    #endregion imbVELES USING

    /// <summary>
    /// 2013c: LowLevel resurs
    /// </summary>
    public abstract class tokenizatorBase : imbBindable
    {
        public tokenizatorBase(nlpTokenizatorSettings __settings)
        {
            settings = __settings;
        }

        #region --- tokenizatorSettings ------- Podesavanja tokenizatora

        private nlpTokenizatorSettings _settings;

        /// <summary>
        /// Podesavanja tokenizatora
        /// </summary>
        [XmlIgnore]
        protected nlpTokenizatorSettings settings
        {
            get { return _settings; }
            set
            {
                _settings = value;
                OnPropertyChanged("settings");
            }
        }

        #endregion --- tokenizatorSettings ------- Podesavanja tokenizatora

        //public abstract IContentPage tokenizeContent(String content);

        public string compressNewLines(string source)
        {
            string out2 = source;
            out2 = tokenization.blankLineSelector.Replace(out2, Environment.NewLine);

            string nnnl = Environment.NewLine + Environment.NewLine + Environment.NewLine;
            string nnl = Environment.NewLine + Environment.NewLine;
            while (out2.Contains(nnnl))
            {
                out2 = out2.Replace(nnnl, nnl);
            }
            return out2;
        }

        /// <summary>
        /// ZASTARELO
        /// </summary>
        /// <typeparam name="TSentence"></typeparam>
        /// <typeparam name="TToken"></typeparam>
        /// <param name="reg"></param>
        /// <param name="sourceLine"></param>
        /// <param name="paragraph"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        protected List<TSentence> setSentencesFromContent<TSentence, TToken>(Regex reg, string sourceLine,
                                                                             IContentParagraph paragraph)
            where TSentence : IContentSentence, new()
            where TToken : IContentToken, new()
        {
            // VRACA VISE OD POTREBNOG BROJA RECENICA
            //  logSystem.log("_setSentencesFromContent: " + sourceLine, logType.Notification);

            List<TSentence> output = new List<TSentence>();
            string lastSourceContent = "";

            int lastIndex = 0;

            int cutStart;
            int cutLength;
            int ln;

            string cleanContent = "";
            string sourceContent = "";
            string splitter = "";

            string sourceLineTemp = tokenization.sentenceInsert_Prefix + sourceLine + tokenization.sentenceInsert_Sufix;

            sourceLine = sourceLine.Trim();

            //sourceLine =  sourceLine + " End.";

            MatchCollection coll = reg.Matches(sourceLineTemp);

            //  nlpSentence newToken = null; // = new nlpToken();

            if (coll.Count == 0)
            {
                if (sourceLine.Length > 0)
                {
                    output.Add(makeSentence<TSentence>(sourceLine, paragraph));
                }
                return output;
            }

            Match cpLast = coll[coll.Count - 1]; //Captures.Count - 1];
            string senSource = "";

            // --- ne uzima deo recenice, vec ceo source

            foreach (Match pmt in coll)
            {
                senSource = sourceLine.Substring(pmt.Index + pmt.Length - 1);

                //if (filter != null)
                //{
                //   // sourceLine = imbFilterModuleEngine.executeSimple(filter, sourceLine);
                //}
                //if (String.IsNullOrEmpty(sourceLine))
                //{
                //}
                //else
                //{
                //    var sen = makeSentence<TSentence>(sourceLine, paragraph, filter);

                //    output.Add(sen);
                //}
                lastIndex = pmt.Index;
            }
            logSystem.log("_setSentencesFromContent done: ", logType.Notification);

            return output;
        }

        /// <summary>
        /// Postavlja tokene u prosledjenu recenicu i vraca listu svih tokena
        /// </summary>
        /// <param name="content"></param>
        /// <param name="sentence"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        internal List<T> setTokensFromContent<T>(string content, IContentSentence sentence)
            where T : IContentToken, new()
        {
            List<T> output = new List<T>();

            MatchCollection coll = tokenization.tokenSelectForWordsAndPunctation.Matches(content);
            //String[] rawTokens = wordSpliter.Split(content);

            foreach (Match pmt in coll)
            {
                int lastIndex = 0;
                foreach (Capture cp in pmt.Captures)
                {
                    string tkn = cp.Value;
                    if (string.IsNullOrEmpty(tkn)) break;
                    tkn = tkn.Trim();
                    if (string.IsNullOrEmpty(tkn)) break;

                    //cp.Index
                    T newToken = new T(); //(cp.Value, sentence, "", cp.Value);
                    newToken.content = cp.Value;
                    newToken.parent = sentence;
                    newToken.spliter = "";
                    newToken.sourceContent = cp.Value;

                    if (sentence != null) sentence.setItem(newToken);
                    output.Add(newToken);
                }
            }

            return output;
        }

        //internal T makeSentence<T>(Match pmt, String sourceLine, IContentParagraph content, Int32 lastIndex, imbFilterModule filter)where T:IContentSentence, new()
        //{
        //    String sl = sourceLine.Substring(lastIndex, pmt.Index + pmt.Length - lastIndex);
        //    T newToken = makeSentence<T>(sl, content, filter);
        //    return newToken;
        //}

        internal T makeSentence<T>(string sourceLine, IContentParagraph content = null)
            where T : IContentSentence, new()
        {
            // Match mtch = cleanSentence.Match(sourceLine);
            //Match mtch = tokenization.sentenceInner.Match(sourceLine);
            string splitter = "";
            string tmpSource = sourceLine.Trim(); //tokenization.sentenceInner.Match(sourceLine).Groups[1].Value.Trim();
            //sourceLine = sentenceInner.Match(sourceLine).Value.Trim();

            var ms = tokenization.sentenceElements.Match(tmpSource);

            if (ms.Success)
            {
                splitter = ms.Groups[ms.Groups.Count - 1].Value;
            }

            //tokenization.punctAtEndOfString.Match(tmpSource).Value;
            if (tmpSource.EndsWith(splitter))
            {
                tmpSource = tmpSource.Substring(0, tmpSource.Length - splitter.Length);
            }

            //if (filter != null)
            //{
            //    //tmpSource = imbFilterModuleEngine.executeSimple(filter, tmpSource);
            //}
            tmpSource = tmpSource.Trim();

            T newToken = new T(); //nlpSentence(tmpSource, content, splitter, tmpSource);}
            newToken.content = tmpSource;
            if (content != null)
            {
                newToken.parent = content;
            }
            newToken.spliter = splitter;
            newToken.sourceContent = sourceLine;
            //  content.setItem(newToken);
            return newToken;
        }
    }
}