using imbSCI.Graph.FreeGraph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.PartOfSpeech.TFModels.vectorSpace
{
    /// <summary>
    /// Enumeration of entity types that are anticipated by <see cref="spaceModelRelation"/>
    /// </summary>
    public enum spaceModelRelationEntityType
    {
        /// <summary>
        /// Local reference, pointing to an entity that hosts the <see cref="spaceModelRelationList"/>
        /// </summary>
        self,

        /// <summary>
        /// Term - basic content unit in space vector model
        /// </summary>
        Term,

        /// <summary>
        /// e.g. N-gram, sequence or other sub-Document representation model element
        /// </summary>
        Phrase,

        /// <summary>
        /// e.g. Web page or text document
        /// </summary>
        Document,

        /// <summary>
        /// e.g. Web site or other collection of <see cref="Document"/>s that is single instance in classification
        /// </summary>
        DocumentSet,

        /// <summary>
        /// e.g. Category or human supplied tag
        /// </summary>
        Label,

        /// <summary>
        /// e.g. Latent topics, extracted by Latent Semantic Analysis or LDA
        /// </summary>
        Topic,

        /// <summary>
        /// e.g. additional dimension/score/measure, attached to the item
        /// </summary>
        Dimension
    }
}