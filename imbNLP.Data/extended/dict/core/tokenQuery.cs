// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tokenQuery.cs" company="imbVeles" >
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
namespace imbNLP.Data.extended.dict.core
{
    using imbNLP.Data.enums.flags;
    using imbSCI.Core.reporting;
    using imbSCI.Data.data;
    using System;

    public class tokenQuery : imbBindable
    {
        private ILogBuilder _loger;

        /// <summary> </summary>
        public ILogBuilder loger
        {
            get
            {
                return _loger;
            }
            set
            {
                _loger = value;
                OnPropertyChanged("loger");
            }
        }

        public tokenQuery(String __token, Object __metadata, tokenQuerySourceEnum __sources)
        {
            sources = __sources;
            token = __token;
            metadata = __metadata;
            // language = imbLanguageFrameworkManager.serbian;
        }

        private extendedLanguage _language;

        /// <summary>
        ///
        /// </summary>
        public extendedLanguage language
        {
            get { return _language; }
            set { _language = value; }
        }

        private tokenQueryResponceCollection _responses = new tokenQueryResponceCollection();

        /// <summary> </summary>
        public tokenQueryResponceCollection responses
        {
            get
            {
                return _responses;
            }
            protected set
            {
                _responses = value;
                OnPropertyChanged("responses");
            }
        }

        private Int32 _limitAnswer = 1;

        /// <summary>
        /// Max. number of matches to be returned on quering knowledge base tables
        /// </summary>
        public Int32 limitAnswer
        {
            get { return _limitAnswer; }
            set { _limitAnswer = value; }
        }

        private contentTokenFlag _focus;

        /// <summary>
        ///
        /// </summary>
        public contentTokenFlag focus
        {
            get { return _focus; }
            set { _focus = value; }
        }

        private String _token;

        /// <summary> </summary>
        public String token
        {
            get
            {
                return _token;
            }
            protected set
            {
                _token = value;
                OnPropertyChanged("token");
            }
        }

        private Object _metadata = new Object();

        /// <summary> </summary>
        public Object metadata
        {
            get
            {
                return _metadata;
            }
            protected set
            {
                _metadata = value;
                OnPropertyChanged("metadata");
            }
        }

        private tokenQuerySourceEnum _sources;

        /// <summary> </summary>
        public tokenQuerySourceEnum sources
        {
            get
            {
                return _sources;
            }
            protected set
            {
                _sources = value;
                OnPropertyChanged("sources");
            }
        }
    }
}