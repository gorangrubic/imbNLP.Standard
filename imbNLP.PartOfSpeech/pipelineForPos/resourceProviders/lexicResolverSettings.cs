// --------------------------------------------------------------------------------------------------------------------
// <copyright file="lexicResolverSettings.cs" company="imbVeles" >
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
// Project: imbNLP.PartOfSpeech
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
using System;
using System.ComponentModel;

namespace imbNLP.PartOfSpeech.resourceProviders
{
    [Serializable]
    public class lexicResolverSettings
    {
        public lexicResolverSettings()
        {
        }

        /// <summary> If true it will log message on tokens that were not resolved </summary>
        [Category("Switch")]
        [DisplayName("doLogUnresolvedTokens")]
        [Description("If true it will log message on tokens that were not resolved")]
        public Boolean doLogUnresolvedTokens { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the resolver should lock multithreads when resolve called
        /// </summary>
        /// <value>
        ///   <c>true</c> if [do use lock on resolve call]; otherwise, <c>false</c>.
        /// </value>
        public Boolean doUseLockOnResolveCall { get; set; } = false;
    }
}