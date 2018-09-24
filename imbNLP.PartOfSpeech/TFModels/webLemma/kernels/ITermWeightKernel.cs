using System;

namespace imbNLP.PartOfSpeech.TFModels.webLemma.kernels
{
    public interface ITermWeightKernel
    {
        /// <summary>
        /// Gets the name of the kernel.
        /// </summary>
        /// <value>
        /// The name of the kernel.
        /// </value>
        String kernelName { get; }

        /// <summary>
        /// Compute and sets weight to all lemmas in the <see cref="kernelComputeWeightTask.lemmas"/>
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        Boolean compute(kernelComputeWeightTask task);
    }
}