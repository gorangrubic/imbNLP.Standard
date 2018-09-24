using imbSCI.Core.files.folders;
using imbSCI.Data.interfaces;
using System;

namespace imbNLP.Toolkit.Core
{


    /// <summary>
    /// Generic class for single relationship definition
    /// </summary>
    /// <typeparam name="TNodeA">The type of the node a.</typeparam>
    /// <typeparam name="TNodeB">The type of the node b.</typeparam>
    public class Relationship<TNodeA, TNodeB>
    where TNodeA : IObjectWithName
    where TNodeB : IObjectWithName
    {
        public TNodeA NodeA { get; set; }
        public TNodeB NodeB { get; set; }
        public double weight { get; set; } = 1;
    }

}