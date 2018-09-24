// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tokenGraph.cs" company="imbVeles" >
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
namespace imbNLP.PartOfSpeech.lexicUnit.tokenGraphs
{
    using imbSCI.Core.extensions.io;
    using imbSCI.Data;
    using imbSCI.Data.enums;
    using System;

    public class tokenGraph : tokenGraphNode
    {
        public void saveDescription(String folder_path, String filename_sufix)
        {
            String fn = name.add(filename_sufix, "_").add(type.ToString(), "_tree_").getCleanFilepath(".txt");
            String path = folder_path.add(fn, "\\");
            String desc = this.ToStringTreeview("", true, 0);
            desc.saveStringToFile(path, getWritableFileMode.overwrite);
        }

        public void savePaths(String folder_path, String filename_sufix)
        {
            String fn = name.add(filename_sufix, "_").add(type.ToString(), "_paths_").getCleanFilepath(".txt");
            String path = folder_path.add(fn, "\\");
            String desc = this.ToStringPathList();
            desc.saveStringToFile(path, getWritableFileMode.overwrite);
        }

        /// <summary>
        /// Initializes a new instance of the root <see cref="tokenGraphNode"/> class -- assigned to the <see cref="tokenGraphNodeType.word_query"/>
        /// </summary>
        /// <param name="__queryToken">The query token.</param>
        public tokenGraph(String __queryToken) : base(__queryToken, tokenGraphNodeType.word_query, null)
        {
        }
    }
}