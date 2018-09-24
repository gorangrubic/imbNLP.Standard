using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Processing;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Space
{
    /// <summary>
    /// Describes associations between entities in the model, e.g. <see cref="SpaceTopic"/>-<see cref="SpaceDocumentModel"/> and similar.
    /// </summary>
    public class SpaceModel
    {
        /// <summary>
        /// Raw frequency dictionary for all terms
        /// </summary>
        /// <value>
        /// The terms.
        /// </value>
        public TokenDictionary terms { get; set; } = new TokenDictionary();


        public SpaceLabel label_unknown { get; set; } = new SpaceLabel();

        public List<SpaceCategoryModel> categories { get; set; } = new List<SpaceCategoryModel>();

        public List<SpaceDocumentModel> documents { get; set; } = new List<SpaceDocumentModel>();

        public Relationships<SpaceLabel, SpaceDocumentModel> LabelToDocumentLinks = new Relationships<SpaceLabel, SpaceDocumentModel>();

        public Relationships<SpaceLabel, SpaceCategoryModel> LabelToCategoryLinks = new Relationships<SpaceLabel, SpaceCategoryModel>();

        /// <summary>
        /// Labels represent categories in the classification problem
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