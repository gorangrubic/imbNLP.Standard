using imbNLP.Toolkit.Documents.Ranking.Core;
using System;

namespace imbNLP.Toolkit.Entity
{
    /// <summary>
    /// Special HTML rendering instruction
    /// </summary>
    public class DocumentRenderInstruction
    {
        /// <summary>
        /// Queries factors for preprocessing requirements
        /// </summary>
        /// <param name="requirements">The requirements.</param>
        /// <returns></returns>
        public ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null)
        {
            if (requirements == null) requirements = new ScoreModelRequirements();

            if (instructionFlags.HasFlag(DocumentRenderInstructionFlags.rel_page))
            {
                requirements.MayUseGraph = true;
            }

            if (instructionFlags.HasFlag(DocumentRenderInstructionFlags.select_links))
            {
                requirements.MayUseGraph = true;
            }

            if (instructionFlags.HasFlag(DocumentRenderInstructionFlags.cur_page))
            {
                requirements.MayUseTextRender = true;
            }


            return requirements;
        }


        public DocumentRenderInstructionFlags instructionFlags { get; set; } = DocumentRenderInstructionFlags.this_page_content;
        //  public DocumentRenderInstructionTypeEnum renderInstructiontype { get; set; } = DocumentRenderInstructionTypeEnum.xPath;

        public Int32 graphExpansionSteps { get; set; } = 1;

        //public const String XPATH_SELECT_TITLE = "/HTML/HEAD/TITLE/text()"; //html/head/title/text()";
        //public const String XPATH_SELECT_DESCRIPTION = "/HTML/HEAD/META[@NAME='Description']";
        //public const String XPATH_SELECT_BODY = "/HTML/BODY/*";

        public const String XPATH_SELECT_TITLE = "title"; //html/head/title/text()";
        public const String XPATH_SELECT_DESCRIPTION = "meta";
        public const String XPATH_SELECT_BODY = "/html/body/*";

        /// <summary>
        /// Gets the title instruction.
        /// </summary>
        /// <returns></returns>
        public static DocumentRenderInstruction GetTitleInstruction()
        {
            var output = new DocumentRenderInstruction("Page title", "", 1.0);
            output.instructionFlags = DocumentRenderInstructionFlags.cur_page | DocumentRenderInstructionFlags.page_title;
            return output;
        }

        /// <summary>
        /// Gets special instruction for visible text rendering, <see cref="BODYTEXT_NAME"/>
        /// </summary>
        /// <returns></returns>
        public static DocumentRenderInstruction GetBodyTextInstruction()
        {
            var output = new DocumentRenderInstruction("Body text", "", 1.0);
            output.instructionFlags = DocumentRenderInstructionFlags.cur_page | DocumentRenderInstructionFlags.page_content;
            return output;
        }

        /// <summary>
        /// Gets the description instruction.
        /// </summary>
        /// <returns></returns>
        public static DocumentRenderInstruction GetDescriptionInstruction()
        {
            var output = new DocumentRenderInstruction("Meta description", "", 1.0);
            output.instructionFlags = DocumentRenderInstructionFlags.cur_page | DocumentRenderInstructionFlags.page_description;
            return output;
        }

        ///// <summary>
        ///// Gets the content instruction.
        ///// </summary>
        ///// <returns></returns>
        //public static DocumentRenderInstruction GetContentInstruction()
        //{
        //    //$x("/html/head/meta[@name='Description'] @Content")
        //    return new DocumentRenderInstruction("Content", "/html/body/*");
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentRenderInstruction"/> class.
        /// </summary>
        /// <param name="_name">The name.</param>
        /// <param name="_xpath">Code, configuration or other string declaration.</param>
        public DocumentRenderInstruction(String _name, String _xpath, Double _weight = 1.0)
        {
            name = _name;
            code = _xpath;
            weight = _weight;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentRenderInstruction"/> class.
        /// </summary>
        public DocumentRenderInstruction()
        {

        }



        ///// <summary>
        ///// This is willcard instruction <see cref="name"/>, executing render of all visible text nodes within the body tag. 
        ///// </summary>
        ///// <remarks>
        ///// 
        ///// </remarks>
        //public const String BODYTEXT_NAME = "::BODYTEXT::";


        //public const String TITLE_NAME = "::TITLE::"; // = xpath = "/html/head/title/text()";

        //public const String DESCRIPTION_NAME = "::DESCRIPTION::";

        //public const String DESCRIPTION_NAME = "::DESCRIPTION::";




        /// <summary>
        /// Human readable comment on the instruction
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public String name { get; set; } = "";

        /// <summary>
        /// XPath used for selecting the nodes. Format for tag name filter: h1|h2  or !a|!nav
        /// </summary>
        /// <value>
        /// The x path.
        /// </value>
        public String code { get; set; } = "";

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>
        /// The weight.
        /// </value>
        public Double weight { get; set; } = 1;


    }

}