using System.Xml.Serialization;

namespace imbNLP.Toolkit.Planes.Core
{

    public abstract class PlaneContextBase : IPlaneContext
    {
        [XmlIgnore]
        public PlaneContextProvider provider { get; set; } = new PlaneContextProvider();
    }

}