// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HITSScore.cs" company="imbVeles" >
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
    public class HITSScore
    {
        public static implicit operator double(HITSScore score)
        {
            return score.a + score.h;
        }

        private double ___a;
        private double ___h;

        /// <summary>
        ///
        /// </summary>
        public double a_delta { get; set; }

        /// <summary>
        ///
        /// </summary>
        public double h_delta { get; set; }

        public void resetChangeMeasure()
        {
            ___a = 0;
            ___h = 0;
            a_delta = 0;
            h_delta = 0;
        }

        public HITSScore(double __a, double __h)
        {
            a = __a;
            h = __h;
        }

        private double _a = 1;

        /// <summary>
        ///
        /// </summary>
        public double a
        {
            get { return _a; }
            set
            {
                ___a = _a;

                a_delta = value - ___a;

                _a = value;
            }
        }

        private double _h = 1;

        /// <summary>
        ///
        /// </summary>
        public double h
        {
            get
            {
                return _h;
            }
            set
            {
                ___h = _h;
                h_delta = value - ___h;

                _h = value;
            }
        }
    }
}