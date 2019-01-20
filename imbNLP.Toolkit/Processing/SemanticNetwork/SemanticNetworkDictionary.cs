using imbSCI.Core.math;
using imbSCI.Graph.FreeGraph;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Processing
{

    public class SemanticNetworkConstructor
    {
        public SemanticNetworkConstructor()
        {

        }

        public SemanticNetworkDictionary StartProjection()
        {
            SemanticNetworkDictionary output = new SemanticNetworkDictionary();

            return output;
        }


        public void Projection(SemanticNetworkDictionary output, IEnumerable<TokenDictionary> primaryTermSets, IEnumerable<TokenDictionary> secondaryTermSets)
        {

        }


        public void ProjectionEntry(SemanticNetworkDictionary output, TokenDictionary primary, TokenDictionary secondary)
        {

            double primarySizeFactor = 1.GetRatio(primary.Count); //1.GetRatio(primary.GetSumFrequency()) * 1.GetRatio(primary.Count);
            double secondarySizeFactor = 1.GetRatio(secondary.Count); //1.GetRatio(secondary.GetSumFrequency()) * 

            List<string> primaryTokens = primary.GetTokens();
            List<string> secondaryTokens = secondary.GetTokens();

            foreach (string token in primaryTokens)
            {
                double score = primary.GetTokenFrequency(token) * primarySizeFactor;
                output.AddNodeOrSum(token, score, 0);
            }

            foreach (string token in secondaryTokens)
            {
                double score = secondary.GetTokenFrequency(token) * secondarySizeFactor;
                output.AddNodeOrSum(token, score, 0);
            }

            foreach (string p_token in primaryTokens)
            {
                Int32 p_freq = primary.GetTokenFrequency(p_token);

                foreach (string s_token in secondaryTokens)
                {
                    Int32 s_freq = secondary.GetTokenFrequency(s_token);

                    double p2s_score = s_freq / p_freq;

                    freeGraphLinkBase link = new freeGraphLinkBase();
                    link.nodeNameA = p_token;
                    link.nodeNameB = s_token;
                    link.weight = p2s_score;
                    link.type = 0;

                    output.AddLinkOrSum(link);

                }
            }

        }

    }

    public class SemanticNetworkDictionary : freeGraph
    {


    }
}
