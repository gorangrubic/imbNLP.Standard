// --------------------------------------------------------------------------------------------------------------------
// <copyright file="lexiconConstructor.cs" company="imbVeles" >
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
namespace imbNLP.Data.semanticLexicon.procedures
{
    using imbACE.Core.core;
    using imbACE.Services.terminal;
    using imbNLP.Data.semanticLexicon.core;
    using imbNLP.Data.semanticLexicon.explore;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.files;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.enums;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Concept = imbNLP.Data.semanticLexicon.Concept;

    public class lexiconConstructor
    {
        /// <summary> </summary>
        public folderStructure projectFolderStructure { get; protected set; }

        /// <summary> </summary>
        public builderForLog output { get; protected set; } = new builderForLog();

        /// <summary> </summary>
        public lexiconConstructionSettings settings { get; set; }

        public lexiconConstructor(lexiconConstructionSettings __settings)
        {
            settings = __settings;
        }

        /// <summary>
        ///
        /// </summary>
        protected Dictionary<string, lexiconTaskBase> tasks { get; set; } = new Dictionary<string, lexiconTaskBase>();

        /// <summary>
        /// Runs the stage.
        /// </summary>
        /// <param name="stageName">Name of the stage.</param>
        /// <param name="response">The response.</param>
        /// <param name="isReset">if set to <c>true</c> [is reset].</param>
        /// <param name="take">The take.</param>
        /// <param name="__savemodels">if set to <c>true</c> [savemodels].</param>
        /// <param name="__debug">if set to <c>true</c> [debug].</param>
        /// <param name="__verbose">if set to <c>true</c> [verbose].</param>
        /// <param name="__response">The response.</param>
        public void runStage(string stageName, bool isReset, int take, bool __savemodels, bool __debug, bool __verbose, ILogBuilder __response)
        {
            var task = tasks[stageName];
            if (isReset) task.stageReset(__response);
            task.sessionStart(take, __savemodels, __debug, __verbose, __response);
        }

        /// <summary>
        /// Starts the construction --- prepares all files and structure
        /// </summary>
        public void startConstruction(string __path = "")
        {
            string projectDir = "projects\\" + settings.corpusProjectName;
            if (!__path.isNullOrEmpty()) projectDir = __path;

            //   projectDirectory = Directory.CreateDirectory(projectDir);

            aceLog.consoleControl.setAsOutput(output, "lexConst");

            //fileOpsBase. = settings.fileTextSearchBlockSize;
            if (projectFolderStructure == null)
            {
                projectFolderStructure = new folderStructure(projectDir, "Corpus project", "Corpus construction project root directory");
                projectFolderStructure.Add(lexiconConstructorProjectFolder.links, "Links", "Text file link lists for later semantic link constructions. Corpus project: " + settings.corpusProjectName);
                projectFolderStructure.Add(lexiconConstructorProjectFolder.metadata, "MetaData", "Serialized termExploreModel instances created by S1 and updated by other stages. Corpus project: " + settings.corpusProjectName);
                projectFolderStructure.Add(lexiconConstructorProjectFolder.splits, "Splits", "Splited parts from the input corpus corpus. Corpus project: " + settings.corpusProjectName);
                projectFolderStructure.Add(lexiconConstructorProjectFolder.scripts, "Scripts", "Repositorium of console and constructor execution scripts. Corpus project: " + settings.corpusProjectName);
                projectFolderStructure.Add(lexiconConstructorProjectFolder.logs, "Logs", "Execution and crash logs. Corpus project: " + settings.corpusProjectName);
                projectFolderStructure.Add(lexiconConstructorProjectFolder.stages, "Stages", "Stage task states and task list files. Corpus project: " + settings.corpusProjectName);
                projectFolderStructure.Add(lexiconConstructorProjectFolder.documents, "Input documents", "Documents (txt, html,csv) used as wordload sources during development and testing. Corpus project: " + settings.corpusProjectName);
                projectFolderStructure.generateReadmeFiles(null);
            }

            output.outputPath = projectFolderStructure[lexiconConstructorProjectFolder.logs].pathFor("lexConstructor.log");

            output.AppendLine("Lexicon construction project directory: " + projectFolderStructure.path);

            if (!tasks.Any())
            {
                tasks.Add("s0", new lexiconConstructTaskZero());
                tasks.Add("s1", new lexiconConstructTaskOne());
                tasks.Add("s2", new lexiconConstructTaskTwo());
                tasks.Add("s3", new lexiconConstructTaskThree());
                tasks.Add("s4", new lexiconConstructTaskFour());
                tasks.Add("s5", new lexiconConstructTaskFive());
            }
            output.AppendLine("--- Lexicon Construction ready to start");
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

        public void DeleteAll(IEnumerable<Object> toDelete)
        {
            foreach (var item in toDelete)
            {
                manager.lexiconContext.DeleteObject(item);
            }
        }

        /// <summary>
        /// Resets the lexicon and corpus.
        /// </summary>
        public void resetLexiconAndCorpus()
        {
            projectFolderStructure[lexiconConstructorProjectFolder.metadata].deleteFiles();
            projectFolderStructure[lexiconConstructorProjectFolder.splits].deleteFiles();
            projectFolderStructure[lexiconConstructorProjectFolder.links].deleteFiles();
            projectFolderStructure[lexiconConstructorProjectFolder.stages].deleteFiles();

            projectFolderStructure.deleteFiles("*.*", false);

            bool cleanLexicon = aceTerminalInput.askYesNo("Are you sure to clean the Semantic Lexicon?", false);
            if (cleanLexicon)
            {
                DeleteAll(manager.lexiconContext.TermInstances);
                DeleteAll(manager.lexiconContext.TermLemmas);
                DeleteAll(manager.lexiconContext.Concepts);

                //manager.lexiconContext.TermInstances.RemoveAll(x=>x!=null);
                //manager.lexiconContext.TermLemmas.RemoveAll(x => x != null);
                //manager.lexiconContext.Concepts.RemoveAll(x => x != null);
                manager.lexiconContext.SaveChanges();
            }

            startConstruction();
        }

        public void saveAll(ILogBuilder externalLog = null)
        {
            if (externalLog != null)
            {
                saveBase.saveToFile(projectFolderStructure[lexiconConstructorProjectFolder.logs].pathFor("console_response_log.txt"), externalLog.logContent);
            }

            output.logContent.saveStringToFile(projectFolderStructure[lexiconConstructorProjectFolder.logs].pathFor("constructor_log.txt"), getWritableFileMode.overwrite);

            string help = manager.console.helpContent;
            if (!help.isNullOrEmpty())
            {
                help.saveStringToFile(projectFolderStructure[lexiconConstructorProjectFolder.scripts].pathFor("help.md"));
            }
        }

        /// <summary>
        /// Adds the term model to the lexicon
        /// </summary>
        /// <param name="termModel">The term model.</param>
        public void addTermModelToLexicon(termExploreModel termModel)
        {
            //var tlt = manager.lexiconContext.TermLemmas.Where<ITermLemma>(x => x.name.Equals(termModel.lemma.inputForm)&& x.type.Equals(termModel.lemma.gramSet.getPosType().toString()));
            ITermLemma tl = manager.lexiconContext.TermLemmas.Where(x => x.name.Equals(termModel.lemmaForm)).FirstOrDefault();

            //IQueryable<ITermLemma> tlt = manager.lexiconContext.TermLemmas.Where(x => x.name.Equals(termModel.lemma.inputForm));
            //List<ITermLemma> tlt_l = tlt.ToList();

            //ITermLemma tl = null;

            if (tl != null)
            {
                // nista nije uradjeno
            }
            else
            {
                tl = manager.lexiconContext.TermLemmas.Create();
                tl.name = termModel.lemmaForm.or(termModel.inputForm);

                tl.gramSet = termModel.getGramSet();

                tl.type = termModel.getPosType().toStringSafe("TEMP");

                //if (termModel tl.type = termModel.gramSet.getPosType().toString();

                foreach (termExploreItem item in termModel.instances)
                {
                    ITermInstance ti = manager.lexiconContext.TermInstances.Where(x => x.name.Equals(item.inputForm)).FirstOrDefault();

                    if (ti == null)
                    {
                        ti = manager.lexiconContext.TermInstances.Create();

                        ti.name = item.inputForm;
                        //
                        ti.gramSet = item.getGramSet(); //.gramSet.GetAll();
                        ti.type = item.getPosType().toStringSafe(); //.// .gramSet.getPosType().toString();
                        ti.lemma = tl;
                    }
                    else
                    {
                        if (manager.settings.doResolveWordsInDebugMode)
                        {
                            output.AppendLine("Item [" + item.inputForm + "] of lemma [" + tl.name + "] was already defined.");
                            //foreach (var lm in ti.)
                            //    output.AppendLine("[" + item.inputForm + "]->lemma [" + tl.name + "] was already defined.");
                        }
                    }

                    manager.lexiconContext.TermInstances.Add(ti);
                }
            }

            manager.lexiconContext.SaveChanges();
        }

        public void addSynonymsAndConceptLinks(termExploreModel termModel, bool saveModel = false)
        {
            // <----------- ADDING SYNONYMS ----------- >

            var lemmas = manager.getLemma(termModel.lemmaForm);

            var lemmasyns = manager.getLemmas(termModel.synonyms);

            builderForLog linkLog = new builderForLog();

            linkLog.open("Creating synonym-2-lemma links");

            termModel.links_synonym = 0;
            foreach (ITermLemma lemma in lemmas)
            {
                foreach (ITermLemma lemsyn in lemmasyns)
                {
                    bool added = false;
                    if (!lemma.relatedTo.Contains(lemsyn))
                    {
                        if (!lemma.relatedFrom.Contains(lemsyn))
                        {
                            lemma.relatedTo.Add(lemsyn);

                            added = true;
                        }
                    }
                    if (added)
                    {
                        termModel.links_synonym++;
                        linkLog.AppendLine("[" + termModel.links_synonym.ToString("D5") + "] " + lemma.name + " -> " + lemsyn.name);
                    }
                    else
                    {
                        linkLog.AppendLine("[Link exists] " + lemma.name + " -> " + lemsyn.name);
                    }
                }
            }
            linkLog.close();

            linkLog.open("Creating concept 2 lemma links");
            // <----------- ADDING SYNSETS
            List<Concept> concepts = new List<Concept>();
            Concept conHead = null;
            termModel.links_lemmaConcept = 0;
            foreach (string code in termModel.wordnetPrimarySymsets)
            {
                if (code.isCleanWord())
                {
                    aceLog.log("wrong symset code -- [" + code + "]  -- ignored!");
                    continue;
                }
                Concept con = manager.getConcept(code, true, "WordNet Code");
                bool added = false;
                foreach (TermLemma lemma in lemmas)
                {
                    if (!con.lemmas.Contains(lemma))
                    {
                        con.lemmas.Add(lemma);
                        added = true;
                    }
                    if (added)
                    {
                        termModel.links_lemmaConcept++;
                        linkLog.AppendLine("[" + termModel.links_lemmaConcept.ToString("D5") + "] " + con.name + " -> " + lemma.name);
                    }
                    else
                    {
                        linkLog.AppendLine("[Link exists] " + con.name + " -> " + lemma.name);
                    }
                }

                concepts.Add(con);
                conHead = con;
            }
            linkLog.close();

            linkLog.open("Creating concept 2 concept links");
            // <--------------------------- linking SYNSET concepts
            termModel.links_conceptConcept = 0;
            foreach (Concept con in concepts)
            {
                foreach (Concept con2 in concepts)
                {
                    bool added = false;
                    if (!con2.relatedTo.Contains(con))
                    {
                        if (!con2.relatedFrom.Contains(con))
                        {
                            var sharedLemmas = con2.lemmas.Where(x => con.lemmas.Contains(x));
                            if (sharedLemmas.Count() > 0)
                            {
                                con2.relatedTo.Add(con);
                                added = true;
                            }
                        }
                    }
                    if (added)
                    {
                        termModel.links_conceptConcept++;
                        linkLog.AppendLine("[" + termModel.links_conceptConcept.ToString("D5") + "] " + con2.name + " -> " + con.name);
                    }
                    else
                    {
                        linkLog.AppendLine("[Link exists] " + con2.name + " -> " + con.name);
                    }
                }
            }
            linkLog.close();

            manager.lexiconContext.SaveChanges();
            string pth = projectFolderStructure[lexiconConstructorProjectFolder.links].pathFor(termModel.filename(".txt"));
            linkLog.ToString().saveStringToFile(pth, getWritableFileMode.overwrite);

            if (saveModel) saveTermModel(termModel);
        }

        /// <summary>
        /// Saves the term model. Prefix is clean filename prefix without spacing character
        /// </summary>
        /// <param name="termModel">The term model.</param>
        /// <param name="prefix">The prefix.</param>
        public void saveTermModel(termExploreModel termModel, string prefix = "")
        {
            string filepath = projectFolderStructure[lexiconConstructorProjectFolder.metadata].path + "\\" + prefix + termModel.filename(".xml");
            FileInfo fi = filepath.getWritableFile(getWritableFileMode.overwrite);
            objectSerialization.saveObjectToXML(termModel, fi.FullName);
        }

        /// <summary>
        /// Loads any term model for the lemma form
        /// </summary>
        /// <param name="lemma">The lemma.</param>
        /// <returns></returns>
        public termExploreModelSet loadTermModels(string lemma, bool dontLoadFromFile = false)
        {
            var lemmas = manager.resolve(lemma);
            termExploreModelSet output = new termExploreModelSet();

            foreach (var lm in lemmas)
            {
                output.Add(getTermModel(lm, dontLoadFromFile));
            }

            return output;
        }

        /// <summary>
        /// Gets model from the Lexicon lemma entry -- loads from file, or if file do not exist - reconstructs it from the TermLema
        /// </summary>
        /// <param name="lemma">The lemma.</param>
        /// <returns></returns>
        public termExploreModel getTermModel(ITermLemma lemma, bool dontLoadFromFile = false)
        {
            termExploreModel output = new termExploreModel();

            if (!dontLoadFromFile)
            {
                string filepath = projectFolderStructure[lexiconConstructorProjectFolder.metadata].pathFor(lemma.name + "_" + lemma.type.ToString() + ".xml");
                if (File.Exists(filepath))
                {
                    output = objectSerialization.loadObjectFromXML<termExploreModel>(filepath);
                    output.modelSource = termExploreModelSource.fromFile;
                    return output;
                }
            }

            output = new termExploreModel(lemma);
            return output;
        }

        /// <summary>
        /// Loads a script for autoexecution
        /// </summary>
        /// <param name="scriptFilename">The script filename.</param>
        /// <returns></returns>
        public List<string> loadScript(string scriptFilename)
        {
            string path = projectFolderStructure[lexiconConstructorProjectFolder.scripts].path.add(scriptFilename, "\\");
            List<string> output = new List<string>();
            if (!File.Exists(path))
            {
                File.WriteAllText(path, "help");
            }
            output = openBase.openFile(path);
            return output;
        }
    }
}