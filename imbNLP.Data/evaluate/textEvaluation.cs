// --------------------------------------------------------------------------------------------------------------------
// <copyright file="textEvaluation.cs" company="imbVeles" >
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
    using imbNLP.Data.semanticLexicon;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.text;
    using imbSCI.Data.data;
    using imbSCI.DataComplex;
    using imbSCI.DataComplex.special;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Performs textEvaluation and contains the results
    /// </summary>
    /// <seealso cref="aceCommonTypes.primitives.imbBindable" />
    public class textEvaluation : imbBindable
    {
        /// <summary>
        ///
        /// </summary>
        [XmlIgnore]
        public textEvaluator parent { get; protected set; }

        public textEvaluation()
        {
        }

        public textEvaluation(textEvaluator __parent, string input)
        {
            parent = __parent;
            if (!input.isNullOrEmpty()) prepareText(input);
            //evaluate(input);
        }

        protected void prepareText(string input)
        {
            inputText = input;
            inputText = inputText.Trim();

            inputTokens = inputText.getTokens(true, true, true, true, 4);

            inputText = inputText.Replace(Environment.NewLine, " | ");
        }

        public void evaluateTokens(List<string> tokens, IWeightTable table, bool sortByFrequency = true)
        {
            inputTokens = tokens;
            evaluate(table, sortByFrequency);
        }

        /// <summary>
        /// Evaluates the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        protected void evaluate(IWeightTable table, bool sortByFrequency = true)
        {
            //if (inputText.Contains("protivpožarni"))
            //{
            //}

            if (inputTokens.Count == 0)
            {
                ratioA = 0;
                resultMode = textEvaluationResultEnum.notEnoughInformation;
                return;
            }

            var sort = new List<string>();
            if (sortByFrequency)
            {
                foreach (var s in inputTokens) tokenFrequency.AddInstanceRange(inputTokens);
                //  tokenFrequency.reCalculate();

                sort = tokenFrequency.getSorted();
            }
            else
            {
                sort.AddRange(inputTokens);
            }

            int take = Math.Min(sort.Count(), testSize);

            testTokens.AddRange(sort.Take(take));

            foreach (string tkn in testTokens)
            {
                bool testA = false;
                bool testB = false;

                //if (tkn.)

                testA = basicLanguageTools.testBoolean(languageA, tkn, basicLanguageCheck.spellCheck);

                testB = basicLanguageTools.testBoolean(languageB, tkn, basicLanguageCheck.spellCheck);

                bool testAB = testA && testB;
                bool testNotAB = (!testA) && (!testB);

                while (testNotAB)
                {
                    if (parent.langNotABTokens.Contains(tkn))
                    {
                        testNotAB = true;
                        break;
                    }
                    else
                    {
                        testA = parent.langATokens.Contains(tkn);
                        testB = parent.langBTokens.Contains(tkn);

                        testNotAB = (!testA) && (!testB);

                        if (testNotAB)
                        {
                            lexiconResponse lemmas = parent.manager.getLexiconItems(tkn);
                            if (lemmas.type != lexiconResponse.responseType.failedQueries)
                            {
                                testA = true;
                                parent.langATokens.AddUnique(tkn);
                            }
                        }

                        testNotAB = (!testA) && (!testB);
                        if (testNotAB)
                        {
                            parent.langNotABTokens.AddUnique(tkn);
                        }
                    }

                    testNotAB = (!testA) && (!testB);
                }

                if (testA) langATokens.Add(tkn);
                if (testB) langBTokens.Add(tkn);

                testAB = testA && testB;

                if (testAB)
                {
                    langABTokens.Add(tkn);
                }
                if (testNotAB)
                {
                    langNotABTokens.AddUnique(tkn);
                }
            }

            if (table != null)
            {
                foreach (string tkA in langATokens)
                {
                    double sc = table.GetTF_IDF(tkA);

                    scoreForA += sc + 0.1;
                }

                foreach (string tkB in langBTokens)
                {
                    double sc = table.GetTF_IDF(tkB);
                    scoreForB += sc + 0.1;
                }

                foreach (string tkN in langNotABTokens)
                {
                    double sc = table.GetTF_IDF(tkN);
                    scoreForNotAB += sc + 0.1;
                }
            }
            else
            {
                scoreForA = langATokens.Count();
                scoreForB = langBTokens.Count();
                scoreForNotAB = langNotABTokens.Count();
            }

            //scoreForA = langATokens.Count() + langABTokens.Count();
            //scoreForB = langBTokens.Count() + langABTokens.Count();

            if ((scoreForA > scoreForB) && (scoreForA > scoreForNotAB))
            {
                isLanguageA = true;
                resultMode = textEvaluationResultEnum.languageA;
            }

            if ((scoreForB > scoreForA) && (scoreForB > scoreForNotAB))
            {
                isLanguageB = true;
                resultMode = textEvaluationResultEnum.languageB;
            }

            if ((!isLanguageA) && (!isLanguageB))
            {
                resultMode = textEvaluationResultEnum.noneOfBoth;
            }
            else
            {
                if (scoreForA == scoreForB)
                {
                    resultMode = textEvaluationResultEnum.uncertain;
                }
            }

            if (scoreForA == 0)
            {
                ratioA = 0;
            }
            else
            {
                double div = (double)(scoreForA + scoreForB + scoreForNotAB);
                if (div == 0)
                {
                    ratioA = 1;
                }
                else
                {
                    ratioA = (double)scoreForA / div;
                }
            }
        }

        public string inputText { get; set; }

        public List<string> inputTokens { get; set; } = new List<string>();

        public List<string> testTokens { get; set; } = new List<string>();

        [XmlIgnore]
        public instanceCountCollection<string> tokenFrequency { get; protected set; } = new instanceCountCollection<string>();

        public List<string> langATokens { get; set; } = new List<string>();

        public List<string> langBTokens { get; set; } = new List<string>();

        public List<string> langABTokens { get; set; } = new List<string>();

        public List<string> langNotABTokens { get; set; } = new List<string>();

        public textEvaluationResultEnum resultMode { get; set; } = textEvaluationResultEnum.none;

        public bool isLanguageA { get; set; } = false;
        public bool isLanguageB { get; set; } = false;
        public double scoreForA { get; set; } = 0;
        public double scoreForB { get; set; } = 0;
        public double scoreForNotAB { get; set; } = 0;
        public double ratioA { get; set; } = 0;

        protected basicLanguage languageA => parent.languageA;
        protected basicLanguage languageB => parent.languageB;

        public const int testSize = 10;
    }
}