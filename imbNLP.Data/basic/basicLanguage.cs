// --------------------------------------------------------------------------------------------------------------------
// <copyright file="basicLanguage.cs" company="imbVeles" >
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
namespace imbNLP.Data.basic
{
    using imbACE.Core;
    using imbACE.Core.core.exceptions;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.enumworks;
    using imbSCI.Core.extensions.text;
    using imbSCI.Data;
    using imbSCI.Data.data;
    using NHunspell;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.IO;

    /// <summary>
    /// Wrapper for Hunspell dictionary with properties for extra information/description
    /// </summary>
    public class basicLanguage : imbBindable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="basicLanguage"/> class.
        /// </summary>
        public basicLanguage()
        {
        }

        internal void deploy(DataRow dr)
        {
            String en = dr[0].toStringSafe("unknown");

            languageEnum = (basicLanguageEnum)typeof(basicLanguageEnum).getEnumByName(en, basicLanguageEnum.unknown);

            file_prefix = dr[1].toStringSafe("");

            if (!file_prefix.isNullOrEmpty())
            {
                affixFilePath = appManager.Application.folder_resources.findFile(file_prefix + ".aff", SearchOption.AllDirectories);
                dictFilePath = appManager.Application.folder_resources.findFile(file_prefix + ".dic", SearchOption.AllDirectories);
            }

            iso2code = dr[2].toStringSafe();
            iso2country = dr[3].toStringSafe();

            languageEnglishName = dr[4].toStringSafe(languageEnum.ToString());

            languageNativeName = dr[5].toStringSafe(languageEnum.ToString());

            String needles = dr[6].toStringSafe(iso2code + "," + iso2country);
            langIDNeedles.AddRange(needles.SplitSmart(",", "", true, true));
        }

        public List<string> langIDNeedles { get; set; } = new List<string>();

        public basicLanguageEnum languageEnum { get; protected set; } = basicLanguageEnum.unknown;

        public String file_prefix { get; protected set; } = "";

        public String iso2code { get; protected set; } = "";

        public String iso2country { get; protected set; } = "";

        public String languageEnglishName { get; protected set; } = "";

        public String languageNativeName { get; protected set; } = "";

        public Boolean isLoaded
        {
            get
            {
                return hunspellEngine != null;
            }
        }

        /// <summary>
        /// Checks if the word was found in the dictionary
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual bool isKnownWord(string input)
        {
            return basicKnownWordTest(input);
        }

        protected bool basicKnownWordTest(string input)
        {
            checkHuspell();

            if (hunspellEngine == null)
            {
                var ex = new aceGeneralException("hunspellEngine not ready at basic language object", null, this, "Hunspell:" + languageEnglishName + " not ready");
                throw ex;

                return false;
            }
            return hunspellEngine.Spell(input);
        }

        /// <summary>
        /// Checks if hunspell files were found, and loads them
        /// </summary>
        internal bool checkHuspell()
        {
            bool output; // = new Boolean();

            bool change = false;

            try
            {
                if (hunspellEngine == null)
                {
                    hunspellEngine = new Hunspell(affixFilePath, dictFilePath);
                    hunspellHypen = new Hyphen(affixFilePath); // NHunspell.Hyphen(hunspellDictStream);

                    change = true;
                }
            }
            catch (Exception ex)
            {
                throw new aceGeneralException("checkHuspellFailed", ex, this, "Huspell check failed");

                return false;
            }

            return (hunspellEngine != null);
        }

        private SpellFactory _hunspellFactory; // = new  SpellFactory();

        /// <summary>
        /// Spell Factory instance
        /// </summary>
        [Category("Misc")]
        [DisplayName("hunspellFactory")]
        [Description("Sve opcije Hunspell biblioteke")]
        internal SpellFactory hunspellFactory
        {
            get { return _hunspellFactory; }
            set
            {
                _hunspellFactory = value;
            }
        }

        private Hyphen _hunspellHypen; // = new Hyphen();

        /// <summary>
        /// Hypenizator
        /// </summary>
        [Category("Misc")]
        [DisplayName("hunspellHypen")]
        [Description("Hypenizator")]
        internal Hyphen hunspellHypen
        {
            get { return _hunspellHypen; }
            set
            {
                _hunspellHypen = value;
            }
        }

        private Hunspell _hunspellEngine; // = new Hunspell();

        /// <summary>
        /// Objekat prema Hunspell engine-u. Objekat bi trebao da bude instanciran
        /// </summary>
        [Category("Misc")]
        [DisplayName("hunspellEngine")]
        [Description("Objekat prema Hunspell engine-u. Objekat bi trebao da bude instanciran")]
        internal Hunspell hunspellEngine
        {
            get
            {
               // prepareHunspellEngine();

                return _hunspellEngine;
            }
            set
            {
                _hunspellEngine = value;
            }
        }

        private string _affixFilePath = ""; // = new String();

        /// <summary>
        /// Path to affix file
        /// </summary>
        [Category("extendedLanguage")]
        [DisplayName("affixFilePath")]
        [Description("Path to affix file")]
        public string affixFilePath
        {
            get
            {
                return _affixFilePath;
            }
            protected set
            {
                _affixFilePath = value;
            }
        }

        private string _dictFilePath = ""; // = new String();

        /// <summary>
        /// Path to dict file
        /// </summary>
        [Category("extendedLanguage")]
        [DisplayName("dictFilePath")]
        [Description("Path to dict file")]
        public string dictFilePath
        {
            get
            {
                return _dictFilePath;
            }
            protected set
            {
                _dictFilePath = value;
            }
        }

        private byte[] _hunspellAffixStream; // = new String();

        /// <summary>
        /// Hunspell affix fajl stream - serijalizovana verzija Hunspell Afix fajla
        /// </summary>
        [Category("Misc")]
        [DisplayName("hunspellAffixStream")]
        [Description("Hunspell affix fajl stream - serijalizovana verzija Hunspell Afix fajla")]
        internal byte[] hunspellAffixStream
        {
            get { return _hunspellAffixStream; }
            set
            {
                _hunspellAffixStream = value;
            }
        }

        private byte[] _hunspellDictStream; // = new String();

        /// <summary>
        /// Hunspell dict fajl stream - serijalizovana verzija Hunspell dict fajla
        /// </summary>
        [Category("Misc")]
        [DisplayName("hunspellDictStream")]
        [Description("Hunspell dict fajl stream - serijalizovana verzija Hunspell dict fajla")]
        internal byte[] hunspellDictStream
        {
            get { return _hunspellDictStream; }
            set
            {
                _hunspellDictStream = value;
            }
        }
    }
}