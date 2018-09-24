using imbNLP.PartOfSpeech.pipeline.core;
using imbNLP.PartOfSpeech.pipelineForPos.node;
using imbNLP.PartOfSpeech.pipelineForPos.subject;
using imbSCI.Core.extensions.data;
using imbSCI.Core.extensions.enumworks;
using imbSCI.Core.extensions.text;
using imbSCI.Data.collection.nested;
using imbSCI.Graph.DGML;
using imbSCI.Graph.DGML.core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace imbNLP.PartOfSpeech.pipelineForPos.render
{
    using imbSCI.Core.style.color;

    /// <summary>
    /// Converts pipeline model (<see cref="IPipelineModel"/> to <see cref="DirectedGraph"/>
    /// </summary>
    public class pipelineToDirectedGraphConvertor
    {
        public pipelineToDirectedGraphConvertor()
        {
        }

        public void SetCategory(Node nd, IPipelineNode node)
        {
            if (node.nodeType.HasFlag(pipelineNodeTypeEnum.taskBuilder)) nd.Category = pipelineNodeTypeEnum.taskBuilder.ToInt32().ToString();
            else if (node.nodeType.HasFlag(pipelineNodeTypeEnum.distributor)) nd.Category = pipelineNodeTypeEnum.distributor.ToInt32().ToString();
            else if (node.nodeType.HasFlag(pipelineNodeTypeEnum.transformer)) nd.Category = pipelineNodeTypeEnum.transformer.ToInt32().ToString();
            else if (node.nodeType.HasFlag(pipelineNodeTypeEnum.bin)) nd.Category = pipelineNodeTypeEnum.bin.ToInt32().ToString();
            else if (node.nodeType.HasFlag(pipelineNodeTypeEnum.model)) nd.Category = pipelineNodeTypeEnum.model.ToInt32().ToString();
            else
            {
                nd.Category = pipelineNodeTypeEnum.none.ToInt32().ToString();
            }
        }

        private Dictionary<IPipelineNode, Node> nodeRegistry = new Dictionary<IPipelineNode, Node>();
        private aceDictionarySet<IPipelineNode, IPipelineNode> ordinalSet = new aceDictionarySet<IPipelineNode, IPipelineNode>();

        public Link AddAndLink(DirectedGraph output, Node parent, IPipelineNode node, List<IPipelineNode> newNextSet, Color linkColor, String label = "")
        {
            if (node == null) return null;
            if (parent == null) return null;

            if (!newNextSet.Contains(node)) newNextSet.Add(node);

            var tNode = output.Nodes.AddNode(node.path, node.Label);

            if (!nodeRegistry.ContainsKey(node)) nodeRegistry.Add(node, tNode);

            Link l = output.Links.AddLink(parent, tNode, label);
            l.Stroke = linkColor.ColorToHex(); //.ColorToHex();

            if (node is IPipelineNodeBin)
            {
                if (node.model != null)
                {
                    if (node == node.model.trashBin)
                    {
                        l.Label = "Rejected";
                        l.StrokeDashArray = "5,5,2,2";
                    }
                    else
                    {
                        l.Label = "Accepted";
                    }
                }
            }

            return l;

            //return null;
        }

        /// <summary>
        /// Automatically sets label
        /// </summary>
        public String GetLabel(Object node)
        {
            String ln = node.GetType().Name.Replace("pipeline", "");
            ln = ln.Replace("Node", "");
            ln = ln.Replace("Transformer", "");
            return ln.imbTitleCamelOperation(true); // + " [" + nodeType.ToString() + "]";
        }

        /// <summary>
        /// Generates simple-styled graph
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="colors">The colors: Color.LightGray, Color.WhiteSmoke, Color.OrangeRed, Color.CadetBlue, Color.Yellow, Color.SteelBlue, Color.Orchid</param>
        /// <param name="limit">The limit.</param>
        /// <returns></returns>
        public DirectedGraph GetModelGraph(pipelineModel<pipelineTaskSubjectContentToken> model, Color[] colors = null, Int32 limit = 10)
        {
            if (colors == null)
            {
                colors = new Color[] { Color.LightGray, Color.WhiteSmoke, Color.OrangeRed, Color.GreenYellow, Color.Yellow, Color.SteelBlue, Color.Orchid, Color.Yellow, Color.SteelBlue, Color.Orchid };
            }
            DirectedGraph output = new DirectedGraph();

            output.Title = model.name;

            output.Layout = imbSCI.Graph.DGML.enums.GraphLayoutEnum.Sugiyama;
            output.NeighborhoodDistance = 20;

            var st_basic = output.Categories.AddOrGetCategory(0, nameof(pipelineNodeTypeEnum.none), "");
            var st_taskBuilder = output.Categories.AddOrGetCategory(pipelineNodeTypeEnum.taskBuilder.ToInt32(), nameof(pipelineNodeTypeEnum.taskBuilder), nameof(pipelineNodeTypeEnum.none));
            var st_distributor = output.Categories.AddOrGetCategory(pipelineNodeTypeEnum.distributor.ToInt32(), nameof(pipelineNodeTypeEnum.distributor), nameof(pipelineNodeTypeEnum.none));
            var st_transformer = output.Categories.AddOrGetCategory(pipelineNodeTypeEnum.transformer.ToInt32(), nameof(pipelineNodeTypeEnum.transformer), nameof(pipelineNodeTypeEnum.none));
            var st_bin = output.Categories.AddOrGetCategory(pipelineNodeTypeEnum.bin.ToInt32(), nameof(pipelineNodeTypeEnum.bin), nameof(pipelineNodeTypeEnum.none));
            var st_model = output.Categories.AddOrGetCategory(pipelineNodeTypeEnum.model.ToInt32(), nameof(pipelineNodeTypeEnum.model), nameof(pipelineNodeTypeEnum.none));

            st_basic.Stroke = colors[0].ColorToHex();
            st_basic.StrokeThinkness = 2;
            st_basic.Background = colors[1].ColorToHex();

            st_bin.StrokeDashArray = "5,2,5,2";

            st_taskBuilder.Background = colors[2].ColorToHex();
            st_transformer.Background = colors[3].ColorToHex();

            st_distributor.Background = colors[4].ColorToHex();

            st_bin.Background = colors[5].ColorToHex();

            st_model.Background = colors[6].ColorToHex();

            List<IPipelineNode> totalSet = new List<IPipelineNode>();
            List<IPipelineNode> nextSet = new List<IPipelineNode>();
            nextSet.Add(model);
            Int32 i = 0;

            Node modelMainNode = null;
            Node trashBinNode = null;
            Node exitBinNode = null;

            while (nextSet.Any())
            {
                i++;
                List<IPipelineNode> newNextSet = new List<IPipelineNode>();
                // <---------------------------------------------------[ * ]
                foreach (IPipelineNode parent in nextSet)
                {
                    if (totalSet.Contains(parent)) continue;

                    if (parent == null) continue;
                    parent.SetLabel();

                    Node parentNode = output.Nodes.AddNode(parent.path, parent.Label);

                    if (parentNode == null) continue;

                    // <-------------------------------------------------------------------------------  //
                    if (parent.parent != null) ordinalSet.Add(parent.parent as IPipelineNode, parent);  //
                    // <-----------------------------------------------------------------------------  //

                    if (!nodeRegistry.ContainsKey(parent)) nodeRegistry.Add(parent, parentNode);

                    SetCategory(parentNode, parent);

                    if (parent is IPipelineModel pipeModel)
                    {
                        trashBinNode = output.Nodes.AddNode(pipeModel.trashBin.path, pipeModel.trashBin.Label);
                        exitBinNode = output.Nodes.AddNode(pipeModel.exitBin.path, pipeModel.exitBin.Label);
                        trashBinNode.Category = pipelineNodeTypeEnum.bin.ToInt32().ToString();
                        exitBinNode.Category = pipelineNodeTypeEnum.bin.ToInt32().ToString();

                        if (!nodeRegistry.ContainsKey(pipeModel.trashBin)) nodeRegistry.Add(pipeModel.trashBin, trashBinNode);

                        if (!nodeRegistry.ContainsKey(pipeModel.exitBin)) nodeRegistry.Add(pipeModel.exitBin, exitBinNode);

                        //var trashNode = output.Nodes.AddNode(.path, pipeModel.trashBin.Label);

                        modelMainNode = parentNode;

                        //  AddAndLink(output, parentNode, pipeModel.exitBin, newNextSet, Color.Green).Label = "Output";
                    }

                    if (parent is pipelineContentTokenLevelDistribution distNode)
                    {
                        if (distNode != null)
                        {
                            AddAndLink(output, parentNode, distNode?.repoPipeline, newNextSet, Color.Violet, "Repository");
                            AddAndLink(output, parentNode, distNode?.sitePipeline, newNextSet, Color.Violet, "Site");
                            AddAndLink(output, parentNode, distNode?.pagePipeline, newNextSet, Color.Violet, "Page");
                            AddAndLink(output, parentNode, distNode?.blockPipeline, newNextSet, Color.Violet, "Block");
                            AddAndLink(output, parentNode, distNode?.streamPipeline, newNextSet, Color.Violet, "Stream");
                            AddAndLink(output, parentNode, distNode?.chunkPipeline, newNextSet, Color.Violet, "Chunk");
                            AddAndLink(output, parentNode, distNode?.tokenPipeline, newNextSet, Color.Violet, "Token");
                        }
                    }
                    else if (parent.nodeType == pipelineNodeTypeEnum.distributor)
                    {
                        parentNode.Label = GetLabel(parent);
                    }
                    else
                    {
                        var lNext = AddAndLink(output, parentNode, parent.next, newNextSet, Color.SteelBlue, "Next");
                        if (parent.next == model.exitBin)
                        {
                            if (lNext != null)
                            {
                                lNext.StrokeDashArray = "2,3,2,3";
                                lNext.Label = "Done";
                            }
                        }

                        var lForward = AddAndLink(output, parentNode, parent.forward, newNextSet, Color.OrangeRed, "Forward");
                    }

                    if (parent.nodeType == pipelineNodeTypeEnum.distributor)
                    {
                        var tl = output.Links.AddLink(parentNode, trashBinNode, "Removed");
                        tl.Stroke = Color.Gray.ColorToHex();
                        tl.StrokeDashArray = "2,2,5,2,5";
                        tl.StrokeThinkness = 8;
                    }

                    if (parent.GetType().Name.Contains("TaskBuilder"))
                    {
                        if (modelMainNode != null)
                        {
                            var ntl = output.Links.AddLink(parentNode, modelMainNode, "New Tasks");
                            ntl.Stroke = Color.MediumVioletRed.ColorToHex();
                            ntl.StrokeDashArray = "2,2,5,2,5";
                            ntl.StrokeThinkness = 8;
                        }
                    }

                    if (parent.parent != null)
                    {
                        IPipelineNode parentFolder = parent.parent as IPipelineNode;

                        var l = new Link(parentFolder.path, parentNode.Id, true);

                        if (parentFolder.next == parent)
                        {
                            l.Stroke = Color.SteelBlue.ColorToHex();
                            l.Label = "No";
                            l.StrokeThinkness = 4;
                            output.Links.Add(l);
                        }
                        else if (parentFolder.forward == parent)
                        {
                            l.Stroke = Color.OrangeRed.ColorToHex();
                            l.Label = "Execute";
                            l.StrokeThinkness = 6;
                            output.Links.Add(l);
                        }
                        else
                        {
                            l.Stroke = Color.DarkGray.ColorToHex();
                            l.Label = "Owner";
                            l.StrokeDashArray = "2,5,2,5";
                            l.StrokeThinkness = 2;
                        }
                        // output.Links.Add(l);
                    }

                    foreach (var pair in parent)
                    {
                        if (!newNextSet.Contains(pair))
                        {
                            newNextSet.Add(pair as IPipelineNode);
                            totalSet.Add(pair as IPipelineNode);
                        }
                    }
                }

                if (i > limit) break;
                nextSet = newNextSet;
            }
            // <---------------------------------------------------[ * ]

            // <---------------------------------------------------[ * ]
            foreach (var pair in ordinalSet)
            {
                IPipelineNode firstPipelineNode = null;
                Node firstNode = null;

                IPipelineNode currentPipelineNode = null;
                Node currentNode = null;

                foreach (IPipelineNode p in pair.Value)
                {
                    if (firstPipelineNode == null)
                    {
                        firstPipelineNode = p;
                        firstNode = nodeRegistry[firstPipelineNode];
                    }
                    else
                    {
                        //var ld = output.Links.AddLink(firstNode, nodeRegistry[p], "Next");
                        //ld.Stroke = Color.SteelBlue.ColorToHex();
                        //ld.StrokeDashArray = "2,5,2,5";
                        //ld.StrokeThinkness = 4;
                        firstNode = nodeRegistry[p];
                    }
                    currentNode = nodeRegistry[p];
                    currentPipelineNode = p;
                }

                if (pair.Key.level > 1)
                {
                    var l = output.Links.AddLink(currentNode, nodeRegistry[model.exitBin], "Done");
                    l.Stroke = Color.SteelBlue.ColorToHex();
                    //l.StrokeDashArray = "";
                    l.StrokeThinkness = 4;
                }
            }
            // <---------------------------------------------------[ * ]

            // <---------------------------------------------------[ * ]
            var allEnums = Enum.GetValues(typeof(pipelineNodeTypeEnum));

            var LEGEND = output.Nodes.AddNode("LEGEND");

            foreach (pipelineNodeTypeEnum en in allEnums)
            {
                var n = output.Nodes.AddNode("LEG" + en.ToString(), en.ToString().imbTitleCamelOperation(true));
                n.Category = en.ToInt32().ToString();
                var l = output.Links.AddLink(LEGEND, n, "");
                l.StrokeDashArray = "5,5,5,5";
            }
            // <---------------------------------------------------[ * ]

            //var st_basic = output.Categories.AddOrGetCategory(0, nameof(pipelineNodeTypeEnum.none), "");
            //var st_taskBuilder = output.Categories.AddOrGetCategory(pipelineNodeTypeEnum.taskBuilder.ToInt32(), nameof(pipelineNodeTypeEnum.taskBuilder), nameof(pipelineNodeTypeEnum.none));
            //var st_distributor = output.Categories.AddOrGetCategory(pipelineNodeTypeEnum.distributor.ToInt32(), nameof(pipelineNodeTypeEnum.distributor), nameof(pipelineNodeTypeEnum.none));
            //var st_transformer = output.Categories.AddOrGetCategory(pipelineNodeTypeEnum.transformer.ToInt32(), nameof(pipelineNodeTypeEnum.transformer), nameof(pipelineNodeTypeEnum.none));
            //var st_bin = output.Categories.AddOrGetCategory(pipelineNodeTypeEnum.bin.ToInt32(), nameof(pipelineNodeTypeEnum.bin), nameof(pipelineNodeTypeEnum.none));
            //var st_model = output.Categories.AddOrGetCategory(pipelineNodeTypeEnum.model.ToInt32(), nameof(pipelineNodeTypeEnum.model), nameof(pipelineNodeTypeEnum.none));

            return output;
        }
    }
}