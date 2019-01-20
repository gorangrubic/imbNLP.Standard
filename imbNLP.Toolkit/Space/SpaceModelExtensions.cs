using imbNLP.Toolkit.Processing;
using imbSCI.Core.extensions.data;
using imbSCI.Core.math;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Space
{
    public static class SpaceModelExtensions
    {
        public static List<T> CloneTerm<T>(this IEnumerable<T> models) where T : SpaceTerm, new()
        {
            List<T> output = new List<T>();

            foreach (T model in models)
            {
                output.Add(model.CloneTerm<T>());
            }
            return output;
        }


        public static List<T> Clone<T>(this IEnumerable<T> models, Boolean cloneChildren) where T : SpaceDocumentModel, new()
        {
            List<T> output = new List<T>();

            foreach (T model in models)
            {
                output.Add(model.Clone<T>(cloneChildren));
            }
            return output;
        }

        /// <summary>
        /// Gets the documents by label.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="includeUnlabeled">if set to <c>true</c> [include unlabeled].</param>
        /// <returns></returns>
        public static Dictionary<String, List<SpaceDocumentModel>> GetDocumentsByLabel(this SpaceModel model, Boolean includeUnlabeled = false)
        {
            var labels = model.labels.Select(x => x.name).ToList();

            if (!includeUnlabeled) labels.Remove(SpaceLabel.UNKNOWN);


            Dictionary<String, List<SpaceDocumentModel>> output = new Dictionary<string, List<SpaceDocumentModel>>();

            foreach (String l in labels)
            {
                output.Add(l, model.documents.Where(x => x.labels.Contains(l)).ToList());
            }


            return output;
        }

        public static List<SpaceDocumentModel> GetDocumentsOfLabel(this SpaceModel model, String labelName)
        {
            List<SpaceDocumentModel> output = model.documents.Where(x => x.labels.Contains(labelName)).ToList();
            return output;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="labeled">if set to <c>true</c> [labeled].</param>
        /// <param name="unlabeled">if set to <c>true</c> [unlabeled].</param>
        /// <returns></returns>
        public static List<String> GetTokens(this SpaceModel model, Boolean labeled, Boolean unlabeled)
        {
            List<String> tokens = new List<string>();

            if (labeled)
            {
                tokens.AddRange(model.terms_known_label.GetTokens(), true);
            }

            if (unlabeled)
            {
                tokens.AddRange(model.terms_unknown_label.GetTokens(), true);
            }



            return tokens;
        }



        /// <summary>
        /// Sets the label.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="label">The label.</param>
        /// <param name="space">The space.</param>
        public static void SetLabel(this SpaceDocumentModel model, SpaceLabel label, SpaceModel space)
        {
            space.LabelToDocumentLinks.Add(label, model, 1.0);

            var ms = model.Children.ToList();
            while (ms.Any())
            {
                List<SpaceDocumentModel> nestMs = new List<SpaceDocumentModel>();
                foreach (var m in ms)
                {
                    nestMs.AddRange(m.Children);
                    space.LabelToDocumentLinks.Add(label, m, 1.0);

                }
                ms = nestMs;
            }


        }

        /// <summary>
        /// Filters the space model features.
        /// </summary>
        /// <param name="spaceModel">The space model.</param>
        /// <param name="selectedFeatures">The selected features.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public static Int32 FilterSpaceModelFeatures(this SpaceModel spaceModel, WeightDictionary selectedFeatures, ILogBuilder log)
        {

            Int32 i = 0;
            Int32 s = spaceModel.documents.Count() / 5;

            Int32 c_filter_out = 0;
            List<String> keys = selectedFeatures.GetKeys();

            foreach (SpaceDocumentModel model in spaceModel.documents)
            {

                c_filter_out += model.FilterSelectedFeatures(keys, true);


                if (i % s == 0)
                {
                    Double r = i.GetRatio(spaceModel.documents.Count());
                    log.log("Filter SelectedFeatures [" + r.ToString("P2") + "]");
                }
                i++;

            }


            spaceModel.terms_known_label.FilterTokens(keys, true);
            spaceModel.terms_unknown_label.FilterTokens(keys, true);

            return c_filter_out;


        }

    }
}