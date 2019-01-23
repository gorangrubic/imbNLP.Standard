using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbSCI.Core.enums;
using imbSCI.Core.files.folders;
using imbSCI.Core.reporting;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Weighting
{




    /// <summary>
    /// Feature filter
    /// </summary>
    public class FeatureFilter : NLPBindable, IDescribe
    {
        private Boolean _isEnabled = true;
        //   private IGlobalElement _function;
        private Int32 _limit = -1;
        private FeatureWeightModel _weightModel = new FeatureWeightModel();

        //  private GlobalFunctionSettings _functionSettings = new GlobalFunctionSettings();


        public WeightDictionary precompiledSelection { get; set; }

        public FeatureWeightModel WeightModel
        {
            get { return _weightModel; }
            set
            {
                _weightModel = value;
                OnPropertyChange(nameof(WeightModel));

            }
        }


        public FeatureFilter()
        {

        }

        public void Describe(ILogBuilder logger)
        {
            if (IsEnabled)
            {
                logger.AppendLine("Feature Selection");
                logger.AppendPair("Limit", limit, true, "\t\t\t");

                logger.AppendLine("Ranking method for n-dimensional Feature Weights: " + nVectorValueSelectionOperation.ToString());


                logger.AppendPair("Function", WeightModel.GetSignature(), true, "\t\t\t");

                WeightModel.Describe(logger);




            }
            else
            {
                logger.AppendLine("Feature Selection method is disabled");
            }
        }

        public void Deploy(ILogBuilder logger, folderNode folder = null)
        {

            if (!outputFilename.isNullOrEmpty())
            {
                if (folder != null)
                {
                    String p_m = folder.pathFor(outputFilename, imbSCI.Data.enums.getWritableFileMode.none, "", false);
                    precompiledSelection = WeightDictionary.LoadFile(p_m, logger);
                }
            }

            if (WeightModel != null)
            {
                WeightModel.DoUseLocalFunction = false;

                WeightModel.Deploy(logger);
            }

            //function = functionSettings.GetFunction(logger);

            //_isEnabled = function.IsEnabled;

        }

        [XmlIgnore]
        public Boolean IsEnabled
        {
            get
            {
                if (WeightModel == null) return false;
                if (!WeightModel.GlobalFactors.Any()) return false;
                if (limit <= 0) return false;
                return true;

            }

        }


        //public Boolean ComputeFeatureScores(WeightDictionary featureScores, SpaceModel space, ILogBuilder log, folderNode folder = null)
        //{



        //    return doAll;

        //}


        /// <summary>
        /// Selects the top <see cref="limit"/> terms, ranked by <see cref="function"/>
        /// </summary>
        /// <param name="space">The space.</param>
        /// <returns></returns>
        public List<KeyValuePair<string, double>> SelectFeatures(SpaceModel space, ILogBuilder log, folderNode folder = null, WeightDictionary featureScores = null)
        {
            Dictionary<String, Double> rank = new Dictionary<string, double>();
            Boolean doAll = false;
            if (limit == -1) doAll = true;

            if (featureScores == null) featureScores = new WeightDictionary();

            var tokens = space.terms_known_label.GetTokens();

            if (precompiledSelection != null && precompiledSelection.Count > 0)
            {
                log.log("Using precompiled selection filter from [" + outputFilename + "]");
                featureScores.Merge(precompiledSelection);
            }
            else
            {
                WeightModel.PrepareTheModel(space, log);

                featureScores = WeightModel.GetElementFactors(tokens, space);
            }


            if (tokens.Count() <= limit) doAll = true;

            if (doAll)
            {
                List<KeyValuePair<string, double>> outAll = new List<KeyValuePair<string, double>>();

                foreach (String tkn in tokens)
                {
                    outAll.Add(new KeyValuePair<string, double>(tkn, 1));
                }
                return outAll;
            }

            //function.PrepareTheModel(space, log);




            if (!outputFilename.isNullOrEmpty())
            {
                if (folder != null)
                {
                    String p_m = folder.pathFor(outputFilename, imbSCI.Data.enums.getWritableFileMode.none, "", false);
                    featureScores.Save(folder, log, outputFilename);
                    //precompiledSelection = WeightDictionary.LoadFile(p_m, logger);
                }
            }


            foreach (WeightDictionaryEntry en in featureScores.index.Values)
            {
                //   rank.Add(en.name, en.weight);
                Double v = 0;

                if (featureScores.nDimensions > 1)
                {
                    v = en.CompressNumericVector(nVectorValueSelectionOperation);
                }
                else
                {
                    v = en.weight;
                }


                Boolean ok = true;

                if (RemoveZero)
                {
                    if (v == 0) ok = false;
                }


                if (ok) rank.Add(en.name, v);
            }

            var rankSorted = rank.OrderByDescending(x => x.Value).ToList();
            List<KeyValuePair<string, double>> top = rankSorted.Take(Math.Min(limit, rankSorted.Count)).ToList();

            return top;
        }





        public String GetSignature()
        {
            String output = WeightModel.GetSignature(); //functionSettings.GetSignature();
            if (limit > 0) output += limit.ToString();
            return output;
        }

        //[XmlIgnore]
        //public IGlobalElement function
        //{
        //    get { return _function; }
        //    set { _function = value; }
        //}

        public Int32 limit
        {
            get { return _limit; }
            set
            {
                OnPropertyChange(nameof(limit));
                _limit = value;
            }
        }


        public Boolean RemoveZero { get; set; } = true;

        public operation nVectorValueSelectionOperation { get; set; } = operation.max;

        public String outputFilename { get; set; } = "";


    }


}