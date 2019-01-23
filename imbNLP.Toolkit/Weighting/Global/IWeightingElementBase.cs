using imbNLP.Toolkit.Weighting.Data;
using System;

namespace imbNLP.Toolkit.Weighting.Global
{
    public interface IWeightingElementBase
    {
        String shortName { get; set; }

        Boolean IsEnabled { get; set; }

        void LoadModelData(WeightingModelData data);

        WeightingModelData SaveModelData();

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        void Dispose();
    }
}