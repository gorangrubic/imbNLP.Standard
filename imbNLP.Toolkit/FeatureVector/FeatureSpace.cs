using imbNLP.Toolkit.Core;

using imbNLP.Toolkit.Space;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Feature
{



    public class FeatureSpace
    {
        public FeatureSpace()
        {

        }

        public Relationships<FeatureVector, SpaceLabel> labelToDocumentAssociations { get; set; } = new Relationships<FeatureVector, SpaceLabel>();

        public List<FeatureVector> documents { get; set; } = new List<FeatureVector>();

        //  public List<SpaceTerm> SelectedFeatures { get; set; } = new List<SpaceTerm>();


    }

}