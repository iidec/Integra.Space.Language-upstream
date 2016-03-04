using Integra.Space.Language.Analysis.Metadata.MetadataNodes;
using Integra.Space.Language.Runtime;
using Irony.Parsing;
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

            Console.WriteLine("[1] for graph, [2] for generate metadata");
            string option = Console.ReadLine();
            //option = "2";

            if (option.Equals("1"))
            {
                while (!finish.ToLower().Equals("stop"))
                {
                    try
                    {
                        Console.WriteLine("Creating graph directory...");
                        CreateDirectory();
                        Console.WriteLine("Write a EQL command or press enter");
                        Console.WriteLine();
                        string eql = Console.ReadLine();
                        Console.WriteLine("Creating execution plan...");

                        if (string.IsNullOrWhiteSpace(eql))
                        {
                            
                            eql = string.Format("from {0} where {1} apply window of {2} group by {3} select top 1 {4} as Llave, {5} as Sumatoria order by asc Sumatoria, Llave",
                                                                                            "SpaceObservable1",
                                                                                            "SpaceObservable1.@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#1.CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)");
                        
                        /*
                            eql = "LEFT JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#0.#0 == \"0100\" " +
                                    "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#0.#0 == \"0110\" " +
                                    "ON t1.@event.Adapter.Name == t2.@event.Adapter.Name and (decimal)t1.@event.Message.#1.#4 == (decimal)t2.@event.Message.#1.#4 and right((string)t1.@event.Message.#1.#43, 5) == right((string)t2.@event.Message.#1.#43, 5)" +
                                    "TIMEOUT '00:00:01' " +
                                    "EVENTLIFETIME '00:00:10' " +
                                    "WHERE  t1.@event.Message.#0.#0 == \"0100\" " +
                                    "SELECT t1.@event.Message.#1.#43 as c1 ";
                          */  
                        }

                        EQLPublicParser parser = new EQLPublicParser(eql);
                        List<PlanNode> plan = parser.Evaluate();

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
            else if (option.Equals("2"))
            {
                while (!finish.ToLower().Equals("stop"))
                {
                    Console.WriteLine("Write a EQL command or press enter");
                    Console.WriteLine();
                    string eql = Console.ReadLine();
                    Console.WriteLine("Creating execution plan...");

                    if (string.IsNullOrWhiteSpace(eql))
                    {
                        
                        eql = string.Format("from {0} where {1} apply window of {2} group by {3} select top 1 {4} as Llave, {5} as Sumatoria order by asc Sumatoria",
                                                                                            "SpaceObservable1",
                                                                                            "SpaceObservable1.@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#1.CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)");
                        
                        
                        eql = "LEFT JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#0.#0 == \"0100\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#0.#0 == \"0110\" " +
                                "ON t1.@event.Adapter.Name == t2.@event.Adapter.Name and (decimal)t1.@event.Message.#1.#4 == (decimal)t2.@event.Message.#1.#4 and right((string)t1.@event.Message.#1.#43, 5) == right((string)t2.@event.Message.#1.#43, 5)" +
                                "TIMEOUT '00:00:01' " +
                                "EVENTLIFETIME '00:00:10' " +
                                "WHERE  t1.@event.Message.#0.#0 == \"0100\" " +
                                "SELECT t1.@event.Message.#1.#43 as c1 ";

                        /*eql = string.Format("from {0} apply window of {2} select {3} as monto",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "(decimal)@event.Message.#1.#4");
                                                                                            */
                    }

                    EQLPublicParser parser = new EQLPublicParser(eql);
                    parser.Parse();

                    Console.WriteLine("Plan generated.");
                    Console.WriteLine("Creating metadata...");

                    Integra.Space.Language.Metadata.MetadataGenerator mg = new Integra.Space.Language.Metadata.MetadataGenerator();
                    SpaceParseTreeNode spaceParseTreeNode = mg.ConvertIronyParseTree(parser.ParseTree.Root);
                    Integra.Space.Language.Metadata.SpaceMetadataTreeNode metadataRootNode = mg.GenerateMetadata(spaceParseTreeNode);

                    Console.WriteLine("Metadata created.");
                    Console.WriteLine("Transforming plan...");

                    PlanNode executionPlanNode = parser.Evaluate().First();
                    /*
                    TreeTransformations tf = new TreeTransformations(executionPlanNode);
                    tf.Transform();
                    */
                    Console.WriteLine("Plan transformed.");
                    Console.WriteLine("Creating graph...");

                    string fileName = DateTime.Now.ToString("yyyy_MM_dd hh_mm_ss");
                    TreeGraphGenerator tgg = new TreeGraphGenerator(fileName);
                    tgg.GenerateGraph(executionPlanNode);

                    Console.WriteLine("Graph created.");
                    Console.WriteLine("Opening graph...");
                    tgg.ShowGraph();
                    Console.WriteLine("Graph opened.");
                    Console.WriteLine("Write 'stop' to finish or enter to continue...");
                    finish = Console.ReadLine();
                    Console.WriteLine();

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
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }
    }
}
