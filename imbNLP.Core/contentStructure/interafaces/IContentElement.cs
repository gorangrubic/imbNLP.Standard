// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContentElement.cs" company="imbVeles" >
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
namespace imbNLP.Core.contentStructure.interafaces
{
    #region imbVELES USING

    using imbNLP.Core.contentStructure.collections;
    using imbNLP.Core.contentStructure.core;
    using imbSCI.Core.attributes;
    using imbSCI.Data.interfaces;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml.Serialization;

    #endregion imbVELES USING

    [imb(imbAttributeName.collectionDisablePrimaryKey)]
    public interface IContentElement : INotifyPropertyChanged, IObjectWithParent, IObjectWithName,
                                       IObjectWithPath, IObjectWithID, IObjectWithPathAndChildSelector
    {
        // reportXmlDocument makeXml(reportXmlDocument xmlReport = null);

        IContentPage page { get; }

        string UID { get; set; }

        contentMatch match { get; set; }

        List<Enum> GetFlags();

        /// <summary>
        /// Izvorni content
        /// </summary>
        // [XmlIgnore]
        [Category("Content")]
        [DisplayName("sourceContent")]
        [Description("Izvorni content")]
        string sourceContent { get; set; }

        /// <summary>
        /// Originalni sadržaj koji je pronađen u tokenu
        /// </summary>
        // [XmlIgnore]
        [Category("Content")]
        [DisplayName("content")]
        [Description("Originalni sadržaj koji je pronađen u tokenu")]
        string content { get; set; }

        /// <summary>
        /// Koji je spliter odredio odvajanje tokena
        /// </summary>
        // [XmlIgnore]
        [Category("Content")]
        [DisplayName("spliter")]
        [Description("Koji je spliter odredio odvajanje  tokena/recenice")]
        string spliter { get; set; }

        /// <summary>
        /// Skup pod-itema
        /// </summary>
        // [XmlIgnore]
        [Category("Misc")]
        [DisplayName("items")]
        [Description("Skup pod-itema")]
        IContentCollectionBase items { get; }

        /// <summary>
        /// Da li je reč o poslednjem elementu?
        /// </summary>
        [XmlIgnore]
        [Category("Order")]
        [DisplayName("isLast")]
        [Description("Da li je reč o poslednjem elementu?")]
        bool isLast { get; }

        /// <summary>
        /// Da li je reč o prvom elementu?
        /// </summary>
        [XmlIgnore]
        [Category("Order")]
        [DisplayName("isFirst")]
        [Description("Da li je reč o prvom elementu?")]
        bool isFirst { get; }

        /// <summary>
        /// Referenca prema sledećem slogu
        /// </summary>
        [XmlIgnore]
        IContentElement next { get; set; }

        /// <summary>
        /// Referenca prema prethodnom slogu
        /// </summary>
        [XmlIgnore]
        IContentElement prev { get; set; }

        /// <summary>
        /// referenca prema parent objektu
        /// </summary>
        [XmlIgnore]
        IContentElement parent { get; set; }

        /// <summary>
        /// Podešava PREV i NEXT za nlpBase klase
        /// </summary>
        /// <remarks>
        /// Radiće u slučaju da je item član liste kao i da nije. Ako nije dodeliće mu prev kao last
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        IContentElement setItem(IContentElement newItem);

        /// <summary>
        /// izvrsava se od
        /// </summary>
        /// <param name="resources"></param>
        void primaryFlaging(params object[] resources);

        void secondaryFlaging(params object[] resources);

        void generalSemanticsFlaging(params object[] resources);

        void specialSematicsFlaging(params object[] resources);
    }
}