// --------------------------------------------------------------------------------------------------------------------
// <copyright file="resourceConverterForGramaticTags.cs" company="imbVeles" >
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
namespace imbNLP.PartOfSpeech.resourceProviders.core
{
    using imbACE.Core.core;
    using imbNLP.PartOfSpeech.flags.basic;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.enumworks;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.extensions.typeworks;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.enums.reporting;
    using imbSCI.Data.enums.tableReporting;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.special;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Resource conversion table set contains conversion specification for a lexical resource entry interpretation
    /// </summary>
    /// <remarks>
    /// <para>The specification is declared in a Excel file, that follows particular structure.</para>
    /// </remarks>
    [Serializable]
    public class resourceConverterForGramaticTags
    {
        /// <summary>
        /// The tablename translation
        /// </summary>
        public const string TABLENAME_TRANSLATION = "translation";

        /// <summary>
        /// The tablename format
        /// </summary>
        public const string TABLENAME_FORMAT = "format";

        /// <summary>
        /// The tablecolumn membername
        /// </summary>
        public const int TABLECOLUMN_MEMBERNAME = 1;

        /// <summary>
        /// The tablecolumn format
        /// </summary>
        public const int TABLECOLUMN_FORMAT = 0;

        /// <summary>
        /// The tablecolumn code
        /// </summary>
        public const int TABLECOLUMN_CODE = 0;

        private Dictionary<string, Type> _pos_enum_types;

        /// <summary>
        /// Locally used dictionary of pos enums
        /// </summary>
        /// <value>
        /// The position enum types.
        /// </value>
        protected Dictionary<string, Type> pos_enum_types
        {
            get
            {
                if (_pos_enum_types == null)
                {
                    prepareEnumTypes();
                }
                return _pos_enum_types;
            }
        }

        private void prepareEnumTypes()
        {
            // <--------- scanning for pos enum types
            var t = typeof(pos_type);

            _pos_enum_types = t.CollectTypes(CollectTypeFlags.includeEnumTypes | CollectTypeFlags.ofSameNamespace | CollectTypeFlags.ofThisAssembly);

            //var a = t.Assembly;
            //var pos_enums = a.GetTypes().Where<Type>(x => x.Namespace == t.Namespace);
            //_pos_enum_types = new Dictionary<string, Type>();
            //foreach (Type pet in pos_enums)
            //{
            //    if (pet.IsEnum)
            //    {
            //        _pos_enum_types.Add(pet.Name, pet);
            //    }
            //}
        }

        private Type interpretMemberName(string memberName, out object memberValue, ILogBuilder logger = null)
        {
            List<string> memberNameParts = memberName.SplitSmart(".", "", true, true);

            string typeName = memberNameParts[0];
            string enumMemberName = memberNameParts[1];
            Type memberType = null;
            if (memberNameParts.Count > 1)
            {
                if (!pos_enum_types.ContainsKey(typeName))
                {
                    String msg = "There is no [" + typeName + "] type found for [" + memberName + "] in " + nameof(resourceConverterForGramaticTags);

                    msg.addLine("Please check the conversion specification file");

                    if (logger == null)
                    {
                        imbACE.Services.terminal.aceTerminalInput.askPressAnyKeyInTime(msg, false, 10, true, 3);
                    }
                    else
                    {
                        logger.log("Type [" + typeName + "] not found for [" + memberName + "]");
                    }
                    memberType = null;
                }
                else
                {
                    memberType = pos_enum_types[typeName];
                }

                if (memberType != null)
                {
                    memberValue = memberType.getEnumByName(enumMemberName);
                }
                else
                {
                    memberValue = null;
                }
                if (memberValue == null)
                {
                    throw new ArgumentOutOfRangeException("Member name [" + memberName + "] not found in type [" + memberType.Name + "]", nameof(memberValue));
                }

                return memberType;
            }
            memberValue = null;
            return null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="resourceConverterForGramaticTags"/> class.
        /// </summary>
        public resourceConverterForGramaticTags()
        {
            prepareEnumTypes();
        }

        /// <summary>
        /// Loads the specification from two CSV files
        /// </summary>
        /// <param name="formatCSVPath">The format CSV path.</param>
        /// <param name="translationCSVPath">The translation CSV path.</param>
        /// <param name="output">The output.</param>
        public void LoadSpecificationCSV(string formatCSVPath, string translationCSVPath, ILogBuilder output)
        {
            DataTable transTable = translationCSVPath.deserializeDataTable(dataTableExportEnum.csv);
            DataTable formatTable = formatCSVPath.deserializeDataTable(dataTableExportEnum.csv);

            LoadSpecification(transTable, formatTable);
        }

        /// <summary>
        /// Loads the specification excel file from the specified path, and prints out the log. [Not working]
        /// </summary>
        /// <param name="grammSpecPath">The gramm spec path.</param>
        /// <param name="output">The output.</param>
        public void LoadSpecificationExcelFile(string grammSpecPath, ILogBuilder output)
        {
            DataSet ds = new DataSet("specifications");
            grammSpecPath.deserializeExcelFileToDataSet(ds, output, dataTableIOFlags.noRowWithColumnNames);

            bool ok = true;

            if (ds.Tables.Count != 2)
            {
                if (output != null) output.log("There are no two tables in the Excel file!");
                ok = false;
            }

            if (ok == false)
            {
                if (output != null) output.log("Specifications loading _aborted_");
                return;
            }

            DataTable transTable = ds.Tables[0];
            DataTable formatTable = ds.Tables[1];

            LoadSpecification(transTable, formatTable, output);

            if (output != null) output.log("Specification loaded :: tag translation entries [" + posEnumVsString.Count + "] -- for categories [" + posTypeVsPattern.Count + "]");
        }

        /// <summary>
        /// Initialize converter with specification from <see cref="DataTable" />s
        /// </summary>
        /// <param name="transTable">The translation table - must have columns: <see cref="TABLECOLUMN_MEMBERNAME" /> and <see cref="TABLECOLUMN_CODE" />.</param>
        /// <param name="formatTable">The format table - must have columns: <see cref="TABLECOLUMN_MEMBERNAME" /> and <see cref="TABLECOLUMN_FORMAT" />.</param>
        /// <param name="logger">The logger.</param>
        public void LoadSpecification(DataTable transTable, DataTable formatTable, ILogBuilder logger = null)
        {
            // <--------- interpretation of the translation table
            foreach (DataRow row in transTable.Rows)
            {
                string code = row[TABLECOLUMN_CODE].toStringSafe();
                string memberName = row[TABLECOLUMN_MEMBERNAME].toStringSafe();
                object memberValue = null;
                Type memberType = interpretMemberName(memberName, out memberValue);

                if (memberValue != null)
                {
                    posEnumVsString.Add(memberValue, code);
                }
            }
            int c = 0;
            // <---------- interpretation of format table
            foreach (DataRow row in formatTable.Rows)
            {
                c++;
                string format_string = row[TABLECOLUMN_FORMAT].toStringSafe();
                string memberName = row[TABLECOLUMN_MEMBERNAME].toStringSafe();

                object memberValue = null;
                Type memberType = interpretMemberName(memberName, out memberValue);

                List<string> typeNames = format_string.SplitSmart(",", "", true, true);
                List<Type> typeList = new List<Type>();
                foreach (string tn in typeNames)
                {
                    Type t = pos_enum_types[tn];
                    typeList.Add(t);
                }

                pos_type pt = (pos_type)memberValue;
                posTypeVsPattern[pt] = typeList;
            }
        }

        /// <summary>
        /// Ordinal format definition per <see cref="pos_type"/>
        /// </summary>
        /// <value>
        /// The position of enum types in string pattern of the format
        /// </value>
        protected translationEnumPatternTable<pos_type> posTypeVsPattern { get; set; } = new translationEnumPatternTable<pos_type>();

        /// <summary>
        /// The POS Enum value vs string table, used for translation
        /// </summary>
        /// <value>
        /// The position enum vs string.
        /// </value>
        protected translationTableMulti<object, string> posEnumVsString { get; set; } = new translationTableMulti<object, string>();

        /// <summary>
        /// Gets the string version for the posEnumValue specified. For <c>none</c> returns empty string
        /// </summary>
        /// <param name="posEnumValue">The POS Enum value to convert</param>
        /// <param name="enforceSupport">if set to <c>true</c> it will enforce support by calling <see cref="Object.ToString()"/></param>
        /// <returns>String representation</returns>
        /// <exception cref="ArgumentOutOfRangeException">Not supported by this specification - posEnumValue</exception>
        public string GetStringFor(object posEnumValue, bool enforceSupport = false)
        {
            var vls = posEnumVsString.GetByKey(posEnumValue);
            var vl = vls.FirstOrDefault();

            if (vl == "none")
            {
                if (enforceSupport) return posEnumValue.toStringSafe("-");
                return "-";
            }
            return vl;
        }

        protected ConcurrentDictionary<String, List<Object>> cachedTags { get; set; } = new ConcurrentDictionary<string, List<object>>();

        /// <summary>
        /// Builds <see cref="grammaticTagCollection"/> instance from string form
        /// </summary>
        /// <param name="tag">The string encoding that is to be interpreted into grammatic tag</param>
        /// <returns>Instance of grammaticTagCollection built from the string input</returns>
        /// <exception cref="ArgumentOutOfRangeException">Tag flag [" + s+ "] not resolved in [" +t.Name + "] - tag</exception>
        public grammaticTagCollection ConvertFromString(string tag)
        {
            grammaticTagCollection output = new grammaticTagCollection();
            if (cachedTags.ContainsKey(tag))
            {
                cachedTags[tag].ForEach(x => output.Add(x));
                return output;
            }

            if (tag.isNullOrEmpty()) return output;

            string pos_type_str = tag[0].ToString();

            object pt = posEnumVsString.GetOfTypeByValue(pos_type_str, typeof(pos_type));
            List<Object> flags = new List<object>();
            try
            {
                if (pt != null)
                {
                    pos_type pos_t = (pos_type)pt; //.GetByValue(pos_type_str);
                    flags.Add(pos_t);

                    List<Type> typeList = posTypeVsPattern[pos_t];
                    for (int i = 1; i < tag.Length; i++)
                    {
                        string s = tag[i].toStringSafe("");

                        if (s != "-")
                        {
                            Type t = typeList[i - 1];

                            object f = posEnumVsString.GetOfTypeByValue(s, t);

                            if (f == null)
                            {
                                List<Object> values = posEnumVsString.GetByValue(s);
                                foreach (object vl in values)
                                {
                                    if (!flags.Any(x => x.GetType() == vl.GetType()))
                                    {
                                        flags.Add(vl);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                flags.Add(f);
                            }

                            //if (f == null)
                            //{
                            //    throw new ArgumentOutOfRangeException("Tag flag [" + s + "] not resolved in [" + t.Name + "]", nameof(tag));
                            //}
                        }
                    }

                    flags.ForEach(x => output.Add(x));
                    cachedTags.TryAdd(tag, flags);
                }
            }
            catch (Exception ex)
            {
                String msg = "[" + ex.Message + "] ---> [" + ex.GetType().Name + "] ";
                output.comment = msg;
                aceLog.log(msg);
            }

            return output;
        }

        /// <summary>
        /// Converts the specified grammatic tag collection into string format according to loaded specification
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public string ConvertToString(grammaticTagCollection tag)
        {
            var sb = new StringBuilder();
            var tags = tag.GetTags();

            var pos_t = tags.getFirstOfType<pos_type>(false, pos_type.none, true);

            if (pos_t == pos_type.none)
            {
                sb.Append("-");
            }
            else
            {
                var typePattern = posTypeVsPattern[pos_t];

                sb.Append(posEnumVsString.GetOfTypeByKey(pos_t));

                foreach (Type t in typePattern)
                {
                    var pos = tag.Get(t);
                    if (pos == null)
                    {
                        sb.Append("-");
                    }
                    else
                    {
                        String p = posEnumVsString.GetOfTypeByKey(pos);
                        if (p.isNullOrEmpty())
                        {
                            sb.Append("-");
                        }
                        else
                        {
                            sb.Append(p);
                        }
                    }
                }
            }
            return sb.ToString();
        }
    }
}