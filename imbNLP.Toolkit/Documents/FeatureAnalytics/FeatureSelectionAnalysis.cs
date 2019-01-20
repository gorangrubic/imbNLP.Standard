using imbNLP.Toolkit.Documents.Ranking;
using imbNLP.Toolkit.Documents.Ranking.Core;
using imbNLP.Toolkit.Documents.Ranking.Data;
using imbNLP.Toolkit.ExperimentModel;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Weighting;
using imbSCI.Core.files;
using imbSCI.Core.files.folders;
using imbSCI.Core.reporting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace imbNLP.Toolkit.Documents.FeatureAnalytics
{
public class FeatureSelectionAnalysis
    {

        public FeatureSelectionAnalysis(String _name, SpaceModel _space, FeatureFilter _filter, Int32 _FSl)
        {
            name = _name;
            //space = _space.Clone();
            inputTerms = _space.terms.Clone();
            filter = _filter.CloneViaXML();
            filter.limit = _FSl;
            

        }

        public String name { get; set; } = "";
        public Int32 FSl { get; set; } = 0;

        public void DeployAndRun(ILogBuilder log, SpaceModel _space, folderNode folder)
        {
            filter.Deploy(log, folder);

            weightedFeatures = new WeightDictionary(name+"_weg" + filter.limit.ToString(), "weighted features, before filter");
            selectedFeatures = new WeightDictionary(name + "_sel" + filter.limit.ToString(), "selected weighted featyres");

            var selected = filter.SelectFeatures(_space, log, folder, weightedFeatures);

            foreach (var pair in selected)
            {
                selectedFeatures.AddEntry(pair.Key, pair.Value);
            }

            weightedFeatures.Save(folder, log, WeightDictionary.GetDictionaryFilename(weightedFeatures.name, folder));
            selectedFeatures.Save(folder, log, WeightDictionary.GetDictionaryFilename(selectedFeatures.name, folder));

        }

        public TokenDictionary inputTerms { get; set; }

        public WeightDictionary weightedFeatures { get; set; }

        public WeightDictionary selectedFeatures { get; set; }

        public FeatureFilter filter { get; set; }


        public FeatureSelectionAnalysis()
        {

        }
    }
}