using imbSCI.Core.extensions.typeworks;
using System;
using System.Collections.Generic;

namespace imbNLP.PartOfSpeech.TFModels.similarityKernels
{
    /// <summary>
    /// Helper class that provides term weight computation kernels
    /// </summary>
    public static class kernelManager
    {
        private static Dictionary<String, Type> _registry;

        /// <summary>
        /// static and autoinitiated object
        /// </summary>
        private static Dictionary<String, Type> registry
        {
            get
            {
                if (_registry == null)
                {
                    _registry = new Dictionary<String, Type>();
                    _registry.Add("CSSRM", typeof(kernelCSSRM));
                }
                return _registry;
            }
        }

        /// <summary>
        /// Registers the specified kernel type
        /// </summary>
        /// <param name="kernelType">Type of the kernel.</param>
        /// <exception cref="System.ArgumentException">
        /// Kernel Type must have parameterless constructor! - kernelType
        /// or
        /// Kernel Type must implement ITermWeightKernel interface! - kernelType
        /// </exception>
        public static void RegisterKernel(Type kernelType)
        {
            if (!kernelType.hasParameterlessConstructor())
            {
                throw new ArgumentException("Kernel Type must have parameterless constructor!", nameof(kernelType));
            }

            ISimilarityKernel output = kernelType.getInstance() as ISimilarityKernel;

            if (output == null)
            {
                throw new ArgumentException("Kernel Type must implement ITermWeightKernel interface!", nameof(kernelType));
            }

            if (!registry.ContainsKey(output.kernelName))
            {
                registry.Add(output.kernelName, kernelType);
            }
        }

        /// <summary>
        /// Gets kernel instance for specified kernel name. If kernel not recognized it returns default: <see cref="kernelTFcIDF"/>
        /// </summary>
        /// <param name="kernelName">Name of the kernel.</param>
        /// <returns></returns>
        public static ISimilarityKernel GetKernel(String kernelName)
        {
            if (registry.ContainsKey(kernelName))
            {
                ISimilarityKernel output = registry[kernelName].getInstance() as ISimilarityKernel;
                return output;
            }

            switch (kernelName)
            {
                default:
                    return new kernelCSSRM();
                    break;
            }
        }
    }
}