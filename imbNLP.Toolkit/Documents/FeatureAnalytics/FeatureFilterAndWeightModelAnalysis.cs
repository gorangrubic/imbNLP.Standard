using imbNLP.Toolkit.Documents.FeatureAnalytics.Core;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Weighting;
using imbSCI.Core.extensions.data;
using imbSCI.Core.files.folders;
using imbSCI.Core.math.range.histogram;
using imbSCI.Core.reporting;
using imbSCI.Data.collection.nested;
using imbSCI.DataComplex.tables;
using imbSCI.Graph.Graphics.HeatMap;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace imbNLP.Toolkit.Documents.FeatureAnalytics
{

    /// <summary>
    /// Comparative analysis of filter and weight models
    /// </summary>
    public class FeatureFilterAndWeightModelAnalysis
    {

        public FeatureFilterAndWeightModelAnalysis(SpaceModel spaceModel, List<FeatureWeightModel> wm, List<FeatureFilter> fm)
        {
            space = spaceModel.Clone();
            WeightModels.AddRange(wm);
            FilterModels.AddRange(fm);
        }

        public SpaceModel space { get; set; }

        public List<FeatureWeightModel> WeightModels { get; set; } = new List<FeatureWeightModel>();

        public List<FeatureFilter> FilterModels { get; set; } = new List<FeatureFilter>();

        public List<Int32> FSRange { get; set; } = new List<int>() { 2000, 4000, 6000, 8000 };

        public aceDictionary2D<Int32, String, FeatureSelectionAnalysis> FSTests { get; set; } = new aceDictionary2D<Int32, String, FeatureSelectionAnalysis>();

        public CWPAnalysisReportsEnum tasks { get; set; } = CWPAnalysisReportsEnum.all;

        public void GenerateOverlapMatrixes(String prefix, ConcurrentDictionary<string, FeatureSelectionAnalysis> concurrentDictionary, ILogBuilder log, folderNode folder)
        {

            List<FeatureSelectionAnalysis> flts = concurrentDictionary.Values.ToList();

            String name_weighted = prefix + "_weighted_overlap";

            List<WeightDictionary> wfs = flts.Select(x => x.weightedFeatures).ToList();

            var model = PublishMatrix(log, folder, name_weighted, wfs);

            String name_selected = prefix + "_selected_overlap";

            List<WeightDictionary> sfs = flts.Select(x => x.selectedFeatures).ToList();

            model = PublishMatrix(log, folder, name_selected, sfs);

        }

        private static imbSCI.Core.math.range.matrix.HeatMapModel PublishMatrix(ILogBuilder log, folderNode folder, string name_selected, List<WeightDictionary> sfs)
        {
            imbSCI.Core.math.range.matrix.HeatMapModel model = sfs.GetHeatMapMatrix();
            model.DetectMinMax();

            model.GetDataTable(name_selected, "Overlaping terms and their frequencies").GetReportAndSave(folder, null, name_selected);

            try
            {

                HeatMapRender heatMapRender = new HeatMapRender();
                heatMapRender.style.accronimLength = 3;
                heatMapRender.style.BaseColor = Color.Black;
                heatMapRender.style.fieldHeight = 50;
                heatMapRender.style.fieldWidth = 50;

                heatMapRender.RenderAndSave(model, folder.pathFor(name_selected, imbSCI.Data.enums.getWritableFileMode.overwrite, "Heat map showing overlaping terms and their frequencies"));
            }
            catch (Exception ex)
            {
                log.log(ex.Message);
            }

            return model;
        }

        public void GenerateOverlapMatrixes(ILogBuilder log, folderNode folder)
        {
            List<histogramModel> histograms = new List<histogramModel>();


            foreach (Int32 atSize in FSTests.Get1stKeys())
            {
                ConcurrentDictionary<string, FeatureSelectionAnalysis> concurrentDictionary = FSTests[atSize];



                String prefix = concurrentDictionary.Keys.toCsvInLine() + "_" + atSize;
                GenerateOverlapMatrixes(prefix, concurrentDictionary, log, folder);


                List<histogramModel> models = new List<histogramModel>();

                foreach (var selcol in concurrentDictionary.Values)
                {
                    //histogramModel model = new histogramModel(50, "SelectedDistributionAt" + atSize);
                    var freq = selcol.weightedFeatures.index.Values.OrderByDescending(x => x.weight);

                    histogramModel model = histogramModelExtensions.GetHistogramModel(freq, "Weights", x => x.weight, 20);

                    models.Add(model);


                }

                models.BlendHistogramModels(prefix).GetReportAndSave(folder, null, "histogram" + prefix);

            }
        }

        public void ExecuteAnalysis(OperationContext context, ILogBuilder log, folderNode folder)
        {


            foreach (FeatureFilter flt in FilterModels)
            {
                foreach (Int32 FSl in FSRange)
                {

                    var ana = new FeatureSelectionAnalysis(flt.GetSignature(), space, flt, FSl);
                    log.log("Performing analysis of [" + ana.name + "]");
                    FSTests[FSl, ana.name] = ana;
                    ana.DeployAndRun(log, space, folder);
                }

            }


            GenerateOverlapMatrixes(log, folder);

        }

    }
}