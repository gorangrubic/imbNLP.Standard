using System;
using System.Linq;
using System.Collections.Generic;
using imbSCI.Core.reporting;
using imbSCI.Data.interfaces;
using System.ComponentModel;

namespace imbNLP.Toolkit.Core
{
public abstract class NLPBindable : INotifyPropertyChanged
    {
        public void OnPropertyChange(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}