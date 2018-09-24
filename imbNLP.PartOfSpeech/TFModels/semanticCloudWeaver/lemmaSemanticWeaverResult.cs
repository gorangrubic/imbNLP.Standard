using imbNLP.PartOfSpeech.analysis;
using imbNLP.PartOfSpeech.TFModels.semanticCloud;
using imbSCI.Core.extensions.data;
using imbSCI.Core.files.folders;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace imbNLP.PartOfSpeech.TFModels.semanticCloudWeaver
{
    /// <summary>
    /// Reporting purpose result of the weaver's contribution to the cloud improvement
    /// </summary>
    public class lemmaSemanticWeaverResult
    {
        public lemmaSemanticWeaverResult()
        {
        }

        public lemmaSemanticWeaverResult(lemmaSemanticCloud _cloud)
        {
            cloud = _cloud;
            cloudClassName = _cloud.className;
            linkRatioInitial = cloud.GetLinkPerNodeRatio();
        }

        /// <summary>
        /// Link per node ratio, before <see cref="lemmaSemanticWeaver"/> processed the cloud
        /// </summary>
        /// <value>
        /// The link ratio initial.
        /// </value>
        public Double linkRatioInitial { get; set; }

        /// <summary>
        /// Link per node ratio after <see cref="lemmaSemanticWeaver.similarWords"/> is applied
        /// </summary>
        /// <value>
        /// The link ratio after ws.
        /// </value>
        public Double linkRatioAfterWS { get; set; }

        /// <summary>
        /// Link per node ratio after <see cref="lemmaSemanticWeaver.apertiumSettings"/> is applied
        /// </summary>
        /// <value>
        /// The link ratio after ds.
        /// </value>
        public Double linkRatioAfterDS { get; set; }

        /// <summary>
        /// Name of the cloud category / class name
        /// </summary>
        /// <value>
        /// The name of the cloud class.
        /// </value>
        public String cloudClassName { get; set; } = "disabled";

        /// <summary>
        /// Reference to the cloud being processed
        /// </summary>
        /// <value>
        /// The cloud.
        /// </value>
        public lemmaSemanticCloud cloud { get; set; }

        /// <summary>
        /// Results on word similarity
        /// </summary>
        /// <value>
        /// The similar words.
        /// </value>
        public wordSimilarityResultSet similarWords { get; set; }

        /// <summary>
        /// Gets or sets the appertium notes.
        /// </summary>
        /// <value>
        /// The appertium notes.
        /// </value>
        public List<String> appertiumNotes { get; set; } = new List<string>();

        /// <summary>
        /// Saves the report into text file in the <c>folder</c>, and returns path of the file
        /// </summary>
        /// <param name="folder">The folder to save into</param>
        /// <param name="filename_sufix">Optional filename suffix to be added after <see cref="cloudClassName"/> </param>
        /// <returns>Path of the saved file</returns>
        public String Save(folderNode folder, String filename_sufix = "")
        {
            String fn = "weaver_report_" + cloudClassName;
            fn = fn.add(filename_sufix, "_");
            fn = fn + ".txt";
            String path = folder.pathFor(fn, imbSCI.Data.enums.getWritableFileMode.overwrite, "Report on [" + cloudClassName + "] processing, done by lemmaSemanticWeaver");

            File.WriteAllText(path, ToString());
            return path;
        }

        /// <summary>
        /// Returns textual description on what was matched
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (similarWords == null)
            {
                sb.AppendLine("# Semantic Cloud Weaver not used - for [" + cloudClassName + "]");
                return sb.ToString();
            }
            String output = similarWords.ToString();

            sb.AppendLine("# Semantic Cloud Weaver report for [" + cloudClassName + "]");

            sb.AppendLine(" > [" + cloud.CountNodes() + "] cloud nodes");
            sb.AppendLine(" > [" + linkRatioInitial.ToString("F5") + "] initial link-per-node ratio");
            sb.AppendLine(" > [" + linkRatioAfterWS.ToString("F5") + "] link-per-node ratio after WordSimilarity used");
            sb.AppendLine(" > [" + linkRatioAfterDS.ToString("F5") + "] link-per-node ratio after DictionarySynonims used");
            sb.AppendLine(" -------------------------------------------------------- ");

            sb.AppendLine(similarWords.ToString());

            sb.AppendLine("## Apertium synonim links");

            Int32 c = 1;
            foreach (var ln in appertiumNotes)
            {
                sb.AppendLine(c.ToString("D5") + " : " + ln);
                c++;
            }

            return sb.ToString();
        }
    }
}