using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.GraphViewerGdi;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms.Integration;
using MemoryVisualizer.Models;
using MessageBox = System.Windows.MessageBox;
using Color = Microsoft.Msagl.Drawing.Color;

namespace MemoryVisualizer
{
    /// <summary>
    /// Main window of the Memory Visualizer application.
    /// This application visualizes hierarchical memory structures using Microsoft Automatic Graph Layout (MSAGL).
    /// It supports both JSON and DOT file formats for data input and provides interactive graph visualization.
    /// </summary>
    /// <remarks>
    /// Key features:
    /// - Load and visualize hierarchical data from JSON files
    /// - Load and visualize graph data from DOT files
    /// - Interactive graph visualization with zoom and pan
    /// - Color-coded nodes based on their type (Company, Division, Employee, etc.)
    /// - Refresh functionality to re-layout the graph
    /// - Comprehensive error logging using NLog
    /// </remarks>
    public partial class MainWindow : Window
    {
        // Logger instance for application-wide logging
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        // The main graph object that holds all nodes and edges
        private Graph? graph;

        // Dictionary mapping node types to their display colors
        private readonly Dictionary<string, Color> nodeColors = new()
        {
            { "Company", Color.LightBlue },
            { "Division", Color.LightGreen },
            { "Employee", Color.LightYellow },
            { "Product", Color.LightPink },
            { "Revenue", Color.LightGray }
        };

        /// <summary>
        /// Initializes the main window and sets up the graph viewer control
        /// </summary>
        public MainWindow()
        {
            try
            {
                Logger.Debug("Initializing MainWindow");
                InitializeComponent();
                graph = new Graph("memory");
                Logger.Debug("MainWindow initialized successfully");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error initializing MainWindow");
                MessageBox.Show($"Error initializing application: {ex.Message}", "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles the Load JSON button click event.
        /// Opens a file dialog and loads the selected JSON file.
        /// </summary>
        private void btnLoadJson_Click(object sender, RoutedEventArgs e)
        {
            LoadGraph("json");
        }

        /// <summary>
        /// Handles the Load DOT button click event.
        /// Opens a file dialog and loads the selected DOT file.
        /// </summary>
        private void btnLoadDot_Click(object sender, RoutedEventArgs e)
        {
            LoadGraph("dot");
        }

        /// <summary>
        /// Common method for loading both JSON and DOT files.
        /// Creates a graph visualization based on the file contents.
        /// </summary>
        /// <param name="filePath">Type of file to load ("json" or "dot")</param>
        private void LoadGraph(string filePath)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = filePath == "json" ? "JSON files (*.json)|*.json|All files (*.*)|*.*" : "DOT files (*.dot)|*.dot|All files (*.*)|*.*",
                InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SampleData")
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    Logger.Debug($"Loading graph from file: {openFileDialog.FileName}");
                    string fileExtension = Path.GetExtension(openFileDialog.FileName).ToLower();

                    if (fileExtension == ".json")
                    {
                        LoadJsonGraph(openFileDialog.FileName);
                    }
                    else if (fileExtension == ".dot")
                    {
                        LoadDotGraph(openFileDialog.FileName);
                    }
                    else
                    {
                        Logger.Error($"Unsupported file extension: {fileExtension}");
                        MessageBox.Show($"Unsupported file extension: {fileExtension}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    try
                    {
                        Logger.Debug("Applying layout settings after loading graph");
                        ApplyLayoutSettings();
                        Logger.Debug("Layout applied successfully");
                    }
                    catch (Exception layoutEx)
                    {
                        Logger.Error(layoutEx, "Layout error occurred but graph was loaded");
                        // Don't rethrow - we want to show the graph even if layout has issues
                    }

                    // Update viewer even if layout had issues
                    Logger.Debug("Updating graph viewer");
                    viewer.Graph = graph;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error loading graph");
                    MessageBox.Show($"Error loading graph: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Loads and parses a JSON file containing hierarchical memory structure.
        /// Creates nodes and edges based on the JSON structure.
        /// </summary>
        /// <param name="filePath">Path to the JSON file</param>
        private void LoadJsonGraph(string filePath)
        {
            try
            {
                Logger.Info($"Loading JSON file: {filePath}");
                var jsonContent = File.ReadAllText(filePath);
                var rootNode = JsonConvert.DeserializeObject<MemoryNode>(jsonContent);
                if (rootNode != null)
                {
                    graph = new Graph("memory");
                    BuildGraphFromNodes(rootNode);
                    ApplyNodeStyles(rootNode); // Apply colors after building the graph
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error loading JSON file: {filePath}");
                MessageBox.Show($"Error loading JSON file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Loads and parses a DOT file containing graph structure.
        /// Creates nodes and edges based on the DOT format specifications.
        /// </summary>
        /// <param name="filePath">Path to the DOT file</param>
        private void LoadDotGraph(string filePath)
        {
            try
            {
                var dotContent = File.ReadAllText(filePath);
                graph = new Graph("memory");

                // Parse DOT content manually
                var lines = dotContent.Split('\n');
                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();

                    // Parse node definitions
                    if (trimmedLine.Contains("[") && !trimmedLine.Contains("->"))
                    {
                        var nodeId = trimmedLine.Split('[')[0].Trim();
                        var node = graph.AddNode(nodeId);

                        if (trimmedLine.Contains("label="))
                        {
                            var labelStart = trimmedLine.IndexOf("label=\"") + 7;
                            var labelEnd = trimmedLine.IndexOf("\"", labelStart);
                            var label = trimmedLine.Substring(labelStart, labelEnd - labelStart);
                            node.LabelText = label.Replace("\\n", "\n");
                        }

                        if (trimmedLine.Contains("fillcolor="))
                        {
                            var colorStart = trimmedLine.IndexOf("fillcolor=") + 10;
                            var colorEnd = trimmedLine.IndexOf("]", colorStart);
                            var color = trimmedLine.Substring(colorStart, colorEnd - colorStart);
                            switch (color.ToLower())
                            {
                                case "lightblue":
                                    node.Attr.FillColor = Color.LightBlue;
                                    break;
                                case "lightgreen":
                                    node.Attr.FillColor = Color.LightGreen;
                                    break;
                                case "lightyellow":
                                    node.Attr.FillColor = Color.LightYellow;
                                    break;
                                case "lightpink":
                                    node.Attr.FillColor = Color.LightPink;
                                    break;
                                case "lightgray":
                                    node.Attr.FillColor = Color.LightGray;
                                    break;
                            }
                        }
                    }

                    // Parse edge definitions
                    if (trimmedLine.Contains("->"))
                    {
                        var parts = trimmedLine.Split(new[] { "->" }, StringSplitOptions.None);
                        if (parts.Length == 2)
                        {
                            var sourceId = parts[0].Trim();
                            var targetId = parts[1].Trim().Split('[')[0].Trim();
                            graph.AddEdge(sourceId, targetId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading DOT file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Recursively builds the graph structure from a hierarchical node tree.
        /// Creates nodes and edges based on parent-child relationships.
        /// </summary>
        /// <param name="root">Root node of the hierarchy</param>
        private void BuildGraphFromNodes(MemoryNode root)
        {
            if (root == null || string.IsNullOrEmpty(root.Id))
            {
                return;
            }

            // Add the root node if it doesn't exist
            if (!graph.NodeMap.ContainsKey(root.Id))
            {
                graph.AddNode(root.Id);
            }

            // Process all children
            foreach (var child in root.Children ?? Enumerable.Empty<MemoryNode>())
            {
                if (string.IsNullOrEmpty(child.Id))
                {
                    continue;
                }

                // Add child node if it doesn't exist
                if (!graph.NodeMap.ContainsKey(child.Id))
                {
                    graph.AddNode(child.Id);
                }

                // Create edge between root and child
                var edge = graph.AddEdge(root.Id, child.Id);

                BuildGraphFromNodes(child);
            }
        }

        /// <summary>
        /// Applies visual styles (colors, tooltips) to nodes based on their type.
        /// Recursively processes the entire node hierarchy.
        /// </summary>
        /// <param name="node">Root node to start styling from</param>
        private void ApplyNodeStyles(MemoryNode node)
        {
            if (node == null || string.IsNullOrEmpty(node.Id) || graph == null)
            {
                return;
            }

            var graphNode = graph.FindNode(node.Id);
            if (graphNode != null)
            {
                // Set node color based on type
                if (!string.IsNullOrEmpty(node.Type) && nodeColors.TryGetValue(node.Type, out var color))
                {
                    graphNode.Attr.FillColor = color;
                }

                // Set tooltip with node information
                graphNode.LabelText = $"{node.Type}\n{node.Id}";
                graphNode.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Box;
            }

            // Process children recursively
            foreach (var child in node.Children ?? Enumerable.Empty<MemoryNode>())
            {
                ApplyNodeStyles(child);
            }
        }

        /// <summary>
        /// Handles the Refresh button click event.
        /// Creates a new graph with the same data and applies fresh layout.
        /// </summary>
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Debug("Refresh button clicked");
                if (graph == null)
                {
                    Logger.Debug("Graph is null during refresh, skipping");
                    return;
                }

                // Store current graph state before refresh
                Logger.Debug($"Current graph state - Nodes: {graph.NodeCount}, Edges: {graph.EdgeCount}, GeometryGraph: {(graph.GeometryGraph != null ? "exists" : "null")}");

                try
                {
                    // Create a new graph with the same data
                    var newGraph = new Graph("memory");
                    Logger.Debug("Created new graph for refresh");

                    // Copy nodes and edges from existing graph
                    foreach (var node in graph.Nodes)
                    {
                        var newNode = newGraph.AddNode(node.Id);
                        newNode.LabelText = node.LabelText;
                        newNode.Attr.FillColor = node.Attr.FillColor;
                        newNode.Attr.Shape = node.Attr.Shape;
                        Logger.Debug($"Copied node: {node.Id}");
                    }

                    foreach (var edge in graph.Edges)
                    {
                        newGraph.AddEdge(edge.Source, edge.Target);
                        Logger.Debug($"Copied edge: {edge.Source} -> {edge.Target}");
                    }

                    // Apply layout to new graph
                    Logger.Debug("Applying layout to new graph");
                    graph = newGraph;
                    ApplyLayoutSettings();

                    // Update viewer
                    Logger.Debug("Updating graph viewer");
                    if (viewer != null)
                    {
                        viewer.Graph = graph;
                        Logger.Debug("Graph viewer updated successfully");
                    }
                    else
                    {
                        Logger.Warn("Cannot update viewer - Viewer is null");
                        throw new InvalidOperationException("Viewer is null after layout");
                    }
                }
                catch (Exception refreshEx)
                {
                    Logger.Error(refreshEx, "Error during refresh");
                    throw;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error during refresh operation");
                Logger.Error($"Exception details - Type: {ex.GetType().Name}, Message: {ex.Message}");
                if (ex.StackTrace != null)
                {
                    Logger.Error($"Stack trace: {ex.StackTrace}");
                }
                if (ex.InnerException != null)
                {
                    Logger.Error($"Inner exception: {ex.InnerException.Message}");
                    Logger.Error($"Inner exception stack trace: {ex.InnerException.StackTrace}");
                }
                MessageBox.Show($"Error refreshing graph: {ex.Message}", "Refresh Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Applies layout settings to the current graph.
        /// Handles graph geometry initialization and layout algorithm application.
        /// </summary>
        private void ApplyLayoutSettings()
        {
            if (graph == null || graph.NodeCount == 0)
            {
                Logger.Debug("Skipping layout for empty graph");
                return;
            }

            try
            {
                Logger.Debug($"Applying layout settings to graph with {graph.NodeCount} nodes and {graph.EdgeCount} edges");
                
                // Initialize the geometry graph if needed
                if (graph.GeometryGraph == null)
                {
                    Logger.Debug("Initializing geometry graph");
                    try
                    {
                        graph.CreateGeometryGraph();
                        Logger.Debug("Geometry graph created successfully");
                    }
                    catch (Exception gex)
                    {
                        Logger.Error(gex, "Error creating geometry graph");
                        throw new Exception("Failed to create geometry graph", gex);
                    }
                }

                // Configure layout settings for optimal visualization
                var settings = new SugiyamaLayoutSettings
                {
                    LayerSeparation = 50,    // Vertical space between layers
                    NodeSeparation = 30,      // Horizontal space between nodes
                    AspectRatio = 1.0,        // Preferred aspect ratio of the layout
                    MinimalWidth = 50,        // Minimum width for the layout
                    EdgeRoutingSettings = 
                    {
                        EdgeRoutingMode = Microsoft.Msagl.Core.Routing.EdgeRoutingMode.Spline
                    }
                };

                // Create and run the layout
                Logger.Debug("Creating LayeredLayout");
                if (graph.GeometryGraph == null)
                {
                    throw new InvalidOperationException("GeometryGraph is null after initialization");
                }

                var layout = new LayeredLayout(graph.GeometryGraph, settings);
                
                Logger.Debug("Running layout");
                try
                {
                    layout.Run();
                    Logger.Debug("Layout applied successfully");
                }
                catch (Exception layoutEx)
                {
                    Logger.Error(layoutEx, "Layout failed");
                    throw;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error applying layout");
                Logger.Error($"Graph state - Nodes: {graph.NodeCount}, Edges: {graph.EdgeCount}, GeometryGraph: {(graph.GeometryGraph != null ? "exists" : "null")}");
                if (ex.InnerException != null)
                {
                    Logger.Error($"Inner exception: {ex.InnerException.Message}");
                    Logger.Error($"Inner exception stack trace: {ex.InnerException.StackTrace}");
                }
                throw;
            }
        }

        /// <summary>
        /// Handles window initialization and sets up size change handling.
        /// Ensures graph layout adjusts properly when window is resized.
        /// </summary>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            try
            {
                Logger.Debug("Window source initialized");
                if (viewer != null)
                {
                    viewer.SizeChanged += (s, args) =>
                    {
                        try
                        {
                            Logger.Debug("Viewer size changed");
                            if (graph != null)
                            {
                                Logger.Debug("Adjusting graph layout for new size");
                                // Store current graph state
                                var backupGraph = graph;
                                try
                                {
                                    ApplyLayoutSettings();
                                    viewer.Graph = graph;
                                    Logger.Debug("Graph layout adjusted for new size");
                                }
                                catch (Exception layoutEx)
                                {
                                    Logger.Error(layoutEx, "Error adjusting layout for new size, restoring previous state");
                                    graph = backupGraph;
                                    viewer.Graph = graph;
                                }
                            }
                            else
                            {
                                Logger.Warn("Cannot adjust layout - Graph is null");
                            }
                        }
                        catch (Exception sizeEx)
                        {
                            Logger.Error(sizeEx, "Error handling size change");
                            // Don't show message box for size change errors to avoid spam
                        }
                    };
                    Logger.Debug("Size change handler registered");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error initializing window source");
            }
        }
    }
}