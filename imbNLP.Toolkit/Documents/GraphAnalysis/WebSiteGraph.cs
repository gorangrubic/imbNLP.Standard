using HtmlAgilityPack;

using imbSCI.Core.reporting;
using imbSCI.Data;
using imbSCI.Graph.FreeGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace imbNLP.Toolkit.Documents.GraphAnalysis
{


    public class WebSiteGraph:freeGraph
    {
        public WebSiteGraph() { }

        public WebSiteGraph(String _name, String _description)
        {
            name = _name;
            description = _description;

        }

        public const Int32 NodeType_InternalVisited = 1;
        public const Int32 NodeType_InternalUnvisited = 2;
        public const Int32 NodeType_External = 3;
    }
}
