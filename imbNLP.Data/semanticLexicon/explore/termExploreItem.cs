// --------------------------------------------------------------------------------------------------------------------
// <copyright file="termExploreItem.cs" company="imbVeles" >
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
namespace imbNLP.Data.semanticLexicon.explore
{
    using imbNLP.Data.semanticLexicon.posCase;
    using imbNLP.PartOfSpeech.flags.basic;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.reporting;
    using imbSCI.Data.data;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// Jedinica term modela
    /// </summary>
    /// <seealso cref="aceCommonTypes.primitives.imbBindable" />
    public class termExploreItem : imbBindable
    {
        public static string inlineFormat = "{0,16} t:{1,8}";

        public override string ToString()
        {
            string output = string.Format(inlineFormat, inputForm, gramSet.ToString());
            return output;
        }

        public pos_type getPosType()
        {
            if (gramSet == null)
            {
                return pos_type.TEMP;
            }
            return gramSet.getPosType();
        }

        public string getGramSet()
        {
            if (gramSet == null)
            {
                return gramSet.GetAll();
            }
            return gramSetDeclaration.or("TEMP");
        }

        /// <summary>
        /// Logs a multiline description of the gramCaseSet
        /// </summary>
        /// <param name="log">The log.</param>
        public void ToString(ILogBuilder log, string itemName = "Instance", bool expanded = false)
        {
            //StringBuilder sb = new StringBuilder();
            log.AppendLine(itemName + ": " + inputForm);
            log.consoleAltColorToggle();
            gramSet.ToString(log, expanded);
            log.consoleAltColorToggle();
        }

        public termExploreItem(string __inputForm, string __gramSetLine)
        {
            gramSet.SetAll(__gramSetLine);
            gramSetDeclaration = __gramSetLine;
            inputForm = __inputForm;
        }

        public termExploreItem(string __inputForm)
        {
            inputForm = __inputForm;
        }

        public termExploreItem()
        {
        }

        private string _inputForm = ""; // = new String();

        /// <summary>
        /// The initial form
        /// </summary>
        [Category("termExploreModel")]
        [DisplayName("inputForm")]
        [Description("Description of $property$")]
        public string inputForm
        {
            get
            {
                return _inputForm;
            }
            set
            {
                _inputForm = value;
                OnPropertyChanged("inputForm");
            }
        }

        private string _gramSetDeclaration;

        /// <summary>
        ///
        /// </summary>
        public string gramSetDeclaration
        {
            get
            {
                return gramSet.GetAll();
            }
            set
            {
                gramSet.SetAll(value);
            }
        }

        private gramCaseSet _gramSet = new gramCaseSet();

        /// <summary> </summary>
        [XmlIgnore]
        public gramCaseSet gramSet
        {
            get
            {
                return _gramSet;
            }
            set
            {
                _gramSet = value;
                OnPropertyChanged("gramSet");
            }
        }

        /// <summary>
        ///
        /// </summary>
        public termExploreItemEnumFlag flags { get; set; } = termExploreItemEnumFlag.none;
    }
}