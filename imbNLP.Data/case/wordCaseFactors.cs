// --------------------------------------------------------------------------------------------------------------------
// <copyright file="wordCaseFactors.cs" company="imbVeles" >
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
using imbSCI.Data.data;

namespace imbNLP.Data.@case
{
    #region imbVELES USING

    using imbNLP.Data.@case.enums;
    using System;
    using System.ComponentModel;

    #endregion imbVELES USING

    public class wordCaseFactors : imbBindable
    {
        #region -----------  wordNumber  -------  [Gramaticki broj u kome se nalazi rec]

        private number _wordNumber = number.undefined; // = new number();

        /// <summary>
        /// Gramaticki broj u kome se nalazi rec
        /// </summary>
        // [XmlIgnore]
        [Category("wordCase")]
        [DisplayName("wordNumber")]
        [Description("Gramaticki broj u kome se nalazi rec")]
        public number wordNumber
        {
            get { return _wordNumber; }
            set
            {
                _wordNumber = value;
                OnPropertyChanged("wordNumber");
            }
        }

        #endregion -----------  wordNumber  -------  [Gramaticki broj u kome se nalazi rec]

        #region -----------  wordFace  -------  [Gramaticko lice u kome se nalazi rec]

        private face _wordFace = face.undefined; // = new face();

        /// <summary>
        /// Gramaticko lice u kome se nalazi rec
        /// </summary>
        // [XmlIgnore]
        [Category("wordCase")]
        [DisplayName("wordFace")]
        [Description("Gramaticko lice u kome se nalazi rec")]
        public face wordFace
        {
            get { return _wordFace; }
            set
            {
                _wordFace = value;
                OnPropertyChanged("wordFace");
            }
        }

        #endregion -----------  wordFace  -------  [Gramaticko lice u kome se nalazi rec]

        #region -----------  wordGenre  -------  [Gramaticki rod u kome se nalazi rec]

        private genre _wordGenre = genre.undefined; // = new genre();

        /// <summary>
        /// Gramaticki rod u kome se nalazi rec
        /// </summary>
        // [XmlIgnore]
        [Category("wordCase")]
        [DisplayName("wordGenre")]
        [Description("Gramaticki rod u kome se nalazi rec")]
        public genre wordGenre
        {
            get { return _wordGenre; }
            set
            {
                _wordGenre = value;
                OnPropertyChanged("wordGenre");
            }
        }

        #endregion -----------  wordGenre  -------  [Gramaticki rod u kome se nalazi rec]

        #region -----------  wordForm  -------  [Kog je oblika trenutna rec]

        private wordForms _wordForm = wordForms.undefined; // = new wordForms();

        /// <summary>
        /// Kog je oblika trenutna rec
        /// </summary>
        // [XmlIgnore]
        [Category("wordCase")]
        [DisplayName("wordForm")]
        [Description("Kog je oblika trenutna rec")]
        public wordForms wordForm
        {
            get { return _wordForm; }
            set
            {
                _wordForm = value;
                OnPropertyChanged("wordForm");
            }
        }

        #endregion -----------  wordForm  -------  [Kog je oblika trenutna rec]

        #region -----------  wordGramaticalCase  -------  [Padez u kome se nalazi rec]

        private gramaticalCase _wordGramaticalCase = gramaticalCase.undefined; // = new gramaticalCase();

        /// <summary>
        /// Padez u kome se nalazi rec
        /// </summary>
        // [XmlIgnore]
        [Category("wordCase")]
        [DisplayName("wordGramaticalCase")]
        [Description("Padez u kome se nalazi rec")]
        public gramaticalCase wordGramaticalCase
        {
            get { return _wordGramaticalCase; }
            set
            {
                _wordGramaticalCase = value;
                OnPropertyChanged("wordGramaticalCase");
            }
        }

        #endregion -----------  wordGramaticalCase  -------  [Padez u kome se nalazi rec]

        #region -----------  root  -------  [Koren reci koji predlaze analiza ]

        private String _root; // = new String();

        /// <summary>
        /// Koren reci koji predlaze analiza
        /// </summary>
        // [XmlIgnore]
        [Category("wordCase")]
        [DisplayName("root")]
        [Description("Koren reci koji predlaze analiza ")]
        public String root
        {
            get { return _root; }
            set
            {
                _root = value;
                OnPropertyChanged("root");
            }
        }

        #endregion -----------  root  -------  [Koren reci koji predlaze analiza ]

        #region -----------  infinitiveRoot  -------  [Koren infinitiva]

        private String _infinitiveRoot; // = new String();

        /// <summary>
        /// Koren infinitiva
        /// </summary>
        // [XmlIgnore]
        [Category("wordCase")]
        [DisplayName("infinitiveRoot")]
        [Description("Koren infinitiva")]
        public String infinitiveRoot
        {
            get { return _infinitiveRoot; }
            set
            {
                _infinitiveRoot = value;
                OnPropertyChanged("infinitiveRoot");
            }
        }

        #endregion -----------  infinitiveRoot  -------  [Koren infinitiva]

        #region -----------  infinitiveSufix  -------  [Infinitivni sufix]

        private String _infinitiveSufix; // = new String();

        /// <summary>
        /// Infinitivni sufix
        /// </summary>
        // [XmlIgnore]
        [Category("wordCase")]
        [DisplayName("infinitiveSufix")]
        [Description("Infinitivni sufix")]
        public String infinitiveSufix
        {
            get { return _infinitiveSufix; }
            set
            {
                _infinitiveSufix = value;
                OnPropertyChanged("infinitiveSufix");
            }
        }

        #endregion -----------  infinitiveSufix  -------  [Infinitivni sufix]

        #region -----------  presentRoot  -------  [Koren u prezentu]

        private String _presentRoot; // = new String();

        /// <summary>
        /// Koren u prezentu
        /// </summary>
        // [XmlIgnore]
        [Category("wordCase")]
        [DisplayName("presentRoot")]
        [Description("Koren u prezentu")]
        public String presentRoot
        {
            get { return _presentRoot; }
            set
            {
                _presentRoot = value;
                OnPropertyChanged("presentRoot");
            }
        }

        #endregion -----------  presentRoot  -------  [Koren u prezentu]
    }
}