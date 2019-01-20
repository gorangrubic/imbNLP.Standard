using imbACE.Core.operations;
using imbACE.Services.consolePlugins;
using imbNLP.Toolkit.Entity;
using imbNLP.Toolkit.Planes;
using imbSCI.Core.files.folders;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace imbNLP.Project.Extensions
{


    public class becWeightingModelExtension : instanceLoadSaveExtension<CorpusPlaneMethodSettings>
    {
        private becWeightingModelEditExtension _model;

        public becWeightingModelExtension(folderNode __folder, IAceOperationSetExecutor __parent) : base(__folder, __parent)
        {
            model = new becWeightingModelEditExtension(__folder, __parent);
            SetSubBinding();
        }

        public becWeightingModelEditExtension model
        {
            get => _model;
            set
            {
                _model = value;
                OnPropertyChanged(nameof(model));
            }
        }

        [Display(GroupName = "set", Name = "Blending", ShortName = "", Description = "Configures blending operation - that glues rendered layers into final text representation")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "Sets blending options")]
        /// <summary>Configures blending operation - that glues rendered layers into final text representation</summary>
        /// <remarks><para>Sets blending options</para></remarks>
        /// <param name="options">Blending options</param>
        /// <param name="steps">--</param>
        /// <param name="debug">--</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setBlending(
        [Description("Blending options")] DocumentBlenderFunctionOptions options = DocumentBlenderFunctionOptions.siteLevel | DocumentBlenderFunctionOptions.uniqueContentUnitsOnly)
        {
            data.blender.options = options;
        }

        //public CorpusPlaneMethodSettings corpusMethod { get; set; } = new CorpusPlaneMethodSettings();

        [Display(GroupName = "set", Name = "Processing", ShortName = "", Description = "Sets stemming, tokenization and transliteration")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will place configuration values for basic language preprocessing")]
        /// <summary>Sets stemming, tokenization and transliteration</summary>
        /// <remarks><para>It will place configuration values for basic language preprocessing</para></remarks>
        /// <param name="stemmer">Class name for word stemmer to be used. Leave blank to keep current</param>
        /// <param name="tokenizer">Name of tokenizer class to be used. Leave blank to keep current.</param>
        /// <param name="transliterationRuleSetId">Transliteration ruleset id. Leave blank to keep current configuration</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setProcessing(
          [Description("Class name for word stemmer to be used. Leave blank to keep current")] String stemmer = "*",
          [Description("Name of tokenizer class to be used. Leave blank to keep current.")] String tokenizer = "*",
          [Description("Transliteration ruleset id. Leave blank to keep current configuration")] String transliterationRuleSetId = "*")
        {
            if (stemmer != "*") data.stemmer = stemmer;
            if (tokenizer != "*") data.tokenizer = tokenizer;
            if (transliterationRuleSetId != "*") data.transliterationRuleSetId = transliterationRuleSetId;
        }

        public override void SetSubBinding()
        {
            model.SetBinding(data, nameof(data.WeightModel), true);
        }
    }
}