// --------------------------------------------------------------------------------------------------------------------
// <copyright file="lexiconConstructTaskOne.cs" company="imbVeles" >
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
    using imbNLP.Data.extended.unitex;
    using imbNLP.Data.semanticLexicon.core;
    using imbNLP.Data.semanticLexicon.explore;
    using imbNLP.Data.semanticLexicon.source;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.reporting;
    using imbSCI.Data.enums;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Task one is about lemmatization
    /// </summary>
    /// <seealso cref="imbNLP.Data.semanticLexicon.procedures.lexiconTaskBase" />
    public class lexiconConstructTaskOne : lexiconTaskBase
    {
        public lexiconConstructTaskOne() : base()
        {
        }

        public override string taskSourcePath
        {
            get
            {
                return semanticLexiconManager.manager.settings.sourceFiles.getFilePaths(lexiconSourceTypeEnum.corpus).First();
            }
        }

        public override string taskInputPath
        {
            get
            {
                return "corpus_input.csv";
            }
        }

        public override string taskOutputPath
        {
            get
            {
                return "lexicon_lemmas.txt";
            }
        }

        public override string taskTitle
        {
            get
            {
                return "stageOne";
            }
        }

        public override bool taskSourcePathIsAppRoot
        {
            get
            {
                return true;
            }
        }

        public override void stageComplete(ILogBuilder response)
        {
            List<string> lemmas = new List<string>();
            foreach (ITermLemma tl in semanticLexiconManager.manager.lexiconContext.TermLemmas)
            {
                lemmas.Add(tl.name);
            }

            lemmas.saveContentOnFilePath(state.folder.pathFor(taskOutputPath));

            state.failedTasks.file.CopyTo(semanticLexiconManager.manager.constructor.projectFolderStructure[lexiconConstructorProjectFolder.splits].pathFor("corpus_explore_fails.csv"), true);

            //state.failedTasks.sa

            state.stateSessionTick(this, true);
        }

        protected override void stageExecute(ILogBuilder response)
        {
            foreach (string word in state.entryList)
            {
                // <------------------------------------------------------------------------------------------------ Exploration

                termExploreModel output = null;
                if (state.verbose)
                {
                    output = termExploreProcedures.exploreWithUnitex(word, response);
                }
                else
                {
                    output = termExploreProcedures.exploreWithUnitex(word, null);
                }

                if (output.wasExploreFailed)
                {
                    if (state.debug)
                    {
                        response.consoleAltColorToggle();
                        response.AppendLine("--- running debug search for [" + word + "]");
                        var exp = languageManagerUnitex.manager.operatorDelaf.Search(word, false, 25);
                        exp.ToString(response, true);

                        string debugLines = exp.ToString();
                        string debugPath = semanticLexiconManager.manager.constructor.projectFolderStructure[lexiconConstructorProjectFolder.logs].pathFor(word + "_failDebug.txt");
                        debugLines.saveStringToFile(debugPath, getWritableFileMode.overwrite);

                        response.consoleAltColorToggle();
                    }
                    state.shadowBuffer.Add(word);
                    state.failedBuffer.Add(word);
                }
                else
                {
                    if (state.saveModel) semanticLexiconManager.manager.constructor.saveTermModel(output);
                    state.shadowBuffer.Add(word);
                    state.shadowBuffer.AddRange(output.GetShadow());
                    try
                    {
                        semanticLexiconManager.manager.constructor.addTermModelToLexicon(output);
                        response.AppendLine("Lexicon update: Lemma [" + output.lemma.inputForm + "][" + output.instances.Count() + "]");
                        state.processedBuffer.Add(output.lemma.inputForm);
                    }
                    catch (Exception ex)
                    {
                        state.failedBuffer.Add(word);
                        response.AppendLine("Lexicon term update failed for [" + word + "][" + output.lemmaForm + "]");
                        output.ToString(response, true);
                    }
                }
            }
        }

        /*
        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="response">The response.</param>
        public override void execute(ILogBuilder response)
        {
            Int32 takeIndex = 0;
            running = true;

            while (running)
            {
                var entry = semanticLexiconManager.manager.constructor.corpusOperater.Take(1, taskShadow);

                if (!entry.Any())
                {
                    running = false;
                    break;
                }

                String word = entry.getLineContentList().First();

                // <------------------------------------------------------------------------------------------------ Exploration

                termExploreModel output = null;
                if (verbose)
                {
                    output = termExploreProcedures.exploreWithUnitex(word, response);
                } else
                {
                    output = termExploreProcedures.exploreWithUnitex(word, null);
                }

                List<String> shadow = new List<string>();

                if (output.wasExploreFailed)
                {
                    if (debug)
                    {
                        response.consoleAltColorToggle();
                        response.AppendLine("--- running debug search for [" + word + "]");
                        var exp = languageManagerUnitex.manager.operatorDelaf.Search(word, false, 25);
                        exp.ToString(response, true);

                        String debugLines = exp.ToString();
                        String debugPath = semanticLexicon.semanticLexiconManager.manager.constructor.projectFolderStructure[lexiconConstructorProjectFolder.logs].pathFor(word + "_failDebug.txt");
                        debugLines.saveStringToFile(debugPath, aceCommonTypes.enums.getWritableFileMode.overwrite);

                        response.consoleAltColorToggle();
                    }
                    shadow.Add(word);
                    semanticLexiconManager.manager.constructor.exploreFails.Append(new String[] { word });
                } else
                {
                    if (saveModel) semanticLexicon.semanticLexiconManager.manager.constructor.saveTermModel(output);
                    shadow.Add(word);
                    shadow.AddRange(output.GetShadow());
                    try
                    {
                        semanticLexiconManager.manager.constructor.addTermModelToLexicon(output);
                        response.AppendLine("Lexicon update: Lemma [" + output.lemma.inputForm + "][" + output.instances.Count() + "]");
                    } catch (Exception ex)
                    {
                        semanticLexiconManager.manager.constructor.exploreFails.Append(new String[] { word });
                        response.AppendLine("Lexicon term update failed for [" + word + "]["+ output.lemmaForm + "]");
                        output.ToString(response, true);
                    }
                }

                // <------------------ removing shadow and preparing the next iteration

                taskShadow.AddRange(shadow);

                takeIndex++;
                checkIndex();
                if (takeIndex >= takeCount)
                {
                    running = false;
                    break;
                }
            }
        }
        */
    }
}