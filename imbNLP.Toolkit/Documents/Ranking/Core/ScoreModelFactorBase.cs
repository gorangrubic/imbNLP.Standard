using imbNLP.Toolkit.Documents.Ranking.Data;
using imbSCI.Core.reporting;
using System;

namespace imbNLP.Toolkit.Documents.Ranking.Core
{

    public abstract class ScoreModelFactorBase : IScoreModelFactor
    {

        public abstract String name { get; set; }

        public Double weight { get; set; } = 1.0;


        public abstract String GetSignature();

        protected String GetWeightSignature()
        {
            if (weight != 1) { return weight.ToString("F2"); }
            return "";
        }


        /// <summary>
        /// If set true it will normalize scores given by this factor - dividing scores by maximum score
        /// </summary>
        /// <value>
        ///   <c>true</c> if [do normalize]; otherwise, <c>false</c>.
        /// </value>
        public Boolean doNormalize { get; set; } = true;

        public abstract Double Score(DocumentSelectResultEntry entry, DocumentSelectResult context, ILogBuilder log);

        public abstract void Prepare(DocumentSelectResult context, ILogBuilder log);

        public abstract ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null);


        // public abstract void Deploy();


    }
}