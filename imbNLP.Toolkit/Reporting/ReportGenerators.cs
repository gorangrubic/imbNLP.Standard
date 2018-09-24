using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Feature;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbSCI.Core.extensions.table;
using imbSCI.Data.interfaces;
using imbSCI.DataComplex.extensions.data.schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace imbNLP.Toolkit.Reporting
{

    public static class ReportGenerators
    {

        /// <summary>
        /// Makes the table.
        /// </summary>
        /// <typeparam name="TNodeA">The type of the node a.</typeparam>
        /// <typeparam name="TNodeB">The type of the node b.</typeparam>
        /// <param name="relationship">The relationship.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        public static DataTable MakeTable<TNodeA, TNodeB>(this Relationships<TNodeA, TNodeB> relationship, String name, String description)
            where TNodeA : IObjectWithName
    where TNodeB : IObjectWithName
        {
            DataTable table = new DataTable();
            table.SetTitle(name);
            table.SetDescription(description);

            DataColumn column_rank = table.Add("Nr", "Order number", "Nr", typeof(Int32), imbSCI.Core.enums.dataPointImportance.normal).SetWidth(10);
            DataColumn column_nodeA = table.Add("A", "Node [" + typeof(TNodeA).Name + "] A", "A", typeof(String), imbSCI.Core.enums.dataPointImportance.normal).SetWidth(30);
            DataColumn column_nodeB = table.Add("B", "Node [" + typeof(TNodeB).Name + "] B", "B", typeof(String), imbSCI.Core.enums.dataPointImportance.normal).SetWidth(30);
            DataColumn column_weight = table.Add("W", "Weight og the relationship", "W", typeof(Double), imbSCI.Core.enums.dataPointImportance.normal, "F2").SetWidth(20);

            Int32 p = 0;

            Int32 c = 1;
            foreach (var pair in relationship.links)
            {
                var dr = table.NewRow();

                dr[column_rank] = c;
                dr[column_nodeA] = pair.NodeA.name;
                dr[column_nodeB] = pair.NodeB.name;
                dr[column_weight] = pair.weight;

                c++;
                table.Rows.Add(dr);
            }

            table.AddExtra("Type of node A [" + typeof(TNodeA).Name + "]");
            table.AddExtra("Type of node B [" + typeof(TNodeB).Name + "]");


            return table;
        }


        public static DataTable MakeTable(this WebSiteDocumentsSet docSet, Dictionary<String, SpaceDocumentModel> docModels)
        {
            DataTable table = new DataTable();
            table.SetTitle(docSet.name);
            table.SetDescription(docSet.description);

            DataColumn column_rank = table.Add("Nr", "Order number", "Nr", typeof(Int32), imbSCI.Core.enums.dataPointImportance.normal).SetWidth(10);
            DataColumn column_domain = table.Add("Domain", "Web site domain", "Domain", typeof(String), imbSCI.Core.enums.dataPointImportance.normal).SetWidth(40);
            DataColumn column_page = table.Add("Pages", "Number of pages for the website", "Pages", typeof(Int32), imbSCI.Core.enums.dataPointImportance.normal).SetWidth(20);
            String g1 = "Presentation";

            DataColumn column_Terms = table.Add("Terms", "Number of distinct terms", "Terms", typeof(Int32), imbSCI.Core.enums.dataPointImportance.normal).SetWidth(20).SetGroup(g1).SetDefaultBackground("#FF6633");
            DataColumn column_Tokens = table.Add("Tokens", "Total number of tokens", "Tokens", typeof(Int32), imbSCI.Core.enums.dataPointImportance.normal).SetWidth(20).SetGroup(g1).SetDefaultBackground("#FF6633");


            Int32 p = 0;

            Int32 c = 1;
            foreach (var pair in docSet)
            {
                var dr = table.NewRow();

                dr[column_rank] = c;
                dr[column_domain] = pair.domain;
                dr[column_page] = pair.documents.Count;

                var docModel = docModels[pair.domain];
                dr[column_Terms] = docModel.terms.Count;
                dr[column_Tokens] = docModel.terms.GetSumFrequency();

                p += pair.documents.Count;
                c++;
                table.Rows.Add(dr);
            }

            table.AddExtra("Category name [" + docSet.name + "]");
            table.AddExtra("Category description [" + docSet.description + "]");


            table.SetAdditionalInfoEntry("Websites", docSet.Count, "Number of websites in the set");
            table.SetAdditionalInfoEntry("Web pages", p, "Total count of pages");
            //    table.SetAdditionalInfoEntry("Total tokens", terms.GetSumFrequency(), "Total number of tokens extracted from the corpus/document, i.e. sum of all frequencies");

            return table;
        }

        public static DataTable MakeTable(this FeatureSpace space, FeatureVectorConstructor constructor, String name, String description)
        {

            DataTable table = new DataTable();
            table.SetTitle(name);
            table.SetDescription(description);

            table.SetAdditionalInfoEntry("Documents", space.documents.Count, "Total count of document vectors");
            table.SetAdditionalInfoEntry("Dimensions", constructor.dimensionFunctionSet.Count, "Number of dimensions");

            DataColumn column_rank = table.Add("Nr", "Order of appereance", "N", typeof(Int32), imbSCI.Core.enums.dataPointImportance.normal).SetWidth(10);

            DataColumn column_token = table.Add("Name", "Name of the document vector", "Name", typeof(String), imbSCI.Core.enums.dataPointImportance.normal).SetWidth(50);
            List<DataColumn> dimensions = new List<DataColumn>();


            foreach (FeatureSpaceDimensionBase dim in constructor.dimensionFunctionSet)
            {
                DataColumn dim_col = null;
                String prefix = dimensions.Count.ToString("D3");
                if (dim is FeatureSpaceDimensionSimilarity dimSim)
                {
                    dim_col = table.Add(prefix + "-" + dimSim.classVector.name, "Dimension computed as [" + dimSim.similarityFunction.GetType().Name + "] between document vector and [" + dimSim.classVector.name + "]", "D" + prefix, typeof(Double), imbSCI.Core.enums.dataPointImportance.important, "F5");
                    dimensions.Add(dim_col);
                }
            }

            DataColumn column_label = table.Add("Label", "Affiliation to a category", "Label", typeof(String), imbSCI.Core.enums.dataPointImportance.normal).SetWidth(50);

            Int32 r = 1;
            foreach (var docVec in space.documents)
            {
                var dr = table.NewRow();

                dr[column_rank] = r;
                dr[column_token] = docVec.name;
                Int32 di = 0;
                foreach (DataColumn dc in dimensions)
                {
                    if (di < docVec.dimensions.Length)
                    {
                        Double val = docVec.dimensions[di];
                        dr[dc] = val;
                    }

                    di++;
                }

                table.Rows.Add(dr);
                String lbl_str = "";
                var lbl = space.labelToDocumentAssociations.GetAllLinked(docVec).FirstOrDefault();
                if (lbl != null) lbl_str = lbl.name;

                dr[column_label] = lbl_str;

                r++;
            }

            return table;


        }


        /// <summary>
        /// Makes ranked weight table
        /// </summary>
        /// <param name="terms">The terms.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="dimension">Custom names of dimensions - for case of vector collection</param>
        /// <param name="limit">The limit.</param>
        /// <returns></returns>
        public static DataTable MakeTable(this WeightDictionary terms, String name, String description, List<String> dimension = null, Int32 limit = 1000)
        {
            DataTable table = new DataTable();
            table.SetTitle(name);
            table.SetDescription(description);

            List<WeightDictionaryEntry> ranking = terms.entries.OrderByDescending(x => x.weight).ToList();

            if (dimension == null)
            {
                dimension = new List<string>();
                dimension.Add("Weight");
            }

            table.SetAdditionalInfoEntry("Count", terms.entries, "Total weighted features in the dictionary");
            table.SetAdditionalInfoEntry("Dimensions", dimension.Count, "Number of dimensions");


            DataColumn column_rank = table.Add("Rank", "Rank by frequency", "R", typeof(Int32), imbSCI.Core.enums.dataPointImportance.normal).SetWidth(10);

            DataColumn column_token = table.Add("Token", "Token", "t", typeof(String), imbSCI.Core.enums.dataPointImportance.normal).SetWidth(50);

            List<DataColumn> dimensions = new List<DataColumn>();

            Int32 cd = 1;
            foreach (String dim in dimension)
            {
                dimensions.Add(table.Add(dim, "Associated dimension [" + cd.ToString() + "] " + dim, dim, typeof(Double), imbSCI.Core.enums.dataPointImportance.normal, "F5", dim));
                cd++;
            }



            var list = ranking.Take(Math.Min(limit, ranking.Count)).ToList();
            if (list.Count < terms.entries.Count)
            {
                table.AddExtra("Table contains top [" + list.Count + "] entries, out of [" + terms.entries.Count + "] enumerated in the feature weighted dictionary");
            }


            Int32 c = 1;
            foreach (var pair in list)
            {
                var dr = table.NewRow();

                dr[column_rank] = c;
                //dr[column_id] = terms.GetTokenID(pair.Key);
                dr[column_token] = pair.name;

                Int32 ci = 0;
                foreach (DataColumn dimCol in dimensions)
                {
                    if (ci < pair.dimensions.Length)
                    {
                        dr[dimCol] = pair.dimensions[ci];
                    }
                    ci++;
                }

                //dr[column_freq] = pair.Value;
                c++;
                table.Rows.Add(dr);
            }



            return table;
        }


        /// <summary>
        /// Makes ranked table with term frequencies
        /// </summary>
        /// <param name="terms">The terms.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="limit">The limit.</param>
        /// <returns></returns>
        public static DataTable MakeTable(this TokenDictionary terms, string name, string description, Int32 limit = 1000)
        {
            DataTable table = new DataTable();
            table.SetTitle(name);
            table.SetDescription(description);

            table.SetAdditionalInfoEntry("Dictinct terms", terms.Count, "Total distinct terms in the dictionary");
            table.SetAdditionalInfoEntry("Max frequency", terms.GetMaxFrequency(), "Highest frequency");
            table.SetAdditionalInfoEntry("Total tokens", terms.GetSumFrequency(), "Total number of tokens extracted from the corpus/document, i.e. sum of all frequencies");

            DataColumn column_rank = table.Add("Rank", "Rank by frequency", "R", typeof(Int32), imbSCI.Core.enums.dataPointImportance.normal).SetWidth(20);
            DataColumn column_id = table.Add("ID", "Token ID", "id", typeof(Int32), imbSCI.Core.enums.dataPointImportance.normal).SetWidth(20);
            DataColumn column_token = table.Add("Token", "Token", "t", typeof(String), imbSCI.Core.enums.dataPointImportance.normal).SetWidth(50);
            DataColumn column_freq = table.Add("Frequency", "Absolute number of token occurrences in the corpus/document", "TF", typeof(Int32), imbSCI.Core.enums.dataPointImportance.normal).SetWidth(30);

            var tokens = terms.GetTokens();

            var list = terms.GetRankedTokenFrequency(limit);
            Int32 c = 1;
            foreach (var pair in list)
            {
                var dr = table.NewRow();

                dr[column_rank] = c;
                dr[column_id] = terms.GetTokenID(pair.Key);
                dr[column_token] = pair.Key;
                dr[column_freq] = pair.Value;
                c++;
                table.Rows.Add(dr);
            }

            if (terms.Count > limit)
            {
                table.AddExtra("Table contains only top [" + limit + "] entries, out of [" + terms.Count + "] enumerated in the dictionary");
            }

            return table;

        }

    }

}