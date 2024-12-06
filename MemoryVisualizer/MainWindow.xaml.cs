using System;
using System.Windows;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System.Windows.Forms.Integration;
using MemoryVisualizer.Models;
using System.Collections.Generic;
using System.Linq;

namespace MemoryVisualizer
{
    public partial class MainWindow : Window
    {
        private GViewer? viewer;
        private Graph? graph;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGraphViewer();
            cboLayout.SelectedIndex = 0;
            UpdateGraph();
        }

        private void InitializeGraphViewer()
        {
            viewer = new GViewer();
            viewer.LayoutAlgorithmSettingsButtonVisible = false;
            viewer.SaveButtonVisible = false;
            viewer.SaveGraphButtonVisible = false;
            viewer.UndoRedoButtonsVisible = false;
            windowsFormsHost.Child = viewer;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateGraph();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Export functionality will be implemented soon!");
        }

        private void UpdateGraph()
        {
            graph = new Graph("memory");

            var sampleData = CreateSampleData();
            BuildGraphFromNodes(sampleData);

            if (viewer != null && graph != null)
            {
                // Customize graph appearance
                foreach (var node in graph.Nodes)
                {
                    var memNode = FindMemoryNode(sampleData, node.Id);
                    if (memNode != null)
                    {
                        // Set node color based on type
                        switch (memNode.Type)
                        {
                            case "Company":
                                node.Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightBlue;
                                break;
                            case "Division":
                                node.Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightGreen;
                                break;
                            case "Employee":
                                node.Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightYellow;
                                break;
                            case "Product":
                                node.Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightPink;
                                break;
                            case "Revenue":
                                node.Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightGray;
                                break;
                        }

                        // Set tooltip
                        if (!string.IsNullOrEmpty(memNode.ToolTip))
                        {
                            node.LabelText = $"{memNode.Label}\\n{memNode.ToolTip}";
                        }
                    }
                }

                viewer.Graph = graph;
            }
        }

        private MemoryNode? FindMemoryNode(MemoryNode root, string id)
        {
            if (root.Id == id) return root;
            foreach (var child in root.Children)
            {
                var found = FindMemoryNode(child, id);
                if (found != null) return found;
            }
            return null;
        }

        private MemoryNode CreateSampleData()
        {
            // Create company
            var company = new MemoryNode
            {
                Label = "TechSales Corp",
                Type = "Company",
                ToolTip = "Annual Revenue: $50M"
            };

            // Create divisions
            var divisions = new[]
            {
                CreateDivision("Hardware Division", 20000000),
                CreateDivision("Software Division", 30000000)
            };

            foreach (var division in divisions)
            {
                company.AddChild(division);
            }

            // Add employees to Hardware Division
            var hwEmployees = new[]
            {
                CreateEmployee("John Smith", "Senior Sales", 1500000),
                CreateEmployee("Alice Johnson", "Regional Manager", 2000000),
                CreateEmployee("Bob Wilson", "Account Executive", 1200000)
            };

            foreach (var emp in hwEmployees)
            {
                divisions[0].AddChild(emp);
                AddProductsToEmployee(emp, true);
            }

            // Add employees to Software Division
            var swEmployees = new[]
            {
                CreateEmployee("Sarah Davis", "Sales Director", 2500000),
                CreateEmployee("Mike Brown", "Solution Architect", 1800000),
                CreateEmployee("Emma White", "Technical Sales", 1600000)
            };

            foreach (var emp in swEmployees)
            {
                divisions[1].AddChild(emp);
                AddProductsToEmployee(emp, false);
            }

            return company;
        }

        private MemoryNode CreateDivision(string name, decimal revenue)
        {
            return new MemoryNode
            {
                Label = name,
                Type = "Division",
                ToolTip = $"Revenue: ${revenue:N0}"
            };
        }

        private MemoryNode CreateEmployee(string name, string title, decimal totalRevenue)
        {
            return new MemoryNode
            {
                Label = name,
                Type = "Employee",
                ToolTip = $"{title}\\nTotal Revenue: ${totalRevenue:N0}"
            };
        }

        private void AddProductsToEmployee(MemoryNode employee, bool isHardware)
        {
            var random = new Random();
            var products = isHardware
                ? new[] { "Laptops", "Servers", "Networking", "Storage" }
                : new[] { "Cloud Services", "Security", "Analytics", "AI Solutions" };

            foreach (var product in products)
            {
                var revenue = random.Next(200000, 800000);
                var productNode = new MemoryNode
                {
                    Label = product,
                    Type = "Product",
                    ToolTip = $"Revenue: ${revenue:N0}"
                };
                employee.AddChild(productNode);
            }
        }

        private void BuildGraphFromNodes(MemoryNode root)
        {
            if (root == null || graph == null) return;

            // Create node for root if it doesn't exist
            if (graph.FindNode(root.Id) == null)
            {
                var node = graph.AddNode(root.Id);
                node.LabelText = root.Label;
            }

            foreach (var child in root.Children)
            {
                // Create node for child if it doesn't exist
                if (graph.FindNode(child.Id) == null)
                {
                    var node = graph.AddNode(child.Id);
                    node.LabelText = child.Label;
                }

                // Create edge between root and child
                var edge = graph.AddEdge(root.Id, child.Id);
                
                BuildGraphFromNodes(child);
            }
        }
    }
}