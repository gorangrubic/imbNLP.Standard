// --------------------------------------------------------------------------------------------------------------------
// <copyright file="htmlContentPage.cs" company="imbVeles" >
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
// Project: imbNLP.Core
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
namespace imbNLP.Core.contentStructureHtml.elements
{
    using imbNLP.Core.contentStructure.display;
    using imbNLP.Core.contentStructure.elements;
    using imbNLP.Core.contentStructure.interafaces;
    using imbSCI.Core.extensions.data;
    using imbSCI.Data;
    using System.Data;

    /// <summary>
    /// 2013c: LowLevel resurs
    /// </summary>

    public class htmlContentPage : contentPage, IContentPage
    {
        public htmlContentPage() : base()
        {
        }

        /// <summary>
        /// Appends its data points into new or existing property collection
        /// </summary>
        /// <param name="data">Property collection to add data into</param>
        /// <returns>Updated or newly created property collection</returns>
        public PropertyCollection AppendDataFields(PropertyCollection data = null)
        {
            if (data == null) data = new PropertyCollection();

            //PropertyCollectionExten data_ext = new PropertyCollectionExtended();

            data.add(templateFieldTKNReport.tkn_tokencount, tokens.Count);
            data.add(templateFieldTKNReport.tkn_sentencecount, sentences.Count);
            data.add(templateFieldTKNReport.tkn_paragraphcount, paragraphs.Count);
            data.add(templateFieldTKNReport.tkn_blockcount, items.Count);

            data.add(templateFieldTKNReport.tkn_chunkcount, chunks.Count);

            data.add(templateFieldTKNReport.tkn_linkcount, chunks.Count);

            // data[target.target_name] = name;
            // data[target.target_description] = description;
            // data[target.target_id] = id;
            // data[target.target_url] = url;
            return data;
        }
    }
}