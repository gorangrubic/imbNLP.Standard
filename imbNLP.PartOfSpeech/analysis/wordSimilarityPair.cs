using System;

namespace imbNLP.PartOfSpeech.analysis
{
    /// <summary>
    /// Represents one pair of similar words
    /// </summary>
    public class wordSimilarityPair
    {
        /// <summary>
        /// Gets or sets the word a.
        /// </summary>
        /// <value>
        /// The word a.
        /// </value>
        public String wordA { get; set; }

        /// <summary>
        /// Gets or sets the word b.
        /// </summary>
        /// <value>
        /// The word b.
        /// </value>
        public String wordB { get; set; }

        /// <summary>
        /// Similarity score
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public Double score { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="wordSimilarityPair"/> class.
        /// </summary>
        /// <param name="_wordA">The word a.</param>
        /// <param name="_wordB">The word b.</param>
        /// <param name="_score">The score.</param>
        public wordSimilarityPair(String _wordA, String _wordB, Double _score)
        {
            wordA = _wordA;
            wordB = _wordB;
            score = _score;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="wordSimilarityPair"/> class.
        /// </summary>
        public wordSimilarityPair()
        {
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{0,-30} : {1,-30}   {2,10}", wordA, wordB, score.ToString("F5"));
        }
    }
}