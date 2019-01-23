using imbNLP.Toolkit.Weighting.Global;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Weighting.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class FunctionSharedDataPool
    {

        public Boolean TryToGet(IGlobalElement globalFunction, ILogBuilder log)
        {
            if (globalFunction is IGlobalElementWithSharedData sharedFunction)
            {
                String fn = sharedFunction.GetType().Name;
                if (storage.ContainsKey(fn))
                {
                    sharedFunction.SetSharedDataStructure(storage[fn]);
                    log.log(":: Function [" + fn + "] retrieved data from the shared pool");
                    return true;
                }
            }
            return false;
        }


        public Boolean TryToSet(IGlobalElement globalFunction, ILogBuilder log)
        {
            if (globalFunction is IGlobalElementWithSharedData sharedFunction)
            {

                String fn = sharedFunction.GetType().Name;
                if (!storage.ContainsKey(fn))
                {
                    storage.Add(fn, sharedFunction.GetSharedDataStructure());
                    log.log(":: Function [" + fn + "] stored data into shared pool");
                    return true;
                }

            }
            return false;
        }

        public Dictionary<String, ISharedDataPool> storage { get; protected set; } = new Dictionary<string, ISharedDataPool>();


    }
}