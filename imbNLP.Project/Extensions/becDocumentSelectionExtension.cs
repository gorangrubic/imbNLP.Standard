using imbACE.Core.operations;
using imbACE.Services.consolePlugins;
using imbNLP.Project.Operations.Setups;
using imbNLP.Toolkit.Documents.Ranking.Core;
using imbNLP.Toolkit.Documents.Ranking.Data;
using imbSCI.Core.enums;
using imbSCI.Core.files.folders;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace imbNLP.Project.Extensions
{

    public class becDocumentSelectionExtension : instanceLoadSaveExtension<SetupDocumentSelection>
    {
        private becWeightingModelExtension _weight;

        public becWeightingModelExtension weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        private becDocumentRenderingExtension _rendering;


        public becDocumentRenderingExtension rendering
        {
            get { return _rendering; }
            set { _rendering = value; }
        }
        private becFeatureVectorExtension _features;
        public becFeatureVectorExtension features
        {
            get { return _features; }
            set { _features = value; }
        }

        public override void SetSubBinding()
        {
            _rendering.SetBinding(data, nameof(data.renderForEvaluation), true);
            _weight.SetBinding(data, nameof(data.corpusForEvaluation), true);
            _features.SetBinding(data, nameof(data.featureMethod), true);
        }


        //public DocumentRankingMethod data { get; set; } = new DocumentRankingMethod();

        public becDocumentSelectionExtension(folderNode __folder, IAceOperationSetExecutor __parent) : base(__folder, __parent)
        {
            //_weighting = new becWeightingModelExtension(__folder.Add(", this);
            _rendering = new becDocumentRenderingExtension(this, __folder);


            _weight = new becWeightingModelExtension(__folder, this);


            _features = new becFeatureVectorExtension(__folder, __parent);
            SetSubBinding();

        }

        [Display(GroupName = "set", Name = "DocumentSelectionQuery", ShortName = "", Description = "Defines document selection query parameters")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will set the parameters to the current DSQ instance. For query text to be used you should have at least one TermWeight factor in the DS ScoreModel")]
        /// <summary>Defines document selection query parameters</summary>
        /// <remarks><para>It will set the parameters to the current DSQ instance. For query text to be used you should have at least one TermWeight factor in the DS ScoreModel</para></remarks>
        /// <param name="docLimit">Number of documents to be selected</param>
        /// <param name="trashold">Score min limit, leave 0 to disable score limit</param>
        /// <param name="query">Text of the query to be used for document score operation</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setDocumentSelectionQuery(
              [Description("Number of documents to be selected")] Int32 docLimit = 3,
              [Description("Score min limit, leave 0 to disable score limit")] Double trashold = 0.0,
              [Description("Text of the query to be used for document score operation")] String query = " ",
              [Description("Result is limited to [docLimit] by domain level, not in total")] DocumentSelectQueryOptions options = DocumentSelectQueryOptions.ApplyDomainLevelLimits)
        {

            data.ranking.query = new DocumentSelectQuery();
            data.ranking.query.QueryTerms = query;
            data.ranking.query.SizeLimit = docLimit;
            data.ranking.query.TrasholdLimit = trashold;
            data.ranking.query.options = options;
        }




        [Display(GroupName = "set", Name = "VectorImportFactor", ShortName = "", Description = "Applies precompiled feature vector dictionary to documents, by AssignID")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "What it will do?")]
        /// <summary>Applies precompiled feature vector dictionary to documents, by AssignID</summary> 
        /// <remarks><para>What it will do?</para></remarks>
        /// <param name="word">--</param>
        /// <param name="steps">--</param>
        /// <param name="debug">--</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setVectorImportFactor(
            [Description("If true it will remove any existing ScoreModel Factors")] Boolean remove = true,
               [Description("Model definition name, leave * to use current weighting model from docSelection setup")] String dictionaryName = "*",
                       [Description("How to compress multidimensional feature fector into single dimension")]
                      operation computation = operation.max,
                      [Description("Weight associated with the term weight based document score factor")] Double weight = 1.0,
                      [Description("Normalize score value on range from 0.0 to 1.0, across the sample")] Boolean normalize = true,
                      Boolean useML = true,
                      String modelName = "*")
        {

            if (remove)
            {
                data.ranking.model.Factors.Clear();
                data.ranking.model.SerializedFactors.Clear();
            }

            ScoreModelVectorImportFactor gf = new ScoreModelVectorImportFactor();
            gf.weight = weight;
            gf.doNormalize = normalize;
            gf.vectorCompression = computation;

            gf.dictionaryFile = dictionaryName;
            gf.useMachineLearning = useML;
            if (useML)
            {
                gf.featureMethod = features.data;
                gf.modelDefinitionFile = modelName;

                if (modelName == "*")
                {
                    gf.TermWeightModel = data.corpusForEvaluation.WeightModel;
                }
            }

            data.ranking.model.Factors.Add(gf);
        }





        [Display(GroupName = "set", Name = "AddCategorySimilarityFactor", ShortName = "", Description = "Adds category similarity factor to the document ScoreModel")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will add TermWeight based, similarity score factor into document score computation model")]
        /// <summary>
        /// Adds category similarity factor to the document ScoreModel
        /// </summary>
        /// <param name="remove">if set to <c>true</c> [remove].</param>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="computation">The computation.</param>
        /// <param name="weight">The weight.</param>
        /// <param name="normalize">if set to <c>true</c> [normalize].</param>
        /// <remarks>
        /// It will add TermWeight based, similarity score factor into document score computation model
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase" />
        public void aceOperation_setAddSimilarityFactor(
             [Description("If true it will remove any existing ScoreModel Factors")] Boolean remove = true,
                      [Description("Model definition name, leave * to use current weighting model from docSelection setup")] String modelName = "*",
                       [Description("Computation mode")]
                      ScoreComputationModeEnum computation = ScoreComputationModeEnum.none,
                      [Description("Weight associated with the term weight based document score factor")] Double weight = 1.0,
                      [Description("Normalize score value on range from 0.0 to 1.0, across the sample")] Boolean normalize = true)
        {
            if (remove)
            {
                data.ranking.model.Factors.Clear();
                data.ranking.model.SerializedFactors.Clear();
            }

            ScoreCategorySimilarity gf = new ScoreCategorySimilarity();
            gf.weight = weight;
            gf.computation = computation;
            gf.doNormalize = normalize;
            gf.modelDefinitionFile = modelName;

            if (modelName == "*")
            {
                gf.TermWeightModel = data.corpusForEvaluation.WeightModel;
            }

            data.ranking.model.Factors.Add(gf);

        }






        /// <summary>
        /// Aces the operation set document selection add graph factor.
        /// </summary>
        /// <param name="remove">if set to <c>true</c> [remove].</param>
        /// <param name="flags">The flags.</param>
        /// <param name="weight">The weight.</param>
        /// <param name="normalize">if set to <c>true</c> [normalize].</param>
        [Display(GroupName = "set", Name = "DocumentSelectionAddGraphFactor", ShortName = "", Description = "Exploits web graph metrics to compute document score factor")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will add graph based factor into document score computation model")]
        /// <summary>Exploits web graph metrics to compute document score factor</summary>
        /// <remarks><para>What it will do?</para></remarks>
        /// <param name="remove">If true it will remove any existing ScoreModel Factors</param>
        /// <param name="flags">Flags that control how graph based score is computed</param>
        /// <param name="weight">Weight associated with the term weight based document score factor</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setDocumentSelectionAddGraphFactor(
                      [Description("If true it will remove any existing ScoreModel Factors")] Boolean remove = true,
                      [Description("Flags that control how graph based score is computed")] GraphFactorFunctionEnum flags = GraphFactorFunctionEnum.count_inbound | GraphFactorFunctionEnum.count_outbound | GraphFactorFunctionEnum.divide_by_graphnodes,
                      [Description("Weight associated with the term weight based document score factor")] Double weight = 1.0,
                      [Description("Normalize score value on range from 0.0 to 1.0, across the sample")] Boolean normalize = true
                      )
        {
            if (remove)
            {
                data.ranking.model.Factors.Clear();
                data.ranking.model.SerializedFactors.Clear();
            }

            ScoreModelGraphFactor gf = new ScoreModelGraphFactor();
            gf.weight = weight;
            gf.functionFlags = flags;
            gf.doNormalize = normalize;

            data.ranking.model.Factors.Add(gf);
        }

        [Display(GroupName = "set", Name = "DocumentSelectionAddTWFactor", ShortName = "", Description = "Defines dataset filter, based on current FeatureWeightFactor model")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will set a ScoreModel factor for the DataSet filtration function")]
        /// <summary>
        /// Defines dataset filter, based on current FeatureWeightFactor model
        /// </summary>
        /// <param name="remove">If true it will remove any existing ScoreModel Factors</param>
        /// <param name="weight">Weight associated with the term weight based document score factor</param>
        /// <remarks>
        /// It will copy current FeatureWeight model as a ScoreModel factor for the DataSet filtration function
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase" />
        public void aceOperation_setDocumentSelectionAddTWFactor(
              [Description("If true it will remove any existing ScoreModel Factors")] Boolean remove = false,
              [Description("Model definition name")] String modelName = "*",
              [Description("Weight associated with the term weight based document score factor")] Double weight = 1.0,
              [Description("Normalize score value on range from 0.0 to 1.0, across the sample")] Boolean normalize = true

            )
        {
            if (remove)
            {
                data.ranking.model.Factors.Clear();
                data.ranking.model.SerializedFactors.Clear();
            }

            ScoreModelTermWeightFactor twf = new ScoreModelTermWeightFactor();
            twf.modelDefinitionFile = modelName;

            // twf.TermWeightModel = weighting.data.weightModel; // toolkitSettings.corpusMethod.weightModel;
            twf.weight = weight;
            twf.doNormalize = normalize;

            data.ranking.model.Factors.Add(twf);
        }







        [Display(GroupName = "set", Name = "AddConnectivityFactor", ShortName = "", Description = "Implements HITS, PageRank and similar connectivity analysis ranking")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "What it will do?")]
        /// <summary>Implements HITS, PageRank and similar connectivity analysis ranking</summary> 
        /// <remarks><para>What it will do?</para></remarks>
        /// <param name="word">--</param>
        /// <param name="steps">--</param>
        /// <param name="debug">--</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setAddConnectivityFactor(
              [Description("If true it will remove any existing ScoreModel Factors")] Boolean remove = false,
              [Description("Algorithm to use")] GraphFactorAlgorithm algorithm = GraphFactorAlgorithm.PageRank,
              [Description("Alpha factor")] Double alpha = 0.065,
              [Description("Convergence trigger")] Double convergence = 0.0001,
              [Description("Weight associated with the term weight based document score factor")] Double weight = 1.0,
              [Description("Normalize score value on range from 0.0 to 1.0, across the sample")] Boolean normalize = true,
              [Description("Weight associated with the term weight based document score factor")] Int32 steps = 200,
              [Description("Weight associated with the term weight based document score factor")] Int32 scoreUnit = 100)
        {

            if (remove)
            {
                data.ranking.model.Factors.Clear();
                data.ranking.model.SerializedFactors.Clear();
            }

            ScoreModelConnectivityFactor scs = new ScoreModelConnectivityFactor();
            scs.algorithm = algorithm;
            scs.alpha = alpha;
            scs.weight = weight;
            scs.convergence = convergence;
            scs.steps = steps;
            scs.scoreUnit = scoreUnit;
            scs.doNormalize = normalize;
            data.ranking.model.Factors.Add(scs);

        }




        [Display(GroupName = "set", Name = "AddWeightTableFactor", ShortName = "", Description = "Defines term weight table factor, based on precompiled WeightDictionary")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will create new factor instance. Optionally, it will remove any existing factors from the score model")]
        /// <summary>Defines term weight table factor, based on precompiled WeightDictionary</summary> 
        /// <remarks><para>It will create new factor instance. Optionally, it will remove any existing factors from the score model</para></remarks>
        /// <param name="word">--</param>
        /// <param name="steps">--</param>
        /// <param name="debug">--</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setAddWeightTableFactor(
              [Description("If true it will remove any existing ScoreModel Factors")] Boolean remove = false,
              [Description("WeightDictionary name")] String dictionaryName = "",
              [Description("Weight associated with the term weight based document score factor")] Double weight = 1.0,
              [Description("Normalize score value on range from 0.0 to 1.0, across the sample")] Boolean normalize = true)
        {
            if (remove)
            {
                data.ranking.model.Factors.Clear();
                data.ranking.model.SerializedFactors.Clear();
            }

            ScoreModelWeightTableFactor twf = new ScoreModelWeightTableFactor();
            twf.dictionaryFile = dictionaryName;

            twf.weight = weight;
            twf.doNormalize = normalize;

            data.ranking.model.Factors.Add(twf);
        }


        /// <summary>
        /// Aces the operation set document selection add metric factor.
        /// </summary>
        /// <param name="remove">The remove.</param>
        /// <param name="metric">The metric.</param>
        /// <param name="weight">The weight.</param>
        /// <param name="normalize">if set to <c>true</c> [normalize].</param>
        [Display(GroupName = "set", Name = "DocumentSelectionAddMetricFactor", ShortName = "", Description = "Computes score factor from basic content metrics")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "What it will do?")]
        /// <summary>Computes score factor from basic content metrics</summary>
        /// <remarks><para>What it will do?</para></remarks>
        /// <param name="remove">If true it will remove any existing ScoreModel Factors</param>
        /// <param name="metric">Kind of content metric to be consumed</param>
        /// <param name="weight">Weight associated with the term weight based document score factor</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setDocumentSelectionAddMetricFactor(
                      [Description("If true it will remove any existing ScoreModel Factors")] Boolean remove = true,
                      [Description("Kind of content metric to be consumed")] ScoreModelMetricFactorEnum metric = ScoreModelMetricFactorEnum.Count,
                      [Description("Weight associated with the term weight based document score factor")] Double weight = 1.0,
                      [Description("Normalize score value on range from 0.0 to 1.0, across the sample")] Boolean normalize = true)
        {
            if (remove)
            {
                data.ranking.model.Factors.Clear();
                data.ranking.model.SerializedFactors.Clear();
            }

            ScoreModelMetricFactor mf = new ScoreModelMetricFactor();

            mf.weight = weight;
            mf.functionName = metric; //.functionFlags = flags;
            mf.doNormalize = normalize;

            data.ranking.model.Factors.Add(mf);
        }
    }
}