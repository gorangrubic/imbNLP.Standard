// --------------------------------------------------------------------------------------------------------------------
// <copyright file="contentMatchCollection.cs" company="imbVeles" >
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
// Project: imbNLP.Core
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
namespace imbNLP.Core.contentStructure.core
{
    #region imbVELES USING

    using imbACE.Core.core.exceptions;
    using imbSCI.Core.collection;
    using imbSCI.Core.extensions.text;
    using imbSCI.Data.collection.nested;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text.RegularExpressions;

    #endregion imbVELES USING

    //public class contentSubSentence

    /// <summary>
    /// Kolekcija sa pronadjenim Regex match-ovima
    /// </summary>
    public class contentMatchCollection : aceConcurrentDictionary<contentMatch> //:List<KeyValuePair<T, Match>>
    {
        public contentMatchCollection()
        {
        }

        #region --- scrambled ------- sadrzaj koji je prekrecen

        private string _scrambled;

        /// <summary>
        /// sadrzaj koji je prekrecen
        /// </summary>
        public string scrambled
        {
            get { return _scrambled; }
            set
            {
                if (string.IsNullOrEmpty(_scrambled))
                {
                    _scrambled = value;
                }
                if (string.IsNullOrEmpty(_sourceContent))
                {
                    _sourceContent = value;
                }

                //OnPropertyChanged("scrambled");
            }
        }

        #endregion --- scrambled ------- sadrzaj koji je prekrecen

        #region --- sourceContent ------- izvorni sadrzaj

        private string _sourceContent = "";

        /// <summary>
        /// izvorni sadrzaj
        /// </summary>
        public string sourceContent
        {
            get { return _sourceContent; }
            set
            {
                if (string.IsNullOrEmpty(_sourceContent))
                {
                    _sourceContent = value;
                }
                else
                {
                    _scrambled = value;
                }

                //  OnPropertyChanged("sourceContent");
            }
        }

        #endregion --- sourceContent ------- izvorni sadrzaj

        /// <summary>
        /// Da li je za dati Regex Match vec sve rezervisano
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool isAllocated(Match m)
        {
            if (m == null) return false;
            return isAllocated(m.Index, m.Length);
        }

        public bool isAllocated(int index, int length)
        {
            if (Count == 0) return false;
            if (allocation.ContainsKey(index)) return true;
            if (allocation.ContainsKey(index + length)) return true;
            contentMatch cm = null;
            int ind = index;
            while (ind < index + length)
            {
                if (allocation.ContainsKey(ind))
                {
                    return true;
                }
                ind++;
            }
            return false;
        }

        /// <summary>
        /// Vraca sve contentMatch instance koje se nalaze od indexa do kraja lengtha
        /// </summary>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public oneOrMore<contentMatch> allocated(int index, int length)
        {
            oneOrMore<contentMatch> output = new oneOrMore<contentMatch>();

            if (Count == 0) return output;
            contentMatch cm = null;
            int ind = index;
            int sc = 0;
            int scl = 10000;
            while (ind < index + length)
            {
                if (allocation.ContainsKey(ind))
                {
                    cm = allocation[ind];
                    if (cm == null)
                    {
                    }
                    else
                    {
                        if (!output.Contains(cm))
                        {
                            output.Add(cm);
                        }

                        if (cm.match.Length > 0)
                        {
                            ind = ind + cm.match.Length;
                        }
                        else
                        {
                            ind++;
                        }
                    }
                }
                else
                {
                    ind++;
                }
                sc++;
                if (sc > scl)
                {
                    throw new aceGeneralException("allocated(" + index + "," + length + ") failed: safety count is triggered [" +
                                        sc + "]");
                    break;
                }
            }
            return output;
        }

        public int lastAlocatedCharIndex(int def = 1)
        {
            if (Count == 0) return def;
            int mx = allocation.Keys.Max();
            return mx;
        }

        /// <summary>
        /// Izvršava Regex i na osnovu rezultata upisuje contentMatch item u kolekciju i prepravlja content
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public string Add(Regex reg, Enum flag)
        {
            return Add(_scrambled, reg, flag);
        }

        /// <summary>
        /// Upisuje u sebe rezultate i vraca verziju ulaznog stringa u kojoj su presvrljana sva pojavljivanja pogodaka
        /// </summary>
        /// <param name="content"></param>
        /// <param name="reg"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public string Add(string content, Regex reg, Enum flag, string mask = "#")
        {
            sourceContent = content;

            _scrambled = content;

            //String scrambled = content;
            if (string.IsNullOrEmpty(content)) return content;

            var matches = reg.Matches(content);

            foreach (Match m in matches)
            {
                if (m.Success)
                {
                    contentMatch cm = new contentMatch(flag, m);

                    Add(cm.name, cm);

                    for (int ind = cm.match.Index; ind <= cm.match.Index + cm.match.Length; ind++)
                    {
                        if (allocation.ContainsKey(ind))
                        {
                            allocation[ind] = cm;
                        }
                        else
                        {
                            allocation.Add(ind, cm);
                        }
                    }

                    // Add(new KeyValuePair<T, Match>(flag, m));
                    _scrambled = _scrambled.overwrite(m, mask);
                    /*
                    scrambled = scrambled.Substring(0, m.Index) + mask.Repeat(m.Length) +
                                scrambled.Substring(m.Index + m.Length);*/
                }
            }

            return _scrambled;
        }

        public void Add(Enum flag, Match m)
        {
            contentMatch cm = new contentMatch(flag, m);
            Add(cm.name, cm);
        }

        #region -----------  allocation  -------  [index -- koji Match zauzima koje karaktere]

        /// <summary>
        /// index -- koji Match zauzima koje karaktere
        /// </summary>
        // [XmlIgnore]
        [Category("contentMatchCollection")]
        [DisplayName("allocation")]
        [Description("index -- koji Match zauzima koje karaktere")]
        public Dictionary<int, contentMatch> allocation { get; set; } = new Dictionary<int, contentMatch>();

        #endregion -----------  allocation  -------  [index -- koji Match zauzima koje karaktere]

        /*
        public void Add(T flag, MatchCollection matches)
        {
            foreach (Match m in matches)
            {
                if (m.Success)
                {
                    Add(new KeyValuePair<T, Match>(flag, m));
                }
            }
        }*/
    }
}