using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Processing
{
    /// <summary>
    /// Stores raw frequencies of the tokens
    /// </summary>
    public class TokenDictionary
    {
        protected Dictionary<string, Int32> TokenFrequency { get; set; } = new Dictionary<string, Int32>();
        protected Dictionary<string, Int32> TokenID { get; set; } = new Dictionary<string, Int32>();
        protected List<String> Tokens { get; set; } = new List<string>();

        /// <summary>
        /// Gets the count of distinct tokens in the dictionary
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public Int32 Count { get { return TokenFrequency.Count; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenDictionary"/> class.
        /// </summary>
        public TokenDictionary()
        {
        }

        /// <summary>
        /// Builds dictionary from tokens specified
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        public TokenDictionary(IEnumerable<String> tokens)
        {
            CountTokens(tokens);
        }

        /// <summary>
        /// Gets list of currently known tokens
        /// </summary>
        /// <returns></returns>
        public List<String> GetTokens()
        {
            return Tokens.ToList();
        }

        /// <summary>
        /// Gets ranked list of tokens and their absolute frequencies
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <returns></returns>
        public List<KeyValuePair<String, Int32>> GetRankedTokenFrequency(Int32 limit = -1)
        {
            if (limit == -1) limit = Tokens.Count;

            List<KeyValuePair<String, Int32>> output = new List<KeyValuePair<string, int>>();
            ///List<KeyValuePair<String, Int32>> list = new List<KeyValuePair<string, int>>();

            List<KeyValuePair<string, int>> list = TokenFrequency.OrderByDescending(x => x.Value).ToList();

            output = list.Take(Math.Min(limit, list.Count)).ToList();
            return output;

        }

        /// <summary>
        /// Collects all tokens and sums their frequencies with existing data in the dictionary
        /// </summary>
        /// <param name="input">The input.</param>
        public void MergeDictionary(TokenDictionary input)
        {
            foreach (KeyValuePair<string, int> pair in input.TokenFrequency)
            {
                CountToken(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Returns list of IDs for given token list
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public List<Int32> GetTokenIDs(IEnumerable<String> input)
        {
            List<Int32> output = new List<int>();

            foreach (String tkn in input)
            {
                if (TokenID.ContainsKey(tkn))
                {
                    output.Add(TokenID[tkn]);
                }
            }
            return output;
        }

        public Boolean Contains(String token)
        {
            return TokenID.ContainsKey(token);
        }

        /// <summary>
        /// Counts the tokens.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        public void CountTokens(IEnumerable<String> tokens)
        {
            foreach (String token in tokens)
            {
                CountToken(token);
            }
        }

        /// <summary>
        /// Returns maximum frequency
        /// </summary>
        /// <returns></returns>
        public Int32 GetMaxFrequency()
        {
            return TokenFrequency.Values.Max();
        }

        /// <summary>
        /// Sum of all frequencies
        /// </summary>
        /// <returns></returns>
        public Int32 GetSumFrequency()
        {
            return TokenFrequency.Values.Sum();
        }

        public Double GetSumSquareFrequencies()
        {
            Double fc = 0;
            foreach (Int32 f in TokenFrequency.Values)
            {
                fc += f * f;
            }
            return fc;
        }

        /// <summary>
        /// Returns square root of summation of the sqares of term frequencies
        /// </summary>
        /// <returns></returns>
        public Double GetSquareRootOfSumSquareFrequencies()
        {
            return Math.Sqrt(GetSumSquareFrequencies());
        }

        /// <summary>
        /// Adds oscore point/s to the token frequency. If the token wasn't registered with the dictionary, it will register it automatically.
        /// </summary>
        /// <param name="token">The token - token in the dictionary</param>
        /// <param name="score">The score - points to be added.</param>
        public void CountToken(String token, Int32 score = 1)
        {
            if (!TokenID.ContainsKey(token))
            {
                Tokens.Add(token);
                TokenID[token] = Tokens.Count - 1;
                TokenFrequency[token] = 0;
            }
            TokenFrequency[token] = TokenFrequency[token] + score;
        }

        /// <summary>
        /// Gets frequency of the token
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public Int32 GetTokenFrequency(String token)
        {
            if (TokenFrequency.ContainsKey(token))
            {
                return TokenFrequency[token];
            }
            return 0;
        }

        /// <summary>
        /// Gets the token by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public string GetTokenByID(int id)
        {
            if (id > Count) return null;
            return Tokens[id];
        }

        /// <summary>
        /// Returns ID of the token specified
        /// </summary>
        /// <param name="str">The token</param>
        /// <returns></returns>
        public int GetTokenID(string str)
        {
            if (TokenID.ContainsKey(str))
            {
                return TokenID[str];
            }
            else
            {
                return AddToken(str);
            }
        }

        /// <summary>
        /// Registers new token, returns assigned ID
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        public int AddToken(string str, Int32 frequency = 1)
        {
            if (!TokenID.ContainsKey(str))
            {
                Tokens.Add(str);
                TokenID[str] = Tokens.Count - 1;
                TokenFrequency[str] = frequency;
                return TokenID[str];
            }
            return -1;
        }
    }
}