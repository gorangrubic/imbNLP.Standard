// --------------------------------------------------------------------------------------------------------------------
// <copyright file="wordCase.cs" company="imbVeles" >
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

namespace imbNLP.Data.@case
{
    #region imbVELES USING

    using System.Collections.Generic;

    #endregion imbVELES USING

    /// <summary>
    /// Izvestaj o stanju trenutne reci
    /// </summary>
    public class wordCase : wordCaseFactors
    {
        #region --- caseVotes ------- Svi prikupljeni glasovi

        private List<wordCaseFactors> _caseVotes = new List<wordCaseFactors>();

        /// <summary>
        /// Svi prikupljeni glasovi
        /// </summary>
        public List<wordCaseFactors> caseVotes
        {
            get { return _caseVotes; }
            set
            {
                _caseVotes = value;
                OnPropertyChanged("caseVotes");
            }
        }

        #endregion --- caseVotes ------- Svi prikupljeni glasovi

        //#region -----------  numberVotes  -------  [Description of $property$]
        //private List<number> _numberVotes = new List<number>(); // = new List<number>();
        ///// <summary>
        ///// Description of $property$
        ///// </summary>
        //// [XmlIgnore]
        //[Category("wordCase")]
        //[DisplayName("numberVotes")]
        //[Description("Description of $property$")]
        //public List<number> numberVotes
        //{
        //    get
        //    {
        //        return _numberVotes;
        //    }
        //    set
        //    {
        //        _numberVotes = value;
        //        OnPropertyChanged("numberVotes");
        //    }
        //}
        //#endregion

        //#region --- genreVotes ------- Bindable property
        //private List<genre> _genreVotes = new List<genre>();
        ///// <summary>
        ///// Bindable property
        ///// </summary>
        //public List<genre> genreVotes
        //{
        //    get
        //    {
        //        return _genreVotes;
        //    }
        //    set
        //    {
        //        _genreVotes = value;
        //        OnPropertyChanged("genreVotes");
        //    }
        //}
        //#endregion

        //#region --- faceVotes ------- Bindable property
        //private List<face> _faceVotes = new List<face>();
        ///// <summary>
        ///// Bindable property
        ///// </summary>
        //public List<face> faceVotes
        //{
        //    get
        //    {
        //        return _faceVotes;
        //    }
        //    set
        //    {
        //        _faceVotes = value;
        //        OnPropertyChanged("faceVotes");
        //    }
        //}
        //#endregion

        //#region --- grammarCaseVotes ------- Bindable property
        //private List<gramaticalCase> _grammarCaseVotes  = new List<gramaticalCase>();
        ///// <summary>
        ///// Bindable property
        ///// </summary>
        //public List<gramaticalCase> grammarCaseVotes
        //{
        //    get
        //    {
        //        return _grammarCaseVotes;
        //    }
        //    set
        //    {
        //        _grammarCaseVotes = value;
        //        OnPropertyChanged("grammarCaseVotes");
        //    }
        //}
        //#endregion

        //#region --- formVotes ------- Bindable property
        //private List<wordForms> _formVotes = new List<wordForms>();
        ///// <summary>
        ///// Bindable property
        ///// </summary>
        //public List<wordForms> formVotes
        //{
        //    get
        //    {
        //        return _formVotes;
        //    }
        //    set
        //    {
        //        _formVotes = value;
        //        OnPropertyChanged("formVotes");
        //    }
        //}
        //#endregion

        /*
                public wordCase(String wordExpression, extendedLanguage language,
                                tosWordType __targetType = tosWordType.unknown)
                {
                    targetType = __targetType;

                    switch (targetType)
                    {
                        case tosWordType.Verb:
                            Match infEnd = morphologyTools.infinitivEnd.Match(wordExpression);
                            if (infEnd.Success)
                            {
                                infinitiveSufix = infEnd.Value;
                                infinitiveRoot = morphologyTools.infinitivEnd.Replace(wordExpression, "");
                                wordForm = wordForms.infinitive;
                            }
                            else
                            {
                                // glasanje
                                foreach (verbMorphology verbMorph in language.morphologies_verbs)
                                {
                                    String longestSufix = "";
                                    foreach (Match mt in verbMorph.regexHasSufix.Matches(wordExpression))
                                    {
                                        if (mt.Value.Length > longestSufix.Length) longestSufix = mt.Value;
                                    }
                                    if (!String.IsNullOrEmpty(longestSufix))
                                    {
                                        caseVotes.AddNullSafe(verbMorph.getFactorsBySufix(longestSufix,morphStemPosition.sufix));
                                    }
                                }

                                countVotes();
                            }
                            break;

                        case tosWordType.Noun:
                            break;

                        default:
                            break;
                    }
                }
                */

        public void countVotes()
        {
            //wordCaseFactors __result = collectionExtendTools.collectionVoteResult(caseVotes, voterMode.mostFrequent);
            //this.setObjectBySource(__result);
        }
    }
}