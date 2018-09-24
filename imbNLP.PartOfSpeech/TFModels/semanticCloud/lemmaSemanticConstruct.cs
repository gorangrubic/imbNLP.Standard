// --------------------------------------------------------------------------------------------------------------------
// <copyright file="lemmaSemanticConstruct.cs" company="imbVeles" >
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
using imbNLP.PartOfSpeech.flags.basic;
using imbNLP.PartOfSpeech.pipelineForPos.subject;
using imbNLP.PartOfSpeech.resourceProviders.core;
using imbNLP.PartOfSpeech.TFModels.webLemma.table;

// using imbNLP.PartOfSpeech.TFModels.semanticCloud.core;
using imbSCI.Core.extensions.data;
using imbSCI.Core.reporting;
using imbSCI.Data;
using imbSCI.DataComplex.special;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace imbNLP.PartOfSpeech.TFModels.semanticCloud
{
    /// <summary>
    /// Temporary object used for easier modification of cloud construction algorithm
    /// </summary>
    public class lemmaSemanticConstruct
    {
        public static lemmaSemanticConstruct NextIteration(lemmaSemanticConstruct lastIteration, ITextResourceResolver resolver, List<webLemmaTerm> allChunks, cloudConstructorSettings settings, List<pipelineTaskMCSiteSubject> subjects, ILogBuilder logger)
        {
            var cl = lastIteration;

            var c = new lemmaSemanticConstruct(subjects);
            c.createdInIteration = lastIteration.createdInIteration + 1;
            c.PTCountMin = Math.Min(lastIteration.PTCountMin, lastIteration.PrimaryTerms.Count);
            c.PTCountMax = Math.Max(lastIteration.PTCountMax, lastIteration.PrimaryTerms.Count);

            if (!c.isCaseCloud)
            {
                c.onTopChunks.AddRange(allChunks.Where(x => x.documentSetFrequency > (settings.documentSetFreqLowLimit + lastIteration.createdInIteration)));
            }
            else
            {
                if (!settings.doFactorToCaseClouds)
                {
                    c.OptimizationDone = true;
                }
                c.onTopChunks = allChunks;
            }

            if (!c.isCaseCloud)
            {
                instanceCountCollection<String> lemmaCounter = new instanceCountCollection<string>();
                List<List<String>> primaryLemmaList = new List<List<String>>();

                foreach (webLemmaTerm chunk in c.onTopChunks)
                {
                    var lemmas = chunk.nominalForm.SplitSmart(" ", "", true, true);
                    lemmaCounter.AddInstanceRange(lemmas);
                }

                lemmaCounter.reCalculate();

                foreach (String st in lemmaCounter)
                {
                    if (lemmaCounter.maxFreq == 1 || lemmaCounter[st] > 1)
                    {
                        var lu = resolver.GetLexicUnit(st, logger);
                        if (lu == null)
                        {
                            c.TrashBin.AddUnique(st);
                        }
                        else
                        {
                            var tg = lu.GetTagFromGramTags<pos_type>(pos_type.none);
                            if (tg.Contains(pos_type.N))
                            {
                                c.PrimaryTerms.AddUnique(st);
                            }
                            else if (tg.Contains(pos_type.A))
                            {
                                c.SecondaryTerms.AddUnique(st);
                            }
                            else
                            {
                                c.TrashBin.AddUnique(st);
                            }
                        }
                    }
                }; // <---------------------------- Primary terms extracted

                if (c.PrimaryTerms.Count == 0)
                {
                    if (c.SecondaryTerms.Any())
                    {
                        logger.log(":: Moving Adjective terms [" + c.SecondaryTerms.Count + "] to Primary Terms category, as no Nouns were qualified to the cateogry");
                        c.PrimaryTerms.AddRange(c.SecondaryTerms);
                        c.SecondaryTerms.Clear();
                    }
                }
            }

            instanceCountCollection<String> secondCounter = new instanceCountCollection<string>();
            foreach (webLemmaTerm chunk in allChunks)
            {
                var lemmas = chunk.nominalForm.SplitSmart(" ", "", true, true);
                secondCounter.AddInstanceRange(lemmas);
            }

            foreach (webLemmaTerm chunk in allChunks)
            {
                var lemmas = chunk.nominalForm.SplitSmart(" ", "", true, true);

                if (lemmas.ContainsAny(c.PrimaryTerms))
                {
                    if (c.onTopChunks.Contains(chunk))
                    {
                        c.primaryChunks.Add(chunk);
                    }
                    else
                    {
                        c.secondaryChunks.Add(chunk);
                    }

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
                                    c.SecondaryTerms.AddUnique(lm);
                                }
                                else
                                {
                                    c.TrashBin.AddUnique(lm);
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (String lm in lemmas)
                    {
                        if (secondCounter[lm] > settings.termInChunkLowerLimit)
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
                                    }
                                    else
                                    {
                                        c.TrashBin.AddUnique(lm);
                                    }
                                }
                            }
                        }
                        else
                        {
                            c.TrashBin.AddUnique(lm);
                        }
                    }
                }
            }

            if (c.OptimizationDone) return c;

            c.PTCountMin = Math.Min(lastIteration.PTCountMin, c.PrimaryTerms.Count);
            c.PTCountMax = Math.Max(lastIteration.PTCountMax, c.PrimaryTerms.Count);

            if (c.PrimaryTerms.Count <= settings.primaryTermLowTargetCount)
            {
                if (lastIteration.PrimaryTerms.Count < c.PrimaryTerms.Count)
                {
                    logger.log("[" +
                    c.createdInIteration.ToString("D3") + "] PrimaryTerms count [" + c.PrimaryTerms.Count + "] after [" + c.createdInIteration + "] iterations optimized ---- Max[" + c.PTCountMax + "] Min[" + c.PTCountMin + "] T:" + Thread.CurrentThread.Name);
                }
                else
                {
                    logger.log("[" + c.createdInIteration.ToString("D3") + "] PrimaryTerms count changed from [" + lastIteration.PrimaryTerms.Count + "] to [" + c.PrimaryTerms.Count + "]  --- Max[" + c.PTCountMax + "] Min[" + c.PTCountMin + "]  T:" + Thread.CurrentThread.Name);

                    logger.log("[" +
                    c.createdInIteration.ToString("D3") + "] previous PrimaryTerms count [" + lastIteration.PrimaryTerms.Count + "] accepted, after [" + c.createdInIteration + "]  ---- Max[" + c.PTCountMax + "] Min[" + c.PTCountMin + "] T:" + Thread.CurrentThread.Name);
                    c = lastIteration;
                }

                c.OptimizationDone = true;
            }
            else
            {
                logger.log("[" + c.createdInIteration.ToString("D3") + "] PrimaryTerms count changed from [" + lastIteration.PrimaryTerms.Count + "] to [" + c.PrimaryTerms.Count + "]  --- Max[" + c.PTCountMax + "] Min[" + c.PTCountMin + "]  T:" + Thread.CurrentThread.Name);
            }

            return c;
        }

        public Boolean OptimizationDone { get; set; } = false;

        public Int32 createdInIteration { get; set; } = 0;

        public Int32 PTCountMax { get; set; } = Int32.MinValue;
        public Int32 PTCountMin { get; set; } = Int32.MaxValue;

        public lemmaSemanticConstruct(List<pipelineTaskMCSiteSubject> _subject)
        {
            subject = _subject;
            if (subject.Count() == 1) isCaseCloud = true;
        }

        public List<pipelineTaskMCSiteSubject> subject { get; set; } = new List<pipelineTaskMCSiteSubject>();

        /// <summary>
        /// Lemma form of primary chunks
        /// </summary>
        /// <value>
        /// The primary chunks.
        /// </value>
        public List<webLemmaTerm> primaryChunks { get; set; } = new List<webLemmaTerm>();

        public List<webLemmaTerm> onTopChunks { get; set; } = new List<webLemmaTerm>();

        public List<webLemmaTerm> secondaryChunks { get; set; } = new List<webLemmaTerm>();

        public List<String> PrimaryTerms { get; set; } = new List<string>();
        public List<String> SecondaryTerms { get; set; } = new List<string>();
        public List<String> ReserveTerms { get; set; } = new List<string>();

        public List<String> RelevantTerms { get; set; } = new List<string>();

        public Boolean isCaseCloud { get; set; } = false;

        public List<String> TrashBin = new List<string>();

        public Dictionary<List<String>, webLemmaTerm> weightDict = new Dictionary<List<String>, webLemmaTerm>();

        public List<List<String>> lemmasList = new List<List<string>>();
        public List<String> nodeNames = new List<string>();

        public Boolean NotProcessed(String lm)
        {
            return !PrimaryTerms.Contains(lm) && !TrashBin.Contains(lm) && !SecondaryTerms.Contains(lm) && !ReserveTerms.Contains(lm);
        }

        public void CollectRelevantTerms(Boolean doReserveTermsForClass)
        {
            RelevantTerms.Clear();
            RelevantTerms.AddRange(PrimaryTerms);
            RelevantTerms.AddRange(SecondaryTerms);

            if (!isCaseCloud)
            {
                if (doReserveTermsForClass)
                {
                    RelevantTerms.AddRange(ReserveTerms);
                }
            }
        }

        public void LogConstruct(ILogBuilder logger)
        {
            if (logger != null)
            {
                logger.log("Primary Terms   [   " + PrimaryTerms.Count + "      ] -- Secondary Terms    [" + SecondaryTerms.Count + "   ]");
                logger.log("Primary Chunks  [   " + primaryChunks.Count + "     ] -- Sec.Chunks         [" + secondaryChunks.Count + "  ]");
                logger.log("Reserve Terms   [   " + RelevantTerms.Count + "     ] -- Trash bin          [" + TrashBin.Count + "         ]");
            }
        }
    }
}