namespace imbNLP.ToolkitTest
{
    public class Program
    {
        static public imbNLPToolkitConsole nlpConsole { get; set; }

        static public imbNLPToolkitApp nlpApp { get; set; }

        static void Main(string[] args)
        {

            nlpApp = new imbNLPToolkitApp();

            nlpApp.StartApplication(args);

            
        }
    }
}
