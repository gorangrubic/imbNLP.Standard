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
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbNLP.Project.Dataset;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class imbNLPToolkitConsole : aceAdvancedConsole<imbNLPToolkitState, imbNLPToolkitWorkspace>
    {
        public override string consoleTitle { get { return "imbNLPToolkitConsole Console"; } }

        public imbNLPToolkitConsole() : base()
        {


        }






        [Display(GroupName = "run", Name = "LoadWebKB", ShortName = "", Description = "Loads WebKB web site datasets")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will load WebKB 7Sectors dataset")]
        /// <summary>Loads WebKB web site datasets</summary> 
        /// <remarks><para>It will load WebKB 7Sectors dataset</para></remarks>
        /// <param name="path">Path to 7Secotrs dataset</param>
        /// <param name="steps">--</param>
        /// <param name="debug">--</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_runLoadWebKB(
              [Description("Path to 7Secotrs dataset")] String path = "G:\\_DOKTORAT_MAIN\\SM03_Datasets\\7sectors")
        {

            WebKBDatasetAdapter webKBDatasetAdapter = new WebKBDatasetAdapter();
            var dataset = webKBDatasetAdapter.LoadDataset(path, response);

        }



        /// <summary>
        /// Gets the workspace.
        /// </summary>
        /// <value>
        /// The workspace.
        /// </value>
        public override imbNLPToolkitWorkspace workspace
        {
            get
            {
                if (_workspace == null)
                {
                    _workspace = new imbNLPToolkitWorkspace(this);
                }
                return _workspace;
            }
        }

        public override void onStartUp()
        {
            //
            base.onStartUp();
        }

        protected override void doCustomSpecialCall(aceCommandActiveInput input)
        {

        }
    }

}