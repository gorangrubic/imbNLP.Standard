// --------------------------------------------------------------------------------------------------------------------
// <copyright file="wordnetTriplet.cs" company="imbVeles" >
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
namespace imbNLP.Data.extended.wordnet
{
    using imbNLP.Transliteration;
    using imbSCI.Core.extensions.text;
    using System;
    using System.Linq;

    public class wordnetTriplet : ITokenDictionaryTriplet
    {
        /// <summary>
        /// The code noun - imenica
        /// </summary>
        public const Int32 CODE_NOUN = 1;

        /// <summary>
        /// The code verb - glagol
        /// </summary>
        public const Int32 CODE_VERB = 2;

        /// <summary>
        /// The code for adjective
        /// </summary>
        public const Int32 CODE_ADJECTIVE = 3;

        /// <summary>
        /// The code adverb: prilog
        /// </summary>
        public const Int32 CODE_ADVERB = 4;

        /// <summary>
        /// The code adjective satellite
        /// </summary>
        public const Int32 CODE_ADJECTIVE_SATELLITE = 5;

        public wordnetTriplet(Object[] input)
        {
            original = input[0].toStringSafe();

            code = input[1].toStringSafe();
            meaning = input[2].toStringSafe();
            if (input.Count() == 4)
            {
                token = input[3].toStringSafe();
            }
            else
            {
                token = original.transliterate();
            }
        }

        private String _original;

        /// <summary>
        ///
        /// </summary>
        public String original
        {
            get { return _original; }
            set { _original = value; }
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