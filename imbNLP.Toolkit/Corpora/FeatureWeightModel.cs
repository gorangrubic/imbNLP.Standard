using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Weighting.Global;
using imbNLP.Toolkit.Weighting.Local;
using imbSCI.Core.reporting;
using imbSCI.Data;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Corpora
{

    /// <summary>
    /// Model for weight computation
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Weighting.Global.IGlobalElement" />
    public class FeatureWeightModel : IDescribe, IGlobalElement
    {

        public String shortName { get; set; } = "";

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
        public void PrepareTheModel(SpaceModel space)
        {
            foreach (FeatureWeightFactor gf in GlobalFactors)
            {
                gf.GlobalFunction.PrepareTheModel(space);
                if (gf.GlobalFunction.resultType == FunctionResultTypeEnum.numericVectorForMultiClass)
                {
                    nDimensions = space.labels.Count;
                    resultType = gf.GlobalFunction.resultType;
                }
            }

            LocalFunction.PrepareTheModel(space);
        }

        public Int32 nDimensions { get; set; } = 1;


        public WeightDictionary GetElementFactors(IEnumerable<string> terms, SpaceModel space, SpaceLabel label = null)
        {
            throw new NotImplementedException();
        }

        public WeightDictionaryEntry GetElementFactorEntry(string term, SpaceModel space, SpaceLabel label = null)
        {
            throw new NotImplementedException();
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
            output.nDimensions = nDimensions;

            foreach (String term in termWhiteList)
            {
                if (document.terms.Contains(term))
                {

                    /*
                    Double lF = LocalFunction.GetElementFactor(term, document);

                    Double gF = 1;
                    foreach (FeatureWeightFactor gf in GlobalFactors)
                    {
                        gF *= gf.GlobalFunction.GetElementFactor(term, space, label) * gf.weight;
                    }

                    Double o = lF * gF;
                    */



                    WeightDictionaryEntry entry = new WeightDictionaryEntry(term, 0);


                    entry = LocalFunction.GetElementFactorEntry(term, document);

                    foreach (FeatureWeightFactor gf in GlobalFactors)
                    {
                        entry = entry * (gf.GlobalFunction.GetElementFactorEntry(term, space, label) * gf.weight);
                    }



                    output.entries.Add(entry);
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
            Double TF = LocalFunction.GetElementFactor(term, document);

            return TF;
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

            return GF;
        }



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


        public FeatureWeightModel()
        {

        }

        public TermFrequencyFunction LocalFunction { get; set; } = new TermFrequencyFunction();


        public List<FeatureWeightFactor> GlobalFactors { get; set; } = new List<FeatureWeightFactor>();
        public bool IsEnabled { get; set; } = true;

        public FunctionResultTypeEnum resultType { get; set; }

        //bool IGlobalElement.IsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

}