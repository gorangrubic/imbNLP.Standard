// --------------------------------------------------------------------------------------------------------------------
// <copyright file="lexiconConsoleSettings.cs" company="imbVeles" >
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
namespace imbNLP.Data.semanticLexicon.console
{
    using imbACE.Core.core;
    using System.ComponentModel;
    using System.IO;
    using System.Xml.Serialization;

    [XmlInclude(typeof(aceSettingsBase))]
    [XmlInclude(typeof(aceSettingsStandaloneBase))]
    public class lexiconConsoleSettings : aceSettingsStandaloneBase
    {
        public lexiconConsoleSettings()
        {
        }

        /// <summary>
        /// Path where the settings is saved
        /// </summary>
        public override string settings_filepath
        {
            get
            {
                return imbACE.Core.application.aceApplicationInfo.FOLDERNAME_CONFIG + Path.DirectorySeparatorChar + "lexiconConsoleSettings.xml";
            }
        }

        private string _defaultSession = "Session01"; // = new String();

        /// <summary>
        /// Name of default Console Session to open
        /// </summary>
        [Category("lexiconConsoleSettings")]
        [DisplayName("Default Session")]
        [Description("Name of default Console Session to open")]
        public string defaultSession
        {
            get
            {
                return _defaultSession;
            }
            set
            {
                _defaultSession = value;
                OnPropertyChanged("defaultSession");
            }
        }

        #region ----------- Boolean [ doRunConsoleOnWorkshop ] -------  [Should automatically start Lexicon Console on entering Lexicon Workshop]

        private bool _doRunConsoleOnWorkshop = true;

        /// <summary>
        /// Should automatically start Lexicon Console on entering Lexicon Workshop
        /// </summary>
        [Category("Switches")]
        [DisplayName("Open Console On Workshop start")]
        [Description("Should automatically start Lexicon Console on entering Lexicon Workshop")]
        public bool doRunConsoleOnWorkshop
        {
            get { return _doRunConsoleOnWorkshop; }
            set { _doRunConsoleOnWorkshop = value; OnPropertyChanged("doRunConsoleOnWorkshop"); }
        }

        #endregion ----------- Boolean [ doRunConsoleOnWorkshop ] -------  [Should automatically start Lexicon Console on entering Lexicon Workshop]

        #region ----------- Boolean [ doRunInitiationAutomatically ] -------  [If true it runs Corpus initiation automatically]

        private bool _doRunInitiationAutomatically = true;

        /// <summary>
        /// If true it runs Corpus initiation automatically
        /// </summary>
        [Category("Switches")]
        [DisplayName("doRunInitiationAutomatically")]
        [Description("If true it runs Corpus initiation automatically")]
        public bool doRunInitiationAutomatically
        {
            get { return _doRunInitiationAutomatically; }
            set { _doRunInitiationAutomatically = value; OnPropertyChanged("doRunInitiationAutomatically"); }
        }

        #endregion ----------- Boolean [ doRunInitiationAutomatically ] -------  [If true it runs Corpus initiation automatically]

        private string _autoexecScriptFilename = "lexiconConsole_autoexec.ace"; // = new String();

        /// <summary>
        /// Path to a script that will be automatically executed on console start up
        /// </summary>
        [Category("lexiconConsoleSettings")]
        [DisplayName("Autoexec Script Filename")]
        [Description("Path to a script that will be automatically executed on console start up")]
        public string autoexecScriptFilename
        {
            get
            {
                return _autoexecScriptFilename;
            }
            set
            {
                _autoexecScriptFilename = value;
                OnPropertyChanged("autoexecScriptFilename");
            }
        }

        #region ----------- Boolean [ doRunAutoexecScript ] -------  [if TRUE it will run autoexec script from corpus project folder]

        private bool _doRunAutoexecScript = true;

        /// <summary>
        /// if TRUE it will run autoexec script from corpus project folder
        /// </summary>
        [Category("Switches")]
        [DisplayName("doRunAutoexecScript")]
        [Description("if TRUE it will run autoexec script from corpus project folder")]
        public bool doRunAutoexecScript
        {
            get { return _doRunAutoexecScript; }
            set { _doRunAutoexecScript = value; OnPropertyChanged("doRunAutoexecScript"); }
        }

        #endregion ----------- Boolean [ doRunAutoexecScript ] -------  [if TRUE it will run autoexec script from corpus project folder]
    }
}