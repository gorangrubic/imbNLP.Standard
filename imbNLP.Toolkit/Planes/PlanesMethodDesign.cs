using imbNLP.Toolkit.Documents.Ranking.Core;
using imbNLP.Toolkit.ExperimentModel;
using imbNLP.Toolkit.Planes.Core;
using imbSCI.Core.files;
using imbSCI.Core.files.folders;
using imbSCI.Core.math.classificationMetrics;
using imbSCI.Core.reporting;
using imbSCI.DataComplex.tables;

namespace imbNLP.Toolkit.Planes
{


    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Planes.MethodDesignBase" />
    /// <seealso cref="imbNLP.Toolkit.Planes.Core.IPlaneMethodDesign" />
    public class PlanesMethodDesign : MethodDesignBase, IPlaneMethodDesign
    {

        #region methods

        public EntityPlaneMethodDesign EntityMethod { get; set; } = new EntityPlaneMethodDesign();

        public CorpusPlaneMethodDesign CorpusMethod { get; set; } = new CorpusPlaneMethodDesign();

        public VectorPlaneMethodDesign VectorMethod { get; set; } = new VectorPlaneMethodDesign();

        public FeaturePlaneMethodDesign FeatureMethod { get; set; } = new FeaturePlaneMethodDesign();

        #endregion


        public void DeploySettings(IPlaneSettings settings, ToolkitExperimentNotes _notes, ILogBuilder logger)
        {
            DeploySettingsBase(_notes);

            PlanesMethodSettings mainSettings = (PlanesMethodSettings)settings;

            EntityMethod.DeploySettings(mainSettings.entityMethod, notes, logger);
            CorpusMethod.DeploySettings(mainSettings.corpusMethod, notes, logger);
            VectorMethod.DeploySettings(mainSettings.vectorMethod, notes, logger);
            FeatureMethod.DeploySettings(mainSettings.featureMethod, notes, logger);

            CacheProvider.Deploy(new System.IO.DirectoryInfo(mainSettings.cachePath));

            EntityMethod.CacheProvider = CacheProvider;
            CorpusMethod.CacheProvider = CacheProvider;
            VectorMethod.CacheProvider = CacheProvider;
            FeatureMethod.CacheProvider = CacheProvider;

            CloseDeploySettingsBase();
        }

        /// <summary>
        /// Creates the context.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="folder">Root folder for experiment</param>
        /// <returns></returns>
        public PlanesMethodContext CreateContext(string name, folderNode folder)
        {
            var generalContext = new PlanesMethodContext();
            generalContext.name = name;
            generalContext.folder = folder.Add(name, name, "Home folder of the experiment");

            return generalContext;
        }

        public IPlaneContext ExecutePlaneMethod(IPlaneContext inputContext, ExperimentModelExecutionContext generalContext, ILogBuilder logger)
        {
            //if (generalContext == null)
            //{
            //    generalContext = new PlanesMethodContext();
            //}
            IEntityPlaneContext entityInputContext = inputContext as IEntityPlaneContext;


            ICorpusPlaneContext entityContext = EntityMethod.ExecutePlaneMethod(inputContext, generalContext, logger) as ICorpusPlaneContext;

            IVectorPlaneContext corpusContext = CorpusMethod.ExecutePlaneMethod(entityContext, generalContext, logger) as IVectorPlaneContext;

            IFeaturePlaneContext vectorContext = VectorMethod.ExecutePlaneMethod(corpusContext, generalContext, logger) as IFeaturePlaneContext;

            IFeaturePlaneContext featureContext = FeatureMethod.ExecutePlaneMethod(vectorContext, generalContext, logger) as IFeaturePlaneContext;

            // --- the results reporting

            var evaluationMetrics = generalContext.truthTable.EvaluateTestResultsToMetricSet(featureContext.testResults, generalContext.runName + "-" + notes.folder.name, logger);

            DataTableTypeExtended<classificationEval> inclassEvalTable = new DataTableTypeExtended<classificationEval>("inclass_evaluation", "Test results, per class");
            evaluationMetrics.GetAllEntries().ForEach(x => inclassEvalTable.AddRow(x));
            inclassEvalTable.AddRow(evaluationMetrics.GetSummary("Sum"));
            notes.SaveDataTable(inclassEvalTable, notes.folder_classification);

            classificationReport averagedReport = new classificationReport(evaluationMetrics, generalContext.averagingMethod);
            averagedReport.Classifier = FeatureMethod.classifier.name;
            averagedReport.saveObjectToXML(notes.folder_classification.pathFor(averagedReport.Name + ".xml", imbSCI.Data.enums.getWritableFileMode.overwrite, "Serialized classification evaluation results summary"));

            generalContext.testSummaries.Add(averagedReport);

            averagedReport.ReportToLog(notes);

            featureContext.provider.Dispose();
            EntityMethod.CacheProvider.Dispose();

            return generalContext;

        }

        public override ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null)
        {
            throw new System.NotImplementedException();
        }
    }
}
