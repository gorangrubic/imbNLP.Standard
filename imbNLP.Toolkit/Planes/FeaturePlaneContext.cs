using imbNLP.Toolkit.Feature;
using imbNLP.Toolkit.Planes.Core;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Planes
{

    /// <summary>
    /// Feature Plane context
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Planes.IFeaturePlaneContext" />
    /// <seealso cref="imbNLP.Toolkit.Planes.Core.IPlaneContext" />
    public class FeaturePlaneContext : PlaneContextBase, IFeaturePlaneContext, IPlaneContext
    {

        public List<FeatureVectorWithLabelID> trainingSet { get; set; } = new List<FeatureVectorWithLabelID>();

        public List<FeatureVector> testSet { get; set; } = new List<FeatureVector>();

        public List<FeatureVectorWithLabelID> testResults { get; set; } = new List<FeatureVectorWithLabelID>();

        public FeatureSpace featureSpace { get; set; } = new FeatureSpace();

        public FeaturePlaneContext()
        {

        }
    }

}