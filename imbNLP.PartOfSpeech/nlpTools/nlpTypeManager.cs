// --------------------------------------------------------------------------------------------------------------------
// <copyright file="nlpTypeManager.cs" company="imbVeles" >
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
using imbACE.Core.core;

namespace imbNLP.PartOfSpeech.nlpTools
{
    public class nlpTypeManager
    {
        private static nlpTypeManager _main;

        /// <summary>
        /// NLP type manager
        /// </summary>
        public static nlpTypeManager main
        {
            get
            {
                if (_main == null)
                {
                    _main = new nlpTypeManager();
                    _main.prepare();
                }
                return _main;
            }
        }

        private pipelineModelTypeManager _modelTypeManager;

        /// <summary>
        /// static and autoinitiated object
        /// </summary>
        public pipelineModelTypeManager modelTypeManager
        {
            get
            {
                if (_modelTypeManager == null)
                {
                    _modelTypeManager = new pipelineModelTypeManager();
                }
                return _modelTypeManager;
            }
        }

        public wlfConstructorTypeManager webLemmaConstructorTypeManager { get; protected set; } = new wlfConstructorTypeManager();

        public itmConstructorTypeManager itmConstructorTypeManager { get; protected set; } = new itmConstructorTypeManager();

        public nlpTypeManager()
        {
            prepare();
        }

        /// <summary>
        /// Calls for type loading
        /// </summary>
        public void prepare()
        {
            builderForLog logger = new builderForLog();

            imbSCI.Core.screenOutputControl.logToConsoleControl.setAsOutput(logger, "nlp");

            modelTypeManager.LoadTypes(logger);

            webLemmaConstructorTypeManager.LoadTypes(logger);
            itmConstructorTypeManager.LoadTypes(logger);
        }
    }
}