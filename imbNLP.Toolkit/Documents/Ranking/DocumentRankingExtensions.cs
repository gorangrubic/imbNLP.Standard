using imbNLP.Toolkit.Documents.Ranking.Core;
using imbNLP.Toolkit.Documents.Ranking.Data;
using imbNLP.Toolkit.Feature;
using imbNLP.Toolkit.Functions;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Weighting;
using imbSCI.Core.extensions.data;
using imbSCI.Core.files.folders;
using imbSCI.Core.math;
using imbSCI.Core.math.range.finder;
using imbSCI.Core.reporting;
using imbSCI.Core.collection;
using imbSCI.Core.data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using imbSCI.Data.collection;

namespace imbNLP.Toolkit.Documents.Ranking
{
    public static class DocumentRankingExtensions
    {



        /// <summary>
        /// Evaluates the ds ranking.
        /// </summary>
        /// <param name="ds_loaded">The ds loaded.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="filepath">The filepath.</param>
        /// <param name="minDiversity">The minimum diversity.</param>
        /// <returns></returns>
        public static Boolean EvaluateDSRanking(DocumentSelectResult ds_loaded, ILogBuilder logger, String filepath = "", Double minDiversity = 0.01)
        {
            if (filepath == "") filepath = ds_loaded.name;

            var distinct = ds_loaded.items.GetDistinctScores();
            Int32 c = distinct.Count();

            Boolean skip = true;



            if (distinct.Contains(Double.NaN))
            {
                logger.log("Ranking scores [" + filepath + "] is refused as it contains NaN entries");
                return false;
            }

            if (c < 2)
            {
                logger.log("Ranking scores [" + filepath + "] is refused as it contains [" + c + "] distinct values");
                return false;
            }

            Double rate = c.GetRatio(ds_loaded.items.Count());

            if (rate < minDiversity)
            {
                logger.log("Ranking scores [" + filepath + "] is refused for having [" + rate.ToString("F5") + "] below criterion [" + minDiversity.ToString("F2") + "]");
                return false;
            }


            logger.log("Ranking scores [" + filepath + "] accepted d=[" + rate.ToString("F5") + "] c=[" + distinct.Count + "] |e|=[" + ds_loaded.items.Count + "]");
            return true;

        }


        /// <summary>
        /// Evaluates the saved ds ranking.
        /// </summary>
        /// <param name="filepath">The filepath.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="minDiversity">The minimum diversity.</param>
        /// <returns></returns>
        public static Boolean EvaluateSavedDSRanking(String filepath, ILogBuilder logger, Double minDiversity = 0.01)
        {

            DocumentSelectResult ds_loaded = null;

            filepath = filepath.Trim();

            if (filepath.isNullOrEmpty())
            {
                logger.log("EvaluateSavedDSRanking -- no filepath specified");
                return false;
            }

            if (!File.Exists(filepath))
            {
                logger.log("Ranking scores not found at [" + filepath + "]");
                return false;
            }

            ds_loaded = DocumentSelectResult.LoadFromFile(filepath, logger);

            return EvaluateDSRanking(ds_loaded, logger, filepath, minDiversity);

        }




        /// <summary>
        /// Gets the distinct scores.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static List<Double> GetDistinctScores(this IEnumerable<DocumentSelectResultEntry> result)
        {
            List<Double> distinctScores = new List<double>();

            foreach (var entry in result)
            {
                if (!distinctScores.Contains(entry.score))
                {
                    distinctScores.Add(entry.score);
                }
            }

            return distinctScores;


        }

        public static List<DocumentSelectResult> LoadDSRankings(IEnumerable<String> filepaths, ILogBuilder output)
        {
            List<DocumentSelectResult> results = new List<DocumentSelectResult>();


            foreach (var fp in filepaths)
            {
                var lr = DocumentSelectResult.LoadFromFile(fp, output);

                results.Add(lr);

            }

            return results;
        }


        /// <summary>
        /// Loads multiple DocumentSelect results
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="inputNames">The input names.</param>
        /// <param name="output">The output.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns></returns>
        public static List<DocumentSelectResult> LoadDSRankings(folderNode folder, String inputNames, ILogBuilder output, String searchPattern = "DS_*_ranking.xml")
        {
            List<string> filepaths = folder.GetOrFindFiles(inputNames, searchPattern, SearchOption.TopDirectoryOnly);

            List<DocumentSelectResult> results = new List<DocumentSelectResult>();


            foreach (var fp in filepaths)
            {
                var lr = DocumentSelectResult.LoadFromFile(fp, output);

                results.Add(lr);

            }

            return results;
        }

        /// <summary>
        /// Merges the ds rankings - searches folder for specified input names or search pattern
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="inputNames">The input names.</param>
        /// <param name="output">The output.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns></returns>
        public static FeatureVectorDictionaryWithDimensions MergeDSRankings(folderNode folder, String inputNames, ILogBuilder output, String searchPattern = "DS_*_ranking.xml")
        {

            List<string> filepaths = folder.GetOrFindFiles(inputNames, searchPattern);

            DocumentSelectResult resultOut = new DocumentSelectResult();

            List<DocumentSelectResult> results = new List<DocumentSelectResult>();
            List<String> existingNames = new List<string>();

            String tmpOutputName = "";

            foreach (var fp in filepaths)
            {
                var lr = DocumentSelectResult.LoadFromFile(fp, output);
                String fn = Path.GetFileNameWithoutExtension(fp);
                if (existingNames.Contains(lr.name))
                {
                    lr.name = fn;
                }
                existingNames.Add(lr.name);

                results.Add(lr);
                tmpOutputName += lr.name;
            }


            FeatureVectorDictionaryWithDimensions featureDict = DocumentRankingExtensions.TransformToFVDictionary(results);

            return featureDict;


        }


        //public static DocumentSelectResult ToDocumentSelectResult(this FeatureVectorDictionaryWithDimensions input)
        //{

        //}




        /// <summary>
        /// Transforms multiple document selection results into FeatureVector dictionary
        /// </summary>
        /// <param name="documentSelections">The document selections.</param>
        /// <returns></returns>
        public static FeatureVectorDictionaryWithDimensions TransformToFVDictionary(this IEnumerable<DocumentSelectResult> documentSelections)
        {

            FeatureVectorDictionaryWithDimensions output = new FeatureVectorDictionaryWithDimensions();


            Dictionary<String, Dictionary<String, DocumentSelectResultEntry>> entryIDvsResultSets = new Dictionary<string, Dictionary<string, DocumentSelectResultEntry>>();

            List<String> existingNames = new List<string>();

            Int32 i = 0;
            foreach (DocumentSelectResult res in documentSelections)
            {

                if (existingNames.Contains(res.name) || res.name.isNullOrEmpty())
                {
                    res.name = i.ToString("D3");
                }
                existingNames.Add(res.name);

                output.dimensions.Add(res.name, res.description, Feature.Settings.FeatureVectorDimensionType.precompiledDocumentScore, res.name);

                foreach (DocumentSelectResultEntry entry in res.items)
                {

                    if (!entryIDvsResultSets.ContainsKey(entry.AssignedID))
                    {
                        entryIDvsResultSets.Add(entry.AssignedID, new Dictionary<string, DocumentSelectResultEntry>());

                    }

                    if (!entryIDvsResultSets[entry.AssignedID].ContainsKey(res.name))
                    {
                        entryIDvsResultSets[entry.AssignedID].Add(res.name, entry);
                    }

                }
                i++;
            }

            foreach (KeyValuePair<string, Dictionary<string, DocumentSelectResultEntry>> pair in entryIDvsResultSets)
            {
                var fv = output.GetOrAdd(pair.Key);

                foreach (KeyValuePair<string, DocumentSelectResultEntry> entries in pair.Value)
                {
                    Int32 id = output.dimensions.IndexOf(entries.Key);

                    if (id > -1)
                    {
                        fv.dimensions[id] = entries.Value.score;
                    }

                }

            }

            return output;
        }



        /// <summary>
        /// Transforms to fv dictionary.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="TermWeightModel">The term weight model.</param>
        /// <param name="function">The function.</param>
        /// <returns></returns>
        public static FeatureVectorSetDictionary TransformToFVDictionaryAsPageSimilarity(this DocumentSelectResult context, FeatureWeightModel TermWeightModel, IVectorSimilarityFunction function, ScoreComputationModeEnum groupmode, ILogBuilder log)
        {


            List<string> selectedTerms = context.selectedFeatures.GetKeys();

            Dictionary<String, WeightDictionary> documentDictionarties = new Dictionary<string, WeightDictionary>();


            foreach (var entry in context.items)
            {

                WeightDictionary documentWeights = TermWeightModel.GetWeights(selectedTerms, entry.spaceDocument, context.spaceModel);
                documentDictionarties.Add(entry.AssignedID, documentWeights);
            }


            FeatureVectorSetDictionary dict = new FeatureVectorSetDictionary();


            Double total = context.Count;
            Int32 i = 0;
            Int32 p = (context.Count / 10);




            Dictionary<string, List<DocumentSelectResultEntry>> relative_groups = null;


            if (groupmode == ScoreComputationModeEnum.category)
            {
                Dictionary<string, List<string>> assignIDByLabel = context.spaceModel.LabelToDocumentLinks.GetAllRelationShipByName(true);

                relative_groups = context.GetByAssignIDCategory(assignIDByLabel, log);
                if (assignIDByLabel.ContainsKey(SpaceLabel.UNKNOWN)) assignIDByLabel.Remove(SpaceLabel.UNKNOWN);
                log.log("... Page Similarity ... Groups by category");

            }
            else if (groupmode == ScoreComputationModeEnum.site)
            {
                relative_groups = context.GetByDomain(log);
                log.log("... Page Similarity ... Groups by site");

            }
            else if (groupmode == ScoreComputationModeEnum.dataset)
            {
                relative_groups = new Dictionary<string, List<DocumentSelectResultEntry>>();
                relative_groups.Add("dataset", context.items);
                log.log("... Page Similarity ... dataset");
            }


            ConcurrentDictionary<String, Double> computedPairs = new ConcurrentDictionary<string, double>();


            foreach (var domainPair in relative_groups)
            {
                List<DocumentSelectResultEntry> relatives = domainPair.Value; //relative_groups[domainPair.Key].ToList();


                foreach (var entry in relatives)
                {

                    i++;
                    FeatureVector fv = new FeatureVector(entry.AssignedID);

                    // List<Double> d = new List<>();

                    fv.dimensions = new double[relatives.Count - 1];


                    // List<String> keys = documentDictionarties.Keys.ToList();

                    Int32 hostInd = relatives.IndexOf(entry);

                    Int32 c = 0;


                    //foreach (var pair in documentDictionarties)
                    //{

                    Parallel.ForEach(relatives, (pair) =>
                    {

                        Int32 ind = relatives.IndexOf(pair); // keys.IndexOf(pair.AssignedID);
                        if (ind >= hostInd)
                        {
                            ind = ind - 1;
                        }

                        if (pair.AssignedID != entry.AssignedID)
                        {
                            Double docToClassSimilarity = 0;

                            if (computedPairs.ContainsKey(entry.AssignedID + pair.AssignedID))
                            {
                                docToClassSimilarity = computedPairs[entry.AssignedID + pair.AssignedID];
                            }
                            else if (computedPairs.ContainsKey(pair.AssignedID + entry.AssignedID))
                            {
                                docToClassSimilarity = computedPairs[pair.AssignedID + entry.AssignedID];
                            }
                            else
                            {
                                var vecA = documentDictionarties[pair.AssignedID];
                                var vecB = documentDictionarties[entry.AssignedID];
                                docToClassSimilarity = function.ComputeSimilarity(vecA, vecB);
                                if (docToClassSimilarity > 0)
                                {

                                }
                                if (!computedPairs.ContainsKey(entry.AssignedID + pair.AssignedID))
                                {
                                    computedPairs.GetOrAdd(entry.AssignedID + pair.AssignedID, docToClassSimilarity);
                                    //computedPairs.AddOrUpdate(entry.AssignedID + pair.Key, docToClassSimilarity);
                                }
                                else if (!computedPairs.ContainsKey(pair.AssignedID + entry.AssignedID))
                                {
                                    computedPairs.GetOrAdd(pair.AssignedID + entry.AssignedID, docToClassSimilarity);
                                }

                            }

                            fv.dimensions[ind] = docToClassSimilarity;

                        }
                    });






                    Int32 r = i % p;
                    if (r == 0)
                    {
                        log.Append(" [" + i.GetRatio(context.Count).ToString("P2") + "] ");
                    }


                    dict.GetOrAdd(domainPair.Key).Add(fv, -1);
                }



            }



            log.log("... Preparation finished ...");

            return dict;


        }

        ///// <summary>
        ///// Transforms to fv dictionary.
        ///// </summary>
        ///// <param name="context">The context.</param>
        ///// <param name="TermWeightModel">The term weight model.</param>
        ///// <param name="function">The function.</param>
        ///// <returns></returns>
        //public static FeatureVectorSetDictionary TransformToFVDictionaryAsPageInCategorySimilarity(this DocumentSelectResult context, FeatureWeightModel TermWeightModel, IVectorSimilarityFunction function, ILogBuilder log)
        //{
        //    log.log("... Page Similarity ...");

        //    List<string> selectedTerms = context.selectedFeatures.GetKeys();



        //    var ByDomain = context.GetByDomain(log);

        //    Dictionary<string, List<string>> assignIDByLabel = context.featureSpace.labelToDocumentAssociations.GetAllRelationShipByName(true);

        //    var ByCategory = context.GetByAssignIDCategory(assignIDByLabel,log);

        //    Dictionary<String, List<DocumentSelectResultEntry>> EntryByLabel = new Dictionary<string, List<DocumentSelectResultEntry>>();



        //    Dictionary<String, WeightDictionary> documentDictionarties = new Dictionary<string, WeightDictionary>();


        //    foreach (var entry in context.items)
        //    {

        //        WeightDictionary documentWeights = TermWeightModel.GetWeights(selectedTerms, entry.spaceDocument, context.spaceModel);
        //        documentDictionarties.Add(entry.AssignedID, documentWeights);
        //    }


        //    FeatureVectorSetDictionary dict = new FeatureVectorSetDictionary();





        //    Double total = context.Count;
        //    Int32 i = 0;
        //    Int32 p = (context.Count / 10);

        //    //List<List<Double>> matrix = new List<List<double>>();

        //    //foreach (var entry in context.items)
        //    //{
        //    //    matrix.Add(new List<double>());
        //    //}


        //    //for (int x = 0; x < context.items.Count; x++)
        //    //{

        //    //    for (int y = 0; y < context.items.Count; x++)
        //    //    {



        //    //    }

        //    //}

        //    ConcurrentDictionary<String, Double> computedPairs = new ConcurrentDictionary<string, double>();


        //    foreach (var domainPair in ByCategory)
        //    {
        //        List<DocumentSelectResultEntry> relatives = ByCategory[domainPair.Key].ToList();


        //        foreach (var entry in relatives)
        //        {

        //            i++;
        //            FeatureVector fv = new FeatureVector(entry.AssignedID);

        //            // List<Double> d = new List<>();

        //            fv.dimensions = new double[relatives.Count - 1];


        //            // List<String> keys = documentDictionarties.Keys.ToList();

        //            Int32 hostInd = relatives.IndexOf(entry);

        //            Int32 c = 0;


        //            //foreach (var pair in documentDictionarties)
        //            //{

        //            Parallel.ForEach(relatives, (pair) =>
        //            {

        //                Int32 ind = relatives.IndexOf(pair); // keys.IndexOf(pair.AssignedID);
        //                if (ind >= hostInd)
        //                {
        //                    ind = ind - 1;
        //                }

        //                if (pair.AssignedID != entry.AssignedID)
        //                {
        //                    Double docToClassSimilarity = 0;

        //                    if (computedPairs.ContainsKey(entry.AssignedID + pair.AssignedID))
        //                    {
        //                        docToClassSimilarity = computedPairs[entry.AssignedID + pair.AssignedID];
        //                    }
        //                    else if (computedPairs.ContainsKey(pair.AssignedID + entry.AssignedID))
        //                    {
        //                        docToClassSimilarity = computedPairs[pair.AssignedID + entry.AssignedID];
        //                    }
        //                    else
        //                    {
        //                        var vecA = documentDictionarties[pair.AssignedID];
        //                        var vecB = documentDictionarties[entry.AssignedID];
        //                        docToClassSimilarity = function.ComputeSimilarity(vecA, vecB);
        //                        if (docToClassSimilarity > 0)
        //                        {

        //                        }
        //                        if (!computedPairs.ContainsKey(entry.AssignedID + pair.AssignedID))
        //                        {
        //                            computedPairs.GetOrAdd(entry.AssignedID + pair.AssignedID, docToClassSimilarity);
        //                            //computedPairs.AddOrUpdate(entry.AssignedID + pair.Key, docToClassSimilarity);
        //                        }
        //                        else if (!computedPairs.ContainsKey(pair.AssignedID + entry.AssignedID))
        //                        {
        //                            computedPairs.GetOrAdd(pair.AssignedID + entry.AssignedID, docToClassSimilarity);
        //                        }

        //                    }

        //                    fv.dimensions[ind] = docToClassSimilarity;

        //                }
        //            });






        //            Int32 r = i % p;
        //            if (r == 0)
        //            {
        //                log.Append(" [" + i.GetRatio(context.Count).ToString("P2") + "] ");
        //            }


        //            dict.GetOrAdd(entry.DomainID).Add(fv, -1);
        //        }



        //    }


        //    //foreach (KeyValuePair<string, FeatureVectorWithLabelIDSet> pair in dict)
        //    //{
        //    //    pair.Value.CloseDeploy();
        //    //}

        //    log.log("... Preparation finished ...");

        //    return dict;


        //}




        /// <summary>
        /// Transforms to fv dictionary.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="TermWeightModel">The term weight model.</param>
        /// <param name="function">The function.</param>
        /// <returns></returns>
        public static FeatureVectorSetDictionary TransformToFVDictionaryAsSiteSimilarity(this DocumentSelectResult context, FeatureWeightModel TermWeightModel, IVectorSimilarityFunction function, ILogBuilder log)
        {
            log.log("... Site Similarity ...");

            List<string> selectedTerms = context.selectedFeatures.GetKeys(); //.entries.Select(x => x.name)?.ToList();

            Dictionary<String, WeightDictionary> categoryDictionarties = new Dictionary<string, WeightDictionary>();
            Dictionary<String, WeightDictionary> documentDictionarties = new Dictionary<string, WeightDictionary>();

            var byDomain = context.GetByDomain(log);

            FeatureVectorSetDictionary dict = new FeatureVectorSetDictionary();


            Double total = context.Count;
            Int32 i = 0;
            Int32 p = (context.Count / 10);

            foreach (var pair in byDomain)
            {
                i++;
                SpaceDocumentModel siteModel = new SpaceDocumentModel();

                foreach (var ent in pair.Value)
                {
                    WeightDictionary documentWeights = TermWeightModel.GetWeights(selectedTerms, ent.spaceDocument, context.spaceModel);
                    documentDictionarties.Add(ent.AssignedID, documentWeights);
                    siteModel.Children.Add(ent.spaceDocument);

                    //siteModel.terms.MergeDictionary(ent.spaceDocument.terms);
                }

                siteModel.Flatten(false);

                categoryDictionarties.Add(pair.Key, TermWeightModel.GetWeights(selectedTerms, siteModel, context.spaceModel));


                foreach (var ent in pair.Value)
                {
                    FeatureVector fv = new FeatureVector(ent.AssignedID);
                    fv.dimensions = new double[context.spaceModel.labels.Count];

                    // documentDictionarties[ent.AssignedID].entries


                    var docToClassSimilarity = function.ComputeSimilarity(categoryDictionarties[pair.Key], documentDictionarties[ent.AssignedID]);

                    fv.dimensions[0] = docToClassSimilarity;

                    dict.GetOrAdd(pair.Key).Add(fv, -1);

                }

                Int32 r = i % p;
                if (r == 0)
                {
                    log.Append(" [" + i.GetRatio(context.Count).ToString("P2") + "] ");
                }


            }



            foreach (KeyValuePair<string, FeatureVectorWithLabelIDSet> pair in dict)
            {
                pair.Value.CloseDeploy();
            }

            log.log("... Preparation finished ...");

            return dict;

        }


        /// <summary>
        /// Transforms to fv dictionary.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="TermWeightModel">The term weight model.</param>
        /// <param name="function">The function.</param>
        /// <returns></returns>
        public static FeatureVectorSetDictionary TransformToFVDictionaryAsCategorySimilarity(this DocumentSelectResult context, FeatureWeightModel TermWeightModel, IVectorSimilarityFunction function, ILogBuilder log)
        {
            log.log("... Category Similarity ...");

            List<string> selectedTerms = context.selectedFeatures.GetKeys(); //.entries.Select(x => x.name)?.ToList();

            Dictionary<String, WeightDictionary> categoryDictionarties = new Dictionary<string, WeightDictionary>();
            foreach (SpaceLabel label in context.spaceModel.labels)
            {
                Relationship<SpaceLabel, SpaceCategoryModel> categoryModel = context.spaceModel.LabelToCategoryLinks.GetAllRelationships(label).FirstOrDefault();

                var c = TermWeightModel.GetWeights(selectedTerms, categoryModel.NodeB, context.spaceModel, label);
                categoryDictionarties.Add(label.name, c);
            }

            FeatureVectorSetDictionary dict = new FeatureVectorSetDictionary();

            String domainNameLast = "";

            Double total = context.Count;
            Int32 i = 0;
            Int32 p = (context.Count / 20);

            foreach (var entry in context.items)
            {

                i++;

                WeightDictionary documentWeights = TermWeightModel.GetWeights(selectedTerms, entry.spaceDocument, context.spaceModel);

                FeatureVector fv = new FeatureVector(entry.AssignedID);
                fv.dimensions = new double[context.spaceModel.labels.Count];

                Int32 c = 0;

                Parallel.ForEach(context.spaceModel.labels, (label) =>
                {
                    var docToClassSimilarity = function.ComputeSimilarity(categoryDictionarties[label.name], documentWeights);
                    fv.dimensions[context.spaceModel.labels.IndexOf(label)] = docToClassSimilarity;

                });


                Int32 r = i % p;
                if (r == 0)
                {
                    log.Append(" [" + i.GetRatio(context.Count).ToString("P2") + "] ");
                }


                dict.GetOrAdd(entry.DomainID).Add(fv, -1);
            }

            foreach (KeyValuePair<string, FeatureVectorWithLabelIDSet> pair in dict)
            {
                pair.Value.CloseDeploy();
            }

            log.log("... Preparation done...");
            return dict;

        }


        public static Dictionary<String, DocumentSelectResultEntry> GetByAssignedID(this IEnumerable<DocumentSelectResultEntry> context, ILogBuilder log)
        {
            Dictionary<String, DocumentSelectResultEntry> output = new Dictionary<string, DocumentSelectResultEntry>();



            foreach (DocumentSelectResultEntry entry in context)
            {
                if (!output.ContainsKey(entry.AssignedID))
                {
                    output.Add(entry.AssignedID, entry);
                }


            }

            return output;
        }

        /// <summary>
        /// Gets the aligned by assigned identifier.
        /// </summary>
        /// <param name="scoreSet">The score set.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public static Dictionary<String, List<DocumentSelectResultEntry>> GetAlignedByAssignedID(this IEnumerable<IEnumerable<DocumentSelectResultEntry>> scoreSet, ILogBuilder log)
        {
            var output = new Dictionary<string, List<DocumentSelectResultEntry>>();

            foreach (var dict in scoreSet)
            {
                Dictionary<string, DocumentSelectResultEntry> byAssignedID = dict.GetByAssignedID(log);
                foreach (var pair in byAssignedID)
                {
                    if (!output.ContainsKey(pair.Key))
                    {
                        output.Add(pair.Key, new List<DocumentSelectResultEntry>());
                    }
                    output[pair.Key].Add(pair.Value);
                }
            }

            return output;
        }

        /// <summary>
        /// Gets the by assigned identifier.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public static Dictionary<String, DocumentSelectResultEntry> GetByAssignedID(this DocumentSelectResult context, ILogBuilder log)
        {

            return context.items.GetByAssignedID(log);
        }

        /// <summary>
        /// Sorts entries by domain name
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public static Dictionary<String, List<DocumentSelectResultEntry>> GetByDomain(this DocumentSelectResult context, ILogBuilder log)
        {
            return context.items.GetByDomain(log);
        }

        public static Dictionary<String, List<DocumentSelectResultEntry>> GetByDomain(this IEnumerable<DocumentSelectResultEntry> context, ILogBuilder log)
        {
            Dictionary<String, List<DocumentSelectResultEntry>> output = new Dictionary<string, List<DocumentSelectResultEntry>>();

            foreach (DocumentSelectResultEntry entry in context)
            {


                if (!output.ContainsKey(entry.DomainID))
                {
                    output.Add(entry.DomainID, new List<DocumentSelectResultEntry>());
                }

                output[entry.DomainID].Add(entry);
            }

            return output;

        }

        /// <summary>
        /// Gets nested dictionaries: [category][domain][assignedID]
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public static Dictionary<String, Dictionary<String, Dictionary<String, DocumentSelectResultEntry>>> GetByCategoryDomainAssignedID(this DocumentSelectResult context, ILogBuilder log)
        {
            Dictionary<String, Dictionary<String, Dictionary<String, DocumentSelectResultEntry>>> output = new Dictionary<string, Dictionary<string, Dictionary<string, DocumentSelectResultEntry>>>();

            var byAssigned = context.GetByAssignedID(log);

            List<string> labels = context.spaceModel.LabelToDocumentLinks.GetAllDistinctNames();

            foreach (String label in labels)
            {
                output.Add(label, new Dictionary<String, Dictionary<String, DocumentSelectResultEntry>>());

                List<SpaceDocumentModel> linked_documents = context.spaceModel.LabelToDocumentLinks.GetAllLinkedB(label);

                List<DocumentSelectResultEntry> underLabel = new List<DocumentSelectResultEntry>();

                foreach (var sdoc in linked_documents)
                {
                    underLabel.Add(byAssigned[sdoc.name]);
                }


                var byDomain = underLabel.GetByDomain(log);

                foreach (var sitePair in byDomain)
                {
                    output[label].Add(sitePair.Key, new Dictionary<string, DocumentSelectResultEntry>());

                    foreach (var pagePair in sitePair.Value)
                    {
                        output[label][sitePair.Key].Add(pagePair.AssignedID, pagePair);
                    }
                }

            }




            return output;

        }


        /// <summary>
        /// Nests the space document model.
        /// </summary>
        /// <param name="distionary">The distionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public static SpaceDocumentModel NestSpaceDocumentModel(this Dictionary<String, SpaceDocumentModel> distionary, String key, ILogBuilder log)
        {
            SpaceDocumentModel output = new SpaceDocumentModel();
            output.name = key;

            foreach (var p in distionary)
            {
                output.Children.Add(p.Value);
                output.terms.MergeDictionary(p.Value.terms);
            }

            return output;
        }


        /// <summary>
        /// Nests the complete space document model.
        /// </summary>
        /// <param name="nest">The nest.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public static SpaceDocumentModel NestCompleteSpaceDocumentModel(this Dictionary<string, Dictionary<string, Dictionary<string, SpaceDocumentModel>>> nest, String name, ILogBuilder log)
        {
            SpaceDocumentModel output = new SpaceDocumentModel();

            Dictionary<String, SpaceDocumentModel> rootTmp = new Dictionary<string, SpaceDocumentModel>();

            foreach (var pairCategory in nest)
            {
                Dictionary<String, SpaceDocumentModel> catTmp = new Dictionary<string, SpaceDocumentModel>();

                foreach (var pairWebsite in pairCategory.Value)
                {

                    catTmp.Add(pairWebsite.Key, pairWebsite.Value.NestSpaceDocumentModel(pairWebsite.Key, log));

                }

                rootTmp.Add(pairCategory.Key, catTmp.NestSpaceDocumentModel(pairCategory.Key, log));


            }

            output = rootTmp.NestSpaceDocumentModel(name, log);

            return output;
        }


        /// <summary>
        /// Gets nested dictionaries: [category][domain][assignedID]
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public static Dictionary<String, Dictionary<String, Dictionary<String, SpaceDocumentModel>>> GetModelsByCategoryDomainAssignedID(this DocumentSelectResult context, ILogBuilder log)
        {
            Dictionary<String, Dictionary<String, Dictionary<String, DocumentSelectResultEntry>>> entries = context.GetByCategoryDomainAssignedID(log);

            Dictionary<String, Dictionary<String, Dictionary<String, SpaceDocumentModel>>> output = new Dictionary<string, Dictionary<string, Dictionary<string, SpaceDocumentModel>>>();

            foreach (var pairCategory in entries)
            {
                var categoryDictionary = new Dictionary<string, Dictionary<string, SpaceDocumentModel>>();

                foreach (var pairWebsite in pairCategory.Value)
                {

                    var domainDictionary = new Dictionary<string, SpaceDocumentModel>();

                    foreach (var pairWebpage in pairWebsite.Value)
                    {
                        domainDictionary.Add(pairWebpage.Key, pairWebpage.Value.spaceDocument);
                    }

                    categoryDictionary.Add(pairWebsite.Key, domainDictionary);
                }

                output.Add(pairCategory.Key, categoryDictionary);
            }


            return output;

        }



        //public static Dictionary<String, Dictionary<String, Dictionary<String, DocumentSelectResultEntry>>> GetByCategoryDomainAssignedID (this IEnumerable<DocumentSelectResultEntry> entries, SpaceModel model, ILogBuilder log)
        //{
        //    Dictionary<String, Dictionary<String, Dictionary<String, DocumentSelectResultEntry>>> output = new Dictionary<string, Dictionary<string, Dictionary<string, DocumentSelectResultEntry>>>();


        //    var byDomain = entries.GetByDomain(log);

        //    var byCategoryAssignedID = GetByAssignIDCategory()

        //}

        /// <summary>
        /// Gets the by assign identifier category.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="catIndex">Index of the cat.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public static Dictionary<String, List<DocumentSelectResultEntry>> GetByAssignIDCategory(this DocumentSelectResult context, Dictionary<string, List<string>> catIndex, ILogBuilder log)
        {
            Dictionary<String, List<DocumentSelectResultEntry>> output = new Dictionary<string, List<DocumentSelectResultEntry>>();


            Dictionary<String, DocumentSelectResultEntry> byID = context.GetByAssignedID(log);


            foreach (var pair in catIndex)
            {
                output.Add(pair.Key, new List<DocumentSelectResultEntry>());

                foreach (var k in pair.Value)
                {
                    output[pair.Key].Add(byID[k]);

                }
            }



            return output;
        }



        /// <summary>
        /// Normalizes score within domain
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public static Dictionary<String, rangeFinder> NormalizeWithinDomain(this IEnumerable<DocumentSelectResultEntry> context, ILogBuilder log)
        {
            Dictionary<String, List<DocumentSelectResultEntry>> byDomain = context.GetByDomain(log);
            Dictionary<String, rangeFinder> output = new Dictionary<string, rangeFinder>();

            foreach (var pair in byDomain)
            {
                rangeFinder ranger = new rangeFinder(pair.Key);

                foreach (DocumentSelectResultEntry entry in pair.Value)
                {
                    ranger.Learn(entry.score);
                }

                foreach (DocumentSelectResultEntry entry in pair.Value)
                {
                    entry.score = ranger.GetPositionInRange(entry.score);
                }

                output.Add(ranger.id, ranger);

            }
            return output;
        }





        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public static DocumentSelectResult ExecuteLimit(this DocumentSelectQuery query, DocumentSelectResult context, ILogBuilder log)
        {


            if (query.options.HasFlag(DocumentSelectQueryOptions.DomainLevelNormalization))
            {
                log.log("DS Scores normalized on website / domain level");
                context.items.NormalizeWithinDomain(log);
            }


            // QUERY LIMITS
            List<DocumentSelectResultEntry> sortedList = context.items.OrderByDescending(x => x.score).ToList();
            context.items.Clear();

            context.items.AddRange(sortedList);


            if (query.TrasholdLimit != 0.0)
            {
                List<DocumentSelectResultEntry> underTrashold = new List<DocumentSelectResultEntry>();
                foreach (DocumentSelectResultEntry entry in context.items)
                {
                    if (entry.score < query.TrasholdLimit)
                    {
                        underTrashold.Add(entry);
                    }
                }
                foreach (DocumentSelectResultEntry entry in underTrashold)
                {
                    context.items.Remove(entry);
                }
            }



            if (query.SizeLimit > 0)
            {

                if (query.options.HasFlag(DocumentSelectQueryOptions.ApplyDomainLevelLimits))
                {

                    List<DocumentSelectResultEntry> overLimit = new List<DocumentSelectResultEntry>();

                    var byDomain = context.GetByDomain(log);

                    foreach (var pair in byDomain)
                    {
                        Int32 count = 0;
                        List<DocumentSelectResultEntry> domainSortedList = pair.Value.OrderByDescending(x => x.score).ToList();

                        if (query.options.HasFlag(DocumentSelectQueryOptions.ForceHomePage))
                        {
                            DocumentSelectResultEntry homeEntry = domainSortedList.GetDocWithShortestID();
                            if (domainSortedList.Remove(homeEntry))
                            {
                                count++;
                            }

                        }

                        foreach (DocumentSelectResultEntry entry in domainSortedList)
                        {

                            if (count >= query.SizeLimit)
                            {
                                overLimit.Add(entry);
                            }
                            count++;
                        }


                    }


                    foreach (DocumentSelectResultEntry entry in overLimit)
                    {
                        context.Remove(entry);
                    }

                }
                else
                {
                    if (context.Count > query.SizeLimit)
                    {
                        context.RemoveRange(query.SizeLimit, context.Count - query.SizeLimit);
                    }
                }
            }




            return context;
        }
    }
}