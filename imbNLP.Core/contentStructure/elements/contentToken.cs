// --------------------------------------------------------------------------------------------------------------------
// <copyright file="contentToken.cs" company="imbVeles" >
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
    using imbNLP.Core.contentStructure.tokenizator;
    using imbNLP.Data.enums;
    using imbNLP.Data.enums.flags;
    using imbSCI.Core.attributes;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.extensions.text;
    using imbSCI.Data;
    using imbSCI.Data.interfaces;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text.RegularExpressions;

    #endregion imbVELES USING

    [imb(imbAttributeName.xmlNodeTypeName, "t")]
    //[imb(imbAttributeName.xmlNodeValueProperty, "sourceContent")]
    [imb(imbAttributeName.xmlEntityOutput, "content")]
    public class contentToken : contentElementBase, IContentToken
    {
        public IEnumerator GetEnumerator()
        {
            if (items == null) return null;

            return items.GetEnumerator();
        }

        public int indexOf(IObjectWithChildSelector child)
        {
            if (items == null) return -1;

            return items.IndexOf(child as IContentToken);
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

        /// <summary>
        /// Regex select tokenSplitter : (([\W\s\+\.\,]+){1,}$)
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        private static Regex __select_tokenSplitter;

        private static object tokenSplitterLock = new object();

        public static Regex _select_tokenSplitter
        {
            get
            {
                if (__select_tokenSplitter == null)
                {
                    lock (tokenSplitterLock)
                    {
                        __select_tokenSplitter = new Regex(@"(([\W\s\+\.\,]+){1,}$)");
                    }
                }
                return __select_tokenSplitter;
            }
        }

        /// <summary>
        /// Bezbedno preuzimanje propertija Token iz objekta next
        /// </summary>
        public contentToken next_Token
        {
            get
            {
                contentToken _out = (contentToken)next;
                if (_out == null) return null;

                return _out;
            }
        }

        public contentToken prev_Token
        {
            get
            {
                contentToken _out = (contentToken)prev;
                if (_out == null) return null;

                return _out;
            }
        }

        public contentToken()
        {
            items = new contentSyllableCollection();
        }

        // #region IContentToken Members

        //IContentCollectionBase IContentElement.items
        //{
        //    get { return _items; }
        //}

        /// <summary>
        /// Primarno obeležavanje -- OVO JE POČETAK OBELEŽAVANAJA: dakle od mikro ka makro nivou, ne oslanjati se na podatke iz viših elemenata
        /// </summary>
        /// <param name="resources"></param>
        public override void primaryFlaging(params object[] resources)
        {
            items.ForEach(x => x.primaryFlaging(resources));

            foreach (tokenDetectionFlag fl in detectionFlags.getEnumListFromFlags())
            {
                switch (fl)
                {
                    case tokenDetectionFlag.acronims:
                        flags = flags.Add(contentTokenFlag.acronim);
                        break;

                    case tokenDetectionFlag.emailAddress:
                        flags = flags.Add(contentTokenFlag.email);
                        break;

                    case tokenDetectionFlag.phonenumber:
                        flags = flags.Add(contentTokenFlag.officePhone);
                        break;

                    case tokenDetectionFlag.postOfficeNumber:
                        flags = flags.Add(contentTokenFlag.zipCodeNumber);

                        break;

                    case tokenDetectionFlag.potentialNamedEntity:
                        flags = flags.Add(contentTokenFlag.namedEntity);

                        setTokenCaseAndGeneral();

                        break;

                    case tokenDetectionFlag.standard:
                        flags = flags.Add(contentTokenFlag.internationalStandard);
                        break;

                    case tokenDetectionFlag.yearNumber:
                        flags = flags.Add(contentTokenFlag.yearNumber);
                        break;

                    case tokenDetectionFlag.none:
                    case tokenDetectionFlag.standardDetection:

                        setTokenCaseAndGeneral();
                        break;

                    default:
                        //logSystem.log(
                        //    "        -- tokenDetectionFlag not supported: + [" + fl.ToString() + "]",
                        //    logType.Notification);
                        break;
                }
            }

            /*
            basicLanguage basicLanguages = resources.getOfType<basicLanguage>();
            if (basicLanguages != null)
            {
                if (flags == contentTokenFlag.languageWord)
                {
                   // basicLanguages.testBoolean(content, basicLanguageCheck.analyze);

                    if (basicLanguages.isKnownWord(content))
                    {
                        flags.Add(contentTokenFlag.languageKnownWord);

                       // Object output = basicLanguages.test(content, basicLanguageCheck.fullAnalysis);
                    }
                    else
                    {
                        flags.Add(contentTokenFlag.languageUnknownWord);
                    }
                }
            }
            */

            //throw new NotImplementedException();
        }

        public override void secondaryFlaging(params object[] resources)
        {
            IContentToken _sub = parent as IContentToken;
            if (_sub != null)
            {
                if (_sub.detectionFlags.HasFlag(tokenDetectionFlag.cityAndPostnumberSubSentences))
                {
                    flags.Add(contentTokenFlag.cityName);
                }
                if (_sub.detectionFlags.HasFlag(tokenDetectionFlag.potentialPersonalNamesSubSentences))
                {
                    flags.Add(contentTokenFlag.personalNameOrLastname);
                }
            }

            if (page != null)
            {
                bool checkHeads = false;
                if (flags.ContainsAll(contentTokenFlag.caseAllUpper, contentTokenFlag.languageWord))
                {
                    checkHeads = true;
                }
                else if (flags.ContainsAll(contentTokenFlag.caseFirstUpper, contentTokenFlag.languageWord, contentTokenFlag.languageUnknownWord))
                {
                    checkHeads = true;
                }
                else if (flags.HasFlag(contentTokenFlag.namedEntity))
                {
                    checkHeads = true;
                }

                if (checkHeads)
                {
                    foreach (contentToken tkn in page.headTokens)
                    {
                        if (content.ToLower() == tkn.content)
                        {
                            if (tkn.origin == contentTokenOrigin.domain)
                            {
                                flags = flags.Add(contentTokenFlag.namedEntity);
                                flags = flags.Add(contentTokenFlag.namedEntityDiscovered);
                            }
                            if (tkn.origin == contentTokenOrigin.title)
                            {
                                if (flags.HasFlag(contentTokenFlag.languageUnknownWord))
                                {
                                    flags = flags.Add(contentTokenFlag.namedEntity);
                                }
                                else
                                {
                                    flags = flags.Add(contentTokenFlag.title);
                                }
                            }
                        }
                    }
                }
            }

            //if (this.items.Query<contentTokenFlags>(enums.contentRelationQueryType.gatherFlags, enums.contentRelationType.manyNext, this, 2).Contains(contentTokenFlag.acronim))
            if (ContainsOneOrMore(contentRelationType.manyNext, 2, contentTokenFlag.acronim, contentTokenFlag.acronimKnown))
            {
                if (flags.HasFlag(contentTokenFlag.namedEntity))
                {
                    flags = flags.Add(contentTokenFlag.namedEntityDiscovered);
                }
                else
                {
                }
            }

            //}

            baseType = nlpTokenBaseType.unknown;
            var fl = flags.getEnumListFromFlags();

            if (fl.ContainsOneOrMore(contentTokenFlag.languageWord, contentTokenFlag.title,
                contentTokenFlag.namedEntity, contentTokenFlag.languageKnownWord))
            {
                baseType = nlpTokenBaseType.word;
            }

            if (fl.ContainsOneOrMore(contentTokenFlag.number, contentTokenFlag.numberFormatted,
                contentTokenFlag.yearNumber, contentTokenFlag.zipCodeNumber, contentTokenFlag.internationalStandard))
            {
                baseType = nlpTokenBaseType.number;
            }

            if (fl.ContainsOneOrMore(contentTokenFlag.numberFormatted))
            {
                baseType = nlpTokenBaseType.mixed;
            }

            items.ForEach(x => x.secondaryFlaging(resources));
            // throw new NotImplementedException();
        }

        /// <summary>
        /// Ovde primenjuje: Nazive gradova, brojeve poste, prezimena,
        /// </summary>
        /// <param name="resources"></param>
        public override void generalSemanticsFlaging(params object[] resources)
        {
            // throw new NotImplementedException();
        }

        public override void specialSematicsFlaging(params object[] resources)
        {
            //throw new NotImplementedException();
        }

        //#endregion

        #region --- items ------- Kolekcija slogova

        // private contentTokenCollection _items = new contentTokenCollection();

        /// <summary>
        /// Kolekcija slogova
        /// </summary>
        //[imb(imbAttributeName.xmlEntityOutput)]
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

        #endregion --- items ------- Kolekcija slogova

        #region --- baseType ------- osnovna klasifikacija tokena

        private nlpTokenBaseType _baseType = nlpTokenBaseType.unknown;

        /// <summary>
        /// osnovna klasifikacija tokena
        /// </summary>
        public nlpTokenBaseType baseType
        {
            get
            {
                return _baseType;
            }
            set
            {
                _baseType = value;
                OnPropertyChanged("baseType");
            }
        }

        #endregion --- baseType ------- osnovna klasifikacija tokena

        #region -----------  detectionFlags  -------  [flagovi dodeljeni prilikom detekcije]

        private tokenDetectionFlag _detectionFlags;

        /// <summary>
        /// flagovi dodeljeni prilikom detekcije
        /// </summary>
        // [XmlIgnore]
        [Category("contentToken")]
        [DisplayName("detectionFlags")]
        [Description("flagovi dodeljeni prilikom detekcije")]
        //   [imb(imbAttributeName.xmlEntityOutput)]
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

        #endregion -----------  detectionFlags  -------  [flagovi dodeljeni prilikom detekcije]

        #region -----------  flags  -------  [flagovi o samom tokenu]

        private contentTokenFlag _flags;

        /// <summary>
        /// flagovi o samom tokenu
        /// </summary>
        // [XmlIgnore]
        [Category("contentToken")]
        [DisplayName("flags")]
        [Description("flagovi o samom tokenu")]
        [imb(imbAttributeName.xmlEntityOutput)]
        public contentTokenFlag flags
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

        #endregion -----------  flags  -------  [flagovi o samom tokenu]

        protected contentTokenOrigin _getOrigin()
        {
            IContentSubSentence parent_IContentSubSentence = null;
            bool isSubSentence = false;
            if (this is IContentSubSentence)
            {
                return contentTokenOrigin.subsentence;
            }
            if (parent != null)
            {
                if (parent is IContentSubSentence)
                {
                    return contentTokenOrigin.subsentenceToken;
                    //parent_IContentSubSentence = (IContentSubSentence)this.parent;
                    //isSubSentence = true;
                }
            }

            return contentTokenOrigin.normal;
        }

        /// <summary>
        /// pod FAZA 2.a: detektovanje slogova - poziva ga faza 2, nema potrebe posebno pozivati --- ne radi dobro
        /// </summary>
        /// <param name="token"></param>
        /// <param name="language"></param>
        public void syllablesDetection(nlpTokenizatorSettings settings)
        {
            return;
            /*
            //token.i = new List<nlpSyllable>();
            MatchCollection coll = null;

            switch (tokenBaseType)
            {
                case nlpTokenBaseType.word:
                    if (genericType == nlpTokenGenericType.wordAbrevation)
                    {
                        coll = tokenization.samoRec.Matches(sourceContent);
                    }
                    else
                    {
                        if (settings.vowelRegex.IsMatch(content))
                        {
                            coll = settings.vowelRegex.Matches(content);
                        }
                    }
                    break;

                case nlpTokenBaseType.number:
                    //if (genericType == nlpTokenGenericType.numberFormated)
                    //{
                    //    String[] npt = nlpTokenizator.numberFormatSymbols.Split(sourceContent);
                    //    foreach (String smc in npt)
                    //    {
                    //        setItem(new nlpSyllable(smc, this, language));
                    //    }
                    //}
                    coll = tokenization.numericSelect.Matches(sourceContent);
                    break;

                case nlpTokenBaseType.mixed:
                    coll = tokenization.samoRec.Matches(sourceContent);
                    //String[] prts = nlpTokenizator.selectLetterToOtherChanges.Split(sourceContent);
                    //foreach (String smc in prts)
                    //{
                    //    setItem(new nlpSyllable(smc, this, language));
                    //}
                    break;

                default:
                    return;
                    break;
            }

            Int32 lastIndex = 0;
            String start = "";
            String ende = "";
            contentSyllable last = null;
            if (coll == null)
            {
            }
            else
            {
                foreach (Match mc in coll)
                {
                    last = setItem(new contentSyllable(mc.Value, this, settings)) as contentSyllable;

                    if ((lastIndex == 0) && (mc.Index > 0))
                    {
                        start = content.Substring(0, mc.Index);
                        setItem(new contentSyllable(start, this, settings));
                    }
                    lastIndex = mc.Index + mc.Length;

                    start = "";
                }
                if (last != null)
                {
                    if (lastIndex < content.Length)
                    {
                        ende = content.Substring(lastIndex);
                        setItem(new contentSyllable(ende, this, settings));
                    }
                }

                if (this.items.Count == 0)
                {
                    last = setItem(new contentSyllable(content, this, settings)) as contentSyllable;
                }
            }
            //syllablesLine = rebuildSyllLine();
             * */
        }

        protected virtual void setTokenCaseAndGeneral()
        {
            string _toTest = content;

            if (string.IsNullOrEmpty(_toTest))
            {
                flags.Add(contentTokenFlag.empty);
                return;
            }
            if (content.isNumber())
            {
                flags.Add(contentTokenFlag.number);

                if (content.isOrdinalNumber())
                {
                    flags.Add(contentTokenFlag.numberOrdinal);
                }
                else if (content.isFormatedNumber())
                {
                    flags.Add(contentTokenFlag.numberFormatted);
                }
                else if (content.isDecimalNumber())
                {
                    flags.Add(contentTokenFlag.number);
                }
            }
            else
            {
                if (_toTest.isWordCaseCamel())
                {
                    flags.Add(contentTokenFlag.caseFirstUpper);
                    flags.Add(contentTokenFlag.languageWord);
                }
                else if (_toTest.isWordCaseUpper())
                {
                    flags.Add(contentTokenFlag.caseAllUpper);
                    flags.Add(contentTokenFlag.languageWord);
                }
                else if (_toTest.isWordCaseLower())
                {
                    flags.Add(contentTokenFlag.caseLower);
                    flags.Add(contentTokenFlag.languageWord);
                }
                else
                {
                    flags.Add(contentTokenFlag.caseIrregular);
                    flags.Add(contentTokenFlag.languageWord);
                }
            }
        }

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
            return items.Query<contentTokenFlag>(contentRelationQueryType.gatherFlags, qRelation, this, limit).ContainsAny(_flgs);
        }

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

            return output;
        }

        #region --- origin ------- poreklo tokena

        private contentTokenOrigin _origin = contentTokenOrigin.unknown;

        /// <summary>
        /// poreklo tokena
        /// </summary>
        [imb(imbAttributeName.xmlEntityOutput)]
        public contentTokenOrigin origin
        {
            get
            {
                if (_origin == contentTokenOrigin.unknown)
                {
                    _origin = _getOrigin();
                }
                return _origin;
            }
            set
            {
                _origin = value;
                OnPropertyChanged("origin");
            }
        }

        IContentCollectionBase IContentElement.items
        {
            get
            {
                return items;
            }
        }

        public bool has_tosInstance => throw new NotImplementedException();

        public nlpTokenGenericType genericType { get; set; }

        #endregion --- origin ------- poreklo tokena
    }
}