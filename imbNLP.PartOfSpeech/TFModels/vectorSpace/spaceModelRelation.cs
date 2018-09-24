using imbSCI.Core.extensions.enumworks;
using System;
using System.Xml.Serialization;

namespace imbNLP.PartOfSpeech.TFModels.vectorSpace
{
    /// <summary>
    /// Describes single entry on relationship between two entities in a <see cref="spaceModel"/>. Relation has direction from A to B. Described by: Value, RelationshipType and optional data
    /// </summary>
    [XmlRoot(ElementName = "r")]
    public class spaceModelRelation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="spaceModelRelation"/> class.
        /// </summary>
        public spaceModelRelation()
        {
        }

        /// <summary>
        /// Typed access to the Entity A type
        /// </summary>
        /// <value>
        /// The type of the entity a.
        /// </value>
        [XmlIgnore]
        public spaceModelRelationEntityType EntityAType
        {
            get
            {
                return (spaceModelRelationEntityType)EntityATypeVal;
            }
            set
            {
                EntityATypeVal = value.ToInt32();
            }
        }

        /// <summary>
        /// Typed access to the Entity B type
        /// </summary>
        /// <value>
        /// The type of the entity B.
        /// </value>
        [XmlIgnore]
        public spaceModelRelationEntityType EntityBType
        {
            get
            {
                return (spaceModelRelationEntityType)EntityBTypeVal;
            }
            set
            {
                EntityBTypeVal = value.ToInt32();
            }
        }

        /// <summary>
        /// Gets or sets the type of the entity a.
        /// </summary>
        /// <value>
        /// The type of the entity a.
        /// </value>
        [XmlAttribute(AttributeName = "At")]
        public Int32 EntityATypeVal { get; set; } = 0;

        /// <summary>
        /// Unique (local) identification of the entity A
        /// </summary>
        /// <value>
        /// The unique ID of entity A, in its local vector space (network, graph...)
        /// </value>
        [XmlAttribute(AttributeName = "A")]
        public String EntityA { get; set; } = "";

        /// <summary>
        /// Value stored for the relationship
        /// </summary>
        [XmlAttribute(AttributeName = "V")]
        public Double Value { get; set; } = 0;

        /// <summary>
        /// Integer flag that describes the nature of the relationship
        /// </summary>
        [XmlAttribute(AttributeName = "Rt")]
        public Int32 RelationshipType { get; set; } = 0;

        /// <summary>
        /// Additional data stored in the relationship.
        /// </summary>
        [XmlAttribute(AttributeName = "T")]
        public String Data { get; set; } = "";

        /// <summary>
        /// Unique (local) identification of the entity B
        /// </summary>
        /// <value>
        /// The unique ID of entity A, in its local vector space (network, graph...)
        /// </value>
        [XmlAttribute(AttributeName = "B")]
        public String EntityB { get; set; } = "";

        /// <summary>
        /// Gets or sets the type of the entity b.
        /// </summary>
        /// <value>
        /// The type of the entity b.
        /// </value>
        [XmlAttribute(AttributeName = "Bt")]
        public Int32 EntityBTypeVal { get; set; } = 0;
    }
}