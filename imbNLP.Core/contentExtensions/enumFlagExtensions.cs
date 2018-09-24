// --------------------------------------------------------------------------------------------------------------------
// <copyright file="enumFlagExtensions.cs" company="imbVeles" >
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
using imbNLP.Data.enums.flags;
using imbSCI.Core.extensions.enumworks;
using System;

namespace imbNLP.Core.contentExtensions
{
    public static class enumFlagExtensions
    {
        public static Boolean ContainsAll(this contentTokenFlag flags, params contentTokenFlag[] tests)
        {
            foreach (contentTokenFlag f in tests)
            {
                if (flags.HasFlag(f)) return false;
            }
            return true;
        }

        /*
        public static Boolean ContainsOnly(this IList flags, params Object[] tests)
        {
            foreach (Object f in flags)
            {
                if (!tests.Contains(f)) return false;
            }
            return true;
        }

        public static Boolean ContainsAll (this IList flags, params Object[] tests)
        {
            foreach (Object f in tests)
            {
                if (!flags.Contains(f)) return false;
            }
            return true;
        }

        public static Boolean ContainsOneOrMore(this IList flags, params Object[] tests)
        {
            foreach (Object f in flags)
            {
                if (tests.Contains(f)) return true;
            }
            return false;
        }

        public static Boolean ContainsOneOrMore(this contentTokenFlag flags, params contentTokenFlag[] tests)
        {
            foreach (contentTokenFlag f in tests)
            {
                if (flags.HasFlag(f)) return true;
            }
            return false;
        }

       */

        public static contentTokenFlag Add(this contentTokenFlag flags, contentTokenFlag newFlag)
        {
            contentTokenFlag output = flags;

            if (flags.ToInt32() == 0)
            {
                output = newFlag;
            }
            else
            {
                output |= newFlag;
            }

            return output;
        }
    }
}