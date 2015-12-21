using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integra.Space.Language.Analysis
{
    class TreeGraphGenerator
    {
        private const string PATH = @"Graphs\";
        private string fileName;
        private string outputFile;
        private int nodeCount;

        public TreeGraphGenerator(string fileName)
        {
            this.fileName = fileName;
            this.outputFile = PATH + fileName + ".png";
            nodeCount = 0;
        }

        public void GenerateGraph(PlanNode executionPlan)
        {
            var getStartProcessQuery = new GetStartProcessQuery();
            var getProcessStartInfoQuery = new GetProcessStartInfoQuery();
            var registerLayoutPluginCommand = new RegisterLayoutPluginCommand(getProcessStartInfoQuery, getStartProcessQuery);

            // GraphGeneration can be injected via the IGraphGeneration interface

            var wrapper = new GraphGeneration(getStartProcessQuery, getProcessStartInfoQuery, registerLayoutPluginCommand);

            string dot = this.ToDotGraph(executionPlan);
            byte[] output = wrapper.GenerateGraph(dot, Enums.GraphReturnType.Png);

            this.ByteArrayToFile(output);
        }

        public void ShowGraph()
        {
            System.Diagnostics.Process.Start(this.outputFile);
        }

        private bool ByteArrayToFile(byte[] _ByteArray)
        {
            try
            {
                // Open file for reading
                System.IO.FileStream _FileStream = new System.IO.FileStream(this.outputFile, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                // Writes a block of bytes to this stream using data from
                // a byte array.
                _FileStream.Write(_ByteArray, 0, _ByteArray.Length);

                // close file stream
                _FileStream.Close();

                return true;
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
            }

            // error occured, return false
            return false;
        }

        private string ToDotGraph(PlanNode root)
        {
            StringBuilder b = new StringBuilder();
            b.Append("digraph G { node [shape=record]; " + Environment.NewLine);
            b.Append(ToDot(root, false));
            this.nodeCount = 0;
            b.Append(ToDot(root, true));
            b.Append("}");
            return b.ToString();
        }

        private string ToDot(PlanNode node, bool second)
        {
            StringBuilder b = new StringBuilder();

            int numRoot = this.nodeCount;
            if (!second)
            {
                b.AppendFormat("{0}[label = \"<f0> {5} |<f1> {1} | {{ Line: {3} | Column: {4} }}\"]; {2}", node.NodeType.ToString() + "_" + numRoot, (node.NodeText == null) ? "" : node.NodeText.Replace("\"", "\\\""), Environment.NewLine, node.Line, node.Column, node.NodeType.ToString());
            }

            if (node.Children != null)
            {
                if (node.Children.Count >= 1)
                {
                    int numLeft = ++this.nodeCount;
                    PlanNode left = node.Children[0];
                    
                    if (second)
                    {
                        b.AppendFormat("{0}:f0 -> {1}:f0 {2}", node.NodeType.ToString() + "_" + numRoot, left.NodeType.ToString() + "_" + numLeft, Environment.NewLine);
                    }

                    b.Append(ToDot(left, second));
                }

                if (node.Children.Count >= 2)
                {
                    int numRight = ++this.nodeCount;
                    PlanNode right = node.Children[1];
                                        
                    if (second)
                    {
                        b.AppendFormat("{0}:f0 -> {1}:f0 {2}", node.NodeType.ToString() + "_" + numRoot, right.NodeType.ToString() + "_" + numRight, Environment.NewLine);
                    }

                    b.Append(ToDot(right, second));
                }
            }

            return b.ToString();
        }
    }
}
