using imbSCI.Data.data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Processing
{

    /// <summary>
    /// Stores raw frequencies of the tokens
    /// </summary>
    [Serializable]
    public class TokenDictionary : changeBindableBase
    {
        protected Dictionary<string, Int32> TokenFrequency { get; set; } = new Dictionary<string, Int32>();
        protected Dictionary<string, Int32> TokenID { get; set; } = new Dictionary<string, Int32>();
        protected List<String> Tokens { get; set; } = new List<string>();


        /// <summary>
        /// Converts to frequency dictionary
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, Double> ToFrequencyDictionary()
        {
            Dictionary<String, Double> output = new Dictionary<string, double>();

            foreach (var pair in TokenFrequency)
            {
                output.Add(pair.Key, (Double)pair.Value);
            }
            return output;
        }


        /// <summary>
        /// Gets the count of distinct tokens in the dictionary
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public Int32 Count { get { return TokenFrequency.Count; } }

        /// <summary>
        /// Builds dictionary from tokens specified
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        public TokenDictionary(IEnumerable<String> tokens)
        {
            CountTokens(tokens);
        }




        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public TokenDictionary Clone()
        {
            TokenDictionary output = new TokenDictionary();
            foreach (String token in Tokens)
            {
                output.TokenFrequency.Add(token, TokenFrequency[token]);
                output.TokenID.Add(token, TokenID[token]);
                output.Tokens.Add(token);
                output.InvokeChanged();
            }
            return output;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenDictionary"/> class.
        /// </summary>
        public TokenDictionary()
        {

        }


        protected void UpdateIfChanged()
        {
            if (HasChanges)
            {
                MaxFrequency = Int32.MinValue;
                SumFrequency = 0;
                SumSquareFrequency = 0;
                SquareRootOfSumSquareFrequency = 0;


                for (int i = 0; i < Count; i++)
                {
                    String tkn = Tokens[i];
                    Int32 f = TokenFrequency[tkn];

                    if (f > MaxFrequency)
                    {
                        MaxFrequency = f;
                    }

                    SumFrequency += f;
                    SumSquareFrequency += f * f;

                }

                SquareRootOfSumSquareFrequency = Math.Sqrt(SumSquareFrequency);
                Accept();
            }
        }


        #region DICTIONARY STATS


        protected Int32 MaxFrequency { get; set; } = 0;
        protected Int32 SumFrequency { get; set; } = 0;
        protected Double SumSquareFrequency { get; set; } = 0;
        protected Double SquareRootOfSumSquareFrequency { get; set; } = 0;

        /// <summary>
        /// Returns maximum frequency
        /// </summary>
        /// <returns></returns>
        public Int32 GetMaxFrequency()
        {
            if (Count == 0) return 0;
            UpdateIfChanged();
            return MaxFrequency;
        }

        /// <summary>
        /// Sum of all frequencies
        /// </summary>
        /// <returns></returns>
        public Int32 GetSumFrequency()
        {
            if (Count == 0) return 0;
            UpdateIfChanged();
            return SumFrequency;
        }

        public Double GetSumSquareFrequencies()
        {
            if (Count == 0) return 0;
            UpdateIfChanged();

            return SumSquareFrequency;
        }

        /// <summary>
        /// Returns square root of summation of the sqares of term frequencies
        /// </summary>
        /// <returns></returns>
        public Double GetSquareRootOfSumSquareFrequencies()
        {
            if (Count == 0) return 0;
            UpdateIfChanged();
            return SquareRootOfSumSquareFrequency;
        }


        #endregion




        public void Clear()
        {
            TokenID.Clear();
            TokenFrequency.Clear();
            Tokens.Clear();
            InvokeChanged();
        }

        /// <summary>
        /// Returns cross-section tokens between the specified list and dictionary
        /// </summary>
        /// <param name="toMatch">To match.</param>
        /// <param name="removeFromToMatch">if set to <c>true</c> it will remove tokens that were matched from specified <c>toMatch</c> list.</param>
        /// <returns></returns>
        public List<String> GetTokens(List<String> toMatch, Boolean removeFromToMatch = true)
        {
            List<String> output = new List<string>();

            for (int i = 0; i < toMatch.Count; i++)
            {
                if (TokenFrequency.ContainsKey(toMatch[i]))
                {
                    output.Add(toMatch[i]);
                }
            }

            if (removeFromToMatch) toMatch.RemoveAll(x => output.Contains(x));


            return output;
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
        /// Adds oscore point/s to the token frequency. If the token wasn't registered with the dictionary, it will register it automatically.
        /// </summary>
        /// <param name="token">The token - token in the dictionary</param>
        /// <param name="score">The score - points to be added.</param>
        public void CountToken(String token, Int32 score = 1)
        {
            if (!TokenID.ContainsKey(token))
            {
                Tokens.Add(token);
                TokenID[token] = TokenID.Count;
                TokenFrequency[token] = 0;
            }
            TokenFrequency[token] = TokenFrequency[token] + score;

            InvokeChanged();
        }

        /// <summary>
        /// Gets frequency of the token
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public Int32 GetTokenFrequencies(IEnumerable<String> tokens)
        {
            if (Count == 0) return 0;
            Int32 output = 0;
            foreach (var tkn in tokens)
            {
                output += GetTokenFrequency(tkn);
            }

            return output;
        }

        /// <summary>
        /// Gets frequency of the token
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public Int32 GetTokenFrequency(String token)
        {
            if (Count == 0) return 0;
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
        /// Gets the token by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public List<string> GetTokenByIDs(IEnumerable<int> ids)
        {
            List<String> output = new List<string>();

            foreach (Int32 i in ids)
            {
                if (Tokens.Count > i)
                {
                    output.Add(Tokens[i]);
                }
            }

            return output;
        }


        /// <summary>
        /// Gets the token by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public List<Int32> GetIDsByTokens(IEnumerable<String> tkns)
        {
            List<Int32> output = new List<Int32>();

            foreach (String i in tkns)
            {
                if (TokenID.ContainsKey(i))
                {
                    output.Add(TokenID[i]);
                }
            }

            return output;
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
                InvokeChanged();
                return TokenID[str];
            }
            return -1;
        }

        public Boolean RemoveToken(String token)
        {
            Boolean output = TokenFrequency.Remove(token);
            output = output && TokenID.Remove(token);
            output = output && Tokens.Remove(token);
            InvokeChanged();
            return output;
        }

        public List<String> GetTokensOtherThan(List<String> notToGet)
        {
            List<String> toRemove = new List<string>();

            var tkns = GetTokens();
            for (int i = 0; i < notToGet.Count; i++)
            {
                tkns.Remove(notToGet[i]);
            }

            return tkns;

        }


        /// <summary>
        /// Removes specified tokens from the dictionary
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="inverseFilter">if set to <c>true</c> [inverse filter].</param>
        /// <returns></returns>
        public Int32 FilterTokens(List<String> tokens, Boolean inverseFilter = true)
        {
            Int32 c = 0;
            if (!inverseFilter)
            {
                Tokens.RemoveAll(x => tokens.Contains(x));


                for (int i = 0; i < tokens.Count; i++)
                {
                    TokenFrequency.Remove(tokens[i]);
                    TokenID.Remove(tokens[i]);
                    c++;
                    
                }
                InvokeChanged();
                return c;
            }

            List<String> toRemove = new List<string>();


            if (inverseFilter)
            {
                var tkns = GetTokens();
                tokens.ForEach(x => tkns.Remove(x));
                toRemove.AddRange(tkns);
            }
            else
            {
                toRemove.AddRange(tokens);
            }

            foreach (String tkn in toRemove)
            {
                if (RemoveToken(tkn))
                {
                    c++;
                }
            }

            InvokeChanged();

            return c;
        }
    }
}