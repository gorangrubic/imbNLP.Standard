using imbACE.Core.operations;
using imbACE.Services.consolePlugins;
using imbNLP.Toolkit.Entity;
using imbNLP.Toolkit.Planes;
using imbSCI.Core.data.help;
using imbSCI.Core.files.folders;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace imbNLP.Project.Extensions
{
    public class becDocumentRenderingExtension : instanceLoadSaveExtension<EntityPlaneMethodSettings>
    {
        //public propertyLoadSaveExtension file { get; set; }

        //public EntityPlaneMethodSettings entityMethod { get; set; } = new EntityPlaneMethodSettings();

        public becDocumentRenderingExtension(IAceOperationSetExecutor __parent, folderNode __folder) : base(__folder, __parent)
        {
        }

        [Display(GroupName = "set", Name = "PageFilter", ShortName = "", Description = "Ranks and selects top-n documents from a document set")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will set DocumentFilter function of EntityPlaneMethod")]
        /// <summary>Ranks and selects top-n documents from a document set</summary>
        /// <remarks><para>It will set DocumentFilter function of EntityPlaneMethod</para></remarks>
        /// <param name="function">Name of filter function class, if empty the filter will be disabled</param>
        /// <param name="limit">number of top n pages to select</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setPageFilter(
              [Description("Name of filter function class")] String function = "DocumentEntropyFunction",
              [Description("number of top n pages to select, -2 will leave existing settings")] Int32 limit = -2,
              [Description("--")] Boolean debug = true)
        {
            data.filterFunctionName = function;
            if (limit != -2)
            {
                data.filterLimit = limit;
            }
        }



        [Display(GroupName = "set", Name = "RenderInstruction", ShortName = "", Description = "Instructs HTML-to-text extraction engine (EntityPlaneMethod) to produce text from xpath, tag filter or web graph extracted content")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will add specified instruction to the rendering instruction set, and optionally remove all existing instructions before it.")]
        [imbMeta(imbMetaAttributeEnum.AddExampleInLine, "exp.RenderInstruction name=\"PageNavigationText\";flags=this_page_content;code=\"A\";weight=1.0;expansion=1;remove=true;", "Renders only hyperlink caption (anchor text) of links in the web document", "Navigation labels")]
        [imbMeta(imbMetaAttributeEnum.AddExampleInLine, "exp.RenderInstruction name=\"PageContent\";flags=this_page_content;code=\"\";weight=1.0;expansion=1;remove=false;", "Renders all textual content within body tag of the document, excluding scripts", "Body text")]
        [imbMeta(imbMetaAttributeEnum.AddExampleInLine, "exp.RenderInstruction name=\"PageInboundLinks\";flags=select_inbound_links|link_caption;code=\"\";weight=1.0;expansion=1;remove=true;",
            "Renders anchor texts from inbound links - extracted from related documents, having hyperlinks to the web document", "Inbound navigation captions")]
        /// <summary>
        /// Instructs HTML to text extraction engine (EntityPlaneMethod) to produce text from xpath
        /// </summary>
        /// <param name="name">Instruction name, it is human-readable descriptive name or special instructin name like ::BODYTEXT::</param>
        /// <param name="flags">The flags.</param>
        /// <param name="code">XPath associated with the instruction, selects nodes to be rendered into text</param>
        /// <param name="weight">Weight factor of the instruction, i.e. number of times the content should be repeated (boosting TF)</param>
        /// <param name="expansion">The expansion.</param>
        /// <param name="remove">if set to <c>true</c> [remove].</param>
        /// <remarks>
        /// It will add specified instruction to the rendering instruction set, and optionally remove all existing instructions before it.
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase" />
        public void aceOperation_setRenderInstruction(
              [Description("Instruction name, it is human-readable descriptive name or special instructin name like ::BODYTEXT::")] String name = "::BODYTEXT::",
              [Description("Instruction flags, controls what and how to render")] DocumentRenderInstructionFlags flags = DocumentRenderInstructionFlags.this_page_content,
              [Description("XPath associated with the instruction, selects nodes to be rendered into text")] String code = "",
              [Description("Weight factor of the instruction, i.e. number of times the content should be repeated (boosting TF)")] Double weight = 1.0,
              [Description("Graph selection expansion steps - to reach 1+ edges far nodes ")] Int32 expansion = 1,
            [Description("If true it will remove any existing instruction in the set")] Boolean remove = false)
        {
            if (remove)
            {
                data.instructions.Clear();
            }

            DocumentRenderInstruction dri = new DocumentRenderInstruction(name, code, weight);
            dri.instructionFlags = flags;
            dri.graphExpansionSteps = expansion;
            dri.weight = weight;
            data.instructions.Add(dri);
        }

        public override void SetSubBinding()
        {

        }
    }
}