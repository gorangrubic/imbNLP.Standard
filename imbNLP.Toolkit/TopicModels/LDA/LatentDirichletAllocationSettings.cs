using imbNLP.Toolkit.Space;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.TopicModels.LDA
{
    public class LatentDirichletAllocationSettings
    {
        /// <summary>
        /// Targeted number of topics - parameter: K
        /// </summary>
        /// <value>
        /// The n of topics.
        /// </value>
        public Int32 K { get; set; } = 5;

        public Double alpha { get; set; } = 1.0;
        public Double eta { get; set; } = 0.001;

        public Int32 iterations { get; set; } = 1000;

        public LatentDirichletAllocationSettings()
        {
        }
    }
}