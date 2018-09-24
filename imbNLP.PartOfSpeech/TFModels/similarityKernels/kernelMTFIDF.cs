using System;

namespace imbNLP.PartOfSpeech.TFModels.similarityKernels
{
    public class kernelMTFIDF : ISimilarityKernel
    {
        public String kernelName { get; set; } = "mTFmIDF";
    }
}