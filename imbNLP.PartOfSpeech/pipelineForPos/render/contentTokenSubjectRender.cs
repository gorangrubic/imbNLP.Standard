// --------------------------------------------------------------------------------------------------------------------
// <copyright file="contentTokenSubjectRender.cs" company="imbVeles" >
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
using imbNLP.PartOfSpeech.flags.basic;
using imbNLP.PartOfSpeech.flags.token;
using imbNLP.PartOfSpeech.lexicUnit;
using imbNLP.PartOfSpeech.map;
using imbNLP.PartOfSpeech.pipelineForPos.subject;
using imbSCI.Core.extensions.data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace imbNLP.PartOfSpeech.pipelineForPos.render
{
    public static class contentTokenSubjectRender
    {
        private static void renderGramCase(StringBuilder sb, IEnumerable tags, Boolean fullForm = false)
        {
            foreach (Object pt in tags)
            {
                if (pt is IList)
                {
                    sb.Append(textMapBase.CONTAINER_OPEN);
                    renderGramCase(sb, pt as IList, fullForm);
                    sb.Append(textMapBase.CONTAINER_CLOSE);
                }
                else
                {
                    if (fullForm)
                    {
                        sb.Append(pt.GetType().Name + "." + pt.ToString());
                    }
                    else
                    {
                        sb.Append(pt.ToString());
                    }
                }

                sb.Append(textMapBase.SUBLEVEL_COMMA);
            }

            //sb.Backspace(textMapBase.SUBLEVEL_COMMA);
        }

        /// <summary>
        /// Renders the token into string form
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        private static String renderString(pipelineTaskSubjectContentToken token, contentTokenSubjectRenderMode mode)
        {
            StringBuilder sb = new StringBuilder();

            if (token == null)
            {
                sb.Append(textMapBase.SEPARATOR);
                return sb.ToString();
            }

            switch (mode)
            {
                default:
                case contentTokenSubjectRenderMode.currentForm:
                    sb.Append(token.currentForm);
                    break;

                case contentTokenSubjectRenderMode.lemmaForm:
                    if (token.graph != null)
                    {
                        sb.Append(token.graph.lemmaForm);
                    }
                    else
                    {
                        sb.Append(token.currentForm);
                    }
                    break;

                case contentTokenSubjectRenderMode.descriptive:
                    sb.Append(token.currentForm);
                    if (token.graph != null)
                    {
                        sb.Append(textMapBase.MAINLEVEL_COMMA);
                        sb.Append(token.graph.lemmaForm);
                    }
                    sb.Append(textMapBase.MAINLEVEL_COMMA);
                    renderGramCase(sb, token.flagBag, false);
                    break;

                case contentTokenSubjectRenderMode.flagsForm:
                    renderGramCase(sb, token.flagBag, false);
                    break;

                case contentTokenSubjectRenderMode.flagsFullForm:
                    renderGramCase(sb, token.flagBag, true);
                    break;

                case contentTokenSubjectRenderMode.initialForm:
                    sb.Append(token.initialForm);
                    break;

                case contentTokenSubjectRenderMode.none:
                    break;

                case contentTokenSubjectRenderMode.posTypeAndGramTagForm:
                    //sb.Append("[");
                    sb.Append(renderString(token, contentTokenSubjectRenderMode.posTypeTagForm));
                    //sb.Backspace(textMapBase.SEPARATOR);

                    if (token.graph != null)
                    {
                        for (int i = 0; i < token.graph.Count(); i++)
                        {
                            lexicGrammarCase pt = token.graph[i] as lexicGrammarCase;

                            renderGramCase(sb, pt.tags.GetTags(), false);

                            if (i < token.graph.Count() - 1)
                            {
                                sb.Append(textMapBase.MAINLEVEL_COMMA);
                            }
                        }
                    }

                    //sb.Append("]");
                    break;

                case contentTokenSubjectRenderMode.posTypeTagForm:

                    List<pos_type> posTypeTags = new List<pos_type>();
                    Boolean ok = false;

                    if (token.graph != null)
                    {
                        var pst = token.graph.GetTagFromGramTags<pos_type>();
                        foreach (var ps in pst) posTypeTags.AddUnique(ps);

                        if (posTypeTags.Any()) ok = true;
                    }

                    if (ok == false)
                    {
                        var pst = token.flagBag.getAllOfType<pos_type>(false);
                        foreach (var ps in pst) posTypeTags.AddUnique(ps);
                    }

                    if (!posTypeTags.Any()) posTypeTags.Add(pos_type.none);

                    foreach (pos_type pt in posTypeTags)
                    {
                        if (pt != pos_type.none)
                        {
                            sb.Append(pt.ToString());
                            if (pt != posTypeTags.Last())
                            {
                                sb.Append(textMapBase.SUBLEVEL_COMMA);
                            }
                        }
                    }

                    break;
            }
            sb.Append(" ");
            return sb.ToString();
        }

        //internal static String RenderRegexPosTypePattern()

        /// <summary>
        /// Renders the open.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        public static String renderOpen(this cnt_level level, contentTokenSubjectRenderMode mode)
        {
            switch (level)
            {
                case flags.token.cnt_level.mcChunk:
                    switch (mode)
                    {
                        case contentTokenSubjectRenderMode.currentForm:
                        case contentTokenSubjectRenderMode.lemmaForm:
                            return " ";
                            break;
                    }
                    return "{";
                    break;

                default:
                case flags.token.cnt_level.mcToken:
                    return "";
                    break;

                case flags.token.cnt_level.mcBlock:
                    return Environment.NewLine;
                    break;

                case flags.token.cnt_level.mcTokenStream:
                    return Environment.NewLine;
                    break;
            }
        }

        /// <summary>
        /// Renders the close.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        public static String renderClose(this cnt_level level, contentTokenSubjectRenderMode mode)
        {
            switch (level)
            {
                case flags.token.cnt_level.mcChunk:

                    switch (mode)
                    {
                        case contentTokenSubjectRenderMode.currentForm:
                        case contentTokenSubjectRenderMode.lemmaForm:
                            return " ";
                            break;
                    }

                    return "}";
                    break;

                default:
                case flags.token.cnt_level.mcToken:
                    return "";
                    break;

                case flags.token.cnt_level.mcBlock:
                    return Environment.NewLine;
                    break;

                case flags.token.cnt_level.mcTokenStream:
                    return "";
                    break;
            }
        }

        private static void renderSub(textMap<pipelineTaskSubjectContentToken> output, pipelineTaskSubjectContentToken token, contentTokenSubjectRenderMode mode)
        {
            switch (token.contentLevelType)
            {
                //output.Add(token, renderString(token, mode));
                //break;
                case flags.token.cnt_level.mcToken:
                    output.Add(token, renderString(token, mode));
                    break;

                case flags.token.cnt_level.mcChunk:
                case flags.token.cnt_level.mcBlock:
                case flags.token.cnt_level.mcTokenStream:
                    output.AddOpen(token, token.contentLevelType.renderOpen(mode));
                    foreach (pipelineTaskSubjectContentToken tkn in token)
                    {
                        renderSub(output, tkn, mode);
                    }
                    output.AddClose(token.contentLevelType.renderClose(mode));
                    break;
            }
        }

        /// <summary>
        /// Renders the textMap from specified token subject
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public static textMap<pipelineTaskSubjectContentToken> render(this pipelineTaskSubjectContentToken token, contentTokenSubjectRenderMode mode)
        {
            textMap<pipelineTaskSubjectContentToken> output = new textMap<pipelineTaskSubjectContentToken>();

            renderSub(output, token, mode);

            output.AddPlainRender(textMapBase.SEPARATOR);

            return output;
        }
    }
}