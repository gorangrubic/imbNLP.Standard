using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Weighting.Data
{

    /// <summary>
    /// Serializable weighting model data
    /// </summary>
    public class WeightingModelData
    {
        public WeightingModelData()
        {

        }

        public String signature { get; set; } = "";

        public TermWeightData properties { get; set; } = new TermWeightData();

        public List<TermWeightData> data { get; set; } = new List<TermWeightData>();

    }
}