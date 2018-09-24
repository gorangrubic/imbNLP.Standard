// --------------------------------------------------------------------------------------------------------------------
// <copyright file="contentCollectionBase.cs" company="imbVeles" >
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
namespace imbNLP.Core.contentStructure.collections
{
    #region imbVELES USING

    using imbACE.Core.core;
    using imbNLP.Core.contentStructure.interafaces;
    using imbNLP.Data.enums;
    using imbSCI.Data;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    #endregion imbVELES USING

    /// <summary>
    /// Osnova svih kolekcija contentStructure porodice - na pocetku je zamrznuta
    /// </summary>
    /// <typeparam name="T">contentElement</typeparam>
    /// <typeparam name="CT">Generic Type</typeparam>
    public class contentCollectionBase<T> : List<T>, IContentCollectionBase
        where T : class, INotifyPropertyChanged, IContentElement
    {
        public contentCollectionBase()
        {
        }

        #region IContentCollectionBase Members

        /// <summary>
        /// Osvezava vezu za prethodnim elementom -- i prijavljuje se kod njega kao sledeci element
        /// </summary>
        /// <param name="item"></param>
        public void Nest(IContentElement item)
        {
            int ps = IndexOf(item as T);
            if (ps > 0)
            {
                item.prev = this[ps - 1];
                item.prev.next = item;
            }
        }

        #endregion IContentCollectionBase Members

        public void Add(IEnumerable<T> items)
        {
            if (items != null)
            {
                foreach (T en in items)
                {
                    Add(en);
                }
            }
        }

        /// <summary>
        /// Specijalni algoritam dodavanja u kolekciju
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            //if (this.Contains(item))
            //{
            //}
            //if (this.Count > 0)
            //{
            //    if (this.Last().content == item.content)
            //    {
            //        return;
            //    }
            //    if (this.Any(x => x.content == item.content))
            //    {
            //    }
            //}
            base.Add(item);
        }

        internal IContentElement take(IContentElement qReference, contentRelationType qRelation)
        {
            switch (qRelation)
            {
                case contentRelationType.self:
                    return qReference;
                    break;

                case contentRelationType.next:
                    return qReference.next;
                    break;

                case contentRelationType.prev:
                    return qReference.next;
                    break;

                case contentRelationType.parent:
                    return qReference.parent;
                    break;
            }
            return null;
        }

        internal IContentElement takeIndex(IContentElement qReference, contentRelationType qRelation, int i, int limit = -1)
        {
            IContentElement hIndex;
            hIndex = take(qReference, qRelation);
            if (limit != -1)
            {
                if (i > limit)
                {
                    hIndex = null;
                }
            }
            return hIndex;
        }

        /// <summary>
        /// Vraca listu elemenata u skladu sa upitom
        /// </summary>
        /// <param name="qRelation"></param>
        /// <param name="qReference"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public contentElementList Query(contentRelationType qRelation, IContentElement qReference, int limit = -1)
        {
            contentElementList output = new contentElementList();
            int i = 0;
            IContentElement hIndex;

            switch (qRelation)
            {
                case contentRelationType.self:
                    output.AddNullSafe(qReference);
                    break;

                case contentRelationType.next:
                    output.AddNullSafe(qReference.next);
                    break;

                case contentRelationType.prev:
                    output.AddNullSafe(qReference.prev);
                    break;

                case contentRelationType.both:
                    output.AddNullSafe(qReference.next);
                    output.AddNullSafe(qReference.prev);
                    break;

                case contentRelationType.manyBoth:

                    break;

                case contentRelationType.manyNext:
                    hIndex = qReference;
                    do
                    {
                        hIndex = takeIndex(hIndex, contentRelationType.next, i, limit);
                        output.AddNullSafe(hIndex);
                        i++;
                    } while (hIndex != null);
                    break;

                case contentRelationType.manyPrev:
                    hIndex = qReference;
                    do
                    {
                        hIndex = takeIndex(hIndex, contentRelationType.prev, i, limit);
                        output.AddNullSafe(hIndex);
                        i++;
                    } while (hIndex != null);
                    break;

                case contentRelationType.parent:
                    output.AddNullSafe(qReference.parent);
                    break;

                case contentRelationType.manyParent:
                    hIndex = qReference;
                    do
                    {
                        hIndex = takeIndex(hIndex, contentRelationType.parent, i, limit);
                        output.AddNullSafe(hIndex);
                        i++;
                    } while (hIndex != null);
                    break;

                case contentRelationType.parentOneBefore:
                    hIndex = qReference.parent;
                    if (hIndex != null)
                    {
                        hIndex = take(hIndex, contentRelationType.prev);
                        output.AddNullSafe(hIndex);
                    }
                    break;

                case contentRelationType.parentOneAfter:
                    hIndex = qReference.parent;
                    if (hIndex != null)
                    {
                        hIndex = take(hIndex, contentRelationType.next);
                        output.AddNullSafe(hIndex);
                    }
                    break;

                case contentRelationType.parentManyBefore:
                    hIndex = qReference.parent;
                    do
                    {
                        hIndex = takeIndex(hIndex, contentRelationType.prev, i, limit);
                        output.AddNullSafe(hIndex);
                        i++;
                    } while (hIndex != null);
                    break;

                case contentRelationType.parentManyAfter:
                    hIndex = qReference.parent;
                    do
                    {
                        hIndex = takeIndex(hIndex, contentRelationType.next, i, limit);
                        output.AddNullSafe(hIndex);
                        i++;
                    } while (hIndex != null);
                    break;

                default:
                    logSystem.log("contentCollection Query :: contentRelationType not supported: [" + qRelation.ToString() + "]", logType.ExecutionError);
                    break;
            }
            return output;
        }

        public List<CF> Query<CF>(contentRelationQueryType qType, contentRelationType qRelation, IContentElement qReference, int limit = -1)
        {
            List<CF> output = new List<CF>();
            contentElementList elements = Query(qRelation, qReference, limit);
            foreach (IContentElement element in elements)
            {
                switch (qType)
                {
                    case contentRelationQueryType.gatherFlags:

                        IContentToken ict = element as IContentToken;
                        if (ict != null) output.AddRange(ict.flags.getEnumListFromFlags<CF>()); // output.populateWith(ict.flags);
                        break;

                    case contentRelationQueryType.gatherSentenceFlags:
                        IContentSentence ics = element as IContentSentence;
                        if (ics != null) output.AddRange(ics.sentenceFlags.getEnumListFromFlags<CF>()); // output.populateWith(ics.sentenceFlags);
                        break;

                    case contentRelationQueryType.gatherParagraphFlags:
                        IContentParagraph icp = element as IContentParagraph;
                        if (icp != null) output.AddRange(icp.flags.getEnumListFromFlags<CF>()); // output.populateWith(icp.flags);
                                                                                                // output.populateWith(icp.flags);
                        break;

                    case contentRelationQueryType.gatherBlockTags:
                        IContentBlock icb = element as IContentBlock;
                        if (icb != null) output.AddRange(icb.flags.getEnumListFromFlags<CF>()); // output.populateWith(icb.flags);
                        break;

                    case contentRelationQueryType.gatherOrigins:
                        throw new NotImplementedException("gatherOrigin");

                        //IContentToken icto = element as IContentToken;
                        //if (icto != null) output.Add((CF)icto.origin);
                        break;

                    default:
                        //output.populateWith(element.flags);
                        break;
                }
            }
            return output;
        }

        public List<string> Query(contentRelationQueryType qType, contentRelationType qRelation, IContentElement qReference, int limit = -1)
        {
            List<string> output = new List<string>();
            contentElementList elements = Query(qRelation, qReference, limit);
            foreach (IContentElement element in elements)
            {
                switch (qType)
                {
                    case contentRelationQueryType.gatherContents:
                        output.Add(element.content);
                        break;

                    default:
                        break;
                }
            }
            return output;
        }
    }
}