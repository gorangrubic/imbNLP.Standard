using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.ExperimentModel;
using imbNLP.Toolkit.Planes.Core;
using imbSCI.Core.files;
using imbSCI.Core.math;
using System;

namespace imbNLP.Toolkit.Planes
{

    /// <summary>
    /// Common base for method design classes
    /// </summary>
    public abstract class MethodDesignBase
    {
        public String setupSignature { get; protected set; } = "";

        public void SetSetupSignature(IPlaneSettings _setup)
        {
            Type t = null;
            if (_setup != null)
            {
                t = _setup.GetType();
            }
            String xml = objectSerialization.ObjectToXML((Object)_setup);

            setupSignature = md5.GetMd5Hash(xml);
        }

        public CacheServiceProvider CacheProvider { get; set; } = new CacheServiceProvider();


        public string name { get; set; } = "";
        public ToolkitExperimentNotes notes { get; set; }

        public void DeploySettingsBase(ToolkitExperimentNotes _notes)
        {
            notes = _notes;
            name = this.GetType().Name.Replace("MethodDesign", "");

            notes.AppendLine(name + " Settings");
            notes.nextTabLevel();
        }

        public void CloseDeploySettingsBase()
        {
            notes.prevTabLevel();
        }
    }

}