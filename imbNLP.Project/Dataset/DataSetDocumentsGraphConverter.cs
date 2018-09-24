using imbSCI.Core.math;
using imbSCI.Core.style.color;
using imbSCI.Graph.Converters;
using imbSCI.Graph.DGML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace imbNLP.Project.Dataset
{
    public static class GraphConverters
    {


        private static Object _documentsConverter_lock = new Object();
        private static DataSetDocumentsGraphConverter _documentsConverter;
        /// <summary>
        /// static and autoinitiated object
        /// </summary>
        public static DataSetDocumentsGraphConverter documentsConverter
        {
            get
            {
                if (_documentsConverter == null)
                {
                    lock (_documentsConverter_lock)
                    {

                        if (_documentsConverter == null)
                        {
                            _documentsConverter = new DataSetDocumentsGraphConverter();
                            // add here if any additional initialization code is required
                        }
                    }
                }
                return _documentsConverter;
            }
        }


        private static Object _DataSetDomainGraphConverter_lock = new Object();
        private static DataSetDomainGraphConverter _DataSetDomainGraphConverter;
        /// <summary>
        /// static and autoinitiated object
        /// </summary>
        public static DataSetDomainGraphConverter DataSetDomainGraphConverter
        {
            get
            {
                if (_DataSetDomainGraphConverter == null)
                {
                    lock (_DataSetDomainGraphConverter_lock)
                    {

                        if (_DataSetDomainGraphConverter == null)
                        {
                            _DataSetDomainGraphConverter = new DataSetDomainGraphConverter();
                            // add here if any additional initialization code is required
                        }
                    }
                }
                return _DataSetDomainGraphConverter;
            }
        }


    }

    public class DataSetDocumentsGraphConverter : graphToDirectedGraphConverterBase<WebDocumentsCategory>
    {
        public DataSetDocumentsGraphConverter() : base()
        {
            this.setup = new imbSCI.Graph.Converters.tools.GraphStylerSettings();
            setup.doAddLinkWeightInTheLabel = false;
            setup.doAddNodeTypeToLabel = false;
            setup.doLinkDirectionFromLowerTypeToHigher = false;
            setup.NodeGradient = ColorGradient.ColorCircleCW;
            setup.LinkGradient = ColorGradient.BlueGrayAtoBPreset;
        }


        public override string GetCategoryID(WebDocumentsCategory nodeOrLink)
        {
            return nodeOrLink.name;
        }

        public override double GetLinkWeight(WebDocumentsCategory nodeA, WebDocumentsCategory nodeB)
        {

            return Math.Max(nodeB.siteDocuments.Count.GetRatio(10, 0.2, 1), 1);
        }

        public override double GetNodeWeight(WebDocumentsCategory node)
        {
            return Math.Max(node.siteDocuments.Count.GetRatio(10, 0.2, 1), 1);
        }

        public override int GetTypeID(WebDocumentsCategory nodeOrLink)
        {
            if (nodeOrLink.isLeaf)
            {
                if (nodeOrLink.siteDocuments.Any())
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                if (nodeOrLink.siteDocuments.Any())
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