using imbNLP.Toolkit.Space;
using imbSCI.Core.files;
using imbSCI.Core.reporting;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace imbNLP.Toolkit.Feature
{



    /// <summary>
    /// Dictionary of feature vectors
    /// </summary>
    /// <seealso cref="System.Collections.Generic.Dictionary{System.String, imbNLP.Toolkit.Feature.FeatureVector}" />
    public class FeatureVectorDictionary : Dictionary<String, FeatureVector>
    {

        /// <summary>
        /// Gets the vectors with label identifier as dimension value. If vector is not found in this dictionary, it will set labelID to 0, for incorrect it will set 1 and for correct 2
        /// </summary>
        /// <param name="CompleteDataSet">The complete data set.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public FeatureVectorWithLabelIDSet GetVectorsWithLabelID(List<String> CompleteDataSet, Double criteria = 0.5, List<String> labels = null)
        {
            if (CompleteDataSet == null)
            {
                CompleteDataSet = new List<string>();
            }

            labels = SpaceLabel.SetDefaultLabelList(CompleteDataSet.Any(), labels);

            CompleteDataSet.AddRange(this.Select(x => x.Key));

            //if (labels==null)
            //{

            //    labels = new List<string>();
            //    labels.Add(SpaceLabel.UNKNOWN);
            //    labels.Add(SpaceLabel.INCORRECT);
            //    labels.Add(SpaceLabel.CORRECT);
            //}


            Int32 l_unknown = labels.IndexOf(SpaceLabel.UNKNOWN);

            Int32 l_correct = labels.IndexOf(SpaceLabel.CORRECT);
            Int32 l_incorrect = labels.IndexOf(SpaceLabel.INCORRECT);

            var output = new FeatureVectorWithLabelIDSet();
            output.DoAutoSetUnknownLabels = false;

            foreach (String id in CompleteDataSet)
            {
                Int32 l = l_unknown;
                FeatureVectorWithLabelID fv_id = null;

                if (ContainsKey(id))
                {
                    if (this[id].dimensions[0] < criteria)
                    {
                        l = l_incorrect;
                    }
                    else
                    {
                        l = l_correct;
                    }
                    fv_id = new FeatureVectorWithLabelID(this[id], l);
                }
                else
                {
                    if (l_unknown > -1)
                    {
                        fv_id = new FeatureVectorWithLabelID(new FeatureVector(id), l);
                    }
                }
                output.Add(fv_id);
            }
            return output;
        }


        public void SaveVectors(String filepath, ILogBuilder logger)
        {

            List<FeatureVector> vectors = this.Values.ToList();

            objectSerialization.saveObjectToXML(vectors, filepath);
        }

        public void LoadVectors(String filepath, ILogBuilder logger)
        {
            if (filepath.isNullOrEmpty())
            {
                throw new ArgumentNullException("Filepath for FeatureVectorDictionary.LoadVectors is null or empty", nameof(filepath));
            }

            if (!File.Exists(filepath))
            {
                throw new ArgumentNullException("File [" + filepath + "] not found, for FeatureVectorDictionary.LoadVectors is null or empty", nameof(filepath));
            }

            List<FeatureVector> vectors = objectSerialization.loadObjectFromXML<List<FeatureVector>>(filepath, logger);

            if (vectors == null)
            {
                throw new Exception("File [" + filepath + "] found, but deserialization of " + nameof(FeatureVector) + " failed");
            }
            foreach (FeatureVector v in vectors)
            {
                if (v == null)
                {
                    throw new Exception("File [" + filepath + "] loaded - but null vectors found in deserialized list");
                }
                else
                {
                    Add(v);
                }
            }
        }


        public void Add(FeatureVector fv)
        {

            if (ContainsKey(fv.name))
            {
                this[fv.name] = fv;
            }
            else
            {
                Add(fv.name, fv);
            }
        }

    }
}