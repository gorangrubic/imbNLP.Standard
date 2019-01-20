using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.ExperimentModel.CrossValidation;
using imbNLP.Toolkit.ExperimentModel.Settings;
using imbSCI.Core.math;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.ExperimentModel
{

    /// <summary>
    /// Sliced category into k-folds
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{imbNLP.Toolkit.Documents.WebSiteDocumentsSet}" />
    public class CategorySlicedFolds : List<WebSiteDocumentsSet>
    {
        public CategorySlicedFolds()
        {

        }

        public List<WebSiteDocumentsSet> WeakClone()
        {

            List<WebSiteDocumentsSet> output = new List<WebSiteDocumentsSet>();



            foreach (WebSiteDocumentsSet set in this)
            {
                output.Add(set.WeakClone());
            }

            return output;
        }

        /// <summary>
        /// Slices category specified <c>input</c> into <c>K</c> folds
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="K">The k.</param>
        /// <param name="random">if set to <c>true</c> [random].</param>
        public void Deploy(WebSiteDocumentsSet input, Int32 K, Boolean random)
        {
            for (int i = 0; i < K; i++)
            {
                WebSiteDocumentsSet slice = new WebSiteDocumentsSet();
                slice.name = input.name;
                slice.description = "[" + i + "/" + K + "] " + input.description;
                Add(slice);
            }

            List<WebSiteDocuments> assigned = new List<WebSiteDocuments>();
            Random rnd = new Random();

            Int32 foldSize = input.Count / K;
            Double foldSizeD = 1.GetRatio(K);

            Int32 p = 0;

            for (int i = 0; i < input.Count; i++)
            {

                if (random)
                {
                    p = rnd.Next(K);
                }
                else
                {
                    p = i % K;
                    //  p = (Convert.ToInt32((i.GetRatio(input.Count)).GetRatio(foldSizeD))) - 1;
                }

                var item = input[i];

                this[p].Add(item);
            }
        }
    }

}