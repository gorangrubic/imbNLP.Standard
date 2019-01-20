using imbNLP.Toolkit.Weighting.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Weighting.Global
{
public interface IWeightingElementBase
    {
        String shortName { get; set; }

        Boolean IsEnabled { get; set; }

        void LoadModelData(WeightingModelData data);

        WeightingModelData SaveModelData();
    }
}