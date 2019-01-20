using imbNLP.Toolkit.Documents.Analysis;
using imbNLP.Toolkit.Processing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Space
{
    /// <summary>
    /// Constructor for the <see cref="SpaceDocumentModel"/> instances
    /// </summary>
    public class SpaceModelConstructor
    {



        /// <summary>
        /// Constructs a document model
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="name">The name.</param>
        /// <param name="context">The context.</param>
        /// <param name="stemmContext">The stemm context.</param>
        /// <param name="tokenizer">The tokenizer.</param>
        /// <param name="metrics">The metrics.</param>
        /// <returns></returns>
        public SpaceDocumentModel ConstructDocument(string text, String name, SpaceModel context, StemmingContext stemmContext, ITokenizer tokenizer, Boolean isKnownDocument, ContentMetrics metrics = null)
        {
            var tokens = tokenizer.Tokenize(text);

            if (metrics != null) metrics.TokensDoc += tokens.Length;               // <----- token length

            TokenDictionary tokenDictionary = new TokenDictionary(tokens);


            if (metrics != null) metrics.UniqueTokensDoc += tokenDictionary.Count; // <---- unique tokens

            TokenDictionary stemmDictionary = new TokenDictionary();

            List<String> tkn = tokenDictionary.GetTokens();
            foreach (String tk in tkn)
            {
                String stk = stemmContext.Stem(tk);
                stemmDictionary.CountToken(stk, tokenDictionary.GetTokenFrequency(tk));
            }

            //  context.terms.MergeDictionary(stemmDictionary);

            if (metrics != null) metrics.StemmedTokensDoc += stemmDictionary.Count; // <---- stemmed

            SpaceDocumentModel document = new SpaceDocumentModel();
            document.name = name;
            document.terms = stemmDictionary;
            document.Length = tokens.Length;

            document.Words = new int[document.Length];

            Int32 c = 0;
            foreach (String tk in tokens)
            {
                String stk = stemmContext.Stem(tk);
                Int32 id = context.terms.GetTokenID(stk);
                if (isKnownDocument)
                {
                    context.terms_known_label.AddToken(stk);
                }
                else
                {
                    context.terms_unknown_label.AddToken(stk);
                }

                document.Words[c] = id;
                c++;
            }

            document.name = name;

            // context.documents.Add(document);



            return document;
        }

        /// <summary>
        /// Adds the label if not already declared within the context
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="context">The context.</param>
        public void AddLabel(String text, SpaceModel context)
        {
            if (!context.labels.Any(x => x.name == text))
            {
                SpaceLabel label = new SpaceLabel();
                label.name = text;
                context.labels.Add(label);
            }
        }

        /// <summary>
        /// Gets the label instances for given list of label names
        /// </summary>
        /// <param name="labelIds">The label ids.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public List<SpaceLabel> GetLabels(IEnumerable<String> labelIds, SpaceModel context)
        {
            List<SpaceLabel> output = new List<SpaceLabel>();

            foreach (String lbl in labelIds)
            {
                var tmp = context.labels.FirstOrDefault(x => x.name == lbl);
                if (tmp != null)
                {
                    output.Add(tmp);
                }
            }

            return output;
        }


    }
}