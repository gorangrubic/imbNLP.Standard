using imbNLP.Toolkit.Documents.Ranking.Data;
using imbNLP.Toolkit.Feature;
using imbNLP.Toolkit.Functions;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Weighting;
using imbNLP.Toolkit.Weighting.Data;
using imbSCI.Core.files;
using imbSCI.Core.math;
using imbSCI.Core.reporting;
using imbSCI.Data;
using System;
using System.IO;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Documents.Ranking.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Documents.Ranking.Core.ScoreModelFactorBase" />
    public class ScoreCategorySimilarity : ScoreModelFactorBase
    {

        public override String GetSignature()
        {
            String output = computation.ToString().Replace(", ", "_");

            if (TermWeightModel != null)
            {
                output += "[" + TermWeightModel.GetSignature() + "]";
            }
            else if (!modelDefinitionFile.isNullOrEmpty())
            {
                output += "[" + modelDefinitionFile + "]";
            }

            output += GetWeightSignature();

            return output;
        }

        public ScoreCategorySimilarity()
        {

        }

        [XmlIgnore]
        public String _name { get; set; } = "";

        public override string name
        {
            get
            {
                if (_name.isNullOrEmpty())
                {
                    String output = GetType().Name + weight.ToString("F2");
                    return output;
                }
                return _name;
            }
            set
            {

                _name = value;
            }
        }

        public String modelDefinitionFile { get; set; } = "";

        public Boolean UseModelData { get; set; } = false;

        public ScoreComputationModeEnum computation { get; set; }


        /// <summary>
        /// Gets or sets the term weight model.
        /// </summary>
        /// <value>
        /// The term weight model.
        /// </value>
        public FeatureWeightModel TermWeightModel { get; set; } = new FeatureWeightModel();


        protected CosineSimilarityFunction function { get; set; } = new CosineSimilarityFunction();




        protected FeatureVectorSetDictionary vectorDictionary { get; set; } = new FeatureVectorSetDictionary();

        public override ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null)
        {
            if (requirements == null) requirements = new ScoreModelRequirements();
            requirements.MayUseSelectedFeatures = true;
            requirements.MayUseTextRender = true;
            requirements.MayUseSpaceModel = true;
            requirements.MayUseSpaceModelCategories = true;
            return requirements;
        }

        public override void Prepare(DocumentSelectResult context, ILogBuilder log)
        {


            String p_m = "";

            String p_d = "";

            modelDefinitionFile = modelDefinitionFile.Replace("*", "");

            if (!modelDefinitionFile.isNullOrEmpty())
            {
                p_m = FeatureWeightModel.GetModelDefinitionFilename(modelDefinitionFile, context.folder);
                p_d = FeatureWeightModel.GetModelDataFilename(modelDefinitionFile, context.folder);
            }

            if (TermWeightModel == null)
            {

                log.log("Loading model from [" + p_m + "]");

                if (File.Exists(p_m))
                {
                    TermWeightModel = FeatureWeightModel.LoadModel(p_m, log);
                }



            }

            TermWeightModel.Deploy(log);

            if (File.Exists(p_d) && UseModelData)
            {
                log.log("Loading model data from [" + p_d + "]");

                var dataset = objectSerialization.loadObjectFromXML<WeightingModelDataSet>(p_d, log);

                //  WeightingModelDataSet
                TermWeightModel.LoadModelDataSet(dataset, log);
            }
            else
            {
                log.log("Preparing model ...");
                TermWeightModel.PrepareTheModel(context.spaceModel, log);
            }


            if (computation.HasFlag(ScoreComputationModeEnum.category))
            {
                vectorDictionary = context.TransformToFVDictionaryAsCategorySimilarity(TermWeightModel, function, log);
            }
            else if (computation.HasFlag(ScoreComputationModeEnum.site))
            {
                vectorDictionary = context.TransformToFVDictionaryAsSiteSimilarity(TermWeightModel, function, log);
            }
            else if (computation.HasFlag(ScoreComputationModeEnum.pageDivergence))
            {
                vectorDictionary = context.TransformToFVDictionaryAsPageSimilarity(TermWeightModel, function, ScoreComputationModeEnum.site, log);
            }
            else if (computation.HasFlag(ScoreComputationModeEnum.pagesOfCategory))
            {
                vectorDictionary = context.TransformToFVDictionaryAsPageSimilarity(TermWeightModel, function, ScoreComputationModeEnum.category, log);
            }
            else if (computation.HasFlag(ScoreComputationModeEnum.pagesOfDataset))
            {
                vectorDictionary = context.TransformToFVDictionaryAsPageSimilarity(TermWeightModel, function, ScoreComputationModeEnum.dataset, log);
            }






            log.log("Category similarity ready ... [" + computation.ToString() + "]");


        }

        public override double Score(DocumentSelectResultEntry entry, DocumentSelectResult context, ILogBuilder log)
        {

            FeatureVectorWithLabelID fv = vectorDictionary.Get(entry.DomainID, entry.AssignedID);
            if (fv == null)
            {
                log.log("Can't find vector dictionary entry for [" + entry.DomainID + "]>[" + entry.AssignedID + "]");
                return 0;
            }
            Double sc = 0;

            if (computation.HasFlag(ScoreComputationModeEnum.offset))
            {
                sc = fv.CompressByTrueDimension(fv.labelID);
            }
            else if (computation.HasFlag(ScoreComputationModeEnum.variance))
            {
                sc = fv.dimensions.GetVarianceCoefficient();
            }
            else if (computation.HasFlag(ScoreComputationModeEnum.distance))
            {
                sc = fv.CompressNumericVector(imbSCI.Core.enums.operation.max);
            }
            else
            {
                sc = fv.dimensions[0];
            }

            if (computation.HasFlag(ScoreComputationModeEnum.inverse))
            {
                sc = -sc;
            }

            if (sc == Double.NaN)
            {
                sc = 0;
            }
            else
            {

            }

            return sc;
        }
    }
}