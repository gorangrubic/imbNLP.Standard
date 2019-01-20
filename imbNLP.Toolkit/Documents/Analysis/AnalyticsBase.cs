using imbSCI.Core.files.folders;
using imbSCI.Core.reporting;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Documents.Analysis
{

    public abstract class AnalyticsBase
    {
        [XmlIgnore]
        public folderNode folder { get; set; }
        [XmlIgnore]
        public ILogBuilder log { get; set; }
    }

}