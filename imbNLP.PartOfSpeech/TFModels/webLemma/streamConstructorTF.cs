using imbMiningContext.MCDocumentStructure;
using imbNLP.PartOfSpeech.flags.token;
using imbNLP.PartOfSpeech.pipeline.machine;
using imbNLP.PartOfSpeech.pipelineForPos.subject;
using imbNLP.PartOfSpeech.resourceProviders.core;
using imbNLP.PartOfSpeech.TFModels.webLemma.table;
using imbSCI.Core.extensions.data;
using imbSCI.Core.math;
using imbSCI.Core.reporting;
using imbSCI.DataComplex.tf_idf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.PartOfSpeech.TFModels.webLemma
{
    /// <summary>
    /// Constructs <see cref="webLemmaTermTable"/> for streams
    /// </summary>
    /// <seealso cref="imbNLP.PartOfSpeech.TFModels.webLemma.ConstructorTFIDFBase" />
    /// <seealso cref="imbNLP.PartOfSpeech.TFModels.webLemma.core.IWLFConstructor" />
    public class streamConstructorTF : ConstructorTFIDFBase
    {
        public streamConstructorTF()
        {
        }

        /// <summary>
        /// Creates multi-line description of current configuration
        /// </summary>
        /// <returns>List of description lines</returns>
        public override List<string> DescribeSelf()
        {
            List<String> output = new List<string>();

            output.Add("## Token Stream TF-IDF constructor");

            output.Add(" > Constructs TF-IDF table for extracted token streams, in order to calculate weights and other statistics.");
            output.Add(" > HTML Tag factors are ignored for token streams");

            output.AddRange(settings.DescribeSelf());

            return output;
        }

        /// <summary>
        /// Prepares the counter.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public TFDFCounter prepareCounter(IEnumerable<IPipelineTaskSubject> source)
        {
            TFDFCounter counter = new TFDFCounter();

            var listChunks = source.ToList();

            listChunks.Sort((x, y) => String.CompareOrdinal(x.currentForm, y.currentForm));

            foreach (pipelineTaskSubjectContentToken mcSubject in source)
            {
                if (mcSubject.contentLevelType == cnt_level.mcTokenStream)
                {
                    counter.Add(mcSubject.currentForm, mcSubject);
                }
            }

            return counter;
        }

        /// <summary>
        /// Processes the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="document_level">The document level.</param>
        /// <param name="table">The table.</param>
        /// <param name="parser">The parser.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="forSingleWebSite">if set to <c>true</c> [for single web site].</param>
        /// <param name="counter">The counter.</param>
        /// <returns></returns>
        public webLemmaTermTable process(IEnumerable<IPipelineTaskSubject> source, cnt_level document_level, webLemmaTermTable table = null, ITextResourceResolver parser = null, ILogBuilder logger = null, bool forSingleWebSite = false, TFDFCounter counter = null)
        {
            if (counter == null) counter = prepareCounter(source);

            List<String> tfdfList = counter.GetIndexForms();

            tfdfList.Sort(String.CompareOrdinal);

            Int32 i = 0;
            Int32 c = 0;
            Int32 li = 0;
            Int32 limit = tfdfList.Count() + 500;

            List<webLemmaTerm> lemmas = new List<webLemmaTerm>();
            Int32 startIndex = tfdfList.Count();
            Int32 cycleLength = startIndex / 5;

            foreach (String term in tfdfList)
            {
                if (term != null)
                {
                    List<imbMCDocumentElement> documentSet = new List<imbMCDocumentElement>();
                    List<imbMCDocumentElement> documents = new List<imbMCDocumentElement>();

                    Double documentFrequency = 0;
                    Double termFrequency = 0;

                    TFDFContainer cn = counter.GetContainer(term);

                    webLemmaTerm lemma = new webLemmaTerm();

                    if (cn != null)
                    {
                        lemma.nominalForm = cn.indexForm;
                        lemma.name = cn.indexForm;

                        foreach (pipelineTaskSubjectContentToken cntPair in cn.items)
                        {
                            imbMCDocument document = cntPair?.mcElement?.GetParentOfType<imbMCDocument>();
                            if (document != null)
                            {
                                documents.AddUnique(document);

                                imbMCDocumentSet docSet = document?.parent as imbMCDocumentSet;
                                if (docSet != null)
                                {
                                    documentSet.AddUnique(docSet);
                                }
                            }
                            termFrequency += 1;
                        }

                        lemma.documentSetFrequency = documentSet.Count;
                        lemma.AFreqPoints = cn.items.Count();
                        lemma.documentFrequency = documents.Count;
                        lemma.termFrequency = termFrequency;
                        lemmas.Add(lemma);
                    }
                    else
                    {
                        //lemma.otherForms.AddUnique(cn.items);
                    }

                    li++;
                    i = i + 1;
                    c = c + 1;

                    if (c > cycleLength)
                    {
                        c = 0;
                        logger.AppendLine();
                        logger.log("Token Streams TF processing: _" + i.GetRatio(startIndex).ToString("P2") + "_ ");
                        logger.AppendLine();
                    }

                    if (li > limit)
                    {
                        logger.log("Limit broken at processing Token Streams TF processing at [" + li.ToString() + "]");
                        break;
                    }
                }
            }

            recompute(table, logger, forSingleWebSite, lemmas);

            return table;
        }
    }
}