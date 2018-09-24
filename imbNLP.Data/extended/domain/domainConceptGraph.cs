// --------------------------------------------------------------------------------------------------------------------
// <copyright file="domainConceptGraph.cs" company="imbVeles" >
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
namespace imbNLP.Data.extended.domain
{
    using imbACE.Services.terminal;
    using imbNLP.Data.semanticLexicon;
    using imbNLP.Data.semanticLexicon.core;
    using imbSCI.Core.reporting;
    using imbSCI.Data.collection.graph;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Concept = semanticLexicon.Concept;
    using TermLemma = semanticLexicon.TermLemma;

    /// <summary>
    /// Lexion conceptual hierarchy construction
    /// </summary>
    /// <seealso cref="imbSCI.Data.collection.graph.graphWrapNode{imbNLP.Data.extended.domain.domainConceptEntry}" />
    /// <seealso cref="System.Collections.Generic.IEnumerable{imbSCI.Data.collection.graph.graphWrapNode{imbNLP.Data.extended.domain.domainConceptEntry}}" />
    public class domainConceptGraph : graphWrapNode<domainConceptEntry>, IEnumerable<graphWrapNode<domainConceptEntry>>
    {
        public domainConceptGraph(String __name)
        {
            name = __name;
        }

        public override string pathSeparator
        {
            get
            {
                return "-";
            }
        }

        private List<graphWrapNode<domainConceptEntry>> _toConnect = new List<graphWrapNode<domainConceptEntry>>();

        /// <summary>
        ///
        /// </summary>
        public List<graphWrapNode<domainConceptEntry>> toConnect
        {
            get { return _toConnect; }
            set { _toConnect = value; }
        }

        /// <summary>
        /// Connects to the hooks
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="simulation">if set to <c>true</c> [simulation].</param>
        public void connectToHooks(ILogBuilder response, Boolean simulation = true)
        {
            response.AppendLine("Connecting concepts by the hooks");

            if (simulation)
            {
                response.AppendLine("Running in the simulation mode -- no changes will be saved. ");
            }

            foreach (domainConceptEntry parent in toConnect)
            {
                List<Concept> concepts = new List<Concept>();
                List<TermLemma> lemmas = new List<TermLemma>();
                foreach (String needle in parent.needles)
                {
                    var reLem = semanticLexiconManager.manager.resolve(needle);
                    List<IConcept> re_concepts = new List<IConcept>();
                    List<ITermLemma> re_synonyms = new List<ITermLemma>();
                    Boolean found = false;
                    foreach (var re in reLem)
                    {
                        found = true;
                        //re_concepts.AddRange(re.concepts);
                        foreach (IConcept rec in re.concepts)
                        {
                            if (!re_concepts.Any(x => x.name == rec.name)) re_concepts.Add(rec);
                        }
                        foreach (TermLemma rel in re.relatedTo)
                        {
                            if (!re_synonyms.Any(x => x.name == rel.name)) re_synonyms.Add(rel);
                        }
                        foreach (TermLemma rel in re.relatedFrom)
                        {
                            if (!re_synonyms.Any(x => x.name == rel.name)) re_synonyms.Add(rel);
                        }
                        re_synonyms.Add(re);
                    }

                    if (!simulation)
                    {
                        if (re_concepts.Contains(parent.concept))
                        {
                            response.log("[" + parent.concept.name + "] [" + needle + "] hook is already deployed ");
                            continue;
                        }
                    }

                    if (found)
                    {
                        String pname = parent.name;
                        if (!simulation)
                        {
                            pname = parent.concept.name;
                        }

                        response.log("[" + pname + "] [" + needle + "] lemmas[" + re_synonyms.Count + "] concepts[" + re_concepts.Count + "] ");

                        if (re_concepts.Any())
                        {
                            response.log("Connecting [" + pname + "] -->  concepts[" + re_concepts.Count + "] ");

                            foreach (Concept c in re_concepts)
                            {
                                if (c != parent.concept)
                                {
                                    response.log("--- [" + pname + "] -->  concept[" + c.name + "] ");

                                    if (!simulation) parent.concept.hypoConcepts.Add(c);
                                }
                            }
                        }
                        else
                        {
                            response.log("Connecting [" + pname + "] -->  lemmas[" + re_synonyms.Count() + "] ");

                            foreach (var c in re_synonyms)
                            {
                                response.log("--- [" + pname + "] -->  lemma[" + c.name + "] ");

                                if (!simulation) parent.concept.lemmas.Add(c);
                            }
                        }
                    }
                    else
                    {
                        response.log("Hook [" + needle + "] failed as no lemma found");
                    }
                }
            }

            if (simulation)
            {
            }
            else
            {
                Boolean doSave = aceTerminalInput.askYesNo("Do you want to save changes to the triplestore?");
                if (doSave)
                {
                    semanticLexiconManager.manager.lexiconContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Builds the conceptual mesh.
        /// </summary>
        /// <param name="response">The response.</param>
        public void buildConceptualMesh(ILogBuilder response, Boolean simulation = true)
        {
            List<graphWrapNode<domainConceptEntry>> output = new List<graphWrapNode<domainConceptEntry>>();
            List<graphWrapNode<domainConceptEntry>> stack = new List<graphWrapNode<domainConceptEntry>>();

            response.AppendLine("Building the conceptual mesh");

            if (simulation)
            {
                response.AppendLine("Running in the simulation mode -- no changes will be saved. ");
            }

            stack.AddRange(this);

            while (stack.Any())
            {
                var n_stack = new List<graphWrapNode<domainConceptEntry>>();

                foreach (graphWrapNode<domainConceptEntry> parent in stack)
                {
                    var pConcept = semanticLexiconManager.manager.getConcept(parent.path, !simulation, parent.item.description, false);
                    parent.item.concept = pConcept;
                    //

                    foreach (graphWrapNode<domainConceptEntry> child in parent)
                    {
                        if (!child.isNullNode)
                        {
                            var cConcept = semanticLexiconManager.manager.getConcept(child.path, !simulation, child.item.description, false);
                            child.item.concept = cConcept;
                            if (!simulation) pConcept.hypoConcepts.Add(cConcept);

                            response.AppendLine("[" + parent.name + "]->[" + child.name + "]");
                        }
                        else
                        {
                            domainConceptEntry newit = new domainConceptEntry(child.name, "", "");
                            child.SetItem(newit);
                            var cConcept = semanticLexiconManager.manager.getConcept(child.path, !simulation, child.item.description, false);
                            child.item.concept = cConcept;
                            if (!simulation) pConcept.hypoConcepts.Add(cConcept);

                            response.AppendLine("[" + parent.name + "]->[" + child.name + "] is a null node");
                        }
                    }

                    if (!parent.isNullNode)
                    {
                        if (parent.item.needles.Any())
                        {
                            toConnect.Add(parent);
                        }
                    }
                    n_stack.AddRange(parent);
                }
                stack = n_stack;
            }

            response.AppendLine("Concepts with hooks to connect: " + toConnect.Count());

            if (simulation)
            {
            }
            else
            {
                semanticLexiconManager.manager.lexiconContext.SaveChanges();
            }
        }
    }
}