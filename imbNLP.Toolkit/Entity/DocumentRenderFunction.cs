using HtmlAgilityPack;
using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Planes.Core;
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
        /// Additional rendering instructions - for meta values, page title...
        /// </summary>
        /// <value>
        /// The instructions.
        /// </value>
        public List<DocumentRenderInstruction> instructions { get; set; } = new List<DocumentRenderInstruction>();

        public override void Describe(ILogBuilder logger)
        {

            if (instructions.Any())
            {
                logger.AppendLine("Additional rendering instructions:");
                Int32 c = 1;
                foreach (var inst in instructions)
                {
                    logger.AppendLine("[" + c.ToString("D2") + "] " + inst.name + " xPath=[" + inst.XPath + "]");
                    c++;
                }
            }
            else
            {
                logger.AppendLine("Without additional rendering instructions (only visible text from content)");
            }

        }

        public TextDocumentSet RenderSiteDocuments(WebSiteDocuments site, ILogBuilder logger)
        {
            TextDocumentSet textSet = new TextDocumentSet(site.domain);

            foreach (WebSiteDocument webPage in site.documents)
            {
                var pg = RenderText(webPage.HTMLSource);
                pg.name = webPage.path;
                textSet.Add(pg);
            }

            return textSet;
        }

        /// <summary>
        /// Renders the specified set into List of <see cref="TextDocumentSet"/>s
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
                TextDocumentSet textSet = RenderSiteDocuments(webSite, logger);
                ti++;
                Double done = ti.GetRatio(target);
                logger.Append(" [" + done.ToString("P2") + "] ");
                textSetForLabel.Add(textSet);
            }

            return textSetForLabel;

        }


        protected String TrimEmptySpace(String input)
        {
            List<String> lines = input.SplitSmart(Environment.NewLine, "", true, false);

            StringBuilder sb = new StringBuilder();

            Int32 whiteLine = 0;
            Int32 whiteLineLimit = 1;
            foreach (String ln in lines.ToList())
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

            return sb.ToString();

        }

        protected TextDocumentLayer BodyTextRender(HtmlDocument htmlDoc, DocumentRenderInstruction instruction)
        {
            TextDocumentLayer mainLayer = new TextDocumentLayer("", TextDocumentLayer.MAINTEXT_LAYER, Convert.ToInt32(instruction.weight)); //output.CreateLayer(TextDocumentLayer.MAINTEXT_LAYER, "");
            StringBuilder sb = new StringBuilder();

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
                                if (desc.ParentNode.Name != "script")
                                {
                                    textNodes.Add(desc);
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
                    sb.AppendLine(n.InnerText);
                    // output.AppendLine(n.InnerText);
                }
            }

            String render = sb.ToString();

            render = TrimEmptySpace(render);

            mainLayer.content = render;
            return mainLayer;
        }

        /// <summary>
        /// Renders the provided HTML source
        /// </summary>
        /// <param name="htmlDocSource">The HTML document source.</param>
        /// <returns></returns>
        public TextDocumentLayerCollection RenderText(String htmlDocSource)
        {
            TextDocumentLayerCollection output = new TextDocumentLayerCollection();

            HtmlDocument htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(htmlDocSource.ToLower());

            //StringBuilder output = new StringBuilder();

            foreach (DocumentRenderInstruction instruction in instructions)
            {
                if (instruction.name == DocumentRenderInstruction.BODYTEXT_NAME)
                {
                    output.Add(BodyTextRender(htmlDoc, instruction));
                }
                else
                {
                    TextDocumentLayer l = output.CreateLayer(instruction.name, instruction.Render(htmlDoc.DocumentNode), Convert.ToInt32(instruction.weight));
                }
            }




            return output;

        }

    }

}