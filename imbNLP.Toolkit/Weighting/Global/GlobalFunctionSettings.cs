using imbNLP.Toolkit.Typology;
using imbNLP.Toolkit.Weighting.Metrics;
using imbSCI.Core.reporting;
using System;
using static imbNLP.Toolkit.Weighting.Global.IDFElement;

namespace imbNLP.Toolkit.Weighting.Global
{

    /// <summary>
    /// Settings on a global weighting function
    /// </summary>
    public class GlobalFunctionSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalFunctionSettings"/> class.
        /// </summary>
        public GlobalFunctionSettings()
        {

        }

        /// <summary>
        /// Gets or sets the name of the function.
        /// </summary>
        /// <value>
        /// The name of the function.
        /// </value>
        public String functionName { get; set; } = "IDFElement";

        /// <summary>
        /// Gets or sets the TDP factor.
        /// </summary>
        /// <value>
        /// The TDP factor.
        /// </value>
        public TDPFactor tdpFactor { get; set; } = TDPFactor.none;

        /// <summary>
        /// Gets or sets the idf computation.
        /// </summary>
        /// <value>
        /// The idf computation.
        /// </value>
        public IDFComputation idfComputation { get; set; } = IDFComputation.logPlus;

        ///// <summary>
        ///// Gets or sets the type of the function.
        ///// </summary>
        ///// <value>
        ///// The type of the function.
        ///// </value>
        //public GlobalFunctionType functionType { get; set; } = GlobalFunctionType.IDF;

        ///// <summary>
        /// Gets or sets the l.
        /// </summary>
        /// <value>
        /// The l.
        /// </value>
        public Double l { get; set; } = 7;

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>
        /// The weight.
        /// </value>
        public Double weight { get; set; } = 1.0;

        public String GetSignature()
        {
            String output = functionName.Replace("Collection", "").Replace("Element", "");
            switch (output)
            {
                case "TDP":
                    output += tdpFactor.ToString();
                    break;
                case "IDF":
                    output += idfComputation.ToString();
                    break;
                case "IGM":
                    output += "l" + l.ToString("F1");
                    break;
            }
            return output;
        }

        /// <summary>
        /// Creates instance of a global function
        /// </summary>
        /// <returns></returns>
        public IGlobalElement GetFunction(ILogBuilder logger)
        {
            IGlobalElement output = TypeProviders.GlobalTermFunction.GetInstance(functionName);
            if (output == null)
            {
                throw new Exception("Global element function [" + functionName + "] was not found by the TypeProviders.GlobalTermFunction!");
            }

            if (output is CollectionTDPElement tdpElement)
            {
                tdpElement.factor = tdpFactor;
                logger.log("Created: TDP function based on [" + tdpFactor.ToString() + "] factor");

            }
            else if (output is IDFElement idfElement)
            {
                idfElement.Computation = idfComputation;
                logger.log("Created: IDF function based on [" + idfComputation.ToString() + "] computation");
            }
            else if (output is IGMElement igmElement)
            {
                igmElement.l = l;
                logger.log("Created: IGM function with landa set to [" + l.ToString() + "]");
            }
            else if (output is ICSdFElement icsdElement)
            {
                logger.log("Created: ICSd function");
            }


            /*
            GlobalElementBase output = null;
            switch (functionType)
            {
                case GlobalFunctionType.ICF:
                    output = new ICFElement();
                    break;
                case GlobalFunctionType.ICSdF:
                    output = new ICSdFElement();
                    break;
                default:
                case GlobalFunctionType.IDF:
                    IDFElement iDFElement = new IDFElement();
                    iDFElement.Computation = idfComputation;
                    output = iDFElement;
                    break;
                case GlobalFunctionType.IGM:
                    IGMElement iGMElement = new IGMElement();
                    iGMElement.l = l;
                    output = iGMElement;
                    break;
                case GlobalFunctionType.TDP:
                    CollectionTDPElement tDP = new CollectionTDPElement();
                    tDP.factor = tdpFactor;
                    output = tDP;
                    break;
            }
            */

            return output;
        }

    }

}