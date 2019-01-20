using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Vectors;
using System;

namespace imbNLP.Toolkit.Feature
{

    public class FeatureSpaceDimensionTerm : FeatureSpaceDimensionBase
    {


        public String term { get; set; }

        public FeatureSpaceDimensionTerm(String _term)
        {
            term = _term;
        }

        public override double ComputeDimension(WeightDictionary vector, Int32 d = 0)
        {
            var entry = vector.GetValue(term, d);
            return entry;
        }

        public override double ComputeDimension(IVector vector, Int32 d = 0)
        {
            var entry = vector.terms.GetValue(term, d);
            return entry;
        }
    }

}