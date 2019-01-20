using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Weighting.Data;
using imbNLP.Toolkit.Weighting.Metrics;
using imbSCI.Core.enums;
using imbSCI.Core.extensions.enumworks;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace imbNLP.Toolkit.Weighting.Global
{
    /// <summary>
    /// Collection level Term Discriminating Power factors
    /// </summary>
    /// <remarks>
    /// <para>
    /// Relevant paper: Lan, Man, Chew Lim Tan, Jian Su, and Yue Lu. 2009. “Supervised and Traditional Term Weighting Methods for Automatic Text Categorization.” IEEE Transactions on Pattern Analysis and Machine Intelligence 31 (4). IEEE: 721–35.
    /// </para>
    /// </remarks>
    /// <seealso cref="imbNLP.Toolkit.Weighting.Elements.ModelElementBase" />
    public class CollectionTDPElement : GlobalElementBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionTDPElement"/> class.
        /// </summary>
        public CollectionTDPElement()
        {
            shortName = "TDP";
            description = "Term Disciminating Power, Relevant paper: Lan, Man, Chew Lim Tan, Jian Su, and Yue Lu. 2009. “Supervised and Traditional Term Weighting Methods for Automatic Text Categorization.” IEEE Transactions on Pattern Analysis and Machine Intelligence 31 (4). IEEE: 721–35. ";
        }

        public operation defaultOperation { get; set; } = operation.max;

        private double GetElementFactor(string term, String labelName)
        {
            return computedModel.index[labelName][term];
        }

        /// <summary>
        /// Gets the element factor.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="space">The space.</param>
        /// <param name="label">The label.</param>
        /// <returns></returns>
        public override double GetElementFactor(string term, SpaceModel space, SpaceLabel label = null)
        {
            // SpaceLabel label = DocumentVsLabel[document];
            Double output = 0;

            List<String> labelNames = new List<string>();
            if (label == null)
            {
                labelNames = computedModel.index.Keys.ToList();
            }
            else
            {
                labelNames.Add(label.name);
            }

            List<Double> scores = new List<double>();

            foreach (String ln in labelNames)
            {
                scores.Add(GetElementFactor(term, ln));
            }

            output = operationExtensions.CompressNumericVector(scores.ToArray(), defaultOperation);


            /*
            if (!computedModel.index.ContainsKey(label.name)) return 0;

            if (!computedModel.index[label.name].ContainsKey(term)) return 0;
            */

            //TermDiscriminatingPower TDP = model[label.name][term];

            //output = TDP.Compute(factor, N);

            //output = computedModel.index[label.name][term];

            return output;
        }

        /// <summary>
        /// Describes the specified logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public override void Describe(ILogBuilder logger)
        {
            base.Describe(logger);
            logger.AppendPair("TDP function", factor.ToString(), true, "\t\t\t");
        }


        /// <summary>
        /// Prepares the model.
        /// </summary>
        /// <param name="space">The space.</param>
        /// <exception cref="System.ArgumentException">A document is already assigned to a label! This model is not applicable for multi-label problem.</exception>
        public override void PrepareTheModel(SpaceModel space, ILogBuilder log)
        {
            List<String> terms = space.GetTokens(true, false);
            List<SpaceLabel> labels = space.labels;

            var labelNames = labels.Select(x => x.name);

            TermDiscriminatingPowerModel model = new TermDiscriminatingPowerModel();

            model.PrepareBlank(labelNames, terms);

            //N = space.documents.Count;

            Dictionary<String, List<SpaceDocumentModel>> documentDict = new Dictionary<String, List<SpaceDocumentModel>>();
            Dictionary<String, List<SpaceDocumentModel>> documentNegativeDict = new Dictionary<String, List<SpaceDocumentModel>>();

            foreach (SpaceLabel label in labels)
            {
                //model.dictionaries.Add(label.name, new TermDiscriminatingPowerDictionary(label.name, terms));

                documentDict.Add(label.name, space.LabelToDocumentLinks.GetAllLinked(label));

                //foreach (SpaceDocumentModel document in documentDict[label])
                //{
                //    if (DocumentVsLabel.ContainsKey(document))
                //    {
                //        throw new ArgumentException("A document is already assigned to a label! This model is not applicable for multi-label problem.");
                //    }
                //    DocumentVsLabel.Add(document, label);
                //}

                documentNegativeDict.Add(label.name, new List<SpaceDocumentModel>());

                //foreach (SpaceDocumentModel doc in space.documents)
                //{
                //    if (!documentDict[label].Contains(doc)) negativeDocuments.Add(doc);
                //}
            }


            foreach (KeyValuePair<String, List<SpaceDocumentModel>> pair in documentDict)
            {
                N += pair.Value.Count;
                foreach (KeyValuePair<String, List<SpaceDocumentModel>> pairSub in documentDict)
                {
                    if (pair.Key != pairSub.Key)
                    {
                        documentNegativeDict[pair.Key].AddRange(pairSub.Value);
                    }
                }
            }

            //   var documents = space.LabelToDocumentLinks.GetAllLinked(label);



            Parallel.ForEach(labels, label =>
            {

                foreach (String term in terms)
                {
                    TermDiscriminatingPower TDP = model[label.name][term];
                    TDP.a = documentDict[label.name].Count(x => x.Contains(term));
                    TDP.b = documentDict[label.name].Count() - TDP.a;

                    TDP.c = documentNegativeDict[label.name].Count(x => x.Contains(term));
                    TDP.d = documentNegativeDict[label.name].Count() - TDP.c;
                }

            });


            computedModel = model.GetComputedModel(factor, N);



            /*
                foreach (SpaceLabel label in labels) // << --- UNKNOWN LABEL IS INCLUDED
                {
                    var documents = space.LabelToDocumentLinks.GetAllLinked(label);

                    foreach (SpaceDocumentModel document in documents)
                    {
                        if (DocumentVsLabel.ContainsKey(document))
                        {
                            throw new ArgumentException("A document is already assigned to a label! This model is not applicable for multi-label problem.");
                        }
                        DocumentVsLabel.Add(document, label);
                    }

                    List<SpaceDocumentModel> negativeDocuments = new List<SpaceDocumentModel>();
                    foreach (SpaceDocumentModel doc in space.documents)
                    {
                        if (!documents.Contains(doc)) negativeDocuments.Add(doc);
                    }

                    foreach (String term in terms)
                    {
                        TermDiscriminatingPower TDP = model[label.name][term];
                        TDP.a = documents.Count(x => x.terms.Contains(term));
                        TDP.b = documents.Count - TDP.b;

                        TDP.c = negativeDocuments.Count(x => x.terms.Contains(term));
                        TDP.d = negativeDocuments.Count - TDP.c;
                    }
                }*/
        }

        public override WeightingModelData SaveModelData()
        {
            WeightingModelData output = new WeightingModelData();
            output.signature = shortName;

            Int32 c = 0;
            foreach (var l in computedModel.labels)
            {

                output.data.Add(new TermWeightData(computedModel.index[l]));
                output.properties.entries.Add(new TermWeightEntry(l, c));

                c++;
            }


            return output;
        }

        public override void LoadModelData(WeightingModelData data)
        {

            List<String> __labels = new List<string>(data.properties.entries.Count);

            var dict = data.properties.GetIndexDictionary();

            foreach (var pair in dict)
            {
                __labels[Convert.ToInt32(pair.Value)] = pair.Key;
            }


            computedModel = new TermDiscriminatingPowerComputedModel(__labels, factor);

            for (int i = 0; i < __labels.Count; i++)
            {
                String label = __labels[i];
                computedModel.index[label] = data.data[i].GetIndexDictionary();
            }

        }

        public override void DeploySettings(GlobalFunctionSettings settings)
        {
            factor = imbEnumExtendBase.GetEnumFromStringFlags<TDPFactor>(settings.flags, factor).FirstOrDefault();
            defaultOperation = imbEnumExtendBase.GetEnumFromStringFlags<operation>(settings.flags, defaultOperation).FirstOrDefault();
            if (defaultOperation == operation.none) defaultOperation = operation.max;
        }



        ///// <summary>
        ///// Gets or sets the document vs label.
        ///// </summary>
        ///// <value>
        ///// The document vs label.
        ///// </value>
        //protected Dictionary<SpaceDocumentModel, SpaceLabel> DocumentVsLabel { get; set; } = new Dictionary<SpaceDocumentModel, SpaceLabel>();

        protected Double N { get; set; }

        //protected 

        protected TermDiscriminatingPowerComputedModel computedModel { get; set; } = new TermDiscriminatingPowerComputedModel();

        public TDPFactor factor { get; set; } = TDPFactor.gr;

        /// <summary>
        /// Gets the type of the result.
        /// </summary>
        /// <value>
        /// The type of the result.
        /// </value>
        public override FunctionResultTypeEnum resultType { get { return FunctionResultTypeEnum.numericVectorForMultiClass; } }
    }; // => throw new NotImplementedException();
}
