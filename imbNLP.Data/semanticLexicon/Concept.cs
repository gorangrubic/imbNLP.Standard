// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Concept.cs" company="imbVeles" >
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
using BrightstarDB.EntityFramework;
using imbNLP.Data.semanticLexicon.core;
using System.Collections.Generic;

namespace imbNLP.Data.semanticLexicon
{
    using System;

    public partial class Concept : BrightstarEntityObject, IConcept
    {
        public Concept(BrightstarEntityContext context, BrightstarDB.Client.IDataObject dataObject) : base(context, dataObject)
        {
        }

        public Concept(BrightstarEntityContext context) : base(context, typeof(Concept))
        {
        }

        public Concept() : base()
        {
        }

        public string Id { get { return GetKey(); } set { SetKey(value); } }

        #region Implementation of imbLanguageFramework.semanticLexicon.core.IConcept

        public string name
        {
            get { return GetRelatedProperty<string>("name"); }
            set { SetRelatedProperty("name", value); }
        }

        public string description
        {
            get { return GetRelatedProperty<string>("description"); }
            set { SetRelatedProperty("description", value); }
        }

        public ICollection<ITermLemma> lemmas
        {
            get { return GetRelatedObjects<ITermLemma>("lemmas"); }
            set { if (value == null) throw new ArgumentNullException("value"); SetRelatedObjects("lemmas", value); }
        }

        public IConcept hyperConcept
        {
            get { return GetRelatedObject<IConcept>("hyperConcept"); }
            set { SetRelatedObject<IConcept>("hyperConcept", value); }
        }

        public ICollection<IConcept> relatedTo
        {
            get { return GetRelatedObjects<IConcept>("relatedTo"); }
            set { if (value == null) throw new ArgumentNullException("value"); SetRelatedObjects("relatedTo", value); }
        }

        public ICollection<IConcept> relatedFrom
        {
            get { return GetRelatedObjects<IConcept>("relatedFrom"); }
            set { if (value == null) throw new ArgumentNullException("value"); SetRelatedObjects("relatedFrom", value); }
        }

        public ICollection<IConcept> hypoConcepts
        {
            get { return GetRelatedObjects<IConcept>("hypoConcepts"); }
            set { if (value == null) throw new ArgumentNullException("value"); SetRelatedObjects("hypoConcepts", value); }
        }

        #endregion Implementation of imbLanguageFramework.semanticLexicon.core.IConcept
    }
}