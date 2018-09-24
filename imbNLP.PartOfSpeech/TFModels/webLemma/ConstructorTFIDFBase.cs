// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConstructorTFIDFBase.cs" company="imbVeles" >
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
using imbNLP.PartOfSpeech.TFModels.webLemma.kernels;
using imbNLP.PartOfSpeech.TFModels.webLemma.table;

// // using imbMiningContext.TFModels.WLF_ISF;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Serialization;

namespace imbNLP.PartOfSpeech.TFModels.webLemma
{
    public abstract class ConstructorTFIDFBase
    {
        /// <summary>
        /// Creates multi-line description of current configuration
        /// </summary>
        /// <returns>List of description lines</returns>
        public abstract List<String> DescribeSelf();

        /// <summary>
        /// Name of the kernel engine to be used for weight computation
        /// </summary>
        /// <value>
        /// The name of the weight kernel.
        /// </value>
        public String weightKernelName { get; set; } = "TF-cIDF";

        public wlfConstructorSettings settings { get; set; } = new wlfConstructorSettings();

        /// <summary>
        /// Recomputes the specified table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="forSingleWebSite">if set to <c>true</c> [for single web site].</param>
        /// <param name="lemmas">The lemmas.</param>
        /// <returns></returns>
        /// <exception cref="aceGeneralException">Permanent Add() lemma problem at [" + table.name + "] - Permanent Lemma TF-IDF Add(Lemma) failure</exception>
        public virtual webLemmaTermTable recompute(webLemmaTermTable table, ILogBuilder logger, Boolean forSingleWebSite, List<webLemmaTerm> lemmas)
        {
            kernelComputeWeightTask kernelTask = new kernelComputeWeightTask(lemmas, logger, forSingleWebSite, settings);

            ITermWeightKernel kernel = kernelManager.GetKernel(weightKernelName);

            kernel.compute(kernelTask);

            #region OLD_CODE

            /*
            Double documentSetFrequencyMax = 0;
            Double documentFrequencyMax = 0;
            Double termFrequencyMax = 0;

           // List<webLemmaTerm> lemmas = tabl//e.GetList();

            if (lemmas.Count == 0)
            {
                logger.log("ERROR: NO ENTRIES IN TF-TDF TABLE [" + table.name + "] - is for single web site [" + forSingleWebSite.ToString() + "]");
            }

            foreach (webLemmaTerm lemma in lemmas)
            {
                documentSetFrequencyMax = Math.Max(documentSetFrequencyMax, lemma.documentSetFrequency);
                documentFrequencyMax = Math.Max(documentFrequencyMax, lemma.documentFrequency);
                termFrequencyMax = Math.Max(termFrequencyMax, lemma.termFrequency);
            }

            if (forSingleWebSite) {
                if (settings.doAdjustIDFForCase)
                {
                    documentFrequencyMax = (documentFrequencyMax * settings.documentFrequencyMaxFactor) + settings.documentFrequencyMaxCorrection;
                } else
                {
                    documentFrequencyMax = documentFrequencyMax + settings.documentFrequencyMaxCorrection;
                }
            } else
            {
                documentFrequencyMax = (documentFrequencyMax * settings.documentFrequencyMaxFactor) + settings.documentFrequencyMaxCorrection;
            }

            /// COMPUTING NON NORMALIZED WEIGHTs
            ///
            Double weightMax = Double.MinValue;

            foreach (webLemmaTerm lemma in lemmas)
            {
                lemma.termFrequency = lemma.termFrequency.GetRatio(termFrequencyMax);

                if (settings.doUseIDF)
                {
                    if (settings.doUseNaturalLog)
                    {
                        lemma.documentFactor = Math.Log(documentFrequencyMax.GetRatio(lemma.documentFrequency));
                    } else
                    {
                        lemma.documentFactor = Math.Log10(documentFrequencyMax.GetRatio(lemma.documentFrequency));
                    }
                } else
                {
                    lemma.documentFactor = 1;
                }

                lemma.weight = lemma.termFrequency * lemma.documentFactor;

                if (settings.doUseDocumentSet)
                {
                    if ((documentSetFrequencyMax != 1) || !forSingleWebSite)
                    {
                        if (lemma.documentSetFrequency == 0)
                        {
                            lemma.weight = 0;
                        }
                        else
                        {
                            Double docSetFactor = (1 - Math.Log10(documentSetFrequencyMax / lemma.documentSetFrequency));
                            lemma.weight = lemma.weight * docSetFactor;
                        }
                    }
                }

                weightMax = Math.Max(weightMax, lemma.weight);
            }

            /// WEIGHT NORMALIZATION
            foreach (webLemmaTerm lemma in lemmas)
            {
                lemma.weight = lemma.weight.GetRatio(weightMax);
            }
            */

            #endregion OLD_CODE

            /// SAVING THE RESULTS
            Int32 globalRetry = retry_global_limit;
            foreach (webLemmaTerm lemma in lemmas)
            {
                Int32 retry = retry_limit;
                while (retry > 0)
                {
                    try
                    {
                        table.Add(lemma);
                        retry = 0;
                    }
                    catch (Exception ex)
                    {
                        retry--;
                        globalRetry--;

                        if (doBeep)
                        {
                            logger.log("WFT [" + table.name + "] add lemma [" + lemma.name + "]  retries left [" + retry + "] global[" + globalRetry + "]");
                            imbACE.Services.terminal.aceTerminalInput.doBeepViaConsole(1200, 200, 1);
                        }
                        Thread.Sleep(250);

                        if (globalRetry < 0)
                        {
                            throw new aceGeneralException("Permanent Add() lemma problem at [" + table.name + "]", ex, this, "Permanent Lemma TF-IDF Add(Lemma) failure");
                        }
                    }
                }
            }

            logger.log("WFT [" + table.name + "] recomputed TFmax[" + kernelTask.weightMax + "] : DFmax[" + kernelTask.documentFrequencyMax + "]  TC[" + lemmas.Count + "]");

            return table;
        }

        public const Int32 retry_limit = 5;
        public const Int32 retry_delay = 250;
        public const Int32 retry_global_limit = 250;

        [XmlIgnore]
        public Boolean doBeep { get; set; } = true;
    }
}