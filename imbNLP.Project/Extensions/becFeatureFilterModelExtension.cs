using imbACE.Core.operations;
using imbACE.Services.consolePlugins;
using imbNLP.Toolkit.Weighting;
using imbSCI.Core.enums;
using imbSCI.Core.files.folders;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace imbNLP.Project.Extensions
{
    public class becFeatureFilterModelExtension : instanceLoadSaveExtension<FeatureFilter>
    {
        private becWeightingModelEditExtension _weight;
        //private becWeightingModelEditExtension _model;

        // public becWeightingModelEditExtension weight { get => _model; set => _model = value; }

        public override void SetSubBinding()
        {
            model.SetBinding(data, nameof(data.WeightModel), true);
        }

        public becFeatureFilterModelExtension(folderNode __folder, IAceOperationSetExecutor __parent) : base(__folder, __parent)
        {
            model = new becWeightingModelEditExtension(__folder, __parent);
            SetSubBinding();
        }

        [Display(GroupName = "Setup", Name = "model", Description = "Term weighting")]
        public becWeightingModelEditExtension model
        {
            get
            {
                return _weight;
            }
            set
            {
                _weight = value;
                OnPropertyChanged(nameof(model));
            }
        }

        [Display(GroupName = "set", Name = "FeatureFilter", ShortName = "", Description = "Configures feature selection filter")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will set feature count limit and function")]
        /// <summary>Configures feature selection filter</summary>
        /// <remarks><para>It will set feature count limit and function</para></remarks>
        /// <param name="function">Name of the global function that will rank the features</param>
        /// <param name="limit">Number of features to be adopted</param>
        /// <param name="TDP">TDP factor to be applied (when used with Collection basedGlobal element)</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setFeatureFilter(
             [Description("Name of the global function that will rank the features")] Boolean RemoveZero,
             [Description("Number of features to be adopted")] Int32 limit = 4000,
             [Description("TDP factor to be applied (when used with Collection basedGlobal element)")]  operation nVectorOperation = operation.max,
             [Description("Inverse Document Frequency computation variation")] String outputFilename = "")
        {

            data.RemoveZero = RemoveZero;
            data.nVectorValueSelectionOperation = nVectorOperation;

            data.limit = limit;
            data.outputFilename = outputFilename; //.functionSettings.flags.Add(IDFc.ToString()); //.idfComputation = IDFc;

            data.Deploy(output);
        }


    }
}