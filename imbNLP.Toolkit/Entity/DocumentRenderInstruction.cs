using HtmlAgilityPack;
using System;
using System.Text;

namespace imbNLP.Toolkit.Entity
{

    /// <summary>
    /// Special HTML rendering instruction
    /// </summary>
    public class DocumentRenderInstruction
    {
        /// <summary>
        /// Gets the title instruction.
        /// </summary>
        /// <returns></returns>
        public static DocumentRenderInstruction GetTitleInstruction()
        {
            return new DocumentRenderInstruction("Page title", "/html/head/title/text()");
        }

        /// <summary>
        /// Gets special instruction for visible text rendering, <see cref="BODYTEXT_NAME"/>
        /// </summary>
        /// <returns></returns>
        public static DocumentRenderInstruction GetBodyTextInstruction()
        {
            return new DocumentRenderInstruction(BODYTEXT_NAME, "");
        }

        /// <summary>
        /// Gets the description instruction.
        /// </summary>
        /// <returns></returns>
        public static DocumentRenderInstruction GetDescriptionInstruction()
        {
            //$x("/html/head/meta[@name='Description'] @Content")
            return new DocumentRenderInstruction("Meta description", "/html/head/meta[@name='Description']");
        }

        /// <summary>
        /// Gets the content instruction.
        /// </summary>
        /// <returns></returns>
        public static DocumentRenderInstruction GetContentInstruction()
        {
            //$x("/html/head/meta[@name='Description'] @Content")
            return new DocumentRenderInstruction("Content", "/html/body/*");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentRenderInstruction"/> class.
        /// </summary>
        /// <param name="_name">The name.</param>
        /// <param name="_xpath">The xpath.</param>
        public DocumentRenderInstruction(String _name, String _xpath, Double _weight = 1.0)
        {
            name = _name;
            XPath = _xpath;
            weight = _weight;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentRenderInstruction"/> class.
        /// </summary>
        public DocumentRenderInstruction()
        {

        }

        /// <summary>
        /// This is willcard instruction <see cref="name"/>, executing render of all visible text nodes within the body tag
        /// </summary>
        public const String BODYTEXT_NAME = "::BODYTEXT::";
        public const String TITLE_NAME = "::TITLE::"; // = xpath = "/html/head/title/text()";
        public const String DESCRIPTION_NAME = "::DESCRIPTION::";
        /// <summary>
        /// Human readable comment on the instruction
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public String name { get; set; } = "";

        /// <summary>
        /// XPath used for selecting the nodes
        /// </summary>
        /// <value>
        /// The x path.
        /// </value>
        public String XPath { get; set; } = "";

        public Double weight { get; set; } = 1;

        /// <summary>
        /// Renders the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public String Render(HtmlNode node)
        {
            StringBuilder sb = new StringBuilder();
            var matchNodes = node.SelectNodes(XPath);
            if (matchNodes == null) return "";
            foreach (var sel in matchNodes)
            {
                switch (sel.NodeType)
                {
                    case HtmlNodeType.Text:
                        sb.AppendLine(sel.InnerText);
                        break;
                    case HtmlNodeType.Element:

                        String tag = sel.Name.ToLower();
                        switch (tag)
                        {
                            case "script":
                                break;
                            default:
                                sb.AppendLine(sel.InnerText);

                                break;
                        }

                        if (sel.Name == "meta")
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
    }

}