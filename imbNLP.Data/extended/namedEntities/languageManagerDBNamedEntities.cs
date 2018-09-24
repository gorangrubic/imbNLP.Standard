// --------------------------------------------------------------------------------------------------------------------
// <copyright file="languageManagerDBNamedEntities.cs" company="imbVeles" >
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
namespace imbNLP.Data.extended.namedEntities
{
    using imbNLP.Data.enums;
    using imbNLP.Data.enums.flags;
    using imbNLP.Data.extended.dict.core;
    using imbNLP.Data.semanticLexicon.explore;
    using imbNLP.PartOfSpeech.flags.basic;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.text;
    using imbSCI.Data;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.Reporting.enums;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;

    /// <summary>
    /// TODO: Make this universal -- at this moment it is made for Serbian language
    /// </summary>
    /// <seealso cref="imbNLP.Data.extended.dict.core.tokenQueryResolverBase" />
    public class languageManagerDBNamedEntities : tokenQueryResolverBase
    {
        private DataSet _namedEntityDataSet;

        /// <summary>
        ///
        /// </summary>
        public DataSet namedEntityDataSet
        {
            get { return _namedEntityDataSet; }
            set { _namedEntityDataSet = value; }
        }

        private DataTable _positions;

        /// <summary>
        ///
        /// </summary>
        public DataTable positions
        {
            get { return _positions; }
            set { _positions = value; }
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

        private static languageManagerDBNamedEntities _manager;

        /// <summary>
        /// Main DB Named Entities manager
        /// </summary>
        public static languageManagerDBNamedEntities manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = new languageManagerDBNamedEntities();
                }
                return _manager;
            }
        }

        public termExploreItem exploreEntities(String form, termExploreModel exploreModel)
        {
            tokenQuery tq = new tokenQuery(form, null, tokenQuerySourceEnum.imb_namedentities);
            tokenQueryResponse responseEntity = languageManagerDBNamedEntities.manager.exploreToken(tq);

            termExploreItem item = exploreModel.instances[form];

            if (item == null)
            {
                item = new termExploreItem(form);
            }

            if (responseEntity.status == tokenQueryResultEnum.accept)
            {
                exploreModel.lemmaForm = item.inputForm;
                String gset = "";
                Boolean __found = true;
                foreach (namedEntity ctf in responseEntity.flags)
                {
                    Boolean keepSearching = false;
                    __found = true;
                    switch (ctf)
                    {
                        //case namedEntity.businessDomain:
                        //    gset = gset.add("N+Comp", "|"); ;
                        //    exploreModel.wordnetPrimarySymsets.Add("MC04-CON-BD");
                        //    break;
                        case namedEntity.countryName:
                            gset = gset.add("N+Top", "|"); ;
                            exploreModel.wordnetPrimarySymsets.Add("MC04-GEO-C");
                            break;

                        case namedEntity.languageName:
                            gset = gset.add("N+Lang", "|"); ;
                            exploreModel.wordnetPrimarySymsets.Add("MC04-GEO-LN");
                            break;

                        case namedEntity.personalName:
                            gset = gset.add("N+First", "|"); ;
                            exploreModel.wordnetPrimarySymsets.Add("MC04-HUM-PN");
                            break;

                        case namedEntity.personalPosition:
                            gset = gset.add("N+Hum", "|"); ;
                            exploreModel.wordnetPrimarySymsets.Add("MC04-HUM-PP");
                            break;

                        case namedEntity.presonalLastName:
                            gset = gset.add("N+Last", "|"); ;
                            exploreModel.wordnetPrimarySymsets.Add("MC04-HUM-LN");
                            break;

                        case namedEntity.title:
                            gset = gset.add("N", "|"); ;
                            exploreModel.wordnetPrimarySymsets.Add("MC04-CON-AD-TITLE");
                            break;

                        case namedEntity.townName:
                            gset = gset.add("N+PGr1", "|");
                            exploreModel.wordnetPrimarySymsets.Add("MC04-CON-AD-TN");
                            break;

                        case namedEntity.townZip:
                            gset = gset.add("NUMnumerical+Top", "|"); ;
                            exploreModel.wordnetPrimarySymsets.Add("MC04-CON-AD-TZ");
                            break;

                        default:
                            keepSearching = true;
                            __found = false;
                            break;
                    }
                    if (!keepSearching) break;
                }

                item.gramSet.SetAll(gset);

                if (__found)
                {
                    exploreModel.instances.Add(item);

                    if (exploreModel.lemma == null) exploreModel.lemma = item;

                    exploreModel.synonyms.AddRange(responseEntity.dataTokens.getTokens());

                    exploreModel.flags = termExploreItemEnumFlag.namedEntity;
                }

                return item;
            }

            return item;
        }

        public const String TABLE_TOWNS = "towns";
        public const String TABLE_PNAMES = "personal_names";
        public const String TABLE_LNAMES = "personal_lastnames";
        public const String TABLE_LANGUAGES_ISO = "languages_iso";
        public const String TABLE_COUNTRIES = "country_iso";
        public const String TABLE_POSITIONS = "positions";
        public const String TABLE_INDUSTRIES = "industries";
        public const String TABLE_PNAMES_GCASES = "personal_names_bycase";

        public void getReady()
        {
            if (!isReady)
            {
                imbLanguageFrameworkManager.log.log("Named entities dataset [town/zip, first names, last names, professional positions] initialization: ");
                namedEntityDataSet = new DataSet("imb_named_entities");

                DirectoryInfo dir = new DirectoryInfo("resources\\entities\\");
                dir.deserializeFolderExcelFilesToDataSet(namedEntityDataSet, imbLanguageFrameworkManager.log, "*.xlsx");

                foreach (DataTable dt in namedEntityDataSet.Tables)
                {
                    if (dt.TableName.StartsWith(TABLE_TOWNS)) //"naselja"
                    {
                        dt.ExtendedProperties[tokenResultEnum.unitexWordType] = pos_type.N; // tosWordType.Noun;
                        dt.ExtendedProperties[tokenResultEnum.dictionaryType] = namedEntity.townName;

                        dt.ExtendedProperties[tokenResultEnum.tokenFlag] = contentTokenFlag.cityNameKnown;
                        dt.ExtendedProperties[tokenResultEnum.query] = makeQueryForDataTable(dt, dataTableStringQueryEnum.exact);
                    }
                    else if (dt.TableName.StartsWith(TABLE_PNAMES)) //"imena"
                    {
                        dt.ExtendedProperties[tokenResultEnum.unitexWordType] = pos_type.N;
                        dt.ExtendedProperties[tokenResultEnum.dictionaryType] = namedEntity.personalName;

                        dt.ExtendedProperties[tokenResultEnum.tokenFlag] = contentTokenFlag.personalNameKnown;
                        dt.ExtendedProperties[tokenResultEnum.query] = makeQueryForDataTable(dt);
                    }
                    else if (dt.TableName.StartsWith(TABLE_LNAMES)) //"prezimena"
                    {
                        dt.ExtendedProperties[tokenResultEnum.unitexWordType] = pos_type.N;
                        dt.ExtendedProperties[tokenResultEnum.dictionaryType] = namedEntity.presonalLastName;

                        dt.ExtendedProperties[tokenResultEnum.tokenFlag] = contentTokenFlag.personalLastnameKnown;
                        dt.ExtendedProperties[tokenResultEnum.query] = makeQueryForDataTable(dt);
                    }
                    else if (dt.TableName.StartsWith(TABLE_LANGUAGES_ISO)) // "languages_iso"
                    {
                        dt.ExtendedProperties[tokenResultEnum.unitexWordType] = pos_type.N;
                        dt.ExtendedProperties[tokenResultEnum.dictionaryType] = namedEntity.languageName;

                        dt.ExtendedProperties[tokenResultEnum.tokenFlag] = contentTokenFlag.languageId;
                        dt.ExtendedProperties[tokenResultEnum.query] = makeQueryForDataTable(dt);
                    }
                    else if (dt.TableName.StartsWith(TABLE_COUNTRIES)) //"serbian_country"
                    {
                        dt.ExtendedProperties[tokenResultEnum.unitexWordType] = pos_type.N;
                        dt.ExtendedProperties[tokenResultEnum.dictionaryType] = namedEntity.countryName;

                        dt.ExtendedProperties[tokenResultEnum.tokenFlag] = contentTokenFlag.countryId;
                        dt.ExtendedProperties[tokenResultEnum.query] = makeQueryForDataTable(dt);
                    }
                    else if (dt.TableName.StartsWith(TABLE_POSITIONS)) // "positions"
                    {
                        dt.ExtendedProperties[tokenResultEnum.unitexWordType] = pos_type.N;
                        dt.ExtendedProperties[tokenResultEnum.dictionaryType] = namedEntity.personalPosition;

                        dt.ExtendedProperties[tokenResultEnum.tokenFlag] = contentTokenFlag.personalTitle;
                        dt.ExtendedProperties[tokenResultEnum.searchFromColumnNumber] = 1;
                        dt.ExtendedProperties[tokenResultEnum.searchUntilColumnNumber] = 5;
                        dt.ExtendedProperties[tokenResultEnum.query] = makeQueryForDataTable(dt, dataTableStringQueryEnum.starts);
                    }
                    else if (dt.TableName.StartsWith(TABLE_INDUSTRIES)) //"delatnosti"
                    {
                        dt.ExtendedProperties[tokenResultEnum.unitexWordType] = pos_type.N;
                        dt.ExtendedProperties[tokenResultEnum.dictionaryType] = namedEntity.businessDomain;

                        dt.ExtendedProperties[tokenResultEnum.tokenFlag] = contentTokenFlag.companyCategory;
                        dt.ExtendedProperties[tokenResultEnum.searchFromColumnNumber] = 3;
                        dt.ExtendedProperties[tokenResultEnum.searchUntilColumnNumber] = 4;
                        dt.ExtendedProperties[tokenResultEnum.query] = makeQueryForDataTable(dt, dataTableStringQueryEnum.substring);
                    }
                    else if (dt.TableName.StartsWith(TABLE_PNAMES_GCASES)) //"padezi_imena"
                    {
                        dt.ExtendedProperties[tokenResultEnum.unitexWordType] = pos_type.N;
                        dt.ExtendedProperties[tokenResultEnum.dictionaryType] = namedEntity.personalName;

                        dt.ExtendedProperties[tokenResultEnum.tokenFlag] = contentTokenFlag.personalNameKnown;
                        dt.ExtendedProperties[tokenResultEnum.searchFromColumnNumber] = 2;
                        dt.ExtendedProperties[tokenResultEnum.searchUntilColumnNumber] = 3;
                        dt.ExtendedProperties[tokenResultEnum.query] = makeQueryForDataTable(dt);
                    }
                }

                total_count = namedEntityDataSet.GetTotalRowsCount();

                imbLanguageFrameworkManager.log.log("Total named-entities defined: " + total_count);
            }
        }

        public override bool isReady
        {
            get
            {
                if (namedEntityDataSet != null)
                {
                    if (namedEntityDataSet.Tables.Count > 0)
                    {
                        //  throw new aceGeneralException("The prepare call is already executed", null, this, "Doble call to prepare!");
                        return true;
                    }
                }
                return false;
            }
        }

        private Int32 _total_count;

        /// <summary> </summary>
        public Int32 total_count
        {
            get
            {
                return _total_count;
            }
            protected set
            {
                _total_count = value;
            }
        }

        /// <summary>
        /// Explores the token.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public override tokenQueryResponse exploreToken(tokenQuery query)
        {
            getReady();

            tokenQueryResponse response = new tokenQueryResponse(query, tokenQuerySourceEnum.ext_dict);
            List<DataRow> matches = new List<DataRow>();
            String expression = "";

            foreach (DataTable dt in namedEntityDataSet.Tables)
            {
                expression = "";
                if (query.focus == contentTokenFlag.none)
                {
                    expression = dt.ExtendedProperties.getProperString(tokenResultEnum.query).Replace("".t(tokenResultEnum.needle), query.token);
                }
                else
                {
                    contentTokenFlag table_focus = (contentTokenFlag)dt.ExtendedProperties[tokenResultEnum.tokenFlag]; //.getProperEnum<contentTokenFlag>(contentTokenFlag.namedEntityKnown, );
                    if (table_focus.HasFlag(query.focus))
                    {
                        expression = dt.ExtendedProperties.getProperString(tokenResultEnum.query).Replace("".t(tokenResultEnum.needle), query.token);
                    }
                }

                if (expression != "")
                {
                    matches.AddRange(dt.Select(expression));
                }
            }

            foreach (DataRow dr in matches)
            {
                var fl = (contentTokenFlag)dr.Table.ExtendedProperties[tokenResultEnum.tokenFlag];
                response.flags.AddInstanceRange(fl.getEnumListFromFlags<contentTokenFlag>());
                response.dataRows.Add(dr);
                foreach (DataColumn dc in dr.Table.Columns)
                {
                    response.dataTokens.AddRange(dr[dc].toStringSafe().getTokens());
                }
            }

            if (response.flags.Count > 0)
            {
                response.setResponse("Known entity", "Flags earned: " + response.flags.toCsvInLine(";"));
            }
            else
            {
                response.setResponse(tokenQueryResultEnum.unknown_token);
            }

            return response;
        }

        /// <summary>
        /// Makes the query for data table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="queryType">Type of the query.</param>
        /// <returns></returns>
        protected String makeQueryForDataTable(DataTable table, dataTableStringQueryEnum queryType = dataTableStringQueryEnum.exact)
        {
            var columns = new List<DataColumn>();
            var output = "";
            table.CaseSensitive = false;

            if (table.ExtendedProperties.ContainsKey(tokenResultEnum.searchFromColumnNumber))
            {
                Int32 from = table.ExtendedProperties.getProperInt32(0, tokenResultEnum.searchFromColumnNumber);
                Int32 to = table.ExtendedProperties.getProperInt32(from + 1, tokenResultEnum.searchUntilColumnNumber);
                to = Math.Min(to, table.Columns.Count);
                from = Math.Min(from, table.Columns.Count);

                for (int i = from; i < to; i++)
                {
                    switch (queryType)
                    {
                        case dataTableStringQueryEnum.ends:
                            output = output.add(table.Columns[i].ColumnName + " LIKE '*".t(tokenResultEnum.needle) + "' ", " OR ");
                            break;

                        case dataTableStringQueryEnum.exact:
                            output = output.add(table.Columns[i].ColumnName + " = '".t(tokenResultEnum.needle) + "' ", " OR ");
                            break;

                        case dataTableStringQueryEnum.starts:
                            output = output.add(table.Columns[i].ColumnName + " LIKE '".t(tokenResultEnum.needle) + "*' ", " OR ");
                            break;

                        case dataTableStringQueryEnum.substring:
                            output = output.add(table.Columns[i].ColumnName + " LIKE '*".t(tokenResultEnum.needle) + "*' ", " OR ");
                            break;
                    }
                }
            }
            else
            {
                foreach (DataColumn column in table.Columns)
                {
                    switch (queryType)
                    {
                        case dataTableStringQueryEnum.ends:
                            output = output.add(column.ColumnName + " LIKE '*".t(tokenResultEnum.needle) + "' ", " OR ");
                            break;

                        case dataTableStringQueryEnum.exact:
                            output = output.add(column.ColumnName + " = '".t(tokenResultEnum.needle) + "' ", " OR ");
                            break;

                        case dataTableStringQueryEnum.starts:
                            output = output.add(column.ColumnName + " LIKE '".t(tokenResultEnum.needle) + "*' ", " OR ");
                            break;

                        case dataTableStringQueryEnum.substring:
                            output = output.add(column.ColumnName + " LIKE '*".t(tokenResultEnum.needle) + "*' ", " OR ");
                            break;
                    }
                }
            }
            return output;
        }

        public override void prepare()
        {
            if (namedEntityDataSet != null)
            {
                if (namedEntityDataSet.Tables.Count > 0)
                {
                    //  throw new aceGeneralException("The prepare call is already executed", null, this, "Double call to prepare!");
                    return;
                }
            }
        }
    }
}