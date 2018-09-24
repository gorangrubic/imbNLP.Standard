using System;

namespace imbNLP.PartOfSpeech.TFModels.semanticCloud
{
    /// <summary>
    /// Flags controling term expansion
    /// </summary>
    [Flags]
    public enum lemmaExpansionOptions
    {
        none = 0,
        weightAsSemanticDistanceFromParent = 1,
        initialWeightToOne = 2,
        initialWeightFromParent = 4,

        divideWeightByNumberOfSynonims = 8,
        weightAsSemanticDistanceThatIsSumOfLinkWeights = 16
    }
}