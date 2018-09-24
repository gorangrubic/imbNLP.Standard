// --------------------------------------------------------------------------------------------------------------------
// <copyright file="lexicLemmaInTypeNode.cs" company="imbVeles" >
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

    /// <summary>
    /// Represents one lemma form per <see cref="pos_type"/>
    /// </summary>
    /// <seealso cref="imbNLP.PartOfSpeech.lexicUnit.lexicGraph" />
    [Serializable]
    public class lexicLemmaInTypeNode : lexicGraph
    {
        public pos_type posType { get; set; }

        public String lemmaForm { get; set; }

        public lexicLemmaInTypeNode() : base()
        {
        }

        public lexicInflection AddInflection(String inflectForm)
        {
            if (mychildren.ContainsKey(inflectForm)) return mychildren[inflectForm] as lexicInflection;

            var output = Add(inflectForm) as lexicInflection;
            output.lemmaForm = lemmaForm;

            return output;
        }

        public lexicLemmaInTypeNode(String lemmaForm, pos_type _posType, String __name = "") : base()
        {
            if (__name == "") __name = lemmaForm + "|" + posType.ToString();
            name = __name;
            lemmaForm = lemmaForm;
            nodeType = lexicGraphNodeType.lemma;
            posType = _posType;
        }

        public override graphNodeCustom CreateChildItem(string nameForChild)
        {
            var output = new lexicInflection();
            output.name = nameForChild;
            return output;
        }
    }
}