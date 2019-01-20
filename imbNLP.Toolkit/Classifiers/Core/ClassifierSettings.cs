using Accord.MachineLearning.VectorMachines.Learning;
using System;
using static imbNLP.Toolkit.Classifiers.Core.mSVMClassifier;

namespace imbNLP.Toolkit.Classifiers.Core
{



    /// <summary>
    /// General settings model for classifiers
    /// </summary>
    public class ClassifierSettings
    {






        public ClassifierSettings()
        {
        }

        public ClassifierSettings(ClassifierType _type, String _name)
        {
            type = _type;
            name = _name;

            switch (type)
            {
                case ClassifierType.backPropagationActivationNeuralNetwork:
                    //HiddenLayerOneNeuronCount = 8;
                    //HiddenLayerTwoNeuronCount = 8;
                    neuralnetwork = new ClassifierNeuralNetworkSettings();
                    neuralnetwork.HiddenLayersNeuronCounts.Add(6);
                    neuralnetwork.HiddenLayersNeuronCounts.Add(5);
                    neuralnetwork.alpha = 2;
                    neuralnetwork.learningRate = 1;
                    neuralnetwork.momentum = 0.5;
                    neuralnetwork.errorLowerLimit = 0.010;
                    neuralnetwork.learningIterationsMax = 50;
                    break;

                case ClassifierType.kNearestNeighbors:
                    kNN_k = 2;
                    break;

                case ClassifierType.multiClassSVM:
                    lossFunctionForTraining = Loss.L2;

                    break;

                case ClassifierType.naiveBayes:

                    break;

                case ClassifierType.simpleTopScore:
                    break;
            }
        }

        public ClassifierNeuralNetworkSettings neuralnetwork { get; set; }

        //public Int32 HiddenLayerTwoNeuronCount { get; set; }

        public Int32 kNN_k { get; set; }

        public Loss lossFunctionForTraining { get; set; }

        public String name { get; set; } = "";

        public mSVMModels svmModel { get; set; } = mSVMModels.linear;

        public DistanceFunctionType distanceFunction { get; set; } = DistanceFunctionType.SquareEuclidean;

        public ClassifierType type { get; set; } = ClassifierType.none;

        public String GetSignature()
        {
            IClassifier cl = GetClassifier();
            return cl.GetSignature();
        }

        public IClassifier GetClassifier()
        {
            IClassifier output = null;


            switch (type)
            {

                case ClassifierType.kNearestNeighbors:
                    output = new kNNClassifier();
                    break;
                case ClassifierType.multiClassSVM:
                    output = new mSVMClassifier();

                    break;
                case ClassifierType.naiveBayes:
                    break;
                case ClassifierType.naiveBayesMultinominal:
                    break;
                case ClassifierType.none:
                    break;
                case ClassifierType.simpleTopScore:
                    break;
                default:
                    output = new kNNClassifier();
                    break;

            }

            output.Deploy(this);

            return output;

        }

    }

}