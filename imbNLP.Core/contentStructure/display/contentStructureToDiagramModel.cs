// --------------------------------------------------------------------------------------------------------------------
// <copyright file="contentStructureToDiagramModel.cs" company="imbVeles" >
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
namespace imbNLP.Core.contentStructure.display
{
    using imbNLP.Core.contentStructure.interafaces;
    using imbSCI.Data;
    using imbSCI.Data.interfaces;
    using imbSCI.DataComplex.diagram.builders;
    using imbSCI.DataComplex.diagram.core;
    using imbSCI.DataComplex.diagram.enums;

    /// <summary>
    /// diagram model builder from content structure
    /// </summary>
    /// <seealso cref="imbSCI.DataComplex.diagram.builders.diagramBuilderUniversalTree" />
    public class contentStructureToDiagramModel : diagramBuilderUniversalTree
    {
        public override string getNodeName(IObjectWithPathAndChildSelector source, string defName = "")
        {
            string output = defName;

            if (source is IContentPage)
            {
                output = "C";
            }
            else if (source is IContentBlock)
            {
                output = "B";
            }
            else if (source is IContentParagraph)
            {
                output = "P";
            }
            else if (source is IContentSentence)
            {
                output = "S";
            }
            else if (source is IContentToken)
            {
                output = "T";
            }

            if (source is IContentElement)
            {
                IContentElement source_IContentToken = (IContentElement)source;
                output = output + source_IContentToken.id;
            }

            return output;
        }

        public override string getNodeDescription(IObjectWithPathAndChildSelector source, string defDescription = "")
        {
            string output = defDescription;

            if (source is IContentPage)
            {
                output = output.add("Page", " ");
            }
            else if (source is IContentBlock)
            {
                output = output.add("Block", " "); // "Block";
            }
            else if (source is IContentParagraph)
            {
                output = output.add("Paragraph", " ");
            }
            else if (source is IContentSentence)
            {
                output = output.add("Sentence", " ");
            }
            else if (source is IContentToken)
            {
                output = output;
                //IContentToken source_IContentToken = (IContentToken)source;
                //String tkn = source_IContentToken.content.TrimToMaxLength(5, "...");

                //output = output.add(tkn, " ");
            }

            return output;
        }

        public override string getLinkDescription(diagramLink child, string defDescription = "")
        {
            if (child.relatedObject is IContentBlock)
            {
                return "block";
            }
            if (child.relatedObject is IContentParagraph)
            {
                return "paragraph";
            }
            if (child.relatedObject is IContentSentence)
            {
                return "sentence";
            }
            if (child.relatedObject is IContentToken)
            {
                return "token";
            }
            return defDescription;
        }

        public override int getColor(diagramNode child, int defColor = 1)
        {
            if (child.relatedObject is IContentBlock)
            {
                return 1;
            }
            if (child.relatedObject is IContentParagraph)
            {
                return 2;
            }
            if (child.relatedObject is IContentSentence)
            {
                return 3;
            }
            if (child.relatedObject is IContentToken)
            {
                return 4;
            }
            return 5;
        }

        public override diagramLinkTypeEnum getLinkTypeEnum(diagramNode child, diagramLinkTypeEnum defType = diagramLinkTypeEnum.normal)
        {
            if (child.relatedObject is IContentBlock)
            {
                return diagramLinkTypeEnum.thick;
            }
            if (child.relatedObject is IContentParagraph)
            {
                return diagramLinkTypeEnum.normal;
            }
            if (child.relatedObject is IContentSentence)
            {
                return diagramLinkTypeEnum.normal;
            }
            if (child.relatedObject is IContentToken)
            {
                return diagramLinkTypeEnum.dotted;
            }
            return defType;
        }

        public override diagramNodeShapeEnum getShapeTypeEnum(IObjectWithPathAndChildSelector child, diagramNodeShapeEnum defType = diagramNodeShapeEnum.normal)
        {
            if (child is IContentBlock)
            {
                return diagramNodeShapeEnum.circle;
            }
            if (child is IContentParagraph)
            {
                return diagramNodeShapeEnum.rhombus;
            }
            if (child is IContentSentence)
            {
                return diagramNodeShapeEnum.rounded;
            }
            if (child is IContentToken)
            {
                return diagramNodeShapeEnum.normal;
            }

            return defType;
        }
    }
}