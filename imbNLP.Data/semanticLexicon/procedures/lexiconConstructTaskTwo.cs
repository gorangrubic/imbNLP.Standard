// --------------------------------------------------------------------------------------------------------------------
// <copyright file="lexiconConstructTaskTwo.cs" company="imbVeles" >
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
    using imbNLP.Data.semanticLexicon.core;
    using imbNLP.Data.semanticLexicon.explore;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.reporting;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class lexiconConstructTaskTwo : lexiconTaskBase
    {
        public lexiconConstructTaskTwo() : base()
        {
        }

        public override string taskInputPath
        {
            get
            {
                return "lexicon_lemmas_input.txt";
            }
        }

        public override string taskOutputPath
        {
            get
            {
                return "lexicon_synsets.txt";
            }
        }

        public override string taskSourcePath
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
                return "stageTwo";
            }
        }

        public override bool iterationAllowParallel
        {
            get
            {
                return true;
            }
        }

        public override void stageComplete(ILogBuilder response)
        {
            File.Copy(state.failedTasks.file.FullName, semanticLexiconManager.manager.constructor.projectFolderStructure.pathFor("lexicon_synset_failed.txt"));

            List<string> synsets = new List<string>();
            foreach (IConcept tl in semanticLexiconManager.manager.lexiconContext.Concepts)
            {
                synsets.Add(tl.name);
            }

            synsets.saveContentOnFilePath(state.folder.pathFor(taskOutputPath));
        }

        protected override void stageExecute(ILogBuilder response)
        {
            foreach (string word in state.entryList)
            {
                // <------------------------------------------ DISCOVERING SYNONYMS
                termExploreModelSet outset = null;

                outset = termExploreProcedures.exploreStageTwo(word, response, state.saveModel, state.debug, state.verbose, this);

                foreach (termExploreModel output in outset)
                {
                    if (output.wasExploreFailed)
                    {
                        if (state.debug)
                        {
                        }
                        state.shadowBuffer.AddUnique(word);
                        state.failedBuffer.AddUnique(word);
                    }
                    else
                    {
                        output.lastModifiedByStage = taskTitle;

                        state.shadowBuffer.AddUnique(output.lemmaForm);
                        state.processedBuffer.AddUnique(output.lemmaForm);

                        outset.lemmasToCheck.AddRange(output.synonyms);
                    }
                }

                // <-------------------------- CHECKING FOR NEWLY ADDED LEMMATA

                var alreadyHave = new List<ITermLemma>();
                List<string> toSearchFor = new List<string>();

                List<string> found = new List<string>();
                List<string> notFound = new List<string>();

                foreach (string lemma in outset.lemmasToCheck)
                {
                    if (state.taskShadow.Contains(lemma))
                    {
                        if (response != null) response.log("lemma[" + lemma + "] is known");
                    }
                    else
                    {
                        var res = semanticLexiconManager.manager.getLemma(lemma);
                        if (res.Any())
                        {
                            alreadyHave.AddRange(res);
                        }
                        else
                        {
                            toSearchFor.AddUnique(lemma);
                        }
                    }
                }

                foreach (string lemma in toSearchFor)
                {
                    var lg = response;
                    if (!state.verbose)
                    {
                        lg = null;
                    }
                    var output = termExploreProcedures.exploreWithUnitex(lemma, lg, true);
                    if (output.wasExploreFailed)
                    {
                        if (response != null) response.log("lemma[" + lemma + "] failed to be discovered");

                        state.shadowBuffer.AddUnique(word);
                        state.failedBuffer.AddUnique(word);
                        notFound.Add(lemma);
                    }
                    else
                    {
                        found.Add(lemma);

                        if (response != null) response.log("lemma[" + lemma + "] discovered!");
                        output.lastModifiedByStage = taskTitle;
                        semanticLexiconManager.manager.constructor.addTermModelToLexicon(output);
                        if (state.saveModel) semanticLexiconManager.manager.constructor.saveTermModel(output);

                        state.shadowBuffer.AddUnique(output.lemmaForm);
                        state.processedBuffer.AddUnique(output.lemmaForm);
                    }
                }

                // <----------------------- connecting
                foreach (termExploreModel output in outset)
                {
                    semanticLexiconManager.manager.constructor.addSynonymsAndConceptLinks(output, state.saveModel);

                    notFound.saveContentOnFilePath(semanticLexiconManager.manager.constructor.projectFolderStructure[lexiconConstructorProjectFolder.links].pathFor(output.filename() + "_notFound.txt"));
                    found.saveContentOnFilePath(semanticLexiconManager.manager.constructor.projectFolderStructure[lexiconConstructorProjectFolder.links].pathFor(output.filename() + "_found.txt"));
                }

                response.log("Lemma [" + word + "] synonyms and concepts construction finished.");
            }
        }
    }
}