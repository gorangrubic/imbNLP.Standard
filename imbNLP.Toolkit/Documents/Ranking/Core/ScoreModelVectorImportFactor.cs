using imbNLP.Toolkit.Classifiers.Core;
using imbNLP.Toolkit.Documents.Ranking.Data;
using imbNLP.Toolkit.Feature;
using imbNLP.Toolkit.Planes;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Weighting;
using imbNLP.Toolkit.Weighting.Data;
using imbSCI.Core.enums;
using imbSCI.Core.extensions.data;
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
    /// Score factor based on a precompiled <see cref="WeightDictionary"/>
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Documents.Ranking.Core.ScoreModelFactorBase" />
    public class ScoreModelVectorImportFactor : ScoreModelFactorBase
    {
        public override String GetSignature()
        {
            String output = "";

            if (useMachineLearning)
            {
                output = "ML";

                if (featureMethod?.classifierSettings != null)
                {
                    output += "[" + featureMethod.classifierSettings.GetSignature() + "]";
                }
                else
                {

                }

                if (TermWeightModel != null)
                {
                    output += "[" + TermWeightModel.GetSignature() + "]";
                }
                else if (!modelDefinitionFile.isNullOrEmpty())
                {
                    output += "[" + modelDefinitionFile + "]";
                }


            }
            else
            {
                output = "VI"; // computation.ToString().Replace(", ", "_");
                if (!dictionaryFile.isNullOrEmpty())
                {
                    output += "[" + dictionaryFile + "]";
                }
            }

            output += GetWeightSignature();

            return output;
        }


        public ScoreModelVectorImportFactor() { }

        [XmlIgnore]
        public String _name { get; set; } = "";

        public override string name
        {
            get
            {
                if (_name.isNullOrEmpty())
                {
                    String output = dictionaryFile + weight.ToString("F2");
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
        /// Feature Vector dictionary root filename
        /// </summary>
        /// <value>
        /// The dictionary file.
        /// </value>
        public String dictionaryFile { get; set; } = "";




        public override ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null)
        {
            if (requirements == null) requirements = new ScoreModelRequirements();
            requirements.MayUseSelectedFeatures = true;
            requirements.MayUseTextRender = true;
            requirements.MayUseResourceProvider = true;
            return requirements;
        }


        protected FeatureVectorDictionaryWithDimensions scoreDictionary { get; set; }

        public operation vectorCompression { get; set; } = imbSCI.Core.enums.operation.max;


        public Boolean useMachineLearning { get; set; } = true;
        public Double criterion { get; set; } = 0.5;


        public String modelDefinitionFile { get; set; } = "";




        /// <summary>
        /// Gets or sets the term weight model.
        /// </summary>
        /// <value>
        /// The term weight model.
        /// </value>
        public FeatureWeightModel TermWeightModel { get; set; } = new FeatureWeightModel();

        [XmlIgnore]
        protected WeightDictionary SelectedTerms { get; set; } = new WeightDictionary();

        [XmlIgnore]
        public List<String> queryTerms { get; set; } = new List<string>();

        public Boolean useStoredData { get; set; } = false;
        public Boolean useSelectedFeatures { get; set; } = true;

        public FeaturePlaneMethodSettings featureMethod = new FeaturePlaneMethodSettings();





        /// <summary>
        /// Gets or sets the classifier.
        /// </summary>
        /// <value>
        /// The classifier.
        /// </value>
        [XmlIgnore]
        public IClassifier classifier { get; set; }

        protected FeatureVectorConstructor fvConstructor { get; set; } = new FeatureVectorConstructor();

        protected Dictionary<string, int> sc_id { get; set; }

        /// <summary>
        /// Prepares the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        /// <exception cref="ArgumentException">context</exception>
        public override void Prepare(DocumentSelectResult context, ILogBuilder log)
        {



            //context.folder.GetOrFindFiles("*", dictionaryFile + "*.xml");

            scoreDictionary = FeatureVectorDictionaryWithDimensions.LoadFile(context.folder, dictionaryFile, log); // WeightDictionary.LoadFile(WeightDictionary.GetDictionaryFilename(dictionaryFile, context.folder), log);

            if (scoreDictionary == null)
            {
                String msg = "Error: Failed to find score dictionary [" + dictionaryFile + "] in " + context.folder.path;
                throw new ArgumentException(msg, nameof(context));
            }

            if (useMachineLearning)
            {


                #region --------------- PREPARING TERM WEIGHT MODEL


                String p_m = FeatureWeightModel.GetModelDefinitionFilename(modelDefinitionFile, context.folder);
                String p_d = FeatureWeightModel.GetModelDataFilename(modelDefinitionFile, context.folder);


                if (TermWeightModel == null)
                {
                    TermWeightModel = FeatureWeightModel.LoadModel(p_m, log);
                }


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

                    TermWeightModel.PrepareTheModel(context.spaceModel,log);
                }

                if (SelectedTerms.Count == 0)
                {
                    SelectedTerms = context.selectedFeatures;
                }
                List<String> sel_tkns = new List<String>();

                sel_tkns.AddRange(SelectedTerms.index.Values.Select(x => x.name));

                if (!sel_tkns.Any())
                {
                    sel_tkns.AddRange(context.spaceModel.terms_known_label.GetTokens());

                }


                #endregion

                fvConstructor.Deploy(featureMethod.constructor, sel_tkns);



                classifier = featureMethod.classifierSettings.GetClassifier();

                sc_id = scoreDictionary.GetVectorsWithLabelID(null, criterion).ToNameVsLabelID();


                List<FeatureVectorWithLabelID> trainingSet = new List<FeatureVectorWithLabelID>();
                foreach (var item in context.items)
                {
                    if (sc_id.ContainsKey(item.AssignedID))
                    {

                        WeightDictionary dc_vec = TermWeightModel.GetWeights(sel_tkns, item.spaceDocument, context.spaceModel);


                        var n_vec = fvConstructor.ConstructFeatureVector(dc_vec, item.AssignedID);

                        FeatureVectorWithLabelID id_vec = new FeatureVectorWithLabelID(n_vec, sc_id[item.AssignedID]);

                        trainingSet.Add(id_vec);

                    }
                }


                log.log("Training [" + classifier.name + "] with [" + sc_id.Count + "] feature vectors.");
                classifier.DoTraining(trainingSet, log);



            }

        }

        /// <summary>
        /// Scores the specified entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public override double Score(DocumentSelectResultEntry entry, DocumentSelectResult context, ILogBuilder log)
        {
            if (useMachineLearning)
            {
                WeightDictionary dc_vec = TermWeightModel.GetWeights(SelectedTerms.GetKeys(), entry.spaceDocument, context.spaceModel);

                var n_vec = fvConstructor.ConstructFeatureVector(dc_vec, entry.AssignedID);


                Double score = 0;
                Int32 l_id = -1;
                if (sc_id.ContainsKey(entry.AssignedID))
                {
                    l_id = sc_id[entry.AssignedID];
                }

                score = classifier.DoScore(n_vec, log, l_id);

                return score;
            }
            else
            {
                if (scoreDictionary.ContainsKey(entry.AssignedID))
                {
                    var fv = scoreDictionary[entry.AssignedID];
                    return fv.CompressNumericVector(vectorCompression);
                }
                else
                {
                    return 0;
                }
            }


        }
    }
}