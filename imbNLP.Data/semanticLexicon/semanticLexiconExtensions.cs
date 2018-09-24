// --------------------------------------------------------------------------------------------------------------------
// <copyright file="semanticLexiconExtensions.cs" company="imbVeles" >
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
namespace imbNLP.Data.semanticLexicon
{
    using imbNLP.Data.semanticLexicon.core;
    using System.Collections.Generic;
    using System.Linq;

    public static class semanticLexiconExtensions
    {
        public static List<TermLemma> getAllRelated(this ITermLemma lemma)
        {
            List<TermLemma> re_synonyms = new List<TermLemma>();
            foreach (TermLemma rel in lemma.relatedTo)
            {
                if (!re_synonyms.Any(x => x.name == rel.name)) re_synonyms.Add(rel);
            }
            foreach (TermLemma rel in lemma.relatedFrom)
            {
                if (!re_synonyms.Any(x => x.name == rel.name)) re_synonyms.Add(rel);
            }
            return re_synonyms;
        }

        public static void SetBrightStarDB()
        {
            // BrightstarDB.Configuration.EmbeddedServiceConfiguration.PreloadConfiguration.Enabled = true;
            //BrightstarDB.Configuration.EnableQueryCache = true;
            //BrightstarDB.Configuration.QueryCacheDirectory = Directory.CreateDirectory("datacache\\rdf").FullName;
            //BrightstarDB.Configuration.QueryCacheDiskSpace = 18200;
            //BrightstarDB.Configuration.QueryCacheMemory = 8200;
            //BrightstarDB.Configuration.ReadStoreObjectCacheSize = 8200;
            //BrightstarDB.Configuration.PersistenceType = BrightstarDB.Storage.PersistenceType.Rewrite;
        }
    }
}