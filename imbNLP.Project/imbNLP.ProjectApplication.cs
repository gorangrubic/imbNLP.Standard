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

namespace imbNLP.Project
{
    using imbACE.Core.application;
    using imbACE.Services.application;
    using imbNLP.Project.console;

    public class ConsoleApplication : aceConsoleApplication<CommandConsole>
    {
        public static void Main(string[] args)
        {
            var application = new ConsoleApplication();

            application.StartApplication(args);

            // here you may place code that has to be performed after user called application closing
            // ...
        }

        public override void setAboutInformation()
        {
            // Insert proper information here
            appAboutInfo = new aceApplicationInfo(); // parametarless constructor takes values from Assembly attributes
            appAboutInfo.license = "Licensed under GNU General Public License v3.0";
            appAboutInfo.welcomeMessage = "imbACE Console Tool";
        }
    }
}