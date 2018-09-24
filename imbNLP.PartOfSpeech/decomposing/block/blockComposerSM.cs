// --------------------------------------------------------------------------------------------------------------------
// <copyright file="blockComposerSM.cs" company="imbVeles" >
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
using System.Linq;

namespace imbNLP.PartOfSpeech.decomposing.block
{
    using HtmlAgilityPack;
    using imbCommonModels.contentBlock;
    using imbMiningContext.MCDocumentStructure;
    using imbNLP.PartOfSpeech.decomposing.html;

    /// <summary>
    /// SM-Crawler model of page to block decomposition
    /// </summary>
    public class blockComposerSM : IBlockComposer
    {
        public blockComposerSM()
        {
        }

        public Int32 targetBlockCount { get; set; } = 4;

        public List<imbMCBlock> process(HtmlDocument html, String name)
        {
            List<imbMCBlock> output = new List<imbMCBlock>();

            nodeTree tree = new nodeTree("document", html);

            //var navigator = html.DocumentNode.CreateNavigator();

            var contentTree = html.buildTree(name, true, false);
            // contentTree = new nodeTree(page.webpage.domain, htmlDoc);
            var contentBlocks = contentTree.getBlocks(targetBlockCount);
            contentBlocks.CalculateScores();

            //var blocks = tree.getBlocks(targetBlockCount);

            Int32 c = 0;
            foreach (nodeBlock bl in contentBlocks)
            {
                c++;
                imbMCBlock mcBlock = new imbMCBlock();
                mcBlock.name = "B" + c.ToString("D3");
                mcBlock.blockModel = bl;

                output.Add(mcBlock);
            }

            if (output.Any())
            {
            }
            else
            {
            }

            return output;
        }
    }
}