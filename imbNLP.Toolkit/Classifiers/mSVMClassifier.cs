using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using imbNLP.Toolkit.Feature;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Classifiers.Core
{

    public class mSVMClassifier : ClassifierBase
    {

        public mSVMClassifier() : base()
        {
            name = "mSVM";
        }

        public override string GetSignature()
        {
            String output = name + setup.lossFunctionForTraining.ToString() + "_" + model.ToString().First();
            return output;
        }

        protected MulticlassSupportVectorLearning<Linear> teacher { get; set; }

        protected MulticlassSupportVectorMachine<Linear> machine { get; set; }

        protected MulticlassSupportVectorLearning<Gaussian> teacherGaussian { get; set; }

        protected MulticlassSupportVectorMachine<Gaussian> machineGaussian { get; set; }

        public enum mSVMModels
        {
            linear,
            gaussian
        }

        public mSVMModels model { get; set; } = mSVMModels.linear;

        public override void Deploy(ClassifierSettings _setup)
        {
            setup = _setup;
            model = setup.svmModel;

            if (model == mSVMModels.linear)
            {
                // Create a one-vs-one multi-class SVM learning algorithm
                teacher = new MulticlassSupportVectorLearning<Linear>()
                {
                    // using LIBLINEAR's L2-loss SVC dual for each SVM
                    Learner = (p) => new LinearDualCoordinateDescent()
                    {
                        Loss = setup.lossFunctionForTraining

                    }
                };
            }
            if (model == mSVMModels.gaussian)
            {
                // Create the multi-class learning algorithm for the machine
                teacherGaussian = new MulticlassSupportVectorLearning<Gaussian>()
                {
                    // Configure the learning algorithm to use SMO to train the
                    //  underlying SVMs in each of the binary class subproblems.
                    Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
                    {
                        // Estimate a suitable guess for the Gaussian kernel's parameters.
                        // This estimate can serve as a starting point for a grid search.
                        UseKernelEstimation = true
                    }
                };
            }


            //teacher.UseKernelEstimation = true;



            // The following line is only needed to ensure reproducible results. Please remove it to enable full parallelization
            // teacher.ParallelOptions.MaxDegreeOfParallelism = 1; // (Remove, comment, or change this line to enable full parallelism)

        }

        public override Double DoScore(FeatureVector target, ILogBuilder logger, Int32 labelID = -1)
        {
            Double result = 0;
            switch (model)
            {
                case mSVMModels.linear:
                    if (labelID == -1)
                    {
                        result = machine.Score(target.dimensions);
                    }
                    else
                    {
                        result = machine.Score(target.dimensions, labelID);
                    }



                    break;
                case mSVMModels.gaussian:
                    if (labelID == -1)
                    {
                        result = machineGaussian.Score(target.dimensions);
                    }
                    else
                    {
                        result = machineGaussian.Score(target.dimensions, labelID);
                    }



                    break;
            }

            return result;
        }

        public override int DoSelect(FeatureVector target, ILogBuilder logger)
        {
            Int32 result = 0;
            switch (model)
            {
                case mSVMModels.linear:
                    result = machine.Decide(target.dimensions);
                    break;
                case mSVMModels.gaussian:
                    result = machineGaussian.Decide(target.dimensions);

                    break;
            }

            return result;
        }

        public override void DoTraining(IEnumerable<FeatureVectorWithLabelID> trainingSet, ILogBuilder logger)
        {

            IEnumerable<double[]> vectors = trainingSet.Select(x => x.vector.dimensions);
            IEnumerable<int> labels = trainingSet.Select(x => x.labelID);
            // Learn a machine
            if (model == mSVMModels.linear)
            {
                machine = teacher.Learn(vectors.ToArray(), labels.ToArray());
            }

            if (model == mSVMModels.gaussian)
            {
                machineGaussian = teacherGaussian.Learn(vectors.ToArray(), labels.ToArray());


                // Create the multi-class learning algorithm for the machine
                var calibration = new MulticlassSupportVectorLearning<Gaussian>()
                {
                    Model = machineGaussian, // We will start with an existing machine

                    // Configure the learning algorithm to use Platt's calibration
                    Learner = (param) => new ProbabilisticOutputCalibration<Gaussian>()
                    {
                        Model = param.Model // Start with an existing machine
                    }
                };

            }

        }
    }

}