using imbNLP.Toolkit.Feature;
using imbNLP.Toolkit.Space;
using imbSCI.Core.enums;
using imbSCI.Core.files.folders;
using imbSCI.Core.math;
using imbSCI.Core.math.range.histogram;
using imbSCI.Core.math.range.matrix;
using imbSCI.Core.reporting;
using imbSCI.Data.collection.nested;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace imbNLP.Toolkit.Processing
{

    /// <summary>
    /// 
    /// </summary>
    public static class WeightDictionaryTools
    {




        public static List<Double> GetDistinctValuesAtVector(this IVectorDimensions vector)
        {
            List<Double> distinct = new List<Double>();

            foreach (Double d in vector.dimensions)
            {
                if (!distinct.Contains(d))
                {
                    distinct.Add(d);
                }
            }

            return distinct;
        }




        public static Int32 GetTokenCount(this List<SpaceDocumentModel> documents)
        {
            Int32 output = 0;

            for (int i = 0; i < documents.Count; i++)
            {
                output += documents[i].terms.Count;
            }
            return output;
        }


        public static Int32 GetTokenFrequency(this List<SpaceDocumentModel> documents, String term)
        {
            Int32 output = 0;

            for (int i = 0; i < documents.Count; i++)
            {
                output += documents[i].terms.GetTokenFrequency(term);
            }
            return output;
        }

        /// <summary>
        /// Gets the heat map matrix.
        /// </summary>
        /// <param name="dictionaries">The dictionaries.</param>
        /// <returns></returns>
        public static HeatMapModel GetHeatMapMatrix(this IEnumerable<WeightDictionary> dictionaries)
        {
            OverlapMatrix<WeightDictionaryEntry> matrix = dictionaries.GetOverlapMatrix(); // = new List<List<WeightDictionaryEntry>>();

            List<String> dictnames = dictionaries.Select(x => x.name).ToList();

            HeatMapModel output = new HeatMapModel(dictnames);


            for (int i = 0; i < matrix.width; i++)
            {
                for (int y = 0; y < matrix.height; y++)
                {

                    output[i, y] = matrix[i, y].GetWeightSum();
                }
            }

            output.DetectMinMax();


            return output;

        }

        public static histogramModel GetHistogram(this WeightDictionary dictionary, Int32 binCount = 50)
        {
            histogramModel model = dictionary.index.Values.ToList().GetHistogramModel(dictionary.name, x => x.weight, binCount); //new histogramModel(binCount, dictionary.name);
            return model;
        }


        public static histogramModel GetHistogram(this SpaceDocumentModel dictionary, Int32 binCount = 50)
        {
            histogramModel model = dictionary.terms.GetRankedTokenFrequency().GetHistogramModel(dictionary.name, x => x.Value, binCount); //new histogramModel(binCount, dictionary.name);
            return model;
        }


        /// <summary>
        /// Gets the heat map matrix.
        /// </summary>
        /// <param name="dictionaries">The dictionaries.</param>
        /// <returns></returns>
        public static HeatMapModel GetHeatMapMatrix(this IEnumerable<SpaceDocumentModel> dictionaries)
        {
            OverlapMatrix<String> matrix = dictionaries.Select(x => x.terms).GetOverlapMatrix(); // = new List<List<WeightDictionaryEntry>>();
            var terms = dictionaries.Select(x => x.terms).ToList();
            List<String> dictnames = dictionaries.Select(x => x.name).ToList();

            HeatMapModel output = new HeatMapModel(dictnames);


            for (int i = 0; i < matrix.width; i++)
            {
                for (int y = 0; y < matrix.height; y++)
                {
                    output[i, y] = terms[i].GetTokenFrequencies(matrix[i, y]);
                }
            }

            output.DetectMinMax();


            return output;

        }

        public static Double GetWeightSum(this IEnumerable<WeightDictionaryEntry> matrix)
        {
            Double output = matrix.Sum(x => x.weight);

            return output;
        }
        /*
        public static HeatMapModel GetHeatMap(this OverlapMatrix<WeightDictionaryEntry> matrix)
        {
            HeatMapModel output = HeatMapModel.Create(matrix.width, matrix.height);

            output.Deploy()
        }
        */

        /// <summary>
        /// Gets the overlap matrix.
        /// </summary>
        /// <param name="dictionaries">The dictionaries.</param>
        /// <returns></returns>
        public static OverlapMatrix<String> GetOverlapMatrix(this IEnumerable<TokenDictionary> dictionaries)
        {
            var collections = new List<List<String>>();

            foreach (var dict in dictionaries)
            {
                collections.Add(dict.GetTokens());
            }

            OverlapMatrix<String> output = new OverlapMatrix<string>(collections);
            return output;
        }


        /// <summary>
        /// Gets the overlap matrix.
        /// </summary>
        /// <param name="dictionaries">The dictionaries.</param>
        /// <returns></returns>
        public static OverlapMatrix<WeightDictionaryEntry> GetOverlapMatrix(this IEnumerable<WeightDictionary> dictionaries)
        {
            List<List<WeightDictionaryEntry>> collections = new List<List<WeightDictionaryEntry>>();

            foreach (var dict in dictionaries)
            {
                collections.Add(dict.index.Values.ToList());
            }

            OverlapMatrix<WeightDictionaryEntry> output = new OverlapMatrix<WeightDictionaryEntry>(collections);
            return output;

        }


        /// <summary>
        /// Merges the ds rankings - searches folder for specified input names or search pattern
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="inputNames">The input names.</param>
        /// <param name="output">The output.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns></returns>
        public static FeatureVectorDictionaryWithDimensions MergeWeightDictionaries(folderNode folder, String inputNames, ILogBuilder output, String searchPattern = "*_wt.xml")
        {
            List<string> filepaths = folder.GetOrFindFiles(inputNames, searchPattern);


            List<WeightDictionary> results = new List<WeightDictionary>();

            String tmpOutputName = "";


            Int32 c = 0;

            foreach (var fp in filepaths)
            {
                var lr = WeightDictionary.LoadFile(fp, output); //DocumentSelectResult.LoadFromFile(fp, output);

                lr.description += "Source name: " + lr.name;
                String fn = Path.GetFileNameWithoutExtension(fp);
                lr.name = fn + c.ToString("D3");
                c++;
                results.Add(lr);

            }


            FeatureVectorDictionaryWithDimensions featureDict = MergeWeightDictionaries(results);

            return featureDict;


        }

        /// <summary>
        /// Merges base dimension of each weight dictionary
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns></returns>
        public static WeightDictionary MergeDimensions(this List<WeightDictionary> results)
        {
            WeightDictionary output = new WeightDictionary();
            output.nDimensions = results.Count;


            Dictionary<String, Double[]> tempMatrix = new Dictionary<string, double[]>();


            Int32 id = 0;
            foreach (var dict in results)
            {
                output.name += dict.name;

                foreach (var en in dict.index)
                {
                    if (!tempMatrix.ContainsKey(en.Key)) tempMatrix.Add(en.Key, new Double[output.nDimensions]);
                    tempMatrix[en.Key][id] = en.Value.weight;
                }

                id++;
            }


            foreach (var pair in tempMatrix)
            {
                output.AddEntry(pair.Key, pair.Value, false);
            }

            return output;
        }


        public static FeatureVectorDictionaryWithDimensions MergeWeightDictionaries(this List<WeightDictionary> results)
        {
            FeatureVectorDictionaryWithDimensions featureDict = new FeatureVectorDictionaryWithDimensions();
            Int32 i = 0;
            foreach (var dict in results)
            {
                featureDict.dimensions.Add(dict.name + i.ToString(), "Weights from [" + dict.name + "]", Feature.Settings.FeatureVectorDimensionType.precompiledDocumentScore, "WeightDictionary");
            }
            WeightDictionary output = MergeDimensions(results);

            var fvs = output.index.Values.ToFeatureVectors();
            foreach (var fv in fvs)
            {
                featureDict.Add(fv);
            }

            return featureDict;
        }

        public static List<FeatureVector> ToFeatureVectors(this IEnumerable<WeightDictionaryEntry> entries)
        {
            List<FeatureVector> output = new List<FeatureVector>();
            foreach (var entry in entries)
            {
                output.Add(entry.ToFeatureVector());
            }
            return output;
        }





        /// <summary>
        /// Computes proportion between value of the true dimension and sum of values of all dimensions. If true dimension is unspecified, it will assume that the highest value represents the true dimension
        /// </summary>
        /// <param name="nVector">The n vector.</param>
        /// <param name="TrueDimension">The true dimension.</param>
        /// <returns></returns>
        public static Double CompressByTrueDimension(this IVectorDimensions nVector, Int32 TrueDimension = -1)
        {
            if (TrueDimension == -1)
            {
                TrueDimension = nVector.GetDominantDimension();
            }


            Int32 c = 0;
            Double m = 0;

            Double TValue = 0;
            Double TAll = 0;
            foreach (Double d in nVector.dimensions)
            {
                if (c == TrueDimension)
                {
                    TValue = d;
                }
                TAll += d;
                c++;
            }


            return TValue.GetRatio(TAll);

        }

        /// <summary>
        /// Returns index of dimension with highest value
        /// </summary>
        /// <returns></returns>
        public static Int32 GetDominantDimension(this IVectorDimensions nVector)
        {
            Int32 i = 0;
            Int32 c = 0;
            Double m = Double.MinValue;
            foreach (Double d in nVector.dimensions)
            {
                if (d > m)
                {
                    m = d;
                    i = c;
                }
                c++;
            }
            return i;
        }




        /// <summary>
        /// Compresses the numeric vector.
        /// </summary>
        /// <param name="nVector">The n vector.</param>
        /// <param name="operation">Allowed operations: <see cref="operation.multiplication"/>, <see cref="operation.max"/>, <see cref="operation.min"/>, <see cref="operation.plus"/></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Operation [" + operation.ToString() + "] not supported by this method. - operation</exception>
        public static Double CompressNumericVector(this IVectorDimensions nVector, operation operation)
        {
            return nVector.dimensions.CompressNumericVector(operation);

        }

    }

}