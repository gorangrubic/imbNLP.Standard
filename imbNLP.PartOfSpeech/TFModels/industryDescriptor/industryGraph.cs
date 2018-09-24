using imbSCI.Graph.FreeGraph;
using System;
using System.ComponentModel;

namespace imbNLP.PartOfSpeech.TFModels.industryDescriptor
{
    public enum industryTermType
    {
        unknown,

        /// <summary>
        /// The product type: e.g. stove, shelf
        /// </summary>
        productType,

        /// <summary>
        /// The product feature: e.g. high-voltage, salepoint (furniture)
        /// </summary>
        productFeature,

        /// <summary>
        /// The product function: e.g. heating
        /// </summary>
        productFunction,

        /// <summary>
        /// The product consumable: e.g. pelet, fuel
        /// </summary>
        productConsumable,

        /// <summary>
        /// The product part: e.g. tube, radiator, CNC
        /// </summary>
        productPart,
    }

    public class industryTerm
    {
        public industryTerm()
        {
        }

        public industryTermType termType { get; set; } = industryTermType.unknown;

        public String name { get; set; }

        public String translation { get; set; }
    }

    public class industryInformation
    {
        /// <summary> name of the industry </summary>
        [Category("Label")]
        [DisplayName("name")] //[imb(imbAttributeName.measure_letter, "")]
        [Description("name of the industry")] // [imb(imbAttributeName.reporting_escapeoff)]
        public String name { get; set; } = default(String);

        /// <summary> description of the industry </summary>
        [Category("Label")]
        [DisplayName("myProperty")] //[imb(imbAttributeName.measure_letter, "")]
        [Description("description of the industry")] // [imb(imbAttributeName.reporting_escapeoff)]
        public String description { get; set; } = default(String);

        public industryGraph graph { get; set; } = new industryGraph();

        public industryInformation()
        {
        }
    }

    public class industryGraph : freeGraph
    {
    }
}