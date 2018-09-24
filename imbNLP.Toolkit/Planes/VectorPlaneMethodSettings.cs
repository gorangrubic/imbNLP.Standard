using imbNLP.Toolkit.Feature.Settings;
using imbNLP.Toolkit.Planes.Core;
using imbSCI.Core.reporting;

namespace imbNLP.Toolkit.Planes
{

    public class VectorPlaneMethodSettings : IPlaneSettings
    {
        public VectorPlaneMethodSettings()
        {

        }

        public FeatureVectorConstructorSettings constructor { get; set; } = new FeatureVectorConstructorSettings();

        public void Describe(ILogBuilder logger)
        {
            constructor.Describe(logger);
        }
    }

}