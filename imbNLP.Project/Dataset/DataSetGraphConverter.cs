using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using imbSCI.Core.math;
using imbSCI.Core.style.color;
using imbSCI.Graph.Converters;
using imbSCI.Graph.DGML;

namespace imbNLP.Project.Dataset
{
    public class DataSetDomainGraphConverter : graphToDirectedGraphConverterBase<WebDomainCategory>
    {
        public DataSetDomainGraphConverter():base()
        {
            this.setup = new imbSCI.Graph.Converters.tools.GraphStylerSettings();
            setup.doAddLinkWeightInTheLabel = false;
            setup.doAddNodeTypeToLabel = false;
            setup.doLinkDirectionFromLowerTypeToHigher = false;
            setup.NodeGradient = ColorGradient.ColorCircleCW;
            setup.LinkGradient = ColorGradient.BlueGrayAtoBPreset;
        }

        public override string GetCategoryID(WebDomainCategory nodeOrLink)
        {
            return nodeOrLink.name;
        }

        public override double GetLinkWeight(WebDomainCategory nodeA, WebDomainCategory nodeB)
        {

            return Math.Max(nodeB.sites.Count.GetRatio(10, 0.2, 1), 1);
        }

        public override double GetNodeWeight(WebDomainCategory node)
        {
            return Math.Max(node.sites.Count.GetRatio(10, 0.2, 1), 1);
        }

        public override int GetTypeID(WebDomainCategory nodeOrLink)
        {
            if (nodeOrLink.isLeaf)
            {
                if (nodeOrLink.sites.Any())
                {
                    return 1;
                } else
                {
                    return 0;
                }
            } else
            {
                if (nodeOrLink.sites.Any())
                {
                    return 2;
                }
                else
                {
                    return 1;
                }
            }
        }
    }
}
