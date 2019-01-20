using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Vectors;
using imbNLP.Toolkit.Weighting;
using imbSCI.Core.reporting;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Entity
{
    public static class DocumentBlenderFunctionExtension
    {


        /*
        public static WeightDictionary GetChildrenWT(WeightDictionary output, SpaceDocumentModel model, FeatureWeightModel weightModel, SpaceModel space, List<string> FV)
        {
            if (output == null)
            {
                output = new WeightDictionary(model.name, "");
            }
            if (model.Children.Any())
            {
                foreach (var child in model.Children)
                {
                    GetChildrenWT(output, child, weightModel, space, FV);

                }

            }
            else
            {
                var wd = weightModel.GetWeights(FV, model, space);
                output.Merge(wd.index.Values, model.weight);
            }
            return output;

        }*/


        public static T BlendToVector<T>(this SpaceDocumentModel model, FeatureWeightModel weightModel, SpaceModel space, List<string> FV) where T : VectorDocument, new()
        {
            T output = new T();

            output.name = model.name;

            var leafs = model.GetLeafs();

            foreach (var leaf in leafs)
            {
                var wd = weightModel.GetWeights(FV, model, space);
                output.terms.Merge(wd);
            }

            //output.terms = 

            //output.Merge(wd.index.Values, model.weight);

            //WeightDictionary wd = new WeightDictionary(model.name, "");

            // GetChildrenWT(wd, model, weightModel, space, FV);




            return output;

        }

        public static List<SpaceDocumentModel> GetDocumentToBlend(DocumentBlenderFunctionOptions options, List<SpaceDocumentModel> spaceModelDocuments, ILogBuilder log)
        {
            List<SpaceDocumentModel> toBlendIntoVectors = new List<SpaceDocumentModel>();
            DocumentBlenderFunctionOptions blendScope = options.GetBlendingScope();



            switch (blendScope)
            {
                default:
                case DocumentBlenderFunctionOptions.siteLevel:
                    foreach (SpaceDocumentModel siteModel in spaceModelDocuments)
                    {
                        toBlendIntoVectors.Add(siteModel);
                    }
                    break;
                case DocumentBlenderFunctionOptions.pageLevel:
                    foreach (SpaceDocumentModel siteModel in spaceModelDocuments)
                    {
                        toBlendIntoVectors.AddRange(siteModel.Children);
                    }
                    break;
                case DocumentBlenderFunctionOptions.blockLevel:
                    foreach (SpaceDocumentModel siteModel in spaceModelDocuments)
                    {
                        foreach (SpaceDocumentModel m in siteModel.Children)
                        {
                            toBlendIntoVectors.AddRange(m.Children);
                        }

                    }
                    break;
            }

            if (log != null) log.log("Blend scope [" + blendScope.ToString() + "] - selected items [" + toBlendIntoVectors.Count() + "] ");


            return toBlendIntoVectors;
        }



        public static DocumentBlenderFunctionOptions GetBlendingScope(this DocumentBlenderFunctionOptions options)
        {

            if (options.HasFlag(DocumentBlenderFunctionOptions.siteLevel))
            {
                return DocumentBlenderFunctionOptions.siteLevel;
            }
            else if (options.HasFlag(DocumentBlenderFunctionOptions.pageLevel))
            {
                return DocumentBlenderFunctionOptions.pageLevel;

            }
            else if (options.HasFlag(DocumentBlenderFunctionOptions.blockLevel))
            {
                return DocumentBlenderFunctionOptions.blockLevel;
            }
            else if (options.HasFlag(DocumentBlenderFunctionOptions.sentenceLevel))
            {
                return DocumentBlenderFunctionOptions.sentenceLevel;
            }
            else
            {
                return DocumentBlenderFunctionOptions.siteLevel;
            }


        }

    }
}