using imbNLP.Toolkit.Feature;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Classifiers.Core
{


    public abstract class ClassifierBase : IClassifier
    {

        protected ClassifierSettings setup { get; set; }

        public abstract void Deploy(ClassifierSettings _setup);

        public abstract String GetSignature();

        public String name { get; set; } = "";


        public abstract void DoTraining(IEnumerable<FeatureVectorWithLabelID> trainingSet, ILogBuilder logger);

        public abstract Int32 DoSelect(FeatureVector target, ILogBuilder logger);

        public virtual void Describe(ILogBuilder logger)
        {
            logger.AppendLine("Classifier: \t\t\t " + name);
        }
    }

}