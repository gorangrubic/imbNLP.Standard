// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineTokenGenericAnnotation.cs" company="imbVeles" >
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
using System;

namespace imbNLP.PartOfSpeech.pipelineForPos.node
{
    using imbMiningContext.MCDocumentStructure;
    using imbNLP.PartOfSpeech.flags.token;
    using imbNLP.PartOfSpeech.pipeline.core;
    using imbNLP.PartOfSpeech.pipeline.machine;
    using imbNLP.PartOfSpeech.pipelineForPos.subject;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.text;

    /// <summary>
    /// Determinates generic, regex based annotation. Can be used for <see cref="imbMCToken"/>, but also for token streams
    /// </summary>
    /// <seealso cref="imbNLP.PartOfSpeech.pipeline.core.pipelineNodeRegular{imbNLP.PartOfSpeech.pipeline.machine.pipelineTaskSubjectContentToken}" />
    public class pipelineTokenGenericAnnotation : pipelineNodeRegular<pipelineTaskSubjectContentToken>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="pipelineTokenGenericAnnotation"/> class.
        /// </summary>
        public pipelineTokenGenericAnnotation()
        {
            _nodeType = pipelineNodeTypeEnum.transformer;
            SetLabel();
        }

        /// <summary>
        /// Processes the specified task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        public override IPipelineNode process(IPipelineTask task)
        {
            pipelineTask<pipelineTaskSubjectContentToken> realTask = task as pipelineTask<pipelineTaskSubjectContentToken>;

            if (realTask == null) return next;

            pipelineTaskSubjectContentToken realSubject = realTask.subject;

            if (realSubject.currentForm.isTokenStream())
            {
                if (realSubject.currentForm.isWithLetterChars())
                {
                    if (realSubject.currentForm.isStrictSentenceCase())
                    {
                        realSubject.flagBag.AddUnique(tkn_stream.sentenceProperCase);
                    }

                    if (realSubject.currentForm.isNoLowerCaseTokenStream()) realSubject.flagBag.AddUnique(tkn_stream.titleAllCaps);

                    if (realSubject.currentForm.isEndsWithEnumerationPunctation()) realSubject.flagBag.AddUnique(tkn_stream.titleForEnumeration);

                    if (realSubject.currentForm.isEndsWithExclamationPunctation()) realSubject.flagBag.AddUnique(tkn_stream.sentenceEclamationEnd);

                    if (realSubject.currentForm.isEndsWithQuestionPunctation()) realSubject.flagBag.AddUnique(tkn_stream.sentenceQuestionEnd);
                }
            }

            if (realSubject.currentForm.isWithNumericChars())
            {
                realSubject.flagBag.AddUnique(tkn_contains.number);

                if (realSubject.currentForm.isNumber())
                {
                    realSubject.flagBag.AddUnique(tkn_numeric.numberClean);
                }
                else if (realSubject.currentForm.isNumberFormatted())
                {
                    realSubject.flagBag.AddUnique(tkn_numeric.numberInFormat);
                }
                else if (realSubject.currentForm.isDecimalNumber())
                {
                    realSubject.flagBag.AddUnique(tkn_numeric.numberDecimal);
                }
                else if (realSubject.currentForm.isOrdinalNumber())
                {
                    realSubject.flagBag.AddUnique(tkn_numeric.numberOrdinal);
                }
                else
                {
                }

                if (realSubject.currentForm.isPercentageNumber())
                {
                    realSubject.flagBag.AddUnique(tkn_numeric.numberInPercentage);
                }
            }

            if (realSubject.currentForm.isWithLetterChars())
            {
                realSubject.flagBag.AddUnique(tkn_contains.letter);

                if (realSubject.currentForm.isCleanWord())
                {
                    realSubject.flagBag.AddUnique(tkn_contains.onlyLetters);
                }

                if (realSubject.currentForm.isAllLowerLetterCaseWord())
                {
                    realSubject.flagBag.AddUnique(tkn_letterword.lowerCase);
                }
                else if (realSubject.currentForm.isFirstCapitalRestLowerCase())
                {
                    realSubject.flagBag.AddUnique(tkn_letterword.firstCapitalRestLower);
                }
                else if (realSubject.currentForm.isAllCapitalLetterCaseWord())
                {
                    realSubject.flagBag.AddUnique(tkn_letterword.upperCase);
                }
                else
                {
                    realSubject.flagBag.AddUnique(tkn_letterword.inproperCase);
                }
            }

            if (realSubject.currentForm.isRegexMatch(@"\p{S}"))
            {
                realSubject.flagBag.AddUnique(tkn_contains.symbols);
            }

            if (realSubject.currentForm.isRegexMatch(@"\p{P}"))
            {
                realSubject.flagBag.AddUnique(tkn_contains.punctation);
            }

            switch (realSubject.contentLevelType)
            {
                case cnt_level.mcBlock:
                    break;

                case cnt_level.mcTokenStream:
                    break;

                case cnt_level.mcToken:

                    var streamSubject = realSubject.parent as pipelineTaskSubjectContentToken;

                    if (streamSubject.flagBag.ContainsAny(new Object[] { tkn_stream.sentenceProperCase }))
                    {
                        if (realSubject.flagBag.ContainsAll(new Object[] { tkn_letterword.upperCase, tkn_contains.letter }))
                        {
                            realSubject.flagBag.Add(tkn_potential.companyNamePart);
                        }

                        if (realSubject.flagBag.ContainsAll(new Object[] { tkn_letterword.firstCapitalRestLower, tkn_contains.onlyLetters }))
                        {
                            realSubject.flagBag.Add(tkn_potential.personName);
                        }
                    }
                    break;
            }

            return forward;
        }
    }
}