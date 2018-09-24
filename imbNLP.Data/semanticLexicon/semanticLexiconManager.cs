// --------------------------------------------------------------------------------------------------------------------
// <copyright file="semanticLexiconManager.cs" company="imbVeles" >
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
    using imbACE.Core.core.exceptions;
    using imbNLP.Data.extended.dict.core;
    using imbNLP.Data.semanticLexicon.console;
    using imbNLP.Data.semanticLexicon.core;
    using imbNLP.Data.semanticLexicon.explore;
    using imbNLP.Data.semanticLexicon.procedures;
    using imbNLP.Data.semanticLexicon.source;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.table;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.search;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public class semanticLexiconManager : tokenQueryResolverBase
    {
        public const int CULTURE_ID_SerbianLatin = 6170;
        public const int CULTURE_ID_English = 1033;

        private static semanticLexiconManager _manager;

        /// <summary>
        /// The main instance of semantic lexicon manager
        /// </summary>
        public static semanticLexiconManager manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = new semanticLexiconManager("");
                }
                return _manager;
            }
        }

        public semanticLexiconManager(string __workspace)
        {
            workspaceFolderPath = __workspace;
        }

        private string _workspaceFolderPath;

        /// <summary>
        ///
        /// </summary>
        public string workspaceFolderPath
        {
            get
            {
                if (_workspaceFolderPath.isNullOrEmpty()) return "projects\\";
                return _workspaceFolderPath;
            }
            set { _workspaceFolderPath = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public lexiconConsole console { get; set; }

        private lexiconConsoleSettings _consoleSettings;

        #region imbObject Property <lexiconConsoleSettings> consoleSettings

        /// <summary>
        /// imbControl property consoleSettings tipa lexiconConsoleSettings
        /// </summary>
        public lexiconConsoleSettings consoleSettings
        {
            get
            {
                if (_consoleSettings == null)
                {
                    _consoleSettings = new lexiconConsoleSettings();
                    _consoleSettings.Load();
                }

                return _consoleSettings;
            }
            set
            {
                _consoleSettings = value;
            }
        }

        #endregion imbObject Property <lexiconConsoleSettings> consoleSettings

        #region imbObject Property <lexiconConstructor> constructor

        /// <summary>
        /// imbControl property constructor tipa lexiconConstructor
        /// </summary>
        public lexiconConstructor constructor { get; set; }

        #endregion imbObject Property <lexiconConstructor> constructor

        private semanticLexiconManagerSettings _settings;

        #region imbObject Property <semanticLexiconManagerSettings> settings

        /// <summary>
        /// imbControl property settings tipa semanticLexiconManagerSettings
        /// </summary>
        public semanticLexiconManagerSettings settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new semanticLexiconManagerSettings();
                    _settings.Load();
                    _settings.checkDefaults();
                    _settings.Save();
                }

                return _settings;
            }
            set
            {
                _settings = value;
            }
        }

        #endregion imbObject Property <semanticLexiconManagerSettings> settings

        private lexiconConstructionSettings _constructionSettings;

        /// <summary>
        ///
        /// </summary>
        public lexiconConstructionSettings constructionSettings
        {
            get
            {
                if (_constructionSettings == null)
                {
                    _constructionSettings = new lexiconConstructionSettings();
                    _constructionSettings.Load();
                }

                return _constructionSettings;
            }
            set { _constructionSettings = value; }
        }

        /// <summary> </summary>
        public semanticLexiconContext lexiconContext { get; set; }

        public override bool isReady
        {
            get
            {
                if (console == null) return false;
                if (lexiconContext == null) return false;
                return true;
            }
        }

        public override tokenQueryResponse exploreToken(tokenQuery query)
        {
            throw new NotImplementedException();
        }

        #region Resolve and Expand

        /// <summary>
        /// Resolves the specified token into one or more <see cref="TermLemma"/>s
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public List<TermLemma> resolve(string token)
        {
            var tkns = new List<string>();
            tkns.Add(token);
            return resolve(tkns);
        }

        /// <summary>
        /// Resolves the tokens into list of TermLemma instances
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <returns></returns>
        public List<TermLemma> resolve(List<string> tokens)
        {
            List<string> tokens_small = new List<string>();
            tokens.ForEach(x => tokens_small.Add(x.ToLower()));

            List<TermLemma> output = new List<TermLemma>();

            var tls_d = lexiconContext.TermLemmas.Where(x => tokens_small.Contains(x.name));

            if (tls_d.Count() > 0)
            {
                foreach (var tls_di in tls_d)
                {
                    output.AddUnique(tls_di);
                    tokens_small.Remove(tls_di.name);
                }
            }

            if (tokens_small.Any())
            {
                var tis_i = lexiconContext.TermInstances.Where(x => tokens_small.Contains(x.name));

                /*
                foreach (var tls_di in tls_d)
                {
                    output.AddUnique(tls_di);
                }*/

                foreach (var tis_ii in tis_i)
                {
                    output.AddUnique(tis_ii.lemma);
                }
            }
            return output;
        }

        /// <summary>
        /// Gets the lemmas from <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="lemmaForms">The lemma forms.</param>
        /// <returns></returns>
        public List<ITermLemma> getLemmas(IEnumerable<string> lemmaForms)
        {
            List<ITermLemma> output = new List<ITermLemma>();
            foreach (string lm in lemmaForms)
            {
                output.AddRange(getLemma(lm), true);
            }
            return output;
        }

        /// <summary>
        /// Gets the lemma from string
        /// </summary>
        /// <param name="lemmaForm">The lemma form.</param>
        /// <returns></returns>
        public List<ITermLemma> getLemma(string lemmaForm)
        {
            lemmaForm = lemmaForm.ToLower();
            List<ITermLemma> output = new List<ITermLemma>();
            IQueryable<ITermLemma> tls = lexiconContext.TermLemmas.Where(x => x.name == lemmaForm);

            foreach (ITermLemma tls_di in tls)
            {
                output.AddUnique(tls_di);
            }

            return output;
        }

        /// <summary>
        /// Gets of builds the concept
        /// </summary>
        /// <param name="conceptName">Name of the concept.</param>
        /// <param name="autoBuild">if set to <c>true</c> [automatic build].</param>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        public Concept getConcept(string conceptName, bool autoBuild = false, string description = "", bool autoSave = true)
        {
            var lcs = lexiconContext.Concepts.Where(x => x.name == conceptName);

            foreach (Concept tls_di in lcs)
            {
                return tls_di;
            }

            if (autoBuild)
            {
                Concept concept = lexiconContext.Concepts.Create() as Concept;
                concept.name = conceptName;
                concept.description = description;
                if (autoSave) lexiconContext.SaveChanges();

                return concept;
            }

            return null;
        }

        private termExplorer _explorer;

        /// <summary>
        ///
        /// </summary>
        public termExplorer explorer
        {
            get
            {
                if (_explorer == null)
                {
                    _explorer = new termExplorer(workspaceFolderPath);
                    _explorer.manager = this;
                }
                return _explorer;
            }
            set { _explorer = value; }
        }

        protected bool lexMatch(ILexiconItem x, string termForm)
        {
            if (x == null)
            {
                //aceLog.loger.consoleAltColorToggle();

                aceLog.log("Lexicon item is null -- " + termForm);

                // aceLog.loger.consoleAltColorToggle();

                throw new aceGeneralException("Lexicon item is null", null, this, "lexMatch (" + termForm + ")");

                return false;
            }

            if (x.name == termForm)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// static and autoinitiated object
        /// </summary>
        public static semanticLexiconCache lexiconCache { get; private set; }

        /// <summary>
        /// Gets the lexicon items.
        /// </summary>
        /// <param name="termForm">The term form.</param>
        /// <param name="loger">The loger.</param>
        /// <param name="callWithAutoDiscovery">if set to <c>true</c> [call with automatic discovery].</param>
        /// <returns></returns>
        public lexiconResponse getLexiconItems(string termForm, ILogBuilder loger = null, bool callWithAutoDiscovery = true)
        {
            //if (!settings.DoInMemoryCache)
            //{
            //    return _getLexiconItems(termForm, loger, callWithAutoDiscovery);
            //}

            lexiconResponse output = lexiconCache.getLexiconItems(termForm);

            //if (!output.Any())
            //{
            //    output = _getLexiconItems(termForm, loger, callWithAutoDiscovery);
            //    if (!output.Any())
            //    {
            //        output = lexiconCache.AddTemp(termForm, loger);
            //    } else
            //    {
            //        lexiconCache.Add(output);
            //    }

            //}

            return output;
        }

        /// <summary>
        /// Gets the lexicon items.
        /// </summary>
        /// <param name="termForm">The term form.</param>
        /// <param name="loger">The loger.</param>
        /// <returns></returns>
        private List<ILexiconItem> _getLexiconItems(string termForm, ILogBuilder loger = null, bool callWithAutoDiscovery = true)
        {
            List<ILexiconItem> output = new List<ILexiconItem>();
            termForm = termForm.ToLower().Trim();

            ILexiconItem tis_i = null;

            try
            {
                //var tis_is = lexiconContext.TermInstances.Where(x => x.Equals(termForm)); // x=>x.name == termForm
                var tis_is = lexiconContext.TermInstances.Where(x => x.name == termForm); //
                if (tis_is != null)
                {
                    foreach (var item in tis_is)
                    {
                        tis_i = item;
                    }
                }
            }
            catch (Exception ex)
            {
                if (loger != null) loger.log("Lexicon query failed: " + termForm + " (lemmas)");
            }

            if (tis_i != null)
            {
                output.Add(tis_i);
            }
            else
            {
                // ITermLemma til_i = null;
                try
                {
                    var til_is = lexiconContext.TermLemmas.Where(x => x.name == termForm); // var til_is = lexiconContext.TermLemmas.Where(x => x.Equals(termForm));
                    if (til_is != null)
                    {
                        int c = 0;
                        foreach (var item in til_is)
                        {
                            output.Add(item);
                            c++;
                            if (c > 10)
                            {
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (loger != null) loger.log("Lexicon query failed: " + termForm + " (instances)");
                }

                //if (til_i != null) output.Add(til_i);
            }

            if (output.Any())
            {
                return output;
            }

            if (loger == null)
            {
                if (settings.doResolveWordsInDebugMode) loger = constructor.output;
            }

            if (callWithAutoDiscovery)
            {
                if (settings.doAllowLexiconSaveOnExplore)
                {
                    output = explorer.exploreAndSave(termForm, loger, true, settings.doResolveWordsInDebugMode, termExploreItemEnumFlag.namedEntity, termExploreItemEnumFlag.aper, termExploreItemEnumFlag.srWNet, termExploreItemEnumFlag.termExplorer);
                }
                else
                {
                    //output = explorer.exploreAndSave(termForm, loger, true, settings.doResolveWordsInDebugMode, termExploreItemEnumFlag.none);
                }
            }

            return output;
        }

        #endregion Resolve and Expand

        public DataTable getLemmaStats(ILogBuilder log)
        {
            DataTable output = new DataTable("LexiconLemmas");
            output.SetDescription("List of all lemma definition contained in the lexicon");

            var dc_lemma = output.Columns.Add("Lemma");
            var dc_type = output.Columns.Add("Type");
            var dc_relTo = output.Columns.Add("RelatedTo").SetValueType(typeof(int));
            var dc_relFrom = output.Columns.Add("RelatedFrom").SetValueType(typeof(int));
            var dc_instances = output.Columns.Add("Instances").SetValueType(typeof(int));
            var dc_concepts = output.Columns.Add("Concepts").SetValueType(typeof(int));
            var dc_conn = output.Columns.Add("Concept_names");
            var dc_reln = output.Columns.Add("Relationships");
            var dc_relni = output.Columns.Add("Relationships_and_instances");

            int i = 0;
            int index = 0;
            int end = lexiconContext.TermLemmas.Count();
            int step = end / 20;
            foreach (ITermLemma lemma in lexiconContext.TermLemmas)
            {
                var dr = output.NewRow();
                dr[0] = lemma.name;
                dr[1] = lemma.type;
                dr[2] = lemma.relatedTo.Count;
                dr[3] = lemma.relatedFrom.Count;
                dr[4] = lemma.instances.Count;
                dr[5] = lemma.concepts.Count;
                string connames = "";
                foreach (IConcept c in lemma.concepts) connames = connames.add(c.name, ",");
                dr[6] = connames;
                int rel = lemma.relatedTo.Count + lemma.relatedFrom.Count + lemma.concepts.Count;
                dr[7] = rel;
                dr[8] = rel + lemma.instances.Count;
                output.Rows.Add(dr);
                i++;
                index++;
                if (index > step)
                {
                    index = 0;
                    double r = ((double)i) / ((double)end);
                    log.Append(" [" + r.ToString("P") + "] ");
                }
            }

            output.AddExtra("Total lemmas: " + end);

            int noConRel = output.Select(dc_concepts.ColumnName + " = 0").count();
            int noTotRel = output.Select(dc_reln.ColumnName + " = 0").count();

            output.AddExtra("Without concept relationship: " + noConRel + " (" + noConRel.imbGetPercentage(end) + ")");
            output.AddExtra("Without any relationship: " + noTotRel + " (" + noTotRel.imbGetPercentage(end) + ")");

            return output;
        }

        public DataTable getConceptStats(ILogBuilder log)
        {
            DataTable output = new DataTable("LexiconConcepts");
            output.SetDescription("List of all concept definition contained in the lexicon");

            var dc_lemma = output.Columns.Add("Concept");
            var dc_desc = output.Columns.Add("Description");
            var dc_relTo = output.Columns.Add("RelatedTo").SetValueType(typeof(int));
            var dc_relFrom = output.Columns.Add("RelatedFrom").SetValueType(typeof(int));
            var dc_hyper = output.Columns.Add("Hyper");
            var dc_hypo = output.Columns.Add("Hypo").SetValueType(typeof(int));
            var dc_lemmas = output.Columns.Add("Lemmas").SetValueType(typeof(int));
            output.Columns.Add("Lemmas names");

            var dc_conn = output.Columns.Add("Concept_relationships").SetValueType(typeof(int));
            var dc_reln = output.Columns.Add("Total_relationships").SetValueType(typeof(int));

            int i = 0;
            int index = 0;

            int end = lexiconContext.Concepts.Count();

            int step = end / 20;
            foreach (IConcept lemma in lexiconContext.Concepts)
            {
                var dr = output.NewRow();
                dr[0] = lemma.name;
                dr[1] = lemma.description;
                dr[2] = lemma.relatedTo.Count;
                dr[3] = lemma.relatedFrom.Count;
                int rel = 0;
                if (lemma.hyperConcept != null)
                {
                    rel++;
                    dr[4] = lemma.hyperConcept.name;
                }
                else
                {
                    dr[4] = "[none]";
                }

                dr[5] = lemma.hypoConcepts.Count;
                dr[6] = lemma.lemmas.Count;

                string connames = "";
                foreach (ITermLemma c in lemma.lemmas) connames = connames.add(c.name, ",");

                dr[7] = connames;
                rel += lemma.relatedTo.Count + lemma.relatedFrom.Count + lemma.lemmas.Count;
                dr[8] = rel;
                rel += lemma.lemmas.Count;
                dr[9] = rel;

                output.Rows.Add(dr);

                i++;
                index++;
                if (index > step)
                {
                    double r = ((double)i) / ((double)end);
                    log.Append(" [" + r.ToString("P") + "] ");
                    index = 0;
                }
            }
            output.AddExtra("Total concept: " + end);

            int noConRel = output.Select(dc_conn.ColumnName + " = 0").count();
            int noTotRel = output.Select(dc_reln.ColumnName + " = 0").count();

            output.AddExtra("Without concept relationship: " + noConRel + " (" + noConRel.imbGetPercentage(end) + ")");
            output.AddExtra("Without any relationship: " + noTotRel + " (" + noTotRel.imbGetPercentage(end) + ")");
            return output;
        }

        /// <summary>
        /// Search the source
        /// </summary>
        /// <param name="needles">The needles.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="useRegex">if set to <c>true</c> [use regex].</param>
        /// <returns></returns>
        public fileTextSearchMultiSourceSet sourceSearch(IEnumerable<string> needles, lexiconSourceTypeEnum sourceType, bool useRegex = false)
        {
            List<string> filepaths = settings.sourceFiles.getFilePaths(sourceType);

            fileTextOperaterMulti fileOperaters = new fileTextOperaterMulti(filepaths);
            fileTextSearchMultiSourceSet fileMultiSourceSet = new fileTextSearchMultiSourceSet(needles, filepaths, useRegex);

            foreach (var fop in fileOperaters)
            {
                fileMultiSourceSet[fop.Key] = fop.Value.Search(needles, useRegex);
            }
            return fileMultiSourceSet;
        }

        public void prepareCache(ILogBuilder loger, folderNode folder)
        {
            lexiconCache = new semanticLexiconCache(folder, !settings.DoPreloadLexicon);
            lexiconCache.prepare(loger, lexiconContext, settings.DoPreloadLexicon);
        }

        /// <summary>
        /// Prepares this instance.
        /// </summary>
        public override void prepare()
        {
            if (isReady) return;

            imbLanguageFrameworkManager.log.log("Semantic lexicon manager prepare start");

            constructionSettings.Poke();
            settings.Poke();

            if (constructor == null)
            {
                constructor = new lexiconConstructor(constructionSettings);
            }

            var missing = settings.sourceFiles.checkMissingFiles();
            foreach (string mfile in missing)
            {
                imbLanguageFrameworkManager.log.log("Source file missing: " + mfile);
            }
            if (missing.Any())
            {
                aceGeneralException axe = new aceGeneralException("Lexicon source files missing", null, missing, "Lexicon source files missing");
                throw axe;
            }

            consoleSettings.Poke();

            console = new lexiconConsole(consoleSettings.defaultSession, consoleSettings);
            semanticLexiconExtensions.SetBrightStarDB();

            //String conString = "type=rest;storesdirectory=G:\\BrightStarDB\\;storename=lex";

            string conString = @"type=rest;endpoint=http://localhost:8090/brightstar;storename=lex";

            lexiconContext = new semanticLexiconContext(conString);

            imbLanguageFrameworkManager.log.log("Semantic lexicon manager prepare finished");
        }
    }
}