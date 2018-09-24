// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceDoc.cs" company="imbVeles" >
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

namespace imbNLP.PartOfSpeech.resourceProviders
{
    /// <summary>
    /// <para>Tools to load, query, convert and use lexicical resources specified in different formats like: Unitex, Multitext etc <para>
    /// </summary>
    /// <remarks>
    /// <para>How to use this namespace?<para>
    /// <para>You can use existing resource parser and grammatical tags converter like: Multitext Resource Parser <see cref="multitextResourceParser"/> to work with MULTITEXT v5.0 morphosyntactic dictionaries<para>
    /// <para>Or you can develop your own parser </para>
    /// <list>
    /// 	<listheader>
    ///			<term>To develop custom resource parser</term>
    ///			<description>e.g.</description>
    ///		</listheader>
    ///		<item>
    ///			<term>Create your Resource Parser</term>
    ///			<description>by inheriting: <see cref="textResourceResolverBase"/> and implementing very few methods required</description>
    ///		</item>
    ///		<item>
    ///			<term>Create Grammatical Tag converter specification</term>
    ///			<description>Use Excel template to create [languageid]_[format]_conversion.xlsx grammatical tag conversion table</description>
    ///		</item>
    /// </list>
    /// </remarks>
    /// <seealso cref="remarks" />
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class NamespaceDoc
    {
    }
}