using imbSCI.Core.files;
using imbSCI.Core.files.folders;
using imbSCI.Core.reporting;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Processing
{
    /// <summary>
    /// Table of terms with weights
    /// </summary>
    public class WeightDictionary
    {
        public void Merge(WeightDictionary dictionary)
        {
            if (nDimensions != dictionary.nDimensions)
            {
                throw new ArgumentOutOfRangeException("Dictionary sent for merge has different number of dimensions. Host dictionary [" + nDimensions + "] - merging with [" + dictionary.nDimensions + "]!", nameof(dictionary));
            }

            foreach (WeightDictionaryEntry entry in dictionary.index.Values)
            {
                if (!index.ContainsKey(entry.name))
                {
                    index.Add(entry.name, entry);
                }
                else
                {
                    for (int i = 0; i < index[entry.name].dimensions.Length; i++)
                    {
                        index[entry.name].dimensions[i] += entry.dimensions[i];
                    }
                }
            }
        }


        public void Merge(WeightDictionaryEntry entry)
        {
            if (!index.ContainsKey(entry.name))
            {
                index.Add(entry.name, entry);
            }
            else
            {
                for (int i = 0; i < index[entry.name].dimensions.Length; i++)
                {
                    index[entry.name].dimensions[i] += entry.dimensions[i];
                }
            }
        }



        /*
        public void Merge(IEnumerable<WeightDictionaryEntry> entries, Double weight = 1.0)
        {
            foreach (WeightDictionaryEntry entry in entries)
            {
                double[] ws = entry.dimensions.ToArray();

                if (weight == 1.0)
                {
                    AddEntry(entry.name, ws, true);
                }
                else
                {
                    List<Double> wsf = new List<double>();
                    foreach (Double w in ws)
                    {
                        wsf.Add(w * weight);
                    }

                    AddEntry(entry.name, wsf.ToArray(), true);
                }
            }
        }
        */

        public String name { get; set; } = "";

        public String description { get; set; } = "";

        /// <summary>
        /// Loads dictionary from specified path
        /// </summary>
        /// <param name="p_m">The p m.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public static WeightDictionary LoadFile(String p_m, ILogBuilder log)
        {
            if (!File.Exists(p_m))
            {
                return null;
            }
            WeightDictionary weightDictionary = objectSerialization.loadObjectFromXML<WeightDictionary>(p_m, log);

            weightDictionary.populateIndex();

            //   weightDictionary.populateIndex();

            if (weightDictionary.name.isNullOrEmpty())
            {
                weightDictionary.name = Path.GetFileNameWithoutExtension(p_m);
            }

            return weightDictionary;
        }

        public void Save(folderNode folder, ILogBuilder log, String outputfilename = "*")
        {
            if (outputfilename == "*") outputfilename = name;

            var p_m = GetDictionaryFilename(outputfilename, folder);

            if (this.name.isNullOrEmpty())
            {
                this.name = Path.GetFileNameWithoutExtension(p_m);
            }

            entries = index.Values.ToList();

            objectSerialization.saveObjectToXML(this, p_m);
            entries.Clear();
        }

        public static String GetDictionaryFilename(String outputfilename, folderNode folder)
        {
            String fn = outputfilename;
            String p_m = folder.pathFor(fn.ensureEndsWith("_wt.xml"), imbSCI.Data.enums.getWritableFileMode.none);
            return p_m;
        }

        public Int32 nDimensions { get; set; } = 1;

        public WeightDictionary()
        {
        }

        public WeightDictionary(String __name, String __description)
        {
            name = __name;
            description = __description;
        }

        /// <summary>
        /// Entries of dictionary
        /// </summary>
        /// <value>
        /// The entries.
        /// </value>
        [XmlArray(ElementName = "entries")]
        [XmlArrayItem(ElementName = "entry")]
        public List<WeightDictionaryEntry> entries
        {
            get { return _entries; }
            set { _entries = value; }
        }

        private Dictionary<String, WeightDictionaryEntry> _index { get; set; } = new Dictionary<string, WeightDictionaryEntry>();

        [XmlIgnore]
        public Int32 Count
        {
            get
            {
                return _index.Count;
            }
        }

        public List<String> GetKeys()
        {
            List<String> output = new List<string>();
            lock (GetEntryAddLock)
            {
                output = index.Keys.ToList();
            }
            return output;
        }

        private void populateIndex()
        {
            _index.Clear();

            foreach (WeightDictionaryEntry entry in entries)
            {
                if (!_index.ContainsKey(entry.name))
                {
                    _index.Add(entry.name, entry);
                }
            }

            entries.Clear();
        }

        private Object indexLock = new Object();

        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="score">The score.</param>
        /// <returns></returns>
        public WeightDictionaryEntry AddEntry(String term, Double score, Boolean sum = false)
        {
            return AddEntry(term, new double[] { score }, sum);
        }

        public WeightDictionaryEntry AddEntry(WeightDictionaryEntry en, Boolean sum = false)
        {
            if (sum == false)
            {
                var entry = _AddEntry(en.name);
                entry.dimensions = en.dimensions;
                return entry;
            }
            else
            {
                return AddEntry(en.name, en.dimensions, true);
            }
        }

        private Object GetEntryAddLock = new Object();
        private List<WeightDictionaryEntry> _entries = new List<WeightDictionaryEntry>();

        private WeightDictionaryEntry _AddEntry(String term)
        {
            WeightDictionaryEntry en = null;

            if (index.ContainsKey(term))
            {
                en = index[term];
            }
            else
            {
                lock (GetEntryAddLock)
                {
                    if (!index.ContainsKey(term))
                    {
                        en = new WeightDictionaryEntry(term, new double[nDimensions]);

                        index.Add(term, en);
                    }
                    else
                    {
                        en = index[term];
                    }
                }
            }
            return en;
        }

        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="score">The score.</param>
        /// <returns></returns>
        public WeightDictionaryEntry AddEntry(String term, Double[] score, Boolean sum = false)
        {
            var en = _AddEntry(term);

            Int32 dimc = Math.Max(en.dimensions.Length, score.Length);

            if (en.dimensions.Length < dimc)
            {
                var tmp = en.dimensions;

                en.dimensions = new Double[dimc];
                tmp.CopyTo(en.dimensions, 0);
            }

            dimc = Math.Min(en.dimensions.Length, score.Length);

            for (int i = 0; i < dimc; i++)
            {
                if (sum)
                {
                    en.dimensions[i] += score[i];
                }
                else
                {
                    en.dimensions[i] = score[i];
                }
            }

            //if (!entries.Any(x => x.name == en.name))
            //{
            //    entries.Add(en);
            //}
            //else
            //{
            //    en = entries.First(x => x.name == en.name); //.weight = score;
            //}

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
                return index[term].dimensions[dimension];
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
        public Dictionary<String, WeightDictionaryEntry> index
        {
            get
            {
                //if (_index.Count == 0)
                //{
                //    lock (indexLock)
                //    {
                //        if (_index.Count == 0)
                //        {
                //            populateIndex();

                //        }
                //    }
                //}
                return _index;
            }
        }
    }
}