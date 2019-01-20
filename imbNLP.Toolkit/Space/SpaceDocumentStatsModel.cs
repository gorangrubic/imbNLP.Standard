using imbNLP.Toolkit.Documents.FeatureAnalytics.Data;
using imbNLP.Toolkit.Entity;
using imbNLP.Toolkit.Processing;
using imbSCI.Core.math;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Space
{
    public class SpaceDocumentStatsModel
    {

        public DocumentBlenderFunctionOptions documentScope
        {
            get { return _documentScope; }
            set
            {
                if (_documentScope != value)
                {

                }
                _documentScope = value;

            }
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public String name { get; set; } = "";

        public SpaceDocumentStatsModel(String __name, ILogBuilder log)
        {

            name = __name;
        }



        public void LearnFrom(SpaceDocumentModel learnFrom, ILogBuilder log, Boolean learnCompleteTreeStructure)
        {
            name = learnFrom.name;
            documentScope = learnFrom.documentScope;

            foreach (SpaceDocumentModel m in learnFrom.GetLeafs())
            {
                terms.MergeDictionary(m.terms);
                termsChildCount.CountTokens(m.terms.GetTokens());
            }

            foreach (SpaceDocumentModel m in learnFrom.Children)
            {
                SpaceDocumentStatsModel statChild = new SpaceDocumentStatsModel(m.name, log);

                if (learnCompleteTreeStructure)
                {
                    statChild.LearnFrom(m, log, learnCompleteTreeStructure);
                }

                Children.Add(statChild);
            }
        }


        //public void Add(SpaceDocumentModel m, ILogBuilder log)
        //{

        //}

        public List<SpaceDocumentStatsModel> Children { get; set; } = new List<SpaceDocumentStatsModel>();


        public Double GetChildWithTermCount(String term)
        {
            return termsChildCount.GetTokenFrequency(term);
        }



        private TokenDictionary _terms = new TokenDictionary();
        private DocumentBlenderFunctionOptions _documentScope = DocumentBlenderFunctionOptions.none;

        public TokenDictionary terms
        {
            get { return _terms; }
            set { _terms = value; }
        }

        public TokenDictionary termsChildCount { get; set; } = new TokenDictionary();


        /// <summary>
        /// Queries for term frequencies.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns></returns>
        public FeatureCWPFrequencies QueryForTermFrequencies(String term, FeatureCWPFrequencies output = null)
        {
            if (output == null) output = new FeatureCWPFrequencies()
            {
                term = term
            };


            List<SpaceDocumentStatsModel> toCollect = new List<SpaceDocumentStatsModel>();

            toCollect.Add(this);

            while (toCollect.Any())
            {

                List<SpaceDocumentStatsModel> nextIteration = new List<SpaceDocumentStatsModel>();

                foreach (SpaceDocumentStatsModel child in toCollect)
                {

                    output.Add(child.documentScope, child.terms.Contains(term));
                    nextIteration.AddRange(child.Children);

                }

                toCollect = nextIteration;

            }

            return output;

        }


        /// <summary>
        /// Queries term density score for term
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns></returns>
        public FeatureCWPAnalysisSiteMetrics QueryForTerm(String term)
        {
            FeatureCWPAnalysisSiteMetrics output = new FeatureCWPAnalysisSiteMetrics(term);


            Int32 C = Children.Count;


            foreach (SpaceDocumentStatsModel child in Children)
            {

                Double n_ck = child.GetChildWithTermCount(term);


                Double N_ck = child.Children.Count;


                Double cs_d = n_ck.GetRatio(N_ck);

                output.Add(child.name, cs_d);

            }





            output.Compute(C);

            return output;

        }


    }
}