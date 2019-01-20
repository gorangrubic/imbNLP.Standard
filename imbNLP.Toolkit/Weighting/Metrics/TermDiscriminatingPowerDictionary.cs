using imbSCI.Data.interfaces;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Weighting.Metrics
{
    public class TermDiscriminatingPowerDictionary : IOnBeforeSaveAfterLoaded
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

        [XmlIgnore]
        public List<Double> DistinctValues { get; set; } = new List<double>();

        /// <summary>
        /// Gets or sets the terms.
        /// </summary>
        /// <value>
        /// The terms.
        /// </value>
        protected Dictionary<String, TermDiscriminatingPower> terms { get; set; } = new Dictionary<string, TermDiscriminatingPower>();


        /// <summary>
        /// Used for serialization and deserialization
        /// </summary>
        /// <value>
        /// The serialized data.
        /// </value>
        public List<TermDiscriminatingPower> serializedData { get; set; } = new List<TermDiscriminatingPower>();

        /// <summary>
        /// To be called before saving
        /// </summary>
        public void OnBeforeSave()
        {
            serializedData = new List<TermDiscriminatingPower>();
            serializedData.AddRange(terms.Values);
        }

        /// <summary>
        /// To be called after deserialization
        /// </summary>
        public void OnAfterLoaded()
        {
            terms.Clear();
            foreach (var dp in serializedData)
            {
                terms.Add(dp.term, dp);
            }
        }

        public Boolean ContainsTerm(String term)
        {
            return terms.ContainsKey(term);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TermDiscriminatingPowerDictionary"/> class.
        /// </summary>
        /// <param name="_label">The label.</param>
        /// <param name="_terms">The terms.</param>
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