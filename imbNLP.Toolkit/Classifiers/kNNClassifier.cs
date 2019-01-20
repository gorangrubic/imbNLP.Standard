using Accord.MachineLearning;
using Accord.Math.Distances;
using imbNLP.Toolkit.Feature;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Classifiers.Core
{

    public class kNNClassifier : ClassifierBase
    {

        public kNNClassifier()
        {
            name = "kNN";
        }

        private IDistance<double[]> _distance = null;

        private KNearestNeighbors<double[]> kNearest { get; set; }

        public override void Deploy(ClassifierSettings _setup)
        {
            setup = _setup;

            // name = setup.kNN_k + "-NN";

            switch (setup.distanceFunction)
            {
                case DistanceFunctionType.SquareEuclidean:
                    _distance = new SquareEuclidean();
                    break;
                case DistanceFunctionType.Jaccard:
                    _distance = new Jaccard();
                    break;
                case DistanceFunctionType.Hamming:
                    _distance = new Hamming();
                    break;
                case DistanceFunctionType.Euclidean:
                    _distance = new Euclidean();
                    break;
                case DistanceFunctionType.Dice:
                    _distance = new Dice();
                    break;
                case DistanceFunctionType.Cosine:
                    _distance = new Cosine();
                    break;
            }

            kNearest = new KNearestNeighbors<Double[]>(k: setup.kNN_k, distance: _distance);
        }

        public override int DoSelect(FeatureVector target, ILogBuilder logger)
        {
            return kNearest.Decide(target.dimensions);
        }

        public override void DoTraining(IEnumerable<FeatureVectorWithLabelID> trainingSet, ILogBuilder logger)
        {
            IEnumerable<double[]> vectors = trainingSet.Select(x => x.vector.dimensions);
            IEnumerable<int> labels = trainingSet.Select(x => x.labelID);

            kNearest.Learn(vectors.ToArray(), labels.ToArray());
        }

        public override void Describe(ILogBuilder logger)
        {
            base.Describe(logger);

        }

        public override string GetSignature()
        {
            String output = name + "_k" + setup.kNN_k + "_" + setup.distanceFunction.ToString().First();
            return output;
        }

        public override Double DoScore(FeatureVector target, ILogBuilder logger, Int32 labelID = -1)
        {
            Double result = 0;

            if (labelID == -1)
            {
                result = kNearest.Score(target.dimensions);
            }
            else
            {
                result = kNearest.Score(target.dimensions, labelID);
            }


            return result;
        }
    }

}