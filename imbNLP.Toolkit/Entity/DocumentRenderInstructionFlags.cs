using System;

namespace imbNLP.Toolkit.Entity
{
///// <summary>
    ///// Render instruction types, controls interpretation of <see cref="DocumentRenderInstruction.code"/> parameter
    ///// </summary>
    //public enum DocumentRenderInstructionTypeEnum
    //{
    //    /// <summary>
    //    /// The code is interpreted as XPath query
    //    /// </summary>
    //    xPath,
    //    /// <summary>
    //    /// The code specifies tag name filter>. The <see cref="code"/> property can specify tag name filter in format [tag1 tag2 tag3] or ![tag1 tag2 tag], the first will render only text from specified tags while the second will render all other than specified tags.
    //    /// </summary>
    //    tagFilter,
    //    /// <summary>
    //    /// Instruction is controled by special instruction name
    //    /// </summary>
    //    special,
    //    /// <summary>
    //    /// Instruction rendering information from the web graph. 
    //    /// The <see cref="code"/> specifies what relationships are rendered: [from, to], while <see cref="DocumentRenderInstruction.name"/> should define what is rendered:
    //    /// [link_caption,link_tokens,rel_page_title,rel_page_description,rel_page_content]
    //    /// </summary>
    //    webGraph,
    //}

    /// <summary>
    /// Defines graph query render instructions. These instructions take 
    /// </summary>
    [Flags]
    public enum DocumentRenderInstructionFlags
    {

        none = 0,

        /// <summary>
        /// Renders relationship caption
        /// </summary>
        link_caption = 1,
        /// <summary>
        /// Renders tokens extracted from the URL
        /// </summary>
        link_tokens = 2,
        /// <summary>
        /// Page title of the related document (if document exists)
        /// </summary>
        page_title = 4,
        /// <summary>
        /// Page description of the related document (if document exists)
        /// </summary>
        page_description = 8,
        /// <summary>
        /// Body content of the related document (if document exists)
        /// </summary>
        page_content = 16,

        page_xpath = 32,

        url_tokens = 64,


        unique_tokens = 128,

        lower_case = 256,


        inbound = 1024,

        outbound = 2048,

        select_links = 4096,

        rel_page = 8192,

        cur_page = 16384,

        select_rel_page = select_links | rel_page,

        select_inbound_links = inbound | select_links,

        select_outbound_links = outbound | select_links,

        this_page_title = cur_page | page_title,
        this_page_description = cur_page | page_description,
        this_page_content = cur_page | page_content,

        rel_page_title = select_rel_page | page_title,
        rel_page_description = select_rel_page | page_description,
        rel_page_content = select_rel_page | page_content,

        


    }
}