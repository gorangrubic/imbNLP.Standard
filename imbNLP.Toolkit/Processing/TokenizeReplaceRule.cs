using System.Text.RegularExpressions;

namespace imbNLP.Toolkit.Processing
{
    /// <summary>
    /// Instance of string replacement rule, utilized by tokenizers
    /// </summary>
    public class TokenizeReplaceRule
    {
        /// <summary>
        /// Regular expression used as needle
        /// </summary>
        /// <value>
        /// The regex search.
        /// </value>
        public string RegexSearch { get; set; } = "";

        /// <summary>
        /// Replacement string value
        /// </summary>
        /// <value>
        /// The regex replace.
        /// </value>
        public string RegexReplace { get; set; } = "";


        /// <summary>
        /// Human-readable comment on Regex replacement instruction
        /// </summary>
        /// <value>
        /// The comment.
        /// </value>
        public string Comment { get; set; } = "Regex replacement";

        protected Regex regex { get; set; }

        /// <summary>
        /// Constructor to be used by XML serializer
        /// </summary>
        public TokenizeReplaceRule()
        {
        }

        /// <summary>
        /// Main constructor, to be used when rule is specified from the code
        /// </summary>
        /// <param name="Search">The search.</param>
        /// <param name="Replace">The replace.</param>
        public TokenizeReplaceRule(string Search, string Replace, string comment)
        {
            RegexSearch = Search;
            RegexReplace = RegexReplace;
            Comment = comment;
            regex = new Regex(Search);
        }

        /// <summary>
        /// Executes the rule against the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>processed version of the text</returns>
        public string Execute(string text)
        {
            if (regex == null)
            {
                regex = new Regex(RegexSearch);
            }
            return regex.Replace(text, RegexReplace);
        }
    }
}