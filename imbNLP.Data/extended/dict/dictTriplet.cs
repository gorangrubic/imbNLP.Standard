// --------------------------------------------------------------------------------------------------------------------
// <copyright file="dictTriplet.cs" company="imbVeles" >
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
namespace imbNLP.Data.extended.dict
{
    using imbNLP.Data.extended.wordnet;
    using System;

    public class dictTriplet : ITokenDictionaryTriplet
    {
        public dictTriplet(String __token, String __meaning, String __code)
        {
            token = __token;
            meaning = __meaning;
            code = __code;
        }

        private String _token;

        /// <summary>
        ///
        /// </summary>
        public String token
        {
            get { return _token; }
            set { _token = value; }
        }

        private String _code;

        /// <summary>
        ///
        /// </summary>
        public String code
        {
            get { return _code; }
            set { _code = value; }
        }

        private String _meaning;

        /// <summary>
        ///
        /// </summary>
        public String meaning
        {
            get { return _meaning; }
            set { _meaning = value; }
        }
    }
}