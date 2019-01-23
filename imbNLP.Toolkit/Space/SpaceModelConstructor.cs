using imbNLP.Toolkit.Documents.Analysis;
using imbNLP.Toolkit.Processing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Space
{

    public class SpaceModelSettings
    {


        public Boolean DoMaintainWordIndex { get; set; } = false;

    }



    /// <summary>
    /// Constructor for the <see cref="SpaceDocumentModel"/> instances
    /// </summary>
    public class SpaceModelConstructor
    {


        private static Object _spaceSettings_lock = new Object();
        private static SpaceModelSettings _spaceSettings;
        /// <summary>
        /// Global control over space model construction and handling
        /// </summary>
        public static SpaceModelSettings spaceSettings
        {
            get
            {
                if (_spaceSettings == null)
                {
                    lock (_spaceSettings_lock)
                    {

                        if (_spaceSettings == null)
                        {
                            _spaceSettings = new SpaceModelSettings();
                            // add here if any additional initialization code is required
                        }
                    }
                }
                return _spaceSettings;
            }
        }



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
            for (int i2 = 0; i2 < tkn.Count; i2++)
            {
                String stk = stemmContext.Stem(tkn[i2]);
                stemmDictionary.CountToken(stk, tokenDictionary.GetTokenFrequency(tkn[i2]));
            }

            //  context.terms.MergeDictionary(stemmDictionary);

            if (metrics != null) metrics.StemmedTokensDoc += stemmDictionary.Count; // <---- stemmed

            SpaceDocumentModel document = new SpaceDocumentModel();
            document.name = name;
            document.terms = stemmDictionary;
            document.Length = tokens.Length;

            if (spaceSettings.DoMaintainWordIndex)
            {
                document.Words = new int[document.Length];
            }

            Int32 c = 0;
            for (int i = 0; i < tokens.Length; i++)
            {
                String stk = stemmContext.Stem(tokens[i]);

                if (isKnownDocument)
                {
                    context.terms_known_label.AddToken(stk);
                }
                else
                {
                    context.terms_unknown_label.AddToken(stk);
                }

                if (spaceSettings.DoMaintainWordIndex)
                {
                    document.Words[c] = context.terms.GetTokenID(stk);
                }
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