using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Feature
{

    /// <summary>
    /// Collection of labeled feature vectors
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{imbNLP.Toolkit.Feature.FeatureVectorWithLabelID}" />
    public class FeatureVectorWithLabelIDSet : List<FeatureVectorWithLabelID>
    {

        public Dictionary<String, Int32> ToNameVsLabelID()
        {
            Dictionary<String, Int32> output = new Dictionary<string, int>();
            foreach (var vec in this)
            {
                output.Add(vec.name, vec.labelID);
            }
            return output;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [do automatic set unknown labels].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [do automatic set unknown labels]; otherwise, <c>false</c>.
        /// </value>
        public Boolean DoAutoSetUnknownLabels { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureVectorWithLabelIDSet"/> class.
        /// </summary>
        public FeatureVectorWithLabelIDSet()
        {

        }

        public FeatureVectorWithLabelID Add(FeatureVector fv, Int32 _labelID = -1)
        {
            FeatureVectorWithLabelID output = new FeatureVectorWithLabelID();
            output.vector = fv;
            output.labelID = -1;
            Add(output);
            return output;
        }


        /// <summary>
        /// Creates set of labeled feature vectors
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="_name">The name.</param>
        /// <param name="_label">The label.</param>
        public FeatureVectorWithLabelIDSet(IEnumerable<FeatureVectorWithLabelID> input, String _name, Int32 _label)
        {
            foreach (var vec in input)
            {
                Add(vec);
            }
            name = _name;
            labelID = _label;

        }


        /// <summary>
        /// Sets label ID to all contained vectors
        /// </summary>
        /// <param name="_labelID">The label identifier.</param>
        public void CloseDeploy(Int32 _labelID = -1)
        {

            if (DoAutoSetUnknownLabels && _labelID == -1)
            {
                var vectors = this.Select(x => x.vector);
                FeatureVectorSet fv_set = new FeatureVectorSet(vectors);
                _labelID = fv_set.GetDominantClass();
            }

            labelID = _labelID;

            foreach (var fv in this)
            {

                fv.labelID = labelID;
            }

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureVectorWithLabelIDSet"/> class.
        /// </summary>
        /// <param name="vectors">The vectors.</param>
        /// <param name="_labelID">The label identifier.</param>
        public void Deploy(String _name, IEnumerable<FeatureVector> vectors, Int32 _labelID = -1)
        {
            name = _name;
            labelID = _labelID;

            if (vectors != null)
            {
                if (DoAutoSetUnknownLabels && labelID == -1)
                {
                    FeatureVectorSet fv_set = new FeatureVectorSet(vectors);
                    labelID = fv_set.GetDominantClass();
                }

                foreach (var fv in vectors)
                {
                    var fv_id = new FeatureVectorWithLabelID(fv, labelID);
                    Add(fv_id);
                }
            }

        }


        /// <summary>
        /// Label ID associated to the set
        /// </summary>
        /// <value>
        /// The label identifier.
        /// </value>
        public Int32 labelID { get; set; } = -1;

        /// <summary>
        /// Name of the set
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public String name { get; set; } = "";
    }
}