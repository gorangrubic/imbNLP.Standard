// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tokenDetection.cs" company="imbVeles" >
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
    using imbACE.Core.core;
    using imbACE.Core.core.exceptions;
    using imbNLP.Core.contentStructure.core;
    using imbNLP.Core.contentStructure.interafaces;
    using imbSCI.Core.collection;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.text;
    using imbSCI.Data;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class tokenDetection
    {
        /// <summary>
        /// Regex select phoneNumber : [\+0^]{1,2}[\s]{0,2}([\d\(\)]{2,5}[\s\(\)\-\.\/\\]{1,2}){3,5}
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_phoneNumber = new Regex(@"[\+0^]{1,2}[\s]{0,2}([\d\(\)]{2,5}[\s\(\)\-\.\/\\]{1,2}){3,5}", RegexOptions.Compiled);

        /// <summary>
        /// Regex select standards : [A-Z]{1,5}[\s\-\:]*[\d]{2,5}[\d\:]*
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_standards = new Regex(@"[A-Z]{1,5}[\s\-\:]*[\d]{2,5}[\d\:]*", RegexOptions.Compiled);

        /// <summary>
        /// Regex select emailAddress : [a-zA-Z\d_\.]+@[a-zA-Z_\.]*?\.[a-zA-Z\.]{2,6}
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_emailAddress = new Regex(@"[a-zA-Z\d_\.]+@[a-zA-Z_\.]*?\.[a-zA-Z\.]{2,6}", RegexOptions.Compiled);

        /// <summary>
        /// Regex select acronimIrregular : [\s]{1}([ZXCVBNMKLJHGFDSQWRTYP]{2,})[\s]{1}
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_acronimIrregular = new Regex(@"[\s]{1}([ZXCVBNMKLJHGFDSQWRTYP]{2,})[\s]{1}", RegexOptions.Compiled);

        /// <summary>
        /// Regex select acronimByLength : [\s\b]{1}([A-Z]{3,4})[\s\b]{1}
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_acronimByLength = new Regex(@"[\s\b]{1}([A-Z]{3,4})[\s\b]{1}", RegexOptions.Compiled);

        /// <summary>
        /// Regex select yearNumber : ([\d]{4}[\.])
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_yearNumber = new Regex(@"([\d]{4}[\.])", RegexOptions.Compiled);

        /// <summary>
        /// Regex select postOfficeNumber : (([\d]{5})|([\d]{2}[\s]{1}[\d]{3})[\b]{1})[\W]{1}
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_postOfficeNumber = new Regex(@"(([\d]{5})|([\d]{2}[\s]{1}[\d]{3})[\b]{1})[\W]{1}", RegexOptions.Compiled);

        /// <summary>
        /// Regex select tokenWithSplitter : ([\w\d\.,:;\"-|]{1,})\b[\W]{0,3}
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_tokenWithSplitter = new Regex(@"([\w\d\.,\\\/:;\""\-_\|]{1,})\b[\W]{0,3}", RegexOptions.Compiled);

        /// <summary>
        /// Regex select tokenWithoutSplitter : ([\w\d\.,:;\""-|]{1,})\b
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_tokenWithoutSplitter = new Regex(@"([\w\d\.,\\\/:;\""\-_\|]{1,})\b", RegexOptions.Compiled);

        /// <summary>
        /// Regex select numberOrdal : ([\w]{1}\.{1}\s{1}){2,}
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_numberOrdal = new Regex(@"([\w]{1}\.{1}\s{1}){2,}", RegexOptions.Compiled);

        /// <summary>
        /// vrsi osnovnu klasifikaciju tokena
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newToken"></param>
        internal static void setTokenCaseAndGeneral<T>(T newToken) where T : IContentToken, new()
        {
            if (newToken == null)
            {
                var isb = new StringBuilder();
                isb.AppendLine("setTokenCaseAndGeneral error");
                isb.AppendLine("Received newToken is : " + newToken.toStringSafe());
                aceGeneralException ex = new aceGeneralException("newToken is null in setTokenCaseAndGeneral");
                throw ex;
                //devNoteManager.note(ex, isb.ToString(), "setTokenCaseAndGeneral error", devNoteType.tokenization);

                return;
            }
            //  logSystem.log("      -- -- -- setTokenCaseAndGeneral: " + newToken.content, logType.Notification);
            /*
            if (String.IsNullOrEmpty(newToken.content))
            {
                newToken.flags.Add(contentTokenFlag.empty);
                return;
            }
            if (newToken.content.isNumber())
            {
                newToken.flags.Add(contentTokenFlag.number);
                newToken.flags.Add(contentTokenFlag.numberDigital);
                if (newToken.content.isOrdinalNumber())
                {
                    newToken.flags.Add(contentTokenFlag.numberOrdinal);
                } else if (newToken.content.isFormatedNumber())
                {
                    newToken.flags.Add(contentTokenFlag.numberFormated);
                } else if (newToken.content.isDecimalNumber())
                {
                    newToken.flags.Add(contentTokenFlag.numberDecimal);
                }
            } else
            {
                if (_select_caseCamel.IsMatch(newToken.content))
                {
                    newToken.flags.Add(contentTokenFlag.caseFirstUpper);
                    newToken.flags.Add(contentTokenFlag.languageWord);
                }
                else if (_select_caseUpper.IsMatch(newToken.content))
                {
                    newToken.flags.Add(contentTokenFlag.caseAllUpper);
                    newToken.flags.Add(contentTokenFlag.languageWord);
                }
                else if (_select_caseLower.IsMatch(newToken.content))
                {
                    newToken.flags.Add(contentTokenFlag.caseLower);
                    newToken.flags.Add(contentTokenFlag.languageWord);
                }
                else
                {
                    newToken.flags.Add(contentTokenFlag.caseIrregular);
                    newToken.flags.Add(contentTokenFlag.languageWord);
                }
            }
            */
            // logSystem.log("      -- -- -- setTokenCaseAndGeneral done: ", logType.Notification);
        }

        /// <summary>
        /// Pravi tokene za prosledjenu recenicu
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sentence"></param>
        /// <param name="setTokenToSentence"></param>
        /// <param name="flags"></param>
        /// <param name="subsentenceMatches"></param>
        /// <returns></returns>
        private static List<IContentToken> setTokensForSentence<T>(IContentSentence sentence, bool setTokenToSentence, tokenDetectionFlag flag, contentMatchCollection subsentenceMatches = null) where T : IContentToken, new()
        {
            contentMatchCollection macroTokens = new contentMatchCollection();
            List<IContentToken> output = new List<IContentToken>();

            String scrambled = sentence.content;
            var flags = flag.getEnumListFromFlags();
            macroTokens.scrambled = sentence.content;

            foreach (tokenDetectionFlag fl in flags)
            {
                switch (fl)
                {
                    case tokenDetectionFlag.emailAddress:
                        macroTokens.Add(_select_emailAddress, fl);
                        break;

                    case tokenDetectionFlag.phonenumber:
                        macroTokens.Add(_select_phoneNumber, fl);
                        break;

                    case tokenDetectionFlag.standard:
                        macroTokens.Add(_select_standards, fl);
                        break;
                }
            }

            foreach (tokenDetectionFlag fl in flags)
            {
                switch (fl)
                {
                    case tokenDetectionFlag.yearNumber:
                        macroTokens.Add(_select_yearNumber, fl);
                        break;

                    case tokenDetectionFlag.postOfficeNumber:
                        macroTokens.Add(_select_phoneNumber, fl);
                        break;
                }
            }

            foreach (tokenDetectionFlag fl in flags)
            {
                switch (fl)
                {
                    case tokenDetectionFlag.acronims:
                        macroTokens.Add(_select_acronimIrregular, fl);
                        macroTokens.Add(_select_acronimByLength, fl);
                        break;
                }
            }

            // logSystem.log("    -- setTokensForSentence: quering performed "+ macroTokens.Count   , logType.Notification);

            if (flags.Contains(tokenDetectionFlag.standardDetection))
            {
                macroTokens.Add(_select_tokenWithSplitter, tokenDetectionFlag.none);
            }

            Int32 i = 0;
            Int32 mx = sentence.content.Length;

            while (i < mx)
            {
                try
                {
                    #region LOOP

                    oneOrMore<contentMatch> cms = macroTokens.allocated(i, 1);

                    if (cms == null)
                    {
                        i = mx;
                        continue;
                    }

                    if (cms.isNothing)
                    {
                        i++;
                        continue;
                    }
                    else
                    {
                        contentMatch cm = cms.First();

                        //
                        if (cm == null)
                        {
                            // logSystem.log("        -- -- -- cm is null " + cm.toStringSafe(), logType.Notification);
                            i++;
                            continue;
                        }
                        else
                        {
                            //        logSystem.log("        -- -- -- cm found " + cm.toStringSafe(), logType.Notification);
                        }

                        i = i + cm.match.Length;

                        T newToken = new T();
                        String mch = cm.match.Value.Trim("#".ToCharArray());
                        newToken.sourceContent = mch;
                        newToken.content = mch;

                        Match sp = _select_tokenWithSplitter.Match(mch);
                        if (sp.Success)
                        {
                            newToken.spliter = sp.Value;
                            newToken.content = newToken.content.removeEndsWith(newToken.spliter);
                        }
                        else
                        {
                            newToken.spliter = "";
                            newToken.content = mch;
                        }

                        IContentSentence _sentence = sentence;

                        if (setTokenToSentence)
                        {
                            if (subsentenceMatches != null)
                            {
                                if (subsentenceMatches.isAllocated(cm.match.Index, cm.match.Length))
                                {
                                    oneOrMore<contentMatch> subcms = subsentenceMatches.allocated(cm.match.Index,
                                                                                                  cm.match.Length);

                                    contentMatch subcm = subcms.First();
                                    if (subcm == null)
                                    {
                                        // logSystem.log("    -- -- -- sub cm is null  ", logType.Notification);
                                    }
                                    else
                                    {
                                        _sentence = subcm.element as IContentSubSentence;
                                    }
                                }
                                else
                                {
                                }
                            }
                            else
                            {
                            }
                            if (_sentence != null)
                            {
                                _sentence.setItem(newToken);
                                if (_sentence == sentence)
                                {
                                    output.Add(newToken);
                                }
                            }
                            else
                            {
                                logSystem.log("    -- -- -- _sentence is null  ", logType.Notification);
                            }
                        }
                        else
                        {
                            output.Add(newToken);
                        }

                        if (cm.associatedKey == null)
                        {
                            logSystem.log("    -- -- -- cm.associatedKey  is null ", logType.Notification);
                        }
                        else
                        {
                            tokenDetectionFlag fl = tokenDetectionFlag.none;

                            Boolean detected = Enum.TryParse(cm.associatedKey.toStringSafe(), true, out fl);

                            newToken.detectionFlags = fl;
                        }

                        cm.element = newToken;
                    }

                    #endregion LOOP
                }
                catch (Exception ex)
                {
                    var isb = new StringBuilder();
                    isb.AppendLine("loop error error");
                    isb.AppendLine("Target is: i=" + i + "[mx=" + mx + "]");
                    //devNoteManager.note(ex, isb.ToString(), "loop error", devNoteType.tokenization);
                }
            }
            return output;
        }

        /// <summary>
        /// Postavlja tokene u prosledjenu recenicu i vraca listu svih tokena
        /// </summary>
        /// <param name="content"></param>
        /// <param name="sentence"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        internal static List<T> setTokensFromContent<T, TS>(params object[] resources)
            where T : class, IContentToken, new()
            where TS : IContentSubSentence, new()
        {
            //logSystem.log("set tokens from content Sentence: " + sentence.content, logType.Notification);
            IContentSentence sentence = resources.getFirstOfType<IContentSentence>();
            contentPreprocessFlag preprocessFlags = resources.getFirstOfType<contentPreprocessFlag>();
            subsentenceDetectionFlag subflags = resources.getFirstOfType<subsentenceDetectionFlag>();
            tokenDetectionFlag flags = resources.getFirstOfType<tokenDetectionFlag>();

            //tokenDetectionFlag[] _flags

            List<T> output = new List<T>();

            try
            {
                //subsentenceDetectionFlags subflags = _subflags;
                // tokenDetectionFlags flags = _flags;

                string pcontent = preprocess.process(sentence.content, preprocessFlags);

                contentMatchCollection subsentenceMatches = subsentenceDetection.setSubSentences<TS>(sentence, subflags);

                foreach (contentMatch dt in subsentenceMatches.Values)
                {
                    IContentSubSentence ss = dt.element as IContentSubSentence;
                    sentence.items.Add(ss);
                    foreach (T sst in ss.items)
                    {
                        output.Add(sst);
                    }
                    //output.AddRange(ss.items);
                }

                List<IContentToken> directTokens = new List<IContentToken>();

                directTokens = setTokensForSentence<T>(sentence, true, flags, subsentenceMatches);

                if (directTokens != null)
                {
                    foreach (IContentToken dt in directTokens)
                    {
                        T tkn = dt as T;
                        if (tkn != null) output.Add(tkn);
                    }
                }
                else
                {
                }

                sentence.content = pcontent;
            }
            catch (Exception ex)
            {
                var isb = new StringBuilder();
                isb.AppendLine("tokenDetection error");
                isb.AppendLine("Target is: " + sentence.toStringSafe());
                throw;
                // devNoteManager.note(sentence, ex, isb.ToString(), "tokenDetection", devNoteType.tokenization);
            }

            // logSystem.log("set tokens from content Sentence done", logType.Notification);
            return output;
        }
    }
}