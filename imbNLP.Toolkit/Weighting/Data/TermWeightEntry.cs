using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace imbNLP.Toolkit.Weighting.Data
{
    public class TermWeightEntry
    {
        public TermWeightEntry(String t, Double w)
        {
            term = t;
            weight = w;
        }

        public TermWeightEntry() { }

        public String term { get; set; } = "";

        public Double weight { get; set; } = 0;

    }
}
