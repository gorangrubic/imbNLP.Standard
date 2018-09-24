using imbSCI.Data.interfaces;
using System;

namespace imbNLP.Toolkit.Space
{
    public interface IObjectWithNameWeightAndType : IObjectWithName
    {
        String name { get; set; }
        Int32 type { get; set; }
        Double weight { get; set; }
    }
}