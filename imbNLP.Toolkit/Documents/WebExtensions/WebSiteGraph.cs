using imbSCI.Graph.FreeGraph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Documents.WebExtensions
{
    /// <summary>
    /// Graph structure describing a web site
    /// </summary>
    /// <seealso cref="imbSCI.Graph.FreeGraph.freeGraph" />
    [Serializable]
    public class WebSiteGraph : freeGraph
    {


        public WebSiteGraphDiagnosticMark diagnosticResults { get; set; } = WebSiteGraphDiagnosticMark.none;

        public WebSiteGraph() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSiteGraph"/> class.
        /// </summary>
        /// <param name="_name">The name - web domain</param>
        /// <param name="_description">Optional description attached to the web graph</param>
        public WebSiteGraph(String _name, String _description)
        {
            name = _name;
            description = _description;

        }

        /// <summary>
        /// Gets all visited.
        /// </summary>
        /// <returns></returns>
        public List<freeGraphNodeBase> GetAllVisited()
        {
            return nodes.Where(x => x.type == NodeType_InternalVisited).ToList();
        }


        public const Int32 NodeType_InternalVisited = 1;
        public const Int32 NodeType_InternalUnvisited = 2;
        public const Int32 NodeType_External = 3;
    }
}
