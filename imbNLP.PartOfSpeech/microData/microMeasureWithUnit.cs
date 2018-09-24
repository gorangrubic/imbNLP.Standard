using imbNLP.PartOfSpeech.flags.data;
using imbNLP.PartOfSpeech.microData.core;
using System.Collections.Generic;

namespace imbNLP.PartOfSpeech.microData
{
    /// <summary>
    /// Measure with unit
    /// </summary>
    /// <seealso cref="imbNLP.PartOfSpeech.microData.core.microDataBase" />
    public class microMeasureWithUnit : microDataBase
    {
        public List<microValueWithUnit> valuesWithUnit { get; set; } = new List<microValueWithUnit>();

        public dat_measure measure { get; set; } = dat_measure.length;

        public dat_datapoint datapointType { get; set; } = dat_datapoint.value1D;
    }
}