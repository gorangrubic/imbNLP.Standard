using imbSCI.Core.files;
using imbSCI.Core.reporting;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.IO;

namespace imbNLP.Toolkit.Feature.Settings
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{imbNLP.Toolkit.Feature.Settings.dimensionSpecification}" />
    public class dimensionSpecificationSet : List<dimensionSpecification>
    {

        public dimensionSpecificationSet()
        {

        }

        public void Save(String filepath, ILogBuilder logger)
        {

            objectSerialization.saveObjectToXML(this, filepath);
        }

        public static dimensionSpecificationSet Load(String filepath, ILogBuilder logger)
        { 
               if (filepath.isNullOrEmpty())
            {
                throw new ArgumentNullException("Filepath for dimensionSpecificationSet.Load is null or empty", nameof(filepath));
    }

            if (!File.Exists(filepath))
            {
                throw new ArgumentNullException("File [" + filepath + "] not found, fordimensionSpecificationSet.Load is null or empty", nameof(filepath));
            }

        
            var output = objectSerialization.loadObjectFromXML<dimensionSpecificationSet>(filepath, logger);
            return output;

        }

        public Int32 IndexOf(String _name)
        {
            foreach (dimensionSpecification dim in this)
            {
                if (dim.name == _name)
                {
                    return IndexOf(dim);
                }
            }
            return -1;
        }

        public dimensionSpecification Add(String _name, String _description, FeatureVectorDimensionType _type, String _functionName = "")
        {
            if (_functionName == "") _functionName = _name;
            dimensionSpecification output = new dimensionSpecification();
            output.Deploy(_name, _description, _type, _functionName);

            Add(output);

            return output;
        }


    }
}