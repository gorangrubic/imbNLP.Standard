using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Planes.Core
{

    /// <summary>
    /// Storage for created contextes
    /// </summary>
    public class PlaneContextProvider
    {
        public void Dispose()
        {
            foreach (var pair in items.ToList())
            {
                pair.Value.provider.Dispose();
            }

            items.Clear();
        }


        /// <summary>
        /// Gets the context of specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetContext<T>() where T : IPlaneContext
        {
            String key = typeof(T).Name;
            if (items.ContainsKey(key))
            {
                return (T)items[key];
            }
            else
            {
                return default(T);
            }
        }

        public PlaneContextProvider() { }

        public PlaneContextProvider(PlaneContextProvider provider)
        {
            Receive(provider);
        }

        /// <summary>
        /// Stores the and receive.
        /// </summary>
        /// <param name="context">The context.</param>
        public void StoreAndReceive(IPlaneContext context)
        {
            Receive(context.provider);
            StoreContext(context);
        }

        /// <summary>
        /// Receives contexts stored at the <c>provider</c>
        /// </summary>
        /// <param name="provider">The provider.</param>
        public void Receive(PlaneContextProvider provider)
        {
            foreach (var pair in provider.items)
            {
                StoreContext(pair.Value);
            }
        }

        /// <summary>
        /// Stores or updates the context
        /// </summary>
        /// <param name="context">The context.</param>
        public void StoreContext(IPlaneContext context)
        {
            String n = context.GetType().Name;
            if (items.ContainsKey(n))
            {
                items[n] = context;
            }
            else
            {
                items.Add(n, context);
            }
        }

        public Dictionary<String, IPlaneContext> items { get; protected set; } = new Dictionary<string, IPlaneContext>();

    }

}