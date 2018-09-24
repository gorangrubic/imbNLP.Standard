using System.Text.RegularExpressions;

namespace imbNLP.Toolkit.Processing
{
    /// <summary>
    /// Basic tokenizer for English - it removes all non-Latin characters during tokenization
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Processing.TokenizerBase" />
    public class TokenizerBasic : TokenizerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenizerBasic"/> class.
        /// </summary>
        public TokenizerBasic()
        {
            InputReplacers.Add(new TokenizeReplaceRule("<[^<>]+>", "", "Removes HTML code remnants"));
            InputReplacers.Add(new TokenizeReplaceRule("[0-9]+", "", "Removes numbers"));
            InputReplacers.Add(new TokenizeReplaceRule(@"(http|https)://[^\s]*", "", "Removes any in-text URL path"));
            InputReplacers.Add(new TokenizeReplaceRule(@"[^\s]+@[^\s]+", "", "Removes e-mail addresses"));
            InputReplacers.Add(new TokenizeReplaceRule("[$]+", "dollar", "Replaces dollar sign with word"));
            InputReplacers.Add(new TokenizeReplaceRule(@"@[^\s]+", "", "Removes username tokens"));
            InputReplacers.Add(new TokenizeReplaceRule("([_]+)", "", "Removes lower line from tokens"));

            tokenSelector = new Regex(@"([a-zA-Z]+)");

            // TokenSplitterChars = " @$/#.-:&*+=[]?!(){},''\">_<;%\\\n\r".ToCharArray();
        }
    }
}