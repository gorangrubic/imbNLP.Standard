using imbSCI.Core.files;
using imbSCI.Data.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Weighting.Metrics
{
public class TermDiscriminatingPowerComputedModel
    {

        public TermDiscriminatingPowerComputedModel()
        {

        }

        public TermDiscriminatingPowerComputedModel(IEnumerable<String> _labels, TDPFactor _factor)
        {
            Deploy(_labels, _factor);
        }

        public void Deploy(IEnumerable<String> _labels, TDPFactor _factor)
        {
            factor = _factor;
            labels = _labels.ToList();
            foreach (String label in labels)
            {
                index.Add(label, new Dictionary<string, double>());
            }
        }


        [XmlIgnore]
        public List<Double> DistinctValues { get; set; } = new List<double>();

        public TDPFactor factor { get; set; } = TDPFactor.gr;

        public List<String> labels { get; set; } = new List<string>();


        public Dictionary<String, Dictionary<String, Double>> index { get; set; } = new Dictionary<string, Dictionary<string, double>>();
    }
}