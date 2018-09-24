using imbNLP.Toolkit.Core;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.ExperimentModel.CrossValidation
{


    /// <summary>
    /// Settings for cross validation model
    /// </summary>
    public class CrossValidationModel : IDescribe
    {
        /// <summary>
        /// In single fold mode - it trains and tests with the same dataset
        /// </summary>
        /// <value>
        ///   <c>true</c> if [single fold]; otherwise, <c>false</c>.
        /// </value>
        public Boolean SingleFold { get; set; } = true;

        /// <summary>
        /// Describes the cross validation settings
        /// </summary>
        /// <param name="logger">The logger.</param>
        public void Describe(ILogBuilder logger)
        {
            logger.AppendLine("--- k-fold crossvalidation settings---");

            logger.AppendPair("Number of folds [k]", K, true, "\t\t\t");
            logger.AppendPair("Training folds [t]", TrainingFolds, true, "\t\t\t");
            logger.AppendPair("Fold randomization", randomFolds.ToString(), true, "\t\t\t");
        }


        /// <summary>
        /// Gets the distribution matrix - showing what folds are to be part of training set (true) and what test set (false)
        /// </summary>
        /// <returns>training set (true), test set (false)</returns>
        public List<List<Boolean>> GetDistributionMatrix()
        {
            List<List<Boolean>> output = new List<List<bool>>();

            for (int i = 0; i < K; i++)
            {
                output.Add(new List<bool>());

                for (int s = 0; s < K; s++)
                {
                    Int32 p = s + TestFolds;
                    Int32 r = p % K;

                    Boolean o = true;
                    if ((i >= s) && (i < p))
                    {
                        o = false;
                    }
                    else
                    {
                        o = true;
                        if (p >= K)
                        {
                            if (i < r)
                            {
                                o = false;
                            }
                        }

                    }
                    output[i].Add(o);
                }
            }

            return output;
        }


        /// <summary>
        /// if set above 0, only specified number of folds will be executed
        /// </summary>
        /// <value>
        /// The limit folds execution.
        /// </value>
        public Int32 LimitFoldsExecution { get; set; } = -1;

        /// <summary>
        /// Gets or sets a value indicating whether distribution among folds should be randomized
        /// </summary>
        /// <value>
        ///   <c>true</c> if [random folds]; otherwise, <c>false</c>.
        /// </value>
        public Boolean randomFolds { get; set; } = true;

        /// <summary>
        /// Number of folds
        /// </summary>
        /// <value>
        /// The k.
        /// </value>
        public Int32 K { get; set; } = 5;

        /// <summary>
        /// Gets or sets the training folds.
        /// </summary>
        /// <value>
        /// The training folds.
        /// </value>
        public Int32 TrainingFolds { get; set; } = 4;

        /// <summary>
        /// Gets the test folds.
        /// </summary>
        /// <value>
        /// The test folds.
        /// </value>
        public Int32 TestFolds
        {
            get
            {
                return K - TrainingFolds;
            }
        }

        public CrossValidationModel()
        {

        }
    }
}
