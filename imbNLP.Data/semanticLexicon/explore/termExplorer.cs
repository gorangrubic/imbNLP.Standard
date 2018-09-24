// --------------------------------------------------------------------------------------------------------------------
// <copyright file="termExplorer.cs" company="imbVeles" >
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
namespace imbNLP.Data.semanticLexicon.explore
{
    using imbNLP.Data.extended.apertium;
    using imbNLP.Data.extended.namedEntities;
    using imbNLP.Data.extended.wordnet;
    using imbNLP.Data.semanticLexicon.core;
    using imbNLP.Data.semanticLexicon.posCase;
    using imbNLP.Data.semanticLexicon.term;
    using imbNLP.PartOfSpeech.flags.basic;
    using imbNLP.PartOfSpeech.providers.dictionary.apertium;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.files.search;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.data;
    using imbSCI.DataComplex.special;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class termExplorer : imbBindable
    {
        //public static translationEnumTable posTypeVsRegex

        //private Dictionary<string, termExploreModel> _models = new Dictionary<string, termExploreModel>();
        ///// <summary>
        /////
        ///// </summary>
        //protected Dictionary<string, termExploreModel> models
        //{
        //    get
        //    {
        //        //if (_models == null)_models = new Dictionary<String, termExploreModel>();
        //        return _models;
        //    }
        //    set { _models = value; }
        //}

        public termExplorer(string projectFolderPath = "")
        {
            if (projectFolderPath.isNullOrEmpty())
            {
                failedList = new fileTextOperater(manager.constructor.projectFolderStructure.pathFor("word_check_fails.txt"), true);
            }
            else
            {
                failedList = new fileTextOperater(projectFolderPath.add("word_check_fails.txt", "\\"), true);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public aceDictionarySet<string, termExploreModel> modelRegistry { get; set; } = new aceDictionarySet<string, termExploreModel>();

        private aceDictionarySet<string, ILexiconItem> _lexicalItemRegistry = new aceDictionarySet<string, ILexiconItem>();

        /// <summary> </summary>
        public aceDictionarySet<string, ILexiconItem> lexicalItemRegistry
        {
            get
            {
                return _lexicalItemRegistry;
            }
            protected set
            {
                _lexicalItemRegistry = value;
                OnPropertyChanged("lexicalItemRegistry");
            }
        }

        private List<string> _missing = new List<string>();

        /// <summary> </summary>
        public List<string> missing
        {
            get
            {
                return _missing;
            }
            protected set
            {
                _missing = value;
                OnPropertyChanged("missing");
            }
        }

        /// <summary>
        ///
        /// </summary>
        public List<termExploreModel> found { get; protected set; } = new List<termExploreModel>();

        public void AddModels(IEnumerable<termExploreModel> __models)
        {
            foreach (var item in __models)
            {
                AddModel(item);
            }
            //__models.ForEach(x => AddModel(x));
        }

        public void AddModel(termExploreModel model)
        {
            modelRegistry.Add(model.inputForm.ToLower(), model);
            foreach (termExploreItem item in model.instances)
            {
                modelRegistry.Add(item.inputForm.ToLower(), model);
            }
        }

        public List<termExploreModel> GetModels(string term)
        {
            if (modelRegistry.ContainsKey(term.ToLower())) return modelRegistry[term];
            List<termExploreModel> output = new List<termExploreModel>();
            output.Add(makeTempModel(term, pos_type.TEMP));
            return output;
        }

        private static regexQuerySet<object> _termDiscoveryResolver;

        /// <summary>
        /// static and autoinitiated object
        /// </summary>
        public static regexQuerySet<object> termDiscoveryResolver
        {
            get
            {
                if (_termDiscoveryResolver == null)
                {
                    _termDiscoveryResolver = new regexQuerySet<object>();

                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})ima$", "A:aemp3g|A:aemp6g|A:aemp7g|A:aefp3g|A:aefp6g|A:aefp7g|A:aenp3g|A:aenp6g|A:aenp7g");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})im$", "A:aems6g|A:aemp3g|A:aemp6g|A:aemp7g|A:aefp3g|A:aefp6g|A:aefp7g|A:aens6g|A:aenp3g|A:aenp6g|A:aenp7g");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})ih$", "A:aemp2g|A:aefp2g|A:aenp2g");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})a$", "A:akms2g|A:akms4v|A:aemw2g|A:aemw4g|A:aefs1g|A:aefs5g|A:akns2g|A:aenw2g|A:aenw4g|A:aenp1g|A:aenp4g|A:aenp5g");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})e$", "A:aemp4g|A:aefs2g|A:aefw2g|A:aefw4g|A:aefp1g|A:aefp4g|A:aefp5g");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})i$", "A:aems5g|A:aemp1g|A:aemp5g");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})o$", "A:aens1g|A:aens4g|A:aens5g");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})og$", "A:adms2g|A:adms4v|A:adns2g");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})oga$", "A:adms2g|A:adms4v|A:adns2g");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})oj$", "A:aefs3g|A:aefs7g");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})om$", "A:adms3g|A:adms7g|A:aefs6g|A:adns3g|A:adns7g");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})ome$", "A:adms3g|A:adms7g|A:adns3g|A:adns7g");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})omu$", "A:adms3g");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})ama$", "N:fp3q|N:fp6q|N:fp7q");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})e$", "N:fs2q|N:fw2q|N:fw4q|N:fp1q|N:fp4q|N:fp5q");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})i$", "N:fs3q|N:fs7q");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})o$", "N:fs5q");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})om$", "N:fs6q");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})u$", "N:fs4q");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})(ti|ći)$", "V:W");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})a$", "V:Pzs|V:Ays|V:Azs");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})ah$", "V:Axs|V:Ixs");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})ahu$", "V:Izp");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})aj$", "V:Yys");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})ajmo$", "V:Yxp");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})ajte$", "V:Yyp");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})aju$", "V:Pzp");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})ajući$", "V:S");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})la$", "V:Gsf|V:Gpn|V:Gwm|V:Gwn");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})le$", "V:Gpf|V:Gwf");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})li$", "V:Gpm");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})lo$", "V:Gsn");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})m$", "V:Pxs");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})te$", "V:Pyp");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})vši$", "V:X");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})će$", "V:Fzs|V:Fzp");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})ćemo$", "V:Fxp");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})ćete$", "V:Fyp");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})ćeš$", "V:Fys");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})ću$", "V:Fxs");
                    _termDiscoveryResolver.AddGramSet("([\\w]{3,})jaše$", "V:Azp|V:Iys|V:Izs");
                    _termDiscoveryResolver.AddGramSet("naj([\\w]{3,})i$", "A:ckms1g");
                    _termDiscoveryResolver.AddGramSet("naj([\\w]{3,})a$", "A:ckfs1g");
                    _termDiscoveryResolver.AddGramSet("naj([\\w]{3,})e$", "A:ckns1g");
                }
                return _termDiscoveryResolver;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public fileTextOperater failedList { get; set; }

        public TermLemma modelToLemma(termExploreModel termModel)
        {
            TermLemma tl = new TermLemma();

            //tl = manager.lexiconContext.TermLemmas.Create();
            tl.name = termModel.inputForm;
            tl.type = termModel.gramSet.getPosType().toStringSafe("N");
            tl.gramSet = termModel.gramSet.GetAll();

            foreach (termExploreItem item in termModel.instances)
            {
                ITermInstance ti = new TermInstance(); //manager.lexiconContext.TermInstances.Create();
                ti.name = item.inputForm;
                ti.type = item.gramSet.getPosType().ToString();
                ti.gramSet = item.gramSet.GetAll();
                ti.lemma = tl;
                // manager.lexiconContext.TermInstances.Add(ti);
            }

            return tl;
        }

        private semanticLexiconManager _manager;

        /// <summary>
        ///
        /// </summary>
        public semanticLexiconManager manager
        {
            get
            {
                if (_manager == null) _manager = semanticLexiconManager.manager;
                return _manager;
            }
            set { _manager = value; }
        }

        public List<ILexiconItem> exploreAndSave(string term, ILogBuilder loger, bool shortExplore = true, bool debug = true, params termExploreItemEnumFlag[] flagsToSave)
        {
            List<ILexiconItem> output = new List<ILexiconItem>();
            if (lexicalItemRegistry.ContainsKey(term))
            {
                return lexicalItemRegistry[term];
            }

            if (!manager.settings.doAutoexplore)
            {
                output.Add(makeTempLemma(term));
                if (loger != null)
                {
                    loger.AppendLine("Autoexplore off [" + term + "] is temporarly created.");
                }
                return output;
            }

            var res = failedList.Search(term, false, 1);
            if (res.getLineContentList().Contains(term))
            {
                output.Add(makeTempLemma(term));
                if (loger != null)
                {
                    loger.AppendLine("Term [" + term + "] is on black list - making temp term.");
                }
                return output;
            }

            List<termExploreModel> models = explore(term, loger, shortExplore, debug);
            if (flagsToSave == null) flagsToSave = new termExploreItemEnumFlag[] { termExploreItemEnumFlag.aper, termExploreItemEnumFlag.namedEntity, termExploreItemEnumFlag.srWNet };

            foreach (termExploreModel model in models)
            {
                if (debug)
                {
                    manager.constructor.saveTermModel(model);
                }
                if (flagsToSave.Contains(model.flags))
                {
                    manager.constructor.output.AppendLine("New term for Lexicon: " + model.inputForm);
                    manager.constructor.addTermModelToLexicon(model);
                    if (manager.settings.doAutoMakeSynonymRelationship) manager.constructor.addSynonymsAndConceptLinks(model, true);
                    output.AddRange(manager.getLexiconItems(model.inputForm, loger, false));
                }
                else
                {
                    output.Add(modelToLemma(model));
                }
            }

            if (!output.Any())
            {
                var md = new TempLemma(term);
                output.Add(makeTempLemma(term));
                md.type = pos_type.TEMP.ToString();

                failedList.Append(new string[] { md.name });

                if (loger != null)
                {
                    loger.AppendLine("Term [" + term + "] not found. Using single-instance spark.");
                }
            }

            lexicalItemRegistry.Add(term, output);

            return output;
        }

        public ITermLemma makeTempLemma(string term)
        {
            var md = new TempLemma(term);
            md.type = pos_type.TEMP.ToString();
            // List<String> fails = new   //new String[] { md.name }

            return md;
        }

        public termExploreModel makeTempModel(string term, pos_type type)
        {
            termExploreModel output = new termExploreModel(term);
            output.lemma = new termExploreItem(term);
            output.gramSet.Add(new gramFlags(type.ToString()));
            output.flags = termExploreItemEnumFlag.temp;
            return output;
        }

        /// <summary>
        /// Explores definition on an unknown term
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="loger">The loger.</param>
        /// <param name="shortExplore">if set to <c>true</c> [short explore].</param>
        /// <param name="debug">if set to <c>true</c> [debug].</param>
        /// <returns></returns>
        public List<termExploreModel> explore(string term, ILogBuilder loger, bool shortExplore = true, bool debug = true, termExploreModel exploreModel = null)
        {
            term = term.Trim();
            List<termExploreModel> output = new List<termExploreModel>();

            if (modelRegistry.ContainsKey(term))
            {
                return modelRegistry[term];
            }
            if (missing.Contains(term))
            {
                return GetModels(term);
            }

            if (term.isNumber())
            {
                termExploreModel tmp = makeTempModel(term, pos_type.NUMnumerical);
                tmp.flags = termExploreItemEnumFlag.datapoint;
                if (loger != null) loger.AppendLine("Term [" + term + "] detected to be number.");
                AddModel(tmp);
                return GetModels(term);
            }

            // <----- drugi test
            exploreModel = termExploreProcedures.exploreWithHunspell(new termExploreItem(term), loger);
            List<string> suggests = new List<string>();

            foreach (var item in exploreModel.instances)
            {
                suggests.Add(item.inputForm);
            }

            //exploreModel.instances.ForEach(x => suggests.Add(x.inputForm));

            //languageManagerDBNamedEntities.manager.exploreEntities(exploreModel.rootWord, exploreModel);

            suggests.Add(exploreModel.rootWord);

            // s

            apertiumDictionaryResult result = languageManagerApertium.manager.query(suggests, apertiumDictQueryScope.exact, apertiumDictNeedleSide.native);
            if (result.Any())
            {
                List<termExploreItem> gramCheck = new List<termExploreItem>();

                gramFlags gr = null;

                if (result.termVsGramFlags.ContainsKey(exploreModel.inputForm))
                {
                    exploreModel.gramSet.Add(new gramFlags(result.termVsGramFlags[exploreModel.inputForm]));

                    if (exploreModel.lemma == null)
                    {
                        exploreModel.lemma = exploreModel.instances[exploreModel.inputForm];
                    }

                    gramCheck.Add(exploreModel);
                    if (debug) if (loger != null) loger.AppendLine("Apertium discovered model [" + exploreModel.inputForm + "]");
                }
                else
                {
                    //if (loger != null) loger.AppendLine("Apertium failed to discover [" + exploreModel.inputForm + "]");
                }

                foreach (termExploreItem item in exploreModel.instances)
                {
                    if (result.termVsGramFlags.ContainsKey(item.inputForm))
                    {
                        item.gramSet.Add(new gramFlags(result.termVsGramFlags[item.inputForm]));
                        gramCheck.Add(exploreModel);
                        exploreModel.lemmaForm = item.inputForm;
                        if (exploreModel.lemma == null) exploreModel.lemma = item;

                        if (debug) if (loger != null) loger.AppendLine("Apertium discovered model [" + item.inputForm + "]");
                    }
                    else
                    {
                        //if (loger != null) loger.AppendLine("Apertium failed to discover [" + item.inputForm + "]");
                    }
                }

                exploreModel.translations.AddRange(result.GetTranslatedWords());

                gramCheck.RemoveAll(x => posConverter.posTypeVsPattern[x.gramSet.getPosType()].Count() == 0);

                int disc = 0;
                foreach (var gram in gramCheck)
                {
                    if (discoverGram(gram, loger, debug)) disc++;
                }

                if (loger != null) loger.AppendLine("Gram [" + term + "] autodiscovered for [" + disc + "] / [" + gramCheck.Count() + "]");
                if (debug) if (loger != null)
                    {
                        exploreModel.ToString(loger, true, false);
                        manager.constructor.saveTermModel(exploreModel, "Apertium_");
                    }

                AddModel(exploreModel);
                exploreModel.flags = termExploreItemEnumFlag.aper;

                if (shortExplore) return GetModels(term);
            }
            else
            {
                if (loger != null) loger.AppendLine("Apertium failed to discover any information on [" + term + "]");
            }

            if (loger != null)
            {
                loger.consoleAltColorToggle();
            }

            // <------------------ APERTIUM ^^

            foreach (string s in suggests)
            {
                languageManagerDBNamedEntities.manager.exploreEntities(s, exploreModel);
            }

            if (exploreModel.flags == termExploreItemEnumFlag.namedEntity)
            {
                AddModel(exploreModel);

                if (debug)
                {
                    if (loger != null)
                    {
                        exploreModel.ToString(loger, true, false);
                        manager.constructor.saveTermModel(exploreModel, "NamedEntity_");
                        loger.AppendLine("Named entities discovered model [" + exploreModel.inputForm + "]:" + exploreModel.gramSet.ToString());
                    }
                }

                if (shortExplore) return GetModels(term);
            }
            else
            {
                if (loger != null)
                {
                    if (debug)
                    {
                        loger.AppendLine("Named entities found nothing for [" + exploreModel.inputForm + "]:" + exploreModel.gramSet.ToString());
                    }
                }
            }

            if (loger != null)
            {
                loger.consoleAltColorToggle();
            }

            // <------------------ NAMED ENTITY ^^

            // <----------------- Wordnet
            wordnetSymsetResults resSrWordnet = languageManagerWordnet.manager.query_srb(suggests, loger);
            bool found = false;
            if (resSrWordnet.Any())
            {
                foreach (termExploreItem item in exploreModel.instances)
                {
                    if (resSrWordnet.GetByKey(item.inputForm).Any())
                    {
                        exploreModel.lemma = item;
                        exploreModel.lemmaForm = item.inputForm;
                        exploreModel.translations.AddRange(resSrWordnet.GetValues());
                        exploreModel.synonyms.AddRange(resSrWordnet.GetKeys());
                        exploreModel.flags = termExploreItemEnumFlag.srWNet;
                        found = true;

                        item.gramSet.Add(new gramFlags(new Enum[] { resSrWordnet.models[item.inputForm].gramSet.getPosType() }));
                    }
                }

                foreach (termExploreItem item in exploreModel.instances)
                {
                    discoverGram(item, loger, debug);
                }
            }

            if (found)
            {
                if (loger != null) loger.AppendLine("SerbianWordNet discovered model [" + term + "]:" + exploreModel.gramSet.ToString());
                if (debug) if (loger != null)
                    {
                        exploreModel.ToString(loger, true, false);
                        manager.constructor.saveTermModel(exploreModel, "SrWordNet_"); ;
                    }

                AddModel(exploreModel);
                exploreModel.flags = termExploreItemEnumFlag.srWNet;

                if (shortExplore) return GetModels(term);
            }
            else
            {
                if (loger != null)
                {
                    if (debug)
                    {
                        loger.AppendLine("Serbian wordnet found nothing for [" + term + "]");
                    }
                }
            }

            // <------------------ SERBIAN WORD NET ^^

            bool failed = discoverGram(exploreModel, loger, debug);

            foreach (var item in exploreModel.instances)
            {
                discoverGram(item, loger, debug);
            }
            //exploreModel.instances.ForEach(x => discoverGram(x, loger, debug));

            int d = 0;

            List<termExploreItem> lastCheck = new List<termExploreItem>();

            foreach (var gram in lastCheck)
            {
                if (discoverGram(gram, loger, debug)) d++;
            }

            if (debug) if (loger != null) loger.AppendLine("The last check [" + term + "] autodiscovered for [" + d + "] / [" + lastCheck.Count() + "]");

            if (d == 0) failed = true;

            if (loger != null)
            {
                loger.consoleAltColorToggle();
            }

            // <------------------ LAST CHECK ^^

            if (!failed)
            {
                exploreModel.flags = termExploreItemEnumFlag.termExplorer;
                AddModel(exploreModel);
                return GetModels(term);
            }
            else
            {
                if (debug) if (loger != null) loger.AppendLine("Exploration failed for [" + term + "] -- creating temporary term model");
                output.Add(makeTempModel(term, pos_type.TEMP));
                missing.Add(term);
                return output;
            }
        }

        public bool discoverGram(termExploreItem item, ILogBuilder loger, bool debug = true)
        {
            //List<termExploreItem> inst = new List<termExploreItem>();
            //exploreModel.instances.ForEach(x => inst.Add(x));

            //inst.Add(exploreModel);

            // instanceCountCollection<pos_type> pct = new instanceCountCollection<pos_type>();
            bool failed = false;

            //// <--------------- Trying to resolve alone
            //foreach (termExploreItem item in inst)
            //{
            if (loger != null) loger.AppendLine("Item:" + item.inputForm);

            instanceCountCollection<object> res = termDiscoveryResolver.resolveQuery(item.inputForm);
            res.reCalculate();

            if (res.Count > 0)
            {
                List<object> sorted = res.getSorted();

                if (item.gramSet.getPosType() != pos_type.none)
                {
                    sorted.RemoveAll(x => x is pos_type);
                }

                gramFlags gf = new gramFlags();

                if (sorted.Any(x => x is pos_type)) gf.Set((pos_type)sorted.First(x => x is pos_type));
                //pct.AddInstance(gf.type, 1);

                var tl = posConverter.posTypeVsPattern[gf.type];
                sorted.RemoveAll(x => !tl.Contains(x.GetType()));

                if (loger != null)
                {
                    loger.AppendLine("Votes:");
                    for (int i = 0; i < Math.Max(sorted.Count(), 20); i++)
                    {
                        loger.Append(sorted[i].ToString() + "; ");
                    }
                }

                if (sorted.Any(x => x is pos_gender)) gf.Set((pos_gender)sorted.First(x => x is pos_gender));
                if (sorted.Any(x => x is pos_gramaticalCase)) gf.Set((pos_gramaticalCase)sorted.First(x => x is pos_gramaticalCase));
                if (sorted.Any(x => x is pos_verbform)) gf.Set((pos_verbform)sorted.First(x => x is pos_verbform));
                if (sorted.Any(x => x is pos_number)) gf.Set((pos_number)sorted.First(x => x is pos_number));
                if (sorted.Any(x => x is pos_degree)) gf.Set((pos_degree)sorted.First(x => x is pos_degree));
                if (sorted.Any(x => x is pos_person)) gf.Set((pos_person)sorted.First(x => x is pos_person));

                if (loger != null) loger.AppendLine("Final gram:" + gf.ToString());
                item.gramSet.Add(gf);
            }
            else
            {
                if (item.inputForm.Length < 4) return false;
                //item.flags = termExploreItemEnumFlag.none;
                failed = true;
            }

            return failed;
        }
    }
}