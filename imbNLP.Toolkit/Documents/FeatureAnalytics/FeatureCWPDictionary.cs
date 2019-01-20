using imbNLP.Toolkit.Documents.FeatureAnalytics.Data;
using imbSCI.Core.files.folders;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Documents.FeatureAnalytics
{
    public interface IFeatureCWPDictionary
    {
        FeatureCWPInfoItemBase GetEntry(String key);

        T GetEntry<T>(String key) where T : FeatureCWPInfoItemBase;
    }

    public abstract class FeatureCWPDictionary<T> : Dictionary<String, T>, IFeatureCWPDictionary where T : FeatureCWPInfoItemBase
    {
        public folderNode folder { get; set; }

        public String name { get; set; } = "";
        public String description { get; set; } = "";

        public Double GetScore(String key, Func<T, Double> selector)
        {
            T entry = GetEntry(key);
            if (entry == null) return 0;
            return selector(entry);
        }

        public T GetEntry(String key)
        {
            if (ContainsKey(key)) return this[key];
            return default(T);
        }

        public void AddEntry(T entry)
        {
            Add(entry.term, entry);
        }

        /// <summary>
        /// removes additional information, used for exploratory analysis
        /// </summary>
        public abstract void DisposeExtraInfo();

        FeatureCWPInfoItemBase IFeatureCWPDictionary.GetEntry(string key)
        {
            return GetEntry(key);
        }

        T1 IFeatureCWPDictionary.GetEntry<T1>(string key)
        {
            return GetEntry(key) as T1;
        }
    }
}