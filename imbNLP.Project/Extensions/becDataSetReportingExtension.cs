using imbACE.Core.operations;
using imbACE.Services.consolePlugins;
using imbNLP.Project.Dataset;
using imbNLP.Project.Extensions.Setups;
using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Documents.Analysis;
using imbNLP.Toolkit.ExperimentModel;
using imbNLP.Toolkit.ExperimentModel.Settings;
using imbSCI.Core.files.folders;
using imbSCI.Core.reporting;
using imbSCI.DataComplex.tables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace imbNLP.Project.Extensions
{
    /// <summary>
    /// Extension providing dataset reporting capability
    /// </summary>
    /// <seealso cref="imbACE.Services.consolePlugins.instanceLoadSaveExtension{imbNLP.Project.Extensions.Setups.SetupDataSetReporting}" />
    public class becDataSetReportingExtension : instanceLoadSaveExtension<SetupDataSetReporting>
    {
        public override void SetSubBinding()
        {
            dataSet.SetBinding(data, nameof(data.dataSetSource), true);
            render.SetBinding(data, nameof(data.render), true);
        }

        public becDataSetProviderExtension dataSet { get; set; }

        public becDocumentRenderingExtension render { get; set; }



        public becDataSetReportingExtension(folderNode __folder, IAceOperationSetExecutor __parent) : base(__folder, __parent)
        {
            dataSet = new becDataSetProviderExtension(__folder, __parent);

            render = new becDocumentRenderingExtension(__parent, __folder);

            SetSubBinding();
        }




        [Display(GroupName = "run", Name = "StructureReport", ShortName = "", Description = "Provides statistics on dataset class structure")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "What it will do?")]
        /// <summary>Provides statistics on dataset class structure</summary> 
        /// <remarks><para>What it will do?</para></remarks>
        /// <param name="word">--</param>
        /// <param name="steps">--</param>
        /// <param name="debug">--</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_runStructureReport(
              [Description("--")] String runName = "word",
              [Description("--")] Int32 steps = 5,
              [Description("--")] Boolean debug = true)
        {

            /*
            if (mainContext == null)
            {
                mainContext = new ExperimentModelExecutionContext(runName);
                String runComment = "Report on dataset imported from path [" + datasetPath + "]";
                mainContext.Deploy(exp.setup.reportOptions, imbACE.Core.appManager.AppInfo, exp.setup.averagingMethod, experimentGroup.folder.Add(runName, runName, runComment), runComment, parent.output);
            }

            WebKBDatasetAdapter adapter = new WebKBDatasetAdapter();
            ILogBuilder dsLoadLogger = parent.output;

            becExperimentSetup setup = exp.setup;

            String p = datasetPath.or(setup.datasetSettings.path);

            WebDocumentsCategory category = adapter.LoadDataset(p, WebDomainCategoryFormatOptions.normalizeDomainname, dsLoadLogger);
            List<WebSiteDocumentsSet> dataset = category.GetFirstLevelCategories();

            foreach (var ds in dataset)
            {
                ds.RemoveEmptyDocuments(parent.output, exp.setup.datasetSettings.minPageLimit, exp.setup.datasetSettings.maxPageLimit);
            }

            ContentAnalytics analytics = new ContentAnalytics(imbACE.Core.appManager.Application.folder_reports.Add(runName, runName, "Dataset [" + setup.datasetSettings.path + "] report"));
            analytics.entityMethodSettings = setup.toolkitSettings.entityMethod;
            analytics.corpusMethodSettings = setup.toolkitSettings.corpusMethod;
            analytics.vectorMethodSettings = setup.toolkitSettings.vectorMethod;

            var report = analytics.ProduceMetrics(runName, dataset, parent.output);
            report.GetDataTable(runName).GetReportAndSave(analytics.folder, imbACE.Core.appManager.AppInfo);

            report.ReportTokens(analytics.folder.Add("tokens", "Tokens", "Tokens"), runName, 1000);

            report.ReportSample(analytics.folder.Add("sample", "sample", "Sample list"), runName, 1000);
            report.ReportHTMLTags(analytics.folder.Add("html", "HTML", "Structural statistics for HTML tags"), runName);
            analytics.folder.generateReadmeFiles(imbACE.Core.appManager.AppInfo);
            */
        }

    }
}