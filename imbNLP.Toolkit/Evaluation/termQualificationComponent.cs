
using imbNLP.Toolkit.Analysis;
using System;

namespace imbNLP.Toolkit.Evaluation
{
    /// <summary>
    /// Term qualification component
    /// </summary>
    public class termQualificationComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="termQualificationComponent"/> class.
        /// </summary>
        public termQualificationComponent()
        {
        }

        /// <summary>
        /// Gets or sets the treshold, used for word similarity
        /// </summary>
        /// <value>
        /// The treshold.
        /// </value>
        public Double treshold { get; set; } = 0.6;

        /// <summary>
        /// Gets or makes neutral <see cref="termQualification"/>
        /// </summary>
        /// <param name="word">The word to be evaluated.</param>
        /// <param name="list">The list with qualification definitions</param>
        /// <returns></returns>
        public termQualification EvaluateToTerm(String word, termQualificationList list)
        {
            if (list == null) return new termQualification(word, 0);

            foreach (termQualification tq in list)
            {
                if (tq.lemmaForm.Equals(word, StringComparison.InvariantCultureIgnoreCase))
                {
                    return tq;
                }
            }

            foreach (var tq in list)
            {
                if (similarity.GetResult(tq.lemmaForm, word) > treshold)
                {
                    return tq;
                }
            }

            return new termQualification(word, 0);
        }

        /// <summary>
        /// Evaluates the specified word
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="list">Qualification list</param>
        /// <returns></returns>
        public termQualificationAnswer Evaluate(String word, termQualificationList list)
        {
            termQualification tq = EvaluateToTerm(word, list);

            return (termQualificationAnswer)tq.score;
        }

        /// <summary>
        /// Settings for cloud waveing when <see cref="useSimilarity"/> is on
        /// </summary>
        /// <value>
        /// The similar words.
        /// </value>
        public wordSimilarityComponent similarity { get; set; } = new wordSimilarityComponent();
    }
}