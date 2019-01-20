using imbNLP.Toolkit.Documents.Ranking.Data;
using imbSCI.Core.reporting;
using imbSCI.Core.reporting.render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Documents.Ranking.Core
{
    /// <summary>
    /// Model declaring how documents are ranked
    /// </summary>
    [Serializable]
    public class ScoreModel : IHasProceduralRequirements
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScoreModel"/> class.
        /// </summary>
        public ScoreModel()
        {

        }

        /// <summary>
        /// Describes the specified logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public void Describe(ITextRender logger)
        {
            Deploy();


        }

        /// <summary>
        /// Gets the signature.
        /// </summary>
        /// <returns></returns>
        public String GetSignature()
        {
            Deploy();
            string output = "";
            foreach (IScoreModelFactor factor in Factors)
            {
                output += factor.GetType().Name + factor.weight.ToString();
            }
            return output;


        }


        private Object serializedLock = new Object();

        /// <summary>
        /// Gets or sets the serialized factors.
        /// </summary>
        /// <value>
        /// The serialized factors.
        /// </value>
        public List<ScoreModelFactorSerialized> SerializedFactors
        {
            get
            {

                if (!_serializedFactors.Any())
                {
                    lock (serializedLock)
                    {
                        if (!_serializedFactors.Any())
                        {
                            if (_Factors.Any())
                            {
                                foreach (IScoreModelFactor fact in _Factors)
                                {

                                    _serializedFactors.Add(new ScoreModelFactorSerialized(fact));
                                }
                            }
                        }
                    }
                }
                return _serializedFactors;
            }
            set { _serializedFactors = value; }
        }

        public Boolean UtilizesSelectedFeatures { get; set; } = false;


        /// <summary>
        /// Deploys settings into new instances of factors. This removes any previously stored knowledge in the factors.
        /// </summary>
        public void Deploy()
        {


            if (_Factors.Count == 0)
            {
                _Factors = new List<IScoreModelFactor>();
                foreach (ScoreModelFactorSerialized serialized in _serializedFactors)
                {
                    _Factors.Add(serialized.GetInstance());
                }
            }

        }



        /// <summary>
        /// Queries factors for preprocessing requirements
        /// </summary>
        /// <param name="requirements">The requirements.</param>
        /// <returns></returns>
        public ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null)
        {
            if (requirements == null) requirements = new ScoreModelRequirements();

            foreach (IScoreModelFactor factor in Factors)
            {
                factor.CheckRequirements(requirements);

            }

            return requirements;
        }

        /// <summary>
        /// Prepares the factors for score operation 
        /// </summary>
        /// <param name="context">The context.</param>
        public void Prepare(DocumentSelectResult context, ILogBuilder log)
        {
            foreach (IScoreModelFactor factor in Factors)
            {

                factor.Prepare(context, log);
            }
        }



        private List<IScoreModelFactor> _Factors = new List<IScoreModelFactor>();
        private List<ScoreModelFactorSerialized> _serializedFactors = new List<ScoreModelFactorSerialized>();

        /// <summary>
        /// Gets the factors.
        /// </summary>
        /// <value>
        /// The factors.
        /// </value>
        [XmlIgnore]
        public List<IScoreModelFactor> Factors
        {
            get
            {
                return _Factors;
            }

        }

    }
}