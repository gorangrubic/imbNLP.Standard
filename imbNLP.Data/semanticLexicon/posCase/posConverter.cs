// --------------------------------------------------------------------------------------------------------------------
// <copyright file="posConverter.cs" company="imbVeles" >
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
    using imbNLP.Data.extended.wordnet;
    using imbNLP.PartOfSpeech.flags.basic;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.enumworks;
    using imbSCI.Core.extensions.text;
    using imbSCI.Data;
    using imbSCI.DataComplex.special;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public static class posConverter
    {
        /// <summary>
        /// The code noun - imenica
        /// </summary>
        public const int CODE_NOUN = 1;

        /// <summary>
        /// The code verb - glagol
        /// </summary>
        public const int CODE_VERB = 2;

        /// <summary>
        /// The code for adjective
        /// </summary>
        public const int CODE_ADJECTIVE = 3;

        /// <summary>
        /// The code adverb: prilog
        /// </summary>
        public const int CODE_ADVERB = 4;

        /// <summary>
        /// The code adjective satellite
        /// </summary>
        public const int CODE_ADJECTIVE_SATELLITE = 5;

        public static int GetWordNetCodeStart(this pos_type type)
        {
            switch (type)
            {
                case pos_type.A:
                    return CODE_ADJECTIVE;
                    break;

                case pos_type.N:
                    return CODE_NOUN;
                    break;

                case pos_type.V:
                    return CODE_VERB;
                    break;

                case pos_type.ADV:
                    return CODE_ADVERB;
                    break;

                default:
                    return CODE_ADJECTIVE_SATELLITE;
                    break;
            }
        }

        private static translationForValuesAndTypes _posFlagsTranslator;

        /// <summary>
        /// static and autoinitiated object
        /// </summary>
        public static translationForValuesAndTypes posFlagsTranslator
        {
            get
            {
                if (_posFlagsTranslator == null)
                {
                    _posFlagsTranslator = new translationForValuesAndTypes();
                    _posFlagsTranslator.Add(typeof(pos_type), "Word type");
                    _posFlagsTranslator.Add(typeof(pos_gender), "Gender");
                    _posFlagsTranslator.Add(pos_gender.f, "female");
                    _posFlagsTranslator.Add(pos_gender.m, "male");
                    _posFlagsTranslator.Add(pos_gender.n, "neutral");
                    _posFlagsTranslator.Add(typeof(pos_number), "Number");
                    _posFlagsTranslator.Add(pos_number.p, "plural");
                    _posFlagsTranslator.Add(pos_number.s, "singular");
                    _posFlagsTranslator.Add(pos_number.w, "collection");
                    _posFlagsTranslator.Add(typeof(pos_verbform), "Form");
                    _posFlagsTranslator.Add(typeof(pos_gramaticalCase), "Case");
                    _posFlagsTranslator.Add(typeof(pos_degree), "Degree");
                    _posFlagsTranslator.Add(typeof(pos_person), "Person");
                    _posFlagsTranslator.Add(typeof(pos_definitness), "Definit.");
                    _posFlagsTranslator.Add(typeof(pos_animatness), "Animat.");
                }
                return _posFlagsTranslator;
            }
        }

        private static translationEnumPatternTable<pos_type> _posTypeVsPattern;

        /// <summary>
        /// static and autoinitiated object
        /// </summary>
        public static translationEnumPatternTable<pos_type> posTypeVsPattern
        {
            get
            {
                if (_posTypeVsPattern == null)
                {
                    _posTypeVsPattern = new translationEnumPatternTable<pos_type>();
                    _posTypeVsPattern[pos_type.N].AddRange(new Type[] { typeof(pos_gender), typeof(pos_number), typeof(pos_gramaticalCase), typeof(pos_animatness) });
                    _posTypeVsPattern[pos_type.V].AddRange(new Type[] { typeof(pos_verbform), typeof(pos_person), typeof(pos_number) });
                    _posTypeVsPattern[pos_type.A].AddRange(new Type[] { typeof(pos_degree), typeof(pos_definitness), typeof(pos_gender), typeof(pos_number), typeof(pos_gramaticalCase), typeof(pos_animatness) });
                    _posTypeVsPattern[pos_type.NUM].AddRange(new Type[] { typeof(pos_gender), typeof(pos_number), typeof(pos_gramaticalCase), typeof(pos_animatness) });
                    _posTypeVsPattern[pos_type.PRO].AddRange(new Type[] { typeof(pos_person), typeof(pos_gender), typeof(pos_number), typeof(pos_gramaticalCase), typeof(pos_clitic), typeof(pos_animatness) });
                }
                return _posTypeVsPattern;
            }
        }

        private static translationEnumTable _posTypeVsString;

        /// <summary>
        /// static and autoinitiated object
        /// </summary>
        public static translationEnumTable posTypeVsString
        {
            get
            {
                if (_posTypeVsString == null)
                {
                    _posTypeVsString = new translationEnumTable();
                    _posTypeVsString.AddEnumAll<pos_gender>();
                    _posTypeVsString.AddEnumAll<pos_animatness>();
                    _posTypeVsString.AddEnumAll<pos_clitic>();
                    _posTypeVsString.AddEnumAll<pos_definitness>();
                    _posTypeVsString.AddEnumAll<pos_degree>();
                    _posTypeVsString.AddEnumAsNumericAll<pos_gramaticalCase>();
                    _posTypeVsString.AddEnumAll<pos_negation>();
                    _posTypeVsString.AddEnumAll<pos_number>();
                    _posTypeVsString.AddEnumAll<pos_person>();
                    _posTypeVsString.AddEnumAll<pos_type>();
                    _posTypeVsString.AddEnumAll<pos_verbform>();
                }
                return _posTypeVsString;
            }
        }

        private static translationTableMulti<Enum, string> _posTypeVsApertiumPosType;

        /// <summary>
        /// Translation between apertium and unitex
        /// </summary>
        public static translationTableMulti<Enum, string> posTypeVsApertiumPosType
        {
            get
            {
                if (_posTypeVsApertiumPosType == null)
                {
                    _posTypeVsApertiumPosType = new translationTableMulti<Enum, string>();
                    _posTypeVsApertiumPosType.Add(pos_type.A, "adj");
                    _posTypeVsApertiumPosType.Add(pos_type.PRO, "prn");
                    _posTypeVsApertiumPosType.Add(pos_type.ADV, "adv");
                    _posTypeVsApertiumPosType.Add(pos_type.PAR, "pr");
                    _posTypeVsApertiumPosType.Add(pos_type.CONJ, "cnjcoo");
                    _posTypeVsApertiumPosType.Add(pos_type.V, "vblex");
                    _posTypeVsApertiumPosType.Add(pos_type.V, "vbmod");
                    _posTypeVsApertiumPosType.Add(pos_type.N, "n");
                    _posTypeVsApertiumPosType.Add(pos_type.ABB, "abbr");
                    _posTypeVsApertiumPosType.Add(pos_type.PREP, "pr");
                    _posTypeVsApertiumPosType.Add(pos_type.PAR, "part");
                    _posTypeVsApertiumPosType.Add(pos_type.NUM, "num");
                    _posTypeVsApertiumPosType.Add(pos_type.PRO, "pro");
                }
                return _posTypeVsApertiumPosType;
            }
        }

        //public static pos_type findType(List<String> apTypes)
        //{
        //    foreach (String ap in apTypes) {
        //        List<pos_type> ps =  posTypeVsApertiumPosType.GetByValue(ap);
        //        if (ps.Any()) return ps.First();
        //    }
        //    return pos_type.none;
        //}

        public static List<Enum> findPosEnumsFromApertium(List<string> apTypes)
        {
            List<Enum> ps = new List<Enum>();
            foreach (string ap in apTypes)
            {
                ps.AddRange(posTypeVsApertiumPosType.GetByValue(ap));
                ps.AddRange(apertiumToPos.GetByValue(ap));
            }
            return ps;
        }

        private static translationTableMulti<Enum, string> _apertiumToPos;

        /// <summary>
        /// static and autoinitiated object
        /// </summary>
        public static translationTableMulti<Enum, string> apertiumToPos
        {
            get
            {
                if (_apertiumToPos == null)
                {
                    _apertiumToPos = new translationTableMulti<Enum, string>();
                    _apertiumToPos.Add(pos_gender.f, "f");
                    _apertiumToPos.Add(pos_gender.m, "ma");
                    _apertiumToPos.Add(pos_animatness.v, "ma");
                    _apertiumToPos.Add(pos_gender.m, "mi");
                    _apertiumToPos.Add(pos_animatness.q, "mi");
                    _apertiumToPos.Add(pos_gender.n, "nt");
                }
                return _apertiumToPos;
            }
        }

        private static translationTable<string, pos_type> _wordNetFirstNumToPosType;

        /// <summary>
        /// static and autoinitiated object
        /// </summary>
        public static translationTable<string, pos_type> wordNetFirstNumToPosType
        {
            get
            {
                if (_wordNetFirstNumToPosType == null)
                {
                    _wordNetFirstNumToPosType = new translationTable<string, pos_type>();
                    _wordNetFirstNumToPosType.Add(wordnetTriplet.CODE_NOUN.ToString(), pos_type.N);
                    _wordNetFirstNumToPosType.Add(wordnetTriplet.CODE_VERB.ToString(), pos_type.V);
                    _wordNetFirstNumToPosType.Add(wordnetTriplet.CODE_ADVERB.ToString(), pos_type.ADV);
                    _wordNetFirstNumToPosType.Add(wordnetTriplet.CODE_ADJECTIVE.ToString(), pos_type.A);
                    _wordNetFirstNumToPosType.Add(wordnetTriplet.CODE_ADJECTIVE_SATELLITE.ToString(), pos_type.PREP);
                }
                return _wordNetFirstNumToPosType;
            }
        }

        public static void AddGramSet(this regexQuerySet<object> termResolver, string regex, string declaration)
        {
            List<string> keys = declaration.SplitSmart("|");

            foreach (string it in keys)
            {
                var gt = new gramFlags(it);
                foreach (var g in gt)
                {
                    if (g.toStringSafe() == "none")
                    {
                    }
                    else
                    {
                        termResolver.Add(regex, g);
                    }
                }
            }
        }

        public static Regex REGEX_UNITEX_Declaration = new Regex("(\\w*):?(?:[\\+]?(\\w*))*:?(\\w*)");
        public static Regex REGEX_UNITEX_DecPosType = new Regex("^(\\w*)");
        public static Regex REGEX_UNITEX_DecGrams = new Regex("(\\w*)$");

        public static string REGEX_UNITEX_InstanceToLemmaFormat = "^{0},([\\w]+)\\.([\\w\\:]*)";
        public static string REGEX_UNITEX_LemmaToInstanceFormat = "^([\\w]*),{0}\\.([\\w\\:]*)";
        public static string REGEX_UNITEX_DeclarationForLemma = "^{0},\\.([\\w\\:\\+]*)";

        public static Regex REGEX_UNITEX_MarkersSelection = new Regex(".\\+([\\w\\+]*):");

        public const string FORMAT_instanceToLemma = "\\: {0},([\\w]*).([\\w\\:]*)";
        public const string FORMAT_lemmaToInstance = "\\: ([\\w]*),{0}.([\\w\\:]*)";

        public static void prepare()
        {
            posTypeVsPattern.Poke();
            posTypeVsString.Poke();
            posFlagsTranslator.Poke();
            imbLanguageFrameworkManager.log.log("Unitex Pos converter ready.");
        }

        public static string getString(Enum flag)
        {
            // if (type == pos_type.none) return "";

            if (flag.ToInt32() == 0) return "";

            return posTypeVsString[flag];
        }

        public static T getFlag<T>(string stringForm)
        {
            return posTypeVsString.GetEnum<T>(stringForm);
        }

        public static object getFlag(Type type, string stringForm)
        {
            return posTypeVsString.GetEnum(type, stringForm);
        }
    }
}