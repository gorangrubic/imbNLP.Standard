// --------------------------------------------------------------------------------------------------------------------
// <copyright file="textMap.cs" company="imbVeles" >
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
using imbSCI.Core.extensions.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace imbNLP.PartOfSpeech.map
{
    public abstract class textMapBase
    {
        public const String SEPARATOR = "║";
        public const String SEPARATOR_REPLACEMENT = "|";

        public const String CONTAINER_OPEN = "╠";
        public const String CONTAINER_CLOSE = "╣";

        public const String MAINLEVEL_COMMA = ":";
        public const String SUBLEVEL_COMMA = ",";
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class textMap<T> : textMapBase where T : class
    {
        // public contentTokenSubjectRenderMode renderMode { get; set; }

        public textMap()
        {
            //
        }

        /// <summary>
        /// Gets or sets the render.
        /// </summary>
        /// <value>
        /// The render.
        /// </value>
        public String render { get; protected set; } = "";

        public String GetCleanRender()
        {
            String output = render;
            output = output.Replace(SEPARATOR, " ");
            output = output.Replace(SEPARATOR_REPLACEMENT, SEPARATOR);
            output = output.Replace(CONTAINER_OPEN, "");
            output = output.Replace(CONTAINER_CLOSE, "");

            return output.Trim();
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        protected List<textMapContainer<T>> items { get; set; } = new List<textMapContainer<T>>();

        /// <summary>
        /// Adds the render.
        /// </summary>
        /// <param name="part">The part.</param>
        protected void AddRender(String part)
        {
            render += SEPARATOR + part;
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="part">The part.</param>
        public void Add(T item, String part)
        {
            part = part.Replace(SEPARATOR, SEPARATOR_REPLACEMENT);
            Int32 pos = render.Length;
            AddRender(part);
            items.Add(new textMapContainer<T>(item, part, pos));
        }

        /// <summary>
        /// Simply adds at the end of rendering the specified content
        /// </summary>
        /// <param name="content">The content.</param>
        public void AddPlainRender(String content)
        {
            render += content;
        }

        protected Stack<textMapContainer<T>> stack { get; set; } = new Stack<textMapContainer<T>>();

        /// <summary>
        /// Adds the open.
        /// </summary>
        /// <param name="item">The item.</param>
        public void AddOpen(T item, String prefix = "")
        {
            render = render + prefix;

            textMapContainer<T> st = new textMapContainer<T>(item, "", render.Length);
            stack.Push(st);
        }

        /// <summary>
        /// Adds the close.
        /// </summary>
        public void AddClose(String sufix = "")
        {
            if (stack.Any())
            {
                textMapContainer<T> st = stack.Pop();
                st.render = render.Substring(st.pos, (render.Length - st.pos));
                st.length = st.render.Length;
                items.Add(st);
                render = render + sufix;
            }
        }

        /// <summary>
        /// Selects the container.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="length">The length.</param>
        /// <returns>Containers</returns>
        public List<textMapContainer<T>> SelectContainer(Int32 pos = -1, Int32 length = -1)
        {
            if (pos == -1) pos = 0;
            if (length == -1) length = render.Length;

            Int32 A = pos;
            Int32 B = pos + length;

            List<textMapContainer<T>> output = new List<textMapContainer<T>>();

            foreach (var it in items)
            {
                if ((it.pos >= A) && (it.pos + it.length < B))
                {
                    output.AddUnique(it);
                }

                //if ((it.pos+it.length >= A) && (it.pos + it.length < B ))
                //{
                //    output.AddUnique(it);
                //}
            }

            return output;
        }

        /// <summary>
        /// Selects the specified position.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="length">The length.</param>
        /// <returns>Items</returns>
        public List<T> Select(Int32 pos = -1, Int32 length = -1)
        {
            List<textMapContainer<T>> output = SelectContainer(pos, length);
            List<T> l = new List<T>();
            foreach (var t in output)
            {
                l.Add(t.item);
            }
            return l;
        }

        /// <summary>
        /// Selects mapped items with the specified matches.
        /// </summary>
        /// <param name="matches">The matches.</param>
        /// <returns></returns>
        public List<List<T>> Select(MatchCollection matches)
        {
            List<textMapContainer<T>> output = new List<textMapContainer<T>>();
            List<List<T>> g_output = new List<List<T>>();
            List<List<T>> l = new List<List<T>>();
            foreach (Match m in matches)
            {
                if (m.Success)
                {
                    var mGroup = SelectContainer(m.Index, m.Length);
                    var g = new List<T>();

                    foreach (var t in mGroup)
                    {
                        g.AddUnique(t.item);
                    }
                    l.Add(g);
                }
            }

            return l;
        }
    }
}