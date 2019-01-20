using HtmlAgilityPack;
using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Documents.Ranking.Core;
using imbNLP.Toolkit.Planes.Core;
using imbSCI.Core.extensions.text;
using imbSCI.Core.math;
using imbSCI.Core.reporting;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace imbNLP.Toolkit.Entity
{
    /// <summary>
    /// Function that renders HTML Document into document layers
    /// </summary>
    public class DocumentRenderFunction : PlaneMethodFunctionBase, IEntityPlaneFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentRenderFunction"/> class.
        /// </summary>
        public DocumentRenderFunction()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether all documents from same domain should be groupped into common TextDocumentSet
        /// </summary>
        /// <value>
        ///   <c>true</c> if [group site documents]; otherwise, <c>false</c>.
        /// </value>
        public Boolean GroupSiteDocuments { get; set; } = true;

        /// <summary>
        /// Additional rendering instructions - for meta values, page title...
        /// </summary>
        /// <value>
        /// The instructions.
        /// </value>
        public List<DocumentRenderInstruction> instructions { get; set; } = new List<DocumentRenderInstruction>();

        /// <summary>
        /// Queries factors for preprocessing requirements
        /// </summary>
        /// <param name="requirements">The requirements.</param>
        /// <returns></returns>
        public ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null)
        {
            if (requirements == null) requirements = new ScoreModelRequirements();

            foreach (DocumentRenderInstruction inst in instructions)
            {
                inst.CheckRequirements(requirements);
            }

            return requirements;
        }

        /// <summary>
        /// Describes the specified logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public override void Describe(ILogBuilder logger)
        {
            if (instructions.Any())
            {
                logger.AppendLine("Rendering instructions:");
                Int32 c = 1;
                foreach (var inst in instructions)
                {
                    logger.AppendLine("[" + c.ToString("D2") + "] " + inst.name + " Code=[" + inst.code + "] Flags=[" + inst.instructionFlags.ToString() + "]");
                    c++;
                }
            }
            else
            {
                logger.AppendLine("No rendering instructions");
            }
        }

        /// <summary>
        /// Renders the specified set of WebSiteDocuments into List of <see cref="TextDocumentSet"/>s
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public List<TextDocumentSet> RenderDocumentSet(WebSiteDocumentsSet input, ILogBuilder logger)
        {
            List<TextDocumentSet> textSetForLabel = new List<TextDocumentSet>();
            Int32 target = input.Count;
            Int32 ti = 0;
            foreach (WebSiteDocuments webSite in input)
            {
                //if (GroupSiteDocuments)
                //{
                TextDocumentSet textSet = RenderSiteDocuments(webSite, logger);
                textSetForLabel.Add(textSet);
                //} else
                //{
                //    foreach (WebSiteDocument webPage in webSite.documents)
                //    {
                //        TextDocumentSet textSet = new TextDocumentSet(webPage.AssociatedID);
                //        TextDocumentLayerCollection pg = RenderText(webPage, webSite);
                //        pg.name = webPage.AssociatedID;
                //        textSet.Add(pg);
                //        textSetForLabel.Add(textSet);
                //    }
                //}
                ti++;
                Double done = ti.GetRatio(target);
                logger.Append(" [" + done.ToString("P2") + "] ");
            }
            return textSetForLabel;
        }

        /// <summary>
        /// Renders a web site into set of documents
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public TextDocumentSet RenderSiteDocuments(WebSiteDocuments site, ILogBuilder logger, Boolean EnableRendering = true)
        {
            TextDocumentSet textSet = new TextDocumentSet(site.domain);

            //Parallel.ForEach(site.documents, (webPage) =>
            //{
            //    TextDocumentLayerCollection pg = RenderText(webPage, site, EnableRendering);
            //    pg.name = webPage.AssociatedID;
            //    textSet.Add(pg);
            //});

            foreach (WebSiteDocument webPage in site.documents)
            {
                TextDocumentLayerCollection pg = RenderText(webPage, site, EnableRendering);

                pg.name = webPage.AssignedID;
                textSet.Add(pg);
            }

            return textSet;
        }

        /// <summary>
        /// Renders the provided HTML source
        /// </summary>
        /// <param name="webPage">The web page.</param>
        /// <param name="site">The site.</param>
        /// <param name="EnableRendering">if set to <c>true</c> [enable rendering].</param>
        /// <param name="htmlRelDoc">The HTML relative document.</param>
        /// <returns></returns>
        public TextDocumentLayerCollection RenderText(WebSiteDocument webPage, WebSiteDocuments site, Boolean EnableRendering = true, HtmlDocument htmlRelDoc = null)
        {
            TextDocumentLayerCollection output = new TextDocumentLayerCollection();
            output.name = webPage.AssignedID;

            if (htmlRelDoc == null)
            {
                htmlRelDoc = HtmlDocumentCache.DefaultDocumentCache.GetDocument(webPage.AssignedID, webPage.HTMLSource);
            }

            foreach (DocumentRenderInstruction instruction in instructions)
            {
                String content = "";
                StringBuilder sb = new StringBuilder();

                if (EnableRendering)
                {
                    DocumentRenderInstructionFlags flags = instruction.instructionFlags;

                    if (flags.HasFlag(DocumentRenderInstructionFlags.cur_page))
                    {
                        RenderDocumentAspect(sb, webPage, htmlRelDoc, flags, instruction.code);
                    }
                    if (flags.HasFlag(DocumentRenderInstructionFlags.select_links))
                    {
                        if (site.extensions.graph == null)
                        {
                            throw new nlpException("WebGraph is null - can't render instruction [" + instruction.name + "]", "Graph is null for site [" + site.domain + "]");
                        }

                        imbSCI.Graph.FreeGraph.freeGraphNodeAndLinks selection =
                        site.extensions.graph.GetLinks(webPage.AssignedID,
                        instruction.instructionFlags.HasFlag(DocumentRenderInstructionFlags.select_outbound_links),
                        instruction.instructionFlags.HasFlag(DocumentRenderInstructionFlags.select_inbound_links));

                        foreach (var link in selection.links)
                        {
                            if (flags.HasFlag(DocumentRenderInstructionFlags.link_caption))
                            {
                                if (link.linkLabel.isNullOrEmpty())
                                {
                                }
                                else
                                {
                                }
                                sb.AppendLine(link.linkLabel);
                            }
                        }

                        foreach (var node in selection.linkedNodeClones.Values)
                        {
                            if (flags.HasFlag(DocumentRenderInstructionFlags.select_rel_page))
                            {
                                WebSiteDocument doc = site.documents.Where(x => x.AssignedID == node.name).FirstOrDefault();
                                RenderDocumentAspect(sb, doc, null, flags, instruction.code);
                            }
                        }
                    }

                    content = sb.ToString();

                    if (flags.HasFlag(DocumentRenderInstructionFlags.lower_case))
                    {
                        content = content.ToLower();
                    }

                    if (flags.HasFlag(DocumentRenderInstructionFlags.unique_tokens))
                    {
                        List<string> r = content.getTokens(true, false, true, true, 1);
                        List<String> ur = new List<string>();
                        foreach (String rk in r)
                        {
                            if (!ur.Contains(rk))
                            {
                                ur.Add(rk);
                            }
                        }

                        StringBuilder sbr = new StringBuilder();
                        foreach (String rk in ur)
                        {
                            sbr.Append(rk + " ");
                        }
                        content = sbr.ToString();
                    }

                    content = TrimEmptySpace(content);
                }

                output.CreateLayer(instruction.name, content, Convert.ToInt32(instruction.weight));
            }

            return output;
        }

        /// <summary>
        /// Renders the document aspect.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="doc">The document.</param>
        /// <param name="htmlRelDoc">The HTML relative document.</param>
        /// <param name="flags">The flags.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void RenderDocumentAspect(StringBuilder sb, WebSiteDocument doc, HtmlDocument htmlRelDoc, DocumentRenderInstructionFlags flags, String code = "")
        {
            if (doc != null)
            {
                if (flags.HasFlag(DocumentRenderInstructionFlags.page_title))
                {
                    HtmlNode titleNode = htmlRelDoc.DocumentNode.Descendants("title").FirstOrDefault();
                    if (titleNode != null)
                    {
                        String titleString = titleNode.InnerText;
                        sb.AppendLine(titleString);
                    }

                    //sb.AppendLine(Render(htmlRelDoc.DocumentNode, DocumentRenderInstruction.XPATH_SELECT_TITLE));
                }

                if (flags.HasFlag(DocumentRenderInstructionFlags.page_description))
                {
                    IEnumerable<HtmlNode> metaNodes = htmlRelDoc.DocumentNode.Descendants("meta");
                    String dsc = "";
                    if (metaNodes != null)
                    {
                        foreach (HtmlNode nd in metaNodes)
                        {
                            String metaName = nd.GetAttributeValue("name", "");
                            if (metaName.Equals("description", StringComparison.CurrentCultureIgnoreCase))
                            {
                                dsc += nd.GetAttributeValue("content", "");
                            }
                        }
                    }

                    if (dsc.isNullOrEmpty())
                    {
                    }
                    else
                    {
                        sb.AppendLine(dsc);
                    }

                    // sb.AppendLine(Render(htmlRelDoc.DocumentNode, DocumentRenderInstruction.XPATH_SELECT_DESCRIPTION));
                }

                if (flags.HasFlag(DocumentRenderInstructionFlags.page_content))
                {
                    sb.AppendLine(BodyTextRender(htmlRelDoc, code));
                }

                if (flags.HasFlag(DocumentRenderInstructionFlags.url_tokens))
                {
                    List<string> r = doc.path.getTokens(true, false, true, true, 1);

                    foreach (String rk in r)
                    {
                        sb.Append(rk + " ");
                    }
                }

                if (flags.HasFlag(DocumentRenderInstructionFlags.page_xpath))
                {
                    if (!code.isNullOrEmpty())
                    {
                        sb.AppendLine(Render(htmlRelDoc.DocumentNode, code));
                    }
                }
            }
        }

        /// <summary>
        /// Renders the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="code">X path code</param>
        /// <returns></returns>
        public String Render(HtmlNode node, String code)
        {
            StringBuilder sb = new StringBuilder();
            code = code.Trim();
            if (code.isNullOrEmpty()) return sb.ToString();

            var nav = node.CreateNavigator(); // htmlRelDoc.CreateNavigator()
                                              //XPathExpression xpe = XPathExpression.Compile(code);

            HtmlNodeCollection matchNodes = node.SelectNodes(code);
            if (matchNodes == null)
            {
                matchNodes = node.SelectNodes(code.ToLower());
            }

            System.Xml.XPath.XPathNodeIterator selectedNodes = nav.Select(code);

            if (matchNodes == null)
            {
                return "";
            }
            else
            {
            }
            foreach (var sel in matchNodes)
            {
                switch (sel.NodeType)
                {
                    case HtmlNodeType.Text:
                        sb.AppendLine(sel.InnerText);
                        break;

                    case HtmlNodeType.Element:
                        sb.AppendLine(sel.InnerText);

                        //String tag = sel.Name.ToLower();
                        //switch (tag)
                        //{
                        //    case "script":
                        //        break;
                        //    default:

                        //        break;
                        //}

                        if (sel.Name.Equals("meta", StringComparison.CurrentCultureIgnoreCase))
                        {
                            sb.AppendLine(sel.GetAttributeValue("content", ""));
                        }
                        else
                        {
                        }
                        break;

                    case HtmlNodeType.Comment:
                        break;

                    case HtmlNodeType.Document:
                        break;
                }
            }
            return sb.ToString();
        }

        public static String TripleNewLine { get; set; } = Environment.NewLine + Environment.NewLine + Environment.NewLine;
        public static String DoubleNewLine { get; set; } = Environment.NewLine + Environment.NewLine;

        public static String DoubleSpace { get; set; } = "  ";
        public static String SingleSpace { get; set; } = " ";

        public static Char[] NewLineSplit { get; set; } = new char[] { '\n', '\r' };

        /// <summary>
        /// Trims the empty space.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        protected String TrimEmptySpace(String input)
        {
            if (input.Contains(Environment.NewLine))
            {
                var lines = input.Split(NewLineSplit, StringSplitOptions.RemoveEmptyEntries);

                StringBuilder sb = new StringBuilder();

                Int32 whiteLine = 0;
                Int32 whiteLineLimit = 1;
                foreach (String ln in lines)
                {
                    String ln_e = ln.Trim();
                    Boolean add = true;
                    if (ln_e == "")
                    {
                        whiteLine++;
                        if (whiteLine > whiteLineLimit)
                        {
                            whiteLine = 0;
                            add = false;
                        }
                    }
                    if (add) sb.AppendLine(ln_e);
                }
                input = sb.ToString();
            }
            /*
            while (input.Contains(TripleNewLine))
            {
                input = input.Replace(TripleNewLine, DoubleNewLine);
            }

            while (input.Contains(DoubleSpace))
            {
                input = input.Replace(DoubleSpace, SingleSpace);
            }
            */
            input = input.Trim();

            return input; //.ToString();
        }

        private static List<String> _DefaultBlackList = null;

        /// <summary>
        /// Gets the default black list.
        /// </summary>
        /// <value>
        /// The default black list.
        /// </value>
        public static List<String> DefaultBlackList
        {
            get
            {
                if (_DefaultBlackList == null)
                {
                    _DefaultBlackList = new List<string>();
                    _DefaultBlackList.Add("script");
                    _DefaultBlackList.Add("style");
                }
                return _DefaultBlackList;
            }
        }

        /// <summary>
        /// Bodies the text render.
        /// </summary>
        /// <param name="htmlDoc">The HTML document.</param>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        protected String BodyTextRender(HtmlDocument htmlDoc, String code)
        {
            StringBuilder sb = new StringBuilder();

            List<String> WhiteList = new List<string>();
            List<String> BlackList = new List<string>();

            var tagList = code.SplitSmart(" ", "", true, true);
            foreach (String t in tagList)
            {
                if (t == " ")
                {
                }
                else
                {
                    if (t.Contains("!"))
                    {
                        BlackList.Add(t.Trim('!'));
                    }
                    else
                    {
                        WhiteList.Add(t);
                    }
                }
            }

            if (!BlackList.Any())
            {
                BlackList = DefaultBlackList;
                // BlackList.Add("script");
            }

            //output.CreateLayer(TextDocumentLayer.MAINTEXT_LAYER, "");

            var bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");
            List<HtmlNode> textNodes = new List<HtmlNode>();

            if (bodyNode != null)
            {
                var descendants = bodyNode.Descendants();
                foreach (var desc in descendants)
                {
                    if (desc.NodeType == HtmlNodeType.Text)
                    {
                        if (!desc.HasChildNodes)
                        {
                            if (desc.ParentNode != null)
                            {
                                Boolean pass = !WhiteList.Any();
                                if (!pass)
                                {
                                    pass = WhiteList.Contains(desc.ParentNode.Name.ToLower());
                                }
                                else
                                {
                                    pass = !BlackList.Contains(desc.ParentNode.Name.ToLower());
                                }

                                if (pass)
                                {
                                    textNodes.Add(desc);
                                }
                                else
                                {
                                }
                            }
                        }
                    }
                }
            }

            foreach (var n in textNodes)
            {
                if (n.InnerText != null && n.InnerText != "")
                {
                    //if (n.InnerText.Contains("font: normal normal"))
                    //{
                    //}

                    sb.AppendLine(n.InnerText);
                    // output.AppendLine(n.InnerText);
                }
            }

            String render = sb.ToString();

            render = TrimEmptySpace(render);

            return render;
        }
    }
}