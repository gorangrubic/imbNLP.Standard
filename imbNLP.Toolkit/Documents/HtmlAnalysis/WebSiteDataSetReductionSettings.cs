using imbNLP.Toolkit.Documents.WebExtensions;
using imbNLP.Toolkit.ExperimentModel.Settings;
using imbSCI.Core.files;
using imbSCI.Core.reporting;
using System;
using System.IO;

namespace imbNLP.Toolkit.Documents.HtmlAnalysis
{

    /// <summary>
    /// Settings for dataset size reduction
    /// </summary>
    public class WebSiteDataSetReductionSettings : DataSetReductionSettingsBase
    {
        public WebSiteDataSetReductionSettings()
        {

        }


        public WebSiteGraphDiagnosticMark marksToRemove { get; set; } = WebSiteGraphDiagnosticMark.none;


        public becDataSetSettings LimitSettings { get; set; } = new becDataSetSettings();

        public HtmlDocumentReductionSettings HtmlDocumentReduction { get; set; } = new HtmlDocumentReductionSettings();

        public static WebSiteDataSetReductionSettings GetDefaultReductionSettings()
        {
            WebSiteDataSetReductionSettings output = new WebSiteDataSetReductionSettings();
            output.HtmlDocumentReduction = HtmlDocumentReductionSettings.GetDefaultReductionSettings();

            output.marksToRemove = WebSiteGraphDiagnosticMark.OnlyHomePageLoaded;

            output.LimitSettings.minPageLimit = 5;
            output.LimitSettings.maxPageLimit = 100;
            output.LimitSettings.filterEmptyDocuments = true;
            output.LimitSettings.flattenCategoryHierarchy = false;

            return output;
        }

        /// <summary>
        /// Loads settings from the path or returns default
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static WebSiteDataSetReductionSettings LoadOrDefault(String path, ILogBuilder logger)
        {
            if (path == "*")
            {
                return GetDefaultReductionSettings();
            }

            if (File.Exists(path))
            {
                return objectSerialization.loadObjectFromXML<WebSiteDataSetReductionSettings>(path, logger);
            }
            else
            {
                return GetDefaultReductionSettings();
            }

        }


    }
}