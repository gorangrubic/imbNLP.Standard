// --------------------------------------------------------------------------------------------------------------------
// <copyright file="textResourceResolverBase.cs" company="imbVeles" >
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
// Project: imbNLP.PartOfSpeech
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
namespace imbNLP.PartOfSpeech.resourceProviders.core
{
    using imbNLP.PartOfSpeech.flags.basic;
    using imbNLP.PartOfSpeech.lexicUnit;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.files.search;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class for a resolver working with a lexical resource encoded in a text format
    /// </summary>
    public abstract class textResourceResolverBase
    {
        /// <summary>
        /// Prepares resource file for querying and loads the grammar tag converter specification
        /// </summary>
        /// <param name="resourceFilePath">The text resource file path</param>
        /// <param name="grammSpecFilename">The Excel file with the grammar tag converter</param>
        /// <param name="output">The output.</param>
        protected void Setup(string resourceFilePath, string grammSpecFilename, ILogBuilder output = null)
        {
            if (resourceFilePath.isNullOrEmpty())
            {
                imbACE.Services.terminal.aceTerminalInput.askYesNo("Resource file path is empty (textResourceResolverBase.Setup)!");
                throw new ArgumentNullException(nameof(resourceFilePath));
                return;
            }

            if (grammSpecFilename.isNullOrEmpty())
            {
                imbACE.Services.terminal.aceTerminalInput.askYesNo("Grammar conversion specification file path is empty (textResourceResolverBase.Setup)!");
                throw new ArgumentNullException(nameof(grammSpecFilename));
                return;
            }

            resourceFileOperater = new fileTextOperater(resourceFilePath, true);

            grammTagConverter = new resourceConverterForGramaticTags();

            if (grammSpecFilename.EndsWith(".xlsx"))
            {
                if (output != null) output.log("Grammar conversion specification loading from Excel file");

                grammTagConverter.LoadSpecificationExcelFile(grammSpecFilename, output);
            }
            else if (grammSpecFilename.EndsWith(".csv"))
            {
                string filebase = Path.GetFileNameWithoutExtension(grammSpecFilename);
                string filepath = Path.GetDirectoryName(grammSpecFilename);

                if (output != null) output.log("Grammar conversion specification loading from CSV files");

                string gramSpecFileFormat = filepath + Path.DirectorySeparatorChar + filebase + "_format.csv";
                string gramSpecFileTranslation = filepath + Path.DirectorySeparatorChar + filebase + "_translation.csv";

                grammTagConverter.LoadSpecificationCSV(gramSpecFileFormat, gramSpecFileTranslation, output);
            }
            else
            {
                if (output != null)
                {
                    output.log("Grammar conversion file format not recognized from the filepath! [" + grammSpecFilename + "]");
                }
                else
                {
                    throw new ArgumentOutOfRangeException("File format not recognized for " + nameof(textResourceResolverBase) + " Setup call.", nameof(grammSpecFilename));
                }
            }
        }

        protected resourceConverterForGramaticTags grammTagConverter { get; set; }

        /// <summary>
        /// Parser for text resource lexic unit definition
        /// </summary>
        /// <param name="line">The lexic unit definition line</param>
        /// <param name="inflectForm">The inflect form of a word</param>
        /// <param name="lemmaForm">The lemma form of a word</param>
        /// <param name="gramTag">String representation of the grammatic information</param>
        protected abstract void SelectFromLine(string line, out string inflectForm, out string lemmaForm, out string gramTag);

        /// <summary>
        /// Builds the search regex pattern for inflected form needle, optionally for lemma needle and gram tag needle
        /// </summary>
        /// <param name="inflectFormNeedle">The inflect form needle.</param>
        /// <param name="lemmaNeedle">The lemma needle.</param>
        /// <param name="gramTagNeedle">The gram tag needle.</param>
        /// <param name="allowPartialInflectedForms">if set to <c>true</c> [allow partial inflected forms].</param>
        /// <param name="allowPartialLemmaForms">if set to <c>true</c> [allow partial lemma forms].</param>
        /// <returns></returns>
        protected abstract string GetSearchRegex(string inflectFormNeedle, string lemmaNeedle = "", string gramTagNeedle = "", Boolean allowPartialInflectedForms = false, Boolean allowPartialLemmaForms = false);

        /// <summary>
        /// Indicates if the instance is ready (connected to the resource file, have gram tag converter ready and etc)
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is ready; otherwise, <c>false</c>.
        /// </value>
        public bool isReady
        {
            get
            {
                if (resourceFileOperater == null) return false;
                if (grammTagConverter == null) return false;
                return true;
            }
        }

        /// <summary>
        /// Default search limit when limit -1 is specified
        /// </summary>
        public const int GENERAL_SEARCH_LIMIT = 25;

        public const int GENERAL_INFLECTION_SEARCH_LIMIT = 10;

        public const int GENERAL_LEXIC_QUERY_SEARCH_LIMIT = 100;

        /// <summary>
        /// Quering the lexic inflections in parallel mode.
        /// </summary>
        /// <param name="words">The words to query inflection graphs for. It may contain duplicates, as it will preprocess list to the unique tokens only.</param>
        /// <param name="limitPerWord">The limit in results per word.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="regOpt">The reg opt.</param>
        /// <returns></returns>
        public lexicGraphSet<lexicInflection> GetLexicInflection(IEnumerable<String> words, int limitPerWord = -1, ILogBuilder logger = null, RegexOptions regOpt = RegexOptions.IgnoreCase)
        {
            lexicGraphSet<lexicInflection> output = new lexicGraphSet<lexicInflection>();
            if (limitPerWord == -1) limitPerWord = GENERAL_INFLECTION_SEARCH_LIMIT;

            List<String> uniqueWords = new List<string>();

            foreach (String word in words)
            {
                uniqueWords.AddUnique(word);
            }

            var needles = new Dictionary<String, String>();

            foreach (String word in uniqueWords)
            {
                needles.Add(GetSearchRegex(word, "", ""), word);
            }
            // IEnumerable<String> __needles, Boolean useRegex = false, StringComparison comparison=StringComparison.CurrentCultureIgnoreCase, RegexOptions regexOptions = RegexOptions.None, Int32 limitResult = -1
            var primResult = resourceFileOperater.Search(needles.Keys, true, regOpt, limitPerWord);

            Parallel.ForEach(primResult, line =>
            {
                String word = needles[line.needle];

                var inflect = new lexicInflection(word);
                inflect = output.GetOrAdd(word, inflect);

                foreach (string content in line.getLineContentList())
                {
                    string inflectForm = "";
                    string lemma = "";
                    string gramTag = "";

                    SelectFromLine(content, out inflectForm, out lemma, out gramTag);

                    var gramTagColl = grammTagConverter.ConvertFromString(gramTag);

                    inflect.AddGrammarCase(gramTagColl);

                    if (inflect.lemmaForm.isNullOrEmpty()) inflect.lemmaForm = lemma;
                }
            });

            return output;
        }

        /// <summary>
        /// Gets multi-lemma in type query graph
        /// </summary>
        /// <param name="word">The word to look for</param>
        /// <param name="limit">The limit - limit on number of entries to take</param>
        /// <param name="logger">The logger - to log out on error</param>
        /// <returns></returns>
        public lexicQuery GetLexicQuery(string word, int limit = -1, ILogBuilder logger = null)
        {
            if (limit == -1) limit = GENERAL_LEXIC_QUERY_SEARCH_LIMIT;

            lexicQuery output = new lexicQuery(word);

            String reg = GetSearchRegex(word, "", "");

            var primResult = resourceFileOperater.Search(reg, true, limit, RegexOptions.IgnoreCase);

            foreach (string line in primResult.getLineContentList())
            {
                string inflectForm = "";
                string lemma = "";
                string gramTag = "";

                SelectFromLine(line, out inflectForm, out lemma, out gramTag);

                var gramTagColl = grammTagConverter.ConvertFromString(gramTag);
                pos_type posType = gramTagColl.Get<pos_type>(pos_type.none);

                lexicLemmaInTypeNode lemmaInType = output.AddLemmaInType(lemma, posType);
                lexicInflection inflection = lemmaInType.AddInflection(inflectForm);
                inflection.AddGrammarCase(gramTagColl);
            }

            return output;
        }

        /// <summary>
        /// Returns single lexicInflection graph, with different grammTags
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="limit">The limit - limit on number of entries to take</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public lexicInflection GetInflectionGraph(String word, int limit = -1, ILogBuilder logger = null)
        {
            if (limit == -1) limit = GENERAL_INFLECTION_SEARCH_LIMIT;

            lexicInflection output = new lexicInflection();
            output.name = word;

            String reg = GetSearchRegex(word, "", "");

            Boolean firstSetup = true;

            var primResult = resourceFileOperater.Search(reg, true, limit, RegexOptions.IgnoreCase);

            foreach (string line in primResult.getLineContentList())
            {
                string inflectForm = "";
                string lemma = "";
                string gramTag = "";

                SelectFromLine(line, out inflectForm, out lemma, out gramTag);

                if (gramTag.isNullOrEmpty())
                {
                    // Something is wrong with gramTag selection
                    String msg = "GramTag failed to be found in [" + line + "] for [" + word + "]";
                    msg += Environment.NewLine + "inflectedForm [" + inflectForm.toStringSafe("not found") + "]";
                    msg += Environment.NewLine + "lemma         [" + lemma.toStringSafe("not found") + "]";
                    msg += Environment.NewLine + "gramTag       [" + gramTag.toStringSafe("not found") + "]";

                    if (logger == null) logger.log(msg);
                }

                var gramTagColl = grammTagConverter.ConvertFromString(gramTag);

                if (firstSetup)
                {
                    output.lemmaForm = lemma;
                    firstSetup = false;
                }

                //pos_type posType = gramTagColl.Get<pos_type>();

                output.AddGrammarCase(gramTagColl);
            }

            return output;
        }

        /// <summary>
        /// Gets grammatic tag collections for specified form, optionally narrowing the search by <see cref="pos_type"/> and/or lemma of preference
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="lemmaOfPreference">The lemma of preference.</param>
        /// <param name="posTypePreference">The position type preference.</param>
        /// <param name="limit">The limit - limit on number of entries to take</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public List<grammaticTagCollection> GetGramTagsFor(string input, string lemmaOfPreference = "", pos_type posTypePreference = pos_type.none, int limit = -1, ILogBuilder logger = null)
        {
            if (limit == -1) limit = GENERAL_SEARCH_LIMIT;

            List<grammaticTagCollection> output = new List<grammaticTagCollection>();

            string posTypeString = grammTagConverter.GetStringFor(posTypePreference, true);
            string reg = GetSearchRegex(input, lemmaOfPreference, posTypeString);
            var result = resourceFileOperater.Search(reg, true, limit, RegexOptions.IgnoreCase);

            if (result.CountThreadSafe == 0)
            {
                if (logger != null) logger.log("No entry found to fit regex  [" + reg + "] using " + GetType().Name);
            }

            foreach (string line in result.getLineContentList())
            {
                string inflectForm = "";
                string lemmaForm = "";
                string gramTag = "";
                SelectFromLine(line, out inflectForm, out lemmaForm, out gramTag);

                output.Add(grammTagConverter.ConvertFromString(gramTag));
            }

            return output;
        }

        /// <summary>
        /// Returns single lemma for specified input word
        /// </summary>
        /// <param name="input">A word to find lemma for</param>
        /// <param name="logger">The logger - to report on fail.</param>
        /// <returns>Lemma form or empty string on failure</returns>
        public string GetLemmaFor(string input, ILogBuilder logger = null)
        {
            string reg = GetSearchRegex(input);
            var result = resourceFileOperater.Search(reg, true, 1, RegexOptions.IgnoreCase);

            //            var result = SearchForFirstMatch(input, logger);
            string inflectForm = "";
            string lemmaForm = "";
            string gramTag = "";

            if (result.CountThreadSafe > 0)
            {
                SelectFromLine(result.First().Value, out inflectForm, out lemmaForm, out gramTag);
            }
            else
            {
                if (logger != null) logger.log("Lemma not found for [" + input + "] using " + GetType().Name);
            }

            return lemmaForm;
        }

        protected fileTextOperater resourceFileOperater { get; set; }
    }
}