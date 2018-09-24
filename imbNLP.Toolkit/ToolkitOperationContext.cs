using imbNLP.Toolkit.Space;

namespace imbNLP.Toolkit
{
    /// <summary>
    /// Containes input, output and data structures used by algorithms in the library.
    /// </summary>
    /// <remarks>
    /// <para>Intended use of the OperationContext:</para>
    /// <list type="bullet">
    /// <item>Assign to the input <see cref="TextDocument"/>s</item>
    /// <item>Assign any relevant extra information, like <see cref="SpaceLabel"/>s for the documents contained</item>
    /// </list>
    /// <para>Once ready, call desired API method of the library and pass the OperationContext to it.</para>
    /// <para>Depending on the method, the OperationContext instance may contain:</para>
    /// <list type="bullet">
    /// <item>The <see cref="SpaceCorpora"/> constructed or updated by the procedure</item>
    /// <item>Set of <see cref="SpaceDocumentModel"/>s, constructed from the input documents</item>
    /// <item>Set of extracted topics <see cref="SpaceTopic"/>, if a method of latent semantic analysis, clusterization or other data mining method was performed</item>
    /// </list>
    /// </remarks>
    public class ToolkitOperationContext
    {
        public SpaceModelConstructor constructor { get; set; } = new SpaceModelConstructor();

        public SpaceModel space { get; protected set; } = new SpaceModel();
    }
}