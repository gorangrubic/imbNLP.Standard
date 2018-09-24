// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tokenGraphNode.cs" company="imbVeles" >
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

//using aceCommonTypes.extensions.text;

namespace imbNLP.PartOfSpeech.lexicUnit.tokenGraphs
{
    using imbSCI.Data;
    using imbSCI.Data.interfaces;
    using imbSCI.DataComplex.special;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Graph structure for processing the results of <see cref="extendedLanguage"/> queries
    /// </summary>
    public class tokenGraphNode : IEnumerable<tokenGraphNode>, IObjectWithParent, IObjectWithPath, IObjectWithName, IObjectWithPathAndChildren
    {
        public const String SEPARATOR = "--> ";
        public const String SEPARATOR_CHILD = "|-> ";

        public String ToStringPathList()
        {
            String output = "";
            var allLeafs = this.getAllLeafs();

            foreach (IObjectWithPathAndChildren pair in allLeafs)
            {
                output = output + pair.path + Environment.NewLine;
            }
            return output;
        }

        public String ToStringTreeview(String prefix = "", Boolean showType = false, Int32 gen = 0)
        {
            String output = prefix + token + "[l:" + level.ToString("D2") + "]";

            if (showType) output += ":" + type.ToString();
            output = output.PadLeft(40 * level);

            Boolean firstChild = true;

            String cPrefix = new String(' ', output.Length);

            foreach (var pair in children)
            {
                if (firstChild)
                {
                    String ch = pair.Value.ToStringTreeview(cPrefix + SEPARATOR, showType, gen + 1);
                    output = output + ch.removeStartsWith(cPrefix);
                    firstChild = false;
                }
                else
                {
                    output = output + Environment.NewLine;
                    output = output + pair.Value.ToStringTreeview(cPrefix + SEPARATOR_CHILD, showType, gen + 1);
                }
            }

            return output;
        }

        public Int32 level
        {
            get
            {
                Int32 output = 1;
                if (parent != null) return parent.level + output;
                return output;
            }
        }

        public List<String> getChildTokens()
        {
            return children.Keys.ToList();
        }

        public tokenGraphNode getFirst()
        {
            if (children.Any())
            {
                return children.First().Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Creating instance of a child token node
        /// </summary>
        /// <param name="__token">The token.</param>
        /// <param name="__type">The type.</param>
        /// <param name="__parent">The parent.</param>
        public tokenGraphNode(String __token, tokenGraphNodeType __type, tokenGraphNode __parent = null)
        {
            token = __token;
            type = __type;
            parent = __parent;
        }

        /// <summary>
        /// Adds children and their children -- where Key is child, and Value is grandchild
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="typeForGrandChild">The type for value.</param>
        /// <param name="typeForChild">The type for key.</param>
        public void AddKeyValueChildren(IEnumerable<KeyValuePair<String, String>> source, tokenGraphNodeType typeForChild, tokenGraphNodeType typeForGrandChild, Boolean inverse = false)
        {
            foreach (var pair in source)
            {
                if (inverse)
                {
                    this[pair.Value, typeForChild].Add(pair.Key, typeForGrandChild);
                }
                else
                {
                    this[pair.Key, typeForChild].Add(pair.Value, typeForGrandChild);
                }
            }
        }

        /// <summary>
        /// Adds the key matches.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="typeForValue">The type for value.</param>
        public void AddKeyMatches(translationTableMulti<String, String> source, tokenGraphNodeType typeForValue)
        {
            foreach (var pair in source)
            {
                if (pair.Key == token)
                {
                    Add(pair.Value, typeForValue);
                }
            }
        }

        /// <summary>
        /// Adds the value matches in the <see cref="imbSCI.DataComplex.special.translationTableMulti{TKey,TValue}",apertiumDictionaryResult or wordnet query result
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="typeForKey">The type for key.</param>
        public void AddValueMatches(translationTableMulti<String, String> source, tokenGraphNodeType typeForKey)
        {
            foreach (var pair in source)
            {
                if (pair.Value == token)
                {
                    Add(pair.Key, typeForKey);
                }
            }
        }

        /// <summary>
        /// Add - generic method
        /// </summary>
        /// <param name="__token">The token.</param>
        /// <param name="__type">The type.</param>
        public void Add(String __token, tokenGraphNodeType __type)
        {
            var tkng = new tokenGraphNode(__token, __type, this);
            if (!children.ContainsKey(__token))
            {
                children.Add(__token, tkng);
            }
        }

        public void Add(IEnumerable<String> __tokens, tokenGraphNodeType __type)
        {
            foreach (String __token in __tokens)
            {
                var tkng = new tokenGraphNode(__token, __type, this);
                if (!children.ContainsKey(__token))
                {
                    children.Add(__token, tkng);
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="tokenGraphNode" /> with the specified <c>t</c>. If <c>t</c> not found a new child with specified <c>cType</c> is created and returned
        /// </summary>
        /// <value>
        /// The <see cref="tokenGraphNode" />.
        /// </value>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        private tokenGraphNode this[String t, tokenGraphNodeType cType = tokenGraphNodeType.none]
        {
            get
            {
                if (children.ContainsKey(t))
                {
                    return children[t];
                }
                else
                {
                    if (cType == tokenGraphNodeType.none)
                    {
                        throw new ArgumentException("There is no [" + t + "] child, and autocreation is prevented because the cType is not set");
                        return null;
                    }

                    Add(t, cType);
                    return children[t];
                }
            }
        }

        public Int32 Count()
        {
            return children.Count();
        }

        public void Remove(String key)
        {
            children.Remove(key);
        }

        IEnumerator<tokenGraphNode> IEnumerable<tokenGraphNode>.GetEnumerator()
        {
            return children.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return children.Values.GetEnumerator();
        }

        IEnumerator<IObjectWithPathAndChildren> IEnumerable<IObjectWithPathAndChildren>.GetEnumerator()
        {
            return children.Values.GetEnumerator();
        }

        private Dictionary<String, tokenGraphNode> _children = new Dictionary<String, tokenGraphNode>();

        /// <summary>
        ///
        /// </summary>
        protected Dictionary<String, tokenGraphNode> children
        {
            get
            {
                return _children;
            }
            set { _children = value; }
        }

        private tokenGraphNode _parent;

        /// <summary> </summary>
        public tokenGraphNode parent
        {
            get
            {
                return _parent;
            }
            protected set
            {
                _parent = value;
            }
        }

        private String _token;

        /// <summary>
        ///
        /// </summary>
        public String token
        {
            get { return _token; }
            protected set { _token = value; }
        }

        private tokenGraphNodeType _type = tokenGraphNodeType.none;

        /// <summary> </summary>
        public tokenGraphNodeType type
        {
            get
            {
                return _type;
            }
            protected set
            {
                _type = value;
            }
        }

        public string path
        {
            get
            {
                String output = name;
                if (parent != null) return parent.path + "\\" + name;
                return output;
            }
        }

        public string name
        {
            get
            {
                return token;
            }
            set
            {
            }
        }

        public object root
        {
            get
            {
                if (parent == null) return this;
                return parent.root;
            }
        }

        object IObjectWithParent.parent
        {
            get
            {
                return parent;
            }
        }
    }
}