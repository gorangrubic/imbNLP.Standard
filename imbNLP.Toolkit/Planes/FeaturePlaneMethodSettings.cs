using imbNLP.Toolkit.Classifiers.Core;
using imbNLP.Toolkit.Feature.Settings;
using imbSCI.Core.reporting;
using imbSCI.Data;
using System;

namespace imbNLP.Toolkit.Planes
{

    /// <summary>
    /// Settings for all vector planes of a NLP experiment
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Planes.Core.IPlaneSettings" />
    public class FeaturePlaneMethodSettings : PlaneSettingsBase
    {
        private FeatureVectorConstructorSettings _constructor = new FeatureVectorConstructorSettings();
        private ClassifierSettings _classifierSettings = new ClassifierSettings();

        public FeaturePlaneMethodSettings() { }

        public override void Describe(ILogBuilder logger)
        {
            constructor.Describe(logger);

            if (!ExportClassifierMemory.isNullOrEmpty())
            {
                logger.AppendLine("Export memory to: " + ExportClassifierMemory);
            }

            if (classifierSettings != null)
            {
                
            }
        }


        /// <summary>
        /// Filename for classifier memory, to be exported after training
        /// </summary>
        /// <value>
        /// The export classifier memory.
        /// </value>
        public String ExportClassifierMemory { get; set; } = "";


        // public Boolean ImportMemoryIfFound { get; set; } = false;

        /// <summary>
        /// Filename for classifier memory, to be imported, instead of training
        /// </summary>
        /// <value>
        /// The import classifier memory.
        /// </value>
        public String ImportClassifierMemory { get; set; } = "";


        public FeatureVectorConstructorSettings constructor
        {
            get { return _constructor; }
            set { _constructor = value; }
        }


        /// <summary>
        /// Settings for the classification algorithm
        /// </summary>
        /// <value>
        /// The classifier settings.
        /// </value>
        public ClassifierSettings classifierSettings
        {
            get { return _classifierSettings; }
            set { _classifierSettings = value; }
        }


    }

}