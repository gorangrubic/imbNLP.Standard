using imbNLP.Toolkit.Entity.DocumentFunctions;
using imbNLP.Toolkit.Functions;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Stemmers;
using imbNLP.Toolkit.Weighting.Global;
using imbNLP.Toolkit.Weighting.Local;
using imbSCI.Core.reporting;
using System;

namespace imbNLP.Toolkit.Typology
{

    /// <summary>
    /// Type providers used for class-name-string to class instance part of settings deployment accross the imbNLP.Toolkit namespace
    /// </summary>
    public static class TypeProviders
    {

        public static void Prepare(ILogBuilder logger)
        {
            similarityFunctions.Prepare(logger);
            LocalTermFunction.Prepare(logger);
            GlobalTermFunction.Prepare(logger);
            InputDocumentFunctions.Prepare(logger);
            tokenizerTypes.Prepare(logger);
            stemmerTypes.Prepare(logger);

        }


        private static Object _similarityFunctions_lock = new Object();
        private static UniversalTypeProvider<IVectorSimilarityFunction> _similarityFunctions;
        /// <summary>
        /// Types related to similarity computation between vectors
        /// </summary>
        public static UniversalTypeProvider<IVectorSimilarityFunction> similarityFunctions
        {
            get
            {
                if (_similarityFunctions == null)
                {
                    lock (_similarityFunctions_lock)
                    {

                        if (_similarityFunctions == null)
                        {
                            _similarityFunctions = new UniversalTypeProvider<IVectorSimilarityFunction>("imbNLP.Toolkit.Functions");
                            // add here if any additional initialization code is required
                        }
                    }
                }
                return _similarityFunctions;
            }
        }


        private static Object _LocalTermFunction_lock = new Object();
        private static UniversalTypeProvider<ILocalElement> _LocalTermFunction;
        /// <summary>
        /// static and autoinitiated object
        /// </summary>
        public static UniversalTypeProvider<ILocalElement> LocalTermFunction
        {
            get
            {
                if (_LocalTermFunction == null)
                {
                    lock (_LocalTermFunction_lock)
                    {

                        if (_LocalTermFunction == null)
                        {
                            _LocalTermFunction = new UniversalTypeProvider<ILocalElement>("imbNLP.Toolkit.Weighting.Local");
                            // add here if any additional initialization code is required
                        }
                    }
                }
                return _LocalTermFunction;
            }
        }


        private static Object _InputDocumentFunctions_lock = new Object();
        private static UniversalTypeProvider<IDocumentFunction> _InputDocumentFunctions;
        /// <summary>
        /// static and autoinitiated object
        /// </summary>
        public static UniversalTypeProvider<IDocumentFunction> InputDocumentFunctions
        {
            get
            {
                if (_InputDocumentFunctions == null)
                {
                    lock (_InputDocumentFunctions_lock)
                    {

                        if (_InputDocumentFunctions == null)
                        {
                            _InputDocumentFunctions = new UniversalTypeProvider<IDocumentFunction>("imbNLP.Toolkit.Entity.DocumentFunctions");
                            // add here if any additional initialization code is required
                        }
                    }
                }
                return _InputDocumentFunctions;
            }
        }



        private static Object _GlobalTermFunction_lock = new Object();
        private static UniversalTypeProvider<IGlobalElement> _GlobalTermFunction;
        /// <summary>
        /// static and autoinitiated object
        /// </summary>
        public static UniversalTypeProvider<IGlobalElement> GlobalTermFunction
        {
            get
            {
                if (_GlobalTermFunction == null)
                {
                    lock (_GlobalTermFunction_lock)
                    {

                        if (_GlobalTermFunction == null)
                        {
                            _GlobalTermFunction = new UniversalTypeProvider<IGlobalElement>("imbNLP.Toolkit.Weighting.Global");
                            // add here if any additional initialization code is required
                        }
                    }
                }
                return _GlobalTermFunction;
            }
        }


        private static Object _tokenizerTypes_lock = new Object();
        private static UniversalTypeProvider<ITokenizer> _tokenizerTypes;
        /// <summary>
        /// static and autoinitiated object
        /// </summary>
        public static UniversalTypeProvider<ITokenizer> tokenizerTypes
        {
            get
            {
                if (_tokenizerTypes == null)
                {
                    lock (_tokenizerTypes_lock)
                    {

                        if (_tokenizerTypes == null)
                        {
                            _tokenizerTypes = new UniversalTypeProvider<ITokenizer>("imbNLP.Toolkit.Processing");
                            // add here if any additional initialization code is required
                        }
                    }
                }
                return _tokenizerTypes;
            }
        }



        private static Object _stemmerTypes_lock = new Object();
        private static UniversalTypeProvider<IStemmer> _stemmerTypes;
        /// <summary>
        /// Provider for stemmer instances
        /// </summary>
        public static UniversalTypeProvider<IStemmer> stemmerTypes
        {
            get
            {
                if (_stemmerTypes == null)
                {
                    lock (_stemmerTypes_lock)
                    {

                        if (_stemmerTypes == null)
                        {
                            _stemmerTypes = new UniversalTypeProvider<IStemmer>("imbNLP.Toolkit.Stemmers");
                            // add here if any additional initialization code is required
                        }
                    }
                }
                return _stemmerTypes;
            }
        }


    }

}