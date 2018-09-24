// --------------------------------------------------------------------------------------------------------------------
// <copyright file="languageManagerWordnet.cs" company="imbVeles" >
//
// Copyright (C) 2018 imbVeles
//
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Project: imbNLP.Data
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
namespace imbNLP.Data.extended.wordnet
{
    using imbNLP.Data.extended.dict.core;
    using imbNLP.Data.semanticLexicon.explore;
    using imbNLP.Data.semanticLexicon.posCase;
    using imbNLP.PartOfSpeech.flags.basic;
    using imbNLP.PartOfSpeech.lexicUnit.tokenGraphs;
    using imbNLP.Transliteration;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.enums.reporting;
    using imbSCI.Data.interfaces;
    using imbSCI.DataComplex.extensions.data.formats;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// WordNET Serbian and English
    /// </summary>
    /// <seealso cref="tokenQueryResolverBase" />
    public class languageManagerWordnet : tokenQueryResolverBase
    {
        /// <summary>
        /// Returns words matching symset codes
        /// </summary>
        /// <param name="symsetCodes">The symset codes.</param>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        public wordnetSymsetResults query_eng_symset(List<String> symsetCodes, ILogBuilder response)
        {
            wordnetSymsetResults output = new wordnetSymsetResults();
            if (!isEngWordNetReady) prepare_eng(response);

            List<DataRow> matches = new List<DataRow>();
            foreach (DataTable dt in wordnet_eng.Tables)
            {
                DataColumn cColumn = dt.Columns[1];
                cColumn.ColumnName = "cCol";
                foreach (String code in symsetCodes)
                {
                    matches.AddRange(dt.Select(cColumn.ColumnName + " = '" + code + "'"));
                }
            }

            foreach (DataRow dr in matches)
            {
                DataColumn wColumn = dr.Table.Columns[0];
                wColumn.ColumnName = "wCol";
                DataColumn cColumn = dr.Table.Columns[1];
                cColumn.ColumnName = "cCol";
                String eng = dr[wColumn].toStringSafe();
                String code = dr[cColumn].toStringSafe();
                output.Add(code, eng);
            }

            if (response != null) response.log("wordnet_eng_symset(" + String.Join(",", symsetCodes.ToArray()) + ") = rows[" + matches.Count + "] => words[" + output.GetEnglish().Count + "]");

            return output;
        }

        /// <summary>
        /// Queries Wordnet_eng codes by words/tokens/lemmas. This should be used before <see cref="query_eng_symset(List{string}, imbSCI.Core.interfaces.ILogBuilder)"/>
        /// </summary>
        /// <param name="eng_tokens">The eng tokens.</param>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        public wordnetSymsetResults query_eng(List<String> eng_tokens, ILogBuilder response)
        {
            //List<symsetSpark> output = new List<symsetSpark>();
            wordnetSymsetResults output = new wordnetSymsetResults();
            if (!isEngWordNetReady) prepare_eng(response);

            List<DataRow> matches = new List<DataRow>();
            foreach (DataTable dt in wordnet_eng.Tables)
            {
                DataColumn wColumn = dt.Columns[0];
                wColumn.ColumnName = "wCol";
                foreach (String tkn in eng_tokens)
                {
                    matches.AddRange(dt.Select(wColumn.ColumnName + " LIKE '" + tkn + "'"));
                }
            }

            if (response != null) response.log("wordnet_eng(" + String.Join(",", eng_tokens.ToArray()) + ") = rows[" + matches.Count() + "]");

            foreach (DataRow dr in matches)
            {
                DataColumn wColumn = dr.Table.Columns[0];
                DataColumn cColumn = dr.Table.Columns[1];
                String eng = dr[wColumn].toStringSafe();
                String code = dr[cColumn].toStringSafe();
                output.Add(code, eng);
            }

            if (response != null) response.log("wordnet_eng(" + String.Join(",", eng_tokens.ToArray()) + ") = rows[" + matches.Count() + "] => token-symset[" + output.Count + "]");

            return output;
        }

        public wordnetSymsetResults query_srb_symset(List<String> symsetCodes, ILogBuilder response)
        {
            wordnetSymsetResults output = new wordnetSymsetResults();
            getReady();

            List<DataRow> matches = new List<DataRow>();
            foreach (String code in symsetCodes)
            {
                matches.AddRange(table.Select(SRB_COLUMN_CODE + " = '" + code + "'"));
            }

            foreach (DataRow dr in matches)
            {
                String srb = dr[SRB_COLUMN_TOKEN].toStringSafe();
                String code = dr[SRB_COLUMN_CODE].toStringSafe();
                output.Add(code, srb);
            }

            if (response != null) response.log("wordnet_srb_symset(" + String.Join(",", symsetCodes) + ") = rows[" + matches.Count + "] => words[" + output.GetEnglish().Count + "]");

            return output;
        }

        public wordnetSymsetResults query_srb(List<String> srb_tokens, ILogBuilder response, Boolean buildModel = true)
        {
            wordnetSymsetResults output = new wordnetSymsetResults();
            getReady();

            List<DataRow> matches = new List<DataRow>();
            foreach (String tkn in srb_tokens)
            {
                matches.AddRange(table.Select(SRB_COLUMN_TOKEN + " LIKE '" + tkn + "'"));
            }

            foreach (DataRow dr in matches)
            {
                String eng = dr[SRB_COLUMN_TOKEN].toStringSafe();
                String code = dr[SRB_COLUMN_CODE].toStringSafe();
                output.Add(code, eng);
            }

            if (buildModel)
            {
                foreach (var pair in output)
                {
                    termExploreModel md = null;
                    String srb = pair.Key;
                    if (!output.models.ContainsKey(srb))
                    {
                        md = new termExploreModel(srb);

                        output.models.Add(srb, md);
                    }
                    else
                    {
                        md = output.models[srb];
                    }

                    String symc = pair.Value[0].ToString();
                    pos_type pt = posConverter.wordNetFirstNumToPosType.getValue(symc, pos_type.none);

                    gramFlags gr = new gramFlags();
                    gr.Set(pt);
                    md.gramSet.Add(gr);

                    md.wordnetPrimarySymsets.AddUnique(pair.Value);
                }
            }

            return output;
        }

        private void resolve(tokenGraphNode graph, ref WordnetSource wn_type, ref WordnetQueryType wn_qtype)
        {
            Boolean doResolve = false;
            if (wn_type == WordnetSource.none) doResolve = true;
            if (wn_qtype == WordnetQueryType.none) doResolve = true;
            if (!doResolve) return;

            if (graph.Count() == 0) return;

            var parentType = graph.type;
            var childType = graph?.getFirst()?.type;

            switch (childType)
            {
                case tokenGraphNodeType.symset_code:
                    if (parentType == tokenGraphNodeType.word_srb)
                    {
                        wn_type = WordnetSource.serbian;
                        wn_qtype = WordnetQueryType.getSymsetCodesByWord;
                    }
                    else
                    {
                        wn_type = WordnetSource.english;
                        wn_qtype = WordnetQueryType.getSymsetCodesByWord;
                    }
                    break;

                case tokenGraphNodeType.word_eng:
                case tokenGraphNodeType.word_query:
                    wn_type = WordnetSource.english;
                    wn_qtype = WordnetQueryType.getWordsBySymsetCode;
                    break;

                case tokenGraphNodeType.word_srb:
                    wn_type = WordnetSource.serbian;
                    wn_qtype = WordnetQueryType.getWordsBySymsetCode;
                    break;
            }
        }

        /// <summary>
        /// Queries apropriate wordnet database according to graph node type.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        public tokenGraphNode queryWithGraph(tokenGraphNode graph, ILogBuilder response, WordnetSource wn_type = WordnetSource.none, WordnetQueryType wn_qtype = WordnetQueryType.none)
        {
            resolve(graph, ref wn_type, ref wn_qtype);

            if (graph.Count() == 0) return graph;

            List<IObjectWithPathAndChildren> leafs = graph.getAllLeafs();
            var tokens = leafs.getNames();

            wordnetSymsetResults res = null;
            switch (wn_type)
            {
                case WordnetSource.english:

                    if (wn_qtype == WordnetQueryType.getSymsetCodesByWord)
                    {
                        res = query_eng(tokens, response);
                        foreach (tokenGraphNode node in leafs)
                        {
                            node.AddValueMatches(res, tokenGraphNodeType.symset_code);
                        }
                        //graph.AddKeyValueChildren(res, tokenGraphNodeType.word_eng, tokenGraphNodeType.symset_code, true);
                    }
                    else
                    {
                        res = query_eng_symset(tokens, response);
                        foreach (tokenGraphNode node in leafs)
                        {
                            node.AddKeyMatches(res, tokenGraphNodeType.word_eng);
                        }
                        //graph.AddKeyValueChildren(res, tokenGraphNodeType.symset_code, tokenGraphNodeType.word_eng);
                    }
                    break;

                case WordnetSource.serbian:
                    if (wn_qtype == WordnetQueryType.getSymsetCodesByWord)
                    {
                        res = query_srb(tokens, response);
                        foreach (tokenGraphNode node in leafs)
                        {
                            node.AddValueMatches(res, tokenGraphNodeType.symset_code);
                        }
                        //graph.AddKeyValueChildren(res, tokenGraphNodeType.word_srb, tokenGraphNodeType.symset_code, true);
                    }
                    else
                    {
                        res = query_srb_symset(tokens, response);
                        foreach (tokenGraphNode node in leafs)
                        {
                            node.AddKeyMatches(res, tokenGraphNodeType.word_srb);
                        }
                        //graph.AddKeyValueChildren(res, tokenGraphNodeType.symset_code, tokenGraphNodeType.word_srb);
                    }
                    break;
            }

            ////var tokens = graph.getChildTokens();

            //switch (childType)
            //{
            //    case tokenGraphNodeType.symset_code:
            //        if (parentType == tokenGraphNodeType.word_srb)
            //        {
            //            var synonimres = query_srb_symset(tokens, response);
            //            graph.AddKeyValueChildren(synonimres, tokenGraphNodeType.symset_code, tokenGraphNodeType.word_srb, true);
            //        } else
            //        {
            //            var synonimres = query_eng_symset(tokens, response);
            //            graph.AddKeyValueChildren(synonimres, tokenGraphNodeType.symset_code, tokenGraphNodeType.word_eng, true);
            //        }
            //        break;
            //    case tokenGraphNodeType.word_eng:
            //    case tokenGraphNodeType.word_query:
            //        var coderes = query_eng(tokens, response);
            //        graph.AddKeyValueChildren(coderes, tokenGraphNodeType.word_eng, tokenGraphNodeType.symset_code, true);
            //        break;
            //    case tokenGraphNodeType.word_srb:
            //        var codesrbres = query_srb(tokens, response);
            //        graph.AddKeyValueChildren(codesrbres, tokenGraphNodeType.word_srb, tokenGraphNodeType.symset_code, true);
            //        break;
            //}

            return graph;
        }

        /// <summary>
        /// Prepares the eng.
        /// </summary>
        /// <param name="response">The response.</param>
        public void prepare_eng(ILogBuilder response)
        {
            String engPath = semanticLexicon.semanticLexiconManager.manager.settings.sourceFiles.getFilePath(semanticLexicon.source.lexiconSourceTypeEnum.englishWordNet);
            wordnet_eng = engPath.deserializeExcelFileToDataSet(wordnet_eng, response);
            wordnet_eng.CaseSensitive = false;
        }

        private DataSet _wordnet_eng;

        /// <summary> </summary>
        public DataSet wordnet_eng
        {
            get
            {
                return _wordnet_eng;
            }
            protected set
            {
                _wordnet_eng = value;
            }
        }

        /// <summary>
        /// da li je spreman engleski wordnet
        /// </summary>
        public Boolean isEngWordNetReady
        {
            get { return (wordnet_eng != null); }
        }

        private FileInfo _resource;

        /// <summary> </summary>
        public FileInfo resource
        {
            get
            {
                return _resource;
            }
            protected set
            {
                _resource = value;
            }
        }

        private static languageManagerWordnet _manager;

        /// <summary>
        /// static and autoinitiated object
        /// </summary>
        public static languageManagerWordnet manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = new languageManagerWordnet();
                }
                return _manager;
            }
        }

        public const String SRB_COLUMN_TOKEN = "token";
        public const String SRB_COLUMN_CODE = "code";
        public const String SRB_COLUMN_MEANING = "meaning";
        public const String SRB_COLUMN_ORIGINAL = "original";

        public void getReady()
        {
            if (!isReady)
            {
                //throw new aceGeneralException("Prepare called twice on languageManagerWordnet", null, this, "Double call");

                resourcesPath = "resources\\sr_wordnet.xlsx";
                table = resourcesPath.deserializeDataTable(dataTableExportEnum.excel, null);

                table.Columns[0].ColumnName = "original";
                table.Columns[1].ColumnName = "code";
                table.Columns[2].ColumnName = "meaning";
                if (!table.Columns.Contains("token"))
                {
                    table.Columns.Add("token");
                    foreach (DataRow sr in table.Rows)
                    {
                        String token = sr["original"].toStringSafe().transliterate();
                        sr["token"] = token;
                    }

                    //resourcesPath.getWritableFile(aceCommonTypes.enums.getWritableFileMode.autoRenameExistingToOld);

                    // table.serializeDataTable(aceCommonTypes.enums.dataTableExportEnum.excel, "sr_wordnet", new DirectoryInfo("resources\\"));
                }

                imbLanguageFrameworkManager.log.log("Serbian WordNet reduced dictionary (sr/en) loaded: " + isReady.ToString());

                if (isReady)
                {
                    imbLanguageFrameworkManager.log.log("> definitions loaded: " + table.Rows.Count + " from source file: [" + resourcesPath + "]");
                }
            }
        }

        public override bool isReady
        {
            get
            {
                return (table != null);
            }
        }

        public override tokenQueryResponse exploreToken(tokenQuery query)
        {
            tokenQueryResponse response = new tokenQueryResponse(query, tokenQuerySourceEnum.ext_dict);

            DataRow[] result = table.Select("token LIKE '" + query.token + "'");

            if (result.Any())
            {
                foreach (DataRow dr in result)
                {
                    wordnetTriplet triplet = new wordnetTriplet(dr.ItemArray);
                    var tqr = new tokenQueryResponse(query, tokenQuerySourceEnum.ext_wordnet);
                    tqr.metadata = triplet;
                    tqr.setResponse(triplet.token, triplet.meaning);
                    query.responses[tokenQuerySourceEnum.ext_wordnet].Add(tqr);
                }
            }

            return response;
        }

        public override void prepare()
        {
        }
    }
}