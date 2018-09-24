// --------------------------------------------------------------------------------------------------------------------
// <copyright file="streamComposerBasic.cs" company="imbVeles" >
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
using System.Collections.Generic;

namespace imbNLP.PartOfSpeech.decomposing.stream
{
    using HtmlAgilityPack;
    using imbCommonModels.contentBlock;
    using imbMiningContext.MCDocumentStructure;
    using imbSCI.Data;

    /// <summary>
    /// Decomposes a block into streams
    /// </summary>
    public class streamComposerBasic : IStreamComposer
    {
        public streamComposerBasic()
        {
        }

        /// <summary>
        /// Processes the specified block.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <returns></returns>
        public List<imbMCStream> process(imbMCBlock block)
        {
            List<imbMCStream> output = new List<imbMCStream>();

            String content = ""; // block.blockModel.getContent(nodeBlockOutputEnum.text);

            foreach (htmlWrapper node in block.blockModel)
            {
                content = node.GetContent(nodeBlockOutputEnum.text);

                content = HtmlEntity.DeEntitize(content);

                var streams = content.SplitSmart(Environment.NewLine, "", true, true);

                Int32 c = 1;
                foreach (String str in streams)
                {
                    imbMCStream stream = new imbMCStream();
                    stream.name = "S" + c.ToString("D5");

                    c++;

                    block.Add(stream);

                    stream.content = str;
                    stream.htmlNode = node.html;

                    output.Add(stream);
                }
            }

            return output;
        }
    }
}