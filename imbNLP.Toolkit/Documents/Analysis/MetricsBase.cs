using System;
using System.Collections.Generic;
using System.Reflection;

namespace imbNLP.Toolkit.Documents.Analysis
{

    /// <summary>
    /// Base class for metrics containers
    /// </summary>
    public abstract class MetricsBase
    {

        protected MetricsBase()
        {
            Init();
        }

        /// <summary>
        /// Pluses the specified b.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="b">The b.</param>
        public void Plus<T>(T b) where T : MetricsBase
        {
            Type tb = typeof(T);

            foreach (var pi in Integers)
            {
                var pib = tb.GetProperty(pi.Name);
                Int32 rb = (Int32)pib.GetValue(b, null);
                Int32 ra = (Int32)pi.GetValue(this, null);
                pi.SetValue(this, rb + ra, null);
            }

            foreach (var pi in Doubles)
            {
                var pib = tb.GetProperty(pi.Name);
                Double rb = (Double)pib.GetValue(b, null);
                Double ra = (Double)pi.GetValue(this, null);
                pi.SetValue(this, rb + ra, null);
            }
        }

        /// <summary>
        /// Minuses the specified b.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="b">The b.</param>
        public void Minus<T>(T b) where T : MetricsBase
        {
            Type tb = typeof(T);

            foreach (var pi in Integers)
            {
                var pib = tb.GetProperty(pi.Name);
                Int32 rb = (Int32)pib.GetValue(b, null);
                Int32 ra = (Int32)pi.GetValue(this, null);
                pi.SetValue(this, rb - ra, null);
            }

            foreach (var pi in Doubles)
            {
                var pib = tb.GetProperty(pi.Name);
                Double rb = (Double)pib.GetValue(b, null);
                Double ra = (Double)pi.GetValue(this, null);
                pi.SetValue(this, rb - ra, null);
            }
        }

        /// <summary>
        /// Divides the specified b.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="b">The b.</param>
        public void Divide<T>(T b) where T : MetricsBase
        {
            Type tb = typeof(T);

            foreach (var pi in Integers)
            {
                var pib = tb.GetProperty(pi.Name);
                Int32 rb = (Int32)pib.GetValue(b, null);
                Int32 ra = (Int32)pi.GetValue(this, null);
                Int32 r = 0;
                if (ra != 0 && rb != 0) r = ra / rb;
                pi.SetValue(this, r, null);
            }

            foreach (var pi in Doubles)
            {
                var pib = tb.GetProperty(pi.Name);
                Double rb = (Double)pib.GetValue(b, null);
                Double ra = (Double)pi.GetValue(this, null);
                Double r = 0;
                if (ra != 0 && rb != 0) r = ra / rb;
                pi.SetValue(this, r, null);
                //pi.SetValue(this, rb / ra, null);
            }
        }

        /// <summary>
        /// Powers the specified b.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="b">The b.</param>
        public void Power<T>(T b) where T : MetricsBase
        {
            Type tb = typeof(T);

            foreach (var pi in Integers)
            {
                var pib = tb.GetProperty(pi.Name);
                Int32 rb = (Int32)pib.GetValue(b, null);
                Int32 ra = (Int32)pi.GetValue(this, null);
                pi.SetValue(this, rb * ra, null);
            }

            foreach (var pi in Doubles)
            {
                var pib = tb.GetProperty(pi.Name);
                Double rb = (Double)pib.GetValue(b, null);
                Double ra = (Double)pi.GetValue(this, null);
                pi.SetValue(this, rb * ra, null);
            }
        }

        /// <summary>
        /// Divides the specified b.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="b">The b.</param>
        public void Divide(Double b)
        {
            foreach (var pi in Integers)
            {
                Int32 ra = (Int32)pi.GetValue(this, null);
                Int32 r = 0;
                if (ra != 0 && b != 0) r = Convert.ToInt32(Convert.ToDouble(ra) / b);
                pi.SetValue(this, r, null);
            }

            foreach (var pi in Doubles)
            {
                Double ra = (Double)pi.GetValue(this, null);
                Double r = 0;
                if (ra != 0 && b != 0) r = Convert.ToDouble(ra) / b;
                pi.SetValue(this, r, null);
            }
        }

        /// <summary>
        /// Powers the specified b.
        /// </summary>
        /// <param name="b">The b.</param>
        public void Power(Double b)
        {
            foreach (var pi in Integers)
            {
                Int32 ra = (Int32)pi.GetValue(this, null);
                pi.SetValue(this, ra * b, null);
            }

            foreach (var pi in Doubles)
            {
                Double ra = (Double)pi.GetValue(this, null);
                pi.SetValue(this, ra * b, null);
            }
        }



        /// <summary>
        /// Initializes this instance, registers all Integers and Doubles
        /// </summary>
        protected void Init()
        {
            var t = GetType();
            var props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var p in props)
            {
                if (p.GetIndexParameters().Length == 0)
                {
                    if (p.PropertyType == typeof(Int32))
                    {
                        Integers.Add(p);
                    }
                    else if (p.PropertyType == typeof(Double))
                    {
                        Doubles.Add(p);
                    }
                }
            }

        }

        protected List<PropertyInfo> Integers { get; set; } = new List<PropertyInfo>();
        protected List<PropertyInfo> Doubles { get; set; } = new List<PropertyInfo>();

    }

}