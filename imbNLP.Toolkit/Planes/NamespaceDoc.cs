namespace imbNLP.Toolkit.Planes
{

    /// <summary>
    /// Multi-plane vector space model is designed for multi-class single-label classification problem. However, it can be used for clusterization, index construction and other related problems.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The model describes main phases, components and vector models - that facilitate document sets (web site) categorization.
    /// </para>
    /// <list type="bullet">
    ///     <listheader>
    ///         <term>Vector space planes</term>
    ///         <description>List of the planes, considered by the model</description>
    ///     </listheader>
    ///     <item>
    ///         <term>Entity plane</term>
    ///         <description>Transforms web site pages into single document representations</description>
    ///     </item>
    ///     <item>
    ///         <term>Corpus plane</term>
    ///         <description>performs tokenization and stemming of the documents, Extracts Vocabulary, Term Frequency tables for Documents (entities), Labels (classes) and Corpus, computes global weight factors, performs feature filtering and/or dimension reduction (pLSI, LDA...).</description>
    ///     </item>
    ///     <item>
    ///         <term>Vector plane</term>
    ///         <description>Constructs Subject descriptors - i.e. Label (class) representation models, Document (entity) representation models. Initially, weighted bag-of-words is only considered representation model, but later it might include bag-of-terms, graphs, language models & etc.</description>
    ///     </item>
    ///     <item>
    ///         <term>Feature plane</term>
    ///         <description>Constructs Feature Vectors for Documents (entities), using specified Feature Dimension constructors (initially: similarity function between Document and Class), sends training vectors to classification model and/or performs classification for unlabeled vectors</description>
    ///     </item>
    /// </list>
    /// <para>
    /// Beside planes, the model design pattern introduces: Context <see cref="IPlaneContext"/> - Settings <see cref="IPlaneSettings"/> - Method <see cref="IPlaneMethodDesign"/>
    /// </para>
    /// <list type="bullet">
    ///     <listheader>
    ///         <term>Class pattern</term>
    ///         <description>Each plane is implemented trough classes, defined by this pattern</description>
    ///     </listheader>
    ///     <item>
    ///         <term>Method Design</term>
    ///         <description>Once <see cref="IPlaneSettings"/> are deployed, it performs actual job of the plane. Inputs are given trough <see cref="IPlaneContext"/>, while on the output side of the plane a <see cref="IPlaneContext"/> instance is created to match input of the next plane in the sequence</description>
    ///     </item>
    ///     <item>
    ///         <term>Method Functions</term>
    ///         <description>Functions are smaller method elements, declared outside this namespace. The Method Design is built from particular set of the functions - depending on required operations.</description>
    ///     </item>
    ///     <item>
    ///         <term>Settings</term>
    ///         <description>Serializable datastructure that implements <see cref="IPlaneSettings"/> and defines parameters that designate the essence of the method design.</description>
    ///     </item>
    ///     <item>
    ///         <term>Context</term>
    ///         <description><para>Datastructures that allow data exchange between planes, their methods and functions. They also allow plane-level caching mechanism. </para><para>Since no operational data (input / output / generated / internally consumed) should be declared outside contextes, complete thread-safeness is achieved</para></description>
    ///     </item>
    ///     
    /// </list>
    /// </remarks>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }

    /// <inheritdoc/>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceGroupDoc : NamespaceDoc
    {

    }

}
