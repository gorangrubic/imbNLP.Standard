// --------------------------------------------------------------------------------------------------------------------
// <copyright file="chunkComposerBasic.cs" company="imbVeles" >
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
using imbNLP.PartOfSpeech.flags.token;
using imbNLP.PartOfSpeech.map;
using imbNLP.PartOfSpeech.pipeline.postprocessor;
using imbNLP.PartOfSpeech.pipelineForPos.render;
using imbNLP.PartOfSpeech.pipelineForPos.subject;
using imbSCI.Core.extensions.data;
using imbSCI.Core.extensions.text;
using imbSCI.Core.reporting;
using imbSCI.Data;
using imbSCI.Data.collection.graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace imbNLP.PartOfSpeech.decomposing.chunk
{
    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="imbNLP.PartOfSpeech.pipeline.postprocessor.pipelinePostprocessBase{imbNLP.PartOfSpeech.pipelineForPos.subject.pipelineTaskSubjectContentToken, imbNLP.PartOfSpeech.decomposing.chunk.chunkComposerBasicSettings}" />
    public class chunkComposerBasic : pipelinePostprocessBase<pipelineTaskSubjectContentToken, chunkComposerBasicSettings>
    {
        public chunkComposerBasic() : base(null)
        {
        }

        //  public chunkComposerBasicSettings settings { get; set; } = new chunkComposerBasicSettings();

        /// <summary>
        /// Creates multi-line description of current configuration
        /// </summary>
        /// <returns>List of description lines</returns>
        public override List<string> DescribeSelf()
        {
            List<String> output = new List<string>();

            output.Add("### Iterative POS chunk constructor");

            output.AddRange(settings.DescribeSelf());

            return output;
        }

        public override List<pipelineTaskSubjectContentToken> process(IEnumerable<pipelineTaskSubjectContentToken> _input, ILogBuilder logger)
        {
            settings.checkReady();

            List<pipelineTaskSubjectContentToken> output = new List<pipelineTaskSubjectContentToken>();
            List<pipelineTaskSubjectContentToken> next = new List<pipelineTaskSubjectContentToken>();

            next = _input.ToList();

            next.Sort((x, y) => String.CompareOrdinal(x.currentForm, y.currentForm));

            while (currentIteration > 0)
            {
                List<pipelineTaskSubjectContentToken> MCNext = new List<pipelineTaskSubjectContentToken>();

                foreach (pipelineTaskSubjectContentToken sub in next)
                {
                    MCNext.AddRange(processIteration(sub), true);
                }

                if (settings.keepAllInOutput)
                {
                    output.AddRange(MCNext, true);
                }
                else
                {
                    output = MCNext;
                }

                logger.log("[" + currentIteration + "] chunk construction in[" + next.Count + "] new[" + MCNext.Count + "] out[" + output.Count + "]");

                if (next.Count == output.Count)
                {
                    logger.log("Aborting the process since last iteation produced no changes");
                    break;
                }
                next = MCNext.ToList();

                if (MCNext.Count == 0) break;
                currentIteration--;
            }

            return output;

            //return base.process(_input, logger);
        }

        protected override List<pipelineTaskSubjectContentToken> processIteration(pipelineTaskSubjectContentToken streamSubject)
        {
            List<pipelineTaskSubjectContentToken> chunks = new List<pipelineTaskSubjectContentToken>();

            if (streamSubject.contentLevelType == cnt_level.mcChunk)
            {
                chunks.Add(streamSubject);
            }

            foreach (imbSCI.Data.interfaces.IObjectWithPathAndChildren chk in streamSubject)
            {
                pipelineTaskSubjectContentToken ch = chk as pipelineTaskSubjectContentToken;
                if (ch.contentLevelType == cnt_level.mcChunk)
                {
                }
            }

            var subchk = pipelineSubjectTools.GetSubjectChildrenTokenType<pipelineTaskSubjectContentToken, IGraphNode>(streamSubject, new cnt_level[] { cnt_level.mcChunk }, true);

            chunks.AddRange(subchk);

            imbMCDocumentElement stream = streamSubject.mcElement as imbMCDocumentElement;

            if (stream == null)
            {
                return chunks;
            }

            subjectRenderLayers layers = new subjectRenderLayers();

            foreach (chunkMatchRule rule in settings.rules)
            {
                textMap<pipelineTaskSubjectContentToken> typeTagFormMap = layers.render(streamSubject, rule.renderMode);

                MatchCollection mchs = rule.regex.Matches(typeTagFormMap.render);
                List<List<pipelineTaskSubjectContentToken>> mchs_s = typeTagFormMap.Select(mchs);

                List<List<pipelineTaskSubjectContentToken>> mchs_subjects = new List<List<pipelineTaskSubjectContentToken>>();
                foreach (List<pipelineTaskSubjectContentToken> mg in mchs_s)
                {
                    List<pipelineTaskSubjectContentToken> mgc = new List<pipelineTaskSubjectContentToken>();
                    foreach (pipelineTaskSubjectContentToken m in mg)
                    {
                        if (rule.contentLevel.Contains(m.contentLevelType))
                        {
                            mgc.Add(m);
                        }
                    }
                    if (mgc.Any())
                    {
                        mchs_subjects.Add(mgc);
                    }
                }

                foreach (List<pipelineTaskSubjectContentToken> mGroup in mchs_subjects)
                {
                    String tkn = imbStringGenerators.getRandomString(4);

                    Boolean createChunk = true;

                    foreach (pipelineTaskSubjectContentToken s in mGroup)
                    {
                        tkn = tkn + s.name;
                    }

                    if (rule.flagTypesToMatch.Any())
                    {
                        Dictionary<Type, Object> flags = new Dictionary<Type, object>();

                        foreach (pipelineTaskSubjectContentToken s in mGroup)
                        {
                            foreach (Type flagType in rule.flagTypesToMatch)
                            {
                                Object fl = s.flagBag.FirstOrDefault(x => x.GetType() == flagType);
                                if (fl == null) continue;

                                if (!flags.ContainsKey(flagType))
                                {
                                    if (fl != null)
                                    {
                                        flags.Add(flagType, fl);
                                    }
                                    continue;
                                }

                                if (settings.doCheckGramTagCriteria)
                                {
                                    if (flags[flagType] != fl)
                                    {
                                        createChunk = false;
                                        break;
                                    }
                                }
                            }
                            if (createChunk == false)
                            {
                                break;
                            }
                        }
                    }

                    if (createChunk)
                    {
                        pipelineTaskSubjectContentToken chunkSubject = new pipelineTaskSubjectContentToken();
                        chunkSubject.name = tkn;

                        streamSubject.Add(chunkSubject);

                        imbMCChunk chunk = new imbMCChunk();
                        chunk.name = tkn;

                        chunkSubject.contentLevelType = flags.token.cnt_level.mcChunk;
                        chunkSubject.flagBag.Add(rule.chunkType);
                        chunkSubject.mcElement = chunk;

                        if (stream != null)
                        {
                            chunk.htmlNode = stream.htmlNode;
                        }

                        chunk.position = mGroup.Min(x => x.mcElement.position);

                        stream.Add(chunk);

                        List<Object> commonFlags = new List<object>();
                        List<Object> forbidenFlags = new List<object>();
                        Boolean isFirstSubject = true;

                        foreach (pipelineTaskSubjectContentToken s in mGroup)
                        {
                            s.mcElement.removeFromParent();
                            chunk.Add(s.mcElement);

                            s.removeFromParent();
                            chunkSubject.Add(s);

                            if (isFirstSubject)
                            {
                                commonFlags.AddRange(s.flagBag, true);
                                isFirstSubject = false;
                            }
                            else
                            {
                                foreach (Object flag in s.flagBag)
                                {
                                    if (!forbidenFlags.Contains(flag))
                                    {
                                        if (!commonFlags.Contains(flag))
                                        {
                                            forbidenFlags.AddUnique(flag);
                                        }
                                    }
                                    else
                                    {
                                        commonFlags.Remove(flag);
                                    }
                                }
                            }
                        }

                        chunkSubject.initialForm = chunkSubject.render(contentTokenSubjectRenderMode.currentForm).render;
                        var lemmaForm = chunkSubject.render(contentTokenSubjectRenderMode.lemmaForm);
                        chunkSubject.currentForm = lemmaForm.GetCleanRender();

                        chunk.content = chunkSubject.currentForm;

                        chunkSubject.flagBag.Clear();
                        chunkSubject.flagBag.AddRange(commonFlags, true);

                        chunks.Add(chunkSubject);
                    }
                }
            }

            return chunks;
        }
    }
}