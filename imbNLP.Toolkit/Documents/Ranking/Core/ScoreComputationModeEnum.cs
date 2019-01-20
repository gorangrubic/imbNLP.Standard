using System;

namespace imbNLP.Toolkit.Documents.Ranking.Core
{
    [Flags]
    public enum ScoreComputationModeEnum
    {
        none = 0,
        category = 1,
        site = 2,
        pages = 4,
        offset = 8,
        variance = 16,
        distance = 32,

        inverse = 64,
        pagesOfCategory = 128,
        dataset = 256,
        pagesOfDataset = 512,

        categoryOffset = category | offset,
        categoryVariance = variance | category,
        selfCentric = site | distance,
        selfCentricInverse = site | distance | inverse,

        pageDivergence = pages | variance,
        pageConvergence = pages | variance | inverse,

        pageInCategoryDivergence = pagesOfCategory | variance,
        pageInCategoryConvergence = pagesOfCategory | variance | inverse,




        pageInDatasetDivergence = pagesOfDataset | variance,
        pageInDatasetConvergence = pagesOfDataset | variance | inverse


    }
}