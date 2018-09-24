// --------------------------------------------------------------------------------------------------------------------
// <copyright file="extendedLanguage.cs" company="imbVeles" >
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
namespace imbNLP.Data.extended
{
    using imbNLP.Data.basic;
    using imbSCI.Core.extensions.io;
    using imbSCI.Data.data;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;

    /// <summary>
    /// 2017> Jezička podešavanja i operacije sa morfologijom
    /// </summary>
    public class extendedLanguage : imbBindable
    {
        private Regex _regexSelectLetters = new Regex("([\\w]+)");

        /// <summary> </summary>
        public Regex regexSelectLetters
        {
            get
            {
                return _regexSelectLetters;
            }
            protected set
            {
                _regexSelectLetters = value;
                // OnPropertyChanged("regexSelectLetters");
            }
        }

        private Dictionary<String, String> _alfabet = new Dictionary<String, String>();

        /// <summary>
        /// Key=>capital letter, Value => Capital followed by minor
        /// </summary>
        protected Dictionary<String, String> alfabet
        {
            get
            {
                //if (_alfabet == null)_alfabet = new Dictionary<String, String>();
                return _alfabet;
            }
            set { _alfabet = value; }
        }

        public Boolean CheckAgainstAlfabet(String token)
        {
            foreach (Match mc in regexSelectLetters.Matches(token))
            {
                var chars = mc.Value.ToUpper().ToList();

                //if (chars.Any(x => !imbLanguageFrameworkManager.serbian.alfabet.ContainsKey(x.ToString()))) {
                //    return false;
                //}
            }

            return true; //as Boolean;
        }

        internal void loadAlfabet(string v)
        {
            var lines = openBase.openFileFilterLines(v);
            foreach (String ln in lines)
            {
                if (ln.Length > 1)
                {
                    alfabet.Add(ln[0].ToString(), ln);
                }
            }
        }

        #region -----------  basic  -------  [Referenca ka basicLanguage objektu]

        private basicLanguage _basic = new basicLanguage();

        /// <summary>
        /// Referenca ka basicLanguage objektu
        /// </summary>
        [XmlIgnore]
        [Category("extendedLanguage")]
        [DisplayName("basic")]
        [Description("Referenca ka basicLanguage objektu")]
        public basicLanguage basic
        {
            get
            {
                return _basic;
            }
            set
            {
                // Boolean chg = (_basic != value);
                _basic = value;
                OnPropertyChanged("basic");
                // if (chg) {}
            }
        }

        #endregion -----------  basic  -------  [Referenca ka basicLanguage objektu]

        /// <summary>
        /// Bezbedno preuzimanje propertija iso2code iz objekta basic
        /// </summary>
        public String iso2code
        {
            get
            {
                if (basic == null) return null;
                if (basic.iso2code == null) return null;
                return basic.iso2code as String;
            }
        }

        public Regex vowelLastRegex;
        public Regex vowelRegex;

        public void prepareModelForExecution()
        {
            String vc = vowels.ToLower() + vowels.ToUpper();

            vowelRegex = new Regex("([^" + vc + "]*[" + vc + "])");

            vowelLastRegex = new Regex("[" + vc + @"]\Z");

            // prepareHunspellEngine();

            /// SAMOGLASNICI
        }

        #region -----------  decimalSeparator  -------  [Decimalni separator]

        private String _decimalSeparator = "."; // = new String();

        /// <summary>
        /// Decimalni separator
        /// </summary>
        // [XmlIgnore]
        [Category("Numbers")]
        [DisplayName("decimalSeparator")]
        [Description("Decimalni separator")]
        public String decimalSeparator
        {
            get { return _decimalSeparator; }
            set
            {
                _decimalSeparator = value;
                OnPropertyChanged("decimalSeparator");
            }
        }

        #endregion -----------  decimalSeparator  -------  [Decimalni separator]

        #region -----------  kiloSeparator  -------  [Separator za hiljade, milione itd]

        private String _kiloSeparator = ","; // = new String();

        /// <summary>
        /// Separator za hiljade, milione itd
        /// </summary>
        // [XmlIgnore]
        [Category("Numbers")]
        [DisplayName("kiloSeparator")]
        [Description("Separator za hiljade, milione itd")]
        public String kiloSeparator
        {
            get { return _kiloSeparator; }
            set
            {
                _kiloSeparator = value;
                OnPropertyChanged("kiloSeparator");
            }
        }

        #endregion -----------  kiloSeparator  -------  [Separator za hiljade, milione itd]

        #region -----------  vowels  -------  [Svi samoglasnici - nije bitan case]

        private String _vowels = "aeiou"; // = new String();

        /// <summary>
        /// Svi samoglasnici - nije bitan case
        /// </summary>
        // [XmlIgnore]
        [Category("Syllable")]
        [DisplayName("vowels")]
        [Description("Svi samoglasnici - nije bitan case")]
        public String vowels
        {
            get { return _vowels; }
            set
            {
                _vowels = value;
                OnPropertyChanged("vowels");
            }
        }

        #endregion -----------  vowels  -------  [Svi samoglasnici - nije bitan case]

        #region -----------  vowelsSpecial  -------  [Pomocni samoglasnici - nije bitan case]

        private String _vowelsSpecial = "r"; // = new String();

        /// <summary>
        /// Pomocni samoglasnici - nije bitan case
        /// </summary>
        // [XmlIgnore]
        [Category("Syllable")]
        [DisplayName("vowelsSpecial")]
        [Description("Pomocni samoglasnici - nije bitan case")]
        public String vowelsSpecial
        {
            get { return _vowelsSpecial; }
            set
            {
                _vowelsSpecial = value;
                OnPropertyChanged("vowelsSpecial");
            }
        }

        #endregion -----------  vowelsSpecial  -------  [Pomocni samoglasnici - nije bitan case]

        #region -----------  syllableLengthLimit  -------  [Koliko je maksimalno dugačak jedan slog - služi za detektovanje slogova (bez samoglasnika) ]

        private Int32 _syllableLengthLimit = 3; // = new Int32();

        /// <summary>
        /// Koliko je maksimalno dugačak jedan slog (bez samoglasnika) - služi za detektovanje slogova -
        /// </summary>
        // [XmlIgnore]
        [Category("Syllable")]
        [DisplayName("syllableLengthLimit")]
        [Description("Koliko je maksimalno dugačak jedan slog - služi za detektovanje slogova")]
        public Int32 syllableLengthLimit
        {
            get { return _syllableLengthLimit; }
            set
            {
                _syllableLengthLimit = value;
                OnPropertyChanged("syllableLengthLimit");
            }
        }

        #endregion -----------  syllableLengthLimit  -------  [Koliko je maksimalno dugačak jedan slog - služi za detektovanje slogova (bez samoglasnika) ]
    }
}