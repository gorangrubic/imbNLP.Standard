
using imbSCI.Data;
using System;
using System.IO;

namespace imbNLP.Toolkit.Documents
{



    /// <summary>
    /// Single web page document
    /// </summary>
    [Serializable]
    public class WebSiteDocument : IAssignedID
    {

        /// <summary>
        /// Temporary ID associated with the web document
        /// </summary>
        /// <value>
        /// Usually derived from absolute URL address.
        /// </value>
        public String AssignedID { get; set; } = "";

        ///// <summary>
        ///// Gets or sets the URL.
        ///// </summary>
        ///// <value>
        ///// The URL.
        ///// </value>
        //[XmlIgnore]
        //public String URL { get; set; } = "";

        private String _hTMLSource = "";
        private String _textContent = "";
        private String _hTTPHeader = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSiteDocument"/> class.
        /// </summary>
        public WebSiteDocument()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSiteDocument"/> class.
        /// </summary>
        /// <param name="_path">The path.</param>
        /// <param name="lazyLoad">if set to <c>true</c> [lazy load].</param>
        /// <param name="_fullPath">The full path.</param>
        public WebSiteDocument(String _path, Boolean lazyLoad, String _fullPath)
        {
            path = _path;
            lazyLoadMode = lazyLoad;
            realPath = _fullPath;

            if (!lazyLoad)
            {
                if (!triedToLoad) Load(_fullPath);
            }
        }


        private Object fileLoadLock = new Object();

        /// <summary>
        /// Sets the specified headers.
        /// </summary>
        /// <param name="_Headers">The headers.</param>
        /// <param name="_Source">The source.</param>
        public void Set(String _Headers, String _Source)
        {
            _hTMLSource = _Source;
            _hTTPHeader = _Headers;
        }

        /// <summary>
        /// Loads the specified full path.
        /// </summary>
        /// <param name="_fullPath">The full path.</param>
        public void Load(String _fullPath = "")
        {

            triedToLoad = true;

            if (_fullPath.isNullOrEmpty()) _fullPath = realPath;
            if (realPath.isNullOrEmpty()) return;

            String sourceCode = File.ReadAllText(_fullPath);
            pathFull = _fullPath;

            Int32 htmlStart = sourceCode.IndexOf('<');

            if (htmlStart > 0)
            {
                _hTTPHeader = sourceCode.Substring(0, htmlStart);
                _hTMLSource = sourceCode.Substring(htmlStart);
            }
            realPath = "";
            lazyLoadMode = false;
        }

        private String realPath { get; set; }

        private Boolean lazyLoadMode { get; set; } = false;

        /// <summary>
        /// URL path, relative to domain name
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public String path { get; set; } = "";

        /// <summary>
        /// Real path of origin - set after loading the page
        /// </summary>
        /// <value>
        /// The path full.
        /// </value>
        public String pathFull { get; set; } = "";

        /// <summary>
        /// Original HTML source code
        /// </summary>
        /// <value>
        /// The HTML source.
        /// </value>
        public String HTMLSource
        {
            get
            {

                if (_hTMLSource == "" && !realPath.isNullOrEmpty())
                {
                    lock (fileLoadLock)
                    {
                        if (_hTMLSource == "" && !realPath.isNullOrEmpty())
                        {
                            if (!triedToLoad) Load();
                        }
                    }
                }
                return _hTMLSource;
            }
            set { _hTMLSource = value; }
        }

        protected Boolean triedToLoad = false;


        /// <summary>
        /// HTTP Header of the document
        /// </summary>
        /// <value>
        /// The HTTP header.
        /// </value>
        public String HTTPHeader
        {
            get
            {
                if (_hTTPHeader == "" && !realPath.isNullOrEmpty())
                {
                    lock (fileLoadLock)
                    {
                        if (_hTTPHeader == "" && !realPath.isNullOrEmpty())
                        {
                            if (!triedToLoad) Load();
                        }
                    }
                }
                return _hTTPHeader;
            }
            set { _hTTPHeader = value; }
        }


    }
}