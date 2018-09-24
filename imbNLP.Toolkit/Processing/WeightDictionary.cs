using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Processing
{
    /// <summary>
    /// Table of terms with weights
    /// </summary>
    public class WeightDictionary
    {
        public Int32 nDimensions { get; set; } = 1;

        public WeightDictionary()
        {
        }

        /// <summary>
        /// Entries of dictionary
        /// </summary>
        /// <value>
        /// The entries.
        /// </value>
        public List<WeightDictionaryEntry> entries { get; set; } = new List<WeightDictionaryEntry>();

        private Dictionary<String, Double[]> _index { get; set; } = new Dictionary<string, double[]>();

        private void populateIndex()
        {
            foreach (WeightDictionaryEntry entry in entries)
            {
                if (!_index.ContainsKey(entry.name))
                {
                    _index.Add(entry.name, entry.dimensions);
                }
            }
        }

        private Object indexLock = new Object();

        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="score">The score.</param>
        /// <returns></returns>
        public WeightDictionaryEntry AddEntry(String term, Double score)
        {
            return AddEntry(term, new double[] { score });
        }

        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="score">The score.</param>
        /// <returns></returns>
        public WeightDictionaryEntry AddEntry(String term, Double[] score)
        {
            WeightDictionaryEntry en = new WeightDictionaryEntry(term, score);

            if (!entries.Any(x => x.name == en.name))
            {
                entries.Add(en);
            }
            else
            {

                en = entries.First(x => x.name == en.name); //.weight = score;
            }

            en.dimensions = new Double[Math.Max(nDimensions, score.Length)];

            for (int i = 0; i < score.Length; i++)
            {

                en.dimensions[i] = score[i];
            }

            //if (!index.ContainsKey(term))
            //{
            //    index.Add(en.name, en.dimensions);

            //}
            return en;
        }

        public Double GetValue(String term, Int32 dimension = 0)
        {
            if (index.ContainsKey(term))
            {
                return index[term][dimension];
            }
            return 0;
        }

        public Boolean ContainsKey(String term)
        {
            return index.ContainsKey(term);
        }

        /// <summary>
        /// Index of weights vs terms
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        [XmlIgnore]
        public Dictionary<String, Double[]> index
        {
            get
            {
                lock (indexLock)
                {
                    if (_index.Count == 0)
                    {
                        lock (indexLock)
                        {
                            populateIndex();
                        }
                    }
                }

                return _index;
            }
        }
    }
}