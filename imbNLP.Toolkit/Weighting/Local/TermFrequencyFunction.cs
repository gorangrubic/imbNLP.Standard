using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbSCI.Core.reporting;
using imbSCI.Data;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Normalization divisor for document
        /// </summary>
        /// <value>
        /// The index of the TFN.
        /// </value>
        protected Dictionary<SpaceDocumentModel, Double> TFN_index { get; set; } = new Dictionary<SpaceDocumentModel, Double>();

        public override void PrepareTheModel(SpaceModel _space)
        {
            shortName = GetFunctionName(computation);

            if (!IsEnabled) return;
            space = _space;

            switch (computation)
            {
                case TFComputation.modifiedTF:
                    SqrTc = Math.Sqrt(space.terms.GetSumFrequency());

                    break;
                default:
                    foreach (SpaceDocumentModel document in space.documents)
                    {
                        TFN_index.Add(document, GetDivisor(document));
                    }
                    break;
            }


        }

        private Double GetDivisor(SpaceDocumentModel document)
        {
            if (document.terms.Count == 0) return 1;

            switch (normalization)
            {
                case TFNormalization.squareRootOfSquareSum:
                    return document.terms.GetSquareRootOfSumSquareFrequencies();
                    break;
                default:
                case TFNormalization.divisionByMaxTF:
                    return document.terms.GetMaxFrequency();
                    break;
            }

        }

        public override double GetElementFactor(string term, SpaceDocumentModel document)
        {
            if (!IsEnabled) return 1;

            Double TF = document.terms.GetTokenFrequency(term);

            switch (computation)
            {
                case TFComputation.modifiedTF:



                    Double Tt = space.terms.GetTokenFrequency(term);

                    Double length_d = document.terms.Count;

                    Double mTF_above = TF * Math.Log(SqrTc / Tt);

                    Double mTF_below_2nd = (length_d * length_d) / SqrTc;

                    Double mTF_below = Math.Log(document.terms.GetSumSquareFrequencies() * mTF_below_2nd);

                    return mTF_above / mTF_below;
                    break;
            }


            Double divisor = 0;
            if (TFN_index.ContainsKey(document))
            {
                divisor = TFN_index[document];
            }
            else
            {
                divisor = GetDivisor(document);
            }

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
            logger.AppendPair("Schema", shortName, true, "\t\t\t");
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
    }

}
