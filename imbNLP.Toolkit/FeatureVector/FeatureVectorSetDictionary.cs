using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Feature
{
    /// <summary>
    /// Two-level hierarchy dictionary of veature vectors
    /// </summary>
    /// <seealso cref="System.Collections.Generic.Dictionary{System.String, imbNLP.Toolkit.Feature.FeatureVectorWithLabelIDSet}" />
    public class FeatureVectorSetDictionary : Dictionary<String, FeatureVectorWithLabelIDSet>
    {



        public void Add(String _name, FeatureVector vector, Int32 _labelID = -1)
        {
            FeatureVectorWithLabelIDSet lids = GetOrAdd(_name, _labelID);
            lids.Add(vector, _labelID);
        }


        public FeatureVectorWithLabelID Get(String domainID, String assignID)
        {
            if (ContainsKey(domainID))
            {
                return this[domainID].FirstOrDefault(x => x.vector.name == assignID);
            }
            else
            {
                return null;
            }
        }





        public FeatureVectorWithLabelIDSet GetOrAdd(String _name, Int32 _labelID = -1)
        {
            if (ContainsKey(_name))
            {
                return this[_name];
            }
            FeatureVectorWithLabelIDSet output = new FeatureVectorWithLabelIDSet();
            output.Deploy(_name, null, _labelID);
            Add(output.name, output);
            return output;
        }


        /// <summary>
        /// Adds new set or returns existing
        /// </summary>
        /// <param name="_name">The name.</param>
        /// <param name="vectors">The vectors.</param>
        /// <param name="_labelID">The label identifier.</param>
        /// <returns></returns>
        public FeatureVectorWithLabelIDSet Add(String _name, IEnumerable<FeatureVector> vectors, Int32 _labelID = -1)
        {
            if (ContainsKey(_name))
            {
                return this[_name];
            }
            FeatureVectorWithLabelIDSet output = new FeatureVectorWithLabelIDSet();
            output.Deploy(_name, vectors, _labelID);
            Add(output.name, output);
            return output;
        }

    }
}