namespace imbNLP.Toolkit.Feature.Settings
{

    public enum FeatureVectorDimensionType
    {
        /// <summary>
        /// The direct term weight - dimension directly supplies the classifier with feature weights
        /// </summary>
        directTermWeight,
        similarityFunction,
        topicWeight,
        precompiledDocumentScore
    }

}