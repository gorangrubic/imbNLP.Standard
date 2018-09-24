using imbMiningContext.MCDocumentStructure;
using imbNLP.PartOfSpeech.TFModels.webLemma.table;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace imbNLP.PartOfSpeech.TFModels.streamIndex
{
    public class tokenStreamIndex
    {
        public tokenStreamIndex()
        {
        }

        public webLemmaTermTable streamTable { get; set; }

        [XmlIgnore]
        public List<imbMCStream> streams { get; set; } = new List<imbMCStream>();
    }
}