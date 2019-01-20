using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Analysis
{
    /// <summary>
    /// Performs word similarity computation and holds settings (serializable)
    /// </summary>
    public class wordSimilarityComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="wordSimilarityComponent"/> class.
        /// </summary>
        public wordSimilarityComponent()
        {
        }

        /// <summary>
        /// Gets the similarity coeficient from 0 to 1
        /// </summary>
        /// <param name="wordA">The word a.</param>
        /// <param name="wordB">The word b.</param>
        /// <returns></returns>
        public Double GetResult(String wordA, String wordB)
        {
            Double score = 0;

            switch (equation)
            {
                case nGramsSimilarityEquationEnum.continualOverlapRatio:
                    score = wordAnalysisTools.GetContinualOverlapRatio(wordAnalysisTools.getNGrams(wordA, N, gramConstruction), wordAnalysisTools.getNGrams(wordB, N, gramConstruction));
                    break;

                case nGramsSimilarityEquationEnum.DiceCoefficient:
                    score = wordAnalysisTools.GetDiceCoefficient(wordAnalysisTools.getNGrams(wordA, N, gramConstruction), wordAnalysisTools.getNGrams(wordB, N, gramConstruction));
                    break;

                case nGramsSimilarityEquationEnum.JaccardIndex:
                    score = wordAnalysisTools.GetJaccardIndex(wordAnalysisTools.getNGrams(wordA, N, gramConstruction), wordAnalysisTools.getNGrams(wordB, N, gramConstruction));
                    break;
            }
            return score;
        }

        /// <summary>
        /// Computes similarity (using: <see cref="equation" />) of the specified words and returns pairs above the <see cref="treshold" /></summary>
        /// <param name="wordsToAnalyse">Set of words to analyse.</param>
        /// <returns>
        /// Sorted collection of pairs that had same or greater score then <see cref="treshold" /></returns>
        public wordSimilarityResultSet GetResult(IEnumerable<String> wordsToAnalyse)
        {
            wordSimilarityResultSet output = new wordSimilarityResultSet();
            output.component = this;
            List<String> words = null;

            if (wordsToAnalyse is List<String>)
            {
                words = (List<String>)wordsToAnalyse;
            }
            else
            {
                words = wordsToAnalyse.ToList();
            }

            List<List<String>> ngrams = new List<List<string>>();

            for (int ia = 0; ia < words.Count; ia++)
            {
                ngrams.Add(wordAnalysisTools.getNGrams(words[ia], N, gramConstruction));
            }

            for (int ia = 0; ia < words.Count - 1; ia++)
            {
                //String wordA = words[ia];

                for (int ib = ia + 1; ib < words.Count; ib++)
                {
                    Double score = 0;

                    switch (equation)
                    {
                        case nGramsSimilarityEquationEnum.continualOverlapRatio:
                            score = wordAnalysisTools.GetContinualOverlapRatio(ngrams[ia], ngrams[ib]);
                            break;

                        case nGramsSimilarityEquationEnum.DiceCoefficient:
                            score = wordAnalysisTools.GetDiceCoefficient(ngrams[ia], ngrams[ib]);
                            break;

                        case nGramsSimilarityEquationEnum.JaccardIndex:
                            score = wordAnalysisTools.GetJaccardIndex(ngrams[ia], ngrams[ib]);
                            break;
                    }

                    if (score >= treshold)
                    {
                        wordSimilarityPair pair = new wordSimilarityPair(words[ia], words[ib], score);
                        output.Add(pair);
                    }
                }
            }

            output.Sort((x, y) => y.score.CompareTo(x.score));
            return output;
        }

        /// <summary>
        /// Gets or sets the output treshold.
        /// </summary>
        /// <value>
        /// The output treshold.
        /// </value>
        [XmlAttribute]
        public Double treshold { get; set; } = 0.6;

        /// <summary>
        /// Gets or sets the n.
        /// </summary>
        /// <value>
        /// The n.
        /// </value>
        [XmlAttribute]
        public Int32 N { get; set; } = 2;

        /// <summary>
        /// Gets or sets the equation.
        /// </summary>
        /// <value>
        /// The equation.
        /// </value>
        public nGramsSimilarityEquationEnum equation { get; set; } = nGramsSimilarityEquationEnum.DiceCoefficient;

        /// <summary>
        /// Gets or sets the gram construction.
        /// </summary>
        /// <value>
        /// The gram construction.
        /// </value>
        public nGramsModeEnum gramConstruction { get; set; } = nGramsModeEnum.overlap;

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# N-Gram size:       " + N.ToString());
            sb.AppendLine("# Split mode:        " + gramConstruction.ToString());
            sb.AppendLine("# Equation:          " + equation.ToString());
            sb.AppendLine("# Treshold:          " + treshold.ToString("F3"));
            sb.AppendLine("----------------------------"); //          " + outputTreshold.ToString("F3"));
            return sb.ToString();
        }
    }
}