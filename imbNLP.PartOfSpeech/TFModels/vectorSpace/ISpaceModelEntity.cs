using imbSCI.Graph.FreeGraph;
using System.Collections.Generic;

namespace imbNLP.PartOfSpeech.TFModels.vectorSpace
{
    /// <summary>
    /// An entity, that can be part of the <see cref="spaceModel"/>
    /// </summary>
    public interface ISpaceModelEntity
    {
        spaceModelRelationList relationList { get; set; }

        string name { get; set; }

        double weight { get; set; }
    }
}