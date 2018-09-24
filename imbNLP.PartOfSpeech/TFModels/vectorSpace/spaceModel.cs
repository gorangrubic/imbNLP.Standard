using imbSCI.Graph.FreeGraph;

namespace imbNLP.PartOfSpeech.TFModels.vectorSpace
{
    /// <summary>
    /// Describes associations between entities in the model, e.g. <see cref="SpaceTopic"/>-<see cref="SpaceDocumentModel"/> and similar.
    /// </summary>
    public class spaceModel
    {
        protected freeGraph relationGraph { get; set; } = new freeGraph();
    }
}