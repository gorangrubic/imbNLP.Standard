// --------------------------------------------------------------------------------------------------------------------
// <copyright file="wlfConstructorTFIDF.cs" company="imbVeles" >
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

// // using imbMiningContext.TFModels.WLF_ISF;
using imbMiningContext.MCDocumentStructure;
using imbNLP.PartOfSpeech.flags.basic;
using imbNLP.PartOfSpeech.flags.token;
using imbNLP.PartOfSpeech.lexicUnit;
using imbNLP.PartOfSpeech.pipeline.machine;
using imbNLP.PartOfSpeech.pipelineForPos.subject;
using imbNLP.PartOfSpeech.resourceProviders.core;
using imbNLP.PartOfSpeech.TFModels.webLemma.core;
using imbNLP.PartOfSpeech.TFModels.webLemma.table;
using imbSCI.Core.extensions.data;
using imbSCI.Core.extensions.text;
using imbSCI.Core.math;
using imbSCI.Core.reporting;
using imbSCI.Data;
using imbSCI.DataComplex.tf_idf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace imbNLP.PartOfSpeech.TFModels.webLemma
{
    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="imbNLP.PartOfSpeech.TFModels.webLemma.core.IWLFConstructor" />
    public class wlfConstructorTFIDF : ConstructorTFIDFBase, IWLFConstructor
    {
        public wlfConstructorTFIDF()
        {
        }

        /// <summary>
        /// Creates multi-line description of current configuration
        /// </summary>
        /// <returns>List of description lines</returns>
        public override List<string> DescribeSelf()
        {
            List<String> output = new List<string>();

            output.Add("## Web Lemma TF-IDF constructor");

            output.Add(" > Constructs TF-IDF table for single-word terms (in lemma form), in order to calculate weights and other statistics.");

            output.AddRange(settings.DescribeSelf());

            return output;
        }

        private Object getAllChildrenLock = new Object();

        //process(IEnumerable<IPipelineTaskSubject> source, cnt_level document_level, webLemmaTermTable table, ILogBuilder logger = null, Boolean forSingleWebSite = false)*
        public webLemmaTermTable process(IEnumerable<IPipelineTaskSubject> source, cnt_level document_level, webLemmaTermTable table = null,
            ITextResourceResolver parser = null, ILogBuilder logger = null, Boolean forSingleWebSite = false)
        {
            if (table.Count > 0)
            {
                logger.log("THIS TABLE " + table.name + " ALREADY HAS [" + table.Count + "] ITEMS --- HALTING BUILD [For single web site: " + forSingleWebSite + "]");
                if (DoBeep == 1)
                {
                    imbACE.Services.terminal.aceTerminalInput.doBeepViaConsole(1200, 250);
                    Interlocked.Increment(ref DoBeep);
                }
                return table;
            }

            TFDFCounter counter = new TFDFCounter();
            lock (getAllChildrenLock)
            {
                var listSource = source.ToList();
                // listSource.Sort((x, y) => String.CompareOrdinal(x.currentForm, y.currentForm));
                source = listSource;
            }

            List<IPipelineTaskSubject> rkns = source.GetSubjectsOfLevel<IPipelineTaskSubject>(new cnt_level[] { cnt_level.mcToken }); // source.GetSubjectChildrenTokenType<pipelineTaskSubjectContentToken, IPipelineTaskSubject>();

            rkns.Sort((x, y) => String.CompareOrdinal(x.currentForm, y.currentForm));

            //var tkns = source.GetSubjectsOfLevel(cnt_level.mcToken);
            Int32 shorties = 0;
            foreach (var tkn in rkns)
            {
                //if (tkn.currentForm.Length > 1)
                //{
                if (tkn.flagBag.ContainsAll(tkn_contains.onlyLetters))
                {
                    counter.Add(tkn.currentForm.ToLower(), tkn);
                }
                //} else
                //{
                //    shorties++;
                //}
            }

            if (shorties > 0)
            {
                logger.log("[" + shorties + "] too short tokens removed");
            }

            /*

            List<pipelineTaskSubjectContentToken> MCPageSubjects = source.ToSubjectTokenType<pipelineTaskSubjectContentToken>();

            foreach (pipelineTaskSubjectContentToken mcSubject in MCPageSubjects)
            {
                counter.NextDocument();
                List<pipelineTaskSubjectContentToken> tkns = new List<pipelineTaskSubjectContentToken>();
                lock (getAllChildrenLock)
                {
                    tkns = mcSubject.getAllChildrenInType<pipelineTaskSubjectContentToken>(null, false, false).GetSubjectsOfLevel(cnt_level.mcToken);
                }
                foreach (var tkn in tkns)
                {
                    if (tkn.flagBag.ContainsAll(tkn_contains.onlyLetters))
                    {
                        counter.Add(tkn.currentForm.ToLower(), tkn);
                    }
                }
            }
            */
            return process(table.name, parser, counter, logger, table, forSingleWebSite);
        }

        protected static Int32 DoBeep = 1;

        /// <summary>
        /// Constructs the webLemmaTable
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="parser">The parser.</param>
        /// <param name="counter">The counter.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        protected webLemmaTermTable process(String tableName, ITextResourceResolver parser, TFDFCounter counter, ILogBuilder logger, webLemmaTermTable table = null, Boolean forSingleWebSite = false)
        {
            if (table == null) table = new webLemmaTermTable(tableName);

            if (table.Count > 0)
            {
                logger.log("THIS TABLE " + tableName + " ALREADY HAS [" + table.Count + "] ITEMS --- HALTING BUILD [For single web site: " + forSingleWebSite + "]");
                if (DoBeep == 1)
                {
                    imbACE.Services.terminal.aceTerminalInput.doBeepViaConsole(1200, 250);
                    Interlocked.Increment(ref DoBeep);
                }
                return table;
            }

            List<String> tfdfList = counter.GetIndexForms();
            Int32 i = 0;
            Int32 c = 0;
            Int32 li = 0;
            Int32 limit = tfdfList.Count + 100;

            if (!tableName.isNullOrEmpty()) table.name = tableName;

            List<webLemmaTerm> lemmas = new List<webLemmaTerm>();

            Int32 startIndex = tfdfList.Count;
            Int32 cycleLength = startIndex / 5;

            while (tfdfList.Any())
            {
                String term = tfdfList.FirstOrDefault();
                Int32 d = tfdfList.Count;

                if (term != null)
                {
                    lexicGraphSetWithLemma inflectSet = parser.GetLemmaSetForInflection(term, tfdfList, logger);
                    d = d - tfdfList.Count;
                    if (d == 0)
                    {
                        table.unresolved.Add(term);
                        tfdfList.Remove(term);
                        d = 1;
                    }
                    else
                    {
                        Boolean ok = true;

                        if (settings.allowedLemmaTypes.Any())
                        {
                            var tps = inflectSet.GetTagsFromGramTags<pos_type>(pos_type.none);

                            if (settings.strictPosTypePolicy)
                            {
                                if (!tps.ContainsAny(settings.allowedLemmaTypes))
                                {
                                    ok = false;
                                }
                                else
                                {
                                    if (tps.Contains(pos_type.V))
                                    {
                                        ok = false;
                                    }
                                    //foreach (pos_type t in tps)
                                    //{
                                    //    if (!settings.allowedLemmaTypes.Contains(t))
                                    //    {
                                    //        ok = false;
                                    //        break;
                                    //    }
                                    //}
                                }
                            }
                            else
                            {
                                if (!tps.ContainsAny(settings.allowedLemmaTypes))
                                {
                                    ok = false;
                                }
                                else
                                {
                                }
                            }
                        }
                        else
                        {
                        }

                        if (ok)
                        {
                            List<imbMCDocumentElement> documents = new List<imbMCDocumentElement>();
                            List<imbMCDocumentElement> documentSet = new List<imbMCDocumentElement>();

                            webLemmaTerm lemma = new webLemmaTerm();
                            lemma.nominalForm = inflectSet.lemmaForm;
                            lemma.name = inflectSet.lemmaForm;

                            Double documentFrequency = 0;
                            Double termFrequency = 0;

                            foreach (lexicInflection inflect in inflectSet.Values)
                            {
                                TFDFContainer cn = counter.GetContainer(inflect.inflectedForm);
                                if (cn != null)
                                {
                                    lemma.AFreqPoints += cn.items.Count;
                                    foreach (pipelineTaskSubjectContentToken cntPair in cn.items)
                                    {
                                        imbMCDocument document = cntPair.mcElement.GetParentOfType<imbMCDocument>();
                                        documents.AddUnique(document);

                                        imbMCDocumentElement docSet = document?.parent as imbMCDocumentElement;
                                        if (docSet != null)
                                        {
                                            documentSet.AddUnique(docSet);
                                        }
                                        else
                                        {
                                            logger.log(cn.indexForm + " (" + cntPair.mcElement.toStringSafe("mcElement=null") + ")");
                                        }

                                        if (cntPair.flagBag.Contains(cnt_containerType.link))
                                        {
                                            termFrequency += settings.anchorTextFactor;
                                        }
                                        else if (cntPair.flagBag.Contains(cnt_containerType.title))
                                        {
                                            termFrequency += settings.titleTextFactor;
                                        }
                                        else
                                        {
                                            termFrequency += settings.contentTextFactor;
                                        }

                                        cntPair.AddGraph(inflect);
                                    }

                                    lemma.otherForms.AddUnique(cn.indexForm);
                                }
                                else
                                {
                                    lemma.otherForms.AddUnique(inflect.inflectedForm);
                                }
                            }
                            lemma.documentSetFrequency = documentSet.Count;
                            lemma.documentFrequency = documents.Count;
                            lemma.termFrequency = termFrequency;
                            lemmas.Add(lemma);
                            //table.Add(lemma);
                        }
                        else
                        {
                        }
                    }
                }
                li++;
                i = i + d;
                c = c + d;
                d = startIndex - tfdfList.Count;

                if (c > cycleLength)
                {
                    c = 0;
                    logger.AppendLine();
                    logger.log("TF-IDF processed: _" + d.GetRatio(startIndex).ToString("P2") + "_");
                    logger.AppendLine();
                }

                if (li > limit)
                {
                    logger.log("Limit broken at processing WEB Lemma Frequency table at [" + li.ToString() + "]");
                    break;
                }
            }

            if (settings.doComputeTFIDF)
            {
                recompute(table, logger, forSingleWebSite, lemmas);
            }
            else
            {
                foreach (var le in lemmas)
                {
                    table.Add(le);
                }
            }

            //  table.ReadOnlyMode = true;

            return table;
        }
    }
}