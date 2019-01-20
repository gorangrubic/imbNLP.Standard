using System.Collections.Generic;

namespace imbNLP.Toolkit.Weighting.Data
{
    /// <summary>
    /// Trained data of a weighting model
    /// </summary>
    public class WeightingModelDataSet
    {

        public WeightingModelDataSet()
        {

        }

        public List<WeightingModelData> modelData { get; set; } = new List<WeightingModelData>();

        public Dictionary<string, WeightingModelData> GetDataDictionary()
        {
            Dictionary<string, WeightingModelData> output = new Dictionary<string, WeightingModelData>();

            foreach (var m in modelData)
            {
                output.Add(m.signature, m);
            }
            return output;
        }

    }
}