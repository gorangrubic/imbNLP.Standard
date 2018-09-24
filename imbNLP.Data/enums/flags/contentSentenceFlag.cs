// --------------------------------------------------------------------------------------------------------------------
// <copyright file="contentSentenceFlag.cs" company="imbVeles" >
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

namespace imbNLP.Data.enums.flags
{
    using System;

    /// <summary>
    /// Flagovi - contentSentenceFlag
    /// </summary>
    [Flags]
    public enum contentSentenceFlag
    {
        none = 0,

        /// <summary>
        /// Predstavlja regularnu recenicu --  zavrsava se sa pravilnim spliterom
        /// </summary>
        regular = 1 << 1,

        caseSentence = 1 << 2,

        caseCapital = 1 << 3,

        /// <summary>
        /// obavezno pocinje malim slovom ili simbolom a ostalo sta bude
        /// </summary>
        caseFragment = 1 << 4,

        /// <summary>
        /// u pitanju je simbolicki sadrzaj koji sluzi za dekoraciju, formatiranje itd
        /// </summary>
        formating = 1 << 5,

        /// <summary>
        /// Nije pronadjen spliter na kraju recenice
        /// </summary>
        inregular = 1 << 6,

        /// <summary>
        /// Izgleda da je naslov u pitanju> sadrzaj pocinje velikim slovom ili je sve velikim slovima
        /// </summary>
        title = 1 << 7,

        /// <summary>
        /// Izgleda da je naslov koji ima "meki" case
        /// </summary>
        titleSoftCase = 1 << 8,

        /// <summary>
        /// Naslov koji je ceo velikim slovima
        /// </summary>
        titleStrongCase = 1 << 9,

        /// <summary>
        /// Kada je rec o naslovu za neku stavku
        /// </summary>
        titleForItem = 1 << 10,

        /// <summary>
        /// Kada znam da je u pitanju tekst iz a taga
        /// </summary>
        titleForLink = 1 << 11,

        /// <summary>
        /// Sadrzi samo navigacioni link
        /// </summary>
        navigationContainer = 1 << 12,

        /// <summary>
        /// Navigacioni link
        /// </summary>
        navigationLink = 1 << 13,

        /// <summary>
        /// Kada je to jedina titularna recenica u bloku - paragrafu
        /// </summary>
        titleForBlock = 1 << 14,

        /// <summary>
        ///
        /// </summary>
        item = 1 << 15,

        /// <summary>
        /// zavrsava se ;
        /// </summary>
        itemInList = 1 << 16,

        /// <summary>
        /// zavrsava se sa ;...
        /// </summary>
        itemInListLast = 1 << 17,

        /// <summary>
        /// zavrsava se uzvikom
        /// </summary>
        exclamation = 1 << 18,

        /// <summary>
        /// Obicna recenica
        /// </summary>
        normal = 1 << 19,

        /// <summary>
        /// zavrsava se upitnikom
        /// </summary>
        question = 1 << 20,

        /// <summary>
        /// zavrsava se karakterom : -- ukazuje na sledecu recenicu, sadrzaj
        /// </summary>
        pointing = 1 << 21,

        /// <summary>
        /// zavrsava se tackama: ... ....
        /// </summary>
        unfinished = 1 << 22,
    }
}