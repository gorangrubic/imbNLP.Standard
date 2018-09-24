// --------------------------------------------------------------------------------------------------------------------
// <copyright file="subsentenceDetection.cs" company="imbVeles" >
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
    using imbNLP.Core.contentStructure.core;
    using imbNLP.Core.contentStructure.interafaces;
    using imbSCI.Data;
    using System;
    using System.Text.RegularExpressions;

    public static class subsentenceDetection
    {
        /// <summary>
        /// Regex select potentialPersonalNames : [A-ZČŠĆŽĐ]{1}[a-zžđščć]{2,}[\s]{1,4}[A-ZČŠĆŽĐ]{1}[a-zćšđčž]{2,}
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_potentialPersonalNames = new Regex(@"[A-ZČŠĆŽĐ]{1}[a-zžđščć]{2,}[\s]{1,4}[A-ZČŠĆŽĐ]{1}[a-zćšđčž]{2,}", RegexOptions.Compiled);

        /// <summary>
        /// Regex select potentialCityAndPost : (\b[\d]{5}[\s\n]{1,3}([A-ZČŠĆŽĐ]{1}[a-zžđščć]{1,}|[A-ZČŠĆŽĐ]{2,}))|(([A-ZČŠĆŽĐ]{1}[a-zžđščć]{1,}|[A-ZČŠĆŽĐ]{2,}){1}[\s\n]{1,3}[\d]{5}\b)
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_potentialCityAndPost = new Regex(@"(\b[\d]{5}[\s\n]{1,3}([A-ZČŠĆŽĐ]{1}[a-zžđščć]{1,}|[A-ZČŠĆŽĐ]{2,}))|(([A-ZČŠĆŽĐ]{1}[a-zžđščć]{1,}|[A-ZČŠĆŽĐ]{2,}){1}[\s\n]{1,3}[\d]{5}\b)", RegexOptions.Compiled);

        /// <summary>
        /// Regex select quotedSubSentence : ([\""]([A-Za-z\s,;:\-])+[\""])
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_quotedSubSentence = new Regex(@"([\""]([A-Za-z\s,;:\-])+[\""])", RegexOptions.Compiled);

        /// <summary>
        /// Regex select enbracedSubSentence : ([\(]([A-Za-z\s,;:\-])+[\)])
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_enbracedSubSentence = new Regex(@"([\(]([A-Za-z\s,;:\-])+[\)])", RegexOptions.Compiled);

        /// <summary>
        /// Regex select enumerationSubSentence : \b([\:]{1}([\s]*([A-ZČŠĆŽĐa-zžđščć]{1,3})+([\s]{0,2}([,\;\-]{1}|[\s]{0,1}[\w]{1}[\s]{1}))*){1,})
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_enumerationSubSentence = new Regex(@"\b([\:]{1}([\s]*([A-ZČŠĆŽĐa-zžđščć]{1,3})+([\s]{0,2}([,\;\-]{1}|[\s]{0,1}[\w]{1}[\s]{1}))*){1,})", RegexOptions.Compiled);

        /// <summary>
        /// Regex select innerSentence : (,{1}[\s]{1})([\w\d\s]*)(,{1}[\s]{1})
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_innerSentence = new Regex(@"(,{1}[\s]{1})([\w\d\s]*)(,{1}[\s]{1})", RegexOptions.Compiled);

        /// <summary>
        /// Vraca pod recenice za prosledjenu recenicu. sentence.content ce dobiti skremblovanu verziju - gde je izbaceno sve sto nije
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <param name="sentence"></param>
        /// <param name="page"></param>
        /// <param name="_subflags"></param>
        /// <returns></returns>
        public static contentMatchCollection setSubSentences<T>(IContentSentence sentence, subsentenceDetectionFlag _subflags) where T : IContentSubSentence, new()
        {
            // List<T> output = new List<T>();
            // logSystem.log("-- set sub sentences for: " + sentence.content, logType.Notification);
            contentMatchCollection subsentenceMatches = new contentMatchCollection();

            String scrambled = sentence.content;
            subsentenceMatches.scrambled = scrambled;

            var subflags = _subflags.getEnumListFromFlags();

            foreach (subsentenceDetectionFlag fl in subflags)
            {
                switch (fl)
                {
                    case subsentenceDetectionFlag.enbracedSubSentences:
                        subsentenceMatches.Add(_select_enbracedSubSentence, fl);
                        break;

                    case subsentenceDetectionFlag.enumerationSubSentences:
                        subsentenceMatches.Add(_select_enumerationSubSentence, fl);
                        break;

                    case subsentenceDetectionFlag.quotationSubSentences:
                        subsentenceMatches.Add(_select_quotedSubSentence, fl);
                        break;
                }
            }

            //foreach (subsentenceDetectionFlag fl in subflags)
            //{
            //    switch (fl)
            //    {
            //        case subsentenceDetectionFlag.potentialPersonalNames:
            //            subsentenceMatches.Add(_select_potentialPersonalNames, fl);
            //            break;
            //        case subsentenceDetectionFlag.cityAndPostnumber:
            //            subsentenceMatches.Add(_select_potentialCityAndPost, fl);
            //            break;

            //    }

            //}

            foreach (subsentenceDetectionFlag fl in subflags)
            {
                switch (fl)
                {
                    case subsentenceDetectionFlag.punctationSubSentences:
                        subsentenceMatches.Add(_select_innerSentence, fl);
                        break;
                }
            }

            foreach (contentMatch cm in subsentenceMatches.Values)
            {
                T subsentence = new T();
                subsentence.parent = sentence;
                subsentence.sourceContent = cm.match.Value;
                subsentence.content = cm.match.Value;

                //subsentence.detectionFlags.Add((subsentenceDetectionFlag)cm.associatedKey);

                //switch ((subsentenceDetectionFlag) cm.associatedKey)
                //{
                //    case subsentenceDetectionFlag.enbracedSubSentences:
                //        subsentence.flags.Add(contentTokenFlag.subsentence_inner);
                //        break;
                //    case subsentenceDetectionFlag.enumerationSubSentences:
                //        subsentence.flags.Add(contentTokenFlag.subsentence_enumeration);
                //        break;
                //    case subsentenceDetectionFlag.quotationSubSentences:
                //        subsentence.flags.Add(contentTokenFlag.subsentence_quoted);
                //        break;
                //    case subsentenceDetectionFlag.cityAndPostnumber:
                //        subsentence.flags.Add(contentTokenFlag.subsentence_information);
                //        break;
                //    case subsentenceDetectionFlag.punctationSubSentences:
                //        subsentence.flags.Add(contentTokenFlag.subsentence_inner);
                //        break;
                //    case subsentenceDetectionFlag.potentialPersonalNames:
                //        subsentence.flags.Add(contentTokenFlag.subsentence_information);
                //        break;

                //}

                cm.element = subsentence;
            }

            sentence.content = scrambled;
            // logSystem.log("-- set sub sentences done: ", logType.Notification);
            return subsentenceMatches;
        }
    }
}