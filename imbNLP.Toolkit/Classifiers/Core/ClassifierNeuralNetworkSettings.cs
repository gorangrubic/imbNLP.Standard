using Accord.MachineLearning.VectorMachines.Learning;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Classifiers.Core
{

    /// <summary>
    /// Expanded classifier settings, used to setup neural networks
    /// </summary>
    public class ClassifierNeuralNetworkSettings
    {
        public ClassifierNeuralNetworkSettings()
        {
        }

        [XmlAttribute]
        public Double errorLowerLimit { get; set; } = 0.010;

        [XmlAttribute]
        public Int32 learningIterationsMax { get; set; } = 50;

        [XmlAttribute]
        public Double learningRate { get; set; }

        [XmlAttribute]
        public Double momentum { get; set; }

        public Double alpha { get; set; }

        public ClassifierNeuralNetworkType networkType { get; set; } = ClassifierNeuralNetworkType.LevenbergMarquardtLearning;

        public List<Int32> HiddenLayersNeuronCounts { get; set; } = new List<int>();
    }

}