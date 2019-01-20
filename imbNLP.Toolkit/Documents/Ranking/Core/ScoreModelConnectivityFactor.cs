using imbNLP.Toolkit.Documents.Ranking.Data;
using imbNLP.Toolkit.Documents.WebExtensions;
using imbSCI.Core.reporting;
using imbSCI.Data;
using imbSCI.Data.collection.math;
using imbSCI.Graph.FreeGraph;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;


namespace imbNLP.Toolkit.Documents.Ranking.Core
{
    public class ScoreModelConnectivityFactor : ScoreModelFactorBase
    {
        public override String GetSignature()
        {
            String output = algorithm.ToString().Replace(", ", "_");

          

            output += GetWeightSignature();

            return output;
        }


        public ScoreModelConnectivityFactor()
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
                    String output = algorithm.ToString().Replace(", ", "_") + weight.ToString("F2");
                    return output;
                }
                return _name;
            }
            set
            {

                _name = value;
            }
        }

        public override ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null)
        {
            if (requirements == null) requirements = new ScoreModelRequirements();
            requirements.MayUseGraph = true;

            return requirements;
        }


        public GraphFactorAlgorithm algorithm { get; set; } = GraphFactorAlgorithm.HITS;

        public Double convergence { get; set; } = 0.00001;
        public Double alpha { get; set; } = 0.065;

        public Int32 steps { get; set; } = 200;
        public Int32 scoreUnit { get; set; } = 100;

        protected Dictionary<String, HITSRank> p_hits { get; set; } = new Dictionary<string, HITSRank>();
        protected Dictionary<String, Dictionary<String, Int32>> p_rank { get; set; } = new Dictionary<string, Dictionary<String, Int32>>();
        protected Dictionary<String, aceRelationMatrix<string, string, Int32>> p_matrix { get; set; } = new Dictionary<string, aceRelationMatrix<string, string, Int32>>();

        /// <summary>
        /// Prepares the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        public override void Prepare(DocumentSelectResult context, ILogBuilder log)
        {
            var byDomain = context.GetByDomain(log);

            foreach (var pair in byDomain)
            {
                WebSiteGraph webSiteGraph = context.domainNameToGraph[pair.Key];

                var matrix = webSiteGraph.GetIDMatrix(scoreUnit);
                p_matrix.Add(pair.Key, matrix);

                switch (algorithm)
                {
                    case GraphFactorAlgorithm.HITS:
                        HITSRank hits = new HITSRank();
                        hits.recalculate(matrix, convergence, steps);
                        p_hits.Add(pair.Key, hits);
                        break;
                    case GraphFactorAlgorithm.PageRank:



                        var pageRank = new PageRank(matrix.GetMatrix(), alpha, convergence, steps);

                        double[] dbl = pageRank.ComputePageRank();
                        List<Int32> pri = new List<Int32>();
                        foreach (Double db in dbl)
                        {
                            pri.Add(Convert.ToInt32(db * scoreUnit));
                        }
                        var ranks = new Dictionary<String, Int32>();
                        ranks = matrix.MapToX(pri);

                        p_rank.Add(pair.Key, ranks);


                        break;
                }
            }




        }

        public override double Score(DocumentSelectResultEntry entry, DocumentSelectResult context, ILogBuilder log)
        {
            // WebSiteGraph webSiteGraph = context.domainNameToGraph[entry.DomainID];

            Double score = 0;

            //var matrix = webSiteGraph.GetIDMatrix();

            switch (algorithm)
            {
                case GraphFactorAlgorithm.HITS:
                    if (p_hits.ContainsKey(entry.DomainID))
                    {

                        HITSRank hits = p_hits[entry.DomainID];

                        if (hits.ContainsKey(entry.AssignedID))
                        {
                            score = hits[entry.AssignedID] * scoreUnit;
                        }
                    }

                    break;
                case GraphFactorAlgorithm.PageRank:

                    if (p_rank[entry.DomainID].ContainsKey(entry.AssignedID))
                    {
                        score = p_rank[entry.DomainID][entry.AssignedID];
                    }


                    break;
            }


            return score;
        }
    }
}