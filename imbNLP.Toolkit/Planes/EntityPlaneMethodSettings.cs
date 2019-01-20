using imbNLP.Toolkit.Entity;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Planes
{

    /// <summary>
    /// Textual rendering and blending of web documents
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Planes.PlaneSettingsBase" />
    public class EntityPlaneMethodSettings : PlaneSettingsBase
    {

        /// <summary>
        /// Document rendering instructions
        /// </summary>
        /// <value>
        /// The instructions.
        /// </value>
        public List<DocumentRenderInstruction> instructions { get; set; } = new List<DocumentRenderInstruction>();



        /// <summary>
        /// Gets or sets the blender options.
        /// </summary>
        /// <value>
        /// The blender options.
        /// </value>
  //      public DocumentBlenderFunctionOptions blenderOptions { get; set; } = DocumentBlenderFunctionOptions.binaryAggregation | DocumentBlenderFunctionOptions.siteLevel | DocumentBlenderFunctionOptions.uniqueContentUnitsOnly;

        public string filterFunctionName { get; set; } = "";

        public Int32 filterLimit { get; set; } = 0;


        /// <summary>
        /// Initializes a new instance of the <see cref="EntityPlaneMethodSettings"/> class.
        /// </summary>
        public EntityPlaneMethodSettings()
        {

        }

        /// <summary>
        /// Describes the specified logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public override void Describe(ILogBuilder logger)
        {

        }
    }

}