// --------------------------------------------------------------------------------------------------------------------
// <copyright file="languageManagerConcepts.cs" company="imbVeles" >
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
namespace imbNLP.Data.extended.domain
{
    using imbNLP.Data.extended.dict.core;
    using imbNLP.Data.semanticLexicon;
    using imbSCI.Core.extensions.text;
    using imbSCI.DataComplex.extensions.data.formats;
    using System;
    using System.Data;

    public class languageManagerConcepts : tokenQueryResolverBase
    {
        public const String COLUMN_PATH = "Path";
        public const String COLUMN_DESCNAME = "Description";
        public const String COLUMN_NEEDLES = "Needles";

        private static languageManagerConcepts _manager;

        /// <summary>
        /// Default manager
        /// </summary>
        public static languageManagerConcepts manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = new languageManagerConcepts();
                }
                return _manager;
            }
        }

        public void getReady()
        {
            if (!isReady)
            {
                imbLanguageFrameworkManager.log.log("Loading domain concepts");

                String sp = semanticLexiconManager.manager.settings.sourceFiles.getFilePath(semanticLexicon.source.lexiconSourceTypeEnum.domainConcepts);
                var format = sp.getExportFormatByExtension();
                table = sp.deserializeDataTable(format);
                table.Columns[0].ColumnName = COLUMN_PATH;
                table.Columns[1].ColumnName = COLUMN_DESCNAME;
                table.Columns[2].ColumnName = COLUMN_NEEDLES;
            }
        }

        public domainConceptGraph getConceptGraph()
        {
            getReady();
            domainConceptGraph output = new domainConceptGraph("root");
            var dt = table.Select(COLUMN_PATH + " <> ''");
            foreach (DataRow dr in dt)
            {
                if (dr[COLUMN_PATH].ToString() != "Name")
                {
                    output.Add(dr[COLUMN_PATH].toStringSafe(), new domainConceptEntry(dr[COLUMN_PATH].toStringSafe(), dr[COLUMN_DESCNAME].toStringSafe(), dr[COLUMN_NEEDLES].toStringSafe()));
                }
            }
            return output;
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
            throw new NotImplementedException();
        }

        public override void prepare()
        {
        }
    }
}