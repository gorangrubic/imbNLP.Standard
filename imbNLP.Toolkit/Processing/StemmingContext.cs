using imbNLP.Toolkit.Stemmers;
using imbNLP.Toolkit.Stemmers.Shaman;
using System;
using System.Collections.Generic;

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


        private Object StemLock = new Object();


        /// <summary>
        /// Stems the specified token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public String Stem(String token)
        {
            lock (StemLock)
            {
                if (wordToStem.ContainsKey(token))
                {
                    return wordToStem[token];
                }

                stemmer.SetCurrent(token);

                stemmer.Stem();

                String stem = stemmer.GetCurrent();

                wordToStem.Add(token, stem);

                return stem;
            }
        }
    }
}