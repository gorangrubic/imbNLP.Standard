// --------------------------------------------------------------------------------------------------------------------
// <copyright file="languageManagerDict.cs" company="imbVeles" >
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
namespace imbNLP.Data.extended.dict
{
    using imbNLP.Data.extended.dict.core;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.extensions.text;
    using imbSCI.Data;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class languageManagerDict : tokenQueryResolverBase
    {
        private Regex _splitRegex = new Regex("([,\"])+");

        /// <summary> </summary>
        public Regex splitRegex
        {
            get
            {
                return _splitRegex;
            }
            protected set
            {
                _splitRegex = value;
            }
        }

        private Regex _regexForLine = new Regex("[En,]*([\\w\\s.,]+)[\\d]?");

        /// <summary> </summary>
        public Regex regexForLine
        {
            get
            {
                return _regexForLine;
            }
            protected set
            {
                _regexForLine = value;
            }
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

        private static languageManagerDict _manager;

        /// <summary>
        /// static and autoinitiated object
        /// </summary>
        public static languageManagerDict manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = new languageManagerDict();
                }
                return _manager;
            }
        }

        public void getReady()
        {
            if (!isReady)
            {
                resourcesPath = "resources\\recnik.csv";
                List<String> lines = openBase.openFileFilterLines(resourcesPath);

                table = new System.Data.DataTable("Dict");
                var lang_cn = table.Columns.Add("Language");
                var token_cn = table.Columns.Add("Token");
                var meaning_cn = table.Columns.Add("Meaning");
                var code_cn = table.Columns.Add("Code");

                Int32 c = 0;
                foreach (String line in lines)
                {
                    if (!line.isNullOrEmpty())
                    {
                        DataRow dr = table.NewRow();
                        String[] parts = splitRegex.Split(line);
                        if (parts.Count() > 2)
                        {
                            if (parts[0].StartsWith("E", StringComparison.CurrentCulture))
                            {
                                dr[meaning_cn] = parts[1].toStringSafe();
                                dr[token_cn] = parts[2].TrimEnd("1234567890".ToArray());
                                dr[code_cn] = parts[2].removeStartsWith(dr[token_cn].toStringSafe());
                            }
                            else
                            {
                                dr[token_cn] = parts[1].toStringSafe();
                                dr[meaning_cn] = parts[2].TrimEnd("1234567890".ToArray());
                                dr[code_cn] = parts[2].removeStartsWith(dr[meaning_cn].toStringSafe());
                            }
                            table.Rows.Add(dr);
                        }
                        else
                        {
                            imbLanguageFrameworkManager.log.log("> dictionary line [" + c.ToString() + "] format error [" + line + "] splitted to parts:[" + parts.Count() + "]");
                        }
                        c++;
                    }
                }

                imbLanguageFrameworkManager.log.log("Aluxuary dictionary (sr/en) loaded: " + isReady.ToString());

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
            getReady();

            var response = new tokenQueryResponse(query, tokenQuerySourceEnum.ext_dict);

            DataRow[] result = table.Select("Token LIKE '" + query.token + "'");

            if (result.Any())
            {
                foreach (DataRow dr in result)
                {
                    dictTriplet triplet = new dictTriplet(__token: dr["token"].toStringSafe(), __meaning: dr["Meaning"].toStringSafe(), __code: dr["Code"].toStringSafe());
                    var tqr = new tokenQueryResponse(query, tokenQuerySourceEnum.ext_dict)
                    {
                        metadata = triplet
                    };
                    tqr.setResponse(triplet.token, triplet.meaning);
                    query.responses[tokenQuerySourceEnum.ext_wordnet].Add(tqr);
                }
            }

            return response;
        }

        public override void prepare()
        {
            //openBase.
        }
    }
}