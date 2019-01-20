using imbNLP.Project.Operations.Data;
using imbNLP.Toolkit;
using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Documents.Ranking.Core;

using imbNLP.Toolkit.Planes;
using imbNLP.Toolkit.Reporting;
using imbSCI.Core.extensions.io;
using imbSCI.Core.files;
using imbSCI.Core.math;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace imbNLP.Project.Operations
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Planes.MethodDesignBase" />
    public class OperationContextReport : MethodDesignBase
    {
        public OperationContextReport()
        {

        }



        /// <summary>
        /// The path limit
        /// </summary>
        public const Int32 pathLimit = 50;

        /// <summary>
        /// Gets the filename.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public string GetFilename(string path)
        {
            String output = path.getFilename();

            if (output.Length > pathLimit)
            {
                String sufix = output.Substring(pathLimit);
                String prefix = output.Substring(0, pathLimit);

                sufix = md5.GetMd5Hash(sufix);
                output = prefix + sufix;
            }
            return output;
        }


        /// <summary>
        /// Generates the reports.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        public void GenerateReports(OperationContext context, OperationReportEnum reportOptions, ILogBuilder log)
        {

            log.log("Generating reports [" + reportOptions.ToString() + "]");

            var renderTextTables = reportOptions.HasFlag(OperationReportEnum.tableExportText);
            var exportExcel = reportOptions.HasFlag(OperationReportEnum.tableExportExcel);



            if (reportOptions.HasFlag(OperationReportEnum.reportRenderingLayers))
            {
                foreach (System.Collections.Generic.KeyValuePair<string, TextDocumentLayerCollection> pair in context.renderLayersByAssignedID)
                {
                    string p = notes.folder_entity.pathFor(GetFilename(pair.Key) + "_layers.xml", imbSCI.Data.enums.getWritableFileMode.overwrite, "Rendered layers", true);
                    objectSerialization.saveObjectToXML(pair.Value, p);
                }
            }

            if (reportOptions.HasFlag(OperationReportEnum.reportPreblendFilter))
            {

            }

            if (reportOptions.HasFlag(OperationReportEnum.reportDataset))
            {
                foreach (KeyValuePair<string, WebSiteDocumentsSet> ds in context.dataset)
                {
                    var subfold = notes.folder_corpus.Add(ds.Key, ds.Key, "List of documents included in corpus, for category [" + ds.Key + "]");

                    var rndfold = subfold.Add("Urls", "Urls", "Urls of documents processed");

                    foreach (WebSiteDocuments pair in ds.Value)
                    {

                        string p = rndfold.pathFor(GetFilename(pair.domain) + ".txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "List of documents included in text render", true);
                        StringBuilder sb = new StringBuilder();
                        foreach (var d in pair.documents)
                        {
                            sb.AppendLine(d.AssignedID);
                        }

                        sb.ToString().saveStringToFile(p, imbSCI.Data.enums.getWritableFileMode.overwrite);
                    }

                }
            }

            if (reportOptions.HasFlag(OperationReportEnum.reportFeatures))
            {
                //if (generalContext.reportOptions.HasFlag(PlanesReportOptions.report_selectedFeatures))
                //{



                if (context.SelectedFeatures != null)
                {

                    if (context.SelectedFeatures.Count > 0)
                    {

                        DataTable dt = context.SelectedFeatures.MakeTable("selected_features", "Features selected for BoW construction", new List<string>() { "Selection Function score" }, 500);


                        if (exportExcel) notes.SaveDataTable(dt, notes.folder_feature);
                        if (renderTextTables) notes.SaveDataTableToText(dt, notes.folder_feature);

                    }

                }



                //}
            }

            if (reportOptions.HasFlag(OperationReportEnum.reportBlendedRenders))
            {

                foreach (KeyValuePair<string, WebSiteDocumentsSet> ds in context.dataset)
                {

                    var subfold = notes.folder_entity.Add(ds.Key, ds.Key, "Text renders [" + ds.Key + "]");

                    var rndfold = subfold.Add("Renders", "Renders", "Serialized textual representation of documents");

                    foreach (WebSiteDocuments pair in ds.Value)
                    {
                        string p = "";

                        //if (context.textDocuments.ContainsKey(pair.domain))
                        //{
                        //    p = rndfold.pathFor(GetFilename(pair.domain) + "_text.xml", imbSCI.Data.enums.getWritableFileMode.overwrite, "Blended text rendering", true);
                        //    objectSerialization.saveObjectToXML(context.textDocuments[pair.domain], p);

                        //}
                        //else
                        //{
                        //    foreach (var d in pair.documents)
                        //    {


                        //        if (context.textDocuments.ContainsKey(d.AssignedID))
                        //        {
                        //            p = rndfold.pathFor(GetFilename(d.AssignedID) + "_text.xml", imbSCI.Data.enums.getWritableFileMode.overwrite, "Blended text rendering", true);
                        //            objectSerialization.saveObjectToXML(context.textDocuments[d.AssignedID], p);

                        //        }

                        //    }

                        //}

                    }

                }

                //    foreach (System.Collections.Generic.KeyValuePair<string, TextDocument> pair in context.textDocuments)
                //{
                //    string p = notes.folder_entity.pathFor(GetFilename(pair.Key) + "_text.xml", imbSCI.Data.enums.getWritableFileMode.overwrite, "Blended text rendering", true);
                //    objectSerialization.saveObjectToXML(pair.Value, p);
                //}
            }

            if (reportOptions.HasFlag(OperationReportEnum.reportSpaceModel))
            {

            }

            if (reportOptions.HasFlag(OperationReportEnum.reportClassification))
            {

            }


            CloseDeploySettingsBase();
        }

        public override ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null)
        {
            if (requirements == null) requirements = new ScoreModelRequirements();




            //foreach (IScoreModelFactor factor in Factors)
            //{
            //    factor.CheckRequirements(requirements);

            //}

            return requirements;
        }

    }
}
