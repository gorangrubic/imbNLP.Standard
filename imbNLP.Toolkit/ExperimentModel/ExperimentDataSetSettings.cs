using imbNLP.Toolkit.ExperimentModel.CrossValidation;
using imbNLP.Toolkit.ExperimentModel.Settings;
using System;

namespace imbNLP.Toolkit.ExperimentModel
{
    public class ExperimentDataSetSettings
    {

        public CrossValidationModel validation { get; set; } = new CrossValidationModel();

        public becDataSetSettings dataset { get; set; } = new becDataSetSettings();

        public ExperimentDataSetSettings()
        {

        }

        public String GetShortSignature()
        {

            return dataset.GetShortSignature() + validation.GetShortSignature();

        }

    }
}