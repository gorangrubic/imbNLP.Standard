using imbSCI.Graph.FreeGraph;
using System.Collections.Generic;

namespace imbNLP.PartOfSpeech.TFModels.vectorSpace
{
    public class spaceGraph : freeGraph, ISpaceModelEntity
    {
        public spaceGraph()
        {
        }

        public spaceModelRelationList relationList { get; set; }

        public string name { get; set; }

        public double weight { get; set; }
    }
}