using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Weighting.Metrics
{
    public class TermDiscriminatingPowerModel
    {
        public TermDiscriminatingPowerModel()
        {
        }

        public Dictionary<String, TermDiscriminatingPowerDictionary> dictionaries { get; set; } = new Dictionary<string, TermDiscriminatingPowerDictionary>();

        public TermDiscriminatingPowerDictionary this[String labelName]
        {
            get
            {
                return dictionaries[labelName];
            }
        }
    }
}