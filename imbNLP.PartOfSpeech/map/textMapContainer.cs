// --------------------------------------------------------------------------------------------------------------------
// <copyright file="textMapContainer.cs" company="imbVeles" >
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

namespace imbNLP.PartOfSpeech.map
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class textMapContainer<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="textMapContainer{T}"/> class.
        /// </summary>
        public textMapContainer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="textMapContainer{T}"/> class.
        /// </summary>
        /// <param name="_item">The item.</param>
        /// <param name="_render">The render.</param>
        /// <param name="_pos">The position.</param>
        public textMapContainer(T _item, String _render, Int32 _pos)
        {
            item = _item;
            render = _render;
            length = _render.Length;
            pos = _pos;
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public Int32 pos { get; set; } = 0;

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public Int32 length { get; set; } = 0;

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        /// <value>
        /// The item.
        /// </value>
        public T item { get; set; } = null;

        /// <summary>
        /// Gets or sets the render.
        /// </summary>
        /// <value>
        /// The render.
        /// </value>
        public String render { get; set; } = "";
    }
}