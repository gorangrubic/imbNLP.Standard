using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace imbNLP.Toolkit.Stemmers
{
    public static class nltkTools
    {


        public static void SetRules(this Dictionary<String, String> dictionary, String rules)
        {
            var mcs = _select_isSuffixRulePattern.Matches(rules);
            foreach (Match m in mcs)
            {
                if (m.Groups.Count > 1)
                {
                    String key = m.Groups[1].Value;
                    String value = m.Groups[2].Value;
                    dictionary.Add(key, value);
                }
            }
        }

        /// <summary>
        /// Regex select SuffixRulePattern : '([\w]*)'\:'([\w]*)',?
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_isSuffixRulePattern = new Regex(@"'([\w]*)'\:'([\w]*)',?", System.Text.RegularExpressions.RegexOptions.Compiled);

        /// <summary>
        /// Test if input matches '([\w]*)'\:'([\w]*)',?
        /// </summary>
        /// <param name="input">String to test</param>
        /// <returns>IsMatch against _select_isSuffixRulePattern</returns>
        public static Boolean isSuffixRulePattern(this String input)
        {
            if (String.IsNullOrEmpty(input)) return false;
            return _select_isSuffixRulePattern.IsMatch(input);
        }



    }
}
