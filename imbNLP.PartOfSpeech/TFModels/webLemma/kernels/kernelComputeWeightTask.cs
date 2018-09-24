using imbNLP.PartOfSpeech.TFModels.webLemma.table;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;

namespace imbNLP.PartOfSpeech.TFModels.webLemma.kernels
{
    /// <summary>
    /// Temporary data used by weight computation kernel
    /// </summary>
    public class kernelComputeWeightTask
    {
        /// <summary>
        /// Highest document set frequency
        /// </summary>
        /// <value>
        /// The document set frequency maximum.
        /// </value>
        public Double documentSetFrequencyMax { get; set; } = 0;

        /// <summary>
        /// Highest document frequency
        /// </summary>
        /// <value>
        /// The document frequency maximum.
        /// </value>
        public Double documentFrequencyMax { get; set; } = 0;

        /// <summary>
        /// Highest term frequency
        /// </summary>
        /// <value>
        /// The term frequency maximum.
        /// </value>
        public Double termFrequencyMax { get; set; } = 0;

        /// <summary>
        /// Sum of all term frequencies, i.e. total number of words processed
        /// </summary>
        /// <value>
        /// The term frequency total.
        /// </value>
        public Double termFrequencyTotal { get; set; } = 0;

        /// <summary>
        /// Total number of distinct terms
        /// </summary>
        /// <value>
        /// The term count.
        /// </value>
        public Int32 termCount { get; set; } = 0;

        /// <summary>
        /// Max. weight before normalizatipon
        /// </summary>
        /// <value>
        /// The weight maximum.
        /// </value>
        public Double weightMax { get; set; } = Double.MinValue;

        public wlfConstructorSettings settings { get; set; } = new wlfConstructorSettings();

        /// <summary>
        /// Lemmas sent to weight computation kernel
        /// </summary>
        /// <value>
        /// The lemmas.
        /// </value>
        public List<webLemmaTerm> lemmas { get; set; } = new List<webLemmaTerm>();

        public ILogBuilder loger { get; set; }

        public Boolean forSingleWebSite { get; set; }

        /// <summary>
        /// Prepares weight computation task for lemmas provided
        /// </summary>
        /// <param name="_lemmas">The lemmas.</param>
        public kernelComputeWeightTask(List<webLemmaTerm> _lemmas, ILogBuilder _loger, Boolean _forSingleWebsite, wlfConstructorSettings _settings)
        {
            lemmas = _lemmas;
            loger = _loger;
            forSingleWebSite = _forSingleWebsite;
            settings = _settings;

            foreach (webLemmaTerm lemma in lemmas)
            {
                documentSetFrequencyMax = Math.Max(documentSetFrequencyMax, lemma.documentSetFrequency);
                documentFrequencyMax = Math.Max(documentFrequencyMax, lemma.documentFrequency);
                termFrequencyMax = Math.Max(termFrequencyMax, lemma.termFrequency);
                termFrequencyTotal += lemma.termFrequency;
            }

            termCount = lemmas.Count;
        }
    }
}