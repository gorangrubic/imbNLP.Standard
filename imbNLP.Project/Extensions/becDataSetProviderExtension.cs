using imbACE.Core.operations;
using imbACE.Services.consolePlugins;
using imbNLP.Project.Dataset;
using imbNLP.Project.Extensions.Setups;
using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.ExperimentModel;
using imbNLP.Toolkit.ExperimentModel.Settings;
using imbSCI.Core.files.folders;
using imbSCI.Core.reporting;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace imbNLP.Project.Extensions
{



    public class becDataSetProviderExtension : instanceLoadSaveExtension<ExperimentDataSetSettings>
    {
        public becDataSetProviderExtension(folderNode __folder, IAceOperationSetExecutor __parent) : base(__folder, __parent)
        {
        }

        public WebDataSetImportContext GetImportContext(becDataSetSettings datasetSettings, ILogBuilder dsLoadLogger, Boolean silentDatasetLoad = true)
        {
            WebKBDatasetAdapter adapter = new WebKBDatasetAdapter();
            //ILogBuilder dsLoadLogger = parent.output;
            if (silentDatasetLoad) dsLoadLogger = null;

            WebDocumentsCategory category = adapter.LoadDataset(datasetSettings.path, WebDomainCategoryFormatOptions.normalizeDomainname, dsLoadLogger);

            List<WebSiteDocumentsSet> dataset = new List<WebSiteDocumentsSet>();
            if (datasetSettings.flattenCategoryHierarchy)
            {
                dataset = category.GetFirstLevelCategories();
            }
            else
            {
                throw new NotImplementedException();
            }

            //  exp.setup.toolkitSettings.entityMethod.cachePath = imbACE.Core.appManager.Application.folder_cache.Add("BEC", "BEC", "Cached objects for BEC").path;

            // vetting the dataset
            foreach (WebSiteDocumentsSet ds in dataset)
            {
                ds.RemoveEmptyDocuments(dsLoadLogger, datasetSettings.minPageLimit, datasetSettings.maxPageLimit);
                ds.AssignID(dsLoadLogger);
            }

            WebDataSetImportContext importContext = new WebDataSetImportContext(datasetSettings.path, dataset);

            return importContext;
        }

        [Display(GroupName = "set", Name = "Validation", ShortName = "", Description = "Configures k-fold cross validation or single-fold calidation")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "Updates active instance of the configuration")]
        /// <summary>Configures k-fold cross validation or single-fold calidation</summary>
        /// <remarks><para>Updates active instance of the configuration</para></remarks>
        /// <param name="K">Number of folds, if 1 (or 0) it will go into single-fold mode</param>
        /// <param name="TestFolds">Number of folds to be used as test folds, usually 1</param>
        /// <param name="Randomize">Shell content of the folds be randomized</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setValidation(
             [Description("Number of folds, if 1 (or 0) it will go into single-fold mode")] Int32 K = 1,
             [Description("Number of folds to be used as test folds, usually 1")] Int32 TestFolds = 1,
             [Description("Shell content of the folds be randomized")] Boolean Randomize = true,
             [Description("When above 0, only specified number of folds will be executed")] Int32 LimitExecution = -1
           )
        {
            data.validation.SingleFold = (K < 2);
            data.validation.K = K;
            data.validation.TrainingFolds = K - TestFolds;
            data.validation.randomFolds = Randomize;
            data.validation.LimitFoldsExecution = LimitExecution;
        }

        [Display(GroupName = "set", Name = "Dataset", ShortName = "", Description = "Configures dataset to be used for the experiment")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "Set path, filter and selection options")]
        /// <summary>Configures dataset to be used for the experiment</summary>
        /// <remarks><para>Set path, filter and selection options</para></remarks>
        /// <param name="path">Diskdrive path, pointing to the root folder of the dataset (WebKB format)</param>
        /// <param name="pageLimit">Minimum number of pages that a document set (website) must have in order to be accepted for the experiment</param>
        /// <param name="filterEmpty">Filters out empty documents from the dataset</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setDataset(
              [Description("Diskdrive path, pointing to the root folder of the dataset (WebKB format)")] String path = @"G:\imbWBI\datasets\7sectors",
              [Description("Minimum number of pages that a document set (website) must have in order to be accepted for the experiment")] Int32 pageLimit = 1,
              [Description("Highest number of pages to be loaded per web site")] Int32 pageLimitMax = -1,
              [Description("Filters out empty documents from the dataset")] Boolean filterEmpty = true)
        {
            data.dataset.path = path;
            data.dataset.minPageLimit = pageLimit;
            data.dataset.maxPageLimit = pageLimitMax;
            data.dataset.filterEmptyDocuments = filterEmpty;
        }

        public override void SetSubBinding()
        {

        }
    }
}