using imbNLP.PartOfSpeech.TFModels.webLemma.table;
using imbSCI.Core.math;
using System;

namespace imbNLP.PartOfSpeech.TFModels.webLemma.kernels
{
    /// <summary>
    /// Default Term weight kernel, proposed by BEC research
    /// </summary>
    /// <seealso cref="imbNLP.PartOfSpeech.TFModels.webLemma.kernels.ITermWeightKernel" />
    public class kernelTFcIDF : ITermWeightKernel
    {
        public String kernelName { get; set; } = "TF-cIDF";

        /// <summary>
        /// Computes the specified task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        public Boolean compute(kernelComputeWeightTask task)
        {
            task.weightMax = Double.MinValue;

            if (task.lemmas.Count == 0)
            {
                task.loger.log("ERROR: NO ENTRIES IN TF-TDF TABLE - is for single web site [" + task.forSingleWebSite.ToString() + "]");
                return false;
            }

            foreach (webLemmaTerm lemma in task.lemmas)
            {
                lemma.termFrequency = lemma.termFrequency.GetRatio(task.termFrequencyMax);

                if (task.settings.doUseIDF)
                {
                    if (task.settings.doUseNaturalLog)
                    {
                        lemma.documentFactor = Math.Log(task.documentFrequencyMax.GetRatio(lemma.documentFrequency));
                    }
                    else
                    {
                        lemma.documentFactor = Math.Log10(task.documentFrequencyMax.GetRatio(lemma.documentFrequency));
                    }
                }
                else
                {
                    lemma.documentFactor = 1;
                }

                lemma.weight = lemma.termFrequency * lemma.documentFactor;

                if (task.settings.doUseDocumentSet)
                {
                    if ((task.documentSetFrequencyMax != 1) || !task.forSingleWebSite)
                    {
                        if (lemma.documentSetFrequency == 0)
                        {
                            lemma.weight = 0;
                        }
                        else
                        {
                            Double docSetFactor = (1 - Math.Log10(task.documentSetFrequencyMax / lemma.documentSetFrequency));
                            lemma.weight = lemma.weight * docSetFactor;
                        }
                    }
                }

                task.weightMax = Math.Max(task.weightMax, lemma.weight);
            }

            /// WEIGHT NORMALIZATION
            foreach (webLemmaTerm lemma in task.lemmas)
            {
                lemma.weight = lemma.weight.GetRatio(task.weightMax);
            }

            return true;
        }
    }
}