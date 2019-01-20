using System;

namespace imbNLP.Project.Operations
{
    [Flags]
    public enum ClassificationDatasetSeparationEnum
    {
        none = 0,

        TrainingLabeled = 1,

        TestLabeled = 2,

        TrainingUnlabeled = 4,

        TestUnlabeled = 8,

        /// <summary>
        /// If will use labeled vectors for training and then perform test over unlabeled ones
        /// </summary>
        TrainingLabeled_TestUnlabeled = TrainingLabeled | TestUnlabeled,

        /// <summary>
        /// It will use all vectors, breaching k-fold sendbox, and test unlabeled. THIS RARELY MAKES SENSE FROM SCIENTIFIC POINT OF VIEW
        /// </summary>
        TrainingAll_TestUnlabeled = TrainingLabeled | TrainingUnlabeled | TestUnlabeled,

        /// <summary>
        /// It will use all vectors, breaching k-fold sendbox, and test all. THIS RARELY MAKES SENSE FROM SCIENTIFIC POINT OF VIEW
        /// </summary>
        TrainingAll_TestAll = TrainingLabeled | TrainingUnlabeled | TestUnlabeled | TestLabeled,

        /// <summary>
        /// It will train with labeled vectors and then test both training and test sets
        /// </summary>
        TrainingLabeled_TestAll = TrainingLabeled | TestUnlabeled | TestLabeled,

        TrainingLabeled_TestLabeled = TrainingLabeled | TestLabeled,



    }
}