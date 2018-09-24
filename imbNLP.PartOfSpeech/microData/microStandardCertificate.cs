using imbNLP.PartOfSpeech.microData.core;
using System;

namespace imbNLP.PartOfSpeech.microData
{
    public class microStandardCertificate : microDataBase
    {
        public String issuer { get; set; } = "ISO";

        public String standardName { get; set; } = "ISO 9001:2008";

        public String mainCode { get; set; } = "9001";

        public String secondCode { get; set; } = "2008";
    }
}