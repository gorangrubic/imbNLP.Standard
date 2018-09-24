// --------------------------------------------------------------------------------------------------------------------
// <copyright file="wordVariationsMethodType.cs" company="imbVeles" >
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
namespace imbNLP.Data.basic.enums
{
    /// <summary>
    /// 2017: Metode za dobijanje predloga varijacija
    /// </summary>
    public enum wordVariationsMethodType
    {
        /// <summary>
        /// Koristi Huspell algoritam za izdvajanje varijacija
        /// </summary>
        Huspell,

        /// <summary>
        /// Koristi morfologiju za generisanje varijacija
        /// </summary>
        /// <remarks>
        /// <para>Potrebno je definisati </para>
        /// <list type="bullet" >
        /// <item>doo</item>
        /// <item>d.o.o.</item>
        /// <item>d. o. o.</item>
        /// <item>Doo</item>
        /// <item>DOO</item>
        /// </list>
        /// </remarks>
        Morphology,

        /// <summary>
        /// Pravi varijacije za skraćenice
        /// </summary>
        /// <remarks>
        /// <para>Skraćenice: ako je uneto DOO pravi varijacije:</para>
        /// <list type="bullet" >
        /// <item>doo</item>
        /// <item>d.o.o.</item>
        /// <item>d. o. o.</item>
        /// <item>Doo</item>
        /// <item>DOO</item>
        /// </list>
        /// </remarks>
        Abrevations,

        /// <summary>
        /// Formalne varijacije za složenice - npr. kod naziva firme.
        /// </summary>
        /// <remarks>
        /// <para>Nazivi firme koji imaju više reči se nekada pišu na različite načine. </para>
        /// <para>Na primer: EURO FORM</para>
        /// <list type="bullet" >
        /// <item>EURO FORM</item>
        /// <item>EURO-FORM</item>
        /// <item>EUROFORM</item>
        /// <item>EuroForm</item>
        /// <item>EURO (samo prva reč)</item>
        /// <item>...</item>
        /// </list>
        /// <para>Postoje i nazivi sa više od dve reči: EURO FORM TECH</para>
        /// <list type="bullet" >
        /// <item>EURO FORM</item>
        /// <item>EUROFORM TECH</item>
        /// <item>EURO FORMTECH</item>
        /// <item>EuroForm Tech</item>
        /// <item>...</item>
        /// </list>
        /// </remarks>
        Formatings,
    }
}