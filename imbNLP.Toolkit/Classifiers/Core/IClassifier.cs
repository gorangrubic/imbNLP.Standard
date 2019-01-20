using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Feature;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Classifiers.Core
{

    /// <summary>
    /// Classification algorithm wrapper
    /// </summary>
    public interface IClassifier : IDescribe
    {
        String GetSignature();

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        String name { get; set; }

        void Deploy(ClassifierSettings _setup);

        void DoTraining(IEnumerable<FeatureVectorWithLabelID> trainingSet, ILogBuilder logger);

        Double DoScore(FeatureVector target, ILogBuilder logger, Int32 labelID = -1);

        Int32 DoSelect(FeatureVector target, ILogBuilder logger);

    }

}