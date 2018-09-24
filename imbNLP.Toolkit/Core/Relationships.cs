using imbSCI.Data.interfaces;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Core
{

    /// <summary>
    /// Describes relationships between instances in the <see cref="SpaceModel"/>
    /// </summary>
    /// <typeparam name="TNodeA">The type of the node a.</typeparam>
    /// <typeparam name="TNodeB">The type of the node b.</typeparam>
    public class Relationships<TNodeA, TNodeB> where TNodeA : IObjectWithName
    where TNodeB : IObjectWithName
    {
        /// <summary>
        /// The links
        /// </summary>
        public List<Relationship<TNodeA, TNodeB>> links = new List<Relationship<TNodeA, TNodeB>>();

        /// <summary>
        /// Adds the specified a.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <param name="weight">The weight.</param>
        /// <returns></returns>
        public Relationship<TNodeA, TNodeB> Add(TNodeA A, TNodeB B, double weight)
        {
            Relationship<TNodeA, TNodeB> output = new Relationship<TNodeA, TNodeB>();
            output.NodeA = A;
            output.NodeB = B;
            output.weight = weight;
            links.Add(output);
            return output;
        }

        /// <summary>
        /// Gets all linked.
        /// </summary>
        /// <param name="nodeA">The node a.</param>
        /// <returns></returns>
        public List<TNodeB> GetAllLinked(TNodeA nodeA)
        {
            List<TNodeB> output = new List<TNodeB>();

            foreach (Relationship<TNodeA, TNodeB> link in links)
            {
                if (link.NodeA.name == nodeA.name)
                {
                    output.Add(link.NodeB);
                }
            }

            return output;
        }

        /// <summary>
        /// Gets all nodes A linked to specified node B
        /// </summary>
        /// <param name="nodeB">The node b.</param>
        /// <returns></returns>
        public List<TNodeA> GetAllLinked(TNodeB nodeB)
        {
            List<TNodeA> output = new List<TNodeA>();

            foreach (Relationship<TNodeA, TNodeB> link in links)
            {
                if (link.NodeB.name == nodeB.name)
                {
                    output.Add(link.NodeA);
                }
            }

            return output;
        }

        public List<Relationship<TNodeA, TNodeB>> GetAllRelationships(TNodeA nodeA)
        {
            var relations = links.Where(x => x.NodeA.name == nodeA.name);
            return relations.ToList();
        }
    }

}