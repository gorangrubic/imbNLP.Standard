
using imbNLP.Toolkit.Classifiers.Core;
using imbNLP.Toolkit.ExperimentModel;
using imbSCI.Core.reporting;

namespace imbNLP.Toolkit.Planes.Core
{

    /// <summary>
    /// Customized design of methods, functions and algorithms - to meet requirements of an experimental method and setup
    /// </summary>
    public interface IPlaneMethodDesign
    {
       

        /// <summary>
        /// Prepares everything for operation
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="_notes">The notes.</param>
        /// <param name="logger">The logger.</param>
        void DeploySettings(IPlaneSettings settings, ToolkitExperimentNotes _notes, ILogBuilder logger);


        /// <summary>
        /// Executes the plane method, invoking contained functions according to the settings
        /// </summary>
        /// <param name="inputContext">The input context - related to this plane.</param>
        /// <param name="generalContext">General execution context, attached to the <see cref="PlanesMethodDesign" /></param>
        /// <param name="logger">The logger.</param>
        /// <returns>Retur</returns>
        IPlaneContext ExecutePlaneMethod(IPlaneContext inputContext, ExperimentModelExecutionContext generalContext, ILogBuilder logger);


    }

}