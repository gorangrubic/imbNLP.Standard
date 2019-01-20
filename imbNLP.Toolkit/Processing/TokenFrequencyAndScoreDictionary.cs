using imbSCI.Core.math;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Processing
{
    /// <summary>
    /// Utility dictionary, used for projection weight construction
    /// </summary>
    public class TokenFrequencyAndScoreDictionary
    {

        /// <summary>
        /// Constructs the weight dictionary according to stored frequency and score information
        /// </summary>
        /// <returns></returns>
        public WeightDictionary ConstructWeightDictionary()
        {
            WeightDictionary output = new WeightDictionary();

            var tkns = frequencyIndex.GetTokens();

            Int32 maxFrequency = frequencyIndex.GetMaxFrequency();
            Double maxWeight = scoreIndex.Values.Max();
            Int32 maxDF = documentIndex.GetMaxFrequency();

            foreach (String token in tkns)
            {

                Double finalWeight = scoreIndex[token].GetRatio(maxWeight); //.GetRatio(frequencyIndex.GetTokenFrequency(token));
                Double TF = frequencyIndex.GetTokenFrequency(token).GetRatio(maxFrequency);
                Double IDF = Math.Log(maxDF / documentIndex.GetTokenFrequency(token)) + 1; // Math.Log(1 - ( / maxDF));

                finalWeight = finalWeight * (TF * IDF);

                output.AddEntry(token, finalWeight);

            }


            return output;

        }

        /// <summary>
        /// Adds the specified tokens.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="score">The score.</param>
        public void Add(TokenDictionary tokens, Double score)
        {
            if (tokens.Count == 0) return;

            foreach (String token in tokens.GetTokens())
            {
                Add(token, score, tokens.GetTokenFrequency(token));
            }
            //foreach ()
        }

        /// <summary>
        /// Adds the specified token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="score">Associated score</param>
        /// <param name="frequency">Token count</param>
        public void Add(String token, Double score, Int32 frequency = 1)
        {
            frequencyIndex.AddToken(token, frequency);
            documentIndex.AddToken(token, 1);

            if (!scoreIndex.ContainsKey(token))
            {
                scoreIndex.Add(token, 0);
            }

            scoreIndex[token] += score;

        }


        protected TokenDictionary frequencyIndex { get; set; } = new TokenDictionary();

        protected TokenDictionary documentIndex { get; set; } = new TokenDictionary();

        protected Dictionary<String, Double> scoreIndex { get; set; } = new Dictionary<string, double>();

    }
}