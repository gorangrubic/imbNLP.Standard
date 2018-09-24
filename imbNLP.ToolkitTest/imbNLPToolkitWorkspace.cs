// --------------------------------------------------------------------------------------------------------------------
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/. 
// </copyright>
// <summary>
// Project: 
// Author: 
// ------------------------------------------------------------------------------------------------------------------
// Created with imbVeles / imbACE framework
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------

namespace imbNLP.ToolkitTest
{
    using imbACE.Services.console;
    using imbSCI.Core.files.folders;

    using System;
    using System.Collections.Generic;
    using System.Text;

    public class imbNLPToolkitWorkspace : aceAdvancedConsoleWorkspace
    {


        /// <summary>
        /// Initializes the workspace for the imbNLPToolkitWorkspace console 
        /// </summary>
        /// <param name="console">The console using this workspace</param>
        public imbNLPToolkitWorkspace(imbNLPToolkitConsole console) : base(console)
        {

        }

        // public folderNode folderName {get; set;} // <---- uncomment if you have need for additional subdirectory in the console state project folder


        /// <summary>
        /// Gets called during workspace construction, the method should initiate any additional subdirectories that console's project uses
        /// </summary>
        /// <remarks>
        /// <example>
        /// Place inside initiation of additional directories, required for your console's project class (inheriting: <see cref="T:imbACE.Services.console.aceAdvancedConsoleStateBase" />)
        /// <code><![CDATA[
        /// folderName = folder.Add(nameof(folderName), "Caption", "Description");
        /// ]]></code></example>
        /// </remarks>
        public override void setAdditionalWorkspaceFolders()
        {
            // place here your additional directories for console's project subdirectory as follows:
            // folderName = folder.Add(nameof(folderName), "Caption", "Description");
        }
    }

}