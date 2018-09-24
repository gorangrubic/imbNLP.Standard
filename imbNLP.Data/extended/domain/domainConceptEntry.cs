// --------------------------------------------------------------------------------------------------------------------
// <copyright file="domainConceptEntry.cs" company="imbVeles" >
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
namespace imbNLP.Data.extended.domain
{
    using imbSCI.Core.extensions.data;
    using imbSCI.Data;
    using imbSCI.Data.data;
    using imbSCI.Data.interfaces;
    using System;
    using System.Collections.Generic;
    using Concept = semanticLexicon.Concept;

    public class domainConceptEntry : imbBindable, IObjectWithName, IObjectWithDescription
    {
        public domainConceptEntry(String __name, String __description, String __needles)
        {
            name = __name;
            description = __description;
            needles = new List<string>();
            if (__needles == "[null]") __needles = "";

            if (!__needles.isNullOrEmpty())
            {
                needles.AddRange(__needles.SplitSmart(",", "", true));
            }
        }

        public void findConnectionTargets()
        {
        }

        private Concept _concept;

        /// <summary>
        ///
        /// </summary>
        public Concept concept
        {
            get { return _concept; }
            set { _concept = value; }
        }

        private String _name;

        /// <summary>
        ///
        /// </summary>
        public String name
        {
            get { return _name; }
            set { _name = value; }
        }

        private String _description;

        /// <summary>
        ///
        /// </summary>
        public String description
        {
            get { return _description; }
            set { _description = value; }
        }

        private List<String> _needles;

        /// <summary>
        ///
        /// </summary>
        public List<String> needles
        {
            get { return _needles; }
            set { _needles = value; }
        }
    }
}