// --------------------------------------------------------------------------------------------------------------------
// <copyright file="lexicQuery.cs" company="imbVeles" >
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
namespace imbNLP.PartOfSpeech.lexicUnit
{
    using imbNLP.PartOfSpeech.flags.basic;
    using imbSCI.Data.collection.graph;
    using System;

    [Serializable]
    public class lexicQuery : lexicGraph
    {
        public lexicQuery(String queryWord)
        {
            name = queryWord;
            nodeType = lexicGraphNodeType.query;
        }

        public lexicLemmaInTypeNode AddLemmaInType(String lemmaForm, pos_type posType)
        {
            var __name = lemmaForm + "|" + posType.ToString();
            if (mychildren.ContainsKey(__name))
            {
                return mychildren[__name] as lexicLemmaInTypeNode;
            }

            var output = new lexicLemmaInTypeNode(lemmaForm, posType, __name);
            Add(output);
            return output;
        }

        public override graphNodeCustom CreateChildItem(string nameForChild)
        {
            var output = new lexicLemmaInTypeNode();
            output.name = nameForChild;
            return output;
        }
    }
}