using imbNLP.Toolkit.Documents.Ranking.Core;

namespace imbNLP.Toolkit.Documents.Ranking
{
    public interface IHasProceduralRequirements
    {
        ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null);


    }
}