// --------------------------------------------------------------------------------------------------------------------
// <copyright file="apertiumDictionaryResult.cs" company="imbVeles" >
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
namespace imbNLP.PartOfSpeech.providers.dictionary.apertium
{
    using imbSCI.Core.extensions.data;
    using imbSCI.Data.collection.nested;
    using imbSCI.DataComplex.special;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Result of Apertium dictionary TODO: currently it is for HBS/Serbian vs English dictionary, make it universally usefull
    /// </summary>
    /// <seealso cref="aceCommonTypes.collection.special.translationTableMulti{System.String, System.String}" />
    public class apertiumDictionaryResult : translationTableMulti<String, String>
    {
        /// <summary>
        /// Regex select serbianWord : \\<p\\>\\<l\\>([\\w\\s-]*)\\<
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_serbianWord = new Regex("\\<p\\>\\<l\\>([\\w\\s-]*)\\<", RegexOptions.Compiled);

        /// <summary>
        /// Match Evaluation for serbianWord : _select_serbianWord
        /// </summary>
        /// <param name="m">Match with value to process</param>
        /// <returns>For m.value "something" returns "SOMETHING"</returns>
        private static String _replace_serbianWord(Match m)
        {
            String output = m.Value.Replace(".", "");
            output = output.Replace(" ", "");

            return output.ToUpper();
        }

        /// <summary>
        /// Regex select englishWord : \\<r\\>([\\w\\s-]*)\\<
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_englishWord = new Regex("\\<r\\>([\\w\\s-]*)\\<", RegexOptions.Compiled);

        /// <summary>
        /// Match Evaluation for englishWord : _select_englishWord
        /// </summary>
        /// <param name="m">Match with value to process</param>
        /// <returns>For m.value "something" returns "SOMETHING"</returns>
        private static String _replace_englishWord(Match m)
        {
            String output = m.Value.Replace(".", "");
            output = output.Replace(" ", "");

            return output.ToUpper();
        }

        /// <summary>
        /// Regex select wordType : n=\"([\\w]*)\"
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_wordType = new Regex("n=\"([\\w]*)\"", RegexOptions.Compiled);

        /// <summary>
        /// Processes the dictionary definition line (used internally by the framework, not intended for outside use)
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="buildModel">if set to <c>true</c> [build model].</param>
        public void addLine(String line, Boolean buildModel = true)
        {
            String srb = "";
            String eng = "";

            Match mc = _select_serbianWord.Match(line);
            if (mc.Success)
            {
                srb = mc.Groups[1].Value;
            }

            mc = _select_englishWord.Match(line);
            if (mc.Success)
            {
                eng = mc.Groups[1].Value;
            }
            if ((!srb.isNullOrEmpty()) && (!eng.isNullOrEmpty()))
            {
                Add(srb, eng);
            }

            if (buildModel)
            {
                MatchCollection mcc = _select_wordType.Matches(line);
                List<String> apTypes = new List<String>();
                foreach (Match mci in mcc)
                {
                    String cp = mci.Groups[1].Value;
                    apTypes.AddUnique(cp);
                    apertiumTypes.AddUnique(cp);
                }
                if (!termVsGramFlags.ContainsKey(srb)) termVsGramFlags.Add(srb, apertiumPOSConverter.findPosEnumsFromApertium(apTypes));

                /*
                termExploreModel md = null;
                if (!models.ContainsKey(srb))
                {
                    md = new termExploreModel(srb);
                    gramFlags gr = new gramFlags();
                    var posenums = ;
                    gr.Set(posenums);
                    md.gramSet.Add(gr);

                    models.Add(srb, md);
                }
                else
                {
                    md = models[srb];
                }
                md.translations.Add(eng);
                */
            }
        }

        private aceDictionarySet<String, Enum> _termVsGramFlags = new aceDictionarySet<String, Enum>();

        /// <summary> </summary>
        public aceDictionarySet<String, Enum> termVsGramFlags
        {
            get
            {
                return _termVsGramFlags;
            }
            protected set
            {
                _termVsGramFlags = value;
                OnPropertyChanged("termVsGramFlags");
            }
        }

        //private Dictionary<pos_type, termExploreModel> _wordTypes = new Dictionary<pos_type, termExploreModel>();
        ///// <summary>
        /////
        ///// </summary>
        //public Dictionary<pos_type, termExploreModel> wordTypes
        //{
        //    get { return _wordTypes; }
        //    protected set { _wordTypes = value; }
        //}

        /*
                private Dictionary<String, termExploreItem> _models = new Dictionary<String, termExploreItem>();
                /// <summary>
                ///
                /// </summary>
                public Dictionary<String, termExploreItem> models
                {
                    get
                    {
                        //if (_models == null)_models = new Dictionary<String, termExploreModel>();
                        return _models;
                    }
                    protected set { _models = value; }
                }
                */

        //private List<termExploreModel> _models = new List<termExploreModel>();
        ///// <summary>
        /////
        ///// </summary>
        //public List<termExploreModel> models
        //{
        //    get { return _models; }
        //    protected set { _models = value; }
        //}

        private List<String> _apertiumTypes = new List<string>();

        /// <summary>
        ///
        /// </summary>
        public List<String> apertiumTypes
        {
            get { return _apertiumTypes; }
            protected set { _apertiumTypes = value; }
        }

        //public void Add(String srb, String eng, String )

        //private gramFlags _gram;
        ///// <summary>
        /////
        ///// </summary>
        //public gramFlags gram
        //{
        //    get { return _gram; }
        //    set { _gram = value; }
        //}

        public Boolean isFound
        {
            get
            {
                if (!entries.Any()) return false;
                if (!apertiumTypes.Any()) return false;
                return true;
            }
        }

        public List<String> GetTranslatedWords()
        {
            return GetValues();
        }

        public List<String> GetNativeWords()
        {
            return GetKeys();
        }

        public List<String> GetTranslations(String word)
        {
            return GetByValue(word);
        }

        public List<String> GetNativeWords(String word)
        {
            return GetByKey(word);
        }

        public List<String> GetAll()
        {
            List<String> output = new List<string>();
            output.AddRange(GetTranslatedWords(), true);
            output.AddRange(GetNativeWords(), true);
            return output;
        }

        public override List<String> GetByValue(String needle)
        {
            List<String> output = new List<String>();
            foreach (var pair in entries)
            {
                if (String.Equals(pair.Value, needle, StringComparison.CurrentCultureIgnoreCase))
                {
                    output.AddUnique(pair.Value);
                }
            }
            return output;
        }

        public override List<String> GetByKey(String needle)
        {
            List<String> output = new List<String>();
            foreach (var pair in entries)
            {
                if (String.Equals(pair.Key, needle, StringComparison.CurrentCultureIgnoreCase))
                {
                    output.AddUnique(pair.Key);
                }
            }
            return output;
        }
    }
}