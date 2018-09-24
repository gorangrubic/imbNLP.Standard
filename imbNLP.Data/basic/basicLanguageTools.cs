// --------------------------------------------------------------------------------------------------------------------
// <copyright file="basicLanguageTools.cs" company="imbVeles" >
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
namespace imbNLP.Data.basic
{
    using imbSCI.Core.extensions.text;

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Staticki alati za rad sa basicLanguage objektima i kolekcijom
    /// </summary>
    public static class basicLanguageTools
    {
        //public static Boolean languageCheck(this String word, imbLanguageFramework.basic.basicLanguage language)
        //{
        //}

        /// <summary>
        /// Izvlaci zajednicki koren iz predlozenih varijacija
        /// </summary>
        /// <param name="suggestSource"></param>
        /// <returns></returns>
        public static string getRootBySuggests(IEnumerable<string> suggestSource)
        {
            foreach (string nvr in suggestSource)
            {
                if (nvr.Contains("-"))
                {
                    string[] sp = nvr.Split('-');
                    return sp[0].Trim();
                }

                if (nvr.Contains(" "))
                {
                    string[] sp = nvr.Split(' ');
                    return sp[0].Trim();
                }
            }
            return "";
        }

        /// <summary>
        /// Izvršava test i vraća rezultat u obliku string reporta
        /// </summary>
        /// <param name="model"></param>
        /// <param name="testWord"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static string testAndReport(this basicLanguage model, string testWord, basicLanguageCheck operation)
        {
            if (model == null)
            {
                return "Selected model is null!" + Environment.NewLine;
            }

            string output = "Language model: " + model.languageEnum.ToString() + Environment.NewLine;

            if (string.IsNullOrEmpty(testWord))
            {
                output += "[error] Test word is empty" + Environment.NewLine;
                return output;
            }
            //  if (!model.checkHuspell()) return null;

            object res = null;

            if (operation == basicLanguageCheck.fullAnalysis)
            {
                basicLanguageCheck[] ops = Enum.GetValues(typeof(basicLanguageCheck)) as basicLanguageCheck[];
                foreach (basicLanguageCheck op in ops)
                {
                    if (op != operation)
                    {
                        output += testAndReport(model, testWord, op) + Environment.NewLine + Environment.NewLine;
                    }
                }
            }
            else
            {
                res = model.test(testWord, operation);
            }

            output += "Test operation: " + operation.ToString() + " >> result: " + res.ToString() + Environment.NewLine;

            List<string> ls = res as List<string>;
            if (ls != null)
            {
                int c = 0;
                foreach (string it in ls)
                {
                    c++;
                    output += "[" + c + "] " + it + Environment.NewLine;
                }
            }

            return output + Environment.NewLine;
        }

        public static bool testBoolean(this basicLanguage model, string testWord, basicLanguageCheck operation)
        {
            object output = test(model, testWord, operation);
            if (output == null) return false;
            return Convert.ToBoolean(output); //output.ConvertTo<Boolean>();
        }

        public static IEnumerable<T> test<T>(this basicLanguage model, string testWord, basicLanguageCheck operation)
            where T : class
        {
            object output = test(model, testWord, operation);

            return output as IEnumerable<T>;
        }

        /// <summary>
        /// Tests sample from provided words
        /// </summary>
        /// <param name="words">The words to take sample from</param>
        /// <param name="take">Words to take</param>
        /// <param name="model">Language model to test against</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="filterSample">if set to <c>true</c> it will skip short words and non-word tokens</param>
        /// <returns>TRUE if more than <c>criteria</c> ratio of taken samples is positive for specified language</returns>
        public static bool languageTestSample(this IEnumerable<string> words, int take, basicLanguage model, double criteria = 0.6, bool filterSample = true)
        {
            bool isSerbian = true;
            List<string> sample = new List<string>();
            int size = take;
            int c = 0;
            foreach (string w in words)
            {
                bool ok = true;

                if (filterSample)
                {
                    if (w.Length < 4) ok = false;
                    if (ok)
                    {
                        ok = w.isCleanWord();
                    }
                }
                if (ok)
                {
                    sample.Add(w);
                    c++;
                }

                if (c == take) break;
            }
            size = c;

            if (size == 0) return false;

            int positive = spellCheckSample(model, sample);

            if (positive == 0) return false;

            double score = (double)positive / (double)size;

            return (score >= criteria);
        }

        /// <summary>
        /// Proverava grupu reci - vraca broj reci koje su prosle spell check
        /// </summary>
        /// <param name="model"></param>
        /// <param name="words"></param>
        /// <returns>Koliko reci je prepoznao</returns>
        public static int spellCheckSample(this basicLanguage model, List<string> words)
        {
            int output = 0;
            foreach (string w in words)
            {
                if (model.testBoolean(w, basicLanguageCheck.spellCheck)) output++;
            }
            return output;
        }

        /// <summary>
        /// Vraca sugestije hunspellEngine-a
        /// </summary>
        /// <param name="model"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public static List<string> getSuggestions(this basicLanguage model, string word)
        {
            return model.hunspellEngine.Suggest(word);
        }

        /// <summary>
        /// Univerzalni test poziv
        /// </summary>
        /// <param name="model"></param>
        /// <param name="testWord"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static object test(this basicLanguage model, string testWord, basicLanguageCheck operation)
        {
            if (!model.checkHuspell())
            {
                return null;
            }

            object output = null;
            if (string.IsNullOrEmpty(testWord))
            {
                //model.note(devNoteType.nlp,
                //           "Test word is null - operation: " + operation.ToString() + " language:[" + model.iso2Code +
                //           "] ", "basicLanguageTools.test()");
                //logSystem.log("Test word is empty", logType.Warning);

                return null;
            }

            try
            {
                switch (operation)
                {
                    case basicLanguageCheck.fullAnalysis:
                        List<object> outlist = new List<object>();
                        outlist.Add(model.hunspellEngine.Spell(testWord));
                        outlist.Add(model.hunspellEngine.Analyze(testWord));
                        outlist.Add(model.hunspellEngine.Stem(testWord));
                        outlist.Add(model.hunspellEngine.Suggest(testWord));
                        return outlist;
                        break;

                    case basicLanguageCheck.spellCheck:
                        return model.hunspellEngine.Spell(testWord);
                        break;

                    case basicLanguageCheck.analyze:
                        return model.hunspellEngine.Analyze(testWord);
                        break;

                    case basicLanguageCheck.getStems:
                        return model.hunspellEngine.Stem(testWord);
                        break;

                    case basicLanguageCheck.getVariations:
                        return model.hunspellEngine.Suggest(testWord);
                        break;

                    default:
                        return "Not supported operation";
                        break;
                }
            }
            catch (Exception ex)
            {
                //  logSystem.log(ex.Message, logType.Warning);
                throw;
            }

            return output;
        }
    }
}