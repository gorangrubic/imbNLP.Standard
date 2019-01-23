using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace imbNLP.Toolkit.Processing
{

    /// <summary>
    /// Common base for different tokenizers.
    /// </summary>
    public abstract class TokenizerBase : ITokenizer
    {
        /// <summary>
        /// Tokenizes the specified text, according to the configuration of the tokenizer
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public virtual string[] Tokenize(string text)
        {
            List<String> output = new List<string>();
            if (LowerCase) text = text.ToLower();

            text = ExecuteInputReplacers(text);

            var mchs = tokenSelector.Matches(text);

            foreach (Match m in mchs)
            {
                if (m.Length >= MinLength) output.Add(m.Value);
            }

            //var output = text.Split(TokenSplitterChars, StringSplitOptions.RemoveEmptyEntries);

            return output.ToArray();
        }

        public Boolean LowerCase { get; set; } = true;

        public Int32 MinLength { get; set; } = 3;

        /// <summary>
        /// Set of replacement rules to be applied on a text, before splitting the text into tokens
        /// </summary>
        /// <value>
        /// The input replacers.
        /// </value>
        public List<TokenizeReplaceRule> InputReplacers { get; set; } = new List<TokenizeReplaceRule>();

        /// <summary>
        /// Executes all replacement rules from the <see cref="InputReplacers"/> collection
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        protected string ExecuteInputReplacers(string text)
        {
            for (int i = 0; i < InputReplacers.Count; i++)
            {
                text = InputReplacers[i].Execute(text);
            }
            return text;
        }

        public Regex tokenSelector { get; set; } = new Regex(@"([a-zA-Z]+)");

        /// <summary>
        /// Characters that are going to be used to split the text into tokens
        /// </summary>
        /// <value>
        /// The token splitter chars.
        /// </value>
        public char[] TokenSplitterChars { get; set; }
    }
}