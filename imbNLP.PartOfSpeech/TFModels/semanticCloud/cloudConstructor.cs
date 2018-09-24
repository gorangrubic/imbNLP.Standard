// --------------------------------------------------------------------------------------------------------------------
// <copyright file="cloudConstructor.cs" company="imbVeles" >
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
using imbACE.Core.core.exceptions;

// // using imbMiningContext.TFModels.WLF_ISF;
using imbNLP.PartOfSpeech.flags.basic;
using imbNLP.PartOfSpeech.pipelineForPos.subject;
using imbNLP.PartOfSpeech.resourceProviders.core;

// using imbNLP.PartOfSpeech.TFModels.semanticCloud.core;
using imbNLP.PartOfSpeech.TFModels.webLemma.table;
using imbSCI.Core.extensions.data;
using imbSCI.Core.math;
using imbSCI.Core.reporting;
using imbSCI.Data;
using imbSCI.DataComplex.special;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.PartOfSpeech.TFModels.semanticCloud
{
    /// <summary>
    /// Constructs <see cref="lemmaSemanticCloud"/>
    /// </summary>
    public class cloudConstructor
    {
        /// <summary>
        /// Creates multi-line description of current configuration
        /// </summary>
        /// <returns>List of description lines</returns>
        public List<string> DescribeSelf()
        {
            List<String> output = new List<string>();

            output.Add("## Semantic Cloud constructor");

            output.Add(" > Constructs Semantic Cloud (i.e. Semantic Lexicon) from collection of POS chunks.");
            output.Add(" > The algorithm assumes that terms (in lemma form) in the chunk are semantically related to each other.");
            output.Add(" > Atomic element of the cloud is single word term, in lemma form, having its weight and collection of links.");
            output.Add(" > Elements of the cloud are related by monotype links, where links may have their own weight other then 1.");

            output.AddRange(settings.DescribeSelf());

            return output;
        }

        public cloudConstructor()
        {
        }

        public cloudConstructorSettings settings { get; set; } = new cloudConstructorSettings();

        /// <summary>
        /// Processes the specified chunk table into semantic cloud
        /// </summary>
        /// <param name="chunkTable">The chunk table.</param>
        /// <param name="termTable">The term table.</param>
        /// <param name="output">The output.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="subjects">The subjects.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns></returns>
        /// <exception cref="aceScienceException">This is stupid. Settings for cloudConstructor have assignChunkTableWeightToLink=true but it will not create new link in case the lemmas are already linked" +
        /// ", therefore resulting weight is assigned just by chance! Change cloudConstructor settings bro, to make some sense. - null - cloudConstructor has irrational settings</exception>
        public lemmaSemanticCloud process(webLemmaTermTable chunkTable, webLemmaTermTable termTable, lemmaSemanticCloud output, ILogBuilder logger, List<pipelineTaskMCSiteSubject> subjects, ITextResourceResolver resolver)
        {
            if (output == null)
            {
                output = new lemmaSemanticCloud();
                output.className = termTable.name;
            }

            switch (settings.algorithm)
            {
                case cloudConstructorAlgorithm.complex:
                    output = processPOSEnhanced(chunkTable, termTable, output, logger, subjects, resolver);
                    break;

                case cloudConstructorAlgorithm.standard:
                    output = processStandard(chunkTable, termTable, output, logger, subjects);
                    break;

                case cloudConstructorAlgorithm.alternative:
                    output = processAlternative(chunkTable, termTable, output, logger, subjects, resolver);
                    break;
            }

            output.RebuildIndex();

            output.weaverReport = settings.cloudWeaver.Process(output, logger);

            output.RebuildIndex();

            return output;
        }

        /// <summary>
        /// Processes the position enhanced.
        /// </summary>
        /// <param name="chunkTable">The chunk table.</param>
        /// <param name="termTable">The term table.</param>
        /// <param name="output">The output.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="subjects">The subjects.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns></returns>
        protected lemmaSemanticCloud processPOSEnhanced(webLemmaTermTable chunkTable, webLemmaTermTable termTable, lemmaSemanticCloud output, ILogBuilder logger, List<pipelineTaskMCSiteSubject> subjects, ITextResourceResolver resolver)
        {
            List<webLemmaTerm> allChunks = chunkTable.GetList();

            if (output == null) output = new lemmaSemanticCloud();

            // <----------------- PRIMARY
            lemmaSemanticConstruct c = new lemmaSemanticConstruct(subjects);
            lemmaSemanticConstruct cl = new lemmaSemanticConstruct(subjects);
            while (c != cl)
            {
                c = cl;
                cl = lemmaSemanticConstruct.NextIteration(cl, resolver, allChunks, settings, subjects, logger);

                if (cl.createdInIteration > settings.primaryTermOptimizationIterationLimit)
                {
                    c = cl;
                    break;
                }

                if (cl.OptimizationDone)
                {
                    break;
                }
            }

            c = cl;

            // <------------------- PRIM

            c.CollectRelevantTerms(settings.doReserveTermsForClass);

            if (!c.isCaseCloud)
            {
                c.LogConstruct(logger);
            }

            // <---------------------------------

            var docSetFreq = allChunks.Where(x => c.RelevantTerms.Any(y => x.nominalForm.SplitSmart(" ", "", true, true).Contains(y)));

            foreach (webLemmaTerm chunk in docSetFreq)
            {
                var lemmas = chunk.nominalForm.SplitSmart(" ", "", true, true);
                List<String> l_out = new List<string>();
                foreach (String lm in lemmas)
                {
                    if (c.NotProcessed(lm))
                    {
                        var lu = resolver.GetLexicUnit(lm, logger);
                        if (lu == null)
                        {
                            c.TrashBin.AddUnique(lm);
                        }
                        else
                        {
                            var tg = lu.GetTagFromGramTags<pos_type>(pos_type.none);
                            if (tg.ContainsAny(new pos_type[] { pos_type.N, pos_type.A }))
                            {
                                c.ReserveTerms.AddUnique(lm);
                                l_out.Add(lm);
                            }
                            else
                            {
                                c.TrashBin.AddUnique(lm);
                            }
                        }
                    }
                    else
                    {
                        if (!c.TrashBin.Contains(lm))
                        {
                            l_out.Add(lm);
                        }
                    }
                }

                if (l_out.Count > 1)
                {
                    l_out.Sort((x, y) => String.CompareOrdinal(x, y));

                    c.lemmasList.Add(l_out);

                    c.weightDict.Add(l_out, chunk);

                    c.nodeNames.AddRange(l_out, true);
                }
            }

            return BuildCloud(c, chunkTable, termTable, output, logger, resolver);
        }

        /// <summary>
        /// Processes the complex.
        /// </summary>
        /// <param name="chunkTable">The chunk table.</param>
        /// <param name="termTable">The term table.</param>
        /// <param name="output">The output.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="subjects">The subjects.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns></returns>
        protected lemmaSemanticCloud processAlternative(webLemmaTermTable chunkTable, webLemmaTermTable termTable, lemmaSemanticCloud output, ILogBuilder logger, List<pipelineTaskMCSiteSubject> subjects, ITextResourceResolver resolver)
        {
            if (output == null) output = new lemmaSemanticCloud();
            lemmaSemanticConstruct c = new lemmaSemanticConstruct(subjects);

            List<webLemmaTerm> allChunks = chunkTable.GetList();

            // <--------------------------------- DETECTING THE MOST IMPORTANT TERMS
            IEnumerable<webLemmaTerm> vipChunks = null;

            if (subjects.Count > 1)
            {
                vipChunks = allChunks.Where(x => x.documentSetFrequency > settings.documentSetFreqLowLimit);
            }
            else
            {
                vipChunks = allChunks;
            }

            instanceCountCollection<String> lemmaCounter = new instanceCountCollection<string>();
            List<List<String>> primaryLemmaList = new List<List<String>>();

            foreach (webLemmaTerm chunk in vipChunks)
            {
                var lemmas = chunk.nominalForm.SplitSmart(" ", "", true, true);
                lemmas = lemmas.Where(x => x.Length > 2).ToList();

                lemmaCounter.AddInstanceRange(lemmas);
            }

            c.RelevantTerms = lemmaCounter.getSorted();

            lemmaCounter.reCalculate();

            foreach (String term in c.RelevantTerms)
            {
                if (lemmaCounter[term] == lemmaCounter.maxFreq)
                {
                    c.PrimaryTerms.Add(term);
                }
                else if (lemmaCounter[term] > lemmaCounter.minFreq)
                {
                    c.SecondaryTerms.Add(term);
                }
                else
                {
                    c.ReserveTerms.Add(term);
                }
            }

            c.CollectRelevantTerms(settings.doReserveTermsForClass);
            c.LogConstruct(logger);

            // <---------------------------------

            var docSetFreq = allChunks.Where(x => c.RelevantTerms.Any(y => x.nominalForm.SplitSmart(" ", "", true, true).Contains(y)));

            foreach (webLemmaTerm chunk in docSetFreq)
            {
                var lemmas = chunk.nominalForm.SplitSmart(" ", "", true, true);
                lemmas = lemmas.Where(x => x.Length > 2).ToList();

                if (lemmas.Count > 1)
                {
                    lemmas.Sort((x, y) => String.CompareOrdinal(x, y));
                    c.lemmasList.Add(lemmas);

                    c.weightDict.Add(lemmas, chunk);

                    c.nodeNames.AddRange(lemmas, true);
                }
            }

            return BuildCloud(c, chunkTable, termTable, output, logger, resolver);
        }

        /// <summary>
        /// Builds the cloud - common part of the algorithm
        /// </summary>
        /// <param name="c">The c.</param>
        /// <param name="chunkTable">The chunk table.</param>
        /// <param name="termTable">The term table.</param>
        /// <param name="output">The output.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns></returns>
        /// <exception cref="aceScienceException">This is stupid. Settings for cloudConstructor have assignChunkTableWeightToLink=true but it will not create new link in case the lemmas are already linked" +
        ///                                                 ", therefore resulting weight is assigned just by chance! Change cloudConstructor settings bro, to make some sense. - null - cloudConstructor has irrational settings</exception>
        protected lemmaSemanticCloud BuildCloud(lemmaSemanticConstruct c, webLemmaTermTable chunkTable, webLemmaTermTable termTable, lemmaSemanticCloud output, ILogBuilder logger, ITextResourceResolver resolver)
        {
            c.TrashBin.ForEach(x => c.nodeNames.Remove(x));

            foreach (String n in c.nodeNames) // <------------ creating nodes
            {
                Double weight = 0;
                if (settings.assignTermTableWeightToNode)
                {
                    var lemma = termTable[n];
                    if (lemma != null)
                    {
                        weight = lemma.weight;
                    }
                }
                else
                {
                    weight = 1;
                }
                if (weight > 0)
                {
                    if (c.isCaseCloud)
                    {
                        if (settings.doFactorToCaseClouds)
                        {
                            if (c.PrimaryTerms.Contains(n))
                            {
                                output.AddNode(n, weight * settings.PrimaryTermWeightFactor, 2);
                            }
                            else if (c.SecondaryTerms.Contains(n))
                            {
                                output.AddNode(n, weight * settings.SecondaryTermWeightFactor, 1);
                            }
                            else
                            {
                                output.AddNode(n, weight * settings.ReserveTermWeightFactor, 0);
                            }
                        }
                        else
                        {
                            output.AddNode(n, weight);
                        }
                    }
                    else
                    {
                        // class cloud
                        if (settings.doFactorToClassClouds)
                        {
                            if (c.PrimaryTerms.Contains(n))
                            {
                                output.AddNode(n, weight * settings.PrimaryTermWeightFactor, 2);
                            }
                            else if (c.SecondaryTerms.Contains(n))
                            {
                                output.AddNode(n, weight * settings.SecondaryTermWeightFactor, 1);
                            }
                            else
                            {
                                output.AddNode(n, weight * settings.ReserveTermWeightFactor, 0);
                            }
                        }
                        else
                        {
                            output.AddNode(n, weight);
                        }
                    }
                }
            }

            foreach (List<String> n in c.lemmasList) // <-------- creating links
            {
                String first = n[0];
                if (c.TrashBin.Contains(first)) continue;

                if (output.ContainsNode(first, true))
                {
                    foreach (String m in n)
                    {
                        if (c.TrashBin.Contains(m)) continue;
                        if (m != first)
                        {
                            if (output.ContainsNode(m, true))
                            {
                                Double weight = 1;
                                if (settings.assignChunkTableWeightToLink)
                                {
                                    weight = c.weightDict[n].weight;
                                }
                                else
                                {
                                    if (settings.doAdjustLinkWeightByChunkSize)
                                    {
                                        weight = (n.Count - 1).GetRatio(1);
                                    }
                                    else
                                    {
                                        weight = 1;
                                    }
                                }
                                var link = output.GetLink(first, m);
                                if (link == null)
                                {
                                    output.AddLink(first, m, weight);
                                }
                                else
                                {
                                    if (settings.doSumExistingLinkWeights)
                                    {
                                        link.weight += weight;
                                    }
                                    else
                                    {
                                        // it will not create new link as it already exists
                                        // this is irrational in case settings.assignChunkTableWeightToLink is true
                                        if (settings.assignChunkTableWeightToLink)
                                        {
                                            throw new aceScienceException("This is stupid. Settings for cloudConstructor have assignChunkTableWeightToLink=true but it will not create new link in case the lemmas are already linked" +
                                                ", therefore resulting weight is assigned just by chance! Change cloudConstructor settings bro, to make some sense.", null, this, "cloudConstructor has irrational settings", settings);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            c.primaryChunks.ForEach(x => output.primaryChunks.Add(x.nominalForm));
            c.secondaryChunks.ForEach(x => output.secondaryChunks.Add(x.nominalForm));

            return output;
        }

        protected lemmaSemanticCloud processStandard(webLemmaTermTable chunkTable, webLemmaTermTable termTable, lemmaSemanticCloud output, ILogBuilder logger, List<pipelineTaskMCSiteSubject> subjects)
        {
            if (output == null) output = new lemmaSemanticCloud();

            List<webLemmaTerm> allChunks = chunkTable.GetList();

            IEnumerable<webLemmaTerm> docSetFreq = null;

            if (subjects.Count > 1)
            {
                docSetFreq = allChunks.Where(x => x.documentSetFrequency > settings.documentSetFreqLowLimit);
            }
            else
            {
                docSetFreq = allChunks;
            }

            //  allChunks.Where(x => x.documentSetFrequency > settings.documentSetFreqLowLimit);

            Dictionary<List<String>, webLemmaTerm> weightDict = new Dictionary<List<String>, webLemmaTerm>();

            List<List<String>> lemmasList = new List<List<string>>();
            List<String> nodeNames = new List<string>();

            foreach (webLemmaTerm chunk in docSetFreq)
            {
                var lemmas = chunk.nominalForm.SplitSmart(" ", "", true, true);
                lemmas = lemmas.Where(x => x.Length > 2).ToList();

                if (lemmas.Count > 1)
                {
                    lemmas.Sort((x, y) => String.CompareOrdinal(x, y));
                    lemmasList.Add(lemmas);

                    weightDict.Add(lemmas, chunk);

                    nodeNames.AddRange(lemmas, true);
                }
            }

            foreach (String n in nodeNames) // <------------ creating nodes
            {
                Double weight = 0;
                if (settings.assignTermTableWeightToNode)
                {
                    weight = termTable.ResolveSingleTerm(n, logger);
                }
                else
                {
                    weight = 1;
                }
                if (weight > 0)
                {
                    output.AddNode(n, weight);
                }
            }

            foreach (List<String> n in lemmasList) // <-------- creating links
            {
                String first = n[0];
                if (output.ContainsNode(first, true))
                {
                    foreach (String m in n)
                    {
                        if (m != first)
                        {
                            if (output.ContainsNode(m, true))
                            {
                                Double weight = 1;
                                if (settings.assignChunkTableWeightToLink)
                                {
                                    weight = weightDict[n].weight;
                                }
                                else
                                {
                                    if (settings.doAdjustLinkWeightByChunkSize)
                                    {
                                        weight = (n.Count - 1).GetRatio(1);
                                    }
                                    else
                                    {
                                        weight = 1;
                                    }
                                }
                                var link = output.GetLink(first, m);
                                if (link == null)
                                {
                                    output.AddLink(first, m, weight);
                                }
                                else
                                {
                                    if (settings.doSumExistingLinkWeights)
                                    {
                                        link.weight += weight;
                                    }
                                    else
                                    {
                                        // it will not create new link as it already exists
                                        // this is irrational in case settings.assignChunkTableWeightToLink is true
                                        if (settings.assignChunkTableWeightToLink)
                                        {
                                            throw new aceScienceException("This is stupid. Settings for cloudConstructor have assignChunkTableWeightToLink=true but it will not create new link in case the lemmas are already linked" +
                                                ", therefore resulting weight is assigned just by chance! Change cloudConstructor settings bro, to make some sense.", null, this, "cloudConstructor has irrational settings", settings);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return output;
        }
    }
}