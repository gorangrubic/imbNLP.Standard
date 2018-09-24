namespace imbNLP.ToolkitTest
{

    public class imbNLPToolkitApp : imbACE.Services.application.aceConsoleApplication<imbNLPToolkitConsole>
    {
        public imbNLPToolkitApp()
        {

        }

        public override void setAboutInformation()
        {
            appAboutInfo = new imbACE.Core.application.aceApplicationInfo()
            {
                software = "NLP Toolkit",
                version = "1.0"
            };
        }
    }

}