// --------------------------------------------------------------------------------------------------------------------
// <copyright file="nlpTokenizatorSettings.cs" company="imbVeles" >
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
// Project: imbNLP.Core
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
namespace imbNLP.Core.contentStructure.tokenizator
{
    #region imbVELES USING

    using imbNLP.Data.enums;
    using imbSCI.Data.data;
    using System.ComponentModel;
    using System.Text.RegularExpressions;

    #endregion imbVELES USING

    /// <summary>
    /// 2013c: LowLevel resurs
    /// </summary>
    public class nlpTokenizatorSettings : imbBindable
    {
        public nlpTokenizatorSettings()
        {
            buildVowelRegex();
        }

        //public nlpTokenizatorSettings(imbFilterModule __contentFilter, imbFilterModule __sentenceFilter)
        //{
        //    contentFilter = __contentFilter;
        //    sentenceFilter = __sentenceFilter;

        //    buildVowelRegex();
        //}

        public void buildVowelRegex(string vowels = "aeiou")
        {
            string vc = vowels.ToLower() + vowels.ToUpper();

            _vowelRegex = new Regex("([^" + vc + "]*[" + vc + "])");

            _vowelLastRegex = new Regex("[" + vc + @"]\Z");
        }

        #region --- vowelRegex ------- Bindable property

        private Regex _vowelRegex;

        /// <summary>
        /// Bindable property
        /// </summary>
        public Regex vowelRegex
        {
            get
            {
                if (_vowelRegex == null) buildVowelRegex();
                return _vowelRegex;
            }
            set
            {
                _vowelRegex = value;
                OnPropertyChanged("vowelRegex");
            }
        }

        #endregion --- vowelRegex ------- Bindable property

        #region --- vowelLastRegex ------- Bindable property

        private Regex _vowelLastRegex;

        /// <summary>
        /// Bindable property
        /// </summary>
        public Regex vowelLastRegex
        {
            get
            {
                if (_vowelLastRegex == null) buildVowelRegex();
                return _vowelLastRegex;
            }
            set
            {
                _vowelLastRegex = value;
                OnPropertyChanged("vowelLastRegex");
            }
        }

        #endregion --- vowelLastRegex ------- Bindable property

        #region DEBUGER

        #region ----------- Boolean [ doCategorizationLog ] -------  [Da li da vodi log kateogirzacije Tokena, Recenica i Paragrapha]

        private bool _doCategorizationLog = true;

        /// <summary>
        /// Da li da vodi log kateogirzacije Tokena, Recenica i Paragrapha
        /// </summary>
        [Category("Switches")]
        [DisplayName("doCategorizationLog")]
        [Description("Da li da vodi log kateogirzacije Tokena, Recenica i Paragrapha")]
        public bool doCategorizationLog
        {
            get { return _doCategorizationLog; }
            set
            {
                _doCategorizationLog = value;
                OnPropertyChanged("doCategorizationLog");
            }
        }

        #endregion ----------- Boolean [ doCategorizationLog ] -------  [Da li da vodi log kateogirzacije Tokena, Recenica i Paragrapha]

        #endregion DEBUGER

        #region elements categorization settings

        #region ----------- Boolean [ doTokenTypeDetection_basic ] -------  [Da li da vrsi osnovnu proveru tipa tokena]

        private bool _doTokenTypeDetection_basic = true;

        /// <summary>
        /// Da li da vrsi osnovnu proveru tipa tokena
        /// </summary>
        [Category("Switches")]
        [DisplayName("doTokenTypeDetection_basic")]
        [Description("Da li da vrsi osnovnu proveru tipa tokena")]
        public bool doTokenTypeDetection_basic
        {
            get { return _doTokenTypeDetection_basic; }
            set
            {
                _doTokenTypeDetection_basic = value;
                OnPropertyChanged("doTokenTypeDetection_basic");
            }
        }

        #endregion ----------- Boolean [ doTokenTypeDetection_basic ] -------  [Da li da vrsi osnovnu proveru tipa tokena]

        #region ----------- Boolean [ doTokenTypeDetection_second ] -------  [Da li da vrsi napredninu proveru tipa, podesava letter case itd]

        private bool _doTokenTypeDetection_second = true;

        /// <summary>
        /// Da li da vrsi napredninu proveru tipa tokena pomocu recnika, podesava letter case itd
        /// </summary>
        [Category("Switches")]
        [DisplayName("doTokenTypeDetection_second")]
        [Description("Da li da vrsi napredninu proveru tipa tokena pomocu recnika, podesava letter case itd")]
        public bool doTokenTypeDetection_second
        {
            get { return _doTokenTypeDetection_second; }
            set
            {
                _doTokenTypeDetection_second = value;
                OnPropertyChanged("doTokenTypeDetection_second");
            }
        }

        #endregion ----------- Boolean [ doTokenTypeDetection_second ] -------  [Da li da vrsi napredninu proveru tipa, podesava letter case itd]

        #region ----------- Boolean [ doTokenTypeDetection_languageBasic ] -------  [Da li da vrsi osnovnu proveru pomocu recnika - da li je poznata rec itd]

        private bool _doTokenTypeDetection_languageBasic = true;

        /// <summary>
        /// Da li da vrsi osnovnu proveru pomocu recnika - da li je poznata rec itd
        /// </summary>
        [Category("Switches")]
        [DisplayName("doTokenTypeDetection_languageBasic")]
        [Description("Da li da vrsi osnovnu proveru pomocu recnika - da li je poznata rec itd")]
        public bool doTokenTypeDetection_languageBasic
        {
            get { return _doTokenTypeDetection_languageBasic; }
            set
            {
                _doTokenTypeDetection_languageBasic = value;
                OnPropertyChanged("doTokenTypeDetection_languageBasic");
            }
        }

        #endregion ----------- Boolean [ doTokenTypeDetection_languageBasic ] -------  [Da li da vrsi osnovnu proveru pomocu recnika - da li je poznata rec itd]

        #region ----------- Boolean [ doTokenTypeDetection_languageAdvanced ] -------  [Da li da vrsi naprednu proveru pomocu jezicnih definicija]

        private bool _doTokenTypeDetection_languageAdvanced = false;

        /// <summary>
        /// Da li da vrsi naprednu proveru pomocu jezicnih definicija
        /// </summary>
        [Category("Switches")]
        [DisplayName("doTokenTypeDetection_languageAdvanced")]
        [Description("Da li da vrsi naprednu proveru pomocu jezicnih definicija")]
        public bool doTokenTypeDetection_languageAdvanced
        {
            get { return _doTokenTypeDetection_languageAdvanced; }
            set
            {
                _doTokenTypeDetection_languageAdvanced = value;
                OnPropertyChanged("doTokenTypeDetection_languageAdvanced");
            }
        }

        #endregion ----------- Boolean [ doTokenTypeDetection_languageAdvanced ] -------  [Da li da vrsi naprednu proveru pomocu jezicnih definicija]

        #region ----------- Boolean [ doSentenceDetection ] -------  [Da li da vrsi proveru recenica]

        private bool _doSentenceDetection = true;

        /// <summary>
        /// Da li da vrsi proveru recenica
        /// </summary>
        [Category("Switches")]
        [DisplayName("doSentenceDetection")]
        [Description("Da li da vrsi proveru recenica")]
        public bool doSentenceDetection
        {
            get { return _doSentenceDetection; }
            set
            {
                _doSentenceDetection = value;
                OnPropertyChanged("doSentenceDetection");
            }
        }

        #endregion ----------- Boolean [ doSentenceDetection ] -------  [Da li da vrsi proveru recenica]

        #region ----------- Boolean [ doParagraphDetection ] -------  [Da li da vrsi proveru tipa paragrafa]

        private bool _doParagraphDetection = true;

        /// <summary>
        /// Da li da vrsi proveru tipa paragrafa
        /// </summary>
        [Category("Switches")]
        [DisplayName("doParagraphDetection")]
        [Description("Da li da vrsi proveru tipa paragrafa")]
        public bool doParagraphDetection
        {
            get { return _doParagraphDetection; }
            set
            {
                _doParagraphDetection = value;
                OnPropertyChanged("doParagraphDetection");
            }
        }

        #endregion ----------- Boolean [ doParagraphDetection ] -------  [Da li da vrsi proveru tipa paragrafa]

        #region ----------- Boolean [ doBlockDetection ] -------  [Da li detektuje tip blokova]

        private bool _doBlockDetection = true;

        /// <summary>
        /// Da li detektuje tip blokova
        /// </summary>
        [Category("Switches")]
        [DisplayName("doBlockDetection")]
        [Description("Da li detektuje tip blokova")]
        public bool doBlockDetection
        {
            get { return _doBlockDetection; }
            set
            {
                _doBlockDetection = value;
                OnPropertyChanged("doBlockDetection");
            }
        }

        #endregion ----------- Boolean [ doBlockDetection ] -------  [Da li detektuje tip blokova]

        #endregion elements categorization settings

        #region --- syllabLengthLimit ------- Maksimalna duzina sloga

        private int _syllabLengthLimit = 5;

        /// <summary>
        /// Maksimalna duzina sloga
        /// </summary>
        public int syllabLengthLimit
        {
            get { return _syllabLengthLimit; }
            set
            {
                _syllabLengthLimit = value;
                OnPropertyChanged("syllabLengthLimit");
            }
        }

        #endregion --- syllabLengthLimit ------- Maksimalna duzina sloga

        //#region -----------  contentFilter  -------  [Filter za ceo text sadrzaj]

        //private imbFilterModule _contentFilter = new imbFilterModule();

        ///// <summary>
        ///// Filter za ceo text sadrzaj
        ///// </summary>
        //// [XmlIgnore]
        //[Category("nlpTokenizatorSettings")]
        //[DisplayName("contentFilter")]
        //[Description("Filter za ceo text sadrzaj")]
        //public imbFilterModule contentFilter
        //{
        //    get { return _contentFilter; }
        //    set
        //    {
        //        // Boolean chg = (_contentFilter != value);
        //        _contentFilter = value;
        //        OnPropertyChanged("contentFilter");
        //        // if (chg) {}
        //    }
        //}

        //#endregion

        //#region -----------  sentenceFilter  -------  [Filter za jednu recenicu]

        //private imbFilterModule _sentenceFilter = new imbFilterModule();

        ///// <summary>
        ///// Filter za jednu recenicu
        ///// </summary>
        //// [XmlIgnore]
        //[Category("nlpTokenizatorSettings")]
        //[DisplayName("sentenceFilter")]
        //[Description("Filter za jednu recenicu")]
        //public imbFilterModule sentenceFilter
        //{
        //    get { return _sentenceFilter; }
        //    set
        //    {
        //        // Boolean chg = (_sentenceFilter != value);
        //        _sentenceFilter = value;
        //        OnPropertyChanged("sentenceFilter");
        //        // if (chg) {}
        //    }
        //}

        //#endregion

        #region --- tknType ------- tip tokenizacije

        private tokenizationType _tknType = tokenizationType.htmlTokenization;

        /// <summary>
        /// tip tokenizacije
        /// </summary>
        public tokenizationType tknType
        {
            get { return _tknType; }
            set
            {
                _tknType = value;
                OnPropertyChanged("tknType");
            }
        }

        #endregion --- tknType ------- tip tokenizacije
    }
}