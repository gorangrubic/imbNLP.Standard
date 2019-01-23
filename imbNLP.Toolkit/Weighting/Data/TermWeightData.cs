using imbNLP.Toolkit.Weighting.Global;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Weighting.Data
{

    public class TermWeightData
    {
        public TermWeightData()
        {

        }

        public List<TermWeightEntry> entries { get; set; } = new List<TermWeightEntry>();



        public Dictionary<String, Double> GetIndexDictionary()
        {
            Dictionary<String, Double> output = new Dictionary<string, double>();
            foreach (var entry in entries)
            {
                if (!output.ContainsKey(entry.term))
                {
                    output.Add(entry.term, entry.weight);
                }
            }
            return output;

        }

        public TermWeightData(Dictionary<String, Double> index)
        {
            foreach (var pair in index)
            {
                entries.Add(new TermWeightEntry(pair.Key, pair.Value));
            }
        }

    }
}