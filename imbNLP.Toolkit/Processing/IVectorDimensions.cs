using System;

namespace imbNLP.Toolkit.Processing
{
    public interface IVectorDimensions : IEquatable<IVectorDimensions>
    {
        String name { get; set; }

        Double[] dimensions { get; set; }


    }
}