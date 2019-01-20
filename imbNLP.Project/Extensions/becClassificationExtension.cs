using imbACE.Core.operations;
using imbACE.Services.consolePlugins;
using imbNLP.Project.Operations;
using imbNLP.Project.Operations.Setups;
using imbSCI.Core.extensions.data;
using imbSCI.Core.files.folders;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace imbNLP.Project.Extensions
{


    public class becClassificationExtension : instanceLoadSaveExtension<SetupDocumentClassification>
    {





        [Display(GroupName = "run", Name = "ResetModels", ShortName = "", Description = "Removes all components of the weight model at feature filter and feature weight models")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "Clears weight models")]
        /// <summary>Removes all components of the weight model at feature filter and feature weight models</summary> 
        /// <remarks><para>Clears weight models</para></remarks>
        /// <param name="filter">--</param>
        /// <param name="weight">--</param>
        /// <param name="render">--</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_runResetModels(
              [Description("--")] Boolean filter = true,
              [Description("--")] Boolean weight = true,
              [Description("--")] Boolean render = false)
        {

            if (filter) data.corpusMethod.filter.WeightModel.GlobalFactors.Clear();

            if (weight) data.corpusMethod.WeightModel.GlobalFactors.Clear();

            if (render) data.entityMethod.instructions.Clear();
        }




        [Display(GroupName = "set", Name = "ExportEvaluation", ShortName = "", Description = "Sets export if classification result of each document to feature vector dictionary")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will set feature vectors with dimension describing classification test result")]
        /// <summary>Exports evaluation of each document to feature vector dictionary</summary> 
        /// <remarks><para>What it will do?</para></remarks>
        /// <param name="filename">Name of the dictionary with evaluation result. Leave empty to disable the export</param>
        /// <param name="correct">--</param>
        /// <param name="incorrect">--</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setExportEvaluation(
              [Description("Name of the dictionary with evaluation result")] String filename = "*",
              [Description("Score to assign for correct classification")] Double correct = 1,
              [Description("Score to assign for incorrect classification")] Double incorrect = 0)
        {
            data.ExportEvaluationAsDocumentSelectionResult = !filename.isNullOrEmpty();
            data.ExportEvaluationCorrectScore = correct;
            data.ExportEvaluationIncorrectScore = incorrect;
            data.ExportEvaluationToFilename = filename;
        }



        [Display(GroupName = "set", Name = "DataSetMode", ShortName = "", Description = "Sets how dataset will be managed for classificator training and testing procedure")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "What it will do?")]
        /// <summary>Sets how dataset will be managed for classificator training and testing procedure</summary> 
        /// <remarks><para>What it will do?</para></remarks>
        /// <param name="ClassificationDatasetSeparationEnum">Mode to be set, leave none to set default</param>
        /// <param name="steps">--</param>
        /// <param name="debug">--</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setDataSetMode(
              [Description("Mode to be set, leave none to set default")] ClassificationDatasetSeparationEnum mode = ClassificationDatasetSeparationEnum.none)
        {

            if (mode == ClassificationDatasetSeparationEnum.none) mode = ClassificationDatasetSeparationEnum.TrainingLabeled_TestUnlabeled;

            data.dataSetMode = mode;
        }

        public override void SetSubBinding()
        {
            _rendering.SetBinding(data, nameof(data.entityMethod), true);
            _features.SetBinding(data, nameof(data.featureMethod), true);
            _weight.SetBinding(data, nameof(data.corpusMethod), true);
            _filter.SetBinding(data.corpusMethod, nameof(data.corpusMethod.filter), true);
        }

        public becClassificationExtension(folderNode __folder, IAceOperationSetExecutor __parent) : base(__folder, __parent)
        {
            _rendering = new becDocumentRenderingExtension(__parent, __folder);
            _features = new becFeatureVectorExtension(__folder, __parent);
            _weight = new becWeightingModelExtension(__folder, __parent);
            _filter = new becFeatureFilterModelExtension(__folder, __parent);
            SetSubBinding();
        }

        private becDocumentRenderingExtension _rendering;

        [Display(GroupName = "Setup", Name = "render", Description = "Document rendering")]
        public becDocumentRenderingExtension rendering
        {
            get
            {
                return _rendering;
            }
            set
            {
                _rendering = value;
                OnPropertyChanged(nameof(rendering));
            }
        }

        private becWeightingModelExtension _weight;

        [Display(GroupName = "Setup", Name = "weight", Description = "Term weighting")]
        public becWeightingModelExtension weight
        {
            get
            {
                return _weight;
            }
            set
            {
                _weight = value;
                OnPropertyChanged(nameof(weight));
            }
        }

        [Display(GroupName = "Setup", Name = "filter", Description = "Term weighting")]
        public becFeatureFilterModelExtension filter
        {
            get
            {
                return _filter;
            }
            set
            {
                _filter = value;
                OnPropertyChanged(nameof(filter));
            }
        }

        private becFeatureVectorExtension _features;
        private becFeatureFilterModelExtension _filter;

        [Display(GroupName = "Setup", Name = "fv", Description = "feature vector constructor")]
        public becFeatureVectorExtension features
        {
            get
            {
                return _features;
            }
            set
            {
                _features = value;
                OnPropertyChanged(nameof(features));
            }
        }
    }
}