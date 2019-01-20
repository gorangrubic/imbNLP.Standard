using imbNLP.Toolkit.Documents.WebExtensions;
using imbNLP.Toolkit.Feature;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbSCI.Core.extensions.data;
using imbSCI.Core.extensions.text;
using imbSCI.Core.files;
using imbSCI.Core.files.folders;
using imbSCI.Core.math;
using imbSCI.Core.reporting;
using imbSCI.Core.reporting.render;
using imbSCI.Core.reporting.render.builders;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Documents.Ranking.Data
{
    /// <summary>
    /// Context of particular document query call
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{imbNLP.Toolkit.Documents.Ranking.Data.DocumentSelectResultEntry}" />
    public class DocumentSelectResult
    {
        [XmlArray(ElementName = "items")]
        [XmlArrayItem(ElementName = "entry")]
        public List<DocumentSelectResultEntry> items { get; set; } = new List<DocumentSelectResultEntry>();

        public String name { get; set; } = "";

        public String description { get; set; } = "";

        public Boolean Remove(DocumentSelectResultEntry entry) => items.Remove(entry);
        public void RemoveRange(Int32 index, Int32 size) => items.RemoveRange(index, size);

        [XmlIgnore]
        public Int32 Count => items.Count;

        //public void CreateTextReport(ITextRender render)
        //{

        //}

        public static String CheckAndMakeFilename(String filename)
        {
            String precompFile = filename;
            precompFile = precompFile.removeEndsWith(".xml");


            precompFile = precompFile.ensureEndsWith("_ranking");
            precompFile = precompFile.ensureStartsWith("DS_");

            precompFile = precompFile.ensureEndsWith(".xml");
            return precompFile;
        }

        public void SaveReport(String filepath)
        {
            builderForMarkdown textRender = new builderForMarkdown();

            Report(textRender);

            File.WriteAllText(filepath, textRender.GetContent());
        }


        /// <summary>
        /// Loads from file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static DocumentSelectResult LoadFromFile(String path, ILogBuilder logger)
        {
            DocumentSelectResult scores = null;

            if (path.Trim().isNullOrEmpty())
            {
                logger.log(" _ NO PATH SPECIFIED _ for DocumentSelectResult.LoadFromFile()");
                return scores;
            }

            if (!File.Exists(path))
            {
                logger.log(" _ FILE NOT FOUND _ for DocumentSelectResult.LoadFromFile(path) ");
                return scores;
            }


            scores = objectSerialization.loadObjectFromXML<DocumentSelectResult>(path, logger);


            String filename = Path.GetFileNameWithoutExtension(path);

            scores.name = scores.name.or(filename);
            scores.description = scores.description.or("Precompiled scores loaded from [" + path + "]");

            return scores;

        }



        //  [XmlIgnore]
        //  public Dictionary<IScoreModelFactor, rangeFinder> FactorScoreRanges { get; set; } = new Dictionary<IScoreModelFactor, rangeFinder>();

        //public String SaveToXML() 

        /// <summary>
        /// Directory of the execution fold
        /// </summary>
        /// <value>
        /// The folder.
        /// </value>
        [XmlIgnore]
        public folderNode folder { get; set; }


        public ITextRender Report(ITextRender output = null)
        {
            if (output == null) output = new builderForMarkdown();

            var scores = items.Select(x => x.score);

            output.AppendHeading("Granularity");

            var distinct = items.GetDistinctScores();
            Int32 dC = distinct.Count();

            output.AppendPair("Distinct", dC);
            output.AppendPair("Entries", scores.Count());
            Double r = (Double)dC.GetRatio(scores.Count());

            output.AppendPair("Distinct / Entries", r);

            output.AppendHeading("Cumulative histogram");


            for (int i = 1; i < 11; i++)
            {
                Double l_min = (i - 1).GetRatio(10);
                Double l_max = i.GetRatio(10);
                var bin = scores.Where(x => (x > l_min) && (x < l_max));
                Double per = bin.Count().GetRatio(scores.Count());
                output.AppendPair("Bin [" + i + "][" + l_max.ToString("F2") + "]", per.ToString("P2"));
            }

            output.AppendHeading("Descriptive statistics");

            DescriptiveStatistics desc = scores.GetStatistics(true);
            desc.Describe(output);



            output.AppendHeading("Document selection result");

            foreach (DocumentSelectResultEntry result in items)
            {
                output.AppendLine(result.score.ToString("F5") + "\t\t" + result.AssignedID);
            }

            output.AppendHorizontalLine();

            query.Describe(output);

            output.AppendHorizontalLine();

            return output;

        }


        /// <summary>
        /// Gets or sets the stemming context.
        /// </summary>
        /// <value>
        /// The stemming context.
        /// </value>
        [XmlIgnore]
        public StemmingContext stemmingContext { get; set; }


        [XmlIgnore]
        public WeightDictionary selectedFeatures { get; set; } = new WeightDictionary();



        /// <summary>
        /// Gets or sets the space model.
        /// </summary>
        /// <value>
        /// The space model.
        /// </value>
        [XmlIgnore]
        public SpaceModel spaceModel { get; set; }

        [XmlIgnore]
        public FeatureSpace featureSpace { get; set; }

        ///// <summary>
        ///// Gets or sets the dataset.
        ///// </summary>
        ///// <value>
        ///// The dataset.
        ///// </value>
        //[XmlIgnore]
        //public List<WebSiteDocumentsSet> dataset { get; set; } = new List<WebSiteDocumentsSet>();

        /// <summary>
        /// Graph 
        /// </summary>
        [XmlIgnore]
        public Dictionary<String, WebSiteGraph> domainNameToGraph = new Dictionary<string, WebSiteGraph>();


        //    public List<TextDocument> corpus_documents { get; set; } = new List<TextDocument>();



        /// <summary>
        /// Reference to the query
        /// </summary>
        /// <value>
        /// The query.
        /// </value>
        public DocumentSelectQuery query { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentSelectResult"/> class.
        /// </summary>
        public DocumentSelectResult()
        {

        }
    }
}