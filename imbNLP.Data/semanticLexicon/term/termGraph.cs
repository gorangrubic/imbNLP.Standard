// --------------------------------------------------------------------------------------------------------------------
// <copyright file="termGraph.cs" company="imbVeles" >
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
namespace imbNLP.Data.semanticLexicon.term
{
    using imbACE.Core.core;
    using imbNLP.Data.semanticLexicon.core;
    using imbSCI.Data;
    using imbSCI.Data.collection.graph;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class termGraph : graphWrapNode<termSparkArm>
    {
        /// <summary>
        /// Gets the spark from graph
        /// </summary>
        /// <returns></returns>
        public termSpark getSpark()
        {
            termSpark spark = new termSpark();
            spark.lemma = lemma;
            var alls = this.getAllChildren();
            List<string> added = new List<string>();
            foreach (graphWrapNode<termSparkArm> nd in alls)
            {
                if (nd.item.lexItem != null)
                {
                    if (!(nd.item.lexItem is IConcept))
                    {
                        if (!added.Contains(nd.item.lexItem.Id))
                        {
                            spark.Add(nd.item);
                            added.Add(nd.item.lexItem.Id);
                        }
                    }
                }
            }
            // spark.weight = spark.GetCWeight();
            return spark;
        }

        public ILexiconItem lexItem
        {
            get
            {
                if (item == null) return null;
                return item.lexItem;
            }
        }

        private TermLemma _lemma;

        /// <summary> </summary>
        public TermLemma lemma
        {
            get
            {
                return _lemma;
            }
            protected set
            {
                if (_lemma == null) _lemma = value;
            }
        }

        #region ----------- Boolean [ termNotFoundInLexicon ] -------  [Word was not found in the lexicon]

        /// <summary>
        /// Word was not found in the lexicon
        /// </summary>

        public bool termNotFoundInLexicon { get; set; } = false;

        #endregion ----------- Boolean [ termNotFoundInLexicon ] -------  [Word was not found in the lexicon]

        public termGraph(string __name)
        {
            name = __name;
            item = new termSparkArm(__name, null, 1);
        }

        public termGraph(ILexiconItem __lexItem)
        {
            item = new termSparkArm(__lexItem.Id, __lexItem, 1);
        }

        internal void getReady(List<ILexiconItem> __items = null)
        {
            if (item.lexItem == null)
            {
                List<ILexiconItem> items = __items;
                if (items == null) items = semanticLexiconManager.manager.getLexiconItems(name);
                if (items.Count() > 1)
                {
                    item = new termSparkArm(name + "[" + items.Count() + "]", null, 1);

                    foreach (ILexiconItem __lexItem in items)
                    {
                        Add(new termSparkArm(__lexItem.name, __lexItem, 1));
                        if (lemma == null)
                        {
                            if (__lexItem is TermLemma) lemma = (TermLemma)__lexItem;
                        }
                    }
                }
                else if (items.Count() == 1)
                {
                    ILexiconItem __lexItem = items.First();
                    if (lemma == null)
                    {
                        if (__lexItem is TermLemma) lemma = (TermLemma)__lexItem;
                    }
                    item = new termSparkArm(__lexItem.name, __lexItem, 1);
                }
                else
                {
                    termNotFoundInLexicon = true;
                    // item = new termSparkArm(name, null, 1);
                }
            }
        }

        protected List<graphWrapNode<termSparkArm>> getLeafs()
        {
            var stack = this.getAllLeafs();
            if (!stack.Any()) stack.Add(this);
            List<graphWrapNode<termSparkArm>> output = new List<graphWrapNode<termSparkArm>>();
            foreach (graphWrapNode<termSparkArm> s in stack) output.Add(s);
            return output;
        }

        public const int expandReachLimit = 50;

        private object ExpandLock = new object();

        /// <summary>
        /// Expands the specified steps.
        /// </summary>
        /// <param name="steps">The steps.</param>
        public void expand(int steps)
        {
            getReady();

            if (item == null) return;
            if (termNotFoundInLexicon) return;

            List<graphWrapNode<termSparkArm>> output = new List<graphWrapNode<termSparkArm>>();
            List<graphWrapNode<termSparkArm>> stack = new List<graphWrapNode<termSparkArm>>();
            List<ILexiconItem> known = new List<ILexiconItem>();

            stack.AddRange(getLeafs());
            int step_i = 1;

            while (stack.Any())
            {
                List<graphWrapNode<termSparkArm>> n_stack = new List<graphWrapNode<termSparkArm>>();

                foreach (graphWrapNode<termSparkArm> child in stack)
                {
                    lexiconItemExpandEnum reach = lexiconItemExpandEnum.upBelowLateral;
                    reach = lexiconItemExpandEnum.upBelowLateral | lexiconItemExpandEnum.conceptUp;

                    //if (child.item.lexItem.getItemTypeName()==nameof(Concept))
                    //{
                    //    graphWrapNode<termSparkArm> p = child.parent as graphWrapNode<termSparkArm>;

                    //    if (p.item.lexItem.getItemTypeName() != nameof(Concept))
                    //    {
                    //        ;
                    //    }
                    //}

                    List<ILexiconItem> exp = null;

                    //lock (ExpandLock) {
                    int ri = 0;
                    int rl = 10;
                    while (ri < rl)
                    {
                        try
                        {
                            exp = child.item.lexItem.expandOnce(reach, known);
                            ri = rl + 1;
                        }
                        catch (Exception ex)
                        {
                            ri++;
                            Thread.Sleep(100);
                            aceLog.log("Expand crashed (" + ex.Message + ") -- retry " + ri + " / " + rl);

                            Thread.SpinWait(100);
                        }
                    }
                    //}

                    foreach (ILexiconItem __lexItem in exp)
                    {
                        if (__lexItem != null)
                        {
                            graphWrapNode<termSparkArm> nd = null;

                            //if (child == this)
                            //{
                            //    nd = child.Add(new termSparkArm(__lexItem.name, __lexItem, 1));
                            //}
                            //else
                            //{
                            nd = child.Add(new termSparkArm(__lexItem.name, __lexItem, (1 / ((double)child.level + 1))));

                            //}

                            known.Add(nd.item.lexItem);

                            if (lemma == null)
                            {
                                if (__lexItem is TermLemma) lemma = (TermLemma)__lexItem;
                            }
                        }
                        if (known.Count() > expandReachLimit)
                        {
                            return;
                            break;
                        }
                    }

                    if (step_i < steps)
                    {
                        n_stack.AddRange(child);
                    }
                }

                step_i++;

                stack = n_stack;
            }
        }
    }
}