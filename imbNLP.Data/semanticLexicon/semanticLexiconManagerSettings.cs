// --------------------------------------------------------------------------------------------------------------------
// <copyright file="semanticLexiconManagerSettings.cs" company="imbVeles" >
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
namespace imbNLP.Data.semanticLexicon
{
    using imbACE.Core.core;
    using imbNLP.Data.semanticLexicon.source;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    [XmlInclude(typeof(aceSettingsBase))]
    [XmlInclude(typeof(aceSettingsStandaloneBase))]
    public class semanticLexiconManagerSettings : aceSettingsStandaloneBase
    {
        /// <summary>
        /// Default value of term expansion
        /// </summary>
        /// <value>
        /// The term expansion default.
        /// </value>
        public int termExpansionDefault { get; set; } = 3;

        public semanticLexiconManagerSettings()
        {
        }

        public override string settings_filepath
        {
            get
            {
                return imbACE.Core.application.aceApplicationInfo.FOLDERNAME_CONFIG + Path.DirectorySeparatorChar + "lexiconManagerSettings.xml";
            }
        }

        public void checkDefaults()
        {
            if (!sourceFiles.Any())
            {
                sourceFiles.setDefaults();
            }
        }

        /// <summary> If <c>true</c> it will perform preprocessing steps over each term query sent to semanticLexiconCache resolver </summary>
        [Category("Flag")]
        [DisplayName("doQueryPreprocess")]
        [Description("If <c>true</c> it will perform preprocessing steps over each term query sent to semanticLexiconCache resolver")]
        public bool doQueryPreprocess { get; set; } = false;

        private bool _DoPreloadLexicon = true; // = new Boolean();

        /// <summary>
        /// It will preload complete Lexicon at startup / prepare() call
        /// </summary>
        [Category("semanticLexiconManagerSettings")]
        [DisplayName("DoPreloadLexicon")]
        [Description("It will preload complete Lexicon at startup / prepare() call")]
        public bool DoPreloadLexicon
        {
            get
            {
                return _DoPreloadLexicon;
            }
            set
            {
                _DoPreloadLexicon = value;
                //if (value) _DoInMemoryCache = true;
                OnPropertyChanged("DoPreloadLexicon");
            }
        }

        private bool _DoInMemoryCache = true; // = new Boolean();

        /// <summary>
        /// It will keep in memory cache of query term vs lemmas
        /// </summary>
        [Category("semanticLexiconManagerSettings")]
        [DisplayName("DoInMemoryCache")]
        [Description("It will keep in memory cache of query term vs lemmas")]
        public bool DoInMemoryCache
        {
            get
            {
                return _DoInMemoryCache;
            }
            set
            {
                _DoInMemoryCache = value;
                OnPropertyChanged("DoInMemoryCache");
            }
        }

        #region ----------- Boolean [ doAutoMakeSynonymRelationship ] -------  [If set true it will automatically make synonym and conceptual links between newly added Terms]

        private bool _doAutoMakeSynonymRelationship = false;

        /// <summary>
        /// If set true it will automatically make synonym and conceptual links between newly added Terms
        /// </summary>
        [Category("Switches")]
        [DisplayName("doAutoMakeSynonymRelationship")]
        [Description("If set true it will automatically make synonym and conceptual links between newly added Terms")]
        public bool doAutoMakeSynonymRelationship
        {
            get { return _doAutoMakeSynonymRelationship; }
            set { _doAutoMakeSynonymRelationship = value; OnPropertyChanged("doAutoMakeSynonymRelationship"); }
        }

        #endregion ----------- Boolean [ doAutoMakeSynonymRelationship ] -------  [If set true it will automatically make synonym and conceptual links between newly added Terms]

        #region ----------- Boolean [ doAllowLexiconSaveOnExplore ] -------  [If true it will allow newly discovered TermLemma and TermInstances to be saved into triplestore]

        /// <summary>
        /// If true it will allow newly discovered TermLemma and TermInstances to be saved into triplestore
        /// </summary>
        [Category("Switches")]
        [DisplayName("doAllowLexiconSaveOnExplore")]
        [Description("If true it will allow newly discovered TermLemma and TermInstances to be saved into triplestore")]
        public bool doAllowLexiconSaveOnExplore { get; set; } = false;

        #endregion ----------- Boolean [ doAllowLexiconSaveOnExplore ] -------  [If true it will allow newly discovered TermLemma and TermInstances to be saved into triplestore]

        #region ----------- Boolean [ doResolveWordsInDebugMode ] -------  [If true it will do token resolving procedure in the Debug mode, generating CSV and txt reports for each lemma]

        /// <summary>
        /// If true it will do token resolving procedure in the Debug mode, generating CSV and txt reports for each lemma
        /// </summary>
        [Category("Switches")]
        [DisplayName("doResolveWordsInDebugMode")]
        [Description("If true it will do token resolving procedure in the Debug mode, generating CSV and txt reports for each lemma")]
        public bool doResolveWordsInDebugMode { get; set; } = true;

        #endregion ----------- Boolean [ doResolveWordsInDebugMode ] -------  [If true it will do token resolving procedure in the Debug mode, generating CSV and txt reports for each lemma]

        /// <summary>
        ///
        /// </summary>
        public lexiconSourceFileList sourceFiles { get; set; } = new lexiconSourceFileList();

        #region ----------- Boolean [ doAutoexplore ] -------  [When True it will automatically explore for undefined terms]

        private bool _doAutoexplore = false;

        /// <summary>
        /// When True it will automatically explore for undefined terms
        /// </summary>
        [Category("Switches")]
        [DisplayName("doAutoexplore")]
        [Description("When True it will automatically explore for undefined terms")]
        public bool doAutoexplore
        {
            get { return _doAutoexplore; }
            set { _doAutoexplore = value; OnPropertyChanged("doAutoexplore"); }
        }

        #endregion ----------- Boolean [ doAutoexplore ] -------  [When True it will automatically explore for undefined terms]
    }
}