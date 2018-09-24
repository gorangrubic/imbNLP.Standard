// --------------------------------------------------------------------------------------------------------------------
// <copyright file="semanticLexiconCache.cs" company="imbVeles" >
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
namespace imbNLP.Data.semanticLexicon
{
    using imbACE.Core.core;
    using imbACE.Services.terminal;
    using imbNLP.Data.semanticLexicon.core;
    using imbNLP.Data.semanticLexicon.term;
    using imbNLP.PartOfSpeech.flags.basic;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.unit;
    using imbSCI.Core.reporting;
    using imbSCI.Data.collection.nested;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///
    /// </summary>
    public class semanticLexiconCache
    {
        public semanticLexiconCache(folderNode __folder, bool __allowLexiconQuery = true)
        {
            folder = __folder;
            doAllowLexiconQuery = __allowLexiconQuery;
        }

        protected folderNode folder { get; set; }

        /// <summary> </summary>
        protected fileunit failedQueries { get; set; }

        /// <summary> </summary>
        public int lemmaCount { get; protected set; } = 0;

        /// <summary> </summary>
        public int instanceCount { get; protected set; } = 0;

        /// <summary> </summary>
        public int conceptCount { get; protected set; } = 0;

        public int failedCount => failedQueries.contentLines.Count;

        public void prepare(ILogBuilder loger, semanticLexiconContext context, bool preload = false)
        {
            aceLog.consoleControl.setAsOutput(loger as IConsoleControl, "LexCache");
            lexiconContext = context;

            loger.log("Counting lexic resources...");
            lemmaCount = context.TermLemmas.Count();
            instanceCount = context.TermInstances.Count();
            conceptCount = context.Concepts.Count();

            twinsSavePath = folder.pathFor(FILENAME_TWINS);

            loger.log("Loading cache files");
            LoadCacheFiles(loger, context);

            AppendStatus(loger);

            if (preload) preloadLexicon(loger, context);
        }

        /// <summary> </summary>
        public string twinsSavePath { get; protected set; } = "";

        public const string FILENAME_TWINS = "lexiconCache_codingTwins.txt";

        protected void LoadCacheFiles(ILogBuilder loger, semanticLexiconContext context)
        {
            failedQueries = new fileunit(folder.pathFor("lexiconCache_negatives.txt"), true);

            loger.log("Negative queries loaded");
            AddTemp(failedQueries.contentLines, loger, true, true);
            loger.log("Loading encoding twins");
            twins.Load(twinsSavePath, loger);
            loger.log("Encoding twins loaded");

            if (twins.Count == 0) rebuildEncodedTwins(loger, context);

            failedQueries.Save();
        }

        private object SaveCacheLock = new object();

        protected void SaveCacheFiles()
        {
            lock (SaveCacheLock)
            {
                failedQueries.Save();
                twins.Append(twinsSavePath);
            }
        }

        protected const int ADDTEMPTOSAVE = 10;

        protected int addTempToSave { get; set; } = 0;

        protected void AppendStatus(ILogBuilder loger)
        {
            loger.AppendLine("Semantic Lexicon => Lemma[" + lemmaCount + "] Instance[" + instanceCount + "] Concept[" + conceptCount + "]");
            loger.AppendLine("Semantic Lexicon Cache => FQueries[" + failedCount + "]:[" + failed.Count + "] encoding twins[" + twins.LoadCount + "]");
        }

        protected ITermLemma makeTempLemma(string term)
        {
            var md = new TempLemma(term);

            md.type = pos_type.TEMP.ToString();

            return md;
        }

        protected semanticLexiconContext lexiconContext { get; set; }

        /// <summary>
        /// Rebuilds the encoded twins.
        /// </summary>
        /// <param name="loger">The loger.</param>
        /// <param name="context">The context.</param>
        public void rebuildEncodedTwins(ILogBuilder loger, semanticLexiconContext context)
        {
            twins.Clear(true);

            loger.log("Rebuilding Semantic Lexicon lower encoding twins");
            double ratio = 0;
            int i = 0;
            int repIndex = lemmaCount / 20;
            foreach (ITermLemma lemma in context.TermLemmas)
            {
                string nl = "";
                if (lemma.name.isNonDosChars())
                {
                    twins.SetWord(lemma.name);
                }

                foreach (ITermInstance instance in lemma.instances)
                {
                    if (instance.name.isNonDosChars())
                    {
                        twins.SetWord(instance.name);
                    }
                }

                i++;
                repIndex--;
                if (repIndex == 0)
                {
                    // twins.Save();
                    repIndex = lemmaCount / 20;
                    ratio = (double)i / (double)lemmaCount;
                    loger.log("Recoded [" + i + "] lemmas (" + ratio.ToString("P2") + ")");
                }
            }
            twins.Save();
            loger.log("Rebuilding Semantic Lexicon lower encoding twins");
        }

        public bool isLexiconPreloaded { get; protected set; }

        /// <summary>
        /// Preloads the lexicon.
        /// </summary>
        /// <param name="loger">The loger.</param>
        /// <param name="context">The context.</param>
        public void preloadLexicon(ILogBuilder loger, semanticLexiconContext context)
        {
            loger.log("Preloading Semantic Lexicon");
            if (isLexiconPreloaded)
            {
                loger.log("Semantic Lexicon is already loaded");
                return;
            }
            double ratio = 0;
            int i = 0;
            int repIndex = lemmaCount / 20;
            foreach (ITermLemma lemma in context.TermLemmas)
            {
                Add(lemma);
                i++;
                repIndex--;
                if (repIndex == 0)
                {
                    repIndex = lemmaCount / 20;
                    ratio = (double)i / (double)lemmaCount;
                    loger.log("Loaded [" + i + "] lemmas (" + ratio.ToString("P2") + ")");
                }
            }
            doAllowLexiconQuery = false;
            isLexiconPreloaded = true;
            loger.log("Semantic Lexicon preload done");
        }

        /// <summary>
        /// Goes automatically <c>false</c> once the lexicon is preloaded
        /// </summary>
        /// <value>
        /// <c>true</c> if [do allow lexicon query]; otherwise, <c>false</c>.
        /// </value>
        public bool doAllowLexiconQuery { get; protected set; } = true;

        /// <summary>
        /// Gets the lemmas with root.
        /// </summary>
        /// <param name="rootWord">The root word.</param>
        /// <param name="loger">The loger.</param>
        /// <returns></returns>
        public List<ITermLemma> getLemmasWithRoot(string rootWord, ILogBuilder loger = null)
        {
            var keys = lemmas.Keys.Where(x => x.StartsWith(rootWord));

            List<ITermLemma> output = new List<ITermLemma>();

            foreach (string key in keys)
            {
                output.AddRange(lemmas[key]);
            }

            return output;
        }

        /// <summary>
        /// Gets the lexicon items from cached terms or failed queris
        /// </summary>
        /// <param name="termForm">The term form.</param>
        /// <returns></returns>
        public lexiconResponse getLexiconItems(string termForm, ILogBuilder loger = null)
        {
            lexiconResponse output = new lexiconResponse();
            string termFormLat = termForm;

            if (semanticLexiconManager.manager.settings.doQueryPreprocess)
            {
                termForm = termForm.ToLower().Trim();
                //  termFormLat = termForm.transliterate(transliterateFlag.cyrilicToLatin);
            }

            termForm = termFormLat;

            output.setType(lexiconResponse.responseType.none);

            if (failed.ContainsKey(termForm))
            {
                output = failed[termForm];
                output.setType(lexiconResponse.responseType.failedQueries);
            }
            else if (lemmas.ContainsKey(termForm))
            {
                output.AddRange(lemmas[termForm]);
                output.setType(lexiconResponse.responseType.cachedLexicon);
            }
            else if (instances.ContainsKey(termForm))
            {
                output.AddRange(instances[termForm]);
                output.setType(lexiconResponse.responseType.cachedLexicon);
            }
            else
            {
                List<string> termForms = new List<string>();
                termForms.Add(termForm);
                translationTextTableEntryEnum entry = twins.checkForEntry(termForm);

                switch (entry)
                {
                    case translationTextTableEntryEnum.keyEntry:
                    case translationTextTableEntryEnum.valueEntry:
                        termForms.Add(twins.GetWord(termForm, out entry));
                        break;

                    case translationTextTableEntryEnum.unknownEntry:
                        if (termForm.isNonDosChars())
                        {
                            termForms.Add(twins.GetWord(termForm, out entry));
                        }
                        break;
                }

                if (doAllowLexiconQuery && !isLexiconPreloaded)
                {
                    output.setType(lexiconResponse.responseType.askingLexiconContext);
                    output = _getLexiconItems(termForms, loger);
                }

                if (output.Any())
                {
                    output.setType(lexiconResponse.responseType.lexicon);
                    Add(output);
                }
                else
                {
                    output = AddTemp(termForms, loger);
                    output.setType(lexiconResponse.responseType.failedQueries);
                }
            }
            //} catch (Exception ex)
            //{
            //    aceGeneralException axe = new aceGeneralException("semanticLexiconCache.getLexiconItems error", ex, this, "lexCache.getLexiconItems() current state: " + output.type);

            //    throw axe;
            //}
            return output;
        }

        private object getLexiconItemsLock = new object();

        /// <summary>
        /// Gets the lexicon items.
        /// </summary>
        /// <param name="termForm">The term form.</param>
        /// <param name="loger">The loger.</param>
        /// <returns></returns>
        private lexiconResponse _getLexiconItems(List<string> termForms, ILogBuilder loger = null, bool callWithAutoDiscovery = true)
        {
            lexiconResponse output = new lexiconResponse();
            //lock (getLexiconItemsLock)
            //{
            output.setType(lexiconResponse.responseType.lexicon);

            ILexiconItem tis_i = null;

            try
            {
                //var tis_is = lexiconContext.TermInstances.Where(x => x.Equals(termForm)); // x=>x.name == termForm
                var tis_is = lexiconContext.TermInstances.Where(x => termForms.Contains(x.name)).FirstOrDefault(); //
                if (tis_is != null)
                {
                    output.Add(tis_is);
                }
            }
            catch (Exception ex)
            {
                if (loger != null) loger.log("Lexicon query failed: " + termForms.toCsvInLine(",") + " (lemmas)");
            }

            if (tis_i == null)
            {
                try
                {
                    var til_is = lexiconContext.TermLemmas.Where(x => termForms.Contains(x.name)).FirstOrDefault(); // var til_is = lexiconContext.TermLemmas.Where(x => x.Equals(termForm));

                    if (til_is != null)
                    {
                        output.Add(til_is);
                    }
                }
                catch (Exception ex)
                {
                    if (loger != null) loger.log("Lexicon query failed: " + termForms.toCsvInLine(",") + " (instances)");
                }
            }
            //}

            return output;
        }

        /// <summary>
        /// If a twin detected it is decoded into proper encoding token:  unicode -> ASCII
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <returns>changed or unchanged tokens</returns>
        public List<string> decodeTwins(IEnumerable<string> tokens)
        {
            List<string> output = new List<string>();

            foreach (string tkn in tokens)
            {
                string tkn2 = tkn.ToLower().Trim();
                var response = twins.checkForEntry(tkn2);

                switch (response)
                {
                    case translationTextTableEntryEnum.keyEntry:
                        output.Add(twins.byKeys[tkn2]);
                        break;

                    case translationTextTableEntryEnum.valueEntry:
                        output.Add(tkn2);
                        break;

                    default:
                    case translationTextTableEntryEnum.newEntry:
                        output.Add(tkn2);
                        break;
                }
            }

            return output;
        }

        /// <summary>
        /// If a twin detected it is decoded into proper encoding token: ASCII -> unicode
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <returns>changed or unchanged tokens</returns>
        public List<string> encodeTwins(IEnumerable<string> tokens)
        {
            List<string> output = new List<string>();

            foreach (string tkn in tokens)
            {
                string tkn2 = tkn.ToLower().Trim();
                var response = twins.checkForEntry(tkn2);

                switch (response)
                {
                    case translationTextTableEntryEnum.keyEntry:
                        output.Add(tkn2);
                        break;

                    case translationTextTableEntryEnum.valueEntry:
                        output.Add(twins.byValues[tkn2]);
                        break;

                    default:
                    case translationTextTableEntryEnum.newEntry:
                        output.Add(tkn2);
                        break;
                }
            }

            return output;
        }

        /// <summary>
        /// Adds the temporary term definition and performs save once enough temp terms were aded
        /// </summary>
        /// <param name="terms">The terms.</param>
        /// <param name="loger">The loger.</param>
        /// <param name="insert">if set to <c>true</c> [insert].</param>
        /// <returns></returns>
        public lexiconResponse AddTemp(IEnumerable<string> terms, ILogBuilder loger = null, bool insert = true, bool isFirstLoad = false)
        {
            lexiconResponse output = new lexiconResponse();
            output.setType(lexiconResponse.responseType.failedQueries);

            if (insert)
            {
                foreach (string term in terms)
                {
                    var tmp = makeTempLemma(term);
                    output.Add(tmp);
                    failed.Add(term, tmp);
                    if (!isFirstLoad) failedQueries.AppendUnique(terms);
                    addTempToSave += 1;
                }
            }

            if (addTempToSave > ADDTEMPTOSAVE)
            {
                addTempToSave = 0;
                if (!isFirstLoad) SaveCacheFiles();
            }

            return output;
        }

        /// <summary>
        /// Adds the temporary.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="loger">The loger.</param>
        /// <returns></returns>
        public lexiconResponse AddTemp(string term, ILogBuilder loger = null, bool insert = true) => AddTemp(new string[] { term }, loger, insert);

        public const int retryLimit = 5;

        /// <summary>
        /// Sets the item into cache
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(ILexiconItem item)
        {
            int retryIndex = 0;

            while (retryIndex < retryLimit)
            {
                try
                {
                    translationTextTableEntryEnum entry = translationTextTableEntryEnum.none;
                    if (item is ITermLemma)
                    {
                        ITermLemma lemma = (ITermLemma)item;
                        lemmas.AddUnique(item.name, lemma);
                        if (item.name.isNonDosChars()) lemmas.AddUnique(twins.GetWord(item.name, out entry), lemma);

                        foreach (TermInstance instance in lemma.instances)
                        {
                            instances.AddUnique(item.name, instance);
                            if (item.name.isNonDosChars()) instances.AddUnique(twins.GetWord(item.name, out entry), instance);
                        }
                    }
                    else if (item is ITermInstance)
                    {
                        instances.AddUnique(item.name, item as ITermInstance);
                        if (item.name.isNonDosChars())
                        {
                            instances.AddUnique(twins.GetWord(item.name, out entry), item as ITermInstance);
                        }
                    }
                    retryIndex = retryLimit + 1;
                }
                catch (Exception ex)
                {
                    aceLog.log("SemanticLexiconCache->Add(" + item.name + ") failed: " + ex.Message + "  Retries left[" + (retryLimit - retryIndex) + "]", null, true);
                    aceTerminalInput.askPressAnyKeyInTime("-- cooldown delay -- press any key to skip --", true, 3, false, 1);
                    retryIndex++;
                }
            }
        }

        /// <summary>
        /// Adds the specified lexicon items into cache
        /// </summary>
        /// <param name="items">The items.</param>
        public void Add(IEnumerable<ILexiconItem> items)
        {
            foreach (ILexiconItem item in items)
            {
                Add(item);
            }
        }

        protected aceDictionarySet<string, ILexiconItem> failed { get; set; } = new aceDictionarySet<string, ILexiconItem>();

        /// <summary> </summary>
        protected aceDictionarySet<string, ITermLemma> lemmas { get; set; } = new aceDictionarySet<string, ITermLemma>();

        /// <summary> </summary>
        protected aceDictionarySet<string, ITermInstance> instances { get; set; } = new aceDictionarySet<string, ITermInstance>();

        protected translationTextTable twins { get; set; } = new translationTextTable(imbStringCleaners.toDosCleanDirect);
    }
}