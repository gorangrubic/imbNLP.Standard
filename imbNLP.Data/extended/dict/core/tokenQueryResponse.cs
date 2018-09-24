// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tokenQueryResponse.cs" company="imbVeles" >
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
    using imbNLP.Data.semanticLexicon.explore;
    using imbSCI.Data.data;
    using imbSCI.DataComplex.special;
    using System;
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// Response about the query about a token
    /// </summary>
    /// <seealso cref="aceCommonTypes.primitives.imbBindable" />
    public class tokenQueryResponse : imbBindable
    {
        private instanceCountCollection<contentTokenFlag> _flags = new instanceCountCollection<contentTokenFlag>();

        /// <summary>
        /// Here it counts every hit my evaluators
        /// </summary>
        public instanceCountCollection<contentTokenFlag> flags
        {
            get { return _flags; }
            set { _flags = value; }
        }

        private termExploreModel _exploreModel;

        /// <summary>
        ///
        /// </summary>
        public termExploreModel exploreModel
        {
            get { return _exploreModel; }
            set { _exploreModel = value; }
        }

        public tokenQueryResponse(tokenQuery query, tokenQuerySourceEnum __source)
        {
            source = __source;
            // tokenCase = new wordCase(query.token, query.language, dictionary.enums.tosWordType.unknown);
        }

        private tokenQuerySourceEnum _source = tokenQuerySourceEnum.unknown;

        /// <summary>
        ///
        /// </summary>
        public tokenQuerySourceEnum source
        {
            get { return _source; }
            set { _source = value; }
        }

        private List<DataRow> _dataRows = new List<DataRow>();

        /// <summary>
        ///
        /// </summary>
        public List<DataRow> dataRows
        {
            get { return _dataRows; }
            set { _dataRows = value; }
        }

        private List<String> _dataTokens = new List<string>();

        /// <summary>
        ///
        /// </summary>
        public List<String> dataTokens
        {
            get { return _dataTokens; }
            set { _dataTokens = value; }
        }

        private tokenQueryResultEnum _status = tokenQueryResultEnum.unknown;

        /// <summary>
        ///
        /// </summary>
        public tokenQueryResultEnum status
        {
            get { return _status; }
            set { _status = value; }
        }

        private Object _metadata = new PropertyCollection();

        /// <summary> </summary>
        public Object metadata
        {
            get
            {
                return _metadata;
            }
            set
            {
                _metadata = value;
                OnPropertyChanged("metadata");
            }
        }

        public void setResponse(tokenQueryResultEnum __status)
        {
            status = __status;
        }

        /// <summary>
        /// Sets the respons status to accept
        /// </summary>
        /// <param name="__response">The response.</param>
        /// <param name="__description">The description.</param>
        public void setResponse(String __response, String __description)
        {
            response = __response;
            description = __description;
            status = tokenQueryResultEnum.accept;
        }

        private String _response = ""; //= new String();

        /// <summary> </summary>
        public String response
        {
            get
            {
                return _response;
            }
            protected set
            {
                _response = value;
                OnPropertyChanged("response");
            }
        }

        private String _description = ""; //= new String();

        /// <summary> </summary>
        public String description
        {
            get
            {
                return _description;
            }
            protected set
            {
                _description = value;
                //OnPropertyChanged("description");
            }
        }
    }
}