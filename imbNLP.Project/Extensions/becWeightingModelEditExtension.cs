using imbACE.Core.operations;
using imbACE.Services.consolePlugins;
using imbNLP.Toolkit.Weighting;
using imbNLP.Toolkit.Weighting.Global;
using imbNLP.Toolkit.Weighting.Local;
using imbNLP.Toolkit.Weighting.Metrics;
using imbSCI.Core.files.folders;
using imbSCI.Data;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace imbNLP.Project.Extensions
{
    public class becWeightingModelEditExtension : instanceLoadSaveExtension<FeatureWeightModel>
    {



        [Display(GroupName = "set", Name = "LocalWeight", ShortName = "", Description = "Configures local function in the feature weighting model")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will set computation and normalization options for feature weighting")]
        /// <summary>Configures local function in the feature weighting model</summary>
        /// <remarks><para>It will set computation and normalization options for feature weighting</para></remarks>
        /// <param name="computation">Computation scheme to be used for the local factor</param>
        /// <param name="normalization">Normalization scheme to be used for the local factor</param>
        /// <param name="debug">--</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setLocalWeight(
              [Description("--")] TFComputation computation = TFComputation.normal,
              [Description("--")] TFNormalization normalization = TFNormalization.divisionByMaxTF)
        {
            data.LocalFunction.computation = computation;
            data.LocalFunction.normalization = normalization;
        }

        [Display(GroupName = "set", Name = "GlobalWeight", ShortName = "", Description = "Configures a global function in the feature weighting model - supports: ICF, ICSd, IDF, IGM, mIDF")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will add specified global factor, optionally if will remove any existing global factors")]
        /// <summary>
        /// Configures a global function in the feature weighting model - supports: ICF, ICSd, IDF, IGM, mIDF
        /// </summary>
        /// <param name="function">Name of function elemenet</param>
        /// <param name="weight">Weigth associated with the function</param>
        /// <param name="IDF">How IDF should be computed</param>
        /// <param name="removeExisting">if set to <c>true</c> [remove existing].</param>
        /// <remarks>
        /// It will add specified global factor, optionally if will remove any existing global factors
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase" />
        public void aceOperation_setGlobalWeight(
              [Description("Name of function elemenet")] String function = "IDFElement",
              [Description("Weigth associated with the function")] Double weight = 1.0,
              [Description("How IDF should be computed")] String flags = "logPlus",
              [Description("If any existing global factor should be removed")] Boolean removeExisting = false)
        {
            if (removeExisting)
            {
                data.GlobalFactors.Clear();
            }
            FeatureWeightFactor model = new FeatureWeightFactor();
            model.Settings.functionName = function;
            model.Settings.flags.AddRange(flags.SplitSmart(","));
            model.Settings.weight = weight;
            data.GlobalFactors.Add(model);
        }

        [Display(GroupName = "set", Name = "GlobalTDPWeight", ShortName = "", Description = "Configures a global function based on Term Discrimination Power")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will add specified global factor, optionally if will remove any existing global factors")]
        /// <summary>Configures a global function based on Term Discrimination Power</summary>
        /// <remarks><para>It will add specified global factor, optionally if will remove any existing global factors</para></remarks>
        /// <param name="factor">What factor should be added</param>
        /// <param name="weight">Weigth associated with the function</param>
        /// <param name="removeExisting">If any existing global factor should be removed</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setGlobalTDPWeight(
              [Description("What factor should be added")] TDPFactor factor = TDPFactor.chi,
              [Description("Weigth associated with the function")] Double weight = 1.0,
              [Description("If any existing global factor should be removed")] Boolean removeExisting = false)
        {
            if (removeExisting)
            {
                data.GlobalFactors.Clear();
            }

            FeatureWeightFactor model = new FeatureWeightFactor();
            model.Settings.functionName = nameof(CollectionTDPElement);
            model.Settings.flags.Add(factor.ToString());
            model.Settings.weight = weight;
            data.GlobalFactors.Add(model);
        }

        [Display(GroupName = "set", Name = "GlobalIGMWeight", ShortName = "", Description = "Configures a global function based on Gravity moment")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "What it will do?")]
        /// <summary>Configures a global function based on Gravity moment</summary>
        /// <remarks><para>What it will do?</para></remarks>
        /// <param name="l">"Lambda factor of IGM"</param>
        /// <param name="weight">Weigth associated with the function</param>
        /// <param name="removeExisting">If any existing global factor should be removed</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setGlobalIGMWeight(
              [Description("Lambda factor of IGM")] Double l = 7.0,
              [Description("Weigth associated with the function")] Double weight = 1.0,
              [Description("If any existing global factor should be removed")] Boolean removeExisting = false)
        {
            if (removeExisting)
            {
                data.GlobalFactors.Clear();
            }

            FeatureWeightFactor model = new FeatureWeightFactor();
            model.Settings.functionName = nameof(IGMElement);
            model.Settings.l = l;
            model.Settings.weight = weight;
            data.GlobalFactors.Add(model);
        }

        public override void SetSubBinding()
        {

        }

        public becWeightingModelEditExtension(folderNode __folder, IAceOperationSetExecutor __parent) : base(__folder, __parent)
        {

        }
    }
}