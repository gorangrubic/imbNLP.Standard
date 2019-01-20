using imbSCI.Core.extensions.data;
using imbSCI.Core.files;
using System;

namespace imbNLP.Toolkit.Documents.Ranking.Core
{
    /// <summary>
    /// Serialized entry for score model factor
    /// </summary>
    [Serializable]
    public class ScoreModelFactorSerialized
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScoreModelFactorSerialized" /> class.
        /// </summary>
        public ScoreModelFactorSerialized()
        {

        }

        public ScoreModelFactorSerialized(IScoreModelFactor factor)
        {
            SetInstance(factor);
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns></returns>
        public IScoreModelFactor GetInstance()
        {
            if (!SerializedInstance.isNullOrEmpty())
            {
                Type output = imbNLP.Toolkit.Typology.TypeProviders.ScoreModelFactorProvider.GetTypeByName(FactorClassName);
                if (output == null)
                {
                    throw new Exception("Factor class name not found [" + FactorClassName + "]");
                }
                return objectSerialization.ObjectFromXML<IScoreModelFactor>(SerializedInstance, output);
            }
            else
            {
                return Typology.TypeProviders.ScoreModelFactorProvider.GetInstance(FactorClassName);
            }
        }

        /// <summary>
        /// Sets the instance.
        /// </summary>
        /// <param name="factor">The factor.</param>
        public void SetInstance(IScoreModelFactor factor)
        {
            Type t = factor.GetType();
            FactorClassName = t.Name;
            SerializedInstance = objectSerialization.ObjectToXML(factor, t);

        }

        /// <summary>
        /// Gets or sets the name of the factor class inheriting <see cref="ScoreModelFactorBase"/>
        /// </summary>
        /// <value>
        /// The name of the factor class.
        /// </value>
        public String FactorClassName { get; set; } = "";


        /// <summary>
        /// Serialized version of the factor instance
        /// </summary>
        /// <value>
        /// The serialized instance.
        /// </value>
        public String SerializedInstance { get; set; } = "";

    }
}