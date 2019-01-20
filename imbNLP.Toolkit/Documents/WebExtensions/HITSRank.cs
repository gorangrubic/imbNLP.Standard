// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HITSRank.cs" company="imbVeles" >
//
// Copyright (C) 2017 imbVeles
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
// Project: imbCommonModels
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
namespace imbNLP.Toolkit.Documents.WebExtensions
{

    using imbSCI.Data.collection.math;
    using System;
    using System.Collections.Generic;

    public class HITSRank
    {
        //    var matrix = wRecord.context.targets.GetLinkMatrix();
        //    pageRank = new PageRank(matrix, alpha, convergence, checkSteps);

        //    Double[] dbl = pageRank.ComputePageRank();
        //    List<Int32> pri = new List<int>();
        //        foreach (Double db in dbl)
        //        {
        //            pri.Add(Convert.ToInt32(db* scoreUnit));
        //}

        //ranks = wRecord.context.targets.linkMatrix.MapToY(pri);

        // public HITSRank(linkMatrix

        public Boolean ContainsKey(String target)
        {
            return targetToScore.ContainsKey(target);
        }

        public HITSScore this[String target]
        {
            get
            {
                return targetToScore[target];
            }
        }

        /// <summary> </summary>
        protected Dictionary<string, HITSScore> targetToScore { get; set; } = new Dictionary<string, HITSScore>();

        public HITSRank()
        {
        }

        public void recalculate(aceRelationMatrix<String, String, Int32> matrix, double convergence = 0.0001, int checkSteps = 20)
        {
            targetToScore = new Dictionary<string, HITSScore>();
            var targets = matrix.GetXAxis();

            foreach (String target in targets)
            {
                targetToScore.Add(target, new HITSScore(1, 1));
            }



            if (matrix == null)
            {
                return;
            }

            for (int i = 0; i < checkSteps; i++)
            {
                // <---- resets value change record, in order to detect convergence at the end of iteration
                foreach (String target in targets)
                {
                    targetToScore[target].resetChangeMeasure();
                }

                // <--- Authority
                foreach (String xTarget in matrix.GetXAxis())
                {
                    foreach (String yTarget in matrix.GetYAxis())
                    {
                        targetToScore[xTarget].a += targetToScore[yTarget].h;
                    }
                }

                // Normalize authority
                double nfact = 0;
                foreach (String target in targets)
                {
                    nfact += Math.Pow(targetToScore[target].a, 2);
                }
                nfact = Math.Sqrt(nfact);

                foreach (String target in targets)
                {
                    if (nfact != 0)
                    {
                        targetToScore[target].a = targetToScore[target].a / nfact;
                    }
                }

                // <--- hub

                foreach (String xTarget in matrix.GetXAxis())
                {
                    foreach (String yTarget in matrix.GetYAxis())
                    {
                        targetToScore[yTarget].h += targetToScore[xTarget].a;
                    }
                }

                // <--- normalization
                nfact = 0;
                foreach (String target in targets)
                {
                    nfact += Math.Pow(targetToScore[target].h, 2);
                }
                nfact = Math.Sqrt(nfact);

                foreach (String target in targets)
                {
                    targetToScore[target].resetChangeMeasure();
                    if (nfact != 0)
                    {
                        targetToScore[target].h = targetToScore[target].h / nfact;
                    }
                }

                double maxChange = 0;

                // <---- resets value change record, in order to detect convergence at the end of iteration
                foreach (String target in targets)
                {
                    maxChange = Math.Max(targetToScore[target].a_delta, maxChange);
                    maxChange = Math.Max(targetToScore[target].h_delta, maxChange);
                }

                if (maxChange < convergence)
                {
                    break;
                }
                else
                {
                }
            }
        }
    }
}