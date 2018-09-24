// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelinePostprocessBase.cs" company="imbVeles" >
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
using imbNLP.PartOfSpeech.pipeline.machine;
using imbSCI.Core.extensions.data;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace imbNLP.PartOfSpeech.pipeline.postprocessor
{
    /// <summary>
    /// Designed for stand alone iterative processing of <see cref="IPipelineTaskSubject"/> instances
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TSettings">The type of the settings.</typeparam>
    public abstract class pipelinePostprocessBase<T, TSettings> where T : IPipelineTaskSubject
        where TSettings : pipelinePostprocessSettings, new()
    {
        /// <summary>
        /// Describes self in multiple lines. Description contains the most important settings and way of operation
        /// </summary>
        /// <returns></returns>
        public abstract List<String> DescribeSelf();

        protected pipelinePostprocessBase(TSettings _settings)
        {
            settings = _settings;
            reset();
        }

        public TSettings settings { get; set; } = new TSettings();

        protected abstract List<T> processIteration(T item);

        [XmlIgnore]
        public Int32 currentIteration { get; set; } = 0;

        public void reset()
        {
            if (settings == null) settings = new TSettings();
            currentIteration = settings.iterations;
        }

        /// <summary>
        /// Processes the specified input.
        /// </summary>
        /// <param name="_input">The input.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public virtual List<T> process(IEnumerable<T> _input, ILogBuilder logger)
        {
            List<T> output = new List<T>();
            List<T> next = new List<T>();

            next = _input.ToList();

            while (currentIteration > 0)
            {
                List<T> MCNext = new List<T>();

                foreach (T sub in next)
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
        }
    }
}