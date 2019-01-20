using imbNLP.Toolkit.Documents.Ranking.Data;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Weighting;
using imbNLP.Toolkit.Weighting.Data;
using imbSCI.Core.extensions.text;
using imbSCI.Core.files;
using imbSCI.Core.reporting;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Documents.Ranking.Core
{

    /// <summary>
    /// Score factor based on a TermWeight model. If term query is defined: it will compute score as sum of matched terms. If there is no term query set, it will return total weight of the document
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Documents.Ranking.Core.ScoreModelFactorBase" />
    public class ScoreModelTermWeightFactor : ScoreModelFactorBase
    {
        public override String GetSignature()
        {
            String output = "TW";

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


        public ScoreModelTermWeightFactor()
        {

        }


        public String modelDefinitionFile { get; set; } = "";


        [XmlIgnore]
        public String _name { get; set; } = "";

        public override string name
        {
            get
            {
                if (_name.isNullOrEmpty())
                {
                    String output = modelDefinitionFile + weight.ToString("F2");
                    return output;
                }
                return _name;
            }
            set
            {

                _name = value;
            }
        }




        /// <summary>
        /// Gets or sets the term weight model.
        /// </summary>
        /// <value>
        /// The term weight model.
        /// </value>
        [XmlIgnore]
        public FeatureWeightModel TermWeightModel { get; set; } = new FeatureWeightModel();

        [XmlIgnore]
        protected WeightDictionary SelectedTerms { get; set; } = new WeightDictionary();

        [XmlIgnore]
        public List<String> queryTerms { get; set; } = new List<string>();

        public Boolean useStoredData { get; set; } = true;
        public Boolean useSelectedFeatures { get; set; } = true;



        public override ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null)
        {
            if (requirements == null) requirements = new ScoreModelRequirements();
            requirements.MayUseSelectedFeatures = true;
            requirements.MayUseTextRender = true;
            return requirements;
        }


        /// <summary>
        /// Prepares the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        /// <exception cref="ArgumentException">context</exception>
        public override void Prepare(DocumentSelectResult context, ILogBuilder log)
        {
            String p_m = FeatureWeightModel.GetModelDefinitionFilename(modelDefinitionFile, context.folder);
            String p_d = FeatureWeightModel.GetModelDataFilename(modelDefinitionFile, context.folder);

            TermWeightModel = FeatureWeightModel.LoadModel(p_m, log);

            //if (File.Exists(p_m))
            //{

            //    //TermWeightModel = objectSerialization.loadObjectFromXML<FeatureWeightModel>(p_m, log);
            //}

            TermWeightModel.Deploy(log);

            if (context.spaceModel == null)
            {
                String msg = "Error: TermWeight factor requires SpaceModel declared in the context for operation";
                throw new ArgumentException(msg, nameof(context));
            }

            if (File.Exists(p_d) && useStoredData)
            {
                WeightingModelDataSet data = objectSerialization.loadObjectFromXML<WeightingModelDataSet>(p_d, log);
                TermWeightModel.LoadModelDataSet(data, log);

                if (useSelectedFeatures)
                {
                    SelectedTerms = WeightDictionary.LoadFile(WeightDictionary.GetDictionaryFilename(modelDefinitionFile + "_sf", context.folder), log);
                }
            }
            else
            {

                TermWeightModel.PrepareTheModel(context.spaceModel, log);
            }

            if (context.query.isNullOrEmpty())
            {
                context.query.QueryTerms = context.query.QueryTerms.Trim();

                List<String> tkns = context.query.QueryTerms.getTokens(true, true, true, false, 4);

                foreach (String tkn in tkns)
                {
                    queryTerms.Add(context.stemmingContext.Stem(tkn));
                }
            }


        }

        /// <summary>
        /// Computes score for given entry
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public override double Score(DocumentSelectResultEntry entry, DocumentSelectResult context, ILogBuilder log)
        {
            Double output = 0;

            foreach (String term in entry.spaceDocument.terms.GetTokens())
            {
                Boolean isOk = true;
                if (context.selectedFeatures != null)
                {
                    if (context.selectedFeatures.Count > 0)
                    {
                        if (!context.selectedFeatures.ContainsKey(term))
                        {
                            isOk = false;
                        }
                    }
                }

                if (isOk && SelectedTerms != null)
                {
                    if (SelectedTerms.Count > 0)
                    {
                        if (!SelectedTerms.ContainsKey(term))
                        {
                            isOk = false;
                        }
                    }
                }

                if (isOk)
                {
                    if (queryTerms.Any())
                    {
                        if (queryTerms.Contains(term))
                        {
                            output += TermWeightModel.GetWeight(term, entry.spaceDocument, context.spaceModel);
                        }
                    }
                    else
                    {
                        output += TermWeightModel.GetWeight(term, entry.spaceDocument, context.spaceModel);
                    }
                }
            }



            return output;
        }
    }
}