using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Weighting.Global;
using imbSCI.Core.reporting;
using System;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Corpora
{

    /// <summary>
    /// 
    /// </summary>
    public class FeatureWeightFactor : IDescribe
    {
        public FeatureWeightFactor()
        {

        }

        public void Deploy(ILogBuilder logger)
        {
            GlobalFunction = Settings.GetFunction(logger);
        }

        public void Describe(ILogBuilder logger)
        {
            logger.AppendLine("Factor [" + GlobalFunction.shortName + "] W[" + weight.ToString("F2") + "]");
            logger.nextTabLevel();
            GlobalFunction.Describe(logger);
            logger.prevTabLevel();
        }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        public GlobalFunctionSettings Settings { get; set; } = new GlobalFunctionSettings();

        [XmlIgnore]
        public IGlobalElement GlobalFunction { get; set; }

        public Double weight { get; set; } = 1.0;

    }

}