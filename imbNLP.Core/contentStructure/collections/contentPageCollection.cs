// --------------------------------------------------------------------------------------------------------------------
// <copyright file="contentPageCollection.cs" company="imbVeles" >
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
namespace imbNLP.Core.contentStructure.collections
{
    using imbACE.Network.extensions;
    using imbNLP.Core.contentStructure.interafaces;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// Kolekcija svih ucitanih sadrzaja - grupisani su u contentPageSet
    /// </summary>
    public class contentPageCollection : ObservableCollection<contentPageSet>, INotifyPropertyChanged
    {
        #region --- current ------- Trenutno selektovani contentPage

        private contentPageSet _currentSet;

        /// <summary>
        /// Trenutno selektovani contentPage
        /// </summary>
        public contentPageSet currentSet
        {
            get
            {
                if (_currentSet == null)
                {
                    if (this.Any())
                    {
                        _currentSet = this.First();
                    }
                }
                return _currentSet;
            }
            set
            {
                _currentSet = value;
                OnPropertyChanged("current");
            }
        }

        #endregion --- current ------- Trenutno selektovani contentPage

        #region --- current ------- Trenutno selektovani contentPage

        private IContentPage _current;

        /// <summary>
        /// Trenutno selektovani contentPage
        /// </summary>
        public IContentPage current
        {
            get
            {
                if (_current == null)
                {
                    if (this.Any())
                    {
                        _current = currentSet.First();
                    }
                }
                return _current;
            }
            set
            {
                _current = value;
                OnPropertyChanged("current");
            }
        }

        #endregion --- current ------- Trenutno selektovani contentPage

        public void moveCurrentSet(bool backward = false)
        {
            int index = IndexOf(currentSet);
            if (backward)
            {
                index--;
            }
            else
            {
                index++;
            }
            if (index < 0)
            {
                index = Count;
            }

            if (index > Count)
            {
                index = 0;
            }
            var lastSet = currentSet;
            currentSet = this[index];
            if (currentSet != lastSet)
            {
                current = currentSet.current;
            }
        }

        public void moveCurrent(bool backward = false)
        {
            //current = currentSet.IndexOf(current);

            int index = currentSet.IndexOf(current);
            if (backward)
            {
                index--;
            }
            else
            {
                index++;
            }
            if (index < 0)
            {
                moveCurrentSet(true);
            }
            else if (index > Count)
            {
                moveCurrentSet(false);
            }
            else
            {
                current = currentSet[index];
            }
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Event koji javlja da je promenjen neki property
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members

        /// <summary>
        /// Poziva se kada nema handlera za PropertyChangedEventHandler
        /// </summary>
        public virtual void OnPropertChangedNoHandler(string name)
        {
        }

        /// <summary>
        /// Kreira event koji obaveštava da je promenjen neki parametar
        /// </summary>
        /// <remarks>
        /// Neće biti kreiran event ako nije spremna aplikacija: imbSettingsManager.current.isReady
        /// </remarks>
        /// <param name="name"></param>
        public void OnPropertyChanged(string name)
        {
            //if (!imbSettingsManager.current.isReady)
            //{
            //    return;
            //}
            //else
            //{
            //}

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
            else
            {
                OnPropertChangedNoHandler(name);
            }
        }

        /// <summary>
        /// Selektuje i/ili instancijra
        /// </summary>
        /// <param name="setName"></param>
        /// <returns></returns>
        public contentPageSet selectSet(string setName)
        {
            contentPageSet output = null;
            if (this.Any(x => x.name == setName))
            {
                output = this.First(x => x.name == setName);
            }
            else
            {
                output = new contentPageSet();
                output.name = setName;
                Add(output);
            }
            return output;
        }

        /// <summary>
        /// Dodaje stranicu u odgovarajuci set - ili kreira novi set
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public contentPageSet Add(IContentPage page)
        {
            contentPageSet pset = selectSet(page.contentUrl.getDomainNameFromUrl(true));
            pset.Add(page);
            return pset;
        }
    }
}