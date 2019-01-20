using imbNLP.Toolkit.Processing;
using imbSCI.Core.math;
using imbSCI.Core.reporting;
using imbSCI.Data.collection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Space
{
    /// <summary>
    /// Describes associations between entities in the model, e.g. <see cref="SpaceTopic"/>-<see cref="SpaceDocumentModel"/> and similar.
    /// </summary>
    [Serializable]
    public class SpaceModel
    {

        public SpaceModel Clone()
        {
            SpaceModel output = new SpaceModel();

            output.terms = terms.Clone();
            output.terms_known_label = terms_known_label.Clone();
            output.terms_unknown_label = terms_unknown_label.Clone();
            output.documents = documents.Clone<SpaceDocumentModel>(false);
            output.categories = categories.Clone<SpaceCategoryModel>(false);
            output.topics = topics.CloneTerm<SpaceTopic>();
            output.labels = labels.CloneTerm<SpaceLabel>();

            foreach (var label in labels)
            {
                var linked = LabelToDocumentLinks.GetAllLinked(label);
                var label2 = output.labels.First(x => x.name == label.name);
                foreach (var l in linked)
                {
                    SpaceDocumentModel doc2 = output.documents.First(x => x.name == l.name);

                    output.LabelToDocumentLinks.Add(label2, doc2, l.weight);
                }
            }

            foreach (var label in labels)
            {
                var linked = LabelToCategoryLinks.GetAllLinked(label);
                var label2 = output.labels.First(x => x.name == label.name);
                foreach (var l in linked)
                {
                    SpaceCategoryModel doc2 = output.categories.First(x => x.name == l.name);

                    output.LabelToCategoryLinks.Add(label2, doc2, l.weight);
                }
            }



            return output;
            //output.categories
        }

        // public List<SpaceDocumentModel> documents { get; set; } = new List<SpaceDocumentModel>();




        /// <summary>
        /// Gets a value indicating whether this instance of the model is ready.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is model ready; otherwise, if no terms, categories or documents were defined, it returns: <c>false</c>.
        /// </value>
        public Boolean IsModelReady
        {
            get
            {
                if (terms_known_label.Count == 0) return false;
                //if (!categories.Any()) return false;
                if (!documents.Any()) return false;
                return true;
            }
        }




        /// <summary>
        /// Makes a subset of input dataset, containing only documents matching assigned IDS
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="assignedIDs">The assigned i ds.</param>
        /// <param name="inverse">if set to <c>true</c> if will keep only given assigned IDS</param>
        public void FilterSpaceModel(List<String> assignedIDs, ILogBuilder log, Boolean applyToUnknownCategory = false)
        {
            foreach (KeyValuePair<string, List<string>> pair in LabelToDocumentLinks.GetAllRelationShipByName())
            {
                List<SpaceDocumentModel> toRemoveDocuments = new List<SpaceDocumentModel>();
                List<SpaceDocumentModel> toScanDocuments = new List<SpaceDocumentModel>();
                Boolean doScan = true;
                if (pair.Key == SpaceLabel.UNKNOWN)
                {
                    doScan = applyToUnknownCategory;
                }

                if (doScan)
                {
                    toScanDocuments = LabelToDocumentLinks.GetAllLinkedB(pair.Key);

                    foreach (var vec in toScanDocuments)
                    {
                        if (!assignedIDs.Contains(vec.name))
                        {
                            toRemoveDocuments.Add(vec);
                        }
                    }

                    foreach (var vec in toRemoveDocuments)
                    {
                        documents.Remove(vec);
                        LabelToDocumentLinks.Remove(vec);
                    }

                    Double removal = toRemoveDocuments.Count.GetRatio(toScanDocuments.Count);
                    log.log("Vector count of [" + pair.Key + "] reduced to [" + removal.ToString("P2") + "] by document selection list");
                }


            }

        }



        /// <summary>
        /// All terms occuring in the input datasets
        /// </summary>
        /// <value>
        /// The terms.
        /// </value>
        public TokenDictionary terms { get; set; } = new TokenDictionary();

        /// <summary>
        /// Raw frequency dictionary for all terms
        /// </summary>
        /// <value>
        /// The terms.
        /// </value>
        public TokenDictionary terms_known_label { get; set; } = new TokenDictionary();

        public TokenDictionary terms_unknown_label { get; set; } = new TokenDictionary();


        public SpaceLabel label_unknown { get; set; } = new SpaceLabel();

        public List<SpaceCategoryModel> categories { get; set; } = new List<SpaceCategoryModel>();

        public List<SpaceDocumentModel> documents { get; set; } = new List<SpaceDocumentModel>();

        public Relationships<SpaceLabel, SpaceDocumentModel> LabelToDocumentLinks = new Relationships<SpaceLabel, SpaceDocumentModel>();

        public Relationships<SpaceLabel, SpaceCategoryModel> LabelToCategoryLinks = new Relationships<SpaceLabel, SpaceCategoryModel>();

        /// <summary>
        /// Labels represent categories in the classification problem, without the UNLABELED category
        /// </summary>
        /// <value>
        /// The labels.
        /// </value>
        public List<SpaceLabel> labels { get; set; } = new List<SpaceLabel>();

        /// <summary>
        /// Gets or sets the topics.
        /// </summary>
        /// <value>
        /// The topics.
        /// </value>
        public List<SpaceTopic> topics { get; set; } = new List<SpaceTopic>();

        //        public SpaceModelRelationships relationships { get; set; } = null;

        // public SpaceCorpora corpora { get; set; } = new SpaceCorpora();



        ///// <summary>
        ///// Constructs the corpora representation for LDA
        ///// </summary>
        ///// <returns></returns>
        //public Corpora ConstructCorporaForLDA()
        //{
        //    Corpora output = new Corpora();
        //    output.totalDocuments = documents.Count;
        //    output.Docs = new Document[documents.Count];
        //    for (int i = 0; i < output.totalDocuments; i++)
        //    {
        //        var doc = documents[i];
        //        output.Docs[i] = new Document();
        //        output.Docs[i].Words = doc.Words;
        //        output.Docs[1].Length = doc.Length;
        //        output.totalWords += doc.Length;
        //    }
        //    return output;
        //}

        public SpaceModel()
        {
        }
    }
}