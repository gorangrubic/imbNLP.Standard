using imbNLP.PartOfSpeech.flags.data;
using imbNLP.PartOfSpeech.microData.core;
using System;

namespace imbNLP.PartOfSpeech.microData
{
    public class microPhoneNumber : microDataBase
    {
        public String areaCode { get; set; } = "";

        public String townCode { get; set; } = "";

        public String number { get; set; } = "";

        public dat_phoneType type { get; set; } = dat_phoneType.officeLine;
    }
}