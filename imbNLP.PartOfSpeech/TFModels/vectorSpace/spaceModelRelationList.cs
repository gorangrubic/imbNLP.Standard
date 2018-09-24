using imbSCI.Graph.FreeGraph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.PartOfSpeech.TFModels.vectorSpace
{
    /// <summary>
    /// Passive list of relationships, defined at an entity
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{imbNLP.PartOfSpeech.TFModels.vectorSpace.spaceModelRelation}" />
    public class spaceModelRelationList : List<spaceModelRelation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="spaceModelRelationList"/> class.
        /// </summary>
        public spaceModelRelationList()
        {
        }
    }
}