using imbNLP.Toolkit.Documents.Ranking.Data;
using imbNLP.Toolkit.Documents.WebExtensions;
using imbSCI.Core.reporting;
using imbSCI.Data;
using imbSCI.Graph.FreeGraph;
using System;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Documents.Ranking.Core
{



    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Documents.Ranking.Core.ScoreModelFactorBase" />
    public class ScoreModelGraphFactor : ScoreModelFactorBase
    {
        public override String GetSignature()
        {
            String output = functionFlags.ToString().Replace(", ", "_");

           

            output += GetWeightSignature();

            return output;
        }


        public ScoreModelGraphFactor()
        {

        }

      

        [XmlIgnore]
        public String _name { get; set; } = "";

        public override string name
        {
            get
            {
                if (_name.isNullOrEmpty())
                {
                    return GetSignature();
                }
                return _name;
            }
            set
            {

                _name = value;
            }
        }


        /// <summary>
        /// Controls how the score is computed
        /// </summary>
        /// <value>
        /// The function flags.
        /// </value>
        public GraphFactorFunctionEnum functionFlags { get; set; } = GraphFactorFunctionEnum.count_inbound | GraphFactorFunctionEnum.count_outbound | GraphFactorFunctionEnum.divide_by_graphnodes;




        public override ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null)
        {
            if (requirements == null) requirements = new ScoreModelRequirements();
            requirements.MayUseGraph = true;
            return requirements;
        }

        /// <summary>
        /// The graph registry
        /// </summary>
        // protected Dictionary<String, WebSiteGraph> GraphRegistry = new Dictionary<string, WebSiteGraph>();

        /// <summary>
        /// Prepares the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        public override void Prepare(DocumentSelectResult context, ILogBuilder log)
        {

        }

        /// <summary>
        /// Scores the specified entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public override double Score(DocumentSelectResultEntry entry, DocumentSelectResult context, ILogBuilder log)
        {
            Double score = 0;



            WebSiteGraph webSiteGraph = context.domainNameToGraph[entry.DomainID]; // GraphRegistry[entry.DomainID];

            freeGraphNodeAndLinks outLinks = webSiteGraph.GetLinks(entry.AssignedID, true, false);
            freeGraphNodeAndLinks inLinks = webSiteGraph.GetLinks(entry.AssignedID, false, true);

            if (functionFlags.HasFlag(GraphFactorFunctionEnum.count_outbound))
            {
                score += outLinks.Count;
            }

            if (functionFlags.HasFlag(GraphFactorFunctionEnum.count_inbound))
            {
                score += inLinks.Count;
            }

            if (score == 0)
            {
                return score;
            }

            if (functionFlags.HasFlag(GraphFactorFunctionEnum.divide_by_graphlinks))
            {
                score = score / webSiteGraph.CountLinks();
            }

            if (functionFlags.HasFlag(GraphFactorFunctionEnum.divide_by_graphnodes))
            {
                score = score / webSiteGraph.CountNodes();
            }

            if (functionFlags.HasFlag(GraphFactorFunctionEnum.divide_by_inbound))
            {
                score = score / inLinks.Count;
            }

            if (functionFlags.HasFlag(GraphFactorFunctionEnum.divide_by_outbound))
            {
                score = score / outLinks.Count;
            }
            if (functionFlags.HasFlag(GraphFactorFunctionEnum.divide_by_linkCount))
            {
                score = score / (inLinks.Count + outLinks.Count);
            }
            return score;
        }
    }
}