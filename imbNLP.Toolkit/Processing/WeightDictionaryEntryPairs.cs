using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Processing
{

    /// <summary>
    /// Set of matched features
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{imbNLP.Toolkit.Processing.WeightDictionaryEntryPair}" />
    public class WeightDictionaryEntryPairs : List<WeightDictionaryEntryPair>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeightDictionaryEntryPairs"/> class.
        /// </summary>
        public WeightDictionaryEntryPairs()
        {

        }

        public WeightDictionaryEntryPairs(WeightDictionary vectorA, WeightDictionary vectorB, Int32 vectorADimID = 0, Int32 vectorBDimID = 0)
        {
            foreach (String k in vectorA.GetKeys())
            {
                if (vectorB.ContainsKey(k))
                {
                    Add(new WeightDictionaryEntryPair(k, vectorA.index[k].dimensions[vectorADimID], vectorA.index[k].dimensions[vectorBDimID]));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightDictionaryEntryPairs"/> class.
        /// </summary>
        /// <param name="vectorA">The vector a.</param>
        /// <param name="vectorB">The vector b.</param>
        /// <param name="vectorADimID">The vector a dim identifier.</param>
        /// <param name="vectorBDimID">The vector b dim identifier.</param>
        public WeightDictionaryEntryPairs(IEnumerable<WeightDictionaryEntry> vectorA, IEnumerable<WeightDictionaryEntry> vectorB, Int32 vectorADimID = 0, Int32 vectorBDimID = 0)
        {
            foreach (WeightDictionaryEntry entryA in vectorA)
            {
                WeightDictionaryEntry matchB = vectorB.FirstOrDefault(x => x.name == entryA.name);
                if (matchB != null)
                {
                    Add(new WeightDictionaryEntryPair(entryA.name, entryA.dimensions[vectorADimID], matchB.dimensions[vectorBDimID]));
                }
            }
        }


    }

}