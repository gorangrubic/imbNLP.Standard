// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tokenCategorization.cs" company="imbVeles" >
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
    using imbNLP.Core.contentExtensions;
    using imbNLP.Core.contentStructure.elements;
    using imbNLP.Core.contentStructure.interafaces;
    using imbNLP.Core.contentStructure.tokenizator;
    using imbNLP.Data.basic;
    using imbNLP.Data.enums;
    using imbNLP.Data.enums.flags;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.io;
    using imbSCI.Data;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Algoritmi za kategorizaciju
    /// </summary>
    internal static class tokenCategorization
    {
        /// <summary>
        /// OSNOVNA ANALIZA TOKENA: sprovodi od FAZE 1 do FAZE 3 - poziva nlpBase alate za svaki od tokena
        /// </summary>
        /// <param name="content"></param>
        /// <param name="language"></param>
        internal static void tokenAnalysis(IContentPage content, nlpTokenizatorSettings settings, basicLanguage language)
        {
            /*
            if (content.tokens == null)
            {
                return ;
            }

            // FAZA 1>
            if (settings.doTokenTypeDetection_basic)
            foreach (IContentToken tk in content.tokens)
            {
                tk.genericType = tokenCategorization.findGenericTypeBasic(tk);
            }

            // FAZA 2>
            if (settings.doTokenTypeDetection_second)
            foreach (IContentToken tk in content.tokens)
            {
                // izgradnja syllables-a
              //  tk.syllablesDetection(settings);

                tk.genericType = tokenCategorization.findGenericTypeSecond(tk, language);
            }

            // Faza 3
            if (settings.doTokenTypeDetection_languageBasic)
            foreach (IContentToken tk in content.tokens)
            {
                deployTokenLanguageBasic(tk, language);
            }

            // Faza 4>
            if (settings.doTokenTypeDetection_languageAdvanced)
            foreach (IContentToken tk in content.tokens)
            {
                deployTokenLanguage(tk, language);
            }
            */
        }

        /*
        /// <summary>
        /// Osnovna obrada na osnovu jezika> da li je poznata rec u pitanju ili nije -
        /// </summary>
        /// <param name="token"></param>
        /// <param name="language"></param>
        private static void deployTokenLanguageBasic(IContentToken token, basicLanguage language)
        {
            switch (token.genericType)
            {
                case nlpTokenGenericType.unknownWord:
                    if (language.isKnownWord(token.content))
                    {
                        token.genericType = nlpTokenGenericType.knownWord;
                    }
                    break;

                case nlpTokenGenericType.number:
                    break;
            }
        }
        */

        /*
        /// <summary>
        /// FAZA 3: Dodatna obrada tokena na osnovu jezickih podesavanja -- nije jos implementirano!!!
        /// </summary>
        /// <param name="token"></param>
        /// <param name="language"></param>
        private static void deployTokenLanguage(IContentToken token, basicLanguage language)
        {
            switch (token.genericType)
            {
                case nlpTokenGenericType.unknownWord:

                    //if (language.testBoolean(token.content, basicLanguageCheck.spellCheck))
                    //{
                    //    token.genericType = nlpTokenGenericType.knownWord;
                    //}

                    // token.wordVariations = languageTools.test<List<string>>(language, token.content, languageModelOperation.getVariations) as List<string>;
                    // List<string> stems = languageTools.test<List<string>>(language, token.content, languageModelOperation.getStems) as List<string>;
                    //token.wordRoot = imbStringOperations.longestCommonSubstring(token.wordVariations);

                    //token.wordRoot = stems[0];
                    break;

                case nlpTokenGenericType.number:

                    break;
            }
        }
        */

        /// <summary>
        /// FAZA 2: podesava letter case, proverava jezik, proverava da li je mozda akronim - funkcionise samo ako su detektovani slogovi
        /// </summary>
        /// <param name="token"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        private static nlpTokenGenericType findGenericTypeSecond(IContentToken token, basicLanguage language)
        {
            nlpTokenGenericType output = token.genericType;
            object testOut;

            /*

            if (token.tokenBaseType == nlpTokenBaseType.word)
            {
                token.letterCase = nlpTextCase.unknown;
                if (tokenization.wordWithCapitalStart.IsMatch(token.content)) token.letterCase = nlpTextCase.firstUpperRestLower;
                if (token.letterCase == nlpTextCase.unknown) if (token.content.ToLower() == token.content) token.letterCase = nlpTextCase.lowerCase;
                if (token.letterCase == nlpTextCase.unknown) if (token.content.ToUpper() == token.content) token.letterCase = nlpTextCase.upperCase;
                if (token.letterCase == nlpTextCase.unknown) token.letterCase = nlpTextCase.mixedCase;
            }
            */

            if (token.flags == contentTokenFlag.languageWord)
            {
                if (language.testBoolean(token.content, basicLanguageCheck.spellCheck))
                {
                    token.flags = token.flags.Add(contentTokenFlag.languageKnownWord);

                    output = nlpTokenGenericType.knownWord;
                }
                else
                {
                    if (token.flags.getEnumListFromFlags().ContainsOneOrMore(contentTokenFlag.acronim, contentTokenFlag.acronimDiscovered, contentTokenFlag.acronimKnown))
                    {
                        output = nlpTokenGenericType.wordAbrevation;
                    }
                    else
                    {
                        if (token.flags.HasFlag(contentTokenFlag.caseAllUpper))
                        {
                            contentToken pt = token.parent as contentToken;
                            if (pt != null)
                            {
                                if (pt.flags.HasFlag(contentTokenFlag.subsentence_title))
                                {
                                    token.flags = token.flags.Add(contentTokenFlag.title);
                                }
                                else if (pt.flags.HasFlag(contentTokenFlag.subsentence_information))
                                {
                                    token.flags = token.flags.Add(contentTokenFlag.namedEntity);
                                }
                            }
                            else
                            {
                                token.flags = token.flags.Add(contentTokenFlag.titleOneWord);
                            }
                        }
                        else if (token.flags.HasFlag(contentTokenFlag.caseFirstUpper))
                        {
                            contentToken pt = token.parent as contentToken;
                            if (pt != null)
                            {
                                if (pt.flags.HasFlag(contentTokenFlag.subsentence_title))
                                {
                                    token.flags = token.flags.Add(contentTokenFlag.title);
                                }
                                else if (pt.flags.HasFlag(contentTokenFlag.subsentence_information))
                                {
                                    token.flags = token.flags.Add(contentTokenFlag.namedEntity);
                                }
                                else
                                {
                                    if (!token.isFirst)
                                    {
                                        token.flags = token.flags.Add(contentTokenFlag.namedEntity);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            token.genericType = output;

            return output;
        }

        /// <summary>
        /// FAZA 1: Osnovni nivo detekcije generickog tipa - koristi niz REGEX testova da bi utvrdio o kakvom se tokenu radi, da li je rec ili nije rec. Ako je rec cisti content tako da ostane samo rec.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static nlpTokenGenericType findGenericTypeBasic(IContentToken source)
        {
            Regex reg = null;
            nlpTokenGenericType output = nlpTokenGenericType.unknown;

            if (string.IsNullOrEmpty(source.content))
            {
                output = nlpTokenGenericType.empty;
                return output;
            }

            if (string.IsNullOrWhiteSpace(source.content))
            {
                output = nlpTokenGenericType.empty;
                return output;
            }

            if (tokenization.numericSelect.IsMatch(source.content))
            {
                // ima brojeva
                if (tokenization.numberOrdinal.IsMatch(source.sourceContent))
                {
                    output = nlpTokenGenericType.numberOrdinal;
                }
                else
                {
                    if (tokenization.numbersFormatedExpr.IsMatch(source.sourceContent))
                    {
                        output = nlpTokenGenericType.numberFormated;
                    }
                    else
                    {
                        if (tokenization.lettersSelect.IsMatch(source.content))
                        {
                            output = nlpTokenGenericType.mixedAlfanumeric;
                        }
                        else
                        {
                            output = nlpTokenGenericType.number;
                        }
                    }
                }
            }
            else
            {
                if (tokenization.lettersSelect.IsMatch(source.content))
                {
                    // ima slova
                    Match flw = tokenization.firstLetterWord.Match(source.content);

                    if (flw.Success)
                    {
                        output = nlpTokenGenericType.unknownWord;

                        if (source.content.Contains('@'))
                        {
                            if (tokenization.emailExpr.IsMatch(source.content))
                            {
                                output = nlpTokenGenericType.email;
                            }
                        }
                    }
                    else
                    {
                        if (tokenization.selectPunctation.IsMatch(source.content))
                        {
                            output = nlpTokenGenericType.mixedAlfasymbolic;
                            // nema brojeva
                        }
                        else
                        {
                            output = nlpTokenGenericType.unknownWord;
                        }
                    }
                }
                else
                {
                    if (tokenization.selectPunctation.IsMatch(source.content))
                    {
                        output = nlpTokenGenericType.symbols;
                        // nema brojeva
                    }
                    else
                    {
                        output = nlpTokenGenericType.unknown;
                    }
                }
            }

            if (genericToBaseType(output) == nlpTokenBaseType.word)
            {
                string clean = tokenization.samoRec.Match(source.content).Value;
                source.content = clean;
                source.spliter = source.sourceContent.Replace(clean, "");
            }

            return output;
        }

        /// <summary>
        /// Jednostavni konvertor izmedju base tipova i generic type
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static nlpTokenBaseType genericToBaseType(nlpTokenGenericType input)
        {
            switch (input)
            {
                case nlpTokenGenericType.email:
                case nlpTokenGenericType.mixedAlfanumeric:
                case nlpTokenGenericType.mixedAlfasymbolic:
                    return nlpTokenBaseType.mixed;
                    break;

                case nlpTokenGenericType.unknownWord:
                case nlpTokenGenericType.knownWord:
                case nlpTokenGenericType.possibleName:
                case nlpTokenGenericType.possibleAcronim:
                case nlpTokenGenericType.wordAbrevation:

                    return nlpTokenBaseType.word;
                    break;

                case nlpTokenGenericType.number:
                case nlpTokenGenericType.numberFormated:
                case nlpTokenGenericType.numberOrdinal:
                    return nlpTokenBaseType.number;
                    break;

                default:
                    return nlpTokenBaseType.unknown;
                    break;
            }
        }
    }
}