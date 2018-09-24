// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineModelTypeManager.cs" company="imbVeles" >
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
using imbACE.Core.plugins;
using imbNLP.PartOfSpeech.pipeline.core;
using imbSCI.Core.reporting;
using System;

namespace imbNLP.PartOfSpeech.nlpTools
{
    /// <summary>
    /// Type manager
    /// </summary>
    public class pipelineModelTypeManager : internalPluginManager<IPipelineModel>
    {
        protected override bool supportDirtyNaming { get { return true; } }

        public void LoadTypes(ILogBuilder loger)
        {
            loadPlugins(loger);
        }

        /// <summary>
        /// Gets the of model
        /// </summary>
        /// <param name="crawler_classname">The crawler classname.</param>
        /// <param name="loger">The loger.</param>
        /// <returns></returns>
        public IPipelineModel GetInstance(String crawler_classname, ILogBuilder loger)
        {
            return GetPluginInstance(crawler_classname, "", loger, null);
        }
    }
}