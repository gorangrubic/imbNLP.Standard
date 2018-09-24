// --------------------------------------------------------------------------------------------------------------------
// <copyright file="itmConstructorBasic.cs" company="imbVeles" >
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
using imbMiningContext.TFModels.ILRT;
using imbNLP.PartOfSpeech.map;

// // using imbMiningContext.TFModels.WLF_ISF;
using imbNLP.PartOfSpeech.TFModels.industryLemma.core;
using imbNLP.PartOfSpeech.TFModels.webLemma.table;
using imbSCI.Data;
using imbSCI.Data.collection.nested;
using imbSCI.DataComplex.special;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.PartOfSpeech.TFModels.industryLemma
{
    /// <summary>
    /// Basic type of constructor that builds IndustryTermModel rank table
    /// </summary>
    /// <seealso cref="imbNLP.PartOfSpeech.TFModels.industryLemma.core.IIndustryTermModelConstructor" />
    public class itmConstructorBasic : IIndustryTermModelConstructor
    {
        public itmConstructorSettings settings { get; set; } = new itmConstructorSettings();

        public industryLemmaRankTable process(webLemmaTermTable chunkTable, webLemmaTermTable termTable, industryLemmaRankTable output)
        {
            List<webLemmaTerm> allChunks = chunkTable.GetList();

            var docSetFreq = allChunks.Where(x => x.documentSetFrequency > 1);

            instanceCountCollection<String> termCounter = new instanceCountCollection<string>();

            aceDictionarySet<String, String> dict = new aceDictionarySet<string, string>();

            foreach (webLemmaTerm chunk in docSetFreq)
            {
                var lemmas = chunk.nominalForm.SplitSmart(textMapBase.SEPARATOR, "", true, true);
                lemmas = lemmas.Where(x => x.Length > 2).ToList();
                termCounter.AddInstanceRange(lemmas);

                foreach (String lm in lemmas)
                {
                    foreach (String lmi in lemmas)
                    {
                        if (lmi != lm)
                        {
                            dict[lm].AddUnique(lmi);
                        }
                    }
                }
            }

            List<String> primaries = new List<string>();

            foreach (var pair in termCounter)
            {
                if (termCounter[pair] > 1)
                {
                    primaries.Add(pair);
                    industryLemmaTerm lemma = output.GetOrCreate(pair);
                    lemma.termType = industryLemmaTermType.primary;
                    lemma.weight = settings.PrimaryTermFactor * termTable[lemma.name].weight;

                    lemma.nominalForm = pair;
                    output.AddOrUpdate(lemma);

                    if (dict.ContainsKey(lemma.nominalForm))
                    {
                        foreach (String secLemmas in dict[lemma.nominalForm])
                        {
                            industryLemmaTerm lemmaSec = output.GetOrCreate(secLemmas);
                            if (lemmaSec.termType == industryLemmaTermType.none)
                            {
                                lemmaSec.termType = industryLemmaTermType.secondary;
                                lemmaSec.weight = settings.SecondaryTermFactor * termTable[lemmaSec.name].weight;
                                lemmaSec.nominalForm = secLemmas;
                                output.AddOrUpdate(lemmaSec);
                            }
                        }
                    }
                }
            }

            //var reserveChunks = allChunks.Where(x => x.nominalForm.ContainsAny(primaries));

            //aceDictionarySet<String, String> dictReserve = new aceDictionarySet<string, string>();

            //foreach (webLemmaTerm chunk in reserveChunks)
            //{
            //    var lemmas = chunk.nominalForm.SplitSmart(textMapBase.SEPARATOR, "", true, true);
            //    lemmas = lemmas.Where(x => x.Length > 2).ToList();

            //    String prim = lemmas.FirstOrDefault(x => primaries.Contains(x));

            //    if (!prim.isNullOrEmpty())
            //    {
            //        foreach (String lm in lemmas)
            //        {
            //            if (prim != lm)
            //            {
            //                dictReserve[prim].AddUnique(lm);
            //            }
            //        }
            //    }

            //}

            //foreach (String prim in primaries)
            //{
            //    if (dictReserve.ContainsKey(prim))
            //    {
            //        foreach (String res in dictReserve[prim])
            //        {
            //            industryLemmaTerm resLemma = output.GetOrCreate(res);
            //            if (resLemma.termType == industryLemmaTermType.none)
            //            {
            //                resLemma.nominalForm = res;
            //                resLemma.weight = settings.ReserveTermFactor  *termTable[resLemma.name].weight;
            //                resLemma.termType = industryLemmaTermType.reserve;
            //            }
            //            output.AddOrUpdate(resLemma);
            //        }

            //    }
            //}

            return output;
        }

        public industryLemmaRankTable GetTable()
        {
            industryLemmaRankTable output = null;

            return output;
        }
    }
}