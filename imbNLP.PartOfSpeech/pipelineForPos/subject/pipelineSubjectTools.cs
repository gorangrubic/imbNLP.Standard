// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineSubjectTools.cs" company="imbVeles" >
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
using imbACE.Network.tools;
using imbNLP.PartOfSpeech.flags.token;
using imbNLP.PartOfSpeech.pipeline.machine;
using imbSCI.Core.extensions.data;
using imbSCI.Data.collection.graph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.PartOfSpeech.pipelineForPos.subject
{
    /// <summary>
    ///
    /// </summary>
    public static class pipelineSubjectTools
    {
        public static String GetCleanCaseName(String properUrl)
        {
            String output = properUrl;

            if (output.ContainsAny(new String[] { ".", "/", ":" }))
            {
                domainAnalysis da = new domainAnalysis(properUrl);

                return da.domainName.Replace(".", "_");
            }
            else
            {
                return output;
            }
        }

        /// <summary>
        /// Returns first parent in ansestor line that is of specified type
        /// </summary>
        /// <typeparam name="T">Type that parent has to be</typeparam>
        /// <param name="source">The source node - to start from</param>
        /// <param name="depthLimit">Number of levels allowed for search</param>
        /// <returns>Parent of specified type or null if not found</returns>
        public static List<T> GetParentOfType<T, TSource>(this IEnumerable<TSource> source, Int32 depthLimit = 10)
            where T : class, IPipelineTaskSubject, IGraphNode
            where TSource : class, IPipelineTaskSubject, IGraphNode
        {
            List<T> output = new List<T>();

            foreach (TSource ts in source)
            {
                T parent = ts.GetParentOfType<T>(depthLimit);
                if (parent != null)
                {
                    if (!output.Contains(parent))
                    {
                        output.Add(parent);
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// Returns first parent in ansestor line that is of specified type
        /// </summary>
        /// <typeparam name="T">Type that parent has to be</typeparam>
        /// <param name="source">The source node - to start from</param>
        /// <param name="depthLimit">Number of levels allowed for search</param>
        /// <returns>Parent of specified type or null if not found</returns>
        public static T GetParentOfType<T>(this IPipelineTaskSubject source, Int32 depthLimit = 10) where T : class, IPipelineTaskSubject, IGraphNode
        {
            IPipelineTaskSubject head = source;

            Int32 i = 0;
            while (i < depthLimit)
            {
                head = head.parent as IPipelineTaskSubject;
                if (head is T)
                {
                    return head as T;
                }
                else if (head == null)
                {
                    return null;
                }
                i++;
            }
            return null;
        }

        private static Object copyListLock = new Object();

        /// <summary>
        /// Gets the type of the subject children token.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="levels">The levels.</param>
        /// <param name="unique">if set to <c>true</c> [unique].</param>
        /// <returns></returns>
        public static List<T> GetSubjectChildrenTokenType<T, TSource>(this IEnumerable<TSource> list, cnt_level[] levels, Boolean unique = true)
            where T : class, IPipelineTaskSubject, IGraphNode
            where TSource : class, IGraphNode
        {
            List<T> output = new List<T>();
            List<T> nextList = new List<T>();

            lock (copyListLock)
            {
                foreach (TSource s in list)
                {
                    nextList.AddUnique(s as T);
                }
            }

            List<T> known = new List<T>();

            while (nextList.Any())
            {
                List<T> cList = nextList.ToList();
                nextList = new List<T>();

                foreach (T l in cList)
                {
                    if (levels.Contains(l.contentLevelType))
                    {
                        if (!output.Contains(l) || !unique)
                        {
                            output.Add(l);
                        }
                    }
                    else
                    {
                        foreach (var ln in l)
                        {
                            T lnt = ln as T;
                            if (lnt != null)
                            {
                                if (known.Contains(lnt))
                                {
                                }
                                else
                                {
                                    nextList.AddUnique(lnt);
                                    known.Add(lnt);
                                }
                            }
                        }
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// To the type of the subject token.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="unique">if set to <c>true</c> [unique].</param>
        /// <returns></returns>
        public static List<T> ToSubjectTokenType<T>(this IEnumerable<IPipelineTaskSubject> list, Boolean unique = true) where T : class, IPipelineTaskSubject
        {
            List<T> output = new List<T>();

            foreach (IPipelineTaskSubject l in list)
            {
                T ln = l as T;

                if (ln != null)
                {
                    if (!unique || !output.Contains(ln))
                    {
                        output.Add(ln);
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// To the subject token.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="unique">if set to <c>true</c> [unique].</param>
        /// <returns></returns>
        public static List<pipelineTaskSubjectContentToken> ToSubjectToken(this IEnumerable<IPipelineTaskSubject> list, Boolean unique = true)
        {
            List<pipelineTaskSubjectContentToken> output = new List<pipelineTaskSubjectContentToken>();

            foreach (IPipelineTaskSubject l in list)
            {
                var ln = l as pipelineTaskSubjectContentToken;
                if (!output.Contains(ln) || !unique)
                {
                    output.Add(ln);
                }
            }

            return output;
        }

        /// <summary>
        /// Gets the subjects of level.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        public static List<T> GetSubjectsOfLevel<T>(this IEnumerable<T> source, cnt_level level) where T : IPipelineTaskSubject
        {
            List<T> MCStreams = new List<T>();
            foreach (T mcSubject in source)
            {
                if (mcSubject.contentLevelType == level) MCStreams.AddUnique(mcSubject);
            }
            return MCStreams;
        }

        /// <summary>
        /// Gets the subjects of level.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="levels">The levels.</param>
        /// <returns></returns>
        public static List<T> GetSubjectsOfLevel<T>(this IEnumerable<T> source, cnt_level[] levels) where T : IPipelineTaskSubject
        {
            List<T> MCStreams = new List<T>();
            foreach (T mcSubject in source)
            {
                if (levels.Contains(mcSubject.contentLevelType)) MCStreams.AddUnique(mcSubject);
            }
            return MCStreams;
        }
    }
}