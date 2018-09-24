using System;

namespace imbNLP.Toolkit.Classifiers
{
    /// <summary>
    /// Simple max vote machine --- used by <see cref="WebPostSimpleClassifier"/> classifier
    /// </summary>
    [Serializable]
    public class simpleMaxVoteMachine
    {
        public simpleMaxVoteMachine()
        {
        }

        /// <summary>
        /// Just returns index of the highest score in the array
        /// </summary>
        /// <param name="scores">The scores.</param>
        /// <returns></returns>
        public Int32 Decide(Double[] scores)
        {
            Double max = Double.MinValue;
            Int32 output = -1;
            Int32 c = 0;
            foreach (Double score in scores)
            {
                if (score > max)
                {
                    max = score;
                    output = c;
                }
                c++;
            }
            return output;
        }
    }
}