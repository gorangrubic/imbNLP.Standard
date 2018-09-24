using imbNLP.PartOfSpeech.microData.core;
using System;

namespace imbNLP.PartOfSpeech.microData
{
    public class microValueWithUnit : microDataBase
    {
        public String unitOfMeasure { get; set; } = "";

        /// <summary>
        /// Numeric value
        /// </summary>
        /// <value>
        /// The numeric value.
        /// </value>
        public Double numericValue { get; set; } = 0;
    }
}