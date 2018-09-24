// --------------------------------------------------------------------------------------------------------------------
// <copyright file="lexiconConstructTaskZero.cs" company="imbVeles" >
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
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.files.search;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.enums;
    using System;
    using System.Linq;

    /// <summary>
    /// Stage zero is about spliting corpora into english, serbian and english-serbian words
    /// </summary>
    /// <seealso cref="imbNLP.Data.semanticLexicon.procedures.lexiconTaskBase" />
    public class lexiconConstructTaskZero : lexiconTaskBase
    {
        public string corpusFilterLanguageEval(string line)
        {
            line = line.Trim("\" ".ToArray());

            throw new NotImplementedException();

            //bool sr_ok = imbLanguageFrameworkManager.serbian.basic.hunspellEngine.Spell(line);
            //bool en_ok = imbLanguageFrameworkManager.english.basic.hunspellEngine.Spell(line);
            //corpusEntryFilterResult answer = corpusEntryFilterResult.none;

            //if (sr_ok)
            //{
            //    if (en_ok)
            //    {
            //        return corpusEntryFilterResult.lang_serbian_english.ToString();
            //    }
            //    else
            //    {
            //        return "";
            //    }
            //}
            //else
            //{
            //    if (en_ok)
            //    {
            //        return corpusEntryFilterResult.lang_english.ToString();
            //    }
            //    else
            //    {
            //        return corpusEntryFilterResult.lang_other.ToString();
            //    }
            //}

            //return answer.ToString();
            return line;
        }

        public lexiconConstructTaskZero() : base()
        {
        }

        public override bool taskSourcePathIsAppRoot
        {
            get
            {
                return true;
            }
        }

        public override string taskSourcePath
        {
            get
            {
                return semanticLexiconManager.manager.settings.sourceFiles.getFilePaths(lexiconSourceTypeEnum.corpus).First();
            }
        }

        public override string taskInputPath
        {
            get
            {
                return "corpus_input.csv";
            }
        }

        public override string taskOutputPath
        {
            get
            {
                return "corpus_input_excluded.csv";
            }
        }

        public override string taskTitle
        {
            get
            {
                return "stageZero";
            }
        }

        public override void stageComplete(ILogBuilder response)
        {
            state.processedTasks.saveContentOnFilePath(taskOutputPath);
            state.scheduledTasks.file.CopyTo(taskInputPath, true);
        }

        protected override void stageExecute(ILogBuilder response)
        {
            string splitPath = semanticLexiconManager.manager.constructor.projectFolderStructure[lexiconConstructorProjectFolder.splits].path;

            fileTextSplitResultSet splits = state.scheduledTasks.Split(lexiconConstructorProjectFolder.splits.ToString().add("{0}.txt", "\\"), corpusFilterLanguageEval, false);

            splits.saveSlits(semanticLexiconManager.manager.constructor.projectFolderStructure.path, getWritableFileMode.overwrite);

            state.processedBuffer.AddRange(splits.getLines());

            state.scheduledTasks.Remove(splits.getLineNumbers(true));

            state.stateSessionTick(this, true);

            stageComplete(response);
        }

        public void corpusLanguageFilter()
        {
        }
    }
}