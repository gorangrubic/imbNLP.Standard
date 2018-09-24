using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Weighting.Metrics
{
    public class TermDiscriminatingPowerDictionary
    {
        /// <summary>
        /// Name of positive category label
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public String label { get; set; } = "";

        public TermDiscriminatingPower this[String term]
        {
            get
            {
                return terms[term];
            }
        }

        public Dictionary<String, TermDiscriminatingPower> terms { get; set; } = new Dictionary<string, TermDiscriminatingPower>();

        public TermDiscriminatingPowerDictionary(String _label, IEnumerable<String> _terms)
        {
            label = _label;
            foreach (String _term in _terms)
            {
                terms.Add(_term, new TermDiscriminatingPower(_term));
            }
        }

        public TermDiscriminatingPowerDictionary()
        {
        }
    }
}