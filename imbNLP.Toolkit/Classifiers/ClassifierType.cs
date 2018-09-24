namespace imbNLP.Toolkit.Classifiers
{

    public enum DistanceFunctionType
    {
        SquareEuclidean,
        Euclidean,

        Cosine,
        Jaccard,
        Hamming,
        Dice

    }

    public enum ClassifierType
    {
        none,
        simpleTopScore,
        kNearestNeighbors,
        naiveBayes,
        naiveBayesMultinominal,
        backPropagationActivationNeuralNetwork,
        multiClassSVM,
    }

}