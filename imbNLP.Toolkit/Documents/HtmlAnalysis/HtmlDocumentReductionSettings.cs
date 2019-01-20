using imbSCI.Core.files;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.IO;

namespace imbNLP.Toolkit.Documents.HtmlAnalysis
{
    public abstract class DataSetReductionSettingsBase
    {
        public Boolean logSiteLevel { get; set; } = true;

        public Boolean logCategoryLevel { get; set; } = true;
    }

    public class HtmlDocumentReductionSettings : DataSetReductionSettingsBase
    {
        public HtmlDocumentReductionSettings()
        {

        }

        /// <summary>
        /// Loads settings from the path or returns default
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static HtmlDocumentReductionSettings LoadOrDefault(String path, ILogBuilder logger)
        {
            if (path == "*")
            {
                return GetDefaultReductionSettings();
            }

            if (File.Exists(path))
            {
                return objectSerialization.loadObjectFromXML<HtmlDocumentReductionSettings>(path, logger);
            }
            else
            {
                return GetDefaultReductionSettings();
            }

        }

        public static HtmlDocumentReductionSettings GetDefaultReductionSettings()
        {
            HtmlDocumentReductionSettings output = new HtmlDocumentReductionSettings();
            output.tagsToRemove.Add("link");
            output.tagsToRemove.Add("style");
            output.tagsToRemove.Add("script");
            output.tagsToRemove.Add("comment");

            output.attributesToRemove.Add("id");
            output.attributesToRemove.Add("class");
            output.attributesToRemove.Add("style");
            return output;
        }

        public List<String> tagsToRemove { get; set; } = new List<string>();

        public List<String> attributesToRemove { get; set; } = new List<string>();





    }
}