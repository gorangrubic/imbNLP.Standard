// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tableReplaceResolver.cs" company="imbVeles" >
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
// Project: imbNLP.PartOfSpeech
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace imbNLP.PartOfSpeech.resourceProviders.core
{
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using System.Data;
    using System.Text.RegularExpressions;

    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="imbNLP.PartOfSpeech.resourceProviders.core.tableResourceResolverBase" />
    public class tableReplaceResolver : tableResourceResolverBase
    {
        public const String CSVSPLITTER = ",";

        protected List<tableReplaceResolverItem> items = new List<tableReplaceResolverItem>();

        protected void feedFromTable(ILogBuilder output, Boolean variateWithoutSpace = true, Boolean ignoreCase = true)
        {
            if (sourceTable == null)
            {
                String msg = "Source table not loaded!";

                imbACE.Services.terminal.aceTerminalInput.askYesNo(msg, true, output);
            }

            foreach (DataRow dr in sourceTable.Rows)
            {
                tableReplaceResolverItem newItem = new tableReplaceResolverItem();

                newItem.proper_form = dr[0].toStringSafe();

                if (dr[2].toStringSafe() == "CSV")
                {
                    newItem.useRegex = false;

                    var protoNeedles = dr[1].toStringSafe().SplitSmart(CSVSPLITTER, "", true, true);

                    List<String> protoOther = new List<string>();

                    foreach (String pn in protoNeedles)
                    {
                        if (ignoreCase)
                        {
                            protoOther.AddUnique(pn.ToLower());
                            protoOther.AddUnique(pn.ToUpper());
                        }
                    }

                    var protoList = new List<String>();

                    protoList.AddRange(protoNeedles);
                    protoList.AddRange(protoOther);

                    protoOther = new List<string>();

                    foreach (String pn in protoList)
                    {
                        if (variateWithoutSpace)
                        {
                            protoOther.AddUnique(pn.Replace(" ", ""));
                        }
                    }

                    newItem.needles = new List<string>();

                    newItem.needles.AddRange(protoList);
                    newItem.needles.AddRange(protoOther);
                }
                else
                {
                    newItem.useRegex = true;

                    newItem.needle = new Regex(dr[1].toStringSafe());
                }

                items.Add(newItem);
            }
        }

        /// <summary>
        /// Replaces found needles in the input
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public String process(String input)
        {
            String output = input;
            foreach (tableReplaceResolverItem item in items)
            {
                if (item.useRegex)
                {
                    output = item.needle.Replace(output, item.proper_form);
                }
                else
                {
                    foreach (String needle in item.needles)
                    {
                        output.Replace(needle, item.proper_form);
                    }
                }
            }

            return output;
        }

        public tableReplaceResolver(String resourceFilePath, ILogBuilder output = null) : base(resourceFilePath, output)
        {
            feedFromTable(output);
        }
    }
}