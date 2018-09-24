// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineTransliterationNode.cs" company="imbVeles" >
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
    using imbACE.Core;
    using imbNLP.PartOfSpeech.pipeline.core;
    using imbNLP.PartOfSpeech.pipeline.machine;
    using imbNLP.PartOfSpeech.pipelineForPos.subject;
    using imbNLP.Transliteration.ruleSet;
    using imbSCI.Core.extensions.data;
    using System.IO;

    /// <summary>
    /// Pipeline transformer node
    /// </summary>
    /// <seealso cref="imbNLP.PartOfSpeech.pipeline.core.pipelineNodeRegular{imbNLP.PartOfSpeech.pipeline.machine.pipelineTaskSubjectContentToken}" />
    public class pipelineTransliterationNode : pipelineNodeRegular<pipelineTaskSubjectContentToken>
    {
        protected transliterationPairSet pairSet { get; set; }
        protected String transFilename { get; set; }
        protected Boolean inverseUse { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="pipelineTransliterationNode"/> class.
        /// </summary>
        public pipelineTransliterationNode(String __transliterationFilename, Boolean __inverseUse)
        {
            _nodeType = pipelineNodeTypeEnum.transformer;

            transFilename = __transliterationFilename;
            inverseUse = __inverseUse;

            tryLoading();
            SetLabel();
        }

        protected void tryLoading()
        {
            String _psPath = appManager.Application.folder_resources.findFile(transFilename, SearchOption.AllDirectories);

            if (_psPath.isNullOrEmpty())
            {
                String msg = "Transliteration file should be at: [build output]\\resources\\transliteration\\";
                throw new ArgumentException("There is no file for transliteration pair set at: [" + transFilename + "] -- have you installed NuGet package: imbNLP.Transliteration ? Did you made the transliteration definition file? " + msg);
            }

            if (File.Exists(_psPath))
            {
                pairSet = new transliterationPairSet();

                String specs = File.ReadAllText(_psPath);
                pairSet.LoadFromString(specs);
            }
            else
            {
                throw new ArgumentException("There is no file for transliteration pair set at: [" + _psPath + "] -- have you installed NuGet package: imbNLP.Transliteration ? Did you made the transliteration definition file?");
            }
        }

        /// <summary>
        /// Processes the specified task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        public override IPipelineNode process(IPipelineTask task)
        {
            var realSubject = task.subject as pipelineTaskSubjectContentToken;

            if (realSubject == null) return next;

            if (inverseUse)
            {
                realSubject.currentForm = pairSet.ConvertFromBtoA(realSubject.currentForm);
            }
            else
            {
                realSubject.currentForm = pairSet.ConvertFromAtoB(realSubject.currentForm);
            }
            // <---- tagging code

            if (realSubject.mcElement != null)
            {
                realSubject.mcElement.content = realSubject.currentForm;
            }

            return forward;
        }
    }
}