// --------------------------------------------------------------------------------------------------------------------
// <copyright file="lexiconConstructTaskFive.cs" company="imbVeles" >
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
namespace imbNLP.Data.semanticLexicon.procedures
{
    using imbNLP.Data.semanticLexicon.source;
    using imbSCI.Core.reporting;
    using System;
    using System.Linq;

    public class lexiconConstructTaskFive : lexiconTaskBase
    {
        public override string taskInputPath
        {
            get
            {
                return "corpus_s5_evaluateAndExpand.txt";
            }
        }

        public override string taskOutputPath
        {
            get
            {
                return "lexicon_s5_expansions.txt";
            }
        }

        public override string taskSourcePath
        {
            get
            {
                return semanticLexiconManager.manager.settings.sourceFiles.getFilePaths(lexiconSourceTypeEnum.corpus).First();
            }
        }

        public override string taskTitle
        {
            get
            {
                return "stageFive";
            }
        }

        public override void stageComplete(ILogBuilder response)
        {
            throw new NotImplementedException();
        }

        protected override void stageExecute(ILogBuilder response)
        {
            throw new NotImplementedException();
        }
    }
}