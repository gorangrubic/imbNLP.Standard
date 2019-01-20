using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Analysis
{
    /// <summary>
    /// Collection of similar words that is returned from <see cref="wordSimilarityComponent.GetResult(List{string})"/> call;
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{imbNLP.PartOfSpeech.analysis.wordSimilarityPair}" />
    public class wordSimilarityResultSet : List<wordSimilarityPair>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="wordSimilarityResultSet"/> class.
        /// </summary>
        public wordSimilarityResultSet()
        {
        }

        /// <summary>
        /// Reference to the component that produced this result set
        /// </summary>
        /// <value>
        /// The component.
        /// </value>
        [XmlIgnore]
        internal wordSimilarityComponent component { get; set; }

        /// <summary>
        /// Returns textual description of the result
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (component != null)
            {
                sb.AppendLine(component.ToString());
            }
            for (int i = 0; i < Count; i++)
            {
                sb.AppendLine((i + 1).ToString("D5") + " : " + this[i].ToString());
            }
            return sb.ToString();
        }
    }
}