// --------------------------------------------------------------------------------------------------------------------
// <copyright file="gramFlags.cs" company="imbVeles" >
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
namespace imbNLP.Data.semanticLexicon.posCase
{
    using BrightstarDB.EntityFramework;
    using imbACE.Core.core.exceptions;
    using imbNLP.PartOfSpeech.flags.basic;
    using imbSCI.Data;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;

    /// <summary>
    /// Jedan gramatički unos iz UNITEX-a
    /// </summary>
    public class gramFlags : IEnumerable<object>
    {
        public gramFlags()
        {
        }

        public gramFlags(string declaration)
        {
            SetAll(declaration);
        }

        public gramFlags(IEnumerable<Enum> flags)
        {
            Set(flags);
        }

        /// <summary>
        ///
        /// </summary>
        public string stringForm { get; set; } = "";

        private pos_type _type = pos_type.none;

        /// <summary>
        /// POS term type
        /// </summary>
        [XmlIgnore]
        public pos_type type
        {
            get
            {
                if (_type == pos_type.none)
                {
                    reinit();
                }
                return _type;
            }
            set { _type = value; }
        }

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore]
        public List<string> markers { get; set; } = new List<string>();

        private Dictionary<Type, object> _items = new Dictionary<Type, object>();

        /// <summary>
        ///
        /// </summary>
        protected Dictionary<Type, object> items
        {
            get
            {
                if (!_items.Any())
                {
                    reinit();
                }
                return _items;
            }
            set { _items = value; }
        }

        private void checkItemsForType(Type valType)
        {
            if (!items.ContainsKey(valType))
            {
                items.Add(valType, valType.GetDefaultValue());
            }
        }

        private void reinit()
        {
            if (!stringForm.isNullOrEmpty())
            {
                SetAll(stringForm);
            }
        }

        public string Get(Type flagType)
        {
            checkItemsForType(flagType);

            return posConverter.getString(items[flagType] as Enum);
        }

        public void Set(string stringForm, Type flagType)
        {
            checkItemsForType(flagType);
            items[flagType] = posConverter.getFlag(flagType, stringForm);
        }

        public T Get<T>() where T : IConvertible
        {
            checkItemsForType(typeof(T));
            return (T)items[typeof(T)];
        }

        public void Set(Enum value)
        {
            checkItemsForType(value.GetType());
            items[value.GetType()] = value;
        }

        public void Set(IEnumerable<Enum> values)
        {
            foreach (Enum e in values)
            {
                Type t = e.GetType();
                if (t == typeof(pos_type)) type = (pos_type)e;
                checkItemsForType(t);
                items[t] = e;
            }
        }

        public string ToString(bool inline = false)
        {
            if (inline)
            {
                return posConverter.posFlagsTranslator.getTranslation(type) + ":" + GetAll();
            }
            StringBuilder sb = new StringBuilder();
            // sb.AppendLine("Declaration: " + GetAll());
            sb.AppendLine(posConverter.posFlagsTranslator.getTranslation(type));
            var ts = posConverter.posTypeVsPattern[type];
            foreach (Type t in ts)
            {
                checkItemsForType(t);
                sb.AppendLine(posConverter.posFlagsTranslator.getTranslation(items[t]));
            }
            sb.AppendLine("Markers: " + String.Join(",", markers));

            return sb.ToString();
        }

        /// <summary>
        /// Returns the complete gram case declaration
        /// </summary>
        /// <param name="parts">The parts.</param>
        /// <returns></returns>
        public string GetAll(gramFlagDeclarationParts parts = gramFlagDeclarationParts.all)
        {
            StringBuilder sb = new StringBuilder();

            if (parts.HasFlag(gramFlagDeclarationParts.posType))
            {
                sb.Append(posConverter.getString(type));
            }

            if (parts.HasFlag(gramFlagDeclarationParts.posMarkers))
            {
                foreach (string marker in markers)
                {
                    sb.Append("+");
                    sb.Append(marker.Trim('+'));
                }

                sb.Append(":");
            }
            if (parts.HasFlag(gramFlagDeclarationParts.posGramFlags))
            {
                foreach (Type t in posConverter.posTypeVsPattern[type])
                {
                    checkItemsForType(t);
                    sb.Append(posConverter.getString(items[t] as Enum));
                }
            }

            stringForm = sb.ToString();

            return sb.ToString();
        }

        /// <summary>
        /// Sets: <see cref="pos_type"/>, and other pos flags contained in the specified declaration
        /// </summary>
        /// <param name="declaration">The declaration in Unitex format -- the part of DELAF entry without word and lemma, after dot: N:mp2q</param>
        /// <param name="parts">The parts.</param>
        /// <exception cref="aceGeneralException">
        /// POS type definition not found in the input declaration [" + declaration + "] - null - POS Type failed in SetAll()
        /// or
        /// POS type set to none by the input declaration [" + declaration + "] - null - POS Type failed in SetAll()
        /// or
        /// POS gram flags not found in the input declaration [" + declaration + "] - null - POS gram flags failed in SetAll()
        /// </exception>
        public void SetAll(string declaration, gramFlagDeclarationParts parts = gramFlagDeclarationParts.all)
        {
            declaration = declaration.Trim();
            if (declaration == ":") return;

            MatchCollection mcl = null; // posConverter.REGEX_UNITEX_Declaration.Matches(declaration);

            if (parts.HasFlag(gramFlagDeclarationParts.posType))
            {
                Match mc = posConverter.REGEX_UNITEX_DecPosType.Match(declaration);

                if (mc.Success)
                {
                    string g_dec = mc.Value;

                    type = (pos_type)posConverter.getFlag(typeof(pos_type), g_dec);
                }
                else
                {
                    throw new aceGeneralException("POS type definition not found in the input declaration [" + declaration + "]", null, this, "POS Type failed in SetAll()");
                }

                if (type == pos_type.none)
                {
                    throw new aceGeneralException("POS type set to none by the input declaration [" + declaration + "]", null, this, "POS Type failed in SetAll()");
                }
            }

            if (parts.HasFlag(gramFlagDeclarationParts.posMarkers))
            {
                if (posConverter.REGEX_UNITEX_MarkersSelection.IsMatch(declaration))
                {
                    string m_dec = posConverter.REGEX_UNITEX_MarkersSelection.Match(declaration).Value;
                    List<string> marks = m_dec.SplitSmart("+");
                    markers.AddRange(marks);
                }
            }

            if (parts.HasFlag(gramFlagDeclarationParts.posGramFlags))
            {
                var ts = posConverter.posTypeVsPattern[type];

                Match mc = posConverter.REGEX_UNITEX_DecGrams.Match(declaration);

                if (mc.Success)
                {
                    string g_dec = mc.Value;
                    //var ts = posConverter.posTypeVsPattern[type];
                    var fls = posConverter.posTypeVsString.GetEnums(ts, g_dec);

                    if (!fls.Any())
                    {
                        if (ts.Count > 0)
                        {
                            throw new aceGeneralException("POS gram flags interpretation failed, no flags found in the input declaration [" + declaration + "]", null, this, "POS gram flags failed in SetAll()");
                        }
                    }

                    foreach (var fl in fls)
                    {
                        Set(fl as Enum);
                    }
                }
                else
                {
                    throw new aceGeneralException("POS gram flags not found in the input declaration [" + declaration + "]", null, this, "POS gram flags failed in SetAll()");
                }
            }

            stringForm = declaration;
        }

        public IEnumerator<object> GetEnumerator()
        {
            return items.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.Values.GetEnumerator();
        }
    }
}