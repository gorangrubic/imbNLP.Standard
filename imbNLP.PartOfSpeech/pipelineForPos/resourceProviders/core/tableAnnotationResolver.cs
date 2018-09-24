// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tableAnnotationResolver.cs" company="imbVeles" >
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
using System;
using System.Collections.Generic;

namespace imbNLP.PartOfSpeech.resourceProviders.core
{
    using imbNLP.PartOfSpeech.flags.data;
    using imbSCI.Core.extensions.enumworks;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.extensions.typeworks;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.collection.nested;
    using System.Data;

    /// <summary>
    /// Annotation resolver, uses definitions from Excel table
    /// </summary>
    /// <seealso cref="imbNLP.PartOfSpeech.resourceProviders.core.tableResourceResolverBase" />
    public class tableAnnotationResolver : tableResourceResolverBase
    {
        protected aceDictionarySet<String, Object> items { get; set; } = new aceDictionarySet<string, object>();

        protected Dictionary<String, Type> annotationTypes { get; set; } = new Dictionary<string, Type>();

        protected void deploy(Type hostPostType)
        {
            annotationTypes = hostPostType.CollectTypes(CollectTypeFlags.includeEnumTypes | CollectTypeFlags.ofParentNamespace | CollectTypeFlags.ofThisAssembly);

            foreach (DataRow dr in sourceTable.Rows)
            {
                var parts = dr[1].toStringSafe().SplitSmart(".", "", true, true);

                if (parts.Count < 2)
                {
                    continue;
                }

                Type t = annotationTypes[parts[0]];

                Object val = t.getEnumByName(parts[1], null);

                if (val != null)
                {
                    items.Add(dr[0].toStringSafe(), val);
                }
            }
        }

        /// <summary>
        /// Processes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public List<Object> process(String input)
        {
            List<Object> output = new List<object>();
            foreach (var pair in items)
            {
                if (input.Contains(pair.Key))
                {
                    output.AddRange(pair.Value);
                }
            }
            return output;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="tableAnnotationResolver"/> class.
        /// </summary>
        /// <param name="annotationTablePath">The annotation table path.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="hostType">Type of the host.</param>
        public tableAnnotationResolver(String annotationTablePath, ILogBuilder logger = null, Type hostType = null) : base(annotationTablePath, logger)
        {
            if (hostType == null) hostType = typeof(dat_business);

            deploy(hostType);
        }
    }
}