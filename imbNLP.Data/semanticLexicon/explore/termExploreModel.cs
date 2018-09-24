// --------------------------------------------------------------------------------------------------------------------
// <copyright file="termExploreModel.cs" company="imbVeles" >
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
namespace imbNLP.Data.semanticLexicon.explore
{
    using imbNLP.Data.semanticLexicon.core;
    using imbNLP.PartOfSpeech.lexicUnit.tokenGraphs;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Temp object to explore a term
    /// </summary>
    /// <seealso cref="termExploreItem" />
    [XmlInclude(typeof(termExploreItem))]
    [XmlInclude(typeof(termExploreItemCollection))]
    // [XmlInclude(typeof(gramFlags))]
    // [XmlInclude(typeof(gramCaseSet))]
    public class termExploreModel : termExploreItem
    {
        /// <summary>
        ///
        /// </summary>
        public string lastModifiedByStage { get; set; } = "unknown";

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore]
        public tokenGraph graph { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int links_synonym { get; set; } = 0;

        /// <summary>
        ///
        /// </summary>
        public int links_lemmaConcept { get; set; } = 0;

        /// <summary>
        ///
        /// </summary>
       // [XmlElement(typeof(Int32), "new_links_c2c")]

        public int links_conceptConcept { get; set; } = 0;

        private termExploreModelSource _modelSource = termExploreModelSource.none;

        /// <summary> </summary>
        [XmlIgnore]
        public termExploreModelSource modelSource
        {
            get
            {
                return _modelSource;
            }
            set
            {
                _modelSource = value;
                OnPropertyChanged("modelSource");
            }
        }

        /// <summary>
        /// To the string.
        /// </summary>
        /// <param name="loger">The loger.</param>
        /// <param name="expanded">if set to <c>true</c> [expanded].</param>
        public void ToString(ILogBuilder loger, bool expanded = false, bool showInstances = true)
        {
            if (loger == null) return;

            loger.AppendLine("Input term: " + inputForm);
            if (!rootWord.isNullOrEmpty()) loger.log("Root: " + rootWord);

            if (lemma != null)
            {
                lemma.ToString(loger, "Lemma");
            }
            if (showInstances)
            {
                foreach (termExploreItem sug in instances)
                {
                    sug.ToString(loger, "Instance", expanded);
                }
            }

            if (synonyms.Any()) loger.AppendLine("Related terms: " + String.Join(",", synonyms));
            if (wordnetSynonyms.Any()) loger.AppendLine("Not accepted: " + String.Join(",", wordnetSynonyms));

            if (translations.Any()) loger.AppendLine("Translations: " + String.Join(",", translations));
            if (translationRelated.Any()) loger.AppendLine("Not accepted: " + String.Join(",", translationRelated));

            // if (wordnetSynonymSerbian.Any()) loger.AppendLine("WordNet translated: " + wordnetSynonymSerbian.Join(','));
            if (wordnetPrimarySymsets.Any()) loger.AppendLine("Symsets: " + String.Join(",", wordnetPrimarySymsets));
            if (wordnetSecondarySymsets.Any()) loger.AppendLine("Not accepted: " + String.Join(",", wordnetSecondarySymsets));
        }

        public void PostProcess()
        {
            synonyms.Remove(lemmaForm);
            wordnetSynonyms.Remove(lemmaForm);

            translationRelated.removeRange(translations);
            wordnetSecondarySymsets.removeRange(wordnetPrimarySymsets);
            wordnetSynonyms.removeRange(synonyms);

            //  wordnetSynonyms.removeRange(synonyms); //.removeRange(synonyms)
        }

        /// <summary>
        /// Filenames the specified extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        public string filename(string extension = ".xml")
        {
            string output = inputForm;
            if (lemma != null)
            {
                output = lemma.inputForm;
                output = output + "_" + lemma.gramSet.getPosType().ToString();
            }

            output = output.add(extension, ".");
            return output;
        }

        public termExploreModel(string __inputForm)
        {
            inputForm = __inputForm;
            modelSource = termExploreModelSource.fromToken;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="termExploreModel"/> class.
        /// </summary>
        /// <param name="lemmaSource">The lemma source.</param>
        public termExploreModel(ITermLemma lemmaSource)
        {
            inputForm = lemmaSource.name;
            modelSource = termExploreModelSource.fromLemma;
            lemma = new termExploreItem(lemmaSource.name, lemmaSource.gramSet);

            foreach (TermInstance ins in lemmaSource.instances)
            {
                instances.Add(new termExploreItem(ins.name, ins.gramSet));
            }
        }

        public termExploreModel()
        {
        }

        /// <summary>
        /// Gets the shadow.
        /// </summary>
        /// <returns></returns>
        public List<string> GetShadow()
        {
            List<string> output = new List<string>();
            output.Add(lemma.inputForm);
            foreach (termExploreItem item in instances)
            {
                output.AddUnique(item.inputForm);
            }
            return output;
        }

        private termExploreItem _lemma;

        /// <summary> </summary>
        public termExploreItem lemma
        {
            get
            {
                return _lemma;
            }
            set
            {
                _lemma = value;
                OnPropertyChanged("lemma");
            }
        }

        private termExploreItemCollection _instances = new termExploreItemCollection();

        /// <summary> </summary>
        public termExploreItemCollection instances
        {
            get
            {
                return _instances;
            }
            set
            {
                _instances = value;
                OnPropertyChanged("instances");
            }
        }

        private List<string> _translations = new List<string>();

        /// <summary> </summary>
        public List<string> translations
        {
            get
            {
                return _translations;
            }
            set
            {
                _translations = value;
                OnPropertyChanged("translations");
            }
        }

        /// <summary>
        ///
        /// </summary>
        public List<string> translationRelated { get; set; } = new List<string>();

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore]
        public List<string> wordnetSynonyms { get; set; } = new List<string>();

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore]
        public List<string> wordnetSynonymSerbian { get; set; } = new List<string>();

        /// <summary>
        ///
        /// </summary>
        public List<string> synonyms { get; set; } = new List<string>();

        /// <summary> </summary>
        [XmlIgnore]
        public List<string> wordnetSecondarySymsets { get; set; } = new List<string>();

        /// <summary>
        ///
        /// </summary>
        public List<string> wordnetPrimarySymsets { get; set; } = new List<string>();

        #region ----------- Boolean [ wasExploreFailed ] -------  [has exploration failed]

        private bool _wasExploreFailed = false;

        /// <summary>
        /// has exploration failed
        /// </summary>
        [Category("Switches")]
        [DisplayName("wasExploreFailed")]
        [Description("has exploration failed")]
        public bool wasExploreFailed
        {
            get { return _wasExploreFailed; }
            set { _wasExploreFailed = value; OnPropertyChanged("wasExploreFailed"); }
        }

        #endregion ----------- Boolean [ wasExploreFailed ] -------  [has exploration failed]

        private string _rootWord = ""; // = new String();

        /// <summary>
        /// Description of $property$
        /// </summary>
        [Category("termExploreModel")]
        [DisplayName("rootWord")]
        [Description("Description of $property$")]
        public string rootWord
        {
            get
            {
                return _rootWord;
            }
            set
            {
                _rootWord = value;
                OnPropertyChanged("rootWord");
            }
        }

        private string _lemmaForm = ""; // = new String();

        /// <summary>
        /// Lemmata form of the word
        /// </summary>
        [Category("termExploreModel")]
        [DisplayName("lemmaForm")]
        [Description("Lemmata form of the word")]
        public string lemmaForm
        {
            get
            {
                if (_lemmaForm.isNullOrEmpty())
                {
                    if (_lemma != null)
                    {
                        _lemmaForm = _lemma.inputForm;
                    }
                }
                return _lemmaForm;
            }
            set
            {
                _lemmaForm = value;
                OnPropertyChanged("lemmaForm");
            }
        }
    }
}