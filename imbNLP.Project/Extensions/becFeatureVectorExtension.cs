using Accord.MachineLearning.VectorMachines.Learning;
using imbACE.Core.operations;
using imbACE.Services.consolePlugins;
using imbNLP.Toolkit.Classifiers;
using imbNLP.Toolkit.Feature.Settings;
using imbNLP.Toolkit.Planes;
using imbSCI.Core.files.folders;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static imbNLP.Toolkit.Classifiers.Core.mSVMClassifier;

namespace imbNLP.Project.Extensions
{
    public class becFeatureVectorExtension : instanceLoadSaveExtension<FeaturePlaneMethodSettings>
    {
        public becFeatureVectorExtension(folderNode folder, IAceOperationSetExecutor __parent) : base(folder, __parent)
        {
        }




        [Display(GroupName = "set", Name = "MemoryExport", ShortName = "", Description = "Controls how classifier memory is handled before and after training")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "What it will do?")]
        /// <summary>Controls how classifier memory is handled before and after training</summary> 
        /// <remarks><para>What it will do?</para></remarks>
        /// <param name="filename">--</param>
        /// <param name="steps">--</param>
        /// <param name="debug">--</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setMemoryExport(
              [Description("--")] String filename = "*")
        {
            if (filename == "*") filename = data.classifierSettings.name;
            data.ExportClassifierMemory = filename;
        }

        [Display(GroupName = "set", Name = "MemoryImport", ShortName = "", Description = "Controls how classifier memory is handled before and after training")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "What it will do?")]
        /// <summary>Controls how classifier memory is handled before and after training</summary> 
        /// <remarks><para>What it will do?</para></remarks>
        /// <param name="filename">--</param>
        /// <param name="steps">--</param>
        /// <param name="debug">--</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setMemoryImport(
            [Description("--")] String filename = "*")
        {
            if (filename == "*") filename = data.classifierSettings.name;
            data.ImportClassifierMemory = filename;
        }



        [Display(GroupName = "set", Name = "AddFeatureDimension", ShortName = "", Description = "Defines new feature dimension")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It creates new feature dimension and optionally clears existing")]
        /// <summary>Defines new feature dimension</summary>
        /// <remarks><para>It creates new feature dimension and optionally clears existing</para></remarks>
        /// <param name="type">Dimension type</param>
        /// <param name="function">Name of the function, if it is required by dimension type (e.g. similarity with class dimensions)</param>
        /// <param name="clearExisting">Removes any existing feature dimension from the experiment settings</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setAddFeatureDimension(
              [Description("Dimension type")] FeatureVectorDimensionType type = FeatureVectorDimensionType.similarityFunction,
              [Description("Name of the function, if it is required by dimension type (e.g. similarity with class dimensions)")] String function = "CosineSimilarityFunction",
              [Description("Removes any existing feature dimension from the experiment settings")] Boolean clearExisting = false)
        {
            if (clearExisting)
            {
                data.constructor.labelDimensions.Clear();
                data.constructor.featureDimensions.Clear();
                data.constructor.topicDimensions.Clear();
            }

            dimensionSpecification dim = new dimensionSpecification();
            dim.type = type;
            dim.functionName = function;

            data.constructor.AddDimensionSpecification(dim);
        }

        [Display(GroupName = "set", Name = "mSVM", ShortName = "", Description = "Sets multi-class Support Vector Machine classifier")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "Removes any existing classifier and sets mSVM with specified settings")]
        /// <summary>Sets multi-class Support Vector Machine classifier</summary>
        /// <remarks><para>Removes any existing classifier and sets mSVM with specified settings</para></remarks>
        /// <param name="loss">Loss function to set</param>
        /// <param name="model">Model to be used with SVM classifier</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setmSVM(
              [Description("Loss function to set")] Loss loss = Loss.L2,
              [Description("Model to be used with SVM classifier")] mSVMModels model = mSVMModels.linear)
        {
            data.classifierSettings.type = imbNLP.Toolkit.Classifiers.ClassifierType.multiClassSVM;
            data.classifierSettings.lossFunctionForTraining = loss;
            data.classifierSettings.svmModel = model;
        }

        [Display(GroupName = "set", Name = "kNN", ShortName = "", Description = "Sets multi-class k-nearest neighbours classifier")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "Removes any existing classifier and sets k-NN with specified settings")]
        /// <summary>Sets multi-class k-nearest neighbours classifier</summary>
        /// <remarks><para>Removes any existing classifier and sets k-NN with specified settings</para></remarks>
        /// <param name="distance">Distance function to be used with k-NN classifier</param>
        /// <param name="k">K parameter - number of neighbours to vote for class membership</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setkNN(
              [Description("Distance function to be used with k-NN classifier")] DistanceFunctionType distance = DistanceFunctionType.SquareEuclidean,
              [Description("K parameter - number of neighbours to vote for class membership")] Int32 k = 5)
        {
            data.classifierSettings.type = imbNLP.Toolkit.Classifiers.ClassifierType.kNearestNeighbors;
            data.classifierSettings.distanceFunction = distance;
            data.classifierSettings.kNN_k = k;
        }

        public override void SetSubBinding()
        {

        }
    }
}