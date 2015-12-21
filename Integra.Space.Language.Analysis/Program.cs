using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integra.Space.Language.Analysis
{
    class Program
    {
        static void Main(string[] args)
        {
            string finish = "";
            while (!finish.ToLower().Equals("stop"))
            {
                try
                {

                    Console.WriteLine("Creating graph directory...");
                    CreateDirectory();
                    Console.WriteLine("Write a EQL command or press enter for a test");
                    Console.WriteLine();
                    string eql = Console.ReadLine();
                    Console.WriteLine("Creating execution plan...");

                    if (string.IsNullOrWhiteSpace(eql))
                    {
                        eql = string.Format("from {0} where {1} select {2} as CampoNulo",
                                                                            "SpaceObservable1",
                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                            "@event.Message.#0.[\"Campo que no existe\"]");
                    }

                    EQLPublicParser parser = new EQLPublicParser(eql);
                    List<PlanNode> plan = parser.Parse();

                    Console.WriteLine("Plan generated.");
                    Console.WriteLine("Creating graph...");

                    string fileName = DateTime.Now.ToString("yyyy_MM_dd hh_mm_ss");
                    TreeGraphGenerator tgg = new TreeGraphGenerator(fileName);
                    tgg.GenerateGraph(plan.First());

                    Console.WriteLine("Graph created.");
                    Console.WriteLine("Opening graph...");
                    tgg.ShowGraph();
                    Console.WriteLine("Graph opened.");
                    Console.WriteLine("Write 'stop' to finish or enter to continue...");
                    finish = Console.ReadLine();
                    Console.WriteLine();
                }
                catch (Exception e)
                {
                    // Get stack trace for the exception with source file information
                    StackTrace st = new StackTrace(e, true);
                    // Get the top stack frame
                    StackFrame frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    int line = frame.GetFileLineNumber();
                    int column = frame.GetFileColumnNumber();

                    Console.WriteLine("Cannot create the graph. Error: Line: {0}, Column: {1}, Message {2}", line, column, e.Message);
                    Console.WriteLine("Write 'stop' to finish or enter to continue...");
                    finish = Console.ReadLine();
                    Console.WriteLine();
                }
            }
        }

        public static void CreateDirectory()
        {
            // Specify the directory you want to manipulate.
            string path = @"Graphs";

            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(path))
                {
                    Console.WriteLine("That path exists already.");
                    return;
                }

                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path);
                Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path));

                // Delete the directory.
                di.Delete();
                Console.WriteLine("The directory was deleted successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }
    }
}
