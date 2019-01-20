using imbNLP.Toolkit.Processing;
using imbSCI.DataComplex.special;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Feature
{
    public class FeatureVectorSet : List<FeatureVector>
    {

        public FeatureVectorSet(IEnumerable<FeatureVector> vectors)
        {
            foreach (var fv in vectors)
            {
                Add(fv);
            }
        }

        public Int32 GetDominantClass()
        {
            instanceCountCollection<Int32> counter = new instanceCountCollection<int>();

            foreach (FeatureVector fv in this)
            {
                Int32 dd = fv.GetDominantDimension();
                if (dd > -1)
                {
                    counter.AddInstance(dd);
                }

            }

            if (counter.Count == 0) return -1;

            return counter.getSorted(1).First();

        }

    }
}