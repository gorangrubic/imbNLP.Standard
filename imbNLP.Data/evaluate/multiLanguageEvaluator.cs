// --------------------------------------------------------------------------------------------------------------------
// <copyright file="multiLanguageEvaluator.cs" company="imbVeles" >
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
// Project: imbNLP.Data
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
namespace imbNLP.Data.evaluate
{
    using imbNLP.Data.basic;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.text;
    using imbSCI.Data;
    using imbSCI.DataComplex.special;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Text evaluator not using SemanticLexicon but multiple dictionaries
    /// </summary>
    public class multiLanguageEvaluator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="multiLanguageEvaluator"/> class.
        /// </summary>
        /// <param name="languageIDs">The language i ds.</param>
        public multiLanguageEvaluator(params basicLanguageEnum[] languageIDs)
        {
            setup(languageIDs);
        }

        protected Dictionary<basicLanguageEnum, basicLanguage> languages { get; set; } = new Dictionary<basicLanguageEnum, basicLanguage>();

        protected List<string> globalIgnoreList { get; set; } = new List<string>();

        protected ConcurrentBag<string> unknownWords { get; set; } = new ConcurrentBag<string>();

        /// <summary>
        /// Setups the specified language i ds.
        /// </summary>
        /// <param name="languageIDs">The language i ds.</param>
        public void setup(IEnumerable<basicLanguageEnum> languageIDs)
        {
            foreach (basicLanguageEnum id in languageIDs)
            {
                if (!languages.Keys.Contains(id))
                {
                    languages.Add(id, imbLanguageFrameworkManager.GetBasicLanguage(id));
                }
            }

            globalIgnoreList.AddRange(new string[] { "google", "maps", "logo", "mail", "facebook", "youtube", "istagram", "skype", "javascript", "browser", "back", "home", "index", "click" });
            globalIgnoreList.AddRange(new string[] { "lorem", "ipsum", "dolor", "amet" });
        }

        public int tokenLengthMin { get; set; } = 3;
        public int validTokenTarget { get; set; } = 50;
        public int testTokenLimit { get; set; } = 150;

        /// <summary>
        /// Evaluates the specified content. Before evaluation the content tokens are transformed with <see cref="termTools.getTokens(true, false, true, true)"/>
        /// </summary>
        /// <param name="content">The textual content</param>
        /// <param name="ignoreTokens">The ignore tokens.</param>
        /// <returns></returns>
        public multiLanguageEvaluation evaluate(string content, List<string> ignoreTokens = null, List<string> processedTokens = null)
        {
            if (ignoreTokens == null) ignoreTokens = new List<string>();

            List<string> contentTokens = content.getTokens(true, false, true, true);

            return evaluate(contentTokens, ignoreTokens, processedTokens);
        }

        /// <summary>
        /// Evaluates the specified content non-unique and non filtered tokens
        /// </summary>
        /// <param name="contentTokens">The content tokens to evaluate</param>
        /// <param name="ignoreTokens">The ignore tokens to skip from evaluation</param>
        /// <returns>Results of evaluation</returns>
        public multiLanguageEvaluation evaluate(List<string> contentTokens, List<string> ignoreTokens = null, List<string> processedTokens = null)
        {
            if (ignoreTokens == null) ignoreTokens = new List<string>();
            multiLanguageEvaluationTask task = new multiLanguageEvaluationTask();
            task.testLanguages.AddRange(languages.Keys);
            task.tokenLengthMin = tokenLengthMin;
            task.validTokenTarget = validTokenTarget;
            task.input_contentTokens = contentTokens;
            task.input_ignoredTokens = ignoreTokens;
            task.testTokenLimit = testTokenLimit;

            return evaluate(task, processedTokens);
        }

        /// <summary>
        /// Gets all proper tokens sorted by frequency.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="__tokenLengthMin">The token length minimum.</param>
        /// <param name="input_ignoredTokens">The input ignored tokens.</param>
        /// <returns></returns>
        public List<string> GetAllProperTokensSortedByFrequency(string content, int __tokenLengthMin = -1, List<string> input_ignoredTokens = null)
        {
            if (__tokenLengthMin == -1) __tokenLengthMin = tokenLengthMin;
            List<string> contentTokens = content.getTokens(true, false, true, true, __tokenLengthMin);
            return GetAllProperTokensSortedByFrequency(contentTokens, __tokenLengthMin, input_ignoredTokens);
        }

        /// <summary>
        /// Gets all proper tokens sorted by frequency.
        /// </summary>
        /// <param name="input_contentTokens">The input content tokens.</param>
        /// <param name="tokenLengthMin">The token length minimum.</param>
        /// <param name="input_ignoredTokens">The input ignored tokens.</param>
        /// <returns></returns>
        public List<string> GetAllProperTokensSortedByFrequency(IEnumerable<string> input_contentTokens, int tokenLengthMin, List<string> input_ignoredTokens)
        {
            instanceCountCollection<string> tokenFrequency = new instanceCountCollection<string>();
            if (input_ignoredTokens == null) input_ignoredTokens = new List<string>();
            // <----- preprocessing token input
            foreach (string token in input_contentTokens)
            {
                bool useOk = true;
                if (useOk && (token.isNullOrEmptyString()))
                {
                    useOk = false;
                }
                if (useOk && (token == Environment.NewLine))
                {
                    useOk = false;
                }
                if (useOk && (token.Length < tokenLengthMin))
                {
                    useOk = false;
                }
                if (useOk && (input_ignoredTokens.Contains(token)))
                {
                    useOk = false;
                }
                if (useOk && (globalIgnoreList.Contains(token)))
                {
                    useOk = false;
                }
                if (useOk)
                {
                    tokenFrequency.AddInstance(token);
                }
            }

            List<string> tokenToTest = tokenFrequency.getSorted();
            return tokenToTest;
        }

        /// <summary>
        /// Evaluates already defined evaluation task
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="processedTokens">Externally preprocessed tokens - if not supplied it will call <see cref="GetAllProperTokensSortedByFrequency(IEnumerable{string}, int, List{string})"/> automatically</param>
        /// <returns></returns>
        public multiLanguageEvaluation evaluate(multiLanguageEvaluationTask task, List<string> processedTokens = null)
        {
            multiLanguageEvaluation evaluation = new multiLanguageEvaluation();
            evaluation.task = task;

            if (processedTokens != null)
            {
                evaluation.allContentTokens = processedTokens;
            }
            else
            {
                evaluation.allContentTokens = GetAllProperTokensSortedByFrequency(task.input_contentTokens, task.tokenLengthMin, task.input_ignoredTokens);
            }
            // <----- test cycle
            bool continueTest = true;
            int validTests = 0;
            int i = 0;
            foreach (string token in evaluation.allContentTokens)
            {
                basicLanguageEnum matchLanguage = basicLanguageEnum.unknown;
                bool isMultiLanguageMatch = false;
                bool isNewUnknownWord = true;

                if (unknownWords.Contains(token))
                {
                    isNewUnknownWord = false;
                }
                else
                {
                    foreach (var pair in languages)
                    {
                        if (pair.Value.isKnownWord(token))
                        {
                            if (matchLanguage == basicLanguageEnum.unknown)
                            {
                                matchLanguage = pair.Key;
                            }
                            else
                            {
                                isMultiLanguageMatch = true;
                                break;
                            }
                        }
                    }
                }

                if (matchLanguage == basicLanguageEnum.serbianCyr)
                {
                    matchLanguage = basicLanguageEnum.serbian;
                }

                if (matchLanguage == basicLanguageEnum.unknown)
                {
                    // <---- no language match
                    if (isNewUnknownWord) unknownWords.Add(token);

                    evaluation.noLanguageTokens.Add(token);
                }
                else
                {
                    if (isMultiLanguageMatch)
                    {
                        evaluation.multiLanguageTokens.Add(token);
                    }
                    else
                    {
                        validTests++;
                        evaluation.singleLanguageTokens.Add(token);
                        evaluation.languageScore.AddInstance(matchLanguage);
                    }
                }

                evaluation.allTestedTokens.Add(token);

                i++;

                if (i >= task.testTokenLimit)
                {
                    continueTest = false;
                    evaluation.comment = evaluation.comment.addLine("Test limit was reached: " + i.ToString());
                }
                if (validTests >= task.validTokenTarget)
                {
                    continueTest = false;
                    evaluation.comment = evaluation.comment.addLine("Valid tokens target reached: " + validTests.ToString() + " after " + i.ToString() + " tests");
                }

                if (!continueTest) break;
            }
            // <----------------------------- end of test cycle

            // <----- eval results

            if (evaluation.languageScore.Count == 0)
            {
                evaluation.comment = evaluation.comment.addLine("None of tokens were recognized by languages used.");
                evaluation.result_language = basicLanguageEnum.unknown;
            }
            else
            {
                var langSorted = evaluation.languageScore.getSorted();
                evaluation.result_language = langSorted.First();

                var builder = new System.Text.StringBuilder();
                builder.Append(evaluation.languageScoreList);

                foreach (basicLanguageEnum id in langSorted)
                {
                    builder.Append(string.Format("{0,20} : {1}", id.ToString(), evaluation.languageScore[id].ToString()) + Environment.NewLine);
                }
                evaluation.languageScoreList = builder.ToString();

                evaluation.result_ratio = (double)evaluation.languageScore[evaluation.result_language] / (double)evaluation.singleLanguageTokens.Count;
            }

            foreach (basicLanguageEnum k in evaluation.languageScore.Keys)
            {
                evaluation.languageEnums.AddUnique(k);
            }

            return evaluation;
        }
    }
}