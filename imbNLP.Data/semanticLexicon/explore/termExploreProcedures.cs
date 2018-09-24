// --------------------------------------------------------------------------------------------------------------------
// <copyright file="termExploreProcedures.cs" company="imbVeles" >
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
    using imbACE.Core.core;
    using imbNLP.Data.extended.apertium;
    using imbNLP.Data.extended.unitex;
    using imbNLP.Data.extended.wordnet;
    using imbNLP.Data.semanticLexicon.posCase;
    using imbNLP.Data.semanticLexicon.procedures;
    using imbNLP.Data.semanticLexicon.source;
    using imbNLP.PartOfSpeech.lexicUnit.tokenGraphs;
    using imbNLP.PartOfSpeech.providers.dictionary.apertium;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.files.search;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.enums;
    using imbSCI.Data.interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class termExploreProcedures
    {
        /// <summary>
        /// The stage two exploration
        /// </summary>
        /// <param name="lemma">The lemma.</param>
        /// <param name="response">The response.</param>
        /// <param name="savemodel">if set to <c>true</c> [savemodel].</param>
        /// <param name="debug">if set to <c>true</c> [debug].</param>
        /// <param name="verbose">if set to <c>true</c> [verbose].</param>
        /// <returns></returns>
        public static termExploreModelSet exploreStageTwo(string lemma, ILogBuilder response, bool savemodel, bool debug, bool verbose, lexiconTaskBase task = null)
        {
            lexiconConstructor constructor = semanticLexiconManager.manager.constructor;
            termExploreModelSet outset = semanticLexiconManager.manager.constructor.loadTermModels(lemma, true);

            if (!Enumerable.Any(outset))
            {
                outset.missingLemmas.Add(lemma);
                return outset;
            }

            foreach (termExploreModel mod in outset)
            {
                builderForLog logout = new builderForLog();
                if (verbose)
                {
                    aceLog.consoleControl.setAsOutput(logout, "stage2");
                }
                termExploreModel model = getSynonymsWithApertium(mod, logout);

                string pt = model.lemma.gramSet.getPosType().ToString();
                if (savemodel)
                {
                    //  model.graph.saveDescription(constructor.projectFolderStructure[lexiconConstructorProjectFolder.logs].path, pt + "_related");
                }

                model = getSynonymsWithWordnetViaApertium(model, logout, true, false);

                if (savemodel)
                {
                    model.graph.saveDescription(constructor.projectFolderStructure[lexiconConstructorProjectFolder.logs].path, pt + "_concepts");
                    // model.graph.savePaths(constructor.projectFolderStructure[lexiconConstructorProjectFolder.logs].path, pt + "_concepts");
                }

                model.PostProcess();

                if (debug)
                {
                    model.ToString(logout, true, true);
                    string fn = model.lemma.inputForm + "_" + pt + "_log.md";
                    logout.ToString(false).saveStringToFile(constructor.projectFolderStructure[lexiconConstructorProjectFolder.logs].pathFor(fn), getWritableFileMode.overwrite);
                }

                if (verbose)
                {
                    aceLog.consoleControl.removeFromOutput(logout);
                }

                if (savemodel)
                {
                    if (task != null)
                    {
                        model.lastModifiedByStage = task.taskTitle;
                    }
                    else
                    {
                        model.lastModifiedByStage = "stageTwo-exploreProcedure";
                    }
                    if (!model.wasExploreFailed)
                    {
                        constructor.saveTermModel(model);
                    }
                    else
                    {
                        outset.failedModels.Add(model);
                    }
                }
            }
            return outset;
        }

        public static termExploreModel explore(string word, ILogBuilder response, termExploreMode mode, bool verbose = false)
        {
            termExploreModel model = new termExploreModel(word);
            termExploreModelSet outset = semanticLexiconManager.manager.constructor.loadTermModels(word, true);

            if (response != null)
            {
                response.consoleAltColorToggle();
                response.AppendHorizontalLine();
                response.AppendLine("Exploring term[" + model.inputForm + "] with [" + mode.ToString() + "]");
                response.consoleAltColorToggle();
            }

            if (Enumerable.Any(outset))
            {
                model = Enumerable.First(outset);

                if (response != null) response.AppendLine("term[" + model.inputForm + "]->lemma[" + model.lemma.inputForm + "]");
            }
            else
            {
                model.lemmaForm = "";
                if (response != null) response.AppendLine("term[" + word + "]->missingLemma[]");
            }

            var output = response;

            if (!verbose) response = null;

            switch (mode)
            {
                case termExploreMode.apertium_direct:
                    model = getSynonymsWithApertium(model, response);
                    break;

                case termExploreMode.apertium_wordnet_eng:
                    model = getSynonymsWithWordnetViaApertium(model, response);
                    break;

                case termExploreMode.apertium_wordnet_srb:
                    model = getSynonymsWithSerbianWordNetAndApertium(model, response);
                    break;

                case termExploreMode.corpus:
                    model = getSynonymsByCorpus(model, response);
                    break;

                case termExploreMode.hunspell_srb:
                    model = getSynonymsWithHunspell(model, response);
                    break;

                case termExploreMode.none:
                    break;

                case termExploreMode.wordnet_srb:
                    model = getSynonymsWithSerbianWordNet(model, response);
                    break;

                case termExploreMode.unitex:
                    model = exploreWithUnitex(word, response);
                    break;
            }

            model.PostProcess();
            if (output != null) model.ToString(output, verbose, false);

            return model;
        }

        public static termExploreModel getSynonymsByCorpus(termExploreModel model, ILogBuilder response)
        {
            tokenGraph result = new tokenGraph(model.lemma.inputForm);

            var lines = semanticLexiconManager.manager.settings.sourceFiles.getOperater(lexiconSourceTypeEnum.corpus).Search(model.lemma.inputForm);
            result.Add(lines.getLineContentList(), tokenGraphNodeType.word_srb);

            model.synonyms.AddRange(result.getAllLeafs().getNames());

            if (response != null)
            {
                response.consoleAltColorToggle();
                string rst = result.ToStringTreeview();

                response.Append(rst);
                response.consoleAltColorToggle();
            }
            model.graph = result;
            return model;
        }

        public static termExploreModel getSynonymsWithApertium(termExploreModel model, ILogBuilder response)
        {
            tokenGraph result = languageManagerApertium.manager.queryForGraph(model.lemma.inputForm, apertiumDictQueryScope.exact);
            if (result.Count() == 0)
            {
                model.wasExploreFailed = true;
            }
            else
            {
                model.translations.AddRange(result.getAllLeafs().getNames());

                languageManagerApertium.manager.queryByGraphNode(result, apertiumDictQueryScope.exact, apertiumDictNeedleSide.translated);
                string st = result.ToStringTreeview();
                if (response != null) response.Append(st);

                model.synonyms.AddRange(result.getAllLeafs().getNames());
            }
            model.graph = result;
            return model;
        }

        public static termExploreModel getSynonymsWithSerbianWordNet(termExploreModel model, ILogBuilder response)
        {
            tokenGraph result = new tokenGraph(model.lemma.inputForm);

            languageManagerWordnet.manager.queryWithGraph(result, response, WordnetSource.serbian, WordnetQueryType.getSymsetCodesByWord);
            model.wordnetSecondarySymsets.AddRange(result.getAllLeafs().getNames());

            if (response != null)
            {
                response.consoleAltColorToggle();
                string rst = result.ToStringTreeview();

                response.Append(rst);
                response.consoleAltColorToggle();
            }

            languageManagerWordnet.manager.queryWithGraph(result, response, WordnetSource.serbian, WordnetQueryType.getWordsBySymsetCode);

            model.synonyms.AddRange(result.getAllLeafs().getNames());

            if (response != null)
            {
                response.consoleAltColorToggle();
                string rst = result.ToStringTreeview();

                response.Append(rst);
                response.consoleAltColorToggle();
            }
            model.graph = result;
            return model;
        }

        public static termExploreModel getSynonymsWithSerbianWordNetAndApertium(termExploreModel model, ILogBuilder response)
        {
            model = getSynonymsWithSerbianWordNet(model, response);
            tokenGraph result = model.graph;

            languageManagerApertium.manager.queryByGraphNode(model.graph, apertiumDictQueryScope.exact, apertiumDictNeedleSide.native);

            model.translations.AddRange(result.getAllLeafs().getNames());

            languageManagerApertium.manager.queryByGraphNode(result, apertiumDictQueryScope.exact, apertiumDictNeedleSide.translated);
            string st = result.ToStringTreeview();
            if (response != null) response.Append(st);

            model.synonyms.AddRange(result.getAllLeafs().getNames());

            model.graph = result;
            return model;
        }

        /// <summary>
        /// Method: word -- translation --- synset ---- other synsets --- collecting all words --- translation --- word
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="response">The response.</param>
        /// <param name="disableCodePrefixFilter">if set to <c>true</c> [disable code prefix filter].</param>
        /// <param name="disableCodeBranchFilter">if set to <c>true</c> [disable code branch filter].</param>
        /// <returns></returns>
        public static termExploreModel getSynonymsWithWordnetViaApertium(termExploreModel model, ILogBuilder response, bool disableCodePrefixFilter = false, bool disableCodeBranchFilter = false)
        {
            tokenGraph result = model.graph;

            result = languageManagerApertium.manager.queryForGraph(model.lemma.inputForm, apertiumDictQueryScope.exact);
            model.translations.AddRange(result.getAllLeafs().getNames());

            if (response != null)
            {
                response.consoleAltColorToggle();
                string rst = result.ToStringTreeview();

                response.Append(rst);
                response.consoleAltColorToggle();
            }

            languageManagerWordnet.manager.queryWithGraph(result, response, WordnetSource.english, WordnetQueryType.getSymsetCodesByWord);

            if (response != null)
            {
                response.consoleAltColorToggle();
                string st = result.ToStringTreeview();

                response.Append(st);
                response.consoleAltColorToggle();
            }

            model.wordnetSecondarySymsets.AddRange(result.getAllLeafs().getDeepest().getNames());

            if (!disableCodePrefixFilter)
            {
                string codeStart = model.lemma.gramSet.getPosType().GetWordNetCodeStart().ToString();
                Regex codeCriteria = new Regex("^" + codeStart + "");

                var badCodes = result.getAllLeafs(codeCriteria, true);

                if (response != null)
                {
                    response.AppendHorizontalLine();
                    response.AppendLine("Reducing to proper codes [" + codeStart + "]->filtered-out[" + badCodes.Count() + "]");
                }

                badCodes.removeFromParent();

                if (response != null)
                {
                    response.consoleAltColorToggle();
                    string rst = result.ToStringTreeview();

                    response.Append(rst);
                    response.consoleAltColorToggle();
                }
            }

            model.wordnetPrimarySymsets.AddRange(result.getAllLeafs().getDeepest().getNames());

            languageManagerWordnet.manager.queryWithGraph(result, response, WordnetSource.english, WordnetQueryType.getWordsBySymsetCode);

            model.translationRelated.AddRange(result.getAllLeafs().getDeepest().getNames());

            if (response != null)
            {
                response.AppendHorizontalLine();
                response.AppendLine("Getting English words by symsetcodes via WordNet");

                response.consoleAltColorToggle();
                string rst = result.ToStringTreeview();

                response.Append(rst);
                response.consoleAltColorToggle();
            }

            languageManagerApertium.manager.queryByGraphNode(result, apertiumDictQueryScope.exact, apertiumDictNeedleSide.translated);
            model.wordnetSynonyms.AddRange(result.getAllLeafs().getDeepest().getNames());

            if (response != null)
            {
                response.AppendHorizontalLine();
                response.AppendLine("Translating back to Serbian via Apertium");

                response.consoleAltColorToggle();
                string rst = result.ToStringTreeview();

                response.Append(rst);
                response.consoleAltColorToggle();
            }

            if (!disableCodeBranchFilter) // <------ removes the symset nodes that contain none of first-level translation words
            {
                var codeLevel = result.getAllChildren().getOnLevel(3);
                List<IObjectWithPathAndChildren> toTakeOut = new List<IObjectWithPathAndChildren>();

                foreach (var clb in codeLevel)
                {
                    foreach (var clb_c in clb)
                    {
                        bool takeOut = true;
                        foreach (var clb_cc in clb_c)
                        {
                            if (clb_cc.name == model.lemma.inputForm)
                            {
                                takeOut = false;
                                break;
                            }
                        }
                        if (takeOut)
                        {
                            if (response != null) response.AppendLine("-- take out: " + clb.path);
                            toTakeOut.Add(clb);
                            break;
                        }
                    }
                }

                toTakeOut.removeFromParent();

                int wps = Enumerable.Count(model.wordnetSecondarySymsets);
                int tr = Enumerable.Count(model.translationRelated);
                int ws = Enumerable.Count(model.wordnetSynonyms);

                if (response != null)
                {
                    response.AppendLine("----- Branch-node filter ----");

                    response.AppendLine("Symsets: " + wps);
                    response.AppendLine("Translations: " + tr);
                    response.AppendLine("Terms: " + ws);

                    response.consoleAltColorToggle();
                    string rst = result.ToStringTreeview();

                    response.Append(rst);
                    response.consoleAltColorToggle();
                }

                model.wordnetPrimarySymsets = result.getAllChildren().getOnLevel(3).getNames(true);
                model.translations = result.getAllChildren().getOnLevel(4).getNames(true);
                model.synonyms = result.getAllChildren().getOnLevel(5).getNames(true);

                wps = wps - Enumerable.Count(model.wordnetPrimarySymsets);
                tr = tr - Enumerable.Count(model.translations);
                ws = ws - Enumerable.Count(model.synonyms);

                if (response != null)
                {
                    //response.AppendLine("----- Branch-node filter ----");

                    response.AppendLine("Reduction of Symsets: " + wps);
                    response.AppendLine("Reduction of Translations: " + tr);
                    response.AppendLine("Reduction of Terms: " + ws);

                    response.consoleAltColorToggle();
                    string rst = result.ToStringTreeview();

                    response.Append(rst);
                    response.consoleAltColorToggle();
                }
            }

            /*
            String rgex_pat = "^([\\w]*\\\\[\\w]*\\\\[\\w]*\\\\[\\w]*\\\\{0}$)";

            Regex rgex = new Regex(String.Format(rgex_pat, model.lemma.inputForm));

            var onlyWithLemma = result.getAllLeafs().getFilterOut(rgex);
            */
            //languageManagerApertium.manager.queryByGraphNode(result, apertiumDictQueryScope.exact, apertiumDictNeedleSide.english);

            model.graph = result;
            return model;
        }

        public static termExploreModelSet exploreWithApertiumAndWordnet(string word, ILogBuilder response)
        {
            //List<TermLemma> lemmas = semanticLexiconManager.manager.resolve(word);

            termExploreModelSet outset = semanticLexiconManager.manager.constructor.loadTermModels(word);
            response.AppendLine("term[" + word + "]->models[" + Enumerable.Count(outset) + "]");

            if (Enumerable.Count(outset) == 0)
            {
                outset.missingLemmas.Add(word);
                response.AppendLine("term[" + word + "]->missingLemma[]");
                return outset;
            }

            int c = 0;
            foreach (termExploreModel model in outset)
            {
                var result = languageManagerApertium.manager.queryForSynonyms(model.lemma.inputForm, apertiumDictQueryScope.exact);
                var srb = result.GetNativeWords();
                var eng = result.GetTranslatedWords();
                model.translations.AddRange(eng);
                model.synonyms.AddRange(srb);
                response.AppendLine("term[" + word + "]->model[" + c.ToString() + "]->lemma[" + model.lemma.inputForm + "] --> Apertium.dic ==> srb[" + srb.Count() + "] eng[" + eng.Count() + "]");
                // <-- wordnet

                wordnetSymsetResults wordnet_res = languageManagerWordnet.manager.query_eng(model.translations, response);

                model.wordnetSecondarySymsets.AddRange(wordnet_res.GetKeys());

                wordnetSymsetResults wordnet_2nd = languageManagerWordnet.manager.query_eng_symset(model.wordnetSecondarySymsets, response);

                model.wordnetSynonyms.AddRange(wordnet_2nd.GetEnglish());

                var synTrans = languageManagerApertium.manager.query(model.wordnetSynonyms, apertiumDictQueryScope.exact, apertiumDictNeedleSide.translated);

                model.wordnetSynonymSerbian.AddRange(synTrans.GetNativeWords());

                response.AppendLine("WordNet(" + eng.Count() + ") ==> synsets[" + Enumerable.Count(model.wordnetSecondarySymsets) + "]  synEng[" + Enumerable.Count(model.wordnetSynonyms) + "] ==> synSrb[" + Enumerable.Count(model.wordnetSynonymSerbian) + "]");

                semanticLexiconManager.manager.constructor.saveTermModel(model);

                c++;
            }

            //termExploreModel output = semanticLexiconManager.manager.constructor.mode
            return outset;
        }

        /// <summary>
        /// Builds a term model out from Word input
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        public static termExploreModel exploreWithUnitex(string word, ILogBuilder response, bool wordIsLemma = false)
        {
            termExploreModel output = new termExploreModel();
            output.modelSource = termExploreModelSource.fromToken;
            output.inputForm = word;

            string lemma = word;

            var tls = semanticLexiconManager.manager.resolve(word);
            if (tls != null)
            {
                if (Enumerable.Count(tls) > 0)
                {
                    if (response != null) response.AppendLine("#1 Lemma already defined in the triplestore [" + word + "] ");
                    output = semanticLexiconManager.manager.constructor.getTermModel(Enumerable.First(tls));
                    return output;
                }
            }

            if (!wordIsLemma)
            {
                if (response != null) response.AppendLine("#1 Finding Lemma for [" + word + "] ");
                string query = string.Format(posConverter.REGEX_UNITEX_InstanceToLemmaFormat, word);
                fileTextSearchResult reslt = languageManagerUnitex.manager.operatorDelaf.Search(query, true, 1, RegexOptions.IgnoreCase);

                if (response != null)
                {
                    reslt.ToString(response, true);
                }

                Regex instanceToLemmaReg = new Regex(query);

                if (reslt.Count() > 0)
                {
                    var lnp = reslt.First();

                    Match mch = instanceToLemmaReg.Match(lnp.Value);
                    lemma = mch.Groups[1].Value;
                }
            }
            else
            {
                if (response != null) response.AppendLine("#1 The word is trusted to be a lemma [" + word + "] - skipping search");
            }
            // <------------------------------------------------------------------- preparing chache ---------------

            var cache = languageManagerUnitex.manager.operatorDelaf.Search(lemma, false, 300);
            if (response != null) response.AppendLine("Cached definitions [" + cache.Count() + "] ");

            // <------------------------------------------------------------  2. finding lemma definition

            output.lemmaForm = lemma;
            output.lemma = new termExploreItem(lemma);

            if (response != null) response.AppendLine("#2 Finding Lemma definition [" + lemma + "] ");

            string lemmaQuery = string.Format(posConverter.REGEX_UNITEX_DeclarationForLemma, lemma);
            Regex lemmaQueryRegex = new Regex(lemmaQuery);
            fileTextSearchResult lemmaResult = languageManagerUnitex.manager.operatorDelaf.Search(cache, lemmaQuery, true, 5, RegexOptions.IgnoreCase);
            if (response != null)
            {
                lemmaResult.ToString(response, true);
            }

            if (lemmaResult.Count() == 0)
            {
                if (response != null)
                {
                    response.consoleAltColorToggle();
                    response.AppendLine("Failed to find lemma definition for [" + word + "]. Aborting exploration.");
                    response.consoleAltColorToggle();
                }
                output.wasExploreFailed = true;
                return output;
            }

            foreach (var lr_lnp in lemmaResult)
            {
                Match lmch = lemmaQueryRegex.Match(lr_lnp.Value);
                if (lmch.Success)
                {
                    output.lemma.gramSet.Add(lmch.Groups[1].Value);
                }
            }
            if (response != null) output.lemma.ToString(response);

            // <------------------------------------------------------------  3. getting all instances for the lemma
            if (response != null) response.AppendLine("#3 Extracting all instances for the Lemma [" + lemma + "] ");

            string instanceQuery = string.Format(posConverter.REGEX_UNITEX_LemmaToInstanceFormat, lemma);
            string instanceUnitexQuery = "," + lemma + ".";
            Regex instanceQueryRegex = new Regex(instanceQuery);
            fileTextSearchResult instanceResult = languageManagerUnitex.manager.operatorDelaf.Search(cache, instanceUnitexQuery, false, 100, RegexOptions.IgnoreCase);

            if (response != null)
            {
                instanceResult.ToString(response, true);
            }

            foreach (var lr_lnp in instanceResult)
            {
                Match lmch = instanceQueryRegex.Match(lr_lnp.Value);
                output.instances.Add(lmch.Groups[1].Value, lmch.Groups[2].Value);
            }

            // <------------------------------------------------------------  4. Resulting term model
            if (response != null)
            {
                response.AppendLine("#4 Resulting term model [" + lemma + "] ");
                output.ToString(response);
            }

            return output;
        }

        //public static termExploreModelSet exploreAndExpandWithUnitex(String word, ILogBuilder response)
        //{
        //    termExploreModelSet output = new termExploreModelSet();
        //    return output;
        //}

        //public static termExploreItem constructTermItemFromUnitex(String unitexDelaf)
        //{
        //}

        public static termExploreModel getSynonymsWithHunspell(this termExploreModel model, ILogBuilder log)
        {
            throw new NotImplementedException();
            // List<string> suggest = imbLanguageFrameworkManager.serbian.basic.hunspellEngine.Suggest(model.lemma.inputForm);
            // model.synonyms.AddRange(suggest);
            return model;
        }

        /// <summary>
        /// Explores the with hunspell.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public static termExploreModel exploreWithHunspell(this termExploreItem item, ILogBuilder log)
        {
            termExploreModel output = new termExploreModel();
            List<string> terms = new List<string>();
            terms.Add(item.inputForm);
            throw new NotImplementedException();

            //List<string> suggest = imbLanguageFrameworkManager.serbian.basic.hunspellEngine.Suggest(item.inputForm);

            //List<string> sug2 = new List<string>();
            //suggest.ForEach(x=>sug2.Add(x.Replace("\\-", "-")));
            //suggest = sug2;
            //if (Enumerable.Any(suggest))
            //{
            //    int min_l = Enumerable.Min(suggest, x => x.Length);

            //    List<string> possibleTerm = new List<string>();

            //    int tocut = min_l - item.inputForm.Length;

            //    string start = item.inputForm;

            //    if (tocut != 0) start = start.substring(tocut);

            //    string rootComposite = "";
            //    int rootCompositeSplit = 0;
            //    foreach (string sug in suggest)
            //    {
            //        if (!sug.Contains(" "))
            //        {
            //            if (sug.Contains("-"))
            //            {
            //                int rcSplit = sug.IndexOf("-");
            //                if (rcSplit > rootCompositeSplit)
            //                {
            //                    rootCompositeSplit = rcSplit;
            //                    rootComposite = sug.Substring(0, rootCompositeSplit).Trim(Enumerable.ToArray("-"));
            //                }
            //            }
            //            else
            //            {
            //                if (sug.StartsWith(start))
            //                {
            //                    possibleTerm.Add(sug);
            //                }
            //            }
            //        }
            //    }

            //    if (tocut == 0)
            //    {
            //        if (possibleTerm.Count == 0)
            //        {
            //            possibleTerm.AddRange(suggest);
            //        }
            //    }
            //    possibleTerm.Add(item.inputForm);

            //    if (rootCompositeSplit == 0)
            //    {
            //        rootComposite = possibleTerm.MinItem(x => x.Length);
            //    }

            //    suggest = possibleTerm.Clone();
            //    possibleTerm.Clear();

            //    string lemmaForm = "";

            //    foreach (string sug in suggest)
            //    {
            //        if (sug.Contains(rootComposite, StringComparison.CurrentCultureIgnoreCase))
            //        {
            //            possibleTerm.Add(sug);
            //            if (lemmaForm.isNullOrEmpty())
            //            {
            //                lemmaForm = sug;
            //            }
            //            if (sug.Length < lemmaForm.Length)
            //            {
            //                lemmaForm = sug;
            //            }
            //        }

            //    }

            //    output.lemmaForm = lemmaForm;
            //    output.rootWord = rootComposite;
            //    output.inputForm = item.inputForm;

            //    foreach (string sug in possibleTerm)
            //    {
            //        output.instances.Add(sug);
            //        //log.log(sug);
            //    }
            //} else
            //{
            //    output.lemmaForm = item.inputForm;
            //    output.rootWord = item.inputForm;
            //    output.inputForm = item.inputForm;
            //}

            ////log.log("Input term: " + item.inputForm);
            ////log.log("Root: " + output.rootWord);
            ////log.log("Lemma: " + output.lemmaForm);
            ////log.log("Instances: ");

            //output.ToString(log);

            return output;
        }
    }
}