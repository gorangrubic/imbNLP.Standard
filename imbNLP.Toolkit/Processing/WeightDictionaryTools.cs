using System;
using System.Linq;
using System.Collections.Generic;
using imbSCI.Core.enums;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Processing
{

    /// <summary>
    /// 
    /// </summary>
    public static class WeightDictionaryTools
    {

        /// <summary>
        /// Compresses the numeric vector.
        /// </summary>
        /// <param name="nVector">The n vector.</param>
        /// <param name="operation">Allowed operations: <see cref="operation.multiplication"/>, <see cref="operation.max"/>, <see cref="operation.min"/>, <see cref="operation.plus"/></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Operation [" + operation.ToString() + "] not supported by this method. - operation</exception>
        public static Double CompressNumericVector(this WeightDictionaryEntry nVector, operation operation)
        {

            switch (operation)
            {

                case operation.multiplication:
                    Double o = 1;
                    for (int i = 0; i < nVector.dimensions.Length; i++)
                    {
                        o = o * nVector.dimensions[i];
                    }
                    return o;
                    break;
                case operation.avg:
                    return nVector.dimensions.Average();
                    break;
                case operation.max:
                    return nVector.dimensions.Max();
                    break;
                case operation.min:
                    return nVector.dimensions.Min();
                    break;
                case operation.plus:
                    return nVector.dimensions.Sum();
                    break;
                case operation.abs:
                case operation.assign:
                case operation.division:
                case operation.divisionNatural:
                case operation.exp:
                case operation.ceil:
                case operation.compare:
                case operation.what10th:
                case operation.what3rd:
                case operation.what4th:
                case operation.what5th:
                case operation.whatHalf:
                case operation.diagonal:
                default:
                    throw new ArgumentOutOfRangeException("Operation [" + operation.ToString() + "] not supported by this method.", nameof(operation));
                    break;
            }
        }

    }

}