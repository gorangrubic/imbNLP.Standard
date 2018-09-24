using System;

namespace imbNLP.PartOfSpeech.TFModels.similarityKernels
{
    public interface ISimilarityKernel
    {
        /// <summary>
        /// Gets the name of the kernel.
        /// </summary>
        /// <value>
        /// The name of the kernel.
        /// </value>
        String kernelName { get; }
    }
}