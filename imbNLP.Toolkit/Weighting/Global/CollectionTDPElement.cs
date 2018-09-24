using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Weighting.Metrics;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public CollectionTDPElement()
        {
            shortName = "TDP";
            description = "Term Disciminating Power, Relevant paper: Lan, Man, Chew Lim Tan, Jian Su, and Yue Lu. 2009. “Supervised and Traditional Term Weighting Methods for Automatic Text Categorization.” IEEE Transactions on Pattern Analysis and Machine Intelligence 31 (4). IEEE: 721–35. ";
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
            if (label == null) return 0;
            if (!model[label.name].terms.ContainsKey(term)) return 0;

            TermDiscriminatingPower TDP = model[label.name][term];

            output = TDP.Compute(factor, N);

            return output;
        }

        public override void Describe(ILogBuilder logger)
        {
            base.Describe(logger);
            logger.AppendPair("TDP function", factor.ToString(), true, "\t\t\t");
        }

        /*
        /// <summary>
        /// Gets the element factor entry.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="space">The space.</param>
        /// <param name="label">The label.</param>
        /// <returns></returns>
        public override WeightDictionaryEntry GetElementFactorEntry(string term, SpaceModel space, SpaceLabel label = null)
        {
            Double[] vec = new Double[space.labels.Count];
            Int32 c = 0;
            WeightDictionaryEntry output = new WeightDictionaryEntry(term, 0);

            foreach (SpaceLabel lb in space.labels)
            {
                    vec[c] = GetElementFactor(term, space, lb);
                    c++;
            }
            output = new WeightDictionaryEntry(term, vec);
            return output;
        }
        */
        /// <summary>
        /// Prepares the model.
        /// </summary>
        /// <param name="space">The space.</param>
        /// <exception cref="System.ArgumentException">A document is already assigned to a label! This model is not applicable for multi-label problem.</exception>
        public override void PrepareTheModel(SpaceModel space)
        {
            List<String> terms = space.terms.GetTokens();
            List<SpaceLabel> labels = space.labels;

            N = space.documents.Count;

            foreach (SpaceLabel label in labels)
            {
                model.dictionaries.Add(label.name, new TermDiscriminatingPowerDictionary(label.name, terms));
            }

            foreach (SpaceLabel label in labels)
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
            }
        }

        /// <summary>
        /// Gets or sets the document vs label.
        /// </summary>
        /// <value>
        /// The document vs label.
        /// </value>
        protected Dictionary<SpaceDocumentModel, SpaceLabel> DocumentVsLabel { get; set; } = new Dictionary<SpaceDocumentModel, SpaceLabel>();

        protected Int32 N { get; set; }

        protected TermDiscriminatingPowerModel model { get; set; } = new TermDiscriminatingPowerModel();

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
