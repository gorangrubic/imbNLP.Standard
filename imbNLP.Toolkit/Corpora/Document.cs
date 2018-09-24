
using System;
using System.Text.RegularExpressions;

namespace imbNLP.Toolkit.Corpora
{
    /// <summary>
    ///
    /// </summary>
    public class Document
    {
        /// <summary>
        /// The words
        /// </summary>
        public int[] Words;

        /// <summary>
        /// The length
        /// </summary>
        public int Length;

        /// <summary>
        /// Initializes the specified string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="WD">The wd.</param>
        public void Init(string str, WordDictionary WD)
        {
            try
            {
                string sp = @"\s+";
                string[] doc = Regex.Split(str, sp);
                Words = new int[doc.Length];
                Length = doc.Length;
                for (int i = 0; i < Length; i++)
                {
                    Words[i] = WD.GetWords(doc[i]);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}