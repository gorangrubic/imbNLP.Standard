using imbACE.Services.console;
using imbSCI.Core.math.classificationMetrics;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace imbNLP.Project.Plugin
{



    public class nlpExperimentMacroScript
    {


        public nlpExperimentMacroScript()
        {
            definitions.Add("OR", "    .GlobalWeight function=\"CollectionTDPElement\";weight=1.0;flags=\"or,max\";", "Odds Ratio function");
            definitions.Add("IG", "    .GlobalWeight function=\"CollectionTDPElement\";weight=1.0;flags=\"ig,max\";", "Information gain function");
            definitions.Add("IDFp", "    .GlobalWeight function=\"CollectionTDPElement\";weight=1.0;flags=\"idf_prob,max\";", "Probabilistic IDF");
            definitions.Add("CHI", "    .GlobalWeight function=\"CollectionTDPElement\";weight=1.0;flags=\"chi,max\";", "Chi-square factor");
            definitions.Add("GR", "    .GlobalWeight function=\"CollectionTDPElement\";weight=1.0;flags=\"gr,max\";", "Information gain ratio");
            definitions.Add("RF", "    .GlobalWeight function=\"CollectionTDPElement\";weight=1.0;flags=\"rf,max\";", "Relevance frequency");

            definitions.Add("IDF", "    .GlobalWeight function=\"IDFElement\";weight=1.0;flags=\"logPlus\";", "Inverse document frequency (IDF)");
            definitions.Add("mIDF", "    .GlobalWeight function=\"IDFElement\";weight=1.0;flags=\"modified\";", "modified inverse document frequency");
            definitions.Add("IGM", "    .GlobalWeight function=\"IGMElement\";weight=1.0;flags=\"logPlus\";", "Inverse gravity moment");
            definitions.Add("ICF", "    .GlobalWeight function=\"ICFElement\";weight=1.0;flags=\"logPlus\";", "Inverse class frequency");
            definitions.Add("ICSdF", "    .GlobalWeight function=\"ICSdFElement\";weight=1.0;flags=\"logPlus\";", "Inverse Class space density frequency");
            definitions.Add("FIP", "    .GlobalWeight function=\"CWPElement\";weight=1.0;flags=\"flatSiteParticularity, inverse\"", "CWP - flat inverse particularity");
            definitions.Add("IP", "    .GlobalWeight function=\"CWPElement\";weight=1.0;flags=\"siteParticularity, inverse\"", "CWP - inverse particularity");
            definitions.Add("BCP", "    .GlobalWeight function=\"CWPElement\";weight=1.0;flags=\"binaryCWParticularity\"", "CWP - inverse particularity");
            definitions.Add("IFD", "    .GlobalWeight function=\"CWPElement\";weight=1.0;flags=\"frequencyDensity, inverse\";", "Inter class mean of density score");
            definitions.Add("IGMD", "    .GlobalWeight function=\"CWPElement\";weight=1.0;flags=\"globalMinDensity, inverse\";", "Inverse global minimum density");

            definitions.Add("G", "    .LocalWeight computation=\"glasgow\";normalization=\"divisionByMaxTF\";", "Glasgow local component");
            definitions.Add("mTF", "    .LocalWeight computation=\"modifiedTF\";normalization=\"divisionByMaxTF\";", "Modified Term Frequency");
            definitions.Add("TF", "    .LocalWeight computation=\"normal\";normalization=\"divisionByMaxTF\";", "normalized Term Frequency");
            definitions.Add("RTF", "    .LocalWeight computation=\"squareRooted\";normalization=\"divisionByMaxTF\";", "Square rooted normalized Term Frequency");



        }

        public reportExpandedData definitions { get; set; } = new reportExpandedData();

        public Regex SELECTOR { get; set; } = new Regex("(.*)--(.*)");
        public Regex SELECTOR_NUMBER { get; set; } = new Regex(@"([\d]*)$");


        String description = "";

        String filter_code = "";
        String size = "";
        String weight_code = "";


        List<String> filter_tags = new List<string>();
        List<String> weight_tags = new List<string>();

        public Boolean RemoveZero = true;

        Int32 fs_size = 0;

        public aceConsoleScript GenerateScript([Description("tags")] String tags = "")
        {

            if (tags.Contains("!"))
            {
                tags = tags.Replace("!", "");
                RemoveZero = false;
            }

            List<string> left_tags = new List<string>();
            List<string> right_tags = new List<String>();

            if (tags.Contains("--"))
            {
                Match mach = SELECTOR.Match(tags);
                left_tags = mach.Groups[1].Value.SplitSmart("-");
                right_tags = mach.Groups[2].Value.SplitSmart("-");
            }
            else
            {
                right_tags = tags.SplitSmart("-");
            }


            String fs_tag = "";
            String fs_tagn = "";
            foreach (var fst in right_tags)
            {
                fs_tagn = SELECTOR_NUMBER.Match(fst).Value; //.Groups[0].Value;
                if (fs_tagn.Length > 0)
                {
                    fs_size = Int32.Parse(fs_tagn);
                    fs_tag = fst;
                }
            }

            if (!fs_tag.isNullOrEmpty())
            {
                right_tags.Remove(fs_tag);
                right_tags.Add(fs_tag.Replace(fs_tagn, ""));
            }

            var left_match = definitions.Where(x => left_tags.Contains(x.key));
            var right_match = definitions.Where(x => right_tags.Contains(x.key));

            List<String> left_lines = new List<string>();
            //left_lines.Add("    .New");

            List<String> right_lines = new List<string>();
            //right_lines.Add("   .New");



            left_lines.AddRange(left_match.Select(x => x.value));
            right_lines.AddRange(right_match.Select(x => x.value));

            tags = tags.Replace("--", "-");


            filter_code = "bec.ops.docClassification.filter.model{" + Environment.NewLine + String.Join(Environment.NewLine, left_lines) + Environment.NewLine + "}";
            weight_code = "bec.ops.docClassification.weight.model{" + Environment.NewLine + String.Join(Environment.NewLine, right_lines) + Environment.NewLine + "}";


            description += "Feature selection based on " + String.Join(",", left_match.Select(x => x.description));

            description += "Term weighting based on " + String.Join(",", right_match.Select(x => x.description));

            StringBuilder sb = new StringBuilder();

            if (!RemoveZero)
            {
                tags = "n" + tags;
                description += " Feature selection without zero-filter.";
            }

            sb.AppendLine($"bec.InitExperimentContext runName=\"{tags}\";runComment=\"{description}\";silendDatasetLoad=true;");
            sb.AppendLine($"bec.ops.docClassification.filter.FeatureFilter RemoveZero={RemoveZero};limit={fs_size};nVectorOperation=\"max\";outputFilename=\"{tags}_selected\";");
            sb.AppendLine("bec.ops.docClassification.ResetModels;");

            sb.AppendLine(filter_code);
            sb.AppendLine(weight_code);

            sb.AppendLine($"bec.ops.ClassificationWithDS outputName=\" \";DSFilename=\" \";DSCount=500;options=\" \";");
            sb.AppendLine($"bec.ops.Execute options=\"clearContextOnStart, clearContextOnFinish, skipExistingExperiment\";");
            sb.AppendLine($"bec.CloseExperimentContext;");

            aceConsoleScript script = sb.ToString();

            // input.setContent(sb.ToString());


            return script;
        }

    }
}
