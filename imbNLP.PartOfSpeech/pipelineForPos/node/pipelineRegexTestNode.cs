// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineRegexTestNode.cs" company="imbVeles" >
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
    using imbNLP.PartOfSpeech.pipeline.core;
    using imbNLP.PartOfSpeech.pipeline.machine;
    using imbNLP.PartOfSpeech.pipelineForPos.subject;
    using imbSCI.Core.extensions.data;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Distribution and/or transformation by regex expression over <see cref="pipelineTaskSubjectContentToken.currentForm"/>
    /// </summary>
    /// <seealso cref="imbNLP.PartOfSpeech.pipeline.core.pipelineNodeRegular{imbNLP.PartOfSpeech.pipeline.machine.pipelineTaskSubjectContentToken}" />
    public class pipelineRegexTestNode : pipelineNodeRegular<pipelineTaskSubjectContentToken>
    {
        protected Regex test { get; set; }

        protected Int32 groupToCurrent { get; set; } = -1;

        protected Object[] tagsToApply { get; set; }

        protected String replacement { get; set; } = "";

        protected pipelineRegexTestTypeEnum testType { get; set; } = pipelineRegexTestTypeEnum.distribution;

        /// <summary>
        /// Sets test for distribution <see cref="pipelineRegexTestNode"/> and optionaly for group replacer
        /// </summary>
        /// <param name="regexTest">The regex test.</param>
        /// <param name="groupToCurrent">The group from regex matches to replace <c>currentForm</c> with</param>
        public pipelineRegexTestNode(String regexTest, Int32 _current = -1, Object[] __tagsToApply = null)
        {
            test = new Regex(regexTest);

            groupToCurrent = _current;
            if (_current > -1)
            {
                testType |= pipelineRegexTestTypeEnum.groupreplacer | pipelineRegexTestTypeEnum.makeMatches;
                _nodeType |= pipelineNodeTypeEnum.transformer;
            }
            if (__tagsToApply != null)
            {
                tagsToApply = __tagsToApply;
                testType |= pipelineRegexTestTypeEnum.tagger;
                _nodeType |= pipelineNodeTypeEnum.transformer;
            }
            SetLabel();
        }

        /// <summary>
        /// Sets test for regex replacement of the matched test
        /// </summary>
        /// <param name="regexTest">The regex test.</param>
        /// <param name="__replacement">The replacement.</param>
        public pipelineRegexTestNode(String regexTest, String __replacement = "")
        {
            test = new Regex(regexTest);

            if (!__replacement.isNullOrEmpty())
            {
                replacement = __replacement;
                testType |= pipelineRegexTestTypeEnum.replacer;
                _nodeType |= pipelineNodeTypeEnum.transformer;
            }
        }

        /// <summary>
        /// Processes the specified task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        public override IPipelineNode process(IPipelineTask task)
        {
            var realTask = task as pipelineTask<pipelineTaskSubjectContentToken>;

            if (realTask == null) return next;

            IPipelineNode direction = null;

            Boolean testResult = test.IsMatch(realTask.subject.currentForm);

            if (testResult)
            {
                direction = forward;

                if (testType.HasFlag(pipelineRegexTestTypeEnum.tagger))
                {
                    realTask.subject.flagBag.AddRange(tagsToApply);
                }

                if (testType.HasFlag(pipelineRegexTestTypeEnum.groupreplacer))
                {
                    var mch = test.Match(realTask.subject.currentForm);

                    if (mch.Groups.Count >= groupToCurrent)
                    {
                        realTask.subject.currentForm = mch.Groups[groupToCurrent].Value;
                    }
                }

                if (testType.HasFlag(pipelineRegexTestTypeEnum.replacer))
                {
                    realTask.subject.currentForm = test.Replace(realTask.subject.currentForm, replacement);
                }
            }
            else
            {
                direction = next;
            }

            return direction;
        }
    }
}