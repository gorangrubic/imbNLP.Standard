using System;

namespace imbNLP.Toolkit.Documents.WebExtensions
{
    /// <summary>
    /// Extended information about <see cref="WebSiteDocuments"/>
    /// </summary>
    [Serializable]
    public class WebSiteDocumentExtensions
    {

        public WebSiteDocumentExtensions()
        {

        }

        /// <summary>
        /// Web Graph
        /// </summary>
        /// <value>
        /// The graph.
        /// </value>
        public WebSiteGraph graph { get; set; } = null;

    }
}
