using imbNLP.Toolkit.Space;

namespace imbNLP.Toolkit.TopicModels
{
    public abstract class TopicModelBase
    {
        /// <summary>
        /// Prepares the model.
        /// </summary>
        /// <param name="space">The space.</param>
        public abstract void PrepareTheModel(SpaceModel space);
    }
}