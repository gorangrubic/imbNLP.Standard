// --------------------------------------------------------------------------------------------------------------------
// <copyright file="contentSentence.cs" company="imbVeles" >
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

    using imbACE.Core.core;
    using imbACE.Core.core.exceptions;
    using imbNLP.Core.contentExtensions;
    using imbNLP.Core.contentPreprocess;
    using imbNLP.Core.contentStructure.collections;
    using imbNLP.Core.contentStructure.core;
    using imbNLP.Core.contentStructure.interafaces;
    using imbNLP.Data.enums;
    using imbNLP.Data.enums.flags;
    using imbSCI.Core.attributes;
    using imbSCI.Core.collection;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.extensions.text;
    using imbSCI.Data;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;
    using System.Text.RegularExpressions;

    #endregion imbVELES USING

    /// <summary>
    /// 2013c: LowLevel resurs
    /// </summary>
    [imb(imbAttributeName.xmlNodeTypeName, "s")]
    public class contentSentence : contentToken, IContentSentence, IContentToken // contentElementBase
    {
        public contentSentence()
        {
            items = new contentTokenCollection();
        }

        //public const Boolean passSubSentenceTokens = true;

        #region STATIC PART

        public const string sentenceEnd_normal = ".";
        public const string sentenceEnd_exclamation = "!";
        public const string sentenceEnd_question = "?";
        public const string sentenceEnd_arrowRight = ">";
        public const string sentenceEnd_arrowLeft = "<";
        public const string sentenceEnd_listStart = ":";
        public const string sentenceEnd_listStart2 = ".:";
        public const string sentenceEnd_listItemEnd = ";";
        public const string sentenceEnd_listItemEnd_listEnd = ";...";

        public const string sentenceEnd_notFinished = "...";

        public const string sentenceEnd_notFinished2 = "....";

        public bool ContainsOneOrMore(contentRelationType qRelation, int limit, params contentTokenFlag[] _flgs)
        {
            return items.Query<contentTokenFlag>(contentRelationQueryType.gatherFlags, qRelation, this, limit).ContainsOneOrMore(_flgs);
        }

        public bool ContainsAll(contentRelationType qRelation, int limit, params contentTokenFlag[] _flgs)
        {
            return items.Query<contentTokenFlag>(contentRelationQueryType.gatherFlags, qRelation, this, limit).ContainsAll(_flgs);
        }

        public bool ContainsOnly(contentRelationType qRelation, int limit, params contentTokenFlag[] _flgs)
        {
            return items.Query<contentTokenFlag>(contentRelationQueryType.gatherFlags, qRelation, this, limit).ContainsOnly(_flgs);
        }

        public bool ContainsOneOrMore(contentRelationType qRelation, int limit, params contentSentenceFlag[] _flgs)
        {
            return items.Query<contentSentenceFlag>(contentRelationQueryType.gatherFlags, qRelation, this, limit).ContainsOneOrMore(_flgs);
        }

        public bool ContainsAll(contentRelationType qRelation, int limit, params contentSentenceFlag[] _flgs)
        {
            return items.Query<contentSentenceFlag>(contentRelationQueryType.gatherFlags, qRelation, this, limit).ContainsAll(_flgs);
        }

        public bool ContainsOnly(contentRelationType qRelation, int limit, params contentSentenceFlag[] _flgs)
        {
            return items.Query<contentSentenceFlag>(contentRelationQueryType.gatherFlags, qRelation, this, limit).ContainsOnly(_flgs);
        }

        /// <summary>
        /// Regex select tokenWithSplitter : ([\w\d\.,:;\"-|]{1,})\b[\W]{0,3}
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_tokenWithSplitter = new Regex(@"([\w\d\.,\\\/:;\""\-_\|]{1,})\b[\W]{0,3}",
                                                                  RegexOptions.Compiled);

        /// <summary>
        /// Regex select tokenWithoutSplitter : ([\w\d\.,:;\""-|]{1,})\b
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_tokenWithoutSplitter = new Regex(@"([\w\d\.,\\\/:;\""\-_\|]{1,})\b",
                                                                     RegexOptions.Compiled);

        #endregion STATIC PART

        #region -----------  sentenceFlags  -------  [flagovi o recenici]

        private contentSentenceFlag _sentenceFlags;

        /// <summary>
        /// flagovi o recenici
        /// </summary>
        // [XmlIgnore]
        [Category("contentSubSentence")]
        [DisplayName("sentenceFlags")]
        [Description("flagovi o recenici")]
        public contentSentenceFlag sentenceFlags
        {
            get { return _sentenceFlags; }
            set
            {
                // Boolean chg = (_sentenceFlags != value);
                _sentenceFlags = value;
                OnPropertyChanged("sentenceFlags");
                // if (chg) {}
            }
        }

        #endregion -----------  sentenceFlags  -------  [flagovi o recenici]

        /// <summary>
        /// Gets the flags.
        /// </summary>
        /// <returns></returns>
        public override List<Enum> GetFlags()
        {
            List<Enum> output = new List<Enum>();

            foreach (Enum fl in flags.getEnumListFromFlags())
            {
                output.Add(fl);
            }

            foreach (Enum fl in detectionFlags.getEnumListFromFlags())
            {
                output.Add(fl);
            }

            foreach (Enum fl in sentenceFlags.getEnumListFromFlags())
            {
                output.Add(fl);
            }

            return output;
        }

        #region IContentSentence Members

        public override void primaryFlaging(params object[] resources)
        {
            items.ForEach(x => x.primaryFlaging(resources));

            if (content.isUpperCase())
            {
                sentenceFlags |= contentSentenceFlag.caseCapital;
            }
            else if (content.isSentenceCase())
            {
                sentenceFlags |= contentSentenceFlag.caseSentence;
            }
            else if (content.isSentenceFragmentCase())
            {
                sentenceFlags |= contentSentenceFlag.caseFragment;
            }

            switch (spliter)
            {
                case sentenceEnd_listStart2:
                case sentenceEnd_listStart:
                case sentenceEnd_arrowRight:
                case sentenceEnd_arrowLeft:
                    sentenceFlags |= contentSentenceFlag.pointing;
                    break;

                case sentenceEnd_notFinished2:
                case sentenceEnd_notFinished:
                    sentenceFlags |= contentSentenceFlag.unfinished;
                    break;

                case sentenceEnd_question:
                    sentenceFlags |= contentSentenceFlag.question;
                    break;

                case sentenceEnd_normal:
                    if (items.Count > 1)
                    {
                        sentenceFlags |= contentSentenceFlag.normal;
                    }

                    break;

                case sentenceEnd_listItemEnd_listEnd:
                case sentenceEnd_listItemEnd:
                    sentenceFlags |= contentSentenceFlag.itemInListLast;
                    break;

                case sentenceEnd_exclamation:
                    sentenceFlags |= contentSentenceFlag.exclamation;
                    break;

                case "":
                    if (sentenceFlags.HasFlag(contentSentenceFlag.caseCapital))
                    {
                        sentenceFlags |= contentSentenceFlag.titleStrongCase & contentSentenceFlag.title;
                    }
                    else if (sentenceFlags.HasFlag(contentSentenceFlag.caseSentence))
                    {
                        sentenceFlags |= contentSentenceFlag.titleSoftCase & contentSentenceFlag.title;
                    }
                    else if (sentenceFlags.HasFlag(contentSentenceFlag.caseFragment))
                    {
                        sentenceFlags |= contentSentenceFlag.item;
                    }
                    break;

                default:

                    if (!string.IsNullOrEmpty(spliter))
                    {
                        //devNoteManager.note(this, devNoteType.nlp, "Unknown spliter for sentence: [" + spliter +
                        //                                           "] - add support for it in> cs constants and sentenceAnalysis()",
                        //                    "blokCategorization");
                    }
                    break;
            }
        }

        public override void secondaryFlaging(params object[] resources)
        {
            // operacije nakon od gore ka dole

            items.ForEach(x => x.secondaryFlaging(resources));
        }

        public override void generalSemanticsFlaging(params object[] resources)
        {
            //throw new NotImplementedException();
        }

        public override void specialSematicsFlaging(params object[] resources)
        {
            //throw new NotImplementedException();
        }

        IContentCollectionBase IContentElement.items
        {
            get { return _items; }
        }

        /// <summary>
        /// Glavni metod za obradu sadrzaja jedne recenice >> prvo poziva setSubSentences, zatim setTokensForSentence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TS"></typeparam>
        /// <param name="resources"> tokenDetectionFlags flags, contentTokenCollection contentTokenCollections</param>
        /// <returns></returns>
        public virtual contentTokenCollection setTokensFromContent<T, TS>(params object[] resources)
            where T : class, IContentToken, new()
            where TS : IContentSubSentence, new()
        {
            //logSystem.log("set tokens from content Sentence: " + sentence.content, logType.Notification);
            IContentSentence sentence = this;

            tokenDetectionFlag detection_flags = resources.getFirstOfType<tokenDetectionFlag>(); // new tokenDetectionFlags();

            contentTokenCollection tokenCollections = resources.getFirstOfType<contentTokenCollection>();
            if (tokenCollections == null) tokenCollections = new contentTokenCollection();

            contentMatchCollection subsentenceMatches = _setSubSentences<TS>(detection_flags, null);

            try
            {
                int subCount = 0;
                for (int dti = 0; dti < subsentenceMatches.Count; dti++)
                {
                    contentMatch dt = subsentenceMatches[subsentenceMatches.Keys.imbGetItemAt(dti).ToString()]; // subsentenceMatches[dti];

                    contentSubSentence ss = dt.element as contentSubSentence;

                    contentTokenCollection subtkns = new contentTokenCollection();
                    //var cs = ss._setTokensForSentence<T>(subtkns, subsentenceMatches, flags);
                    var cs = ss._setTokensForSentence<T>(subtkns, detection_flags);
                    //var cs = ss._setTokensForSentence<T>(tokenCollections, flags);
                    //var cs = tokenCollectionsss._set
                    //var cs = ss._setTokensForSentence<T>(flags);
                    for (int ci = 0; ci < cs.Count; ci++)
                    {
                        ss.setItem(cs[ci]);
                    }

                    //cs = ss._setTokensForSentence<T>(subtkns, subsentenceMatches);
                    // ss.items.AddRange(cs);

                    //  contentTokenCollection subtkns = ss.setTokensFromContent<T>(resources);

                    //ss.items.Add(ss);
                    //foreach (T sst in ss.items)
                    //{
                    //    tokenCollections.Add(sst);
                    //}
                    //tokenCollections.Add(ss);
                    //dt.element = ss;

                    //  subCount++;
                }

                List<IContentToken> directTokens = new List<IContentToken>();

                directTokens = _setTokensForSentence<T>(subsentenceMatches, detection_flags, tokenCollections, directTokens);

                if (directTokens != tokenCollections)
                {
                    for (int dti = 0; dti < directTokens.Count; dti++)
                    {
                        IContentToken dt = directTokens[dti];

                        T tkn = dt as T;
                        if (tkn != null)
                        {
                            tokenCollections.Add(tkn);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isb = new StringBuilder();
                isb.AppendLine("tokenDetection error");
                isb.AppendLine("Target is: " + sentence.toStringSafe());
                throw;
                // devNoteManager.note(sentence, ex, isb.ToString(), "tokenDetection", devNoteType.tokenization);
            }

            foreach (var tk in tokenCollections)
            {
                //subsentenceMatches.allocated(tk.)
                setItem(tk);
            }

            // logSystem.log("set tokens from content Sentence done", logType.Notification);
            return tokenCollections;
        }

        #endregion IContentSentence Members

        #region --- items ------- svi tokeni u recenici

        //  private contentTokenCollection _items = new contentTokenCollection();

        /// <summary>
        /// svi tokeni u recenici
        /// </summary>

        public contentTokenCollection items
        {
            get
            {
                if (_items == null) _items = new contentTokenCollection();
                return _items as contentTokenCollection;
            }
            set
            {
                _items = value;
                OnPropertyChanged("items");
            }
        }

        #endregion --- items ------- svi tokeni u recenici

        #region --- flags ------- flagovi

        private contentTokenFlag _flags = new contentTokenFlag();

        /// <summary>
        /// flagovi
        /// </summary>
        [imb(imbAttributeName.xmlEntityOutput)]
        public contentTokenFlag flags
        {
            get { return _flags; }
            set
            {
                _flags = value;
                OnPropertyChanged("flags");
            }
        }

        #endregion --- flags ------- flagovi

        #region -----------  detectionFlags  -------  [flagovi dobijeni tokom detektovanja]

        private tokenDetectionFlag _detectionFlags;

        /// <summary>
        /// flagovi dobijeni tokom detektovanja
        /// </summary>
        // [XmlIgnore]
        [Category("contentSentence")]
        [DisplayName("detectionFlags")]
        [Description("flagovi dobijeni tokom detektovanja")]
        [imb(imbAttributeName.xmlEntityOutput)]
        public tokenDetectionFlag detectionFlags
        {
            get { return _detectionFlags; }
            set
            {
                // Boolean chg = (_detectionFlags != value);
                _detectionFlags = value;
                OnPropertyChanged("detectionFlags");
                // if (chg) {}
            }
        }

        #endregion -----------  detectionFlags  -------  [flagovi dobijeni tokom detektovanja]

        /// <summary>
        /// Može da sam izvrši macroTokens ili da dobije gotove. Primenjuje subsentence algoritam i vrši standardnu detekciju tokena --- NAJBITNIJE JE STO SLAZE TOKENE/SUBSENTENCE u parent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resources"></param>
        /// <returns></returns>
        public virtual contentTokenCollection _setTokensForSentence<T>(params object[] resources)
            where T : IContentToken, new()
        {
            contentMatchCollection subsentenceMatches = resources.getFirstOfType<contentMatchCollection>();
            if (subsentenceMatches == null) subsentenceMatches = new contentMatchCollection();

            contentMatchCollection macroTokens = null;
            if (macroTokens == null) macroTokens = _setMacroTokensForSentence<T>(resources);

            contentTokenCollection output = resources.getFirstOfType<contentTokenCollection>();

            tokenDetectionFlag flags = resources.getFirstOfType<tokenDetectionFlag>();

            if (flags.HasFlag(tokenDetectionFlag.standardDetection))
                macroTokens.Add(_select_tokenWithSplitter, tokenDetectionFlag.standardDetection);

            string source = "";
            source = content;

            int i = 0;
            int mx = source.Length;
            int sI = 0;
            int sLimit = mx;
            DateTime processStart = DateTime.Now;

            while (i < mx)
            {
                try
                {
                    if (sI > sLimit)
                    {
                        aceLog.log("Content sentence tokenization broken");
                        break;
                    }
                    sI++;

                    #region LOOP

                    oneOrMore<contentMatch> cms = macroTokens.allocated(i, 1);

                    if (cms == null)
                    {
                        i = mx;
                        continue;
                    }

                    if (cms.isNothing)
                    {
                        i++;
                        continue;
                    }
                    else
                    {
                        contentMatch cm = cms.First();
                        if (cm == null)
                        {
                            i++;
                            continue;
                        }

                        i = i + cm.match.Length;

                        IContentToken newToken = new T();
                        string mch = cm.match.Value.Trim("#".ToCharArray());
                        newToken.sourceContent = mch;
                        newToken.content = mch;

                        Match sp = _select_tokenSplitter.Match(mch);
                        if (sp.Success)
                        {
                            newToken.spliter = sp.Value;
                            newToken.content = newToken.content.removeEndsWith(newToken.spliter).Trim();
                        }
                        else
                        {
                            //if (cm.match.Groups.Count > 1)
                            //{
                            //    mch = cm.match.Groups[1].Value;
                            //}
                            //else
                            //{
                            //}
                            newToken.spliter = "";
                            newToken.content = mch.Trim();
                        }

                        if (DateTime.Now.Subtract(processStart).Minutes > 2)
                        {
                            aceLog.log("TOKENIZATION TIME LIMIT BROKEN !!!");
                            break;
                        }

                        IContentSentence _sentence = this;

                        if (cm.element is IContentSubSentence)
                        {
                            IContentSubSentence sub = cm.element as IContentSubSentence;
                            sub.masterToken = newToken;
                            newToken = (IContentToken)cm.element;
                        }

                        /*
                        if (subsentenceMatches.isAllocated(cm.match.Index, cm.match.Length))
                        {
                            oneOrMore<contentMatch> subcms = subsentenceMatches.allocated(cm.match.Index, cm.match.Length);
                            contentMatch subcm = subcms.First();
                            if (subcm == null)
                            {
                                // logSystem.log("    -- -- -- sub cm is null  ", logType.Notification);
                            }
                            else
                            {
                                if (subcm.element != null)
                                {
                                }
                                _sentence = subcm.element as IContentSubSentence;

                                if (_sentence != null)
                                {
                                    IContentSubSentence _subSentence = _sentence as IContentSubSentence;
                                    newToken.flags.Add(contentTokenFlag.subsentence_inner);
                                    _subSentence.masterToken = newToken;
                                    newToken = (T)(_subSentence as IContentToken);

                                    //_sentence.setItem(newToken);

                                    //if (output.Contains(_sentence as IContentToken))
                                    //{
                                    //}
                                    //else
                                    //{
                                    //     output.Add(_sentence as IContentToken);
                                    //}
                                }
                                else
                                {
                                   // output.Add(newToken);
                                }
                            }
                        }*/

                        /*
                        if (_sentence != null)
                        {
                            //setItem(_sentence);

                            if (_sentence == this)
                            {
                                output.Add(newToken);
                            } else
                            {
                                setItem(newToken);
                                if (output.Contains(_sentence as IContentToken))
                                {
                                }
                                else
                                {
                                   // output.Add(_sentence as IContentToken);
                                }
                            }
                        }
                        else
                        {
                            setItem(newToken);
                        }
                        */
                        if (cm.associatedKey != null)
                        {
                            tokenDetectionFlag fl = tokenDetectionFlag.none;
                            bool detected = Enum.TryParse(cm.associatedKey.toStringSafe(), true, out fl);
                            newToken.detectionFlags = fl;
                        }
                        if (output.Contains(newToken))
                        {
                        }
                        else
                        {
                            if (newToken == this)
                            {
                            }
                            else
                            {
                                output.Add(newToken);
                            }
                        }
                    }

                    #endregion LOOP
                }
                catch (Exception ex)
                {
                    var isb = new StringBuilder();
                    isb.AppendLine("loop error error");
                    isb.AppendLine("Target is: i=" + i + "[mx=" + mx + "]");
                    throw new aceGeneralException(isb.ToString(), null, this, "Loop");
                    // devNoteManager.note(ex, isb.ToString(), "loop error", devNoteType.tokenization);
                }
            }

            return output;
        }

        /// <summary>
        /// Izdvaja makro tokene unutar recenice ili pod recenice
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resources"></param>
        /// <returns></returns>
        internal virtual contentMatchCollection _setMacroTokensForSentence<T>(params object[] resources)
            where T : IContentToken, new()
        {
            contentMatchCollection macroTokens = resources.getFirstOfType<contentMatchCollection>();

            if (macroTokens == null) macroTokens = new contentMatchCollection();
            List<tokenDetectionFlag> flist = flags.getEnumListFromFlags<tokenDetectionFlag>();

            string scrambled = content;
            macroTokens.scrambled = scrambled;

            foreach (tokenDetectionFlag fl in flist)
            {
                switch (fl)
                {
                    case tokenDetectionFlag.emailAddress:
                        macroTokens.Add(imbStringSelect._select_isEmailAddress, fl);
                        break;

                    case tokenDetectionFlag.phonenumber:
                        macroTokens.Add(imbStringSelect._select_isPhoneNumber, fl);
                        break;

                    case tokenDetectionFlag.standard:
                        macroTokens.Add(imbStringSelect._select_isStandards, fl);
                        break;
                }
            }

            foreach (tokenDetectionFlag fl in flist)
            {
                switch (fl)
                {
                    case tokenDetectionFlag.yearNumber:
                        macroTokens.Add(imbStringSelect._select_isYearNumber, fl);
                        break;

                    case tokenDetectionFlag.postOfficeNumber:
                        macroTokens.Add(imbStringSelect._select_isPostOfficeNumber, fl);
                        break;
                }
            }

            foreach (tokenDetectionFlag fl in flist)
            {
                switch (fl)
                {
                    case tokenDetectionFlag.acronims:
                        macroTokens.Add(imbStringSelect._select_isAcronimIrregular, fl);
                        macroTokens.Add(imbStringSelect._select_isAcronimByLength, fl);
                        break;
                }
            }

            return macroTokens;
        }

        /// <summary>
        /// Postavlja pod recenice -- proslediti> tokenDetectionFlags
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resources"> tokenDetectionFlags subflags, contentMatchCollection subsentenceMatches</param>
        /// <returns></returns>
        internal virtual contentMatchCollection _setSubSentences<T>(tokenDetectionFlag subflags, contentMatchCollection subsentenceMatches)
            where T : IContentSubSentence, new()
        {
            // List<T> output = new List<T>();
            // logSystem.log("-- set sub sentences for: " + sentence.content, logType.Notification);
            // tokenDetectionFlags subflags = new tokenDetectionFlags(resources);

            // contentMatchCollection subsentenceMatches = resources.getOfType<contentMatchCollection>();
            if (subsentenceMatches == null) subsentenceMatches = new contentMatchCollection();

            string scrambled = content;
            subsentenceMatches.scrambled = scrambled;

            var subflist = subflags.getEnumListFromFlags();

            foreach (tokenDetectionFlag fl in subflist)
            {
                switch (fl)
                {
                    case tokenDetectionFlag.enbracedSubSentences:
                        subsentenceMatches.Add(imbStringIsTests._select_isEnbracedSubSentence, fl);
                        break;

                    case tokenDetectionFlag.enumerationSubSentences:
                        subsentenceMatches.Add(imbStringIsTests._select_isEnumerationSubSentence, fl);
                        break;

                    case tokenDetectionFlag.quotationSubSentences:
                        subsentenceMatches.Add(imbStringIsTests._select_isQuotedSubSentence, fl);
                        break;
                }
            }

            foreach (tokenDetectionFlag fl in subflist)
            {
                switch (fl)
                {
                    case tokenDetectionFlag.potentialStreetAndNumber:
                        subsentenceMatches.Add(imbStringIsTests._select_isPotentialStreetAndNumber, fl);
                        break;
                }
            }

            foreach (tokenDetectionFlag fl in subflist)
            {
                switch (fl)
                {
                    case tokenDetectionFlag.potentialPersonalNamesSubSentences:
                        subsentenceMatches.Add(imbStringIsTests._select_isPotentialPersonalNamePair, fl);
                        break;

                    case tokenDetectionFlag.cityAndPostnumberSubSentences:
                        subsentenceMatches.Add(imbStringIsTests._select_isIsPotentialCityAndPost, fl);
                        break;
                }
            }

            foreach (tokenDetectionFlag fl in subflist)
            {
                switch (fl)
                {
                    case tokenDetectionFlag.punctationSubSentences:
                        subsentenceMatches.Add(imbStringIsTests._select_isInnerSentence, fl);
                        break;
                }
            }

            foreach (contentMatch cm in subsentenceMatches.Values)
            {
                T subsentence = new T();
                subsentence.parent = this;
                subsentence.content = cm.match.Value;
                subsentence.sourceContent = cm.match.Value;

                subsentence.match = cm;

                //  subsentence.detectionFlags.Add((tokenDetectionFlag) cm.associatedKey);

                switch ((tokenDetectionFlag)cm.associatedKey)
                {
                    case tokenDetectionFlag.enbracedSubSentences:
                        subsentence.flags = subsentence.flags.Add(contentTokenFlag.subsentence);
                        break;

                    case tokenDetectionFlag.enumerationSubSentences:
                        subsentence.flags = subsentence.flags.Add(contentTokenFlag.subsentence_enumeration);
                        break;

                    case tokenDetectionFlag.quotationSubSentences:
                        subsentence.flags = subsentence.flags.Add(contentTokenFlag.subsentence_quoted);
                        break;

                    case tokenDetectionFlag.cityAndPostnumberSubSentences:
                        subsentence.flags = subsentence.flags.Add(contentTokenFlag.subsentence_information);
                        break;

                    case tokenDetectionFlag.punctationSubSentences:

                        //subsentence.content = cm.match.Groups[2].Value;
                        subsentence.flags = subsentence.flags.Add(contentTokenFlag.subsentence);
                        break;

                    case tokenDetectionFlag.potentialPersonalNamesSubSentences:
                        subsentence.flags = subsentence.flags.Add(contentTokenFlag.subsentence_information);
                        break;
                }

                cm.element = subsentence;

                //  setItem(subsentence);
            }

            content = scrambled;
            // logSystem.log("-- set sub sentences done: ", logType.Notification);
            return subsentenceMatches;
        }
    }
}