using System;

namespace imbNLP.Project.Operations.Core
{
    [Flags]
    public enum ProceduralStackOptions
    {
        none = 0,
        clearContextOnStart = 1,
        clearContextOnFinish = 2,
        delayOneSecond = 4,
        delayFiveSeconds = 8,
        skipExistingExperiment = 16,
    }
}