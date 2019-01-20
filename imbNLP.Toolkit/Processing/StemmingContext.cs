using imbNLP.Toolkit.Stemmers;
using imbNLP.Toolkit.Stemmers.Shaman;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Processing
{
    /// <summary>
    /// Provides basic optimization for stemmer by caching results for the words
    /// </summary>
    public class StemmingContext
    {
        /// <summary>
        /// Gets or sets the stemmer.
        /// </summary>
        /// <value>
        /// The stemmer.
        /// </value>
        public IStemmer stemmer { get; set; } = new EnglishStemmer();

        /// <summary>
        /// Gets or sets the word to stem.
        /// </summary>
        /// <value>
        /// The word to stem.
        /// </value>
        public Dictionary<String, String> wordToStem { get; set; } = new Dictionary<string, string>();

        public StemmingContext(IStemmer _stemmer)
        {
            stemmer = _stemmer;
        }

        /// <summary>
        /// Gets the distinct tokens.
        /// </summary>
        /// <returns></returns>
        public List<String> GetDistinctTokens()
        {
            return wordToStem.Keys.ToList();
        }

        /// <summary>
        /// Gets the dictinct stems.
        /// </summary>
        /// <returns></returns>
        public List<String> GetDictinctStems()
        {
            return wordToStem.Values.Distinct().ToList();

        }


        /// <summary>
        /// Returns stems to words dictionary
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, List<String>> GetStemToWords()
        {
            Dictionary<String, List<String>> output = new Dictionary<string, List<string>>();

            foreach (var pair in wordToStem)
            {
                if (!output.ContainsKey(pair.Value)) output.Add(pair.Value, new List<string>());
                output[pair.Value].Add(pair.Key);

            }



            return output;
        }



        private Object StemLock = new Object();


        /// <summary>
        /// Stems the specified token - using stemmer for new tokens or already known stem-word pair
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public String Stem(String token)
        {
            String stem = "";

            if (!wordToStem.ContainsKey(token))
            {
                lock (StemLock)
                {
                    if (!wordToStem.ContainsKey(token))
                    {
                        stemmer.SetCurrent(token);

                        stemmer.Stem();

                        stem = stemmer.GetCurrent();

                        wordToStem.Add(token, stem);
                    }

                }
            }


            if (wordToStem.ContainsKey(token))
            {
                stem = wordToStem[token];
                return stem;
            }



            return stem;
        }
    }
}