using System;

namespace imbNLP.Toolkit.Weighting.Metrics
{
    /// <summary>
    /// Term-Document-Class class-local statistic for a term
    /// </summary>
    public class TermDiscriminatingPower
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TermDiscriminatingPower"/> class - sets the term
        /// </summary>
        /// <param name="_term">The term.</param>
        public TermDiscriminatingPower(String _term)
        {
            term = _term;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TermDiscriminatingPower"/> class - for XML Serialization
        /// </summary>
        public TermDiscriminatingPower()
        {
        }

        public String term { get; set; } = "";

        /// <summary>
        /// Computes the factor
        /// </summary>
        /// <param name="factor">What computation schema to be used</param>
        /// <param name="N">Total number of documents in the collection</param>
        /// <returns></returns>
        public Double Compute(TDPFactor factor, double N)
        {
            Double output = 0;
            TermDiscriminatingPower TDP = this;

            Double up = 0;
            Double down = 0;

            switch (factor)
            {
                case TDPFactor.rf:
                    output = Math.Log(2 + (TDP.a / Math.Max(1, TDP.c)));
                    break;

                case TDPFactor.chi:
                    up = (TDP.a * TDP.d) - (TDP.b * TDP.c);
                    up = up * up;
                    down = (TDP.a + TDP.c) * (TDP.b + TDP.d) * (TDP.a + TDP.b) * (TDP.c + TDP.d);
                    output = N * (up / down);
                    break;

                case TDPFactor.gr:
                    up = Compute(TDPFactor.ig, N);
                    down = -(TDP.a + TDP.b) / N;
                    down = down * Math.Log(((TDP.a + TDP.b) / N) - ((TDP.c + TDP.d) / N));
                    down = down * Math.Log((TDP.c + TDP.d) / N);
                    output = up / down;
                    break;

                case TDPFactor.idf:
                    output = Math.Log(N / (TDP.a + TDP.b));
                    break;

                case TDPFactor.idf_prob:
                    output = Math.Log((TDP.b + TDP.d) / (TDP.a + TDP.c));
                    break;

                case TDPFactor.ig:

                    output += (TDP.a / N) * Math.Log((TDP.a * N) / ((TDP.a + TDP.c) * (TDP.a + TDP.b)));
                    output += (TDP.b / N) * Math.Log((TDP.b * N) / ((TDP.b + TDP.d) * (TDP.a + TDP.b)));
                    output += (TDP.c / N) * Math.Log((TDP.c * N) / ((TDP.a + TDP.c) * (TDP.c + TDP.d)));
                    output += (TDP.d / N) * Math.Log((TDP.d * N) / ((TDP.b + TDP.d) * (TDP.c + TDP.d)));

                    break;

                case TDPFactor.or:
                    up = (TDP.a * TDP.d);
                    down = (TDP.b * TDP.c);
                    if (up == 0)
                    {
                        output = 0;
                    }
                    else if (down == 0)
                    {
                        output = 0;
                    }
                    else
                    {
                        output = up / down;
                    }

                    break;
            }

            return output;
        }

        /// <summary>
        /// number of documents in the positive catogory which contain this term
        /// </summary>
        /// <value>
        /// a.
        /// </value>
        public double a { get; set; }

        /// <summary>
        /// number of documents in the positive catogory which do not contain this term
        /// </summary>
        /// <value>
        /// The b.
        /// </value>
        public double b { get; set; }

        /// <summary>
        /// number of documents in a negative catogory which contain this term
        /// </summary>
        /// <value>
        /// The c.
        /// </value>
        public double c { get; set; }

        /// <summary>
        /// number of documents in a negative catogory which do not contain this term
        /// </summary>
        /// <value>
        /// The d.
        /// </value>
        public double d { get; set; }
    }
}