using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Weighting.Data;
using imbSCI.Core.reporting;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Weighting.Local
{
    /// <summary>
    /// Term Frequency function from TF-IDF. It supports regular TF normalization but also some of derived computation methods for local term weighting factor (mTF, Glasgow, RTF...)
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Weighting.Local.LocalElementBase" />
    public class TermFrequencyFunction : LocalElementBase
    {

        [XmlIgnore]
        public String description { get; set; } = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="TermFrequencyFunction"/> class.
        /// </summary>
        public TermFrequencyFunction()
        {

            shortName = GetFunctionName(computation);
        }

        public String GetSignature()
        {
            return GetFunctionName(computation) + GetNormalizationName(normalization);
        }

        public static String GetNormalizationName(TFNormalization norm)
        {
            switch (norm)
            {
                case TFNormalization.divisionByMaxTF:
                    return "[max]";
                    break;
                case TFNormalization.squareRootOfSquareSum:
                    return "[Sqr(sum)]";
                    break;
            }
            return norm.ToString();
        }

        public static String GetFunctionName(TFComputation comp)
        {

            switch (comp)
            {
                case TFComputation.glasgow:
                    return "Glasgow";
                    break;
                case TFComputation.modifiedTF:
                    return "mTF";
                    break;
                case TFComputation.normal:
                    return "TF";
                    break;
                case TFComputation.squareRooted:
                    return "RTF";
                    break;
            }
            return comp.ToString();
        }


        /// <summary>
        /// Gets or sets the TF normalization.
        /// </summary>
        /// <value>
        /// The normalization.
        /// </value>
        public TFNormalization normalization { get; set; } = TFNormalization.squareRootOfSquareSum;

        /// <summary>
        /// Gets or sets the computation.
        /// </summary>
        /// <value>
        /// The computation.
        /// </value>
        public TFComputation computation { get; set; } = TFComputation.normal;

        protected Double SqrTc { get; set; }






        ///// <summary>
        ///// Normalization divisor for document
        ///// </summary>
        ///// <value>
        ///// The index of the TFN.
        ///// </value>
        //protected Dictionary<SpaceDocumentModel, Double> TFN_index { get; set; } = new Dictionary<SpaceDocumentModel, Double>();





        public override void PrepareTheModel(SpaceModel _space, ILogBuilder log)
        {

            var space = _space;

            TokenDictionary training_terms = space.GetTerms(true, true);

            List<SpaceLabel> labels = space.labels.ToList();




            shortName = GetFunctionName(computation);

            if (!IsEnabled) return;


            switch (computation)
            {
                case TFComputation.modifiedTF:
                    SqrTc = Math.Sqrt(training_terms.GetSumFrequency());

                    break;
                default:
                    //foreach (SpaceDocumentModel document in space.documents)
                    //{
                    //    TFN_index.Add(document, GetDivisor(document));
                    //}
                    break;
            }

            index = training_terms.ToFrequencyDictionary();
        }

        private Double GetDivisor(TokenDictionary document)
        {
            if (document.Count == 0) return 1;

            switch (normalization)
            {
                case TFNormalization.squareRootOfSquareSum:
                    return document.GetSquareRootOfSumSquareFrequencies();
                    break;
                default:
                case TFNormalization.divisionByMaxTF:
                    return document.GetMaxFrequency();
                    break;
            }

        }

        public override double GetElementFactor(string term, SpaceDocumentModel document)
        {
            if (!IsEnabled) return 1;


            TokenDictionary docDict = document.GetTerms(true, true);



            Double TF = docDict.GetTokenFrequency(term);

            switch (computation)
            {
                case TFComputation.modifiedTF:

                    if (!index.ContainsKey(term))
                    {
                        return 0;
                    }

                    Double Tt = index[term]; // training_terms.GetTokenFrequency(term);

                    Double length_d = docDict.Count; //.GetTokenCount();

                    Double mTF_above = TF * Math.Log(SqrTc / Tt);

                    Double mTF_below_2nd = (length_d * length_d) / SqrTc;

                    Double mTF_below = Math.Log(docDict.GetSumSquareFrequencies() * mTF_below_2nd);

                    return mTF_above / mTF_below;
                    break;
            }


            Double divisor = GetDivisor(docDict);

            //if (TFN_index.ContainsKey(document))
            //{
            //    divisor = TFN_index[document];
            //}
            //else
            //{
            //    divisor 
            //}

            switch (computation)
            {
                default:
                case TFComputation.normal:
                    return TF / divisor;
                    break;

                case TFComputation.squareRooted:
                    return Math.Sqrt(TF / divisor);
                    break;

                case TFComputation.glasgow:
                    return Math.Log(TF + 1) / divisor;
                    break;
            }
        }

        public override WeightDictionaryEntry GetElementFactorEntry(string term, SpaceDocumentModel document)
        {
            return new WeightDictionaryEntry(term, GetElementFactor(term, document));
        }

        public override void Describe(ILogBuilder logger)
        {
            logger.AppendPair("Schema", GetFunctionName(computation), true, "\t\t\t");
            if (!description.isNullOrEmpty())
            {
                logger.AppendComment(description);
            }
            String norm_ln = "";
            switch (normalization)
            {
                case TFNormalization.divisionByMaxTF:
                    norm_ln = "Division by max(TF) in the document";
                    break;
                case TFNormalization.squareRootOfSquareSum:
                    norm_ln = "Square root of square sums of TFs in the document";
                    break;
            }
            logger.AppendPair("Normalizaton", norm_ln);

        }

        public override WeightingModelData SaveModelData()
        {
            var output = SaveModelDataBase();

            output.properties.entries.Add(new TermWeightEntry(nameof(SqrTc), SqrTc));
            return output;
        }

        public override void LoadModelData(WeightingModelData data)
        {
            LoadModelDataBase(data);
            var dict = data.properties.GetIndexDictionary();

            SqrTc = dict[nameof(SqrTc)]; //(Double)data.properties[nameof(SqrTc)];

        }


    }

}
