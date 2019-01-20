using imbSCI.Core.files;
using imbSCI.Core.files.folders;
using imbSCI.Core.math;
using imbSCI.Core.math.measurement;
using imbSCI.Core.reporting.render;
using imbSCI.Core.reporting.render.builders;
using imbSCI.DataComplex.tables;
using System;
using System.Collections.Generic;
using System.IO;

namespace imbNLP.Toolkit.Documents.DatasetStructure
{
    /// <summary>
    /// Basic structural information on dataset
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Documents.Analysis.MetricsBase" />
    public class DatasetStructureReport : MetricsBase
    {




        /// <summary>
        /// Dataset name
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public String name { get; set; } = "";


        public Int32 classes { get; set; } = 0;
        public Int32 sites { get; set; } = 0;
        public Int32 pages { get; set; } = 0;



        public void Compute()
        {
            sitesPerClass = sites.GetRatio(classes);
            pagesPerSite = pages.GetRatio(sites);
        }

        public Double sitesPerClass { get; set; } = 0;

        public Double pagesPerSite { get; set; } = 0;


        public void Describe(ITextRender output)
        {
            if (classes > 1)
            {
                output.AppendPair("Classes", classes);
                output.AppendPair("Sites per class", sitesPerClass.ToString("F3"));
            }

            output.AppendPair("Sites", sites);
            output.AppendPair("Pages", pages);

            output.AppendPair("Pages per site", pagesPerSite.ToString("F3"));

        }


        public void Publish(folderNode folder, Boolean exportXML = true, Boolean exportDescribe = true, Boolean exportDatatable = true)
        {
            if (exportXML)
            {
                String xml = objectSerialization.ObjectToXML(this);

                String x_path = folder.pathFor(name + "_report.xml", imbSCI.Data.enums.getWritableFileMode.overwrite, "Serialized dtructural report of a dataset");

                File.WriteAllText(x_path, xml);
            }


            if (exportDescribe)
            {

                builderForText builderForText = new builderForText();

                String t_path = folder.pathFor(name + "_report.txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "Summary of structural report of a dataset");

                Describe(builderForText);

                File.WriteAllText(t_path, builderForText.GetContent());
            }

            if (exportDatatable)
            {
                DataTableTypeExtended<DatasetStructureReport> dt_reports = new DataTableTypeExtended<DatasetStructureReport>(name, "Dataset structure stats");

                dt_reports.AddRow(this);
                foreach (var ch in Children)
                {
                    dt_reports.AddRow(ch);
                }

                dt_reports.GetReportAndSave(folder, null, name + "_report");
            }

        }



        public DatasetStructureReport()
        {

        }

        public List<DatasetStructureReport> Children { get; set; } = new List<DatasetStructureReport>();

        public static DatasetStructureReport MakeStructureReport(WebSiteDocumentsSet category)
        {

            DatasetStructureReport output = new DatasetStructureReport();
            output.name = category.name;

            output.classes = 1;

            output.sites = category.Count;

            output.pages = category.CountDocumentsTotal();

            output.Compute();

            return output;

        }


        public static DatasetStructureReport MakeStructureReport(IEnumerable<WebSiteDocumentsSet> dataset, String _name)
        {

            DatasetStructureReport output = new DatasetStructureReport();
            output.name = _name;

            foreach (var ds in dataset)
            {
                DatasetStructureReport ds_output = MakeStructureReport(ds);
                output.Plus(ds_output);
                output.Children.Add(ds_output);

            }

            output.Compute();

            return output;

        }

    }
}
