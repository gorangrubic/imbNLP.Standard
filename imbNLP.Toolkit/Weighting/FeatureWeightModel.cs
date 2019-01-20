using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Weighting.Data;
using imbNLP.Toolkit.Weighting.Global;
using imbNLP.Toolkit.Weighting.Local;
using imbSCI.Core.extensions.io;
using imbSCI.Core.files;
using imbSCI.Core.files.folders;
using imbSCI.Core.reporting;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Weighting
{

    /// <summary>
    /// Model for weight computation
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Weighting.Global.IGlobalElement" />
    public class FeatureWeightModel : IDescribe, IGlobalElement
    {

        /// <summary>
        /// Gets or sets a value indicating whether the whitelistterms parameter is used for <see cref="GetWeights(List{string}, SpaceDocumentModel, SpaceModel, SpaceLabel)"/> calls
        /// </summary>
        /// <value>
        ///   <c>true</c> if [kerneloption use whitelistterms]; otherwise, <c>false</c>.
        /// </value>
        public static Boolean KERNELOPTION_USE_WHITELISTTERMS { get; set; } = false;



        /// <summary>
        /// Gets or sets the short name.
        /// </summary>
        /// <value>
        /// The short name.
        /// </value>
        public String shortName { get; set; } = "";


        /// <summary>
        /// Loads the model.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public static FeatureWeightModel LoadModel(String path, ILogBuilder log)
        {

            if (File.Exists(path))
            {
                FeatureWeightModel TermWeightModel = objectSerialization.loadObjectFromXML<FeatureWeightModel>(path, log);

                TermWeightModel.LocalFunction.shortName = TermFrequencyFunction.GetFunctionName(TermWeightModel.LocalFunction.computation);

                return TermWeightModel;
            }
            else
            {
                return null;
            }

        }


        /// <summary>
        /// Deploys this instance.
        /// </summary>
        public void Deploy(ILogBuilder logger)
        {

            foreach (FeatureWeightFactor f in GlobalFactors)
            {
                f.Deploy(logger);
            }
        }

        /// <summary>
        /// Gets the signature.
        /// </summary>
        /// <returns></returns>
        public String GetSignature()
        {
            String output = LocalFunction.GetSignature();
            foreach (FeatureWeightFactor gf in GlobalFactors)
            {
                output = output.add(gf.Settings.GetSignature(), "-");
            }
            return output;
        }

        /// <summary>
        /// Prepares the model.
        /// </summary>
        /// <param name="space">The space.</param>
        public void PrepareTheModel(SpaceModel space, ILogBuilder log)
        {
            foreach (FeatureWeightFactor gf in GlobalFactors)
            {
                gf.GlobalFunction.PrepareTheModel(space, log);
                if (gf.GlobalFunction.resultType == FunctionResultTypeEnum.numericVectorForMultiClass)
                {
                    nDimensions = space.labels.Count;
                    resultType = gf.GlobalFunction.resultType;
                }
            }

            LocalFunction.PrepareTheModel(space, log);

            if (!GlobalFactors.Any())
            {
                log.log("WARNING: NO GLOBAL FACTORS DEFINED AT FEATURE WEIGHT MODEL");
            }
        }

        public Int32 nDimensions { get; set; } = 1;


        /// <summary>
        /// Constructs global weight fictionary using global elements
        /// </summary>
        /// <param name="terms">The terms.</param>
        /// <param name="space">The space.</param>
        /// <param name="label">The label.</param>
        /// <returns></returns>
        public WeightDictionary GetElementFactors(IEnumerable<string> terms, SpaceModel space, SpaceLabel label = null)
        {
            var output = new WeightDictionary();


            output.name = GetSignature() + "_globalOnly";

            foreach (String term in terms)
            {
                Double score = GetElementFactor(term, space, label);
                WeightDictionaryEntry entry = new WeightDictionaryEntry(term, score);

                output.AddEntry(entry, true);
            }

            output.description = "Global weights for [" + output.Count + "] terms.";

            return output;
        }

        public WeightDictionaryEntry GetElementFactorEntry(string term, SpaceModel space, SpaceLabel label = null)
        {
            Double score = GetElementFactor(term, space, label);
            WeightDictionaryEntry entry = new WeightDictionaryEntry(term, score);
            return entry;
        }


        public WeightDictionaryEntry GetCompositeEntry(String term, SpaceDocumentModel document, SpaceModel space)
        {
            WeightDictionaryEntry output = new WeightDictionaryEntry(term, 0);
            List<Double> dimensions = new List<double>();

            dimensions.Add(LocalFunction.GetElementFactor(term, document));

            foreach (var gf in GlobalFactors)
            {
                dimensions.Add(gf.GlobalFunction.GetElementFactor(term, space) * gf.weight);
            }

            output.dimensions = dimensions.ToArray();

            return output;

        }



        public static String GetModelDefinitionFilename(String outputfilename, folderNode folder)
        {
            String fn = outputfilename;
            String p_m = folder.pathFor(fn.ensureEndsWith("_model.xml"), imbSCI.Data.enums.getWritableFileMode.none);
            return p_m;

        }

        public static String GetModelDataFilename(String outputfilename, folderNode folder)
        {
            String fn = outputfilename;
            String p_m = folder.pathFor(fn.ensureEndsWith("_data.xml"), imbSCI.Data.enums.getWritableFileMode.none);
            return p_m;

        }

        public static String GetSelectedFeaturesDataFilename(String outputfilename, folderNode folder)
        {
            String fn = outputfilename;
            String p_m = folder.pathFor(fn.ensureEndsWith("_selected.xml"), imbSCI.Data.enums.getWritableFileMode.none);
            return p_m;

        }

        /// <summary>
        /// Gets the weights.
        /// </summary>
        /// <param name="termWhiteList">The term white list.</param>
        /// <param name="document">The document.</param>
        /// <param name="space">The space.</param>
        /// <param name="label">The label.</param>
        /// <returns></returns>
        public WeightDictionary GetWeights(List<String> termWhiteList, SpaceDocumentModel document, SpaceModel space, SpaceLabel label = null)
        {
            WeightDictionary output = new WeightDictionary();
            output.name = GetSignature() + "_" + document.name;
            output.description = "Feature weight table constructed by [" + GetSignature() + "] for features [" + termWhiteList.Count + "] in document [" + document.name + "]";
            output.nDimensions = nDimensions;

            if (KERNELOPTION_USE_WHITELISTTERMS)
            {
                foreach (String term in termWhiteList)
                {
                    if (document.terms.Contains(term))
                    {

                        throw new NotImplementedException();
                        //output.entries.Add(entry);
                    }
                }
            }
            else
            {
                List<String> terms = document.terms.GetTokens();

                for (int i = 0; i < document.terms.Count; i++)
                {
                    String term = terms[i];

                    WeightDictionaryEntry entry = new WeightDictionaryEntry(term, 0);


                    entry = LocalFunction.GetElementFactorEntry(term, document);

                    foreach (FeatureWeightFactor gf in GlobalFactors)
                    {
                        entry = entry * (gf.GlobalFunction.GetElementFactorEntry(term, space, label) * gf.weight);
                    }

                    if (document.weight != 1)
                    {
                        entry = entry * document.weight;
                    }

                    output.Merge(entry);
                    //output.AddEntry(term, entry.dimensions, false);
                }

            }

            return output;
        }

        public Double GetWeight(String term, SpaceDocumentModel document, SpaceModel space, SpaceLabel label = null)
        {
            return GetElementFactor(term, document) * GetElementFactor(term, space, label);
        }

        /// <summary>
        /// Gets the local element factor
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        public double GetElementFactor(String term, SpaceDocumentModel document)
        {
            if (LocalFunction != null)
            {
                if (LocalFunction.IsEnabled)
                {
                    Double TF = LocalFunction.GetElementFactor(term, document);

                    return TF;
                }
            }
            return 1;
        }

        /// <summary>
        /// Gets the product of global element factors
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="space">The space.</param>
        /// <param name="label">The label.</param>
        /// <returns></returns>
        public double GetElementFactor(string term, SpaceModel space, SpaceLabel label = null)
        {

            Double GF = 1;

            foreach (FeatureWeightFactor gf in GlobalFactors)
            {
                GF = GF * (gf.GlobalFunction.GetElementFactor(term, space, label) * gf.weight);
            }

            //if (Double.IsInfinity(GF))
            //{

            //}

            return GF;
        }


        /// <summary>
        /// Saves the specified output filename.
        /// </summary>
        /// <param name="outputFilename">The output filename.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="logger">The logger.</param>
        public void Save(String outputFilename, folderNode folder, ILogBuilder logger)
        {

            String p_m = FeatureWeightModel.GetModelDefinitionFilename(outputFilename, folder);

            String p_d = FeatureWeightModel.GetModelDataFilename(outputFilename, folder);

            //  String p_f = FeatureWeightModel.GetSelectedFeaturesDataFilename(outputFilename, folder);

            Toolkit.Weighting.Data.WeightingModelDataSet dataset = SaveModelDataSet(logger);

            String xmlModel = objectSerialization.ObjectToXML(this);


            String xmlData = objectSerialization.ObjectToXML(dataset);

            var p_fi = p_m.getWritableFile(imbSCI.Data.enums.getWritableFileMode.existing, logger);

            File.WriteAllText(p_fi.FullName, xmlModel);


            var d_fi = p_d.getWritableFile(imbSCI.Data.enums.getWritableFileMode.existing, logger);

            File.WriteAllText(d_fi.FullName, xmlData);




            //var d_sf = p_f.getWritableFile(imbSCI.Data.enums.getWritableFileMode.existing, logger);

            //File.WriteAllText(d_sf.FullName, xmlData);

            /*
            corpusOperation.weightModel.saveObjectToXML(
                notes.folder.pathFor(setup.OutputFilename.ensureEndsWith("_model.xml"), imbSCI.Data.enums.getWritableFileMode.autoRenameThis, "Weight model [" + corpusOperation.weightModel.shortName + "]"));

            dataset.saveObjectToXML(notes.folder.pathFor(setup.OutputFilename.ensureEndsWith("_data.xml"), imbSCI.Data.enums.getWritableFileMode.autoRenameThis, "Weight model [" + corpusOperation.weightModel.shortName + "]"));
            */
        }


        /// <summary>
        /// Loads the specified output filename.
        /// </summary>
        /// <param name="outputFilename">The output filename.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="loadData">if set to <c>true</c> [load data].</param>
        /// <returns></returns>
        public static FeatureWeightModel Load(String outputFilename, folderNode folder, ILogBuilder logger, Boolean loadData = true)
        {
            String p_m = FeatureWeightModel.GetModelDefinitionFilename(outputFilename, folder);

            String p_d = FeatureWeightModel.GetModelDataFilename(outputFilename, folder);

            FeatureWeightModel output = objectSerialization.loadObjectFromXML<FeatureWeightModel>(p_m, logger);

            if (loadData)
            {
                WeightingModelDataSet dataset = objectSerialization.loadObjectFromXML<WeightingModelDataSet>(p_d, logger);
                output.LoadModelDataSet(dataset, logger);

            }

            return output;

        }



        /// <summary>
        /// Saves the model data set.
        /// </summary>
        /// <returns></returns>
        public WeightingModelDataSet SaveModelDataSet(ILogBuilder log)
        {
            WeightingModelDataSet output = new WeightingModelDataSet();
            output.modelData.Add(LocalFunction.SaveModelData());

            foreach (FeatureWeightFactor gf in GlobalFactors)
            {
                output.modelData.Add(gf.GlobalFunction.SaveModelData());
            }

            return output;
        }


        /// <summary>
        /// Loads the model data set.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        public void LoadModelDataSet(WeightingModelDataSet dataset, ILogBuilder log)
        {
            Dictionary<string, WeightingModelData> dict = dataset.GetDataDictionary();

            if (dict.ContainsKey(LocalFunction.shortName))
            {
                var localData = dict[LocalFunction.shortName];
                LocalFunction.LoadModelData(localData);
            }
            else
            {
                log.log("Loaded data set contains no data for [" + LocalFunction.shortName + "]");
            }

            foreach (FeatureWeightFactor gf in GlobalFactors)
            {
                gf.GlobalFunction.LoadModelData(dict[gf.GlobalFunction.shortName]);
            }

        }


        public void DiagnosticDump(folderNode folder, ILogBuilder log)
        {

            foreach (var gf in GlobalFactors)
            {
                if (gf.GlobalFunction.DistinctReturns.Count < 10)
                {
                    log.log("Factor [" + gf.Settings.GetSignature() + "] returned only [" + gf.GlobalFunction.DistinctReturns.Count + "] distinct scores");

                    String p_o = folder.pathFor(gf.GlobalFunction.shortName + "_fw_model_diagnostic.txt", imbSCI.Data.enums.getWritableFileMode.autoRenameThis, "Dump of distinct scores and first terms that received it");

                    StringBuilder sb = new StringBuilder();

                    foreach (var p in gf.GlobalFunction.DistinctReturns)
                    {
                        sb.AppendLine(p.Value + " = " + p.Key.ToString("F5"));
                    }

                    File.WriteAllText(p_o, sb.ToString());
                }
            }
        }



        /// <summary>
        /// Describes the specified logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public void Describe(ILogBuilder logger)
        {
            logger.AppendLine("Feature Weighting model");


            logger.AppendLine("Local weight model");
            logger.nextTabLevel();
            LocalFunction.Describe(logger);
            logger.prevTabLevel();


            logger.AppendLine("Global weight model(s)");
            logger.nextTabLevel();
            foreach (var lf in GlobalFactors)
            {
                lf.Describe(logger);
            }
            logger.prevTabLevel();

        }

        public void LoadModelData(WeightingModelData data)
        {

        }

        public WeightingModelData SaveModelData()
        {
            return null;
        }

        public void DeploySettings(GlobalFunctionSettings settings)
        {

        }

        public FeatureWeightModel()
        {

        }

        public TermFrequencyFunction LocalFunction { get; set; } = new TermFrequencyFunction();


        public List<FeatureWeightFactor> GlobalFactors { get; set; } = new List<FeatureWeightFactor>();

        public bool IsEnabled { get; set; } = true;

        public FunctionResultTypeEnum resultType { get; set; }

        [XmlIgnore]
        public Dictionary<double, string> DistinctReturns { get; set; } = new Dictionary<double, string>();

        //bool IGlobalElement.IsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

}