// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tokenComposerBasic.cs" company="imbVeles" >
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
using imbMiningContext.MCDocumentStructure;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace imbNLP.PartOfSpeech.decomposing.token
{
    public class tokenComposerBasic : ITokenComposer
    {
        private Regex tokenSelect = new Regex(@"([\w\.-]+|[\d\.,:;]+)\b");

        public tokenComposerBasic()
        {
        }

        /// <summary>
        /// Processes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public List<imbMCToken> process(imbMCStream stream)
        {
            List<imbMCToken> output = new List<imbMCToken>();

            // var str = tokenSelect.Split(stream.content);

            var mchs = tokenSelect.Matches(stream.content);

            Int32 c = 1;
            foreach (Match m in mchs)
            {
                imbMCToken mct = new imbMCToken();

                mct.name = "T" + c.ToString("D5");
                mct.htmlNode = stream.htmlNode;

                mct.content = m.Value;
                mct.position = m.Index;

                stream.Add(mct);

                output.Add(mct);
                c++;
            }

            return output;
        }
    }
}