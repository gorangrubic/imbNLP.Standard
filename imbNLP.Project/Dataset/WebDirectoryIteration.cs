using System;
using System.Collections.Generic;

namespace imbNLP.Project.Dataset
{
    /// <summary>
    /// Iteration in web directory harvesting operation
    /// </summary>
    public class WebDirectoryIteration
    {
        public WebDirectoryIteration()
        {
        }

        public WebDirectoryIteration(String _URL)
        {
            URL = _URL;
        }

        public WebDomainCategory DirectoryNode { get; set; }

        public String URL { get; set; } = "";

        public String DirectoryPath { get; set; } = "";

        public List<String> SubdirectoryList { get; set; } = new List<string>();

        public List<String> OtherInnerLinks { get; set; } = new List<string>();

        public List<String> WebsiteList { get; set; } = new List<string>();
    }
}