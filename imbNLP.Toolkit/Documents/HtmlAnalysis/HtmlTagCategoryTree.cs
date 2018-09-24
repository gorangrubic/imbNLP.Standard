using HtmlAgilityPack;
using imbSCI.Core.extensions.data;
using imbSCI.Core.extensions.table;
using imbSCI.Data;
using imbSCI.DataComplex.tables;
using System;
using System.Collections.Generic;
using System.Data;

namespace imbNLP.Toolkit.Documents.HtmlAnalysis
{

    /// <summary>
    /// Provides statistics on HTML node structures
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Documents.HtmlAnalysis.HtmlTagCounter" />
    public class HtmlTagCategoryTree : HtmlTagCounter
    {

        /// <summary>
        /// Merges the specified item b.
        /// </summary>
        /// <param name="itemB">The item b.</param>
        public void Merge(HtmlTagCategoryTree itemB)
        {

            List<HtmlTagCounter> newNodes = new List<HtmlTagCounter>();

            List<imbSCI.Data.interfaces.IObjectWithPathAndChildren> leaves = itemB.getAllLeafs();

            leaves.ForEach(x => newNodes.Add(x as HtmlTagCounter));


            while (newNodes.Count != 0)
            {
                List<HtmlTagCounter> actNodes = newNodes;
                newNodes = new List<HtmlTagCounter>();
                foreach (HtmlTagCounter n in actNodes)
                {
                    var an = this.GetOrAddCategory(n.path, n.description, n.weight);
                    an.Score += n.Score;

                    foreach (String cn in n.getChildNames())
                    {
                        newNodes.Add(n[cn] as HtmlTagCounter);
                    }


                }
            }


            UnknownTags.Score += itemB.UnknownTags.Score;

            DistinctTags.AddRange(itemB.DistinctTags, true);
            DistinctUnknownTags.AddRange(itemB.DistinctUnknownTags, true);
        }


        /// <summary>
        /// Gets the result list.
        /// </summary>
        /// <returns></returns>
        public List<HtmlTagCounterResult> GetResultList()
        {
            List<HtmlTagCounterResult> output = new List<HtmlTagCounterResult>();

            List<HtmlTagCounter> newNodes = new List<HtmlTagCounter>();
            newNodes.Add(this);

            while (newNodes.Count != 0)
            {
                List<HtmlTagCounter> actNodes = newNodes;
                newNodes = new List<HtmlTagCounter>();
                foreach (HtmlTagCounter n in actNodes)
                {
                    HtmlTagCounterResult result = new HtmlTagCounterResult();
                    result.Path = n.path;
                    result.Name = n.name;
                    result.Description = n.description;
                    result.Tags = n.tag.add(n.alias.toCsvInLine(), ",");
                    result.Score = Convert.ToInt32(n.GetScore(true, false));
                    result.WeightedScore = n.GetScore(true, true);

                    output.Add(result);

                    foreach (HtmlTagCounter sc in n)
                    {
                        newNodes.Add(sc);
                    }
                    //newNodes.AddRange(n);
                }
            }

            return output;
        }

        /// <summary>
        /// Gets the data table.
        /// </summary>
        /// <param name="_name">The name.</param>
        /// <param name="_description">The description.</param>
        /// <returns></returns>
        public DataTable GetDataTable(String _name, String _description)
        {
            DataTableTypeExtended<HtmlTagCounterResult> output = new DataTableTypeExtended<HtmlTagCounterResult>(_name, _description);
            var results = GetResultList();
            results.ForEach(x => output.AddRow(x));

            output.SetAdditionalInfoEntry("Unknown dist. tags", DistinctUnknownTags.Count, "Number of distinct unknown tags");
            output.SetAdditionalInfoEntry("Distinct tags", DistinctTags.Count, "Number of known distinct tags");
            output.SetAdditionalInfoEntry("Unknown tags", UnknownTags.Count(), "Number of unknown distinct tags");

            output.AddExtra("Distinct tags found: [" + DistinctTags.toCsvInLine() + "]");
            output.AddExtra("Unknown tags found: [" + UnknownTags.toCsvInLine() + "]");

            return output;
        }

        /// <summary>
        /// Counts the tags.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        public void CountTags(IEnumerable<HtmlNode> nodes)
        {
            List<HtmlNode> newNodes = new List<HtmlNode>();
            newNodes.AddRange(nodes);

            while (newNodes.Count != 0)
            {
                List<HtmlNode> actNodes = newNodes;
                newNodes = new List<HtmlNode>();
                foreach (HtmlNode n in actNodes)
                {
                    if (n.NodeType == HtmlNodeType.Element)
                    {
                        String nl = n.Name.ToLower();
                        if (counterDictionary.ContainsKey(nl))
                        {
                            counterDictionary[nl].ScoreUp(nl);
                            if (!DistinctTags.Contains(nl))
                            {
                                DistinctTags.Add(nl);
                            }
                        }
                        else
                        {
                            UnknownTags.Score++;
                            if (!DistinctUnknownTags.Contains(nl))
                            {
                                DistinctUnknownTags.Add(nl);
                            }
                        }

                        newNodes.AddRange(n.ChildNodes);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the unknown tags.
        /// </summary>
        /// <value>
        /// The unknown tags.
        /// </value>
        public HtmlTagCounter UnknownTags { get; set; } = new HtmlTagCounter("Unknown", "Tags that are not defined by the HtmlTagCategory tree model", 0);

        //public HtmlTagCounter root { get; set; } = new HtmlTagCounter();

        /// <summary>
        /// Declares standard 
        /// </summary>
        /// <returns></returns>
        public static HtmlTagCategoryTree GetIMBStandardCategoryTree()
        {
            HtmlTagCategoryTree output = new HtmlTagCategoryTree("Tags", "Root category with all HTML tags");

            var tmp = output.GetOrAddCategory("Forms", "Input elements and input form"); //.DefineTags("input", "textarea", "")
            tmp.GetOrAddCategory("Descriptive", "Tags providing description, in the context of data entry form").DefineTags("legend", "label", "output");
            tmp.GetOrAddCategory("Inputs", "Data inputs and related logistics").DefineTags("input", "button", "option", "select", "textarea", "datalist");
            tmp.GetOrAddCategory("Structural", "Tags providing organization and structure of the form elements").DefineTags("form", "optgroup", "fieldset");

            tmp = output.GetOrAddCategory("Multimedia", "Multimedia elements and graphics");
            tmp.GetOrAddCategory("Meta").DefineTags("source", "embed", "iframe", "script"); //.DefineTags("input", "textarea", "")
            tmp.GetOrAddCategory("Images", "").DefineTags("img", "picture", "map", "area");
            tmp.GetOrAddCategory("Graphics", "").DefineTags("canvas", "svg", "figure");
            tmp.GetOrAddCategory("AudioVideo", "").DefineTags("audio", "video");
            tmp.GetOrAddCategory("Informative", "").DefineTags("figcaption", "track");

            tmp = output.GetOrAddCategory("Links", "Tags declaring links");
            tmp.GetOrAddCategory("Navigation", "").DefineTags("a", "nav");
            tmp.GetOrAddCategory("Resources", "").DefineTags("link");

            tmp = output.GetOrAddCategory("Content", "Tags defining content structure");
            tmp.GetOrAddCategory("Semantic", "Content elements with clear semantic association").DefineTags("header", "footer", "details", "summary", "aside", "dialog", "main", "section", "article", "hr", "acronym", "abbr", "blockquote");
            tmp.GetOrAddCategory("DataPoint", "Elements defining exact data point").DefineTags("address", "cite", "code", "samp", "ruby", "time", "main", "var", "meter", "dfn", "i", "u");
            tmp.GetOrAddCategory("Emphasis", "Emphased content").DefineTags("mark", "em", "strong", "b");
            tmp.GetOrAddCategory("Unsemantic", "Content elements without semantic association").DefineTags("div", "span", "p", "br", "sup", "sub", "small");
            tmp.GetOrAddCategory("Headings", "").DefineTags("title", "h1", "h2", "h3", "h4", "h5", "h6");

            var tmp2 = tmp.GetOrAddCategory("Metadata", "Tags declaring lists of items");
            tmp2.GetOrAddCategory("Information", "").DefineTags("head", "meta", "data", "object", "param");
            tmp2.GetOrAddCategory("Presentation", "").DefineTags("style", "base", "basefont");


            tmp2 = tmp.GetOrAddCategory("Lists", "Tags declaring lists of items");
            tmp2.GetOrAddCategory("Structure", "Different list types").DefineTags("ol", "ul", "dl");
            tmp2.GetOrAddCategory("Items", "Items in the list").DefineTags("li", "dt");
            tmp2.GetOrAddCategory("Descriptive", "Description items in the list").DefineTags("dd");

            tmp2 = tmp.GetOrAddCategory("Tables", "Tags declaring tabular information");
            tmp2.GetOrAddCategory("Structure", "Structural elements").DefineTags("table", "tr", "td", "col", "colgroup", "tbody");
            tmp2.GetOrAddCategory("Heading", "Heading and caption wrappers").DefineTags("caption", "th", "thead");
            tmp2.GetOrAddCategory("Footer", "Footers").DefineTags("tfoot");

            output.Prepare();

            //tmp = tmp.GetOrAddCategory("Structure", "Tags declaring main structural organization of the content");

            return output;

        }

        protected void Register(String k, HtmlTagCounter a)
        {
            if (!counterDictionary.ContainsKey(k))
            {
                counterDictionary.Add(k, a);
            }
        }

        protected void Prepare()
        {
            List<imbSCI.Data.interfaces.IObjectWithPathAndChildren> all = this.getAllChildren();
            foreach (HtmlTagCounter a in all)
            {
                Register(a.tag, a);
                a.alias.ForEach(x => Register(x, a));
            }
        }

        public Dictionary<String, HtmlTagCounter> counterDictionary { get; set; } = new Dictionary<string, HtmlTagCounter>();

        public HtmlTagCategoryTree(String _name, String _description) : base(_name, _description, 1)
        {

        }

        public List<String> DistinctTags { get; set; } = new List<string>();
        public List<String> DistinctUnknownTags { get; set; } = new List<string>();


    }

}