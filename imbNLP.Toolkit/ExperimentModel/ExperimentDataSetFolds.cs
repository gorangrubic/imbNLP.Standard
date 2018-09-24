using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.ExperimentModel.CrossValidation;
using imbNLP.Toolkit.Space;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.ExperimentModel
{

    /// <summary>
    /// Folds for the cross validation experiment
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{imbNLP.Toolkit.ExperimentModel.ExperimentDataSetFold}" />
    public class ExperimentDataSetFolds : List<ExperimentDataSetFold>
    {

        public string name { get; set; } = "";

        public CrossValidationModel settings { get; set; } = new CrossValidationModel();

        public ExperimentDataSetFolds() { }




        /// <summary>
        /// Deploys the specified settings.
        /// </summary>
        /// <param name="_settings">The settings.</param>
        /// <param name="_dataset">Un-folded dataset, without having the unknown class defined</param>
        /// <param name="logger">The logger.</param>
        public void Deploy(CrossValidationModel _settings, List<WebSiteDocumentsSet> _dataset, ILogBuilder logger)
        {
            settings = _settings;

            if (settings.SingleFold)
            {
                name = "1-fold -- single fold override";
                ExperimentDataSetFold fold = new ExperimentDataSetFold();
                fold.name = "SingleFold";
                fold.AddRange(_dataset);
                Add(fold);
                return;
            }
            name = settings.K + "-fold Tr[" + _settings.TrainingFolds + "] Ts[" + _settings.TestFolds + "]";

            List<CategorySlicedFolds> folds = new List<CategorySlicedFolds>();

            Dictionary<WebSiteDocumentsSet, CategorySlicedFolds> slicedFolds = new Dictionary<WebSiteDocumentsSet, CategorySlicedFolds>();

            foreach (WebSiteDocumentsSet cat in _dataset)
            {
                CategorySlicedFolds fold = new CategorySlicedFolds();
                fold.Deploy(cat, settings.K, settings.randomFolds);
                folds.Add(fold);
                slicedFolds.Add(cat, fold);
            }

            // --------------------------------------------------------- //

            var distributionMatrix = settings.GetDistributionMatrix();

            Int32 foldsToCreate = settings.K;
            if (settings.LimitFoldsExecution > 0)
            {
                foldsToCreate = settings.LimitFoldsExecution;
            }
            for (int i = 0; i < foldsToCreate; i++)
            {
                ExperimentDataSetFold setFold = new ExperimentDataSetFold();
                setFold.name = settings.K + "-fold[" + i + "]";

                setFold.CopyLabelNames(_dataset);

                WebSiteDocumentsSet unknownCat = new WebSiteDocumentsSet(SpaceLabel.UNKNOWN, "Test category - " + setFold.name);

                setFold.Add(unknownCat);

                foreach (KeyValuePair<WebSiteDocumentsSet, CategorySlicedFolds> catPair in slicedFolds)
                {
                    WebSiteDocumentsSet cat = setFold.First(x => x.name == catPair.Key.name);

                    for (int s = 0; s < settings.K; s++)
                    {
                        bool toTraining = distributionMatrix[i][s];

                        if (toTraining)
                        {
                            cat.AddRange(catPair.Value[s]);
                        }
                        else
                        {
                            unknownCat.AddRange(catPair.Value[s]);
                        }
                    }
                }

                Add(setFold);
            }



        }

    }

}