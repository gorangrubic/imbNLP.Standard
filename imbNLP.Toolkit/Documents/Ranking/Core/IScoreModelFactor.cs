using imbNLP.Toolkit.Documents.Ranking.Data;
using imbSCI.Core.reporting;
using System;

namespace imbNLP.Toolkit.Documents.Ranking.Core
{
    public interface IScoreModelFactor : IHasProceduralRequirements
    {
        Double weight { get; set; }

        Boolean doNormalize { get; set; }

        Double Score(DocumentSelectResultEntry entry, DocumentSelectResult context, ILogBuilder log);

        void Prepare(DocumentSelectResult context, ILogBuilder log);

        ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null);

        //void Deploy();
    }
}