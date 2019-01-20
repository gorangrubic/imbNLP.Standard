using imbNLP.Toolkit.Documents.FeatureAnalytics.Core;
using imbNLP.Toolkit.Documents.FeatureAnalytics.Data;
using imbNLP.Toolkit.Documents.Ranking;
using imbNLP.Toolkit.Documents.Ranking.Core;
using imbNLP.Toolkit.Documents.Ranking.Data;
using imbNLP.Toolkit.ExperimentModel;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbSCI.Core.files.folders;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Documents.FeatureAnalytics
{
    public class FeatureCWPAnalysis
    {
        public FeatureCWPAnalysisSettings settings { get; set; }

        public FeatureCWPAnalysis(FeatureCWPAnalysisSettings _settings)
        {
            settings = _settings;
        }

        //public Dictionary<string, Dictionary<string, Dictionary<string, Space.SpaceDocumentModel>>> nested_dict { get; set; }

        protected void SetMetrics(FeatureCWPAnalysisSiteMetrics metrics)
        {
            if (metrics.score == 0)
            {
                return;
            }

            if (metrics.commonality_score > settings.Commonality)
            {
                if (metrics.mean_score > settings.HighFrequency)
                {
                    metrics.featureClass = FeatureCWPTermClass.commonHighFrequency;
                }
                else if (metrics.mean_score < settings.LowFrequency)
                {
                    metrics.featureClass = FeatureCWPTermClass.particularForAspect;
                }
                else
                {
                    metrics.featureClass = FeatureCWPTermClass.normal;
                }
            }
            else
            {
            }

            if (metrics.particularity_score > settings.Particularity)
            {
                metrics.featureClass = FeatureCWPTermClass.particularForEntry;
            }
            else
            {
            }
        }

        /// <summary>
        /// Gets the metric.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public Double GetMetric(string term, CWPAnalysisScopeEnum scope, String property)
        {
            switch (scope)
            {
                case CWPAnalysisScopeEnum.categoryLevel:

                    return globalReport.GetMetricValue(term, property);

                    break;

                case CWPAnalysisScopeEnum.siteLevel:
                    return unitaryReport.GetMetricValue(term, property);

                    break;

                case CWPAnalysisScopeEnum.flatSiteLevel:
                    return flatReport.GetMetricValue(term, property);
                    break;
            }
            return 0;
        }

        public WeightDictionaryEntry GetMetric(string term, CWPAnalysisScopeEnum scope)
        {
            switch (scope)
            {
                case CWPAnalysisScopeEnum.categoryLevel:
                    if (globalReport.EntryDictionary.index.ContainsKey(term))
                    {
                        return globalReport.EntryDictionary.index[term];
                    }
                    break;

                case CWPAnalysisScopeEnum.siteLevel:
                    if (unitaryReport.EntryDictionary.index.ContainsKey(term))
                    {
                        return unitaryReport.EntryDictionary.index[term];
                    }
                    break;

                case CWPAnalysisScopeEnum.flatSiteLevel:
                    if (flatReport.EntryDictionary.index.ContainsKey(term))
                    {
                        return flatReport.EntryDictionary.index[term];
                    }
                    break;
            }
            return null;
        }

        public FeatureCWPAnalysisEntryReport unitaryReport { get; set; } = null;

        public FeatureCWPAnalysisEntryReport globalReport { get; set; } = null;

        public FeatureCWPAnalysisEntryReport flatReport { get; set; } = null;

        public FeatureCWPAnalysisDatasetReport datasetReport { get; set; } = null;

        /// <summary>
        /// Gets the score.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="computation">The computation.</param>
        /// <returns></returns>
        public Double GetScore(String term, CWPAnalysusScoreOutput computation)
        {
            Double output = 0;

            switch (computation)
            {
                case CWPAnalysusScoreOutput.categoryCommonality:
                    output = globalReport.GetScore(term, x => x.commonality_score);
                    break;

                case CWPAnalysusScoreOutput.categoryParticularity:
                    output = globalReport.GetScore(term, x => x.particularity_score);
                    break;

                case CWPAnalysusScoreOutput.siteParticularity:
                    output = unitaryReport.GetScore(term, x => x.particularity_score);
                    break;

                case CWPAnalysusScoreOutput.siteCommonality:
                    output = unitaryReport.GetScore(term, x => x.commonality_score);
                    break;

                case CWPAnalysusScoreOutput.flatSiteParticularity:
                    output = flatReport.GetScore(term, x => x.particularity_score);
                    break;

                case CWPAnalysusScoreOutput.frequencyDensity:
                    output = globalReport.GetScore(term, x => x.mean_score);
                    break;

                case CWPAnalysusScoreOutput.globalMinDensity:
                    output = globalReport.GetScore(term, x => x.min_score);
                    break;

                case CWPAnalysusScoreOutput.binaryCWParticularity:
                    output = datasetReport.GetScore(term, x => x.MacroParticularity);
                    if (output != 1)
                    {
                        if (datasetReport.GetScore(term, x => x.MaxParticularity) == 1)
                        {
                            output = 0;
                        }
                        else
                        {
                            output = 1;
                        }
                    }
                    break;
            }

            return output;
        }

        /// <summary>
        /// Analysises the specified notes.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        protected FeatureCWPAnalysisEntryReport SubAnalysis(SpaceDocumentStatsModel model, folderNode folder, ILogBuilder log)
        {
            FeatureCWPAnalysisEntryReport entryReport = new FeatureCWPAnalysisEntryReport(model.name, "Feature analysis for dataset branch [" + model.name + "]", folder);

            foreach (var term in model.terms.GetTokens())
            {
                FeatureCWPAnalysisSiteMetrics metrics = model.QueryForTerm(term);
                SetMetrics(metrics);
                entryReport.Append(metrics, false);
            }

            return entryReport;
        }

        /// <summary>
        /// Analysises the specified notes.
        /// </summary>
        /// <param name="notes">The notes.</param>
        public void Analysis(ToolkitExperimentNotes notes)
        {
            folderNode folder = notes.folder_corpus;
            Analysis(folder, notes);
        }

        /// <summary>
        /// Analysises the specified folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="log">The log.</param>
        public void Analysis(folderNode folder, ILogBuilder log)
        {
            if (settings.RequiredScopes.HasFlag(CWPAnalysisScopeEnum.globalLevel))
            {
                FeatureCWPAnalysisEntryReport entryReport = null;

                entryReport = new FeatureCWPAnalysisEntryReport("Global", "Feature analysis for complete dataset ", folder?.Add("_global", "Global", "GlobalReport"));

                if (log != null) log.log("Making global dataset report");
                foreach (var term in datasetStatsModel.terms.GetTokens())
                {
                    FeatureCWPAnalysisSiteMetrics metrics = datasetStatsModel.QueryForTerm(term); // QueryTermGlobalLevel(term);
                    SetMetrics(metrics);
                    entryReport.Append(metrics, false);
                }

                entryReport.Save(log);

                globalReport = entryReport;
            }

            if (settings.RequiredScopes.HasFlag(CWPAnalysisScopeEnum.categoryLevel))
            {
                if (folder != null)
                {
                    foreach (SpaceDocumentStatsModel category in datasetStatsModel.Children)
                    {
                        folder.Add(category.name, category.name, "");
                    }
                }

                if (log != null) log.log("Making category level dataset reports");

                foreach (var category in datasetStatsModel.Children)
                {
                    FeatureCWPAnalysisEntryReport rp = null;

                    if (folder != null)
                    {
                        rp = SubAnalysis(category, folder[category.name], log);
                    }
                    else
                    {
                        rp = SubAnalysis(category, null, log);
                    }
                    rp.Save(log, false);

                    categoryReports.Add(rp);
                }


            }

            if (log != null) log.log("Making unitary and flat level reports");

            if (settings.RequiredScopes.HasFlag(CWPAnalysisScopeEnum.mainLevel))
            {
                datasetReport = new FeatureCWPAnalysisDatasetReport("Dataset", "Final report on the dataset", folder?.Add("_main", "main", ""), categoryReports);
                datasetReport.Save(log);
            }

            if (settings.RequiredScopes.HasFlag(CWPAnalysisScopeEnum.unitaryLevel))
            {
                unitaryReport = new FeatureCWPAnalysisEntryReport("Fusioned report", "Cross category report with MAX(particularity) and MAX(commonality)", folder?.Add("_unitary", "Unitary", ""));

                foreach (var pair in categoryReports)
                {
                    foreach (System.Collections.Generic.KeyValuePair<string, FeatureCWPAnalysisSiteMetrics> e in pair)
                    {
                        SetMetrics(e.Value);
                        unitaryReport.AddMerge(e.Value, false);
                    }
                }

                unitaryReport.PostMerge();
                unitaryReport.Save(log, false);
            }

            if (settings.RequiredScopes.HasFlag(CWPAnalysisScopeEnum.rawLevel))
            {
                frequencies.Deploy(datasetStatsModel);
                frequencies.PublishTableBlocks(folder.Add("_freq", "Frequencies", "Absolute frequencies by scope"));
            }

            if (settings.RequiredScopes.HasFlag(CWPAnalysisScopeEnum.flatSiteLevel))
            {
                flatReport = new FeatureCWPAnalysisEntryReport("Flat report", "Report produced as if all sites are in single cateogry", folder?.Add("_flat", "Flat", ""));
                flatReport = SubAnalysis(flatDataSetStatsModel, flatReport.folder, log);
                flatReport.Save(log, false);
            }
        }

        public FeatureCWPFrequencyDictionary frequencies = new FeatureCWPFrequencyDictionary();

        public List<FeatureCWPAnalysisEntryReport> categoryReports = new List<FeatureCWPAnalysisEntryReport>();

        public SpaceDocumentStatsModel datasetStatsModel { get; set; }

        public SpaceDocumentModel datasetModel { get; set; } = new SpaceDocumentModel();

        public SpaceDocumentStatsModel flatDataSetStatsModel { get; set; }

        public void Prepare(SpaceModel spaceModel, ILogBuilder log, bool excludeUnknown = true)
        {
            var labels = spaceModel.LabelToDocumentLinks.GetAllDistinctNames();

            if (excludeUnknown) labels.Remove(SpaceLabel.UNKNOWN);

            datasetStatsModel = new SpaceDocumentStatsModel("Stats", log);
            datasetStatsModel.documentScope = Entity.DocumentBlenderFunctionOptions.datasetLevel;
            flatDataSetStatsModel = new SpaceDocumentStatsModel("FlatStats", log);
            flatDataSetStatsModel.documentScope = Entity.DocumentBlenderFunctionOptions.datasetLevel;

            foreach (string label in labels)
            {
                var documents = spaceModel.documents.Where(x => x.labels.Contains(label));

                //var documents = spaceModel.LabelToDocumentLinks.GetAllLinkedB(label);
                SpaceDocumentModel labelDocModel = new SpaceDocumentModel(label);
                labelDocModel.documentScope = Entity.DocumentBlenderFunctionOptions.categoryLevel;
                labelDocModel.Children.AddRange(documents);
                labelDocModel.Flatten(false);
                //  var categoryModel = new SpaceDocumentStatsModel(labelDocModel, log);

                SpaceDocumentStatsModel categoryModel = new SpaceDocumentStatsModel(labelDocModel.name, log);
                categoryModel.LearnFrom(labelDocModel, log, true);

                datasetStatsModel.Children.Add(categoryModel);

                datasetStatsModel.terms.MergeDictionary(labelDocModel.terms);
                datasetStatsModel.termsChildCount.CountTokens(labelDocModel.terms.GetTokens());

                if (log != null) log.log("Statistics for category [" + label + "]");
            }

            if (settings.RequiredScopes.HasFlag(CWPAnalysisScopeEnum.flatSiteLevel))
            {
                if (log != null) log.log("Creating flat report");

                foreach (var document in spaceModel.documents)
                {
                    if (!document.labels.Contains(SpaceLabel.UNKNOWN))
                    {
                        flatDataSetStatsModel.LearnFrom(document, log, true);
                    }
                }
            }
        }

        /// <summary>
        /// Prepares the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        public void Prepare(DocumentSelectResult context, ILogBuilder log, bool excludeUnknown = true)
        {

            var nested_dict = context.GetModelsByCategoryDomainAssignedID(log);

            if (nested_dict.ContainsKey(SpaceLabel.UNKNOWN))
            {
                nested_dict.Remove(SpaceLabel.UNKNOWN);
            }

            datasetModel = nested_dict.NestCompleteSpaceDocumentModel(context.name, log);

            datasetStatsModel = new SpaceDocumentStatsModel(datasetModel.name, log);
            datasetStatsModel.LearnFrom(datasetModel, log, true);
        }

        public ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null)
        {
            if (requirements == null) requirements = new ScoreModelRequirements();

            return requirements;
        }
    }
}