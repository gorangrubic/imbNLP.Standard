using imbNLP.Toolkit.Weighting.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Weighting.Global
{

    public abstract class WeightingElementBase : IWeightingElementBase
    {
        [XmlIgnore]
        public String shortName { get; set; } = "local";

        public Boolean IsEnabled { get; set; } = true;

        protected Dictionary<String, Double> index { get; set; } = new Dictionary<string, double>();


        protected WeightingModelData SaveModelDataBase()
        {
            WeightingModelData output = new WeightingModelData();
            output.signature = shortName;
            var d = new TermWeightData(index);
            output.data.Add(d);
            return output;
        }


        /// <summary>
        /// Saves model data 
        /// </summary>
        /// <returns></returns>
        public abstract WeightingModelData SaveModelData();

        /// <summary>
        /// Loads pre-trained data into model
        /// </summary>
        /// <param name="data">The data.</param>
        public abstract void LoadModelData(WeightingModelData data);

        protected void LoadModelDataBase(WeightingModelData data)
        {
            if (data.data.Any())
            {
                index = data.data.First().GetIndexDictionary();
            }

        }

    }


}