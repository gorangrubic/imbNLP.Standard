// --------------------------------------------------------------------------------------------------------------------
// <copyright file="contentParagraph.cs" company="imbVeles" >
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
namespace imbNLP.Core.contentStructure.elements
{
    #region imbVELES USING

    using imbNLP.Core.contentExtensions;
    using imbNLP.Core.contentPreprocess;
    using imbNLP.Core.contentStructure.collections;
    using imbNLP.Core.contentStructure.core;
    using imbNLP.Core.contentStructure.interafaces;
    using imbNLP.Data.basic;
    using imbNLP.Data.enums;
    using imbNLP.Data.enums.flags;
    using imbSCI.Core.attributes;
    using imbSCI.Core.extensions.data;
    using imbSCI.Data;
    using imbSCI.Data.interfaces;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text.RegularExpressions;

    #endregion imbVELES USING

    [imb(imbAttributeName.xmlNodeTypeName, "p")]
    [imb(imbAttributeName.xmlNodeValueProperty, "sourceContent")]
    public class contentParagraph : contentElementBase, IContentParagraph
    {
        public IEnumerator GetEnumerator()
        {
            if (items == null) return null;

            return items.GetEnumerator();
        }

        public int indexOf(IObjectWithChildSelector child)
        {
            if (items == null) return -1;

            return items.IndexOf(child as IContentSentence);
        }

        public int Count()
        {
            if (items == null) return 0;

            return items.Count;
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public object this[int key]
        {
            get
            {
                if (items == null) return null;

                return items[key];
            }
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified child name.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="childName">Name of the child.</param>
        /// <returns></returns>
        public object this[string childName]
        {
            get
            {
                foreach (IContentElement ch in items)
                {
                    if (ch.name == childName)
                    {
                        return ch;
                    }
                }
                return null;
            }
        }

        public bool ContainsOneOrMore(contentRelationType qRelation, int limit, params contentParagraphFlag[] _flgs)
        {
            return items.Query<contentParagraphFlag>(contentRelationQueryType.gatherFlags, qRelation, this, limit).ContainsOneOrMore(_flgs);
        }

        public bool ContainsAll(contentRelationType qRelation, int limit, params contentParagraphFlag[] _flgs)
        {
            return items.Query<contentParagraphFlag>(contentRelationQueryType.gatherFlags, qRelation, this, limit).ContainsAll(_flgs);
        }

        public bool ContainsOnly(contentRelationType qRelation, int limit, params contentParagraphFlag[] _flgs)
        {
            return items.Query<contentParagraphFlag>(contentRelationQueryType.gatherFlags, qRelation, this, limit).ContainsOnly(_flgs);
        }

        public contentParagraph() : base()
        {
            items = new contentSentenceCollection();
        }

        public contentParagraph(string __content, IContentElement __parent)
        {
            items = new contentSentenceCollection();
            content = __content;
            parent = __parent;
        }

        /// <summary>
        /// Regex select sentenceTerminator : ([\.;!\?]+)$
        /// </summary>
        /// <remarks>
        /// <para>Selektuje karaktere koji predstavljaju kraj recenice</para>
        /// <para></para>
        /// </remarks>
        public static Regex _select_sentenceTerminator = new Regex(@"([\.;!\?]+)$", RegexOptions.Compiled);

        /// <summary>
        /// Regex select sentenceSpliter :
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_sentenceSpliter = new Regex(@"(?<=[\.;!\?])\s*(?=[A-ZČŠĆŽĐ\d])",
                                                                RegexOptions.Compiled);

        #region IContentParagraph Members

        IContentCollectionBase IContentElement.items
        {
            get { return _items; }
        }

        public override void primaryFlaging(params object[] resources)
        {
            items.ForEach(x => x.primaryFlaging(resources));

            if (items.Count == 1)
            {
                IContentSentence sen = (IContentSentence)items[0];
                if (sen.sentenceFlags.HasFlag(contentSentenceFlag.title))
                {
                    flags |= contentParagraphFlag.heading;
                    if (parent is IContentBlock)
                    {
                        IContentBlock bl = parent as IContentBlock;
                        bl.title += sen.content;
                    }
                }
            }
            // throw new System.NotImplementedException();
        }

        public override void secondaryFlaging(params object[] resources)
        {
            items.ForEach(x => x.secondaryFlaging(resources));
            // throw new System.NotImplementedException();
        }

        public override void generalSemanticsFlaging(params object[] resources)
        {
            // throw new NotImplementedException();
        }

        public override void specialSematicsFlaging(params object[] resources)
        {
            //  throw new NotImplementedException();
        }

        #endregion IContentParagraph Members

        #region --- items ------- Skup svih recenica

        private contentSentenceCollection _items = new contentSentenceCollection();

        /// <summary>
        /// Skup svih recenica
        /// </summary>
       // [imb(imbAttributeName.xmlEntityOutput)]
        public contentSentenceCollection items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged("items");
            }
        }

        #endregion --- items ------- Skup svih recenica

        #region -----------  flags  -------  [Description of $property$]

        private contentParagraphFlag _flags;

        /// <summary>
        /// Description of $property$
        /// </summary>
        // [XmlIgnore]
        [Category("contentParagraph")]
        [DisplayName("flags")]
        [Description("Description of $property$")]
        [imb(imbAttributeName.xmlEntityOutput)]
        public contentParagraphFlag flags
        {
            get { return _flags; }
            set
            {
                // Boolean chg = (_flags != value);
                _flags = value;
                OnPropertyChanged("flags");
                // if (chg) {}
            }
        }

        #endregion -----------  flags  -------  [Description of $property$]

        internal virtual List<string> splitContentToSentences(string input)
        {
            List<string> inputSentences = new List<string>();

            if (_select_sentenceSpliter.IsMatch(input))
            {
                // ima vise recenica
                string[] _ins = _select_sentenceSpliter.Split(input);
                foreach (var s in _ins) inputSentences.Add(s.Trim());
            }
            else
            {
                // postoji samo jedna recenica
                inputSentences.Add(input.Trim());
            }
            return inputSentences;
        }

        /// <summary>
        /// GLAVNA KOMANDA KOD TOKENIZACIJE - Za prosledjen paragraph pravi recenice, podrecenice i tokene. Vrsi registrovanje tokena i recenica u IContentPage output-u ako bude prosledjen
        /// </summary>
        /// <typeparam name="TSentence">Tip za recenice</typeparam>
        /// <typeparam name="TSubSentence">Tip za pod recenice</typeparam>
        /// <typeparam name="TToken">Tip za tokene</typeparam>
        /// <param name="paragraph"></param>
        /// <param name="resources">IContentPage za registraciju sadrzaja;  paragraphDetectionFlags; sentenceDetectionFlags; contentPreprocessFlags;tokenDetectionFlags;tokenDetectionFlags</param>
        public virtual void setParagraphFromContent<TSentence, TSubSentence, TToken>(params object[] resources)
            where TSentence : IContentSentence, new()
            where TSubSentence : IContentSubSentence, new()
            where TToken : class, IContentToken, new()
        {
            IContentPage output = resources.getFirstOfType<IContentPage>();

            basicLanguage basicLanguages = resources.getFirstOfType<basicLanguage>();
            if (basicLanguages == null) basicLanguages = new basicLanguage();

            // IContentBlock block = resources.getOfType<IContentBlock>();

            paragraphDetectionFlag flags = resources.getFirstOfType<paragraphDetectionFlag>();
            sentenceDetectionFlag sentenceFlags = resources.getFirstOfType<sentenceDetectionFlag>();
            contentPreprocessFlag preprocessFlags = resources.getFirstOfType<contentPreprocessFlag>();
            //  subsentenceDetectionFlags subsentenceFlags = new subsentenceDetectionFlags(resources);
            tokenDetectionFlag tokenFlags = resources.getFirstOfType<tokenDetectionFlag>(); // new tokenDetectionFlags(resources);

            contentSentenceCollection snt = _setSentencesFromContent<TSentence>(sentenceFlags, preprocessFlags);
            // sentenceDetection._setSentencesFromContent<TSentence>(paragraph, sentenceFlags, preprocessFlags);

            foreach (TSentence sn in snt)
            {
                // sn._setTokensForSentence<TSubSentence>(sentenceFlags, tokenFlags);
                var tkns = sn.setTokensFromContent<TToken, TSubSentence>(flags, sentenceFlags, preprocessFlags,
                                                                         tokenFlags, resources, basicLanguages);

                //tokenDetection.setTokensFromContent<TToken, TSubSentence>(sn, subsentenceFlags, tokenFlags);

                if (flags.HasFlag(paragraphDetectionFlag.dropSentenceWithNoToken))
                {
                    if (sn.items.Count == 0)
                    {
                        continue;
                    }
                }
                if (sentenceFlags.HasFlag(sentenceDetectionFlag.setSentenceToParagraph))
                {
                    setItem(sn);
                }

                //if (output != null)
                //{
                //    output.sentences.Add(sn);
                //    output.tokens.CollectAll(sn.items);
                //}
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TSentence"></typeparam>
        /// <param name="resources"></param>
        /// <returns></returns>
        protected virtual contentSentenceCollection _setSentencesFromContent<TSentence>(params object[] resources)
            where TSentence : IContentSentence, new()
        {
            string input = content;

            sentenceDetectionFlag flags = resources.getFirstOfType<sentenceDetectionFlag>(); //new sentenceDetectionFlags(resources);
            contentPreprocessFlag preprocessFlags = resources.getFirstOfType<contentPreprocessFlag>(); // new contentPreprocessFlags(resources);
            contentSentenceCollection output = new contentSentenceCollection();

            // preuzima parent page ako je prosledjen
            IContentPage parentPage = resources.getFirstOfType<IContentPage>();

            if (flags.HasFlag(sentenceDetectionFlag.preprocessParagraphContent))
                input = preprocess.process(input, preprocessFlags);

            List<string> inputSentences = splitContentToSentences(input);

            foreach (string _inputSentece in inputSentences)
            {
                TSentence newSentence = new TSentence();
                newSentence.sourceContent = _inputSentece;
                newSentence.content = _inputSentece;
                if (_select_sentenceTerminator.IsMatch(_inputSentece))
                {
                    newSentence.sentenceFlags |= contentSentenceFlag.regular;
                    Match m = _select_sentenceTerminator.Match(_inputSentece);
                    if (m.Success)
                    {
                        newSentence.spliter = m.Value;
                        newSentence.content = _inputSentece.Substring(0,
                                                                      _inputSentece.Length - newSentence.spliter.Length);
                    }
                }
                else
                {
                    newSentence.sentenceFlags |= contentSentenceFlag.inregular;
                }
                output.Add(newSentence);
            }

            return output;
        }

        public override List<Enum> GetFlags()
        {
            List<Enum> output = new List<Enum>();

            foreach (Enum fl in flags.getEnumListFromFlags())
            {
                output.Add(fl as Enum);
            }

            return output;
        }
    }
}