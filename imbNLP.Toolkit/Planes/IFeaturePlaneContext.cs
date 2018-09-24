using imbNLP.Toolkit.Feature;
using imbNLP.Toolkit.Planes.Core;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Planes
{

    public interface IFeaturePlaneContext : IPlaneContext
    {
        List<FeatureVectorWithLabelID> trainingSet { get; set; }

        List<FeatureVector> testSet { get; set; }

        List<FeatureVectorWithLabelID> testResults { get; set; }

        FeatureSpace featureSpace { get; set; }
    }

}