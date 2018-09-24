using imbNLP.Toolkit.Classifiers.Core;
using imbNLP.Toolkit.Planes.Core;
using imbSCI.Core.reporting;

namespace imbNLP.Toolkit.Planes
{

    /// <summary>
    /// Settings for all vector planes of a NLP experiment
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Planes.Core.IPlaneSettings" />
    public class FeaturePlaneMethodSettings : IPlaneSettings
    {

        public FeaturePlaneMethodSettings() { }

        public void Describe(ILogBuilder logger)
        {

        }
        /// <summary>
        /// Settings for the classification algorithm
        /// </summary>
        /// <value>
        /// The classifier settings.
        /// </value>
        public ClassifierSettings classifierSettings { get; set; } = new ClassifierSettings();


    }

}