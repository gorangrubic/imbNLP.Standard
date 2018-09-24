using System;

namespace imbNLP.PartOfSpeech.flags.data
{
    [Flags]
    public enum dat_datapoint
    {
        none = 0,
        value1D = 1,
        value2D = 2,
        value3D = 3,
        rangeValue1DUpTo = rangeValue | upTo | value1D,
        rangeValue1DFromTo = rangeValue | fromTo | value1D,
        rangeValue = 1 << 40,
        upTo = 1 << 50,
        fromTo = 1 << 60,
    }
}