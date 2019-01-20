using imbSCI.Core.extensions.data;
using imbSCI.Core.files;
using imbSCI.Data.interfaces;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Weighting.Metrics
{


    public class TermDiscriminatingPowerModel : IOnBeforeSaveAfterLoaded
    {
        public TermDiscriminatingPowerModel()
        {
        }

        /// <summary>
        /// Creates precomputed model
        /// </summary>
        /// <param name="factor">Computation factor</param>
        /// <param name="N">Number of documents.</param>
        /// <returns></returns>
        public TermDiscriminatingPowerComputedModel GetComputedModel(TDPFactor factor, Double N)
        {
            TermDiscriminatingPowerComputedModel output = new TermDiscriminatingPowerComputedModel(dictionaries.Keys, factor);



            foreach (KeyValuePair<string, TermDiscriminatingPowerDictionary> pair in dictionaries)
            {
                pair.Value.OnBeforeSave();

                foreach (var termPair in pair.Value.serializedData)
                {
                    Double v = termPair.Compute(factor, N);
                    output.index[pair.Key].Add(termPair.term, v);

                    if (!pair.Value.DistinctValues.Contains(v))
                    {
                        pair.Value.DistinctValues.Add(v);
                    }
                }

                output.DistinctValues.AddRange(pair.Value.DistinctValues, true);
            }

            return output;
        }


        public void PrepareBlank(IEnumerable<String> labels, IEnumerable<String> terms)
        {

            foreach (String label in labels)
            {
                var dict = new TermDiscriminatingPowerDictionary(label, terms);
                dictionaries.Add(label, dict);
            }

        }

        public void OnBeforeSave()
        {
            serializedData = new List<TermDiscriminatingPowerDictionary>();
            serializedData.AddRange(dictionaries.Values);

            foreach (var d in serializedData)
            {
                d.OnBeforeSave();

            }
        }

        public void OnAfterLoaded()
        {
            foreach (var d in serializedData)
            {
                d.OnAfterLoaded();
                dictionaries.Add(d.label, d);
            }
        }


        public String SaveToXML()
        {
            OnBeforeSave();
            return objectSerialization.ObjectToXML(this);
        }

        public static TermDiscriminatingPowerModel LoadFromXML(string XML)
        {
            var output = objectSerialization.ObjectFromXML<TermDiscriminatingPowerModel>(XML);
            output.OnAfterLoaded();
            return output;
        }

        /// <summary>
        /// Loads from path
        /// </summary>
        /// <param name="path">File path of saved XML</param>
        /// <returns></returns>
        public static TermDiscriminatingPowerModel LoadFrom(string path)
        {
            var output = objectSerialization.loadObjectFromXML<TermDiscriminatingPowerModel>(path);
            output.OnAfterLoaded();
            return output;
        }

        /// <summary>
        /// Saves the model to the path
        /// </summary>
        /// <param name="path">The path.</param>
        public void SaveAs(string path)
        {
            OnBeforeSave();

            objectSerialization.saveObjectToXML(this, path);
        }

        public List<TermDiscriminatingPowerDictionary> serializedData { get; set; } = new List<TermDiscriminatingPowerDictionary>();

        /// <summary>
        /// Gets or sets the dictionaries.
        /// </summary>
        /// <value>
        /// The dictionaries.
        /// </value>
        [XmlIgnore]
        protected Dictionary<String, TermDiscriminatingPowerDictionary> dictionaries { get; set; } = new Dictionary<string, TermDiscriminatingPowerDictionary>();


        /// <summary>
        /// Gets the <see cref="TermDiscriminatingPowerDictionary"/> with the specified label name.
        /// </summary>
        /// <value>
        /// The <see cref="TermDiscriminatingPowerDictionary"/>.
        /// </value>
        /// <param name="labelName">Name of the label.</param>
        /// <returns></returns>
        public TermDiscriminatingPowerDictionary this[String labelName]
        {
            get
            {
                return dictionaries[labelName];
            }
        }
    }
}